// SharpDelegator.cpp : Implementation of CSharpDelegator

#include "stdafx.h"
#include "SharpDelegator.h"
#include "psapi.h"

// CSharpDelegator

	STDMETHODIMP CSharpDelegator::RefreshProfileeStatus(BSTR SystemName, BSTR* profileeStatus)
		{		
		USES_CONVERSION;
		CLock lock (g_cs);
		try
			{
				SysFreeString(SystemName);
			}
		catch(...){}

		DWORD dwProcessIds[512];
		DWORD dwSizeReturned=0;
		DWORD noOfProcesses=0;
		EnumProcesses(dwProcessIds,sizeof(DWORD)*512,&dwSizeReturned);
		noOfProcesses=dwSizeReturned/ sizeof(DWORD);

		string returnString="";

		for(UINT x=0;x<noOfProcesses;x++)
			{
				char buffer[16];
				string thisString="Global\\";
				thisString+=ltoa(dwProcessIds[x],buffer,10);
				thisString+="?";

				HANDLE hMutex=NULL;
				try
					{						
						hMutex=OpenMutex (1,FALSE,(thisString+"FALSE").data())	;
					}
				catch(...){}
				if(hMutex)
					{
							returnString+= thisString+"FALSE";
							returnString+="?";							

							HANDLE hProcess = NULL;
							try
								{
									hProcess = OpenProcess( PROCESS_QUERY_INFORMATION |
												PROCESS_VM_READ, FALSE, dwProcessIds[x] );
								}
							catch(...){}
								   if ( hProcess )
										{								
											TCHAR szProcessName[512];
											GetModuleBaseName( hProcess,NULL,szProcessName,512);
											returnString+=szProcessName;
											CloseHandle( hProcess );
										}
							
							ReleaseMutex(hMutex);
							CloseHandle(hMutex);
							hMutex=NULL;
							returnString+="\n";
							continue;
					}
				try
					{
						hMutex=OpenMutex(1,FALSE,(thisString+"TRUE").data());
					}
				catch(...){}
				if(hMutex)
					{
							returnString+= thisString+"TRUE";
							returnString+="?";

							HANDLE hProcess = NULL;
							try
								{
									hProcess = OpenProcess( PROCESS_QUERY_INFORMATION |
												PROCESS_VM_READ, FALSE, dwProcessIds[x] );
								}
							catch(...){}
								   if ( hProcess )
										{								
											TCHAR szProcessName[512];
											GetModuleBaseName( hProcess,NULL,szProcessName,512);
											returnString+=szProcessName;
											CloseHandle( hProcess );
										}
							
							ReleaseMutex(hMutex);
							CloseHandle(hMutex);
							hMutex=NULL;		
							returnString+="\n";
					}
				
			}

		returnString+="\n";
		CComBSTR bstrReturnString=returnString.data();
		*profileeStatus=bstrReturnString.Detach(); 

		return S_OK;
		}


	STDMETHODIMP CSharpDelegator::DeferMethodCall(LONG pid, BSTR methodString, LONG lParam)
		{
			CLock lock (g_cs);
			if(!hProfiler)
				{
					try
						{
							SysFreeString(methodString);
						}
					catch(...){}
					return E_FAIL;
				}

			USES_CONVERSION;

			LPDWORD proc=(LPDWORD)(GetProcAddress( hProfiler, W2A(methodString)));
			if(proc==NULL)
				{
					HRESULT hr=HRESULT_FROM_WIN32(GetLastError());
					try
						{
							SysFreeString(methodString);
						}
					catch(...){}
					return hr;
				}

			HANDLE hProfileeProcess=NULL;
			while(true)
				{
					hProfileeProcess=OpenProcess(PROCESS_ALL_ACCESS,FALSE,pid);
					if(hProfileeProcess)
						break;
					hProfileeProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_DUP_HANDLE|PROCESS_VM_OPERATION
						|PROCESS_VM_WRITE |PROCESS_VM_READ|PROCESS_QUERY_INFORMATION ,FALSE,pid);
					if(hProfileeProcess)
						break;
					hProfileeProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_OPERATION
						|PROCESS_VM_WRITE |PROCESS_VM_READ|PROCESS_QUERY_INFORMATION ,FALSE,pid);
					if(hProfileeProcess)
						break;					
					hProfileeProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_OPERATION|PROCESS_VM_WRITE ,FALSE,pid);
					if(hProfileeProcess)
						break;
					hProfileeProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_WRITE ,FALSE,pid);
					if(hProfileeProcess)
						break;
					hProfileeProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_READ ,FALSE,pid);
					//if(hProfileeProcess && GetLastError()==ERROR_SUCCESS) //DO U WANT TO STAY IN THE INFINITE LOOP ???
						break;
						//Sleep (100);
				}

			if(hProfileeProcess==NULL)
				{
					HRESULT hr=HRESULT_FROM_WIN32(GetLastError());
					try
						{
							SysFreeString(methodString);
						}
					catch(...){}
					return hr;
				}
			HANDLE hThread=NULL;
			if(lParam)
				{
					hThread=CreateRemoteThread(hProfileeProcess,NULL,0,(LPTHREAD_START_ROUTINE)proc,(LPVOID)lParam,0,NULL); 
				}
			else
				{
					hThread=CreateRemoteThread(hProfileeProcess,NULL,0,(LPTHREAD_START_ROUTINE)proc,NULL,0,NULL); 

				}

			if(!hThread)
				{
					HRESULT hr=HRESULT_FROM_WIN32(GetLastError());
					try	{SysFreeString(methodString);}catch(...){}
					try	{CloseHandle(hProfileeProcess);}catch(...){}					
					return hr;
				}

			//PUT IN A SEPARATE THREAD
				DWORD dwStatus=E_FAIL;
				BOOL success;
				while(true)
				{
					success=GetExitCodeThread(hThread,&dwStatus);
					if(success==FALSE || dwStatus !=0x00000103L)//0x00000103L :-  status_running
						break;
					Sleep(100);
				} 
			//PUT IN A SEPARATE THREAD


			////////////////////
				
				try	{SysFreeString(methodString);}catch(...){}
				try	{CloseHandle(hProfileeProcess);}catch(...){}
				try	{CloseHandle(hThread);}catch(...){}

				return dwStatus;				
			
		}	

	STDMETHODIMP CSharpDelegator::put_profilerName(BSTR newVal)
		{		
			CLock lock (g_cs);

			USES_CONVERSION;
			HRESULT hr=S_OK;
			if(!PathFileExists(W2A(newVal)))
			{
				return HRESULT_FROM_WIN32(GetLastError()); 
			}

			if(hProfiler==NULL)
				{	
					hProfiler=LoadLibrary(W2A(newVal));	
					if(!hProfiler)
					{
						hr=HRESULT_FROM_WIN32(GetLastError());						
					}
					else
					{
						hr=S_OK;
					}
				}	
			else
				{
					hr=HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
				}
			
			try {SysFreeString(newVal);} catch(...){}
			return hr;

		}

    



	STDMETHODIMP CSharpDelegator::InstallServiceEnvironment(BSTR EnvSetter,BSTR targetProcess)
		{

		CLock lock (g_cs);
		USES_CONVERSION;

		if(!PathFileExists(W2A(EnvSetter)))
		{
			return HRESULT_FROM_WIN32(GetLastError()); 
		}
		HMODULE hKernelModule=LoadLibraryA("Kernel32.dll");
		FARPROC procLoadLibrary=GetProcAddress(hKernelModule,"LoadLibraryA");
		//try{CloseHandle(hKernelModule);}catch(...){}
		if(procLoadLibrary==NULL)
			{
				HRESULT hr=HRESULT_FROM_WIN32(GetLastError()); 	
				try{SysFreeString(EnvSetter) ;} catch(...){}
				try{SysFreeString(targetProcess) ;} catch(...){}			
				return hr;
			}        

		DWORD dwProcessIds[512];
		DWORD dwSizeReturned=0;
		DWORD noOfProcesses=0;
		EnumProcesses(dwProcessIds,sizeof(DWORD)*512,&dwSizeReturned);
		noOfProcesses=dwSizeReturned/ sizeof(DWORD);

		string svcName="";

		HRESULT hr=HRESULT_FROM_WIN32(ERROR_NOT_FOUND) ;

		for(UINT x=0;x<noOfProcesses;x++)
			{
				HANDLE hProcess = NULL;
							try
								{
									hProcess = OpenProcess( PROCESS_QUERY_INFORMATION |
												PROCESS_VM_READ, FALSE, dwProcessIds[x] );
								}
							catch(...){}
								   if ( hProcess )
										{
											TCHAR szProcessName[512];
											GetModuleBaseName( hProcess,NULL,szProcessName,512);
											svcName=szProcessName;
											try{CloseHandle( hProcess );}catch(...){}
											hProcess =NULL;
										}	
								   else
									   {
											continue;
									   }

									if(targetProcess!=NULL)
										{
											if(wcslen(targetProcess)!=0)
												{
													CComBSTR _bstrProcess=svcName.data() ;
													_bstrProcess.ToUpper(); 

													CComBSTR _bstrParam=targetProcess;
													_bstrParam.ToUpper(); 

													if(_bstrProcess!=_bstrParam)
													{
															continue;
													}
												}
										}

								   hr=S_OK;

								   try
									   {
										while(true)
												{
												try{CloseHandle( hProcess );}catch(...){}
													hProcess=NULL;

													hProcess=OpenProcess(PROCESS_ALL_ACCESS,FALSE,dwProcessIds[x]);
													if(hProcess)
														break;
													hProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_DUP_HANDLE|PROCESS_VM_OPERATION
														|PROCESS_VM_WRITE |PROCESS_VM_READ|PROCESS_QUERY_INFORMATION ,FALSE,dwProcessIds[x]);
													if(hProcess)
														break;
													hProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_OPERATION
														|PROCESS_VM_WRITE |PROCESS_VM_READ|PROCESS_QUERY_INFORMATION ,FALSE,dwProcessIds[x]);
													if(hProcess)
														break;					
													hProcess=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_VM_OPERATION|PROCESS_VM_WRITE ,FALSE,dwProcessIds[x]);
														break;
												}
											if(!hProcess)
													throw NULL;

											/////////////////

											char * pszLibName=W2A(EnvSetter);
											int cnt=256;//sizeof(pszLibName)+1;//1 for the null
											
											char *pszLibFileRemote = (PSTR)::VirtualAllocEx(
												hProcess, 
												NULL, 
												cnt, 
												MEM_COMMIT, 
												PAGE_READWRITE
												);

											if(!pszLibFileRemote)
												{
													throw NULL;
												}
											
											BOOL bWritten=::WriteProcessMemory(
												hProcess, 
												(void*)pszLibFileRemote, 
												(void*)(pszLibName), 
												cnt, 
												NULL) ;	

											if(!bWritten)
												{
													throw NULL;
												}

											HANDLE hThread=CreateRemoteThread(hProcess,NULL,0,(LPTHREAD_START_ROUTINE)procLoadLibrary ,pszLibFileRemote,0,NULL);
											if(!hThread)
												{
													throw NULL;
												}
											DWORD dwStatus=E_FAIL;
											BOOL success=FALSE;
											while(true)
												{
													BOOL success=GetExitCodeThread(hThread,&dwStatus);
													if(success==FALSE || dwStatus !=0x00000103L)//0x00000103L :-  status_running
													break;
													Sleep(100);
												}
											try{CloseHandle(hThread);}catch(...){}
											hThread=NULL;											
                                            
									   }
								   catch(...)
									   {									

//											hr=HRESULT_FROM_WIN32(ERROR_PARTIAL_COPY);
									   }
				if(hProcess)
					try{CloseHandle(hProcess);}catch(...){}
					hProcess =NULL;
			}

		try{SysFreeString(EnvSetter);}catch(...){}
		try{SysFreeString(targetProcess) ;} catch(...){}			
		return hr;
		}
