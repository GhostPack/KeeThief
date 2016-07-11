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

#ifndef ___SH_INST_UTIL_H___
#define ___SH_INST_UTIL_H___

#pragma once

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN
#endif

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#ifndef WINVER
#define WINVER 0x0501
#endif
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0501
#endif						
#ifndef _WIN32_WINDOWS
#define _WIN32_WINDOWS 0x0410
#endif
#ifndef _WIN32_IE
#define _WIN32_IE 0x0600
#endif

#ifndef MMNOMIDI
#define MMNOMIDI
#endif
#ifndef MMNOAUX
#define MMNOAUX
#endif
#ifndef MMNOMIXER
#define MMNOMIXER
#endif

#if (_MSC_VER >= 1400) // Manifest linking
#if defined(_M_IX86)
#pragma comment(linker, "/manifestdependency:\"type='win32' " \
	"name='Microsoft.Windows.Common-Controls' " \
	"version='6.0.0.0' " \
	"processorArchitecture='x86' " \
	"publicKeyToken='6595b64144ccf1df' " \
	"language='*'\"")
#elif defined(_M_AMD64)
#pragma comment(linker, "/manifestdependency:\"type='win32' " \
	"name='Microsoft.Windows.Common-Controls' " \
	"version='6.0.0.0' " \
	"processorArchitecture='amd64' " \
	"publicKeyToken='6595b64144ccf1df' " \
	"language='*'\"")
#elif defined(_M_IA64)
#pragma comment(linker, "/manifestdependency:\"type='win32' " \
	"name='Microsoft.Windows.Common-Controls' " \
	"version='6.0.0.0' " \
	"processorArchitecture='ia64' " \
	"publicKeyToken='6595b64144ccf1df' " \
	"language='*'\"")
#endif
#endif // (_MSC_VER >= 1400)

#include <windows.h>
#include <commctrl.h>
#include <shellapi.h>
#include <tchar.h>
#include <string>
#include <vector>
#include <algorithm>

typedef std::basic_string<TCHAR> std_string;

int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
	LPTSTR lpCmdLine, int nCmdShow);

void UpdateNativeImage(bool bInstall);
void RegisterPreLoad(bool bRegister);
std_string GetNetInstallRoot();
std_string GetKeePassExePath();
void EnsureTerminatingSeparator(std_string& strPath);
std_string FindNGen();
void FindNGenRec(const std_string& strPath, std_string& strNGenPath,
	ULONGLONG& ullVersion);
ULONGLONG SiuGetFileVersion(const std_string& strFilePath);
void CheckDotNetInstalled();

#endif // ___SH_INST_UTIL_H___
