/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2016 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Diagnostics;

using KeePass.App.Configuration;
using KeePass.Forms;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.Util.Spr
{
	/// <summary>
	/// String placeholders and field reference replacement engine.
	/// </summary>
	public static partial class SprEngine
	{
		private const uint MaxRecursionDepth = 12;
		private const StringComparison ScMethod = StringComparison.OrdinalIgnoreCase;

		private static string m_strAppExePath = string.Empty;
		// private static readonly char[] m_vPlhEscapes = new char[] { '{', '}', '%' };

		// Important notes for plugin developers subscribing to the following events:
		// * If possible, prefer subscribing to FilterCompile instead of
		//   FilterCompilePre.
		// * If your plugin provides an active transformation (e.g. replacing a
		//   placeholder that changes some state or requires UI interaction), you
		//   must only perform the transformation if the ExtActive bit is set in
		//   args.Context.Flags of the event arguments object args provided to the
		//   event handler.
		// * Non-active transformations should only be performed if the ExtNonActive
		//   bit is set in args.Context.Flags.
		// * If your plugin provides a placeholder (like e.g. {EXAMPLE}), you
		//   should add this placeholder to the FilterPlaceholderHints list
		//   (e.g. add the string "{EXAMPLE}"). Please remove your strings from
		//   the list when your plugin is terminated.
		public static event EventHandler<SprEventArgs> FilterCompilePre;
		public static event EventHandler<SprEventArgs> FilterCompile;

		private static List<string> m_lFilterPlh = new List<string>();
		// See the events above
		public static List<string> FilterPlaceholderHints
		{
			get { return m_lFilterPlh; }
		}

		private static void InitializeStatic()
		{
			m_strAppExePath = WinUtil.GetExecutable();
		}

		[Obsolete]
		public static string Compile(string strText, bool bIsAutoTypeSequence,
			PwEntry pwEntry, PwDatabase pwDatabase, bool bEscapeForAutoType,
			bool bEscapeQuotesForCommandLine)
		{
			SprContext ctx = new SprContext(pwEntry, pwDatabase, SprCompileFlags.All,
				bEscapeForAutoType, bEscapeQuotesForCommandLine);
			return Compile(strText, ctx);
		}

		public static string Compile(string strText, SprContext ctx)
		{
			if(strText == null) { Debug.Assert(false); return string.Empty; }
			if(strText.Length == 0) return string.Empty;

			SprEngine.InitializeStatic();

			if(ctx == null) ctx = new SprContext();
			ctx.RefsCache.Clear();

			string str = SprEngine.CompileInternal(strText, ctx, 0);

			// if(bEscapeForAutoType && !bIsAutoTypeSequence)
			//	str = SprEncoding.MakeAutoTypeSequence(str);

			return str;
		}

		private static string CompileInternal(string strText, SprContext ctx,
			uint uRecursionLevel)
		{
			if(strText == null) { Debug.Assert(false); return string.Empty; }
			if(ctx == null) { Debug.Assert(false); ctx = new SprContext(); }

			if(uRecursionLevel >= SprEngine.MaxRecursionDepth)
			{
				Debug.Assert(false); // Most likely a recursive reference
				return string.Empty; // Do not return strText (endless loop)
			}

			string str = strText;
			MainForm mf = Program.MainForm;

			bool bExt = ((ctx.Flags & (SprCompileFlags.ExtActive |
				SprCompileFlags.ExtNonActive)) != SprCompileFlags.None);
			if(bExt && (SprEngine.FilterCompilePre != null))
			{
				SprEventArgs args = new SprEventArgs(str, ctx.Clone());
				SprEngine.FilterCompilePre(null, args);
				str = args.Text;
			}

			if((ctx.Flags & SprCompileFlags.Comments) != SprCompileFlags.None)
				str = RemoveComments(str);

			if((ctx.Flags & SprCompileFlags.TextTransforms) != SprCompileFlags.None)
				str = PerformTextTransforms(str, ctx, uRecursionLevel);

			if((ctx.Flags & SprCompileFlags.AppPaths) != SprCompileFlags.None)
				str = AppLocator.FillPlaceholders(str, ctx);

			if(ctx.Entry != null)
			{
				if((ctx.Flags & SprCompileFlags.PickChars) != SprCompileFlags.None)
					str = ReplacePickPw(str, ctx, uRecursionLevel);

				if((ctx.Flags & SprCompileFlags.EntryStrings) != SprCompileFlags.None)
					str = FillEntryStrings(str, ctx, uRecursionLevel);

				if((ctx.Flags & SprCompileFlags.EntryStringsSpecial) != SprCompileFlags.None)
				{
					// ctx.UrlRemoveSchemeOnce = true;
					// str = SprEngine.FillIfExists(str, @"{URL:RMVSCM}",
					//	ctx.Entry.Strings.GetSafe(PwDefs.UrlField), ctx, uRecursionLevel);
					// Debug.Assert(!ctx.UrlRemoveSchemeOnce);

					str = FillEntryStringsSpecial(str, ctx, uRecursionLevel);
				}

				if(((ctx.Flags & SprCompileFlags.PasswordEnc) != SprCompileFlags.None) &&
					(str.IndexOf(@"{PASSWORD_ENC}", SprEngine.ScMethod) >= 0))
				{
					string strPwCmp = SprEngine.FillIfExists(@"{PASSWORD}",
						@"{PASSWORD}", ctx.Entry.Strings.GetSafe(PwDefs.PasswordField),
						ctx.WithoutContentTransformations(), uRecursionLevel);

					str = SprEngine.FillPlaceholder(str, @"{PASSWORD_ENC}",
						StrUtil.EncryptString(strPwCmp), ctx);
				}

				PwGroup pg = ctx.Entry.ParentGroup;
				if(((ctx.Flags & SprCompileFlags.Group) != SprCompileFlags.None) &&
					(pg != null))
					str = FillGroupPlh(str, @"{GROUP", pg, ctx, uRecursionLevel);
			}

			if((ctx.Flags & SprCompileFlags.Paths) != SprCompileFlags.None)
			{
				if(mf != null)
				{
					PwGroup pgSel = mf.GetSelectedGroup();
					if(pgSel != null)
						str = FillGroupPlh(str, @"{GROUP_SEL", pgSel, ctx, uRecursionLevel);
				}

				str = SprEngine.FillIfExists(str, @"{APPDIR}", new ProtectedString(
					false, UrlUtil.GetFileDirectory(m_strAppExePath, false, false)),
					ctx, uRecursionLevel);
			}

			if(ctx.Database != null)
			{
				if((ctx.Flags & SprCompileFlags.Paths) != SprCompileFlags.None)
				{
					// For backward compatibility only
					str = SprEngine.FillIfExists(str, @"{DOCDIR}", new ProtectedString(
						false, UrlUtil.GetFileDirectory(ctx.Database.IOConnectionInfo.Path,
						false, false)), ctx, uRecursionLevel);

					str = SprEngine.FillIfExists(str, @"{DB_PATH}", new ProtectedString(
						false, ctx.Database.IOConnectionInfo.Path), ctx, uRecursionLevel);
					str = SprEngine.FillIfExists(str, @"{DB_DIR}", new ProtectedString(
						false, UrlUtil.GetFileDirectory(ctx.Database.IOConnectionInfo.Path,
						false, false)), ctx, uRecursionLevel);
					str = SprEngine.FillIfExists(str, @"{DB_NAME}", new ProtectedString(
						false, UrlUtil.GetFileName(ctx.Database.IOConnectionInfo.Path)),
						ctx, uRecursionLevel);
					str = SprEngine.FillIfExists(str, @"{DB_BASENAME}", new ProtectedString(
						false, UrlUtil.StripExtension(UrlUtil.GetFileName(
						ctx.Database.IOConnectionInfo.Path))), ctx, uRecursionLevel);
					str = SprEngine.FillIfExists(str, @"{DB_EXT}", new ProtectedString(
						false, UrlUtil.GetExtension(ctx.Database.IOConnectionInfo.Path)),
						ctx, uRecursionLevel);
				}
			}

			if((ctx.Flags & SprCompileFlags.Paths) != SprCompileFlags.None)
			{
				str = SprEngine.FillIfExists(str, @"{ENV_DIRSEP}", new ProtectedString(
					false, Path.DirectorySeparatorChar.ToString()), ctx, uRecursionLevel);

				string strPF86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				if(string.IsNullOrEmpty(strPF86))
					strPF86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				if(strPF86 != null)
					str = SprEngine.FillIfExists(str, @"{ENV_PROGRAMFILES_X86}",
						new ProtectedString(false, strPF86), ctx, uRecursionLevel);
				else { Debug.Assert(false); }
			}

			if((ctx.Flags & SprCompileFlags.AutoType) != SprCompileFlags.None)
			{
				// Use Bksp instead of Del (in order to avoid Ctrl+Alt+Del);
				// https://sourceforge.net/p/keepass/discussion/329220/thread/4f1aa6b8/
				str = StrUtil.ReplaceCaseInsensitive(str, @"{CLEARFIELD}",
					@"{HOME}+({END}){BKSP}{DELAY 50}");
			}

			if((ctx.Flags & SprCompileFlags.DateTime) != SprCompileFlags.None)
			{
				DateTime dtNow = DateTime.Now; // Local time
				str = SprEngine.FillIfExists(str, @"{DT_YEAR}", new ProtectedString(
					false, dtNow.Year.ToString("D4")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_MONTH}", new ProtectedString(
					false, dtNow.Month.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_DAY}", new ProtectedString(
					false, dtNow.Day.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_HOUR}", new ProtectedString(
					false, dtNow.Hour.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_MINUTE}", new ProtectedString(
					false, dtNow.Minute.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_SECOND}", new ProtectedString(
					false, dtNow.Second.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_SIMPLE}", new ProtectedString(
					false, dtNow.ToString("yyyyMMddHHmmss")), ctx, uRecursionLevel);

				dtNow = dtNow.ToUniversalTime();
				str = SprEngine.FillIfExists(str, @"{DT_UTC_YEAR}", new ProtectedString(
					false, dtNow.Year.ToString("D4")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_MONTH}", new ProtectedString(
					false, dtNow.Month.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_DAY}", new ProtectedString(
					false, dtNow.Day.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_HOUR}", new ProtectedString(
					false, dtNow.Hour.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_MINUTE}", new ProtectedString(
					false, dtNow.Minute.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_SECOND}", new ProtectedString(
					false, dtNow.Second.ToString("D2")), ctx, uRecursionLevel);
				str = SprEngine.FillIfExists(str, @"{DT_UTC_SIMPLE}", new ProtectedString(
					false, dtNow.ToString("yyyyMMddHHmmss")), ctx, uRecursionLevel);
			}

			if((ctx.Flags & SprCompileFlags.References) != SprCompileFlags.None)
				str = SprEngine.FillRefPlaceholders(str, ctx, uRecursionLevel);

			if(((ctx.Flags & SprCompileFlags.EnvVars) != SprCompileFlags.None) &&
				(str.IndexOf('%') >= 0))
			{
				// Replace environment variables
				foreach(DictionaryEntry de in Environment.GetEnvironmentVariables())
				{
					string strKey = (de.Key as string);
					string strValue = (de.Value as string);

					if((strKey != null) && (strValue != null))
						str = SprEngine.FillIfExists(str, @"%" + strKey + @"%",
							new ProtectedString(false, strValue), ctx, uRecursionLevel);
					else { Debug.Assert(false); }
				}
			}

			if((ctx.Flags & SprCompileFlags.Env) != SprCompileFlags.None)
				str = FillUriSpecial(str, ctx, @"{BASE", (ctx.Base ?? string.Empty),
					ctx.BaseIsEncoded, uRecursionLevel);

			str = EntryUtil.FillPlaceholders(str, ctx, uRecursionLevel);

			if((ctx.Flags & SprCompileFlags.PickChars) != SprCompileFlags.None)
				str = ReplacePickChars(str, ctx, uRecursionLevel);

			if(bExt && (SprEngine.FilterCompile != null))
			{
				SprEventArgs args = new SprEventArgs(str, ctx.Clone());
				SprEngine.FilterCompile(null, args);
				str = args.Text;
			}

			if(ctx.EncodeAsAutoTypeSequence)
			{
				str = StrUtil.NormalizeNewLines(str, false);
				str = str.Replace("\n", @"{ENTER}");
			}

			return str;
		}

		private static string FillIfExists(string strData, string strPlaceholder,
			ProtectedString psParsable, SprContext ctx, uint uRecursionLevel)
		{
			// // The UrlRemoveSchemeOnce property of ctx must be cleared
			// // before this method returns and before any recursive call
			// bool bRemoveScheme = false;
			// if(ctx != null)
			// {
			//	bRemoveScheme = ctx.UrlRemoveSchemeOnce;
			//	ctx.UrlRemoveSchemeOnce = false;
			// }

			if(strData == null) { Debug.Assert(false); return string.Empty; }
			if(strPlaceholder == null) { Debug.Assert(false); return strData; }
			if(strPlaceholder.Length == 0) { Debug.Assert(false); return strData; }
			if(psParsable == null) { Debug.Assert(false); return strData; }

			if(strData.IndexOf(strPlaceholder, SprEngine.ScMethod) >= 0)
			{
				string strReplacement = SprEngine.CompileInternal(
					psParsable.ReadString(), ctx.WithoutContentTransformations(),
					uRecursionLevel + 1);

				// if(bRemoveScheme)
				//	strReplacement = UrlUtil.RemoveScheme(strReplacement);

				return SprEngine.FillPlaceholder(strData, strPlaceholder,
					strReplacement, ctx);
			}

			return strData;
		}

		private static string FillPlaceholder(string strData, string strPlaceholder,
			string strReplaceWith, SprContext ctx)
		{
			if(strData == null) { Debug.Assert(false); return string.Empty; }
			if(strPlaceholder == null) { Debug.Assert(false); return strData; }
			if(strPlaceholder.Length == 0) { Debug.Assert(false); return strData; }
			if(strReplaceWith == null) { Debug.Assert(false); return strData; }

			return StrUtil.ReplaceCaseInsensitive(strData, strPlaceholder,
				SprEngine.TransformContent(strReplaceWith, ctx));
		}

		public static string TransformContent(string strContent, SprContext ctx)
		{
			if(strContent == null) { Debug.Assert(false); return string.Empty; }

			string str = strContent;

			if(ctx != null)
			{
				if(ctx.EncodeQuotesForCommandLine)
					str = SprEncoding.MakeCommandQuotes(str);

				if(ctx.EncodeAsAutoTypeSequence)
					str = SprEncoding.MakeAutoTypeSequence(str);
			}

			return str;
		}

		private static string FillEntryStrings(string str, SprContext ctx,
			uint uRecursionLevel)
		{
			List<string> vKeys = ctx.Entry.Strings.GetKeys();

			// Ensure that all standard field names are in the list
			// (this is required in order to replace the standard placeholders
			// even if the corresponding standard field isn't present in
			// the entry)
			List<string> vStdNames = PwDefs.GetStandardFields();
			foreach(string strStdField in vStdNames)
			{
				if(!vKeys.Contains(strStdField)) vKeys.Add(strStdField);
			}

			// Do not directly enumerate the strings in ctx.Entry.Strings,
			// because strings might change during the Spr compilation
			foreach(string strField in vKeys)
			{
				string strKey = (PwDefs.IsStandardField(strField) ?
					(@"{" + strField + @"}") :
					(@"{" + PwDefs.AutoTypeStringPrefix + strField + @"}"));

				if(!ctx.ForcePlainTextPasswords && strKey.Equals(@"{" +
					PwDefs.PasswordField + @"}", StrUtil.CaseIgnoreCmp) &&
					Program.Config.MainWindow.IsColumnHidden(AceColumnType.Password))
				{
					str = SprEngine.FillIfExists(str, strKey, new ProtectedString(
						false, PwDefs.HiddenPassword), ctx, uRecursionLevel);
					continue;
				}

				// Use GetSafe because the field doesn't necessarily exist
				// (might be a standard field that has been added above)
				str = SprEngine.FillIfExists(str, strKey, ctx.Entry.Strings.GetSafe(
					strField), ctx, uRecursionLevel);
			}

			return str;
		}

		private static string FillEntryStringsSpecial(string str, SprContext ctx,
			uint uRecursionLevel)
		{
			return FillUriSpecial(str, ctx, @"{URL", ctx.Entry.Strings.ReadSafe(
				PwDefs.UrlField), false, uRecursionLevel);
		}

		private static string FillUriSpecial(string strText, SprContext ctx,
			string strPlhInit, string strData, bool bDataIsEncoded,
			uint uRecursionLevel)
		{
			Debug.Assert(strPlhInit.StartsWith(@"{") && !strPlhInit.EndsWith(@"}"));
			Debug.Assert(strData != null);

			string[] vPlhs = new string[] {
				strPlhInit + @"}",
				strPlhInit + @":RMVSCM}",
				strPlhInit + @":SCM}",
				strPlhInit + @":HOST}",
				strPlhInit + @":PORT}",
				strPlhInit + @":PATH}",
				strPlhInit + @":QUERY}",
				strPlhInit + @":USERINFO}",
				strPlhInit + @":USERNAME}",
				strPlhInit + @":PASSWORD}"
			};

			string str = strText;
			string strDataCmp = null;
			Uri uri = null;
			for(int i = 0; i < vPlhs.Length; ++i)
			{
				string strPlh = vPlhs[i];
				if(str.IndexOf(strPlh, SprEngine.ScMethod) < 0) continue;

				if(strDataCmp == null)
				{
					SprContext ctxData = (bDataIsEncoded ?
						ctx.WithoutContentTransformations() : ctx);
					strDataCmp = SprEngine.CompileInternal(strData, ctxData,
						uRecursionLevel + 1);
				}

				string strRep = null;
				if(i == 0) strRep = strDataCmp;
				else if(i == 1) strRep = UrlUtil.RemoveScheme(strDataCmp);
				else
				{
					try
					{
						if(uri == null) uri = new Uri(strDataCmp);

						int t;
						switch(i)
						{
							case 2: strRep = uri.Scheme; break;
							case 3: strRep = uri.Host; break;
							case 4:
								strRep = uri.Port.ToString(
									NumberFormatInfo.InvariantInfo);
								break;
							case 5: strRep = uri.AbsolutePath; break;
							case 6: strRep = uri.Query; break;
							case 7: strRep = uri.UserInfo; break;
							case 8:
								strRep = uri.UserInfo;
								t = strRep.IndexOf(':');
								if(t >= 0) strRep = strRep.Substring(0, t);
								break;
							case 9:
								strRep = uri.UserInfo;
								t = strRep.IndexOf(':');
								if(t < 0) strRep = string.Empty;
								else strRep = strRep.Substring(t + 1);
								break;
							default: Debug.Assert(false); break;
						}
					}
					catch(Exception) { } // Invalid URI
				}
				if(strRep == null) strRep = string.Empty; // No assert

				str = StrUtil.ReplaceCaseInsensitive(str, strPlh, strRep);
			}

			return str;
		}

		private const string StrRemStart = @"{C:";
		private const string StrRemEnd = @"}";
		private static string RemoveComments(string strSeq)
		{
			string str = strSeq;

			while(true)
			{
				int iStart = str.IndexOf(StrRemStart, SprEngine.ScMethod);
				if(iStart < 0) break;
				int iEnd = str.IndexOf(StrRemEnd, iStart + 1, SprEngine.ScMethod);
				if(iEnd <= iStart) break;

				str = (str.Substring(0, iStart) + str.Substring(iEnd + StrRemEnd.Length));
			}

			return str;
		}

		internal const string StrRefStart = @"{REF:";
		internal const string StrRefEnd = @"}";
		private static string FillRefPlaceholders(string strSeq, SprContext ctx,
			uint uRecursionLevel)
		{
			if(ctx.Database == null) return strSeq;

			string str = strSeq;

			int nOffset = 0;
			for(int iLoop = 0; iLoop < 20; ++iLoop)
			{
				str = SprEngine.FillRefsUsingCache(str, ctx);

				int nStart = str.IndexOf(StrRefStart, nOffset, SprEngine.ScMethod);
				if(nStart < 0) break;
				int nEnd = str.IndexOf(StrRefEnd, nStart + 1, SprEngine.ScMethod);
				if(nEnd <= nStart) break;

				string strFullRef = str.Substring(nStart, nEnd - nStart + 1);
				char chScan, chWanted;
				PwEntry peFound = FindRefTarget(strFullRef, ctx, out chScan, out chWanted);

				if(peFound != null)
				{
					string strInsData;
					if(chWanted == 'T')
						strInsData = peFound.Strings.ReadSafe(PwDefs.TitleField);
					else if(chWanted == 'U')
						strInsData = peFound.Strings.ReadSafe(PwDefs.UserNameField);
					else if(chWanted == 'A')
						strInsData = peFound.Strings.ReadSafe(PwDefs.UrlField);
					else if(chWanted == 'P')
						strInsData = peFound.Strings.ReadSafe(PwDefs.PasswordField);
					else if(chWanted == 'N')
						strInsData = peFound.Strings.ReadSafe(PwDefs.NotesField);
					else if(chWanted == 'I')
						strInsData = peFound.Uuid.ToHexString();
					else { nOffset = nStart + 1; continue; }

					if((chWanted == 'P') && !ctx.ForcePlainTextPasswords &&
						Program.Config.MainWindow.IsColumnHidden(AceColumnType.Password))
						strInsData = PwDefs.HiddenPassword;

					SprContext sprSub = ctx.WithoutContentTransformations();
					sprSub.Entry = peFound;

					string strInnerContent = SprEngine.CompileInternal(strInsData,
						sprSub, uRecursionLevel + 1);
					strInnerContent = SprEngine.TransformContent(strInnerContent, ctx);

					// str = str.Substring(0, nStart) + strInnerContent + str.Substring(nEnd + 1);
					SprEngine.AddRefToCache(strFullRef, strInnerContent, ctx);
					str = SprEngine.FillRefsUsingCache(str, ctx);
				}
				else { nOffset = nStart + 1; continue; }
			}

			return str;
		}

		public static PwEntry FindRefTarget(string strFullRef, SprContext ctx,
			out char chScan, out char chWanted)
		{
			chScan = char.MinValue;
			chWanted = char.MinValue;

			if(strFullRef == null) { Debug.Assert(false); return null; }
			if(!strFullRef.StartsWith(StrRefStart, SprEngine.ScMethod) ||
				!strFullRef.EndsWith(StrRefEnd, SprEngine.ScMethod))
				return null;
			if((ctx == null) || (ctx.Database == null)) { Debug.Assert(false); return null; }

			string strRef = strFullRef.Substring(StrRefStart.Length,
				strFullRef.Length - StrRefStart.Length - StrRefEnd.Length);
			if(strRef.Length <= 4) return null;
			if(strRef[1] != '@') return null;
			if(strRef[3] != ':') return null;

			chScan = char.ToUpper(strRef[2]);
			chWanted = char.ToUpper(strRef[0]);

			SearchParameters sp = SearchParameters.None;
			sp.SearchString = strRef.Substring(4);
			sp.RespectEntrySearchingDisabled = false;

			if(chScan == 'T') sp.SearchInTitles = true;
			else if(chScan == 'U') sp.SearchInUserNames = true;
			else if(chScan == 'A') sp.SearchInUrls = true;
			else if(chScan == 'P') sp.SearchInPasswords = true;
			else if(chScan == 'N') sp.SearchInNotes = true;
			else if(chScan == 'I') sp.SearchInUuids = true;
			else if(chScan == 'O') sp.SearchInOther = true;
			else return null;

			PwObjectList<PwEntry> lFound = new PwObjectList<PwEntry>();
			ctx.Database.RootGroup.SearchEntries(sp, lFound);

			return ((lFound.UCount > 0) ? lFound.GetAt(0) : null);
		}

		private static string FillRefsUsingCache(string strText, SprContext ctx)
		{
			string str = strText;

			foreach(KeyValuePair<string, string> kvp in ctx.RefsCache)
			{
				// str = str.Replace(kvp.Key, kvp.Value);
				str = StrUtil.ReplaceCaseInsensitive(str, kvp.Key, kvp.Value);
			}

			return str;
		}

		private static void AddRefToCache(string strRef, string strValue,
			SprContext ctx)
		{
			if(strRef == null) { Debug.Assert(false); return; }
			if(strValue == null) { Debug.Assert(false); return; }
			if(ctx == null) { Debug.Assert(false); return; }

			// Only add if not exists, do not overwrite
			if(!ctx.RefsCache.ContainsKey(strRef))
				ctx.RefsCache.Add(strRef, strValue);
		}

		// internal static bool MightChange(string strText)
		// {
		//	if(string.IsNullOrEmpty(strText)) return false;
		//	return (strText.IndexOfAny(m_vPlhEscapes) >= 0);
		// }

		internal static bool MightChange(string str)
		{
			if(str == null) { Debug.Assert(false); return false; }

			int iBStart = str.IndexOf('{');
			if(iBStart >= 0)
			{
				int iBEnd = str.LastIndexOf('}');
				if(iBStart < iBEnd) return true;
			}

			int iPFirst = str.IndexOf('%');
			if(iPFirst >= 0)
			{
				int iPLast = str.LastIndexOf('%');
				if(iPFirst < iPLast) return true;
			}

			return false;
		}

		/// <summary>
		/// Fast probabilistic test whether a string might be
		/// changed when compiling with <c>SprCompileFlags.Deref</c>.
		/// </summary>
		internal static bool MightDeref(string strText)
		{
			if(strText == null) return false;
			return (strText.IndexOf('{') >= 0);
		}

		internal static string DerefFn(string str, PwEntry pe)
		{
			if(!MightDeref(str)) return str;

			SprContext ctx = new SprContext(pe,
				Program.MainForm.DocumentManager.SafeFindContainerOf(pe),
				SprCompileFlags.Deref);
			// ctx.ForcePlainTextPasswords = false;

			return Compile(str, ctx);
		}

		/// <summary>
		/// Parse and remove a placeholder of the form
		/// <c>{PLH:/Param1/Param2/.../}</c>.
		/// </summary>
		internal static bool ParseAndRemovePlhWithParams(ref string str,
			SprContext ctx, uint uRecursionLevel, string strPlhStart,
			out int iStart, out List<string> lParams, bool bSprCmpParams)
		{
			Debug.Assert(strPlhStart.StartsWith(@"{") && !strPlhStart.EndsWith(@"}"));

			iStart = str.IndexOf(strPlhStart, StrUtil.CaseIgnoreCmp);
			if(iStart < 0) { lParams = null; return false; }

			lParams = new List<string>();

			try
			{
				int p = iStart + strPlhStart.Length;
				if(p >= str.Length) throw new FormatException();

				char chSep = str[p];

				while(true)
				{
					if((p + 1) >= str.Length) throw new FormatException();

					if(str[p + 1] == '}') break;

					int q = str.IndexOf(chSep, p + 1);
					if(q < 0) throw new FormatException();

					lParams.Add(str.Substring(p + 1, q - p - 1));
					p = q;
				}

				Debug.Assert(str[p + 1] == '}');
				str = str.Remove(iStart, (p + 1) - iStart + 1);
			}
			catch(Exception)
			{
				str = str.Substring(0, iStart);
			}

			if(bSprCmpParams && (ctx != null))
			{
				SprContext ctxSub = ctx.WithoutContentTransformations();
				for(int i = 0; i < lParams.Count; ++i)
					lParams[i] = CompileInternal(lParams[i], ctxSub, uRecursionLevel);
			}

			return true;
		}

		private static string PerformTextTransforms(string strText, SprContext ctx,
			uint uRecursionLevel)
		{
			string str = strText;
			int iStart;
			List<string> lParams;

			while(ParseAndRemovePlhWithParams(ref str, ctx, uRecursionLevel,
				@"{T-REPLACE-RX:", out iStart, out lParams, true))
			{
				if(lParams.Count < 2) continue;
				if(lParams.Count == 2) lParams.Add(string.Empty);

				try
				{
					string strNew = Regex.Replace(lParams[0], lParams[1], lParams[2]);
					strNew = TransformContent(strNew, ctx);
					str = str.Insert(iStart, strNew);
				}
				catch(Exception) { }
			}

			while(ParseAndRemovePlhWithParams(ref str, ctx, uRecursionLevel,
				@"{T-CONV:", out iStart, out lParams, true))
			{
				if(lParams.Count < 2) continue;

				try
				{
					string strNew = lParams[0];
					string strCmd = lParams[1].ToLower();

					if((strCmd == "u") || (strCmd == "upper"))
						strNew = strNew.ToUpper();
					else if((strCmd == "l") || (strCmd == "lower"))
						strNew = strNew.ToLower();
					else if(strCmd == "base64")
					{
						byte[] pbUtf8 = StrUtil.Utf8.GetBytes(strNew);
						strNew = Convert.ToBase64String(pbUtf8);
					}
					else if(strCmd == "hex")
					{
						byte[] pbUtf8 = StrUtil.Utf8.GetBytes(strNew);
						strNew = MemUtil.ByteArrayToHexString(pbUtf8);
					}
					else if(strCmd == "uri")
						strNew = Uri.EscapeDataString(strNew);
					else if(strCmd == "uri-dec")
						strNew = Uri.UnescapeDataString(strNew);

					strNew = TransformContent(strNew, ctx);
					str = str.Insert(iStart, strNew);
				}
				catch(Exception) { Debug.Assert(false); }
			}

			return str;
		}

		private static string FillGroupPlh(string strData, string strPlhPrefix,
			PwGroup pg, SprContext ctx, uint uRecursionLevel)
		{
			Debug.Assert(strPlhPrefix.StartsWith("{"));
			Debug.Assert(!strPlhPrefix.EndsWith("_"));
			Debug.Assert(!strPlhPrefix.EndsWith("}"));

			string str = strData;

			str = SprEngine.FillIfExists(str, strPlhPrefix + @"}",
				new ProtectedString(false, pg.Name), ctx, uRecursionLevel);

			ProtectedString psGroupPath = new ProtectedString(false, pg.GetFullPath());
			str = SprEngine.FillIfExists(str, strPlhPrefix + @"_PATH}", psGroupPath,
				ctx, uRecursionLevel);
			str = SprEngine.FillIfExists(str, strPlhPrefix + @"PATH}", psGroupPath,
				ctx, uRecursionLevel); // Obsolete; for backward compatibility

			str = SprEngine.FillIfExists(str, strPlhPrefix + @"_NOTES}",
				new ProtectedString(false, pg.Notes), ctx, uRecursionLevel);

			return str;
		}
	}
}
