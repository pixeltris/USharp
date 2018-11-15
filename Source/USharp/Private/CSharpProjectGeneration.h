#pragma once

#if PLATFORM_WINDOWS

//#include "Windows/AllowWindowsPlatformTypes.h"

// Define WIN32_LEAN_AND_MEAN to exclude rarely-used services from windows headers.
//#define WIN32_LEAN_AND_MEAN
//#include <windows.h>
//#include <metahost.h>
//#pragma comment(lib, "mscoree.lib")
//
//#include "Windows/HideWindowsPlatformTypes.h"

#endif

class CSharpProjectGeneration
{
	static bool CopySolutionTemplate();
	static bool GenerateProjectVariablesFile();

public:
	static bool GenerateProject();
};