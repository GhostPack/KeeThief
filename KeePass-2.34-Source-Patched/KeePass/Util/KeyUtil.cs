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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Resources;
using KeePassLib.Utility;
using KeePassLib.Serialization;

using KeePass.Forms;

namespace KeePass.Util
{
	public static class KeyUtil
	{
		public static CompositeKey KeyFromCommandLine(CommandLineArgs args)
		{
			if(args == null) throw new ArgumentNullException("args");

			CompositeKey cmpKey = new CompositeKey();
			string strPassword = args[AppDefs.CommandLineOptions.Password];
			string strPasswordEnc = args[AppDefs.CommandLineOptions.PasswordEncrypted];
			string strPasswordStdIn = args[AppDefs.CommandLineOptions.PasswordStdIn];
			string strKeyFile = args[AppDefs.CommandLineOptions.KeyFile];
			string strUserAcc = args[AppDefs.CommandLineOptions.UserAccount];

			if(strPassword != null)
				cmpKey.AddUserKey(new KcpPassword(strPassword));
			else if(strPasswordEnc != null)
				cmpKey.AddUserKey(new KcpPassword(StrUtil.DecryptString(strPasswordEnc)));
			else if(strPasswordStdIn != null)
			{
				KcpPassword kcpPw = ReadPasswordStdIn(true);
				if(kcpPw != null) cmpKey.AddUserKey(kcpPw);
			}
			
			if(strKeyFile != null)
			{
				if(Program.KeyProviderPool.IsKeyProvider(strKeyFile))
				{
					KeyProviderQueryContext ctxKP = new KeyProviderQueryContext(
						IOConnectionInfo.FromPath(args.FileName), false, false);

					bool bPerformHash;
					byte[] pbProvKey = Program.KeyProviderPool.GetKey(strKeyFile, ctxKP,
						out bPerformHash);
					if((pbProvKey != null) && (pbProvKey.Length > 0))
					{
						try { cmpKey.AddUserKey(new KcpCustomKey(strKeyFile, pbProvKey, bPerformHash)); }
						catch(Exception exCKP)
						{
							MessageService.ShowWarning(exCKP);
							return null;
						}

						Array.Clear(pbProvKey, 0, pbProvKey.Length);
					}
					else return null; // Provider has shown error message
				}
				else // Key file
				{
					try { cmpKey.AddUserKey(new KcpKeyFile(strKeyFile)); }
					catch(Exception exKey)
					{
						MessageService.ShowWarning(strKeyFile, KPRes.KeyFileError, exKey);
						return null;
					}
				}
			}
			
			if(strUserAcc != null)
			{
				try { cmpKey.AddUserKey(new KcpUserAccount()); }
				catch(Exception exUA)
				{
					MessageService.ShowWarning(exUA);
					return null;
				}
			}

			if(cmpKey.UserKeyCount > 0)
			{
				ClearKeyOptions(args, true);
				return cmpKey;
			}

			return null;
		}

		private static void ClearKeyOptions(CommandLineArgs args, bool bOnlyIfOptionEnabled)
		{
			if(args == null) { Debug.Assert(false); return; }

			if(bOnlyIfOptionEnabled && !Program.Config.Security.ClearKeyCommandLineParams)
				return;

			args.Remove(AppDefs.CommandLineOptions.Password);
			args.Remove(AppDefs.CommandLineOptions.PasswordEncrypted);
			args.Remove(AppDefs.CommandLineOptions.PasswordStdIn);
			args.Remove(AppDefs.CommandLineOptions.KeyFile);
			args.Remove(AppDefs.CommandLineOptions.PreSelect);
			args.Remove(AppDefs.CommandLineOptions.UserAccount);
		}

		private static bool m_bReadPwStdIn = false;
		private static string m_strReadPwStdIn = null;
		/// <summary>
		/// Read a password from StdIn. The password is read only once
		/// and then cached.
		/// </summary>
		internal static KcpPassword ReadPasswordStdIn(bool bFailWithUI)
		{
			string strPw = null;

			if(m_bReadPwStdIn) strPw = m_strReadPwStdIn;
			else
			{
				try { strPw = Console.ReadLine(); }
				catch(Exception exCon)
				{
					if(bFailWithUI) MessageService.ShowWarning(exCon);
				}
			}

			if(strPw == null)
			{
				m_strReadPwStdIn = null;
				m_bReadPwStdIn = true;

				return null;
			}

			strPw = strPw.Trim();

			m_strReadPwStdIn = strPw;
			m_bReadPwStdIn = true;

			return new KcpPassword(strPw);
		}

		internal static string[] MakeCtxIndependent(string[] vCmdLineArgs)
		{
			if(vCmdLineArgs == null) { Debug.Assert(false); return new string[0]; }

			CommandLineArgs cl = new CommandLineArgs(vCmdLineArgs);
			List<string> lFlt = new List<string>();

			foreach(string strArg in vCmdLineArgs)
			{
				KeyValuePair<string, string> kvpArg = CommandLineArgs.GetParameter(strArg);
				if(kvpArg.Key.Equals(AppDefs.CommandLineOptions.PasswordStdIn, StrUtil.CaseIgnoreCmp))
				{
					KcpPassword kcpPw = ReadPasswordStdIn(true);

					if((cl[AppDefs.CommandLineOptions.Password] == null) &&
						(cl[AppDefs.CommandLineOptions.PasswordEncrypted] == null) &&
						(kcpPw != null))
					{
						lFlt.Add("-" + AppDefs.CommandLineOptions.Password + ":" +
							kcpPw.Password.ReadString()); // No quote wrapping/encoding
					}
				}
				else lFlt.Add(strArg);
			}

			return lFlt.ToArray();
		}

		public static bool ReAskKey(PwDatabase pwDatabase, bool bFailWithUI)
		{
			if(pwDatabase == null) { Debug.Assert(false); return false; }

			KeyPromptForm dlg = new KeyPromptForm();
			dlg.InitEx(pwDatabase.IOConnectionInfo, false, true,
				KPRes.EnterCurrentCompositeKey);
			if(UIUtil.ShowDialogNotValue(dlg, DialogResult.OK)) return false;

			CompositeKey ck = dlg.CompositeKey;
			bool bResult = ck.EqualsValue(pwDatabase.MasterKey);

			if(!bResult)
				MessageService.ShowWarning(KLRes.InvalidCompositeKey,
						KLRes.InvalidCompositeKeyHint);

			UIUtil.DestroyForm(dlg);
			return bResult;
		}
	}
}
