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

#include "ShInstUtil.h"

#pragma warning(push)
#pragma warning(disable: 4996) // SCL warning
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string/trim.hpp>
#pragma warning(pop)

static const std_string g_strNGenInstall = _T("ngen_install");
static const std_string g_strNGenUninstall = _T("ngen_uninstall");
static const std_string g_strNetCheck = _T("net_check");
static const std_string g_strPreLoadRegister = _T("preload_register");
static const std_string g_strPreLoadUnregister = _T("preload_unregister");

static LPCTSTR g_lpPathTrimChars = _T("\"' \t\r\n");

int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
	LPTSTR lpCmdLine, int nCmdShow)
{
	UNREFERENCED_PARAMETER(hInstance);
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);
	UNREFERENCED_PARAMETER(nCmdShow);

	INITCOMMONCONTROLSEX icc;
	ZeroMemory(&icc, sizeof(INITCOMMONCONTROLSEX));
	icc.dwSize = sizeof(INITCOMMONCONTROLSEX);
	icc.dwICC = ICC_STANDARD_CLASSES;
	InitCommonControlsEx(&icc);

	std_string strCmdLine = GetCommandLine();
	boost::trim_if(strCmdLine, boost::is_any_of(g_lpPathTrimChars));

	std::transform(strCmdLine.begin(), strCmdLine.end(), strCmdLine.begin(), tolower);

	if((strCmdLine.size() >= g_strNGenInstall.size()) && (strCmdLine.substr(
		strCmdLine.size() - g_strNGenInstall.size()) == g_strNGenInstall))
	{
		UpdateNativeImage(false);
		Sleep(200);
		UpdateNativeImage(true);
	}

	if((strCmdLine.size() >= g_strNGenUninstall.size()) && (strCmdLine.substr(
		strCmdLine.size() - g_strNGenUninstall.size()) == g_strNGenUninstall))
	{
		UpdateNativeImage(false);
	}

	if((strCmdLine.size() >= g_strPreLoadRegister.size()) && (strCmdLine.substr(
		strCmdLine.size() - g_strPreLoadRegister.size()) == g_strPreLoadRegister))
	{
		RegisterPreLoad(true);
	}

	if((strCmdLine.size() >= g_strPreLoadUnregister.size()) && (strCmdLine.substr(
		strCmdLine.size() - g_strPreLoadUnregister.size()) == g_strPreLoadUnregister))
	{
		RegisterPreLoad(false);
	}

	if((strCmdLine.size() >= g_strNetCheck.size()) && (strCmdLine.substr(
		strCmdLine.size() - g_strNetCheck.size()) == g_strNetCheck))
	{
		CheckDotNetInstalled();
	}

	return 0;
}

void UpdateNativeImage(bool bInstall)
{
	const std_string strNGen = FindNGen();
	if(strNGen.size() == 0) return;

	const std_string strKeePassExe = GetKeePassExePath();
	if(strKeePassExe.size() == 0) return;

	std_string strParam = (bInstall ? _T("") : _T("un"));
	strParam += _T("install \"");
	strParam += strKeePassExe + _T("\"");

	SHELLEXECUTEINFO sei;
	ZeroMemory(&sei, sizeof(SHELLEXECUTEINFO));
	sei.cbSize = sizeof(SHELLEXECUTEINFO);
	sei.fMask = SEE_MASK_NOCLOSEPROCESS;
	sei.lpVerb = _T("open");
	sei.lpFile = strNGen.c_str();
	sei.lpParameters = strParam.c_str();
	sei.nShow = SW_HIDE;
	ShellExecuteEx(&sei);

	if(sei.hProcess != NULL)
	{
		WaitForSingleObject(sei.hProcess, 16000);
		CloseHandle(sei.hProcess);
	}
}

void RegisterPreLoad(bool bRegister)
{
	const std_string strPath = GetKeePassExePath();
	if(strPath.size() == 0) return;

	HKEY hKey = NULL;
	if(RegOpenKeyEx(HKEY_LOCAL_MACHINE, _T("Software\\Microsoft\\Windows\\CurrentVersion\\Run"),
		0, KEY_WRITE, &hKey) != ERROR_SUCCESS) return;
	if(hKey == NULL) return;

	const std_string strItemName = _T("KeePass 2 PreLoad");
	std_string strItemValue = _T("\"");
	strItemValue += strPath;
	strItemValue += _T("\" --preload");

	if(bRegister)
		RegSetValueEx(hKey, strItemName.c_str(), 0, REG_SZ,
			(const BYTE*)strItemValue.c_str(), static_cast<DWORD>((strItemValue.size() +
			1) * sizeof(TCHAR)));
	else
		RegDeleteValue(hKey, strItemName.c_str());

	RegCloseKey(hKey);
}

std_string GetNetInstallRoot()
{
	std_string str;

	HKEY hNet = NULL;
	LONG lRes = RegOpenKeyEx(HKEY_LOCAL_MACHINE, _T("SOFTWARE\\Microsoft\\.NETFramework"),
		0, KEY_READ, &hNet);
	if((lRes != ERROR_SUCCESS) || (hNet == NULL)) return str;

	const DWORD cbData = 2050;
	BYTE pbData[cbData];
	ZeroMemory(pbData, cbData * sizeof(BYTE));
	DWORD dwData = cbData - 2;
	lRes = RegQueryValueEx(hNet, _T("InstallRoot"), NULL, NULL, pbData, &dwData);
	if(lRes == ERROR_SUCCESS) str = (LPCTSTR)(LPTSTR)pbData;

	RegCloseKey(hNet);
	return str;
}

std_string GetKeePassExePath()
{
	const DWORD cbData = 2050;
	TCHAR tszName[cbData];
	ZeroMemory(tszName, cbData * sizeof(TCHAR));

	GetModuleFileName(NULL, tszName, cbData - 2);

	for(int i = static_cast<int>(_tcslen(tszName)) - 1; i >= 0; --i)
	{
		if(tszName[i] == _T('\\')) break;
		else tszName[i] = 0;
	}

	std_string strPath = tszName;
	boost::trim_if(strPath, boost::is_any_of(g_lpPathTrimChars));
	if(strPath.size() == 0) return strPath;

	return strPath + _T("KeePass.exe");
}

void EnsureTerminatingSeparator(std_string& strPath)
{
	if(strPath.size() == 0) return;
	if(strPath.c_str()[strPath.size() - 1] == _T('\\')) return;

	strPath += _T("\\");
}

std_string FindNGen()
{
	std_string strNGen;

	std_string strRoot = GetNetInstallRoot();
	if(strRoot.size() == 0) return strNGen;
	EnsureTerminatingSeparator(strRoot);

	ULONGLONG ullVersion = 0;
	FindNGenRec(strRoot, strNGen, ullVersion);

	return strNGen;
}

#pragma warning(push)
#pragma warning(disable: 4127) // Conditional expression is constant
void FindNGenRec(const std_string& strPath, std_string& strNGenPath,
	ULONGLONG& ullVersion)
{
	const std_string strSearch = strPath + _T("*.*");
	const std_string strNGen = _T("ngen.exe");

	WIN32_FIND_DATA wfd;
	ZeroMemory(&wfd, sizeof(WIN32_FIND_DATA));
	HANDLE hFind = FindFirstFile(strSearch.c_str(), &wfd);
	if(hFind == INVALID_HANDLE_VALUE) return;

	while(true)
	{
		if((wfd.cFileName[0] == 0) || (_tcsicmp(wfd.cFileName, _T(".")) == 0) ||
			(_tcsicmp(wfd.cFileName, _T("..")) == 0)) { }
		else if((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
			FindNGenRec((strPath + wfd.cFileName) + _T("\\"), strNGenPath, ullVersion);
		else if(_tcsicmp(wfd.cFileName, strNGen.c_str()) == 0)
		{
			const std_string strFullPath = strPath + strNGen;
			const ULONGLONG ullThisVer = SiuGetFileVersion(strFullPath);
			if(ullThisVer >= ullVersion)
			{
				strNGenPath = strFullPath;
				ullVersion = ullThisVer;
			}
		}

		if(FindNextFile(hFind, &wfd) == FALSE) break;
	}

	FindClose(hFind);
}
#pragma warning(pop)

ULONGLONG SiuGetFileVersion(const std_string& strFilePath)
{
	DWORD dwDummy = 0;
	const DWORD dwVerSize = GetFileVersionInfoSize(
		strFilePath.c_str(), &dwDummy);
	if(dwVerSize == 0) return 0;

	boost::scoped_array<BYTE> vVerInfo(new BYTE[dwVerSize]);
	if(vVerInfo.get() == NULL) return 0; // Out of memory

	if(GetFileVersionInfo(strFilePath.c_str(), 0, dwVerSize,
		vVerInfo.get()) == FALSE) return 0;

	VS_FIXEDFILEINFO* pFileInfo = NULL;
	UINT uFixedInfoLen = 0;
	if(VerQueryValue(vVerInfo.get(), _T("\\"), (LPVOID*)&pFileInfo,
		&uFixedInfoLen) == FALSE) return 0;
	if(pFileInfo == NULL) return 0;

	return ((static_cast<ULONGLONG>(pFileInfo->dwFileVersionMS) <<
		32) | static_cast<ULONGLONG>(pFileInfo->dwFileVersionLS));
}

void CheckDotNetInstalled()
{
	OSVERSIONINFO osv;
	ZeroMemory(&osv, sizeof(OSVERSIONINFO));
	osv.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&osv);
	if(osv.dwMajorVersion >= 6) return; // .NET ships with Vista and higher

	const std_string strNGen = FindNGen();
	if(strNGen.size() == 0)
	{
		std_string strMsg = _T("KeePass 2.x requires the Microsoft .NET Framework >= 2.0, ");
		strMsg += _T("however this framework currently doesn't seem to be installed ");
		strMsg += _T("on your computer. Without this framework, KeePass will not run.\r\n\r\n");
		strMsg += _T("The Microsoft .NET Framework is available as free download from the ");
		strMsg += _T("Microsoft website.\r\n\r\n");
		strMsg += _T("Do you want to visit the Microsoft website now?");

		const int nRes = MessageBox(NULL, strMsg.c_str(), _T("KeePass Setup"),
			MB_ICONQUESTION | MB_YESNO);
		if(nRes == IDYES)
		{
			SHELLEXECUTEINFO sei;
			ZeroMemory(&sei, sizeof(SHELLEXECUTEINFO));
			sei.cbSize = sizeof(SHELLEXECUTEINFO);
			sei.lpVerb = _T("open");
			sei.lpFile = _T("http://msdn.microsoft.com/en-us/netframework/aa569263.aspx");
			sei.nShow = SW_SHOW;
			ShellExecuteEx(&sei);
		}
	}
}
