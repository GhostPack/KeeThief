// This is a generated file!
// Do not edit manually, changes will be overwritten.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

using KeePassLib.Interfaces;

namespace KeePass.Util.XmlSerialization
{
	public sealed partial class XmlSerializerEx : IXmlSerializerEx
	{
		private static char[] m_vEnumSeps = new char[] {
			' ', '\t', '\r', '\n', '|', ',', ';', ':'
		};

		private static KeePass.App.Configuration.AppConfigEx ReadAppConfigEx(XmlReader xr)
		{
			KeePass.App.Configuration.AppConfigEx o = new KeePass.App.Configuration.AppConfigEx();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Meta":
						o.Meta = ReadAceMeta(xr);
						break;
					case "Application":
						o.Application = ReadAceApplication(xr);
						break;
					case "Logging":
						o.Logging = ReadAceLogging(xr);
						break;
					case "MainWindow":
						o.MainWindow = ReadAceMainWindow(xr);
						break;
					case "UI":
						o.UI = ReadAceUI(xr);
						break;
					case "Security":
						o.Security = ReadAceSecurity(xr);
						break;
					case "Native":
						o.Native = ReadAceNative(xr);
						break;
					case "PasswordGenerator":
						o.PasswordGenerator = ReadAcePasswordGenerator(xr);
						break;
					case "Defaults":
						o.Defaults = ReadAceDefaults(xr);
						break;
					case "Integration":
						o.Integration = ReadAceIntegration(xr);
						break;
					case "Custom":
						o.CustomSerialized = ReadArrayOfAceKvp(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KPTranslation ReadKPTranslation(XmlReader xr)
		{
			KeePassLib.Translation.KPTranslation o = new KeePassLib.Translation.KPTranslation();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Properties":
						o.Properties = ReadKPTranslationProperties(xr);
						break;
					case "StringTables":
						o.StringTables = ReadListOfKPStringTable(xr);
						break;
					case "Forms":
						o.Forms = ReadListOfKPFormCustomization(xr);
						break;
					case "UnusedText":
						o.UnusedText = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceMeta ReadAceMeta(XmlReader xr)
		{
			KeePass.App.Configuration.AceMeta o = new KeePass.App.Configuration.AceMeta();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "PreferUserConfiguration":
						o.PreferUserConfiguration = ReadBoolean(xr);
						break;
					case "OmitItemsWithDefaultValues":
						o.OmitItemsWithDefaultValues = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceApplication ReadAceApplication(XmlReader xr)
		{
			KeePass.App.Configuration.AceApplication o = new KeePass.App.Configuration.AceApplication();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "LanguageFile":
						o.LanguageFile = ReadString(xr);
						break;
					case "HelpUseLocal":
						o.HelpUseLocal = ReadBoolean(xr);
						break;
					case "LastUpdateCheck":
						o.LastUpdateCheck = ReadString(xr);
						break;
					case "LastUsedFile":
						o.LastUsedFile = ReadIOConnectionInfo(xr);
						break;
					case "MostRecentlyUsed":
						o.MostRecentlyUsed = ReadAceMru(xr);
						break;
					case "RememberWorkingDirectories":
						o.RememberWorkingDirectories = ReadBoolean(xr);
						break;
					case "WorkingDirectories":
						o.WorkingDirectoriesSerialized = ReadArrayOfString(xr);
						break;
					case "Start":
						o.Start = ReadAceStartUp(xr);
						break;
					case "FileOpening":
						o.FileOpening = ReadAceOpenDb(xr);
						break;
					case "VerifyWrittenFileAfterSaving":
						o.VerifyWrittenFileAfterSaving = ReadBoolean(xr);
						break;
					case "UseTransactedFileWrites":
						o.UseTransactedFileWrites = ReadBoolean(xr);
						break;
					case "FileTxExtra":
						o.FileTxExtra = ReadBoolean(xr);
						break;
					case "UseFileLocks":
						o.UseFileLocks = ReadBoolean(xr);
						break;
					case "SaveForceSync":
						o.SaveForceSync = ReadBoolean(xr);
						break;
					case "FileClosing":
						o.FileClosing = ReadAceCloseDb(xr);
						break;
					case "TriggerSystem":
						o.TriggerSystem = ReadEcasTriggerSystem(xr);
						break;
					case "PluginCachePath":
						o.PluginCachePath = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceLogging ReadAceLogging(XmlReader xr)
		{
			KeePass.App.Configuration.AceLogging o = new KeePass.App.Configuration.AceLogging();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Enabled":
						o.Enabled = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceMainWindow ReadAceMainWindow(XmlReader xr)
		{
			KeePass.App.Configuration.AceMainWindow o = new KeePass.App.Configuration.AceMainWindow();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "X":
						o.X = ReadInt32(xr);
						break;
					case "Y":
						o.Y = ReadInt32(xr);
						break;
					case "Width":
						o.Width = ReadInt32(xr);
						break;
					case "Height":
						o.Height = ReadInt32(xr);
						break;
					case "Maximized":
						o.Maximized = ReadBoolean(xr);
						break;
					case "SplitterHorizontalFrac":
						o.SplitterHorizontalFrac = ReadDouble(xr);
						break;
					case "SplitterVerticalFrac":
						o.SplitterVerticalFrac = ReadDouble(xr);
						break;
					case "Layout":
						o.Layout = ReadAceMainWindowLayout(xr);
						break;
					case "AlwaysOnTop":
						o.AlwaysOnTop = ReadBoolean(xr);
						break;
					case "CloseButtonMinimizesWindow":
						o.CloseButtonMinimizesWindow = ReadBoolean(xr);
						break;
					case "EscMinimizesToTray":
						o.EscMinimizesToTray = ReadBoolean(xr);
						break;
					case "MinimizeToTray":
						o.MinimizeToTray = ReadBoolean(xr);
						break;
					case "ShowFullPathInTitle":
						o.ShowFullPathInTitle = ReadBoolean(xr);
						break;
					case "DropToBackAfterClipboardCopy":
						o.DropToBackAfterClipboardCopy = ReadBoolean(xr);
						break;
					case "MinimizeAfterClipboardCopy":
						o.MinimizeAfterClipboardCopy = ReadBoolean(xr);
						break;
					case "MinimizeAfterLocking":
						o.MinimizeAfterLocking = ReadBoolean(xr);
						break;
					case "MinimizeAfterOpeningDatabase":
						o.MinimizeAfterOpeningDatabase = ReadBoolean(xr);
						break;
					case "QuickFindSearchInPasswords":
						o.QuickFindSearchInPasswords = ReadBoolean(xr);
						break;
					case "QuickFindExcludeExpired":
						o.QuickFindExcludeExpired = ReadBoolean(xr);
						break;
					case "QuickFindDerefData":
						o.QuickFindDerefData = ReadBoolean(xr);
						break;
					case "FocusResultsAfterQuickFind":
						o.FocusResultsAfterQuickFind = ReadBoolean(xr);
						break;
					case "FocusQuickFindOnRestore":
						o.FocusQuickFindOnRestore = ReadBoolean(xr);
						break;
					case "FocusQuickFindOnUntray":
						o.FocusQuickFindOnUntray = ReadBoolean(xr);
						break;
					case "CopyUrlsInsteadOfOpening":
						o.CopyUrlsInsteadOfOpening = ReadBoolean(xr);
						break;
					case "EntrySelGroupSel":
						o.EntrySelGroupSel = ReadBoolean(xr);
						break;
					case "DisableSaveIfNotModified":
						o.DisableSaveIfNotModified = ReadBoolean(xr);
						break;
					case "ToolBar":
						o.ToolBar = ReadAceToolBar(xr);
						break;
					case "EntryView":
						o.EntryView = ReadAceEntryView(xr);
						break;
					case "TanView":
						o.TanView = ReadAceTanView(xr);
						break;
					case "EntryListColumnCollection":
						o.EntryListColumns = ReadListOfAceColumn(xr);
						break;
					case "EntryListColumnDisplayOrder":
						o.EntryListColumnDisplayOrder = ReadString(xr);
						break;
					case "EntryListAutoResizeColumns":
						o.EntryListAutoResizeColumns = ReadBoolean(xr);
						break;
					case "EntryListAlternatingBgColors":
						o.EntryListAlternatingBgColors = ReadBoolean(xr);
						break;
					case "EntryListShowDerefData":
						o.EntryListShowDerefData = ReadBoolean(xr);
						break;
					case "EntryListShowDerefDataAsync":
						o.EntryListShowDerefDataAsync = ReadBoolean(xr);
						break;
					case "EntryListShowDerefDataAndRefs":
						o.EntryListShowDerefDataAndRefs = ReadBoolean(xr);
						break;
					case "ListSorting":
						o.ListSorting = ReadListSorter(xr);
						break;
					case "ListGrouping":
						o.ListGrouping = ReadInt32(xr);
						break;
					case "ShowEntriesOfSubGroups":
						o.ShowEntriesOfSubGroups = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceUI ReadAceUI(XmlReader xr)
		{
			KeePass.App.Configuration.AceUI o = new KeePass.App.Configuration.AceUI();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "TrayIcon":
						o.TrayIcon = ReadAceTrayIcon(xr);
						break;
					case "Hiding":
						o.Hiding = ReadAceHiding(xr);
						break;
					case "RepeatPasswordOnlyWhenHidden":
						o.RepeatPasswordOnlyWhenHidden = ReadBoolean(xr);
						break;
					case "StandardFont":
						o.StandardFont = ReadAceFont(xr);
						break;
					case "PasswordFont":
						o.PasswordFont = ReadAceFont(xr);
						break;
					case "ForceSystemFontUnix":
						o.ForceSystemFontUnix = ReadBoolean(xr);
						break;
					case "BannerStyle":
						o.BannerStyle = ReadBannerStyle(xr);
						break;
					case "ShowImportStatusDialog":
						o.ShowImportStatusDialog = ReadBoolean(xr);
						break;
					case "ShowDbMntncResultsDialog":
						o.ShowDbMntncResultsDialog = ReadBoolean(xr);
						break;
					case "ShowRecycleConfirmDialog":
						o.ShowRecycleConfirmDialog = ReadBoolean(xr);
						break;
					case "ToolStripRenderer":
						o.ToolStripRenderer = ReadString(xr);
						break;
					case "OptimizeForScreenReader":
						o.OptimizeForScreenReader = ReadBoolean(xr);
						break;
					case "DataEditorRect":
						o.DataEditorRect = ReadString(xr);
						break;
					case "DataEditorFont":
						o.DataEditorFont = ReadAceFont(xr);
						break;
					case "DataEditorWordWrap":
						o.DataEditorWordWrap = ReadBoolean(xr);
						break;
					case "CharPickerRect":
						o.CharPickerRect = ReadString(xr);
						break;
					case "AutoTypeCtxRect":
						o.AutoTypeCtxRect = ReadString(xr);
						break;
					case "AutoTypeCtxFlags":
						o.AutoTypeCtxFlags = ReadInt64(xr);
						break;
					case "AutoTypeCtxColumnWidths":
						o.AutoTypeCtxColumnWidths = ReadString(xr);
						break;
					case "UIFlags":
						o.UIFlags = ReadUInt64(xr);
						break;
					case "KeyCreationFlags":
						o.KeyCreationFlags = ReadUInt64(xr);
						break;
					case "KeyPromptFlags":
						o.KeyPromptFlags = ReadUInt64(xr);
						break;
					case "SecureDesktopPlaySound":
						o.SecureDesktopPlaySound = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceSecurity ReadAceSecurity(XmlReader xr)
		{
			KeePass.App.Configuration.AceSecurity o = new KeePass.App.Configuration.AceSecurity();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "WorkspaceLocking":
						o.WorkspaceLocking = ReadAceWorkspaceLocking(xr);
						break;
					case "Policy":
						o.Policy = ReadAppPolicyFlags(xr);
						break;
					case "MasterPassword":
						o.MasterPassword = ReadAceMasterPassword(xr);
						break;
					case "MasterKeyTries":
						o.MasterKeyTries = ReadInt32(xr);
						break;
					case "MasterKeyOnSecureDesktop":
						o.MasterKeyOnSecureDesktop = ReadBoolean(xr);
						break;
					case "ClipboardClearOnExit":
						o.ClipboardClearOnExit = ReadBoolean(xr);
						break;
					case "ClipboardClearAfterSeconds":
						o.ClipboardClearAfterSeconds = ReadInt32(xr);
						break;
					case "UseClipboardViewerIgnoreFormat":
						o.UseClipboardViewerIgnoreFormat = ReadBoolean(xr);
						break;
					case "ClearKeyCommandLineParams":
						o.ClearKeyCommandLineParams = ReadBoolean(xr);
						break;
					case "SslCertsAcceptInvalid":
						o.SslCertsAcceptInvalid = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceNative ReadAceNative(XmlReader xr)
		{
			KeePass.App.Configuration.AceNative o = new KeePass.App.Configuration.AceNative();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "NativeKeyTransformations":
						o.NativeKeyTransformations = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AcePasswordGenerator ReadAcePasswordGenerator(XmlReader xr)
		{
			KeePass.App.Configuration.AcePasswordGenerator o = new KeePass.App.Configuration.AcePasswordGenerator();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "AutoGeneratedPasswordsProfile":
						o.AutoGeneratedPasswordsProfile = ReadPwProfile(xr);
						break;
					case "LastUsedProfile":
						o.LastUsedProfile = ReadPwProfile(xr);
						break;
					case "UserProfiles":
						o.UserProfiles = ReadListOfPwProfile(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceDefaults ReadAceDefaults(XmlReader xr)
		{
			KeePass.App.Configuration.AceDefaults o = new KeePass.App.Configuration.AceDefaults();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "NewEntryExpiresInDays":
						o.NewEntryExpiresInDays = ReadInt32(xr);
						break;
					case "OptionsTabIndex":
						o.OptionsTabIndex = ReadUInt32(xr);
						break;
					case "TanCharacters":
						o.TanCharacters = ReadString(xr);
						break;
					case "TanExpiresOnUse":
						o.TanExpiresOnUse = ReadBoolean(xr);
						break;
					case "SearchParameters":
						o.SearchParameters = ReadSearchParameters(xr);
						break;
					case "FileSaveAsDirectory":
						o.FileSaveAsDirectory = ReadString(xr);
						break;
					case "RememberKeySources":
						o.RememberKeySources = ReadBoolean(xr);
						break;
					case "KeySources":
						o.KeySources = ReadListOfAceKeyAssoc(xr);
						break;
					case "CustomColors":
						o.CustomColors = ReadString(xr);
						break;
					case "WinFavsBaseFolderName":
						o.WinFavsBaseFolderName = ReadString(xr);
						break;
					case "WinFavsFileNamePrefix":
						o.WinFavsFileNamePrefix = ReadString(xr);
						break;
					case "WinFavsFileNameSuffix":
						o.WinFavsFileNameSuffix = ReadString(xr);
						break;
					case "RecycleBinCollapse":
						o.RecycleBinCollapse = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceIntegration ReadAceIntegration(XmlReader xr)
		{
			KeePass.App.Configuration.AceIntegration o = new KeePass.App.Configuration.AceIntegration();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "HotKeyGlobalAutoType":
						o.HotKeyGlobalAutoType = ReadUInt64(xr);
						break;
					case "HotKeySelectedAutoType":
						o.HotKeySelectedAutoType = ReadUInt64(xr);
						break;
					case "HotKeyShowWindow":
						o.HotKeyShowWindow = ReadUInt64(xr);
						break;
					case "HotKeyEntryMenu":
						o.HotKeyEntryMenu = ReadUInt64(xr);
						break;
					case "UrlOverride":
						o.UrlOverride = ReadString(xr);
						break;
					case "UrlSchemeOverrides":
						o.UrlSchemeOverrides = ReadAceUrlSchemeOverrides(xr);
						break;
					case "SearchKeyFiles":
						o.SearchKeyFiles = ReadBoolean(xr);
						break;
					case "SearchKeyFilesOnRemovableMedia":
						o.SearchKeyFilesOnRemovableMedia = ReadBoolean(xr);
						break;
					case "LimitToSingleInstance":
						o.LimitToSingleInstance = ReadBoolean(xr);
						break;
					case "AutoTypeMatchByTitle":
						o.AutoTypeMatchByTitle = ReadBoolean(xr);
						break;
					case "AutoTypeMatchByUrlInTitle":
						o.AutoTypeMatchByUrlInTitle = ReadBoolean(xr);
						break;
					case "AutoTypeMatchByUrlHostInTitle":
						o.AutoTypeMatchByUrlHostInTitle = ReadBoolean(xr);
						break;
					case "AutoTypeMatchByTagInTitle":
						o.AutoTypeMatchByTagInTitle = ReadBoolean(xr);
						break;
					case "AutoTypeExpiredCanMatch":
						o.AutoTypeExpiredCanMatch = ReadBoolean(xr);
						break;
					case "AutoTypeAlwaysShowSelDialog":
						o.AutoTypeAlwaysShowSelDialog = ReadBoolean(xr);
						break;
					case "AutoTypePrependInitSequenceForIE":
						o.AutoTypePrependInitSequenceForIE = ReadBoolean(xr);
						break;
					case "AutoTypeReleaseAltWithKeyPress":
						o.AutoTypeReleaseAltWithKeyPress = ReadBoolean(xr);
						break;
					case "AutoTypeAdjustKeyboardLayout":
						o.AutoTypeAdjustKeyboardLayout = ReadBoolean(xr);
						break;
					case "AutoTypeAllowInterleaved":
						o.AutoTypeAllowInterleaved = ReadBoolean(xr);
						break;
					case "AutoTypeCancelOnWindowChange":
						o.AutoTypeCancelOnWindowChange = ReadBoolean(xr);
						break;
					case "AutoTypeCancelOnTitleChange":
						o.AutoTypeCancelOnTitleChange = ReadBoolean(xr);
						break;
					case "AutoTypeInterKeyDelay":
						o.AutoTypeInterKeyDelay = ReadInt32(xr);
						break;
					case "ProxyType":
						o.ProxyType = ReadProxyServerType(xr);
						break;
					case "ProxyAddress":
						o.ProxyAddress = ReadString(xr);
						break;
					case "ProxyPort":
						o.ProxyPort = ReadString(xr);
						break;
					case "ProxyAuthType":
						o.ProxyAuthType = ReadProxyAuthType(xr);
						break;
					case "ProxyUserName":
						o.ProxyUserName = ReadString(xr);
						break;
					case "ProxyPassword":
						o.ProxyPassword = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceKvp[] ReadArrayOfAceKvp(XmlReader xr)
		{
			List<KeePass.App.Configuration.AceKvp> l = new List<KeePass.App.Configuration.AceKvp>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.App.Configuration.AceKvp oElem = ReadAceKvp(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePassLib.Translation.KPTranslationProperties ReadKPTranslationProperties(XmlReader xr)
		{
			KeePassLib.Translation.KPTranslationProperties o = new KeePassLib.Translation.KPTranslationProperties();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Application":
						o.Application = ReadString(xr);
						break;
					case "ApplicationVersion":
						o.ApplicationVersion = ReadString(xr);
						break;
					case "NameEnglish":
						o.NameEnglish = ReadString(xr);
						break;
					case "NameNative":
						o.NameNative = ReadString(xr);
						break;
					case "Iso6391Code":
						o.Iso6391Code = ReadString(xr);
						break;
					case "RightToLeft":
						o.RightToLeft = ReadBoolean(xr);
						break;
					case "AuthorName":
						o.AuthorName = ReadString(xr);
						break;
					case "AuthorContact":
						o.AuthorContact = ReadString(xr);
						break;
					case "Generator":
						o.Generator = ReadString(xr);
						break;
					case "FileUuid":
						o.FileUuid = ReadString(xr);
						break;
					case "LastModified":
						o.LastModified = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePassLib.Translation.KPStringTable> ReadListOfKPStringTable(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Translation.KPStringTable> o = new System.Collections.Generic.List<KeePassLib.Translation.KPStringTable>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Translation.KPStringTable oElem = ReadKPStringTable(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePassLib.Translation.KPFormCustomization> ReadListOfKPFormCustomization(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Translation.KPFormCustomization> o = new System.Collections.Generic.List<KeePassLib.Translation.KPFormCustomization>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Translation.KPFormCustomization oElem = ReadKPFormCustomization(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.String ReadString(XmlReader xr)
		{
			return xr.ReadElementString();
		}

		private static System.Boolean ReadBoolean(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToBoolean(strValue);
		}

		private static KeePassLib.Serialization.IOConnectionInfo ReadIOConnectionInfo(XmlReader xr)
		{
			KeePassLib.Serialization.IOConnectionInfo o = new KeePassLib.Serialization.IOConnectionInfo();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Path":
						o.Path = ReadString(xr);
						break;
					case "UserName":
						o.UserName = ReadString(xr);
						break;
					case "Password":
						o.Password = ReadString(xr);
						break;
					case "CredProtMode":
						o.CredProtMode = ReadIOCredProtMode(xr);
						break;
					case "CredSaveMode":
						o.CredSaveMode = ReadIOCredSaveMode(xr);
						break;
					case "PropertiesEx":
						o.PropertiesEx = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceMru ReadAceMru(XmlReader xr)
		{
			KeePass.App.Configuration.AceMru o = new KeePass.App.Configuration.AceMru();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "MaxItemCount":
						o.MaxItemCount = ReadUInt32(xr);
						break;
					case "Items":
						o.Items = ReadListOfIOConnectionInfo(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.String[] ReadArrayOfString(XmlReader xr)
		{
			List<System.String> l = new List<System.String>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				System.String oElem = ReadString(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePass.App.Configuration.AceStartUp ReadAceStartUp(XmlReader xr)
		{
			KeePass.App.Configuration.AceStartUp o = new KeePass.App.Configuration.AceStartUp();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "OpenLastFile":
						o.OpenLastFile = ReadBoolean(xr);
						break;
					case "CheckForUpdate":
						o.CheckForUpdate = ReadBoolean(xr);
						break;
					case "CheckForUpdateConfigured":
						o.CheckForUpdateConfigured = ReadBoolean(xr);
						break;
					case "MinimizedAndLocked":
						o.MinimizedAndLocked = ReadBoolean(xr);
						break;
					case "PluginCacheDeleteOld":
						o.PluginCacheDeleteOld = ReadBoolean(xr);
						break;
					case "PluginCacheClearOnce":
						o.PluginCacheClearOnce = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceOpenDb ReadAceOpenDb(XmlReader xr)
		{
			KeePass.App.Configuration.AceOpenDb o = new KeePass.App.Configuration.AceOpenDb();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "ShowExpiredEntries":
						o.ShowExpiredEntries = ReadBoolean(xr);
						break;
					case "ShowSoonToExpireEntries":
						o.ShowSoonToExpireEntries = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceCloseDb ReadAceCloseDb(XmlReader xr)
		{
			KeePass.App.Configuration.AceCloseDb o = new KeePass.App.Configuration.AceCloseDb();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "AutoSave":
						o.AutoSave = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasTriggerSystem ReadEcasTriggerSystem(XmlReader xr)
		{
			KeePass.Ecas.EcasTriggerSystem o = new KeePass.Ecas.EcasTriggerSystem();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Enabled":
						o.Enabled = ReadBoolean(xr);
						break;
					case "Triggers":
						o.TriggerArrayForSerialization = ReadArrayOfEcasTrigger(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Int32 ReadInt32(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToInt32(strValue);
		}

		private static System.Double ReadDouble(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToDouble(strValue);
		}

		private static Dictionary<string, KeePass.App.Configuration.AceMainWindowLayout> m_dictAceMainWindowLayout = null;
		private static KeePass.App.Configuration.AceMainWindowLayout ReadAceMainWindowLayout(XmlReader xr)
		{
			if(m_dictAceMainWindowLayout == null)
			{
				m_dictAceMainWindowLayout = new Dictionary<string, KeePass.App.Configuration.AceMainWindowLayout>();
				m_dictAceMainWindowLayout["Default"] = KeePass.App.Configuration.AceMainWindowLayout.Default;
				m_dictAceMainWindowLayout["SideBySide"] = KeePass.App.Configuration.AceMainWindowLayout.SideBySide;
			}

			string strValue = xr.ReadElementString();
			KeePass.App.Configuration.AceMainWindowLayout eResult;
			if(!m_dictAceMainWindowLayout.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static KeePass.App.Configuration.AceToolBar ReadAceToolBar(XmlReader xr)
		{
			KeePass.App.Configuration.AceToolBar o = new KeePass.App.Configuration.AceToolBar();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Show":
						o.Show = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceEntryView ReadAceEntryView(XmlReader xr)
		{
			KeePass.App.Configuration.AceEntryView o = new KeePass.App.Configuration.AceEntryView();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Show":
						o.Show = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceTanView ReadAceTanView(XmlReader xr)
		{
			KeePass.App.Configuration.AceTanView o = new KeePass.App.Configuration.AceTanView();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "UseSimpleView":
						o.UseSimpleView = ReadBoolean(xr);
						break;
					case "ShowIndices":
						o.ShowIndices = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePass.App.Configuration.AceColumn> ReadListOfAceColumn(XmlReader xr)
		{
			System.Collections.Generic.List<KeePass.App.Configuration.AceColumn> o = new System.Collections.Generic.List<KeePass.App.Configuration.AceColumn>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.App.Configuration.AceColumn oElem = ReadAceColumn(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.UI.ListSorter ReadListSorter(XmlReader xr)
		{
			KeePass.UI.ListSorter o = new KeePass.UI.ListSorter();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Column":
						o.Column = ReadInt32(xr);
						break;
					case "Order":
						o.Order = ReadSortOrder(xr);
						break;
					case "CompareNaturally":
						o.CompareNaturally = ReadBoolean(xr);
						break;
					case "CompareTimes":
						o.CompareTimes = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceTrayIcon ReadAceTrayIcon(XmlReader xr)
		{
			KeePass.App.Configuration.AceTrayIcon o = new KeePass.App.Configuration.AceTrayIcon();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "ShowOnlyIfTrayed":
						o.ShowOnlyIfTrayed = ReadBoolean(xr);
						break;
					case "SingleClickDefault":
						o.SingleClickDefault = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceHiding ReadAceHiding(XmlReader xr)
		{
			KeePass.App.Configuration.AceHiding o = new KeePass.App.Configuration.AceHiding();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "SeparateHidingSettings":
						o.SeparateHidingSettings = ReadBoolean(xr);
						break;
					case "HideInEntryWindow":
						o.HideInEntryWindow = ReadBoolean(xr);
						break;
					case "UnhideButtonAlsoUnhidesSource":
						o.UnhideButtonAlsoUnhidesSource = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceFont ReadAceFont(XmlReader xr)
		{
			KeePass.App.Configuration.AceFont o = new KeePass.App.Configuration.AceFont();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Family":
						o.Family = ReadString(xr);
						break;
					case "Size":
						o.Size = ReadSingle(xr);
						break;
					case "GraphicsUnit":
						o.GraphicsUnit = ReadGraphicsUnit(xr);
						break;
					case "Style":
						o.Style = ReadFontStyle(xr);
						break;
					case "OverrideUIDefault":
						o.OverrideUIDefault = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static Dictionary<string, KeePass.UI.BannerStyle> m_dictBannerStyle = null;
		private static KeePass.UI.BannerStyle ReadBannerStyle(XmlReader xr)
		{
			if(m_dictBannerStyle == null)
			{
				m_dictBannerStyle = new Dictionary<string, KeePass.UI.BannerStyle>();
				m_dictBannerStyle["Default"] = KeePass.UI.BannerStyle.Default;
				m_dictBannerStyle["WinXPLogin"] = KeePass.UI.BannerStyle.WinXPLogin;
				m_dictBannerStyle["WinVistaBlack"] = KeePass.UI.BannerStyle.WinVistaBlack;
				m_dictBannerStyle["KeePassWin32"] = KeePass.UI.BannerStyle.KeePassWin32;
				m_dictBannerStyle["BlueCarbon"] = KeePass.UI.BannerStyle.BlueCarbon;
			}

			string strValue = xr.ReadElementString();
			KeePass.UI.BannerStyle eResult;
			if(!m_dictBannerStyle.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static System.Int64 ReadInt64(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToInt64(strValue);
		}

		private static System.UInt64 ReadUInt64(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToUInt64(strValue);
		}

		private static KeePass.App.Configuration.AceWorkspaceLocking ReadAceWorkspaceLocking(XmlReader xr)
		{
			KeePass.App.Configuration.AceWorkspaceLocking o = new KeePass.App.Configuration.AceWorkspaceLocking();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "LockOnWindowMinimize":
						o.LockOnWindowMinimize = ReadBoolean(xr);
						break;
					case "LockOnWindowMinimizeToTray":
						o.LockOnWindowMinimizeToTray = ReadBoolean(xr);
						break;
					case "LockOnSessionSwitch":
						o.LockOnSessionSwitch = ReadBoolean(xr);
						break;
					case "LockOnSuspend":
						o.LockOnSuspend = ReadBoolean(xr);
						break;
					case "LockOnRemoteControlChange":
						o.LockOnRemoteControlChange = ReadBoolean(xr);
						break;
					case "LockAfterTime":
						o.LockAfterTime = ReadUInt32(xr);
						break;
					case "LockAfterGlobalTime":
						o.LockAfterGlobalTime = ReadUInt32(xr);
						break;
					case "ExitInsteadOfLockingAfterTime":
						o.ExitInsteadOfLockingAfterTime = ReadBoolean(xr);
						break;
					case "AlwaysExitInsteadOfLocking":
						o.AlwaysExitInsteadOfLocking = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.AppPolicyFlags ReadAppPolicyFlags(XmlReader xr)
		{
			KeePass.App.AppPolicyFlags o = new KeePass.App.AppPolicyFlags();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Plugins":
						o.Plugins = ReadBoolean(xr);
						break;
					case "Export":
						o.Export = ReadBoolean(xr);
						break;
					case "ExportNoKey":
						o.ExportNoKey = ReadBoolean(xr);
						break;
					case "Import":
						o.Import = ReadBoolean(xr);
						break;
					case "Print":
						o.Print = ReadBoolean(xr);
						break;
					case "PrintNoKey":
						o.PrintNoKey = ReadBoolean(xr);
						break;
					case "NewFile":
						o.NewFile = ReadBoolean(xr);
						break;
					case "SaveFile":
						o.SaveFile = ReadBoolean(xr);
						break;
					case "AutoType":
						o.AutoType = ReadBoolean(xr);
						break;
					case "AutoTypeWithoutContext":
						o.AutoTypeWithoutContext = ReadBoolean(xr);
						break;
					case "CopyToClipboard":
						o.CopyToClipboard = ReadBoolean(xr);
						break;
					case "CopyWholeEntries":
						o.CopyWholeEntries = ReadBoolean(xr);
						break;
					case "DragDrop":
						o.DragDrop = ReadBoolean(xr);
						break;
					case "UnhidePasswords":
						o.UnhidePasswords = ReadBoolean(xr);
						break;
					case "ChangeMasterKey":
						o.ChangeMasterKey = ReadBoolean(xr);
						break;
					case "ChangeMasterKeyNoKey":
						o.ChangeMasterKeyNoKey = ReadBoolean(xr);
						break;
					case "EditTriggers":
						o.EditTriggers = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceMasterPassword ReadAceMasterPassword(XmlReader xr)
		{
			KeePass.App.Configuration.AceMasterPassword o = new KeePass.App.Configuration.AceMasterPassword();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "MinimumLength":
						o.MinimumLength = ReadUInt32(xr);
						break;
					case "MinimumQuality":
						o.MinimumQuality = ReadUInt32(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Cryptography.PasswordGenerator.PwProfile ReadPwProfile(XmlReader xr)
		{
			KeePassLib.Cryptography.PasswordGenerator.PwProfile o = new KeePassLib.Cryptography.PasswordGenerator.PwProfile();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Name":
						o.Name = ReadString(xr);
						break;
					case "GeneratorType":
						o.GeneratorType = ReadPasswordGeneratorType(xr);
						break;
					case "CollectUserEntropy":
						o.CollectUserEntropy = ReadBoolean(xr);
						break;
					case "Length":
						o.Length = ReadUInt32(xr);
						break;
					case "CharSetRanges":
						o.CharSetRanges = ReadString(xr);
						break;
					case "CharSetAdditional":
						o.CharSetAdditional = ReadString(xr);
						break;
					case "Pattern":
						o.Pattern = ReadString(xr);
						break;
					case "PatternPermutePassword":
						o.PatternPermutePassword = ReadBoolean(xr);
						break;
					case "ExcludeLookAlike":
						o.ExcludeLookAlike = ReadBoolean(xr);
						break;
					case "NoRepeatingCharacters":
						o.NoRepeatingCharacters = ReadBoolean(xr);
						break;
					case "ExcludeCharacters":
						o.ExcludeCharacters = ReadString(xr);
						break;
					case "CustomAlgorithmUuid":
						o.CustomAlgorithmUuid = ReadString(xr);
						break;
					case "CustomAlgorithmOptions":
						o.CustomAlgorithmOptions = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePassLib.Cryptography.PasswordGenerator.PwProfile> ReadListOfPwProfile(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Cryptography.PasswordGenerator.PwProfile> o = new System.Collections.Generic.List<KeePassLib.Cryptography.PasswordGenerator.PwProfile>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Cryptography.PasswordGenerator.PwProfile oElem = ReadPwProfile(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.UInt32 ReadUInt32(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToUInt32(strValue);
		}

		private static KeePassLib.SearchParameters ReadSearchParameters(XmlReader xr)
		{
			KeePassLib.SearchParameters o = new KeePassLib.SearchParameters();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "SearchString":
						o.SearchString = ReadString(xr);
						break;
					case "RegularExpression":
						o.RegularExpression = ReadBoolean(xr);
						break;
					case "SearchInTitles":
						o.SearchInTitles = ReadBoolean(xr);
						break;
					case "SearchInUserNames":
						o.SearchInUserNames = ReadBoolean(xr);
						break;
					case "SearchInPasswords":
						o.SearchInPasswords = ReadBoolean(xr);
						break;
					case "SearchInUrls":
						o.SearchInUrls = ReadBoolean(xr);
						break;
					case "SearchInNotes":
						o.SearchInNotes = ReadBoolean(xr);
						break;
					case "SearchInOther":
						o.SearchInOther = ReadBoolean(xr);
						break;
					case "SearchInUuids":
						o.SearchInUuids = ReadBoolean(xr);
						break;
					case "SearchInGroupNames":
						o.SearchInGroupNames = ReadBoolean(xr);
						break;
					case "SearchInTags":
						o.SearchInTags = ReadBoolean(xr);
						break;
					case "ComparisonMode":
						o.ComparisonMode = ReadStringComparison(xr);
						break;
					case "ExcludeExpired":
						o.ExcludeExpired = ReadBoolean(xr);
						break;
					case "RespectEntrySearchingDisabled":
						o.RespectEntrySearchingDisabled = ReadBoolean(xr);
						break;
					case "DataTransformation":
						o.DataTransformation = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePass.App.Configuration.AceKeyAssoc> ReadListOfAceKeyAssoc(XmlReader xr)
		{
			System.Collections.Generic.List<KeePass.App.Configuration.AceKeyAssoc> o = new System.Collections.Generic.List<KeePass.App.Configuration.AceKeyAssoc>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.App.Configuration.AceKeyAssoc oElem = ReadAceKeyAssoc(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.App.Configuration.AceUrlSchemeOverrides ReadAceUrlSchemeOverrides(XmlReader xr)
		{
			KeePass.App.Configuration.AceUrlSchemeOverrides o = new KeePass.App.Configuration.AceUrlSchemeOverrides();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "BuiltInOverridesEnabled":
						o.BuiltInOverridesEnabled = ReadUInt64(xr);
						break;
					case "CustomOverrides":
						o.CustomOverrides = ReadListOfAceUrlSchemeOverride(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static Dictionary<string, KeePassLib.ProxyServerType> m_dictProxyServerType = null;
		private static KeePassLib.ProxyServerType ReadProxyServerType(XmlReader xr)
		{
			if(m_dictProxyServerType == null)
			{
				m_dictProxyServerType = new Dictionary<string, KeePassLib.ProxyServerType>();
				m_dictProxyServerType["None"] = KeePassLib.ProxyServerType.None;
				m_dictProxyServerType["System"] = KeePassLib.ProxyServerType.System;
				m_dictProxyServerType["Manual"] = KeePassLib.ProxyServerType.Manual;
			}

			string strValue = xr.ReadElementString();
			KeePassLib.ProxyServerType eResult;
			if(!m_dictProxyServerType.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static Dictionary<string, KeePassLib.ProxyAuthType> m_dictProxyAuthType = null;
		private static KeePassLib.ProxyAuthType ReadProxyAuthType(XmlReader xr)
		{
			if(m_dictProxyAuthType == null)
			{
				m_dictProxyAuthType = new Dictionary<string, KeePassLib.ProxyAuthType>();
				m_dictProxyAuthType["None"] = KeePassLib.ProxyAuthType.None;
				m_dictProxyAuthType["Default"] = KeePassLib.ProxyAuthType.Default;
				m_dictProxyAuthType["Manual"] = KeePassLib.ProxyAuthType.Manual;
				m_dictProxyAuthType["Auto"] = KeePassLib.ProxyAuthType.Auto;
			}

			string strValue = xr.ReadElementString();
			KeePassLib.ProxyAuthType eResult;
			if(!m_dictProxyAuthType.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static KeePass.App.Configuration.AceKvp ReadAceKvp(XmlReader xr)
		{
			KeePass.App.Configuration.AceKvp o = new KeePass.App.Configuration.AceKvp();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Key":
						o.Key = ReadString(xr);
						break;
					case "Value":
						o.Value = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KPStringTable ReadKPStringTable(XmlReader xr)
		{
			KeePassLib.Translation.KPStringTable o = new KeePassLib.Translation.KPStringTable();

			while(xr.MoveToNextAttribute())
			{
				switch(xr.LocalName)
				{
					case "Name":
						o.Name = xr.Value;
						break;
					default:
						Debug.Assert(false);
						break;
				}
			}

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Strings":
						o.Strings = ReadListOfKPStringTableItem(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KPFormCustomization ReadKPFormCustomization(XmlReader xr)
		{
			KeePassLib.Translation.KPFormCustomization o = new KeePassLib.Translation.KPFormCustomization();

			while(xr.MoveToNextAttribute())
			{
				switch(xr.LocalName)
				{
					case "FullName":
						o.FullName = xr.Value;
						break;
					default:
						Debug.Assert(false);
						break;
				}
			}

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Window":
						o.Window = ReadKPControlCustomization(xr);
						break;
					case "ChildControls":
						o.Controls = ReadListOfKPControlCustomization(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static Dictionary<string, KeePassLib.Serialization.IOCredProtMode> m_dictIOCredProtMode = null;
		private static KeePassLib.Serialization.IOCredProtMode ReadIOCredProtMode(XmlReader xr)
		{
			if(m_dictIOCredProtMode == null)
			{
				m_dictIOCredProtMode = new Dictionary<string, KeePassLib.Serialization.IOCredProtMode>();
				m_dictIOCredProtMode["None"] = KeePassLib.Serialization.IOCredProtMode.None;
				m_dictIOCredProtMode["Obf"] = KeePassLib.Serialization.IOCredProtMode.Obf;
			}

			string strValue = xr.ReadElementString();
			KeePassLib.Serialization.IOCredProtMode eResult;
			if(!m_dictIOCredProtMode.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static Dictionary<string, KeePassLib.Serialization.IOCredSaveMode> m_dictIOCredSaveMode = null;
		private static KeePassLib.Serialization.IOCredSaveMode ReadIOCredSaveMode(XmlReader xr)
		{
			if(m_dictIOCredSaveMode == null)
			{
				m_dictIOCredSaveMode = new Dictionary<string, KeePassLib.Serialization.IOCredSaveMode>();
				m_dictIOCredSaveMode["NoSave"] = KeePassLib.Serialization.IOCredSaveMode.NoSave;
				m_dictIOCredSaveMode["UserNameOnly"] = KeePassLib.Serialization.IOCredSaveMode.UserNameOnly;
				m_dictIOCredSaveMode["SaveCred"] = KeePassLib.Serialization.IOCredSaveMode.SaveCred;
			}

			string strValue = xr.ReadElementString();
			KeePassLib.Serialization.IOCredSaveMode eResult;
			if(!m_dictIOCredSaveMode.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static System.Collections.Generic.List<KeePassLib.Serialization.IOConnectionInfo> ReadListOfIOConnectionInfo(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Serialization.IOConnectionInfo> o = new System.Collections.Generic.List<KeePassLib.Serialization.IOConnectionInfo>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Serialization.IOConnectionInfo oElem = ReadIOConnectionInfo(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasTrigger[] ReadArrayOfEcasTrigger(XmlReader xr)
		{
			List<KeePass.Ecas.EcasTrigger> l = new List<KeePass.Ecas.EcasTrigger>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.Ecas.EcasTrigger oElem = ReadEcasTrigger(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePass.App.Configuration.AceColumn ReadAceColumn(XmlReader xr)
		{
			KeePass.App.Configuration.AceColumn o = new KeePass.App.Configuration.AceColumn();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Type":
						o.Type = ReadAceColumnType(xr);
						break;
					case "CustomName":
						o.CustomName = ReadString(xr);
						break;
					case "Width":
						o.Width = ReadInt32(xr);
						break;
					case "HideWithAsterisks":
						o.HideWithAsterisks = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static Dictionary<string, System.Windows.Forms.SortOrder> m_dictSortOrder = null;
		private static System.Windows.Forms.SortOrder ReadSortOrder(XmlReader xr)
		{
			if(m_dictSortOrder == null)
			{
				m_dictSortOrder = new Dictionary<string, System.Windows.Forms.SortOrder>();
				m_dictSortOrder["None"] = System.Windows.Forms.SortOrder.None;
				m_dictSortOrder["Ascending"] = System.Windows.Forms.SortOrder.Ascending;
				m_dictSortOrder["Descending"] = System.Windows.Forms.SortOrder.Descending;
			}

			string strValue = xr.ReadElementString();
			System.Windows.Forms.SortOrder eResult;
			if(!m_dictSortOrder.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static System.Single ReadSingle(XmlReader xr)
		{
			string strValue = xr.ReadElementString();
			return XmlConvert.ToSingle(strValue);
		}

		private static Dictionary<string, System.Drawing.GraphicsUnit> m_dictGraphicsUnit = null;
		private static System.Drawing.GraphicsUnit ReadGraphicsUnit(XmlReader xr)
		{
			if(m_dictGraphicsUnit == null)
			{
				m_dictGraphicsUnit = new Dictionary<string, System.Drawing.GraphicsUnit>();
				m_dictGraphicsUnit["World"] = System.Drawing.GraphicsUnit.World;
				m_dictGraphicsUnit["Display"] = System.Drawing.GraphicsUnit.Display;
				m_dictGraphicsUnit["Pixel"] = System.Drawing.GraphicsUnit.Pixel;
				m_dictGraphicsUnit["Point"] = System.Drawing.GraphicsUnit.Point;
				m_dictGraphicsUnit["Inch"] = System.Drawing.GraphicsUnit.Inch;
				m_dictGraphicsUnit["Document"] = System.Drawing.GraphicsUnit.Document;
				m_dictGraphicsUnit["Millimeter"] = System.Drawing.GraphicsUnit.Millimeter;
			}

			string strValue = xr.ReadElementString();
			System.Drawing.GraphicsUnit eResult;
			if(!m_dictGraphicsUnit.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static Dictionary<string, System.Drawing.FontStyle> m_dictFontStyle = null;
		private static System.Drawing.FontStyle ReadFontStyle(XmlReader xr)
		{
			if(m_dictFontStyle == null)
			{
				m_dictFontStyle = new Dictionary<string, System.Drawing.FontStyle>();
				m_dictFontStyle["Regular"] = System.Drawing.FontStyle.Regular;
				m_dictFontStyle["Bold"] = System.Drawing.FontStyle.Bold;
				m_dictFontStyle["Italic"] = System.Drawing.FontStyle.Italic;
				m_dictFontStyle["Underline"] = System.Drawing.FontStyle.Underline;
				m_dictFontStyle["Strikeout"] = System.Drawing.FontStyle.Strikeout;
			}

			string strValue = xr.ReadElementString();
			System.Drawing.FontStyle eResult = (System.Drawing.FontStyle)0;
			string[] vValues = strValue.Split(m_vEnumSeps, StringSplitOptions.RemoveEmptyEntries);
			foreach(string strPart in vValues)
			{
				System.Drawing.FontStyle ePart;
				if(m_dictFontStyle.TryGetValue(strPart, out ePart))
					eResult |= ePart;
				else { Debug.Assert(false); }
			}
			return eResult;
		}

		private static Dictionary<string, KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType> m_dictPasswordGeneratorType = null;
		private static KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType ReadPasswordGeneratorType(XmlReader xr)
		{
			if(m_dictPasswordGeneratorType == null)
			{
				m_dictPasswordGeneratorType = new Dictionary<string, KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType>();
				m_dictPasswordGeneratorType["CharSet"] = KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType.CharSet;
				m_dictPasswordGeneratorType["Pattern"] = KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType.Pattern;
				m_dictPasswordGeneratorType["Custom"] = KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType.Custom;
			}

			string strValue = xr.ReadElementString();
			KeePassLib.Cryptography.PasswordGenerator.PasswordGeneratorType eResult;
			if(!m_dictPasswordGeneratorType.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static Dictionary<string, System.StringComparison> m_dictStringComparison = null;
		private static System.StringComparison ReadStringComparison(XmlReader xr)
		{
			if(m_dictStringComparison == null)
			{
				m_dictStringComparison = new Dictionary<string, System.StringComparison>();
				m_dictStringComparison["CurrentCulture"] = System.StringComparison.CurrentCulture;
				m_dictStringComparison["CurrentCultureIgnoreCase"] = System.StringComparison.CurrentCultureIgnoreCase;
				m_dictStringComparison["InvariantCulture"] = System.StringComparison.InvariantCulture;
				m_dictStringComparison["InvariantCultureIgnoreCase"] = System.StringComparison.InvariantCultureIgnoreCase;
				m_dictStringComparison["Ordinal"] = System.StringComparison.Ordinal;
				m_dictStringComparison["OrdinalIgnoreCase"] = System.StringComparison.OrdinalIgnoreCase;
			}

			string strValue = xr.ReadElementString();
			System.StringComparison eResult;
			if(!m_dictStringComparison.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static KeePass.App.Configuration.AceKeyAssoc ReadAceKeyAssoc(XmlReader xr)
		{
			KeePass.App.Configuration.AceKeyAssoc o = new KeePass.App.Configuration.AceKeyAssoc();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "DatabasePath":
						o.DatabasePath = ReadString(xr);
						break;
					case "KeyFilePath":
						o.KeyFilePath = ReadString(xr);
						break;
					case "KeyProvider":
						o.KeyProvider = ReadString(xr);
						break;
					case "UserAccount":
						o.UserAccount = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePass.App.Configuration.AceUrlSchemeOverride> ReadListOfAceUrlSchemeOverride(XmlReader xr)
		{
			System.Collections.Generic.List<KeePass.App.Configuration.AceUrlSchemeOverride> o = new System.Collections.Generic.List<KeePass.App.Configuration.AceUrlSchemeOverride>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.App.Configuration.AceUrlSchemeOverride oElem = ReadAceUrlSchemeOverride(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePassLib.Translation.KPStringTableItem> ReadListOfKPStringTableItem(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Translation.KPStringTableItem> o = new System.Collections.Generic.List<KeePassLib.Translation.KPStringTableItem>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Translation.KPStringTableItem oElem = ReadKPStringTableItem(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KPControlCustomization ReadKPControlCustomization(XmlReader xr)
		{
			KeePassLib.Translation.KPControlCustomization o = new KeePassLib.Translation.KPControlCustomization();

			while(xr.MoveToNextAttribute())
			{
				switch(xr.LocalName)
				{
					case "Name":
						o.Name = xr.Value;
						break;
					case "BaseHash":
						o.BaseHash = xr.Value;
						break;
					default:
						Debug.Assert(false);
						break;
				}
			}

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Text":
						o.Text = ReadString(xr);
						break;
					case "Layout":
						o.Layout = ReadKpccLayout(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<KeePassLib.Translation.KPControlCustomization> ReadListOfKPControlCustomization(XmlReader xr)
		{
			System.Collections.Generic.List<KeePassLib.Translation.KPControlCustomization> o = new System.Collections.Generic.List<KeePassLib.Translation.KPControlCustomization>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePassLib.Translation.KPControlCustomization oElem = ReadKPControlCustomization(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasTrigger ReadEcasTrigger(XmlReader xr)
		{
			KeePass.Ecas.EcasTrigger o = new KeePass.Ecas.EcasTrigger();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Guid":
						o.UuidString = ReadString(xr);
						break;
					case "Name":
						o.Name = ReadString(xr);
						break;
					case "Comments":
						o.Comments = ReadString(xr);
						break;
					case "Enabled":
						o.Enabled = ReadBoolean(xr);
						break;
					case "InitiallyOn":
						o.InitiallyOn = ReadBoolean(xr);
						break;
					case "TurnOffAfterAction":
						o.TurnOffAfterAction = ReadBoolean(xr);
						break;
					case "Events":
						o.EventArrayForSerialization = ReadArrayOfEcasEvent(xr);
						break;
					case "Conditions":
						o.ConditionsArrayForSerialization = ReadArrayOfEcasCondition(xr);
						break;
					case "Actions":
						o.ActionArrayForSerialization = ReadArrayOfEcasAction(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static Dictionary<string, KeePass.App.Configuration.AceColumnType> m_dictAceColumnType = null;
		private static KeePass.App.Configuration.AceColumnType ReadAceColumnType(XmlReader xr)
		{
			if(m_dictAceColumnType == null)
			{
				m_dictAceColumnType = new Dictionary<string, KeePass.App.Configuration.AceColumnType>();
				m_dictAceColumnType["Title"] = KeePass.App.Configuration.AceColumnType.Title;
				m_dictAceColumnType["UserName"] = KeePass.App.Configuration.AceColumnType.UserName;
				m_dictAceColumnType["Password"] = KeePass.App.Configuration.AceColumnType.Password;
				m_dictAceColumnType["Url"] = KeePass.App.Configuration.AceColumnType.Url;
				m_dictAceColumnType["Notes"] = KeePass.App.Configuration.AceColumnType.Notes;
				m_dictAceColumnType["CreationTime"] = KeePass.App.Configuration.AceColumnType.CreationTime;
				m_dictAceColumnType["LastModificationTime"] = KeePass.App.Configuration.AceColumnType.LastModificationTime;
				m_dictAceColumnType["LastAccessTime"] = KeePass.App.Configuration.AceColumnType.LastAccessTime;
				m_dictAceColumnType["ExpiryTime"] = KeePass.App.Configuration.AceColumnType.ExpiryTime;
				m_dictAceColumnType["Uuid"] = KeePass.App.Configuration.AceColumnType.Uuid;
				m_dictAceColumnType["Attachment"] = KeePass.App.Configuration.AceColumnType.Attachment;
				m_dictAceColumnType["CustomString"] = KeePass.App.Configuration.AceColumnType.CustomString;
				m_dictAceColumnType["PluginExt"] = KeePass.App.Configuration.AceColumnType.PluginExt;
				m_dictAceColumnType["OverrideUrl"] = KeePass.App.Configuration.AceColumnType.OverrideUrl;
				m_dictAceColumnType["Tags"] = KeePass.App.Configuration.AceColumnType.Tags;
				m_dictAceColumnType["ExpiryTimeDateOnly"] = KeePass.App.Configuration.AceColumnType.ExpiryTimeDateOnly;
				m_dictAceColumnType["Size"] = KeePass.App.Configuration.AceColumnType.Size;
				m_dictAceColumnType["HistoryCount"] = KeePass.App.Configuration.AceColumnType.HistoryCount;
				m_dictAceColumnType["AttachmentCount"] = KeePass.App.Configuration.AceColumnType.AttachmentCount;
				m_dictAceColumnType["Count"] = KeePass.App.Configuration.AceColumnType.Count;
			}

			string strValue = xr.ReadElementString();
			KeePass.App.Configuration.AceColumnType eResult;
			if(!m_dictAceColumnType.TryGetValue(strValue, out eResult))
				{ Debug.Assert(false); }
			return eResult;
		}

		private static KeePass.App.Configuration.AceUrlSchemeOverride ReadAceUrlSchemeOverride(XmlReader xr)
		{
			KeePass.App.Configuration.AceUrlSchemeOverride o = new KeePass.App.Configuration.AceUrlSchemeOverride();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Enabled":
						o.Enabled = ReadBoolean(xr);
						break;
					case "Scheme":
						o.Scheme = ReadString(xr);
						break;
					case "UrlOverride":
						o.UrlOverride = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KPStringTableItem ReadKPStringTableItem(XmlReader xr)
		{
			KeePassLib.Translation.KPStringTableItem o = new KeePassLib.Translation.KPStringTableItem();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "Name":
						o.Name = ReadString(xr);
						break;
					case "Value":
						o.Value = ReadString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePassLib.Translation.KpccLayout ReadKpccLayout(XmlReader xr)
		{
			KeePassLib.Translation.KpccLayout o = new KeePassLib.Translation.KpccLayout();

			while(xr.MoveToNextAttribute())
			{
				switch(xr.LocalName)
				{
					case "X":
						o.X = xr.Value;
						break;
					case "Y":
						o.Y = xr.Value;
						break;
					case "Width":
						o.Width = xr.Value;
						break;
					case "Height":
						o.Height = xr.Value;
						break;
					default:
						Debug.Assert(false);
						break;
				}
			}

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				Debug.Assert(false);
				xr.Skip();

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasEvent[] ReadArrayOfEcasEvent(XmlReader xr)
		{
			List<KeePass.Ecas.EcasEvent> l = new List<KeePass.Ecas.EcasEvent>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.Ecas.EcasEvent oElem = ReadEcasEvent(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePass.Ecas.EcasCondition[] ReadArrayOfEcasCondition(XmlReader xr)
		{
			List<KeePass.Ecas.EcasCondition> l = new List<KeePass.Ecas.EcasCondition>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.Ecas.EcasCondition oElem = ReadEcasCondition(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePass.Ecas.EcasAction[] ReadArrayOfEcasAction(XmlReader xr)
		{
			List<KeePass.Ecas.EcasAction> l = new List<KeePass.Ecas.EcasAction>();

			if(SkipEmptyElement(xr)) return l.ToArray();

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				KeePass.Ecas.EcasAction oElem = ReadEcasAction(xr);
				l.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return l.ToArray();
		}

		private static KeePass.Ecas.EcasEvent ReadEcasEvent(XmlReader xr)
		{
			KeePass.Ecas.EcasEvent o = new KeePass.Ecas.EcasEvent();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "TypeGuid":
						o.TypeString = ReadString(xr);
						break;
					case "Parameters":
						o.Parameters = ReadListOfString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasCondition ReadEcasCondition(XmlReader xr)
		{
			KeePass.Ecas.EcasCondition o = new KeePass.Ecas.EcasCondition();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "TypeGuid":
						o.TypeString = ReadString(xr);
						break;
					case "Parameters":
						o.Parameters = ReadListOfString(xr);
						break;
					case "Negate":
						o.Negate = ReadBoolean(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static KeePass.Ecas.EcasAction ReadEcasAction(XmlReader xr)
		{
			KeePass.Ecas.EcasAction o = new KeePass.Ecas.EcasAction();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				switch(xr.LocalName)
				{
					case "TypeGuid":
						o.TypeString = ReadString(xr);
						break;
					case "Parameters":
						o.Parameters = ReadListOfString(xr);
						break;
					default:
						Debug.Assert(false);
						xr.Skip();
						break;
				}

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}

		private static System.Collections.Generic.List<System.String> ReadListOfString(XmlReader xr)
		{
			System.Collections.Generic.List<System.String> o = new System.Collections.Generic.List<System.String>();

			if(SkipEmptyElement(xr)) return o;

			Debug.Assert(xr.NodeType == XmlNodeType.Element);
			xr.ReadStartElement();
			xr.MoveToContent();

			while(true)
			{
				XmlNodeType nt = xr.NodeType;
				if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;
				if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }

				System.String oElem = ReadString(xr);
				o.Add(oElem);

				xr.MoveToContent();
			}

			Debug.Assert(xr.NodeType == XmlNodeType.EndElement);
			xr.ReadEndElement();
			return o;
		}
	}
}
