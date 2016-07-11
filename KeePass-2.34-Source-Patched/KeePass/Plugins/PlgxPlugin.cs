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
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Resources;
using System.Security.Cryptography;

using Microsoft.CSharp;
// using Microsoft.VisualBasic;

using KeePass.App;
using KeePass.Forms;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Native;
using KeePassLib.Resources;
using KeePassLib.Utility;

namespace KeePass.Plugins
{
	public sealed class PlgxException : Exception
	{
		private string m_strMsg;

		public override string Message
		{
			get { return m_strMsg; }
		}

		public PlgxException(string strMessage)
		{
			if(strMessage == null) throw new ArgumentNullException("strMessage");

			m_strMsg = strMessage;
		}
	}

	public static class PlgxPlugin
	{
		public const string PlgxExtension = "plgx";

		private const uint PlgxSignature1 = 0x65D90719;
		private const uint PlgxSignature2 = 0x3DDD0503;
		private const uint PlgxVersion = 0x00010000;
		private const uint PlgxVersionMask = 0xFFFF0000;

		private const ushort PlgxEOF = 0;
		private const ushort PlgxFileUuid = 1;
		private const ushort PlgxBaseFileName = 2;
		private const ushort PlgxBeginContent = 3;
		private const ushort PlgxFile = 4;
		private const ushort PlgxEndContent = 5;
		private const ushort PlgxCreationTime = 6;
		private const ushort PlgxGeneratorName = 7;
		private const ushort PlgxGeneratorVersion = 8;
		private const ushort PlgxPrereqKP = 9; // KeePass version
		private const ushort PlgxPrereqNet = 10; // .NET Framework version
		private const ushort PlgxPrereqOS = 11; // Operating system
		private const ushort PlgxPrereqPtr = 12; // Pointer size
		private const ushort PlgxBuildPre = 13;
		private const ushort PlgxBuildPost = 14;

		private const ushort PlgxfEOF = 0;
		private const ushort PlgxfPath = 1;
		private const ushort PlgxfData = 2;

		public static void Load(string strFilePath, IStatusLogger slStatus)
		{
			try { LoadPriv(strFilePath, slStatus, true, true, true, null); }
			catch(PlgxException exPlgx)
			{
				MessageService.ShowWarning(strFilePath + MessageService.NewParagraph +
					KPRes.PluginLoadFailed + MessageService.NewParagraph +
					exPlgx.Message);
			}
			catch(Exception exLoad)
			{
				PluginManager.ShowLoadError(strFilePath, exLoad, slStatus);
			}
		}

		public static void CreateInfoFile(string strPlgxPath)
		{
			FileStream fsOut = null;
			TextWriter twLog = null;

			try
			{
				fsOut = new FileStream(strPlgxPath + ".txt", FileMode.Create,
					FileAccess.Write, FileShare.None);
				twLog = new StreamWriter(fsOut, new UTF8Encoding(false));

				NullStatusLogger sl = new NullStatusLogger();
				LoadPriv(strPlgxPath, sl, false, false, false, twLog);
			}
			catch(Exception ex)
			{
				MessageService.ShowWarning(strPlgxPath, ex);
			}
			finally
			{
				if(twLog != null) twLog.Close();
				if(fsOut != null) fsOut.Close();
			}
		}

		private static void LoadPriv(string strFilePath, IStatusLogger slStatus,
			bool bAllowCached, bool bAllowCompile, bool bAllowLoad, TextWriter twLog)
		{
			if(strFilePath == null) { Debug.Assert(false); return; }

			FileInfo fi = new FileInfo(strFilePath);
			if(fi.Length < 12) return; // Ignore file, don't throw

			FileStream fs = new FileStream(strFilePath, FileMode.Open,
				FileAccess.Read, FileShare.Read);
			BinaryReader br = new BinaryReader(fs);

			PlgxPluginInfo plgx = new PlgxPluginInfo(true, bAllowCached, bAllowCompile);
			plgx.LogStream = twLog;

			string strPluginPath = null;
			try { strPluginPath = ReadFile(br, plgx, slStatus); }
			finally
			{
				br.Close();
				fs.Close();
			}

			if(!string.IsNullOrEmpty(strPluginPath) && bAllowLoad)
				Program.MainForm.PluginManager.LoadPlugin(strPluginPath,
					plgx.BaseFileName, strFilePath, false);
		}

		private static string ReadFile(BinaryReader br, PlgxPluginInfo plgx,
			IStatusLogger slStatus)
		{
			uint uSig1 = br.ReadUInt32();
			uint uSig2 = br.ReadUInt32();
			uint uVersion = br.ReadUInt32();

			if((uSig1 != PlgxSignature1) || (uSig2 != PlgxSignature2))
				return null; // Ignore file, don't throw
			if((uVersion & PlgxVersionMask) > (PlgxVersion & PlgxVersionMask))
				throw new PlgxException(KLRes.FileVersionUnsupported);

			string strPluginPath = null;
			string strTmpRoot = null;
			bool? bContent = null;
			string strBuildPre = null, strBuildPost = null;

			while(true)
			{
				KeyValuePair<ushort, byte[]> kvp = ReadObject(br);

				if(kvp.Key == PlgxEOF) break;
				else if(kvp.Key == PlgxFileUuid)
					plgx.FileUuid = new PwUuid(kvp.Value);
				else if(kvp.Key == PlgxBaseFileName)
					plgx.BaseFileName = StrUtil.Utf8.GetString(kvp.Value);
				else if(kvp.Key == PlgxCreationTime) { } // Ignore
				else if(kvp.Key == PlgxGeneratorName) { }
				else if(kvp.Key == PlgxGeneratorVersion) { }
				else if(kvp.Key == PlgxPrereqKP)
				{
					ulong uReq = MemUtil.BytesToUInt64(kvp.Value);
					if(uReq > PwDefs.FileVersion64)
						throw new PlgxException(KLRes.FileNewVerReq);
				}
				else if(kvp.Key == PlgxPrereqNet)
				{
					ulong uReq = MemUtil.BytesToUInt64(kvp.Value);
					ulong uInst = WinUtil.GetMaxNetFrameworkVersion();
					if((uInst != 0) && (uReq > uInst))
						throw new PlgxException(KPRes.NewerNetRequired);
				}
				else if(kvp.Key == PlgxPrereqOS)
				{
					string strOS = "," + WinUtil.GetOSStr() + ",";
					string strReq = "," + StrUtil.Utf8.GetString(kvp.Value) + ",";
					if(strReq.IndexOf(strOS, StrUtil.CaseIgnoreCmp) < 0)
						throw new PlgxException(KPRes.PluginOperatingSystemUnsupported);
				}
				else if(kvp.Key == PlgxPrereqPtr)
				{
					uint uReq = MemUtil.BytesToUInt32(kvp.Value);
					if(uReq > (uint)IntPtr.Size)
						throw new PlgxException(KPRes.PluginOperatingSystemUnsupported);
				}
				else if(kvp.Key == PlgxBuildPre)
					strBuildPre = StrUtil.Utf8.GetString(kvp.Value);
				else if(kvp.Key == PlgxBuildPost)
					strBuildPost = StrUtil.Utf8.GetString(kvp.Value);
				else if(kvp.Key == PlgxBeginContent)
				{
					if(bContent.HasValue)
						throw new PlgxException(KLRes.FileCorrupted);

					string strCached = PlgxCache.GetCacheFile(plgx, true, false);
					if(!string.IsNullOrEmpty(strCached) && plgx.AllowCached)
					{
						strPluginPath = strCached;
						break;
					}

					if(slStatus != null)
						slStatus.SetText(KPRes.PluginsCompilingAndLoading,
							LogStatusType.Info);

					bContent = true;
					if(plgx.LogStream != null) plgx.LogStream.WriteLine("Content:");
				}
				else if(kvp.Key == PlgxFile)
				{
					if(!bContent.HasValue || !bContent.Value)
						throw new PlgxException(KLRes.FileCorrupted);

					if(strTmpRoot == null) strTmpRoot = CreateTempDirectory();
					ExtractFile(kvp.Value, strTmpRoot, plgx);
				}
				else if(kvp.Key == PlgxEndContent)
				{
					if(!bContent.HasValue || !bContent.Value)
						throw new PlgxException(KLRes.FileCorrupted);

					bContent = false;
				}
				else { Debug.Assert(false); }
			}

			if((strPluginPath == null) && plgx.AllowCompile)
				strPluginPath = Compile(strTmpRoot, plgx, strBuildPre, strBuildPost);

			return strPluginPath;
		}

		private static string CreateTempDirectory()
		{
			string strTmpRoot = UrlUtil.GetTempPath();
			strTmpRoot = UrlUtil.EnsureTerminatingSeparator(strTmpRoot, false);
			strTmpRoot += (new PwUuid(true)).ToHexString();

			Directory.CreateDirectory(strTmpRoot);

			Program.TempFilesPool.AddDirectory(strTmpRoot, true);
			return strTmpRoot;
		}

		private static KeyValuePair<ushort, byte[]> ReadObject(BinaryReader br)
		{
			try
			{
				ushort uType = br.ReadUInt16();
				uint uLength = br.ReadUInt32();
				byte[] pbData = ((uLength > 0) ? br.ReadBytes((int)uLength) : null);

				return new KeyValuePair<ushort, byte[]>(uType, pbData);
			}
			catch(Exception) { throw new PlgxException(KLRes.FileCorrupted); }
		}

		private static void WriteObject(BinaryWriter bw, ushort uType, byte[] pbData)
		{
			bw.Write(uType);
			bw.Write((uint)((pbData != null) ? pbData.Length : 0));
			if((pbData != null) && (pbData.Length > 0)) bw.Write(pbData);
		}

		private static void ExtractFile(byte[] pbData, string strTmpRoot,
			PlgxPluginInfo plgx)
		{
			MemoryStream ms = new MemoryStream(pbData, false);
			BinaryReader br = new BinaryReader(ms);

			string strPath = null;
			byte[] pbContent = null;

			while(true)
			{
				KeyValuePair<ushort, byte[]> kvp = ReadObject(br);

				if(kvp.Key == PlgxfEOF) break;
				else if(kvp.Key == PlgxfPath)
					strPath = StrUtil.Utf8.GetString(kvp.Value);
				else if(kvp.Key == PlgxfData) pbContent = kvp.Value;
				else { Debug.Assert(false); }
			}

			br.Close();
			ms.Close();

			if(!string.IsNullOrEmpty(strPath) && (pbContent != null))
			{
				string strTmpFile = UrlUtil.EnsureTerminatingSeparator(strTmpRoot,
					false) + UrlUtil.ConvertSeparators(strPath);

				string strTmpDir = UrlUtil.GetFileDirectory(strTmpFile, false, true);
				if(!Directory.Exists(strTmpDir)) Directory.CreateDirectory(strTmpDir);

				byte[] pbDecompressed = MemUtil.Decompress(pbContent);
				File.WriteAllBytes(strTmpFile, pbDecompressed);

				// Although the temporary directory will be deleted recursively
				// anyway, add the extracted file here manually, in order to
				// minimize left-over files in case the recursive deletion fails
				// due to locked / in-use files
				Program.TempFilesPool.Add(strTmpFile);

				if(plgx.LogStream != null)
				{
					MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
					byte[] pbMD5 = md5.ComputeHash(pbDecompressed);
					plgx.LogStream.Write(MemUtil.ByteArrayToHexString(pbMD5));
					plgx.LogStream.WriteLine(" " + strPath);
				}
			}
			else { Debug.Assert(false); }
		}

		private static void AddFile(BinaryWriter bw, string strRootDir,
			string strSourceFile)
		{
			if(strSourceFile.EndsWith(".suo", StrUtil.CaseIgnoreCmp)) return;

			MemoryStream msFile = new MemoryStream();
			BinaryWriter bwFile = new BinaryWriter(msFile);

			strRootDir = UrlUtil.EnsureTerminatingSeparator(strRootDir, false);
			string strRel = UrlUtil.ConvertSeparators(UrlUtil.MakeRelativePath(
				strRootDir + "Sentinel.txt", strSourceFile), '/');
			WriteObject(bwFile, PlgxfPath, StrUtil.Utf8.GetBytes(strRel));

			byte[] pbData = (File.ReadAllBytes(strSourceFile) ?? new byte[0]);
			if(pbData.LongLength >= (long)(int.MaxValue / 2)) // Max 1 GB
				throw new OutOfMemoryException();

			byte[] pbCompressed = MemUtil.Compress(pbData);
			WriteObject(bwFile, PlgxfData, pbCompressed);

			WriteObject(bwFile, PlgxfEOF, null);

			WriteObject(bw, PlgxFile, msFile.ToArray());
			bwFile.Close();
			msFile.Close();

			if(!MemUtil.ArraysEqual(MemUtil.Decompress(pbCompressed), pbData))
				throw new InvalidOperationException();
		}

		public static void CreateFromCommandLine()
		{
			try
			{
				string strDir = Program.CommandLineArgs.FileName;
				if(string.IsNullOrEmpty(strDir))
				{
					FolderBrowserDialog dlg = UIUtil.CreateFolderBrowserDialog(KPRes.Plugin);
					if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }
					strDir = dlg.SelectedPath;
					dlg.Dispose();
				}

				CreateFromDirectory(strDir);
			}
			catch(Exception ex) { MessageService.ShowWarning(ex); }
		}

		private static void CreateFromDirectory(string strDirPath)
		{
			string strPlgx = strDirPath + "." + PlgxExtension;

			PlgxPluginInfo plgx = new PlgxPluginInfo(false, true, true);
			PlgxCsprojLoader.LoadDefault(strDirPath, plgx);

			FileStream fs = new FileStream(strPlgx, FileMode.Create,
				FileAccess.Write, FileShare.None);
			BinaryWriter bw = new BinaryWriter(fs);

			bw.Write(PlgxSignature1);
			bw.Write(PlgxSignature2);
			bw.Write(PlgxVersion);
			WriteObject(bw, PlgxFileUuid, (new PwUuid(true)).UuidBytes);
			WriteObject(bw, PlgxBaseFileName, StrUtil.Utf8.GetBytes(
				plgx.BaseFileName));
			WriteObject(bw, PlgxCreationTime, StrUtil.Utf8.GetBytes(
				TimeUtil.SerializeUtc(DateTime.Now)));
			WriteObject(bw, PlgxGeneratorName, StrUtil.Utf8.GetBytes(
				PwDefs.ShortProductName));
			WriteObject(bw, PlgxGeneratorVersion, MemUtil.UInt64ToBytes(
				PwDefs.FileVersion64));

			string strKP = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxPrereqKP];
			if(!string.IsNullOrEmpty(strKP))
			{
				ulong uKP = StrUtil.ParseVersion(strKP);
				if(uKP != 0) WriteObject(bw, PlgxPrereqKP, MemUtil.UInt64ToBytes(uKP));
			}

			string strNet = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxPrereqNet];
			if(!string.IsNullOrEmpty(strNet))
			{
				ulong uNet = StrUtil.ParseVersion(strNet);
				if(uNet != 0) WriteObject(bw, PlgxPrereqNet, MemUtil.UInt64ToBytes(uNet));
			}

			string strOS = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxPrereqOS];
			if(!string.IsNullOrEmpty(strOS))
				WriteObject(bw, PlgxPrereqOS, StrUtil.Utf8.GetBytes(strOS));

			string strPtr = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxPrereqPtr];
			if(!string.IsNullOrEmpty(strPtr))
			{
				uint uPtr;
				if(uint.TryParse(strPtr, out uPtr))
					WriteObject(bw, PlgxPrereqPtr, MemUtil.UInt32ToBytes(uPtr));
			}

			string strBuildPre = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxBuildPre];
			if(!string.IsNullOrEmpty(strBuildPre))
				WriteObject(bw, PlgxBuildPre, StrUtil.Utf8.GetBytes(strBuildPre));

			string strBuildPost = Program.CommandLineArgs[AppDefs.CommandLineOptions.PlgxBuildPost];
			if(!string.IsNullOrEmpty(strBuildPost))
				WriteObject(bw, PlgxBuildPost, StrUtil.Utf8.GetBytes(strBuildPost));

			WriteObject(bw, PlgxBeginContent, null);

			RecursiveFileAdd(bw, strDirPath, new DirectoryInfo(strDirPath));

			WriteObject(bw, PlgxEndContent, null);
			WriteObject(bw, PlgxEOF, null);

			bw.Close();
			fs.Close();

			// Test loading not possible, because MainForm not available
			// PlgxPlugin.Load(strPlgx);
		}

		private static void RecursiveFileAdd(BinaryWriter bw, string strRootDir,
			DirectoryInfo di)
		{
			if(di.Name.Equals(".svn", StrUtil.CaseIgnoreCmp)) return; // Skip SVN

			foreach(FileInfo fi in di.GetFiles())
			{
				if((fi.Name == ".") || (fi.Name == "..")) continue;

				AddFile(bw, strRootDir, fi.FullName);
			}

			foreach(DirectoryInfo diSub in di.GetDirectories())
				RecursiveFileAdd(bw, strRootDir, diSub);
		}

		private static string Compile(string strTmpRoot, PlgxPluginInfo plgx,
			string strBuildPre, string strBuildPost)
		{
			if(strTmpRoot == null) { Debug.Assert(false); return null; }

			RunBuildCommand(strBuildPre, UrlUtil.EnsureTerminatingSeparator(
				strTmpRoot, false), null);

			PlgxCsprojLoader.LoadDefault(strTmpRoot, plgx);

			List<string> vCustomRefs = new List<string>();
			foreach(string strIncRefAsm in plgx.IncludedReferencedAssemblies)
			{
				string strSrcAsm = plgx.GetAbsPath(UrlUtil.ConvertSeparators(
					strIncRefAsm));
				string strCached = PlgxCache.AddCacheFile(strSrcAsm, plgx);
				if(string.IsNullOrEmpty(strCached))
					throw new InvalidOperationException();
				vCustomRefs.Add(strCached);
			}

			CompilerParameters cp = plgx.CompilerParameters;
			cp.OutputAssembly = UrlUtil.EnsureTerminatingSeparator(strTmpRoot, false) +
				UrlUtil.GetFileName(PlgxCache.GetCacheFile(plgx, false, false));
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = false;
			cp.IncludeDebugInformation = false;
			cp.TreatWarningsAsErrors = false;
			cp.ReferencedAssemblies.Add(WinUtil.GetExecutable());
			foreach(string strCustomRef in vCustomRefs)
				cp.ReferencedAssemblies.Add(strCustomRef);

			CompileEmbeddedRes(plgx);
			PrepareSourceFiles(plgx);

			string[] vCompilers;
			Version vClr = Environment.Version;
			int iClrMajor = vClr.Major, iClrMinor = vClr.Minor;
			if((iClrMajor >= 5) || ((iClrMajor == 4) && (iClrMinor >= 5)))
			{
				vCompilers = new string[] {
					null,
					"v4.5",
					"v4", // Suggested in CodeDomProvider.CreateProvider doc
					"v4.0", // Suggested in community content of the above
					"v4.0.30319", // Deduced from file system
					"v3.5"
				};
			}
			else if(iClrMajor == 4) // 4.0
			{
				vCompilers = new string[] {
					null,
					"v4", // Suggested in CodeDomProvider.CreateProvider doc
					"v4.0", // Suggested in community content of the above
					"v4.0.30319", // Deduced from file system
					"v4.5",
					"v3.5"
				};
			}
			else // <= 3.5
			{
				vCompilers = new string[] {
					null,
					"v3.5",
					"v4", // Suggested in CodeDomProvider.CreateProvider doc
					"v4.0", // Suggested in community content of the above
					"v4.0.30319", // Deduced from file system
					"v4.5"
				};
			}

			CompilerResults cr = null;
			StringBuilder sbCompilerLog = new StringBuilder();
			bool bCompiled = false;
			for(int iCmp = 0; iCmp < vCompilers.Length; ++iCmp)
			{
				if(CompileAssembly(plgx, out cr, vCompilers[iCmp]))
				{
					bCompiled = true;
					break;
				}

				if(cr != null)
					AppendCompilerResults(sbCompilerLog, vCompilers[iCmp], cr);
			}

			if(!bCompiled)
			{
				if(Program.CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null)
					SaveCompilerResults(plgx, sbCompilerLog);

				throw new InvalidOperationException();
			}

			Program.TempFilesPool.Add(cr.PathToAssembly);

			Debug.Assert(cr.PathToAssembly == cp.OutputAssembly);
			string strCacheAsm = PlgxCache.AddCacheAssembly(cr.PathToAssembly, plgx);

			RunBuildCommand(strBuildPost, UrlUtil.EnsureTerminatingSeparator(
				strTmpRoot, false), UrlUtil.GetFileDirectory(strCacheAsm, true, false));

			return strCacheAsm;
		}

		private static bool CompileAssembly(PlgxPluginInfo plgx,
			out CompilerResults cr, string strCompilerVersion)
		{
			cr = null;

			const string StrCoreRef = "System.Core";
			const string StrCoreDll = "System.Core.dll";
			bool bHasCore = false, bCoreAdded = false;
			foreach(string strAsm in plgx.CompilerParameters.ReferencedAssemblies)
			{
				if(UrlUtil.AssemblyEquals(strAsm, StrCoreRef))
				{
					bHasCore = true;
					break;
				}
			}
			if((strCompilerVersion != null) && strCompilerVersion.StartsWith(
				"v", StrUtil.CaseIgnoreCmp))
			{
				ulong v = StrUtil.ParseVersion(strCompilerVersion.Substring(1));
				if(!bHasCore && (v >= 0x0003000500000000UL))
				{
					plgx.CompilerParameters.ReferencedAssemblies.Add(StrCoreDll);
					bCoreAdded = true;
				}
			}

			bool bResult = false;
			try
			{
				Dictionary<string, string> dictOpt = new Dictionary<string, string>();
				if(!string.IsNullOrEmpty(strCompilerVersion))
					dictOpt.Add("CompilerVersion", strCompilerVersion);

				// Windows 98 only supports the parameterless constructor;
				// check must be separate from the instantiation method
				if(WinUtil.IsWindows9x) dictOpt.Clear();

				CodeDomProvider cdp = null;
				if(plgx.ProjectType == PlgxProjectType.CSharp)
					cdp = ((dictOpt.Count == 0) ? new CSharpCodeProvider() :
						CreateCscProvider(dictOpt));
				// else if(plgx.ProjectType == PlgxProjectType.VisualBasic)
				//	cdp = ((dictOpt.Count == 0) ? new VBCodeProvider() :
				//		new VBCodeProvider(dictOpt));
				else throw new InvalidOperationException();

				cr = cdp.CompileAssemblyFromFile(plgx.CompilerParameters,
					plgx.SourceFiles.ToArray());

				bResult = ((cr.Errors == null) || !cr.Errors.HasErrors);
			}
			catch(Exception) { }

			if(bCoreAdded)
				plgx.CompilerParameters.ReferencedAssemblies.Remove(StrCoreDll);

			return bResult;
		}

		private static void AppendCompilerResults(StringBuilder sb, string strCompiler,
			CompilerResults cr)
		{
			if((sb == null) || (cr == null)) { Debug.Assert(false); return; }
			// strCompiler may be null

			if(sb.Length > 0)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine();
			}

			sb.AppendLine(new string('=', 78));
			sb.AppendLine(@"Compiler '" + (strCompiler ?? "null") + @"':");
			sb.AppendLine();

			foreach(string str in cr.Output)
			{
				if(str == null) { Debug.Assert(false); continue; }

				sb.AppendLine(str.Trim());
			}
		}

		private static void SaveCompilerResults(PlgxPluginInfo plgx,
			StringBuilder sb)
		{
			string strFile = Path.GetTempFileName();
			File.WriteAllText(strFile, sb.ToString(), StrUtil.Utf8);

			MessageService.ShowWarning(plgx.BaseFileName,
				"Compilation failed. Compiler results have been saved to:" +
				Environment.NewLine + strFile);
		}

		// Windows 98 only supports the parameterless constructor, therefore
		// the instantiation of the one with parameters must be in a separate
		// method; check must be separate from the instantiation method
		private static CodeDomProvider CreateCscProvider(IDictionary<string,
			string> iOpts)
		{
			return new CSharpCodeProvider(iOpts);
		}

		private static void CompileEmbeddedRes(PlgxPluginInfo plgx)
		{
			foreach(string strResSrc in plgx.EmbeddedResourceSources)
			{
				string strResFileName = plgx.BaseFileName + "." + UrlUtil.ConvertSeparators(
					UrlUtil.MakeRelativePath(plgx.CsprojFilePath, strResSrc), '.');
				string strResFile = UrlUtil.GetFileDirectory(plgx.CsprojFilePath, true,
					true) + strResFileName;

				if(strResSrc.EndsWith(".resx", StrUtil.CaseIgnoreCmp))
				{
					PrepareResXFile(strResSrc);

					string strRsrc = UrlUtil.StripExtension(strResFile) + ".resources";
					ResXResourceReader r = new ResXResourceReader(strResSrc);
					ResourceWriter w = new ResourceWriter(strRsrc);

					r.BasePath = UrlUtil.GetFileDirectory(strResSrc, false, true);

					foreach(DictionaryEntry de in r)
						w.AddResource((string)de.Key, de.Value);

					w.Generate();
					w.Close();
					r.Close();

					if(File.Exists(strRsrc))
					{
						plgx.CompilerParameters.EmbeddedResources.Add(strRsrc);
						Program.TempFilesPool.Add(strRsrc);
					}
				}
				else
				{
					File.Copy(strResSrc, strResFile, true);
					plgx.CompilerParameters.EmbeddedResources.Add(strResFile);
				}
			}
		}

		private static void PrepareResXFile(string strFilePath)
		{
			if(!NativeLib.IsUnix()) return;

			string[] v = File.ReadAllLines(strFilePath, Encoding.UTF8);

			// Fix directory separators in ResX file;
			// Mono's ResXResourceReader doesn't convert them
			for(int i = 0; i < (v.Length - 1); ++i)
			{
				if((v[i].IndexOf(@"<data") >= 0) && (v[i].IndexOf(
					@"System.Resources.ResXFileRef") >= 0) && (v[i + 1].IndexOf(
					@"<value>") >= 0))
				{
					v[i + 1] = UrlUtil.ConvertSeparators(v[i + 1]);
				}
			}

			File.WriteAllLines(strFilePath, v, new UTF8Encoding(false));
		}

		private static void PrepareSourceFiles(PlgxPluginInfo plgx)
		{
			if(plgx.ProjectType != PlgxProjectType.VisualBasic) return;

			string strImports = string.Empty;
			foreach(string strImport in plgx.VbImports)
				strImports += "Imports " + strImport + "\r\n";
			if(strImports.Length == 0) return;

			foreach(string strFile in plgx.SourceFiles)
			{
				if(!strFile.EndsWith(".vb", StrUtil.CaseIgnoreCmp)) continue;

				string strData = File.ReadAllText(strFile);
				File.WriteAllText(strFile, strImports + strData);
			}
		}

		private static void RunBuildCommand(string strCmd, string strTmpDir,
			string strCacheDir)
		{
			if(string.IsNullOrEmpty(strCmd)) return; // No assert

			string str = strCmd;
			if(strTmpDir != null)
				str = StrUtil.ReplaceCaseInsensitive(str, @"{PLGX_TEMP_DIR}", strTmpDir);
			if(strCacheDir != null)
				str = StrUtil.ReplaceCaseInsensitive(str, @"{PLGX_CACHE_DIR}", strCacheDir);

			// str = UrlUtil.ConvertSeparators(str);
			str = SprEngine.Compile(str, null);

			string strApp, strArgs;
			StrUtil.SplitCommandLine(str, out strApp, out strArgs);

			try
			{
				if((strArgs != null) && (strArgs.Length > 0))
					Process.Start(strApp, strArgs);
				else
					Process.Start(strApp);
			}
			catch(Exception exRun)
			{
				if(Program.CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null)
					throw new PlgxException(exRun.Message);
				throw;
			}
		}
	}
}
