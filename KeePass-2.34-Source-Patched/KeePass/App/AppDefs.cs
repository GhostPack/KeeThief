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
using System.Drawing;
using System.Windows.Forms;

using KeePass.UI;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.App
{
	public static class AppDefs
	{
		public static readonly Color ColorControlNormal = SystemColors.Window;
		public static readonly Color ColorControlDisabled = SystemColors.Control;
		public static readonly Color ColorEditError = Color.FromArgb(255, 192, 192);

		public const string XslFilesDir = "XSL";
		public const string XslFileHtmlLite = "KDBX_DetailsLite.xsl";
		public const string XslFileHtmlFull = "KDBX_DetailsFull.xsl";
		public const string XslFileHtmlTabular = "KDBX_Tabular.xsl";

		public const string PluginsDir = "Plugins";
		public const string PluginProductName = "KeePass Plugin";

		public static class FileNames
		{
			public const string Program = "KeePass.exe";
			public const string XmlSerializers = "KeePass.XmlSerializers.dll";

			public const string NativeLib32 = "KeePassLibC32.dll";
			public const string NativeLib64 = "KeePassLibC64.dll";

			public const string ShInstUtil = "ShInstUtil.exe";
		}

		// public const string MruNameValueSplitter = @"/::/";

		/// <summary>
		/// Hot key IDs (used in <c>WM_HOTKEY</c> window messages).
		/// </summary>
		public static class GlobalHotKeyId
		{
			public const int AutoType = 195;
			public const int AutoTypeSelected = 196;
			public const int ShowWindow = 226;
			public const int EntryMenu = 227;

			internal const int TempRegTest = 225;
		}

		public static class HelpTopics
		{
			public const string Acknowledgements = "base/credits";
			public const string License = "v2/license";

			public const string DatabaseSettings = "v2/dbsettings";
			public const string DbSettingsGeneral = "general";
			public const string DbSettingsSecurity = "security";
			// public const string DbSettingsProtection = "protection";
			public const string DbSettingsCompression = "compression";

			public const string AutoType = "base/autotype";
			public const string AutoTypeObfuscation = "v2/autotype_obfuscation";
			public const string AutoTypeWindowFilters = "autowindows";

			public const string Entry = "v2/entry";
			public const string EntryGeneral = "general";
			public const string EntryStrings = "advanced";
			public const string EntryAutoType = "autotype";
			public const string EntryHistory = "history";

			public const string KeySources = "base/keys";
			public const string PwGenerator = "base/pwgenerator";
			public const string IOConnections = "v2/ioconnect";
			public const string UrlField = "base/autourl";
			public const string CommandLine = "base/cmdline";
			public const string FieldRefs = "base/fieldrefs";

			public const string ImportExport = "base/importexport";
			public const string ImportExportGenericCsv = "genericcsv";
			public const string ImportExportSteganos = "imp_steganos";
			public const string ImportExportPassKeeper = "imp_passkeeper";

			public const string AppPolicy = "v2/policy";

			public const string Triggers = "v2/triggers";
			public const string TriggersEvents = "events";
			public const string TriggersConditions = "conditions";
			public const string TriggersActions = "actions";

			public const string Setup = "v2/setup";
			public const string SetupMono = "mono";

			// public const string FaqTech = "base/faq_tech";
			// public const string FaqTechMemProt = "memprot";

			public const string XmlReplace = "v2/xml_replace";
		}

		public static class CommandLineOptions
		{
			public const string Password = "pw";
			public const string KeyFile = "keyfile";
			public const string UserAccount = "useraccount";

			public const string PasswordEncrypted = "pw-enc";
			public const string PasswordStdIn = "pw-stdin";

			public const string PreSelect = "preselect";

			public const string IoCredUserName = "iousername";
			public const string IoCredPassword = "iopassword";
			public const string IoCredFromRecent = "iocredfromrecent";
			public const string IoCredIsComplete = "ioiscomplete";

			// User-friendly Pascal-case (shown in UAC dialog)
			public const string FileExtRegister = "RegisterFileExt";
			public const string FileExtUnregister = "UnregisterFileExt";

			public const string PreLoad = "preload";
			// public const string PreLoadRegister = "registerpreload";
			// public const string PreLoadUnregister = "unregisterpreload";

			public const string ExitAll = "exit-all";
			public const string Minimize = "minimize";
			public const string AutoType = "auto-type";
			public const string AutoTypeSelected = "auto-type-selected";
			public const string OpenEntryUrl = "entry-url-open";
			public const string LockAll = "lock-all";
			public const string UnlockAll = "unlock-all";
			public const string IpcEvent = "e";

			public const string Uuid = "uuid";

			public const string Help = @"?";
			public const string HelpLong = "help";

			public const string WorkaroundDisable = "wa-disable";

			public const string ConfigPathLocal = "cfg-local";

			public const string ConfigSetUrlOverride = "set-urloverride";
			public const string ConfigClearUrlOverride = "clear-urloverride";
			public const string ConfigGetUrlOverride = "get-urloverride";

			public const string ConfigSetLanguageFile = "set-languagefile";

			public const string PlgxCreate = "plgx-create";
			public const string PlgxCreateInfo = "plgx-create-info";
			public const string PlgxPrereqKP = "plgx-prereq-kp";
			public const string PlgxPrereqNet = "plgx-prereq-net";
			public const string PlgxPrereqOS = "plgx-prereq-os";
			public const string PlgxPrereqPtr = "plgx-prereq-ptr";
			public const string PlgxBuildPre = "plgx-build-pre";
			public const string PlgxBuildPost = "plgx-build-post";

			public const string Debug = "debug";
			public const string DebugThrowException = "debug-throwexcp";
			// public const string SavePluginCompileRes = "saveplgxcr"; // Now: Debug
			public const string ShowAssemblyInfo = "showasminfo";
			public const string MakeXmlSerializerEx = "makexmlserializerex";
			public const string MakeXspFile = "makexspfile";

#if DEBUG
			public const string TestGfx = "testgfx";
#endif

			public const string Version = "version"; // For Unix

			// #if (DEBUG && !KeePassLibSD)
			// public const string MakePopularPasswordTable = "makepopularpasswordtable";
			// #endif
		}

		public static class FileExtension
		{
			public const string FileExt = "kdbx";
			public const string ExtId = "kdbxfile";

			public const string KeyFile = "key";
		}

		public const string AutoRunName = "KeePass Password Safe 2";
		public const string PreLoadName = "KeePass 2 PreLoad";

		public const string MutexName = "KeePassAppMutex";
		public const string MutexNameGlobal = "KeePassAppMutexEx";

		// public const string ScriptExtension = "kps";

		public const int InvalidWindowValue = -16381;

		public static class NamedEntryColor
		{
			public static readonly Color LightRed = Color.FromArgb(255, 204, 204);
			public static readonly Color LightGreen = Color.FromArgb(204, 255, 204);
			public static readonly Color LightBlue = Color.FromArgb(153, 204, 255);
			public static readonly Color LightYellow = Color.FromArgb(255, 255, 153);
		}

		public static class FileDialogContext
		{
			// Values must not contain '@'

			public const string Database = "Database";
			public const string Sync = "Sync";
			public const string KeyFile = "KeyFile";
			public const string Import = "Import";
			public const string Export = "Export";
			public const string Attachments = "Attachments";
			public const string Xsl = "Xsl";
		}

		public const string DefaultTrlAuthor = "Dominik Reichl";
		public const string DefaultTrlContact = @"http://www.dominik-reichl.de/";

		// public const string LanguageInfoFileName = "LanguageInfo.xml";

		public const string ColumnIdnGroup = "Group";
		public const string ColumnIdnCreationTime = "CreationTime";
		public const string ColumnIdnLastModificationTime = "LastModificationTime";
		public const string ColumnIdnLastAccessTime = "LastAccessTime";
		public const string ColumnIdnExpiryTime = "ExpiryTime";
		public const string ColumnIdnUuid = "UUID";
		public const string ColumnIdnAttachment = "Attachment";

		public static string GetEntryField(PwEntry pe, string strFieldId)
		{
			if(pe == null) throw new ArgumentNullException("pe");
			if(strFieldId == null) throw new ArgumentNullException("strFieldId");

			if(strFieldId == AppDefs.ColumnIdnGroup)
				return ((pe.ParentGroup != null) ? pe.ParentGroup.Name : string.Empty);
			else if(strFieldId == AppDefs.ColumnIdnCreationTime)
				return TimeUtil.ToDisplayString(pe.CreationTime);
			else if(strFieldId == AppDefs.ColumnIdnLastModificationTime)
				return TimeUtil.ToDisplayString(pe.LastModificationTime);
			else if(strFieldId == AppDefs.ColumnIdnLastAccessTime)
				return TimeUtil.ToDisplayString(pe.LastAccessTime);
			else if(strFieldId == AppDefs.ColumnIdnExpiryTime)
				return (pe.Expires ? TimeUtil.ToDisplayString(pe.ExpiryTime) :
					KPRes.NeverExpires);
			else if(strFieldId == AppDefs.ColumnIdnUuid)
				return pe.Uuid.ToHexString();
			else if(strFieldId == AppDefs.ColumnIdnAttachment)
				return pe.Binaries.UCount.ToString();

			return pe.Strings.ReadSafe(strFieldId);
		}

		internal const string Rsa4096PublicKeyXml =
			@"<RSAKeyValue><Modulus>9Oa8Bb9if4rSYBxczLVQ3Yyae95dWQrNJ1FlqS7DoF" +
			@"RF80tD2hq84vxDE8slVeSHs68KMFnJhPsXFD6nM9oTRBaUlU/alnRTUU+X/cUXbr" +
			@"mhYN9DkJhM0OcWk5Vsl9Qxl613sA+hqIwmPc+el/fCM/1vP6JkHo/JTJ2OxQvDKN" +
			@"4cC55pHYMZt+HX6AhemsPe7ejTG7l9nN5tHGmD+GrlwuxBTddzFBARmoknFzDPWd" +
			@"QHddjuK1mXDs6lWeu73ODlSLSHMc5n0R2xMwGHN4eaiIMGzEbt0lv1aMWz+Iy1H3" +
			@"XgFgWGDHX9kx8yefmfcgFIK4Y/xHU5EyGAV68ZHPatv6i4pT4ZuecIb5GSoFzVXq" +
			@"8BZjbe+zDI+Wr1u8jLcBH0mySTWkF2gooQLvE1vgZXP1blsA7UFZSVFzYjBt36HQ" +
			@"SJLpQ9AjjB5MKpMSlvdb5SnvjzREiFVLoBsY7KH2TMz+IG1Rh3OZTGwjQKXkgRVj" +
			@"5XrEMTFRmT1zo2BHWhx8vrY6agVzqsCVqxYRbjeAhgOi6hLDMHSNAVuNg6ZHOKS8" +
			@"6x6kmBcBhGJriwY017H3Oxuhfz33ehRFX/C05egCvmR2TAXbqm+CUgrq1bZ96T/y" +
			@"s+O5uvKpe7H+EZuWb655Y9WuQSby+q0Vqqny7T6Z2NbEnI8nYHg5ZZP+TijSxeH0" +
			@"8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
	}
}
