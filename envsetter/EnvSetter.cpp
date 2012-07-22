// EnvSetter.cpp : Defines the entry point for the DLL application.
//


#include "stdafx.h"
#include <crtdbg.h>
BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	if(ul_reason_for_call==DLL_PROCESS_ATTACH)
		{

		HKEY hEnvKey=NULL;	
		try
			{            						
			DWORD dwStaus=RegOpenKeyEx(HKEY_LOCAL_MACHINE,TEXT("SOFTWARE\\Profiler#"),0,KEY_QUERY_VALUE ,&hEnvKey);
			if(dwStaus !=ERROR_SUCCESS || hEnvKey==NULL)
				throw NULL;

			DWORD dwType=0;
			BYTE EnvSetup[16];
			ULONG lKeySize= 16;

			dwStaus=RegQueryValueEx(hEnvKey,TEXT("EnvSetup"),NULL,&dwType,EnvSetup,&lKeySize);
			if(dwStaus !=ERROR_SUCCESS || dwType !=REG_DWORD)
				throw NULL;	

			int iEnvSetup=(int)(*EnvSetup);
            if(iEnvSetup==0)
				{
					SetEnvironmentVariable("COR_PROFILER","{383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D}");
					SetEnvironmentVariable("COR_ENABLE_PROFILING","0x0");		
					goto A;
				}
			else
				{
						SetEnvironmentVariable("COR_PROFILER","{383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D}");
						SetEnvironmentVariable("COR_ENABLE_PROFILING","0x1");				
				}
			}
		catch(...)
			{
				SetEnvironmentVariable("COR_PROFILER","{383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D}");
				SetEnvironmentVariable("COR_ENABLE_PROFILING","0x1");	
			}
A:		if(hEnvKey!=NULL)
			{
			try{RegCloseKey(hEnvKey);}catch(...){}
			}
		}

	return FALSE;//set environment and unload
}

