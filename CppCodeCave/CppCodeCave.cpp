#define _CRT_SECURE_NO_WARNINGS 

#include <iostream>
#include <Windows.h>
#include <TlHelp32.h>
#include <cstdlib>
#include <stdio.h>
#include <string.h>
#include <tchar.h>

typedef int(__stdcall *__MessageBoxA)(HWND, LPCSTR, LPCSTR, UINT);

/* wchar_t to char */
char* wtoc(const wchar_t* w, size_t max)
{
	char* c = new char[max];
	wcstombs(c, w, max);
	return c;
}

/* char to wchar_t */
wchar_t* ctow(const char* c, size_t max)
{
	wchar_t* w = new wchar_t[max];
	mbstowcs(w, c, max);
	return w;
}

class cavedata
{
public:
	char chMessage[256];
	char chTitle[256];

	DWORD paMessageBoxA;
};

DWORD GetProcId(char* procname)
{
	PROCESSENTRY32 pe;
	HANDLE hSnap;

	pe.dwSize = sizeof(PROCESSENTRY32);
	hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
	if (Process32First(hSnap, &pe))
	{
		do
		{
			if (strcmp(wtoc(pe.szExeFile, strlen(procname)), procname) == 0)
				break;
		} while (Process32Next(hSnap, &pe));
	}
	return pe.th32ProcessID;
}

DWORD __stdcall RemoteThread(cavedata *cData)
{
	__MessageBoxA MsgBox = (__MessageBoxA)cData->paMessageBoxA;
	MsgBox(NULL, cData->chMessage, cData->chTitle, MB_ICONINFORMATION);
	return EXIT_SUCCESS;
}

int main() 
{
	cavedata CaveData;
	ZeroMemory(&CaveData, sizeof(cavedata));
	strcpy(CaveData.chMessage, "Hi, I was called from remote process!");
	strcpy(CaveData.chTitle, "Hello from code cave!");

	HINSTANCE hUserModule = LoadLibrary(TEXT("user32.dll"));
	CaveData.paMessageBoxA = (DWORD)GetProcAddress(hUserModule, "MessageBoxA");
	FreeLibrary(hUserModule);

	HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetProcId("calc.exe"));
	LPVOID pRemoteThread = VirtualAllocEx(hProcess, NULL, sizeof(cavedata), MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
	WriteProcessMemory(hProcess, pRemoteThread, (LPVOID)RemoteThread, sizeof(cavedata), 0);
	cavedata *pData = (cavedata*)VirtualAllocEx(hProcess, NULL, sizeof(cavedata), MEM_COMMIT, PAGE_READWRITE);
	WriteProcessMemory(hProcess, pData, &CaveData, sizeof(cavedata), NULL);
	HANDLE  hThread = CreateRemoteThread(hProcess, 0, 0, (LPTHREAD_START_ROUTINE)pRemoteThread, pData, 0, 0);
	CloseHandle(hThread);
	VirtualAllocEx(hProcess, pRemoteThread, sizeof(cavedata), MEM_RELEASE, PAGE_EXECUTE_READWRITE);
	CloseHandle(hProcess);

	getchar();
	return 0;
}