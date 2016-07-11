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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Diagnostics;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.DataExchange;
using KeePass.Forms;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.Archive;
using KeePass.Util.XmlSerialization;
using KeePass.Ecas;
using KeePass.Plugins;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Keys;
using KeePassLib.Resources;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Translation;
using KeePassLib.Utility;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass
{
	public static class Program
	{
		private const string m_strWndMsgID = "EB2FE38E1A6A4A138CF561442F1CF25A";

		private static CommandLineArgs m_cmdLineArgs = null;
		private static Random m_rndGlobal = null;
		private static int m_nAppMessage = 0;
		private static MainForm m_formMain = null;
		private static AppConfigEx m_appConfig = null;
		private static KeyProviderPool m_keyProviderPool = null;
		private static KeyValidatorPool m_keyValidatorPool = null;
		private static FileFormatPool m_fmtPool = null;
		private static KPTranslation m_kpTranslation = new KPTranslation();
		private static TempFilesPool m_tempFilesPool = null;
		private static EcasPool m_ecasPool = null;
		private static EcasTriggerSystem m_ecasTriggers = null;
		private static CustomPwGeneratorPool m_pwGenPool = null;
		private static ColumnProviderPool m_colProvPool = null;

		private static bool m_bDesignMode = true;
#if DEBUG
		private static bool m_bDesignModeQueried = false;
#endif

		public enum AppMessage
		{
			Null = 0,
			RestoreWindow = 1,
			Exit = 2,
			IpcByFile = 3,
			AutoType = 4,
			Lock = 5,
			Unlock = 6,
			AutoTypeSelected = 7
		}

		public static CommandLineArgs CommandLineArgs
		{
			get
			{
				if(m_cmdLineArgs == null)
				{
					Debug.Assert(false);
					m_cmdLineArgs = new CommandLineArgs(null);
				}

				return m_cmdLineArgs;
			}
		}

		public static Random GlobalRandom
		{
			get { return m_rndGlobal; }
		}

		public static int ApplicationMessage
		{
			get { return m_nAppMessage; }
		}

		public static MainForm MainForm
		{
			get { return m_formMain; }
		}

		public static AppConfigEx Config
		{
			get
			{
				if(m_appConfig == null) m_appConfig = new AppConfigEx();
				return m_appConfig;
			}
		}

		public static KeyProviderPool KeyProviderPool
		{
			get
			{
				if(m_keyProviderPool == null) m_keyProviderPool = new KeyProviderPool();
				return m_keyProviderPool;
			}
		}

		public static KeyValidatorPool KeyValidatorPool
		{
			get
			{
				if(m_keyValidatorPool == null) m_keyValidatorPool = new KeyValidatorPool();
				return m_keyValidatorPool;
			}
		}

		public static FileFormatPool FileFormatPool
		{
			get
			{
				if(m_fmtPool == null) m_fmtPool = new FileFormatPool();
				return m_fmtPool;
			}
		}

		public static KPTranslation Translation
		{
			get { return m_kpTranslation; }
		}

		public static TempFilesPool TempFilesPool
		{
			get
			{
				if(m_tempFilesPool == null) m_tempFilesPool = new TempFilesPool();
				return m_tempFilesPool;
			}
		}

		public static EcasPool EcasPool // Construct on first access
		{
			get
			{
				if(m_ecasPool == null) m_ecasPool = new EcasPool(true);
				return m_ecasPool;
			}
		}

		public static EcasTriggerSystem TriggerSystem
		{
			get
			{
				if(m_ecasTriggers == null) m_ecasTriggers = new EcasTriggerSystem();
				return m_ecasTriggers;
			}
		}

		public static CustomPwGeneratorPool PwGeneratorPool
		{
			get
			{
				if(m_pwGenPool == null) m_pwGenPool = new CustomPwGeneratorPool();
				return m_pwGenPool;
			}
		}

		public static ColumnProviderPool ColumnProviderPool
		{
			get
			{
				if(m_colProvPool == null) m_colProvPool = new ColumnProviderPool();
				return m_colProvPool;
			}
		}

		public static ResourceManager Resources
		{
			get { return KeePass.Properties.Resources.ResourceManager; }
		}

		public static bool DesignMode
		{
			get
			{
#if DEBUG
				m_bDesignModeQueried = true;
#endif
				return m_bDesignMode;
			}
		}

		/// <summary>
		/// Main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
#if DEBUG
			// Program.DesignMode should not be queried before executing
			// Main (e.g. by a static Control) when running the program
			// normally
			Debug.Assert(!m_bDesignModeQueried);
#endif
			m_bDesignMode = false; // Designer doesn't call Main method

			m_cmdLineArgs = new CommandLineArgs(args);

			// Before loading the configuration
			string strWaDisable = m_cmdLineArgs[
				AppDefs.CommandLineOptions.WorkaroundDisable];
			if(!string.IsNullOrEmpty(strWaDisable))
				MonoWorkarounds.SetEnabled(strWaDisable, false);

			DpiUtil.ConfigureProcess();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.DoEvents(); // Required

#if DEBUG
			string strInitialWorkDir = WinUtil.GetWorkingDirectory();
#endif

			if(!CommonInit()) { CommonTerminate(); return; }

			if(m_appConfig.Application.Start.PluginCacheClearOnce)
			{
				PlgxCache.Clear();
				m_appConfig.Application.Start.PluginCacheClearOnce = false;
				AppConfigSerializer.Save(Program.Config);
			}

			if(m_cmdLineArgs[AppDefs.CommandLineOptions.FileExtRegister] != null)
			{
				ShellUtil.RegisterExtension(AppDefs.FileExtension.FileExt, AppDefs.FileExtension.ExtId,
					KPRes.FileExtName, WinUtil.GetExecutable(), PwDefs.ShortProductName, false);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.FileExtUnregister] != null)
			{
				ShellUtil.UnregisterExtension(AppDefs.FileExtension.FileExt, AppDefs.FileExtension.ExtId);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.PreLoad] != null)
			{
				// All important .NET assemblies are in memory now already
				try { SelfTest.Perform(); }
				catch(Exception) { Debug.Assert(false); }
				MainCleanUp();
				return;
			}
			/* if(m_cmdLineArgs[AppDefs.CommandLineOptions.PreLoadRegister] != null)
			{
				string strPreLoadPath = WinUtil.GetExecutable().Trim();
				if(strPreLoadPath.StartsWith("\"") == false)
					strPreLoadPath = "\"" + strPreLoadPath + "\"";
				ShellUtil.RegisterPreLoad(AppDefs.PreLoadName, strPreLoadPath,
					@"--" + AppDefs.CommandLineOptions.PreLoad, true);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.PreLoadUnregister] != null)
			{
				ShellUtil.RegisterPreLoad(AppDefs.PreLoadName, string.Empty,
					string.Empty, false);
				MainCleanUp();
				return;
			} */
			if((m_cmdLineArgs[AppDefs.CommandLineOptions.Help] != null) ||
				(m_cmdLineArgs[AppDefs.CommandLineOptions.HelpLong] != null))
			{
				AppHelp.ShowHelp(AppDefs.HelpTopics.CommandLine, null);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ConfigSetUrlOverride] != null)
			{
				Program.Config.Integration.UrlOverride = m_cmdLineArgs[
					AppDefs.CommandLineOptions.ConfigSetUrlOverride];
				AppConfigSerializer.Save(Program.Config);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ConfigClearUrlOverride] != null)
			{
				Program.Config.Integration.UrlOverride = string.Empty;
				AppConfigSerializer.Save(Program.Config);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ConfigGetUrlOverride] != null)
			{
				try
				{
					string strFileOut = UrlUtil.EnsureTerminatingSeparator(
						UrlUtil.GetTempPath(), false) + "KeePass_UrlOverride.tmp";
					string strContent = ("[KeePass]\r\nKeeURLOverride=" +
						Program.Config.Integration.UrlOverride + "\r\n");
					File.WriteAllText(strFileOut, strContent);
				}
				catch(Exception) { Debug.Assert(false); }
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ConfigSetLanguageFile] != null)
			{
				Program.Config.Application.LanguageFile = m_cmdLineArgs[
					AppDefs.CommandLineOptions.ConfigSetLanguageFile];
				AppConfigSerializer.Save(Program.Config);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.PlgxCreate] != null)
			{
				PlgxPlugin.CreateFromCommandLine();
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.PlgxCreateInfo] != null)
			{
				PlgxPlugin.CreateInfoFile(m_cmdLineArgs.FileName);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ShowAssemblyInfo] != null)
			{
				MessageService.ShowInfo(Assembly.GetExecutingAssembly().ToString());
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.MakeXmlSerializerEx] != null)
			{
				XmlSerializerEx.GenerateSerializers(m_cmdLineArgs);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.MakeXspFile] != null)
			{
				XspArchive.CreateFile(m_cmdLineArgs.FileName, m_cmdLineArgs["d"]);
				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.Version] != null)
			{
				Console.WriteLine(PwDefs.ShortProductName + " " + PwDefs.VersionString);
				Console.WriteLine(PwDefs.Copyright);
				MainCleanUp();
				return;
			}
#if DEBUG
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.TestGfx] != null)
			{
				List<Image> lImg = new List<Image>();
				lImg.Add(Properties.Resources.B16x16_Browser);
				lImg.Add(Properties.Resources.B48x48_Keyboard_Layout);
				ImageArchive aHighRes = new ImageArchive();
				aHighRes.Load(Properties.Resources.Images_Client_HighRes);
				lImg.Add(aHighRes.GetForObject("C12_IRKickFlash"));
				if(File.Exists("Test.png"))
					lImg.Add(Image.FromFile("Test.png"));
				Image img = GfxUtil.ScaleTest(lImg.ToArray());
				img.Save("GfxScaleTest.png", ImageFormat.Png);
				return;
			}
#endif
			// #if (DEBUG && !KeePassLibSD)
			// if(m_cmdLineArgs[AppDefs.CommandLineOptions.MakePopularPasswordTable] != null)
			// {
			//	PopularPasswords.MakeList();
			//	MainCleanUp();
			//	return;
			// }
			// #endif

			try { m_nAppMessage = NativeMethods.RegisterWindowMessage(m_strWndMsgID); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }

			if(m_cmdLineArgs[AppDefs.CommandLineOptions.ExitAll] != null)
			{
				BroadcastAppMessageAndCleanUp(AppMessage.Exit);
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.AutoType] != null)
			{
				BroadcastAppMessageAndCleanUp(AppMessage.AutoType);
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.AutoTypeSelected] != null)
			{
				BroadcastAppMessageAndCleanUp(AppMessage.AutoTypeSelected);
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.OpenEntryUrl] != null)
			{
				string strEntryUuid = m_cmdLineArgs[AppDefs.CommandLineOptions.Uuid];
				if(!string.IsNullOrEmpty(strEntryUuid))
				{
					IpcParamEx ipUrl = new IpcParamEx(IpcUtilEx.CmdOpenEntryUrl,
						strEntryUuid, null, null, null, null);
					IpcUtilEx.SendGlobalMessage(ipUrl);
				}

				MainCleanUp();
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.LockAll] != null)
			{
				BroadcastAppMessageAndCleanUp(AppMessage.Lock);
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.UnlockAll] != null)
			{
				BroadcastAppMessageAndCleanUp(AppMessage.Unlock);
				return;
			}
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.IpcEvent] != null)
			{
				string strName = m_cmdLineArgs[AppDefs.CommandLineOptions.IpcEvent];
				if(!string.IsNullOrEmpty(strName))
				{
					string[] vFlt = KeyUtil.MakeCtxIndependent(args);

					IpcParamEx ipEvent = new IpcParamEx(IpcUtilEx.CmdIpcEvent, strName,
						CommandLineArgs.SafeSerialize(vFlt), null, null, null);
					IpcUtilEx.SendGlobalMessage(ipEvent);
				}

				MainCleanUp();
				return;
			}

			// Mutex mSingleLock = TrySingleInstanceLock(AppDefs.MutexName, true);
			bool bSingleLock = GlobalMutexPool.CreateMutex(AppDefs.MutexName, true);
			// if((mSingleLock == null) && m_appConfig.Integration.LimitToSingleInstance)
			if(!bSingleLock && m_appConfig.Integration.LimitToSingleInstance)
			{
				ActivatePreviousInstance(args);
				MainCleanUp();
				return;
			}

			Mutex mGlobalNotify = TryGlobalInstanceNotify(AppDefs.MutexNameGlobal);

			AutoType.InitStatic();

			UserActivityNotifyFilter nfActivity = new UserActivityNotifyFilter();
			Application.AddMessageFilter(nfActivity);

#if DEBUG
			if(m_cmdLineArgs[AppDefs.CommandLineOptions.DebugThrowException] != null)
				throw new Exception(AppDefs.CommandLineOptions.DebugThrowException);

			m_formMain = new MainForm();
			Application.Run(m_formMain);
#else
			try
			{
				if(m_cmdLineArgs[AppDefs.CommandLineOptions.DebugThrowException] != null)
					throw new Exception(AppDefs.CommandLineOptions.DebugThrowException);

				m_formMain = new MainForm();
				Application.Run(m_formMain);
			}
			catch(Exception exPrg)
			{
				// Catch message box exception;
				// https://sourceforge.net/p/keepass/patches/86/
				try { MessageService.ShowFatal(exPrg); }
				catch(Exception) { Console.Error.WriteLine(exPrg.ToString()); }
			}
#endif

			Application.RemoveMessageFilter(nfActivity);

			Debug.Assert(GlobalWindowManager.WindowCount == 0);
			Debug.Assert(MessageService.CurrentMessageCount == 0);

			MainCleanUp();

#if DEBUG
			string strEndWorkDir = WinUtil.GetWorkingDirectory();
			Debug.Assert(strEndWorkDir.Equals(strInitialWorkDir, StrUtil.CaseIgnoreCmp));
#endif

			if(mGlobalNotify != null) { GC.KeepAlive(mGlobalNotify); }
			// if(mSingleLock != null) { GC.KeepAlive(mSingleLock); }
		}

		/// <summary>
		/// Common program initialization function that can also be
		/// used by applications that use KeePass as a library
		/// (like e.g. KPScript).
		/// </summary>
		public static bool CommonInit()
		{
			m_bDesignMode = false; // Again, for the ones not calling Main

			int nRandomSeed = (int)DateTime.UtcNow.Ticks;
			// Prevent overflow (see Random class constructor)
			if(nRandomSeed == int.MinValue) nRandomSeed = 17;
			m_rndGlobal = new Random(nRandomSeed);

			InitEnvSecurity();

			// try { NativeMethods.SetProcessDPIAware(); }
			// catch(Exception) { }

			try { SelfTest.TestFipsComplianceProblems(); }
			catch(Exception exFips)
			{
				MessageService.ShowWarning(KPRes.SelfTestFailed, exFips);
				return false;
			}

			// Set global localized strings
			PwDatabase.LocalizedAppName = PwDefs.ShortProductName;
			KdbxFile.DetermineLanguageId();

			m_appConfig = AppConfigSerializer.Load();
			if(m_appConfig.Logging.Enabled)
				AppLogEx.Open(PwDefs.ShortProductName);

			AppPolicy.Current = m_appConfig.Security.Policy.CloneDeep();

			m_appConfig.Apply(AceApplyFlags.All);

			m_ecasTriggers = m_appConfig.Application.TriggerSystem;
			m_ecasTriggers.SetToInitialState();

			string strHelpFile = UrlUtil.StripExtension(WinUtil.GetExecutable()) + ".chm";
			AppHelp.LocalHelpFile = strHelpFile;

			// InitEnvWorkarounds();
			LoadTranslation();

			CustomResourceManager.Override(typeof(KeePass.Properties.Resources));

			return true;
		}

		public static void CommonTerminate()
		{
#if DEBUG
			Debug.Assert(ShutdownBlocker.Instance == null);
			Debug.Assert(!SendInputEx.IsSending);
			// GC.Collect(); // Force invocation of destructors
#endif

			AppLogEx.Close();

			if(m_tempFilesPool != null)
			{
				m_tempFilesPool.Clear(TempClearFlags.All);
				m_tempFilesPool.WaitForThreads();
			}

			EnableThemingInScope.StaticDispose();
		}

		private static void MainCleanUp()
		{
			IpcBroadcast.StopServer();

			EntryMenu.Destroy();

			GlobalMutexPool.ReleaseAll();

			CommonTerminate();
		}

		private static void InitEnvSecurity()
		{
			try
			{
				// Do not load libraries from the current working directory
				if(!NativeMethods.SetDllDirectory(string.Empty)) { Debug.Assert(false); }
			}
			catch(Exception) { } // Throws on Unix and Windows < XP SP1
		}

		// internal static Mutex TrySingleInstanceLock(string strName, bool bInitiallyOwned)
		// {
		//	if(strName == null) throw new ArgumentNullException("strName");
		//	try
		//	{
		//		bool bCreatedNew;
		//		Mutex mSingleLock = new Mutex(bInitiallyOwned, strName, out bCreatedNew);
		//		if(!bCreatedNew) return null;
		//		return mSingleLock;
		//	}
		//	catch(Exception) { }
		//	return null;
		// }

		internal static Mutex TryGlobalInstanceNotify(string strBaseName)
		{
			if(strBaseName == null) throw new ArgumentNullException("strBaseName");

			try
			{
				string strName = "Global\\" + strBaseName;
				string strIdentity = Environment.UserDomainName + "\\" +
					Environment.UserName;
				MutexSecurity ms = new MutexSecurity();

				MutexAccessRule mar = new MutexAccessRule(strIdentity,
					MutexRights.FullControl, AccessControlType.Allow);
				ms.AddAccessRule(mar);

				SecurityIdentifier sid = new SecurityIdentifier(
					WellKnownSidType.WorldSid, null);
				mar = new MutexAccessRule(sid, MutexRights.ReadPermissions |
					MutexRights.Synchronize, AccessControlType.Allow);
				ms.AddAccessRule(mar);

				bool bCreatedNew;
				return new Mutex(false, strName, out bCreatedNew, ms);
			}
			catch(Exception) { } // Windows 9x and Mono 2.0+ (AddAccessRule) throw

			return null;
		}

		// internal static void DestroyMutex(Mutex m, bool bReleaseFirst)
		// {
		//	if(m == null) return;
		//	if(bReleaseFirst)
		//	{
		//		try { m.ReleaseMutex(); }
		//		catch(Exception) { Debug.Assert(false); }
		//	}
		//	try { m.Close(); }
		//	catch(Exception) { Debug.Assert(false); }
		// }

		private static void ActivatePreviousInstance(string[] args)
		{
			if((m_nAppMessage == 0) && !NativeLib.IsUnix())
			{
				Debug.Assert(false);
				return;
			}

			try
			{
				if(string.IsNullOrEmpty(m_cmdLineArgs.FileName))
				{
					// NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST,
					//	m_nAppMessage, (IntPtr)AppMessage.RestoreWindow, IntPtr.Zero);
					IpcBroadcast.Send(AppMessage.RestoreWindow, 0, false);
				}
				else
				{
					string[] vFlt = KeyUtil.MakeCtxIndependent(args);

					IpcParamEx ipcMsg = new IpcParamEx(IpcUtilEx.CmdOpenDatabase,
						CommandLineArgs.SafeSerialize(vFlt), null, null, null, null);

					IpcUtilEx.SendGlobalMessage(ipcMsg);
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		// For plugins
		public static void NotifyUserActivity()
		{
			if(Program.m_formMain != null) Program.m_formMain.NotifyUserActivity();
		}

		public static IntPtr GetSafeMainWindowHandle()
		{
			if(m_formMain == null) return IntPtr.Zero;

			try { return m_formMain.Handle; }
			catch(Exception) { Debug.Assert(false); }

			return IntPtr.Zero;
		}

		private static void BroadcastAppMessageAndCleanUp(AppMessage msg)
		{
			try
			{
				// NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST,
				//	m_nAppMessage, (IntPtr)msg, IntPtr.Zero);
				IpcBroadcast.Send(msg, 0, false);
			}
			catch(Exception) { Debug.Assert(false); }

			MainCleanUp();
		}

		private static void LoadTranslation()
		{
			string strLangFile = m_appConfig.Application.LanguageFile;
			if(string.IsNullOrEmpty(strLangFile)) return;

			string[] vLangDirs = new string[]{
				AppConfigSerializer.AppDataDirectory,
				AppConfigSerializer.LocalAppDataDirectory,
				UrlUtil.GetFileDirectory(WinUtil.GetExecutable(), false, false)
			};

			foreach(string strLangDir in vLangDirs)
			{
				string strLangPath = UrlUtil.EnsureTerminatingSeparator(
					strLangDir, false) + strLangFile;

				try
				{
					// Performance optimization
					if(!File.Exists(strLangPath)) continue;

					XmlSerializerEx xs = new XmlSerializerEx(typeof(KPTranslation));
					m_kpTranslation = KPTranslation.Load(strLangPath, xs);

					KPRes.SetTranslatedStrings(
						m_kpTranslation.SafeGetStringTableDictionary(
						"KeePass.Resources.KPRes"));
					KLRes.SetTranslatedStrings(
						m_kpTranslation.SafeGetStringTableDictionary(
						"KeePassLib.Resources.KLRes"));

					StrUtil.RightToLeft = m_kpTranslation.Properties.RightToLeft;
					break;
				}
				// catch(DirectoryNotFoundException) { } // Ignore
				// catch(FileNotFoundException) { } // Ignore
				catch(Exception) { Debug.Assert(false); }
			}
		}

		internal static bool IsDevelopmentSnapshot()
		{
			try
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				byte[] pk = asm.GetName().GetPublicKeyToken();
				string strPk = MemUtil.ByteArrayToHexString(pk);
				return !strPk.Equals("fed2ed7716aecf5c", StrUtil.CaseIgnoreCmp);
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		/* private static void InitEnvWorkarounds()
		{
			InitFtpWorkaround();
		} */

		/* private static void InitFtpWorkaround()
		{
			// http://support.microsoft.com/kb/2134299
			// https://connect.microsoft.com/VisualStudio/feedback/details/621450/problem-renaming-file-on-ftp-server-using-ftpwebrequest-in-net-framework-4-0-vs2010-only
			try
			{
				if((Environment.Version.Major >= 4) && !NativeLib.IsUnix())
				{
					Type tFtp = typeof(FtpWebRequest);

					Assembly asm = Assembly.GetAssembly(tFtp);
					Type tFlags = asm.GetType("System.Net.FtpMethodFlags");
					Debug.Assert(Enum.GetUnderlyingType(tFlags) == typeof(int));
					int iAdd = (int)Enum.Parse(tFlags, "MustChangeWorkingDirectoryToPath");
					Debug.Assert(iAdd == 0x100);

					FieldInfo fiMethod = tFtp.GetField("m_MethodInfo",
						BindingFlags.Instance | BindingFlags.NonPublic);
					if(fiMethod == null) { Debug.Assert(false); return; }
					Type tMethod = fiMethod.FieldType;

					FieldInfo fiKnown = tMethod.GetField("KnownMethodInfo",
						BindingFlags.Static | BindingFlags.NonPublic);
					if(fiKnown == null) { Debug.Assert(false); return; }
					Array arKnown = (Array)fiKnown.GetValue(null);

					FieldInfo fiFlags = tMethod.GetField("Flags",
						BindingFlags.Instance | BindingFlags.NonPublic);
					if(fiFlags == null) { Debug.Assert(false); return; }

					foreach(object oKnown in arKnown)
					{
						int i = (int)fiFlags.GetValue(oKnown);
						i |= iAdd;
						fiFlags.SetValue(oKnown, i);
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		} */
	}
}
