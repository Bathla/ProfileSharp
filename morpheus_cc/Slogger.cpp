// Slogger.cpp : Implementation of CSlogger

#include "stdafx.h"
#include "Slogger.h"
#include "StackInfo.h"
#include "CalleeFunctionInfo.h"
#include ".\slogger.h"

// CSlogger
	
	HRESULT __stdcall CSlogger::GarbageCollectionStarted( 
            int cGenerations,
            BOOL generationCollected[  ],
            COR_PRF_GC_REASON reason)
			{
				return S_OK;
			}
		
		HRESULT __stdcall CSlogger::SurvivingReferences(
                unsigned long cSurvivingObjectIDRanges, 
                UINT_PTR* objectIDRangeStart, 
                unsigned long* cObjectIDRangeLength)
		{
			return S_OK;
		}

		HRESULT __stdcall CSlogger::ThreadNameChanged(
              ThreadID threadId,
              ULONG cchName,
             WCHAR name[  ])
		{
			return S_OK;
		}

		
		HRESULT __stdcall CSlogger::GarbageCollectionFinished()
		{
			return S_OK;
		}
			
		HRESULT __stdcall CSlogger::FinalizeableObjectQueued(
                unsigned long finalizerFlags, 
                UINT_PTR objectId)
		{
			return S_OK;
		}
			
		HRESULT __stdcall CSlogger::RootReferences2(
                unsigned long cRootRefs, 
                UINT_PTR* rootRefIds, 
                COR_PRF_GC_ROOT_KIND* rootKinds, 
                COR_PRF_GC_ROOT_FLAGS* rootFlags, 
                UINT_PTR* rootIds)
		{
			return S_OK;
		}
				
		HRESULT __stdcall CSlogger::HandleCreated(
                UINT_PTR handleId, 
                UINT_PTR initialObjectId)
				{
					return S_OK;
				}
		HRESULT __stdcall CSlogger::HandleDestroyed(UINT_PTR handleId)
		{
			return S_OK;
		}

	
HRESULT __stdcall CSlogger::Initialize( IUnknown *pICorProfilerInfoUnk )
{
	CLock lock(g_csFunction);	

	g_hWaitToDumpObjectData=NULL;

	#ifdef _DEBUG
	Sleep(10000);
	#endif
	
		
	if(hGlobalDebugCheckInit==NULL)
		{
			try
			{
				DWORD dwID;
				hGlobalDebugCheckInit=::CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
			}catch(...){}
		}

	char buffer[32];
	g_pid=GetCurrentProcessId();


		CString strMutexString;
	if(true)
		{ 

			/////should Suspend on start? ////////
			if(m_bSuspendOnStart)
			{
				m_bSuspendOnStart=false;	//		m_bSuspendOnStart can not be true on startup
			}	
			try
			{
				IsProcessSuspended();
			}
			catch(...)
			{
				m_bSuspendOnStart=false;			
			}

			strMutexString="Global\\";
			strMutexString.Append(ltoa(g_pid,buffer,10));
			strMutexString.Append("?");
			strMutexString += (m_bSuspendOnStart==true)?"TRUE":"FALSE" ;


			////////////Is Object Allocation monitored
			if(m_bObjectAllocation )
			{
				m_bObjectAllocation=false;	//		m_bObjectAllocation can not be true on startup
			}	
			try
			{
				m_bObjectAllocation=IsObjectAllocationMonitored();
			}
			catch(...)
			{
				m_bObjectAllocation=false;			
			}  
			//////////////
			if(m_bObjectAllocation)
			{
				CString strOAMutexString="Global\\";
				strOAMutexString.Append(ltoa(g_pid,buffer,10));
				strOAMutexString.Append("?OA");
				m_hOAMutex=NULL;
				m_hOAMutex=CreateMutex(NULL,false,strOAMutexString);
			}
		}

	
	////////////////////Resolve Filters///////////////
	try
		{
			ResolveFilters();
		}catch(...){}

	//////////////////////////////////////////////////

	HRESULT hr;
    hr = pICorProfilerInfoUnk->QueryInterface( __uuidof(ICorProfilerInfo2),
                                               (void **)&m_pCorProfilerInfo2); 
	
	
	if(FAILED(hr) || !m_pCorProfilerInfo2)
	{		
		m_bIsTwoDotO=false;
		hr=pICorProfilerInfoUnk->QueryInterface( __uuidof(ICorProfilerInfo),
                                               (void **)&m_pCorProfilerInfo2); 

		if(true)
			{
	
				hr=m_pCorProfilerInfo2->SetEnterLeaveFunctionHooks((FunctionEnter *)&EnterNaked,
																	(FunctionLeave *)&LeaveNaked,
																	(FunctionTailcall *)&TailcallNaked );   
				if(FAILED(hr))
				{
					SetLastError(E_INVALIDARG);  
					return E_INVALIDARG;
				}	
			}
		
	}
	else
	{
		m_bIsTwoDotO=true;
		/*hr = m_pCorProfilerInfo2->SetEnterLeaveFunctionHooks2( EnterNaked2,
                                                                LeaveNaked2,
                                                                TailcallNaked2 );//Can do it sometime later.*/
		if(true)
			{
	
				hr=m_pCorProfilerInfo2->SetEnterLeaveFunctionHooks((FunctionEnter *)&EnterNaked,
                                                               (FunctionLeave *)&LeaveNaked,
                                                               (FunctionTailcall *)&TailcallNaked );

			}
	}

	
	if ( FAILED(hr) )
	{	
		m_pCorProfilerInfo2->SetEventMask( COR_PRF_MONITOR_NONE);		
		m_dwEventMask=COR_PRF_MONITOR_NONE ;
//		m_pCorProfilerInfo2.Release();  // no need to release.It will be released in destructor
		SetLastError(HRESULTTOWIN32(hr));  
		return hr;
    }	

	if(false)
		{
				m_dwEventMask=(DWORD)(COR_PRF_MONITOR_MODULE_LOADS | COR_PRF_DISABLE_INLINING | COR_PRF_MONITOR_JIT_COMPILATION | COR_PRF_ENABLE_JIT_MAPS);				
		}

	else
		{
			if(IsInprocEnabled() || m_bObjectAllocation)
				{
					if(m_bIsTwoDotO)
						{	 //
							m_dwEventMask =
						(DWORD) (   COR_PRF_MONITOR_THREADS	|
									COR_PRF_DISABLE_INLINING |
									COR_PRF_ENABLE_JIT_MAPS |				
									COR_PRF_MONITOR_MODULE_LOADS|
									COR_PRF_MONITOR_EXCEPTIONS |
									COR_PRF_MONITOR_CLR_EXCEPTIONS
												| COR_PRF_MONITOR_ENTERLEAVE
												|COR_PRF_MONITOR_GC 												
												|COR_PRF_MONITOR_SUSPENDS 			                  
												| COR_PRF_MONITOR_CACHE_SEARCHES
												|COR_PRF_MONITOR_CODE_TRANSITIONS
												|COR_PRF_ENABLE_INPROC_DEBUGGING 
				//								|COR_PRF_ENABLE_FUNCTION_ARGS
				//								|COR_PRF_ENABLE_FRAME_INFO 
												|COR_PRF_ENABLE_STACK_SNAPSHOT
				//								|COR_PRF_MONITOR_CLASS_LOADS
												
									) ;
							if(m_bObjectAllocation)
								{	
									m_dwEventMask = (DWORD) (  m_dwEventMask|COR_PRF_ENABLE_OBJECT_ALLOCATED| COR_PRF_MONITOR_OBJECT_ALLOCATED);									
								}
							
						}
					else
						{
							m_dwEventMask =
						(DWORD) (   COR_PRF_MONITOR_THREADS	|
									COR_PRF_DISABLE_INLINING |
									COR_PRF_ENABLE_JIT_MAPS |				
									COR_PRF_MONITOR_MODULE_LOADS|
									COR_PRF_MONITOR_EXCEPTIONS |
									COR_PRF_MONITOR_CLR_EXCEPTIONS
												| COR_PRF_MONITOR_ENTERLEAVE
												|COR_PRF_MONITOR_GC 												
												|COR_PRF_MONITOR_SUSPENDS 			                  
												| COR_PRF_MONITOR_CACHE_SEARCHES
												|COR_PRF_MONITOR_CODE_TRANSITIONS
												|COR_PRF_ENABLE_INPROC_DEBUGGING 
				//								|COR_PRF_MONITOR_CLASS_LOADS
									) ;
							if(m_bObjectAllocation)
								{	
									m_dwEventMask = (DWORD) (  m_dwEventMask|COR_PRF_ENABLE_OBJECT_ALLOCATED| COR_PRF_MONITOR_OBJECT_ALLOCATED);
								}
					}
				if(IsInprocEnabled())
					{  
						CString strInprocMutexString="Global\\";
						strInprocMutexString.Append(ltoa(g_pid,buffer,10));
						strInprocMutexString.Append("?INPROC");
						m_hInprocMutex=NULL;
						m_hInprocMutex=CreateMutex(NULL,false,strInprocMutexString);
					}
				}	
			else
			{
				try
					{
						CanRunGCBeforeOC();
					}catch(...){m_bRunGC_BOC=true;}
				
				m_dwEventMask =
				(DWORD) (   COR_PRF_MONITOR_THREADS	|
						COR_PRF_DISABLE_INLINING |											
						COR_PRF_MONITOR_MODULE_LOADS | COR_PRF_MONITOR_GC 											
									|COR_PRF_MONITOR_SUSPENDS 			                  
									| COR_PRF_MONITOR_CACHE_SEARCHES
									|COR_PRF_MONITOR_CODE_TRANSITIONS		
						) ;

				if(m_bRunGC_BOC)  //Basic Memory Profiling
					{						
						m_dwEventMask= m_dwEventMask|COR_PRF_ENABLE_OBJECT_ALLOCATED 
									| COR_PRF_MONITOR_OBJECT_ALLOCATED;
					}
				else	//Exception Tracing
					{
						CString strExceptionMutexString="Global\\";
						strExceptionMutexString.Append(ltoa(g_pid,buffer,10));
						strExceptionMutexString.Append("?EXCEPTIONS");
						CreateMutex(NULL,false,strExceptionMutexString);
						m_dwEventMask= m_dwEventMask|COR_PRF_MONITOR_EXCEPTIONS |
						COR_PRF_MONITOR_CLR_EXCEPTIONS
									| COR_PRF_MONITOR_ENTERLEAVE ;
					}
			}
		}

	hr = m_pCorProfilerInfo2->SetEventMask( m_dwEventMask );
	
    if ( FAILED(hr) )
	{
		m_pCorProfilerInfo2->SetEventMask( COR_PRF_MONITOR_NONE);		
		m_dwEventMask=COR_PRF_MONITOR_NONE ;		
		SetLastError(HRESULTTOWIN32(hr));  		
		return hr;
    }		

	
	if(true)
		{
			m_hSuspensionMutexFlag=NULL;    
			m_hSuspensionMutexFlag=CreateMutex(NULL,false,strMutexString);
			DWORD dwLastError=GetLastError();
			if(m_hSuspensionMutexFlag==NULL || FAILED(HRESULT_FROM_WIN32(dwLastError)) || dwLastError==ERROR_ALREADY_EXISTS ||dwLastError==ERROR_ACCESS_DENIED)
			{	
				m_bSuspendOnStart=false;	//do not suspend processes if Mutex flag is not created.Mutex is critical for process resumption						
			}		
		}

	try
			{		
				if(m_bSuspendOnStart)
				{					
					CString strSyncEventString="Global\\";
					strSyncEventString.Append(ltoa(g_pid,buffer,10));
					strSyncEventString.Append("?SYNC_SUSPEND");
					g_hCLRHandle=NULL;
					g_hCLRHandle=CreateEvent(NULL,TRUE,FALSE,strSyncEventString);
					if(g_hCLRHandle) 
						{			
							HANDLE hAutoAttach=NULL;//V. Imp.
							if(CanAutoAttach())
							{
								HWND hwndClient=NULL;								
								hwndClient=FindWindowA(NULL," ProfileSharp Enterprise Edition");								
								if(hwndClient)
								{								
									SendMessage(hwndClient,WM_USER+911,g_pid ,0);
									CloseHandle(hwndClient);
								}	
								else
								{									
									CString strAutoAttachString="Global\\";
									strAutoAttachString.Append(ltoa(g_pid,buffer,10));
									strAutoAttachString.Append("?AUTO_ATTACH");
									hAutoAttach=CreateMutex(NULL,false,strAutoAttachString);									
								}
							}

							DWORD dwResult=0;							
							dwResult=WaitForSingleObject(g_hCLRHandle,INFINITE);//Wait On..
							///////////////////////
							try
							{	
								if(hAutoAttach)
									{
										ReleaseMutex(hAutoAttach);
										CloseHandle(hAutoAttach);
									}
							}catch(...){}
							hAutoAttach=NULL;
							/////////////////////
							if(dwResult==WAIT_TIMEOUT)
								{
									RestoreCLR();
								}
							else
								{
									try
									{	
										if(g_hCLRHandle)
											{
												CloseHandle(g_hCLRHandle);
											}
									}catch(...){}
									g_hCLRHandle=NULL;
								}
					}
				}
			}
			catch(...){}	
	return S_OK;
}

HRESULT __stdcall CSlogger::ModuleLoadFinished( ModuleID moduleID,HRESULT hrStatus  )
		{			
			/*if(!m_hInprocMutex)
				return S_OK;*/
			
			CComPtr<IMetaDataImport> pMetaDataImport = NULL;
			if(FAILED(hrStatus))
				goto A;			
			HRESULT hr;
			if(g_Slogger->binder==NULL)
			{
				try
				{
					hr=g_Slogger->binder.CoCreateInstance(L"CorSymBinder_SxS",NULL,CLSCTX_INPROC_SERVER);  												

				}catch(...){}
			}								
			
		try
		{	
			if(g_Slogger->binder!=NULL)
			{
				WCHAR moduleName[MAX_LENGTH];
				moduleName[0]=NULL;
				hr=m_pCorProfilerInfo2->GetModuleInfo( moduleID,NULL,MAX_LENGTH,NULL,moduleName,NULL);
				if (FAILED(hr))
					goto A;
				
				hr = m_pCorProfilerInfo2->GetModuleMetaData(moduleID, ofRead, IID_IMetaDataImport,
				(IUnknown** )&pMetaDataImport);
				if (FAILED(hr) || !pMetaDataImport)
					goto A;
				CComPtr<ISymUnmanagedReader> reader;
				hr = binder->GetReaderForFile(pMetaDataImport,moduleName,L".",&reader);
				if (FAILED(hr) || !reader)
					goto A;
				ULONG32 size=MAX_LENGTH;
				WCHAR szSymbolFileName[MAX_LENGTH];
				szSymbolFileName[0]=NULL;				
				hr=reader->GetSymbolStoreFileName(MAX_LENGTH,&size,szSymbolFileName);
				if(FAILED(hr) || !szSymbolFileName)
				{					
					reader.Release(); 
					goto A;
				}
				else
				{
					m_debugModuleSet.insert(moduleID);
					if(IS_PROFILEDPROCESSFOROBJECTS && WANT_OBJECT_ALLOCATION_DATA && WANT_SRC_ANALYSIS_ONLY)
						{
							m_FilterModuleSet.insert(moduleID);
						}
				}

				reader.Release(); 
			}
		}catch(...){}

A:		
		if(pMetaDataImport)
			pMetaDataImport.Release(); 

		/////////////////////////////////////////
		//m_FilterModuleSet is used as a common collection set for any kind of module filteration for performance profiling

		if(false)
		{
			//Do Nothing
		}
		else
			{
				try
					{					 
						if(FAILED(hrStatus))				
							{
								goto B;
							}
						if(m_functionModuleFilter.size()>0)  
							{

								if(m_FilterModuleSet.find(moduleID)!=m_FilterModuleSet.end())
										{
											goto B;
										}
								else
										{
									
											WCHAR moduleName[MAX_LENGTH];
											moduleName[0]=NULL;
											HRESULT hr=m_pCorProfilerInfo2->GetModuleInfo( moduleID,NULL,MAX_LENGTH,NULL,moduleName,NULL);
											if(FAILED(hr) || !moduleName)
											{
												m_FilterModuleSet.insert(moduleID);	   //We want to profile by default!
												goto B;
											}
											CString cstrModule(moduleName);
											cstrModule=cstrModule.Right(cstrModule.GetLength()- (cstrModule.ReverseFind('\\')+1)); 
											if(m_bFunctionModulePassthrough )
											{
												for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
												{	
													if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
														continue;

													if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0) 											
														{
															m_FilterModuleSet.insert(moduleID);
															goto B;
														}
												}

													goto B;
													
											}
											else
											{
												//block these 
												for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
												{	
													if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
														{
															m_FilterModuleSet.insert(moduleID);
															goto B;
														}

													if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0)
														{
															goto B;
														}
														
												}
														///Passed ..Allow to move on
														m_FilterModuleSet.insert(moduleID);
															goto B;

											}

									}
							}
				
				}
				catch(...){}
			}

		///////////////////////////////////////
B:		

C:		   //Leave

				return S_OK;
		}


HRESULT __stdcall CSlogger::ModuleUnloadFinished( ModuleID moduleID, HRESULT hrStatus )
		{
			if(SUCCEEDED(hrStatus))
			{
				try
				{
					m_debugModuleSet.erase(moduleID);

				}catch(...){}
				try
					{
						if(m_FilterModuleSet.find(moduleID)!=m_FilterModuleSet.end())
							{
								m_FilterModuleSet.erase(moduleID);
							}
					}catch(...){}
			}			
			return S_OK;
		}


	HRESULT __stdcall CSlogger::Shutdown()
{	
	
	CFC();
	COC(); 
//	ClearObjAllocCache();
	EndAll();
	if(binder)
	{
		try
		{
			binder.Release(); 
		}
		catch(...){}
	}

	try
		{		
		if(m_hInprocMutex && !m_bIsTwoDotO)
			{
				for(map<ThreadID,DWORD>::iterator iter=m_mapDebugMap.begin();iter!=m_mapDebugMap.end();iter++)
				{
					m_pCorProfilerInfo2->EndInprocDebugging(iter->second); 
				}
			}
		}catch(...){}
		m_mapDebugMap.clear(); 
	return S_OK;
}

void CSlogger::Enter(FunctionID funcID)
{
	if(m_dwFunctionFilter)	//means performance profiling
		{
			if(g_hWaitToDumpObjectData) 
				{
					WaitForSingleObject( g_hWaitToDumpObjectData,300*(1000));//Wait On..300 Secs is enough				
				}
		}

	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)		   
		{	
		//SYNCHRONIZATION B/W CSLOGGER::ENTER AND DOD/DFD IS REQUIRED
			if(WANT_NO_FUNCTION_CALLEE_INFORMATION)
				{
				if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
				}
			ThreadID tid;
			HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
			FunctionInfo* pFunctionInfo = GetThreadInfo(tid)->GetFunctionInfo( funcID );
			GetThreadInfo(tid)->GetStackInfo()->PushFunction( pFunctionInfo);
		}

	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
					ThreadID tid;
					HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
					FunctionInfo* pFunctionInfo = GetThreadInfo(tid)->GetFunctionInfo( funcID );
					GetThreadInfo(tid)->GetStackInfo()->PushFunction( pFunctionInfo);
				}
		}
}

void CSlogger::Leave(FunctionID funcID)
{
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
		{
			UINT64 ui64EndTime=rdtsc(); //call rdtsc() as early as possible when popping
			if(WANT_NO_FUNCTION_CALLEE_INFORMATION)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
				}
			ThreadID tid;			
			HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
			if(SUCCEEDED(hr)&& tid )
			{	
				GetThreadInfo(tid)->GetStackInfo()->PopFunction(ui64EndTime);
			}

		}
	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
				
					UINT64 ui64EndTime=0; //call rdtsc() as early as possible when popping
					ThreadID tid;			
					HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
					if(SUCCEEDED(hr)&& tid )
					{	
						GetThreadInfo(tid)->GetStackInfo()->PopFunction(ui64EndTime);
					}
				}
		}
}

void CSlogger::Tailcall(FunctionID funcID)
{
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
		{
			ThreadID tid;
			UINT64 ui64TailTime=rdtsc();
			if(WANT_NO_FUNCTION_CALLEE_INFORMATION)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
				}
			HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
			if(SUCCEEDED(hr)&& tid )
			{
				GetThreadInfo(tid)->GetStackInfo()->PopFunction(ui64TailTime );			
			}
		}
	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(funcID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return;	
								}
						}
					UINT64 ui64EndTime=0; //call rdtsc() as early as possible when popping
					ThreadID tid;			
					HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
					if(SUCCEEDED(hr)&& tid )
					{	
						GetThreadInfo(tid)->GetStackInfo()->PopFunction(ui64EndTime);
					}
				}
		}
} 

HRESULT __stdcall CSlogger::UnmanagedToManagedTransition( FunctionID functionID, COR_PRF_TRANSITION_REASON reason )
{
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
		{
			if(WANT_FUNCTION_MANAGED_ONLY)
				{
					return S_OK;
				}
			if ( reason == COR_PRF_TRANSITION_RETURN )
			{
				if(WANT_NO_FUNCTION_CALLEE_INFORMATION)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(functionID,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return S_OK;	
								}
						}
				}
				ThreadID tid;
				HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
				if(SUCCEEDED(hr)&& tid )
				{
					FunctionInfo* pFunctionInfo = GetThreadInfo(tid)->GetFunctionInfo( functionID );
					GetThreadInfo(tid)->GetStackInfo()->PushFunction( pFunctionInfo );
				}
			}
		}
	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					if ( reason == COR_PRF_TRANSITION_RETURN )
						{
								if(m_FilterModuleSet.size()>0)
									{
										ModuleID modID=NULL;
										ClassID classId=NULL;					
										m_pCorProfilerInfo2->GetFunctionInfo(functionID,&classId,&modID,NULL); 
										if(!CanContinue(classId,modID))
											{
												return S_OK;	
											}
									}
								ThreadID tid;
								HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
								FunctionInfo* pFunctionInfo = GetThreadInfo(tid)->GetFunctionInfo( functionID );
								GetThreadInfo(tid)->GetStackInfo()->PushFunction( pFunctionInfo);
						}
				}
		}

			return S_OK;
}

HRESULT __stdcall CSlogger::ManagedToUnmanagedTransition( FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
{
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
	{
			if(WANT_FUNCTION_MANAGED_ONLY)
				{
					return S_OK;
				}

			if ( reason == COR_PRF_TRANSITION_CALL )
			{
				UINT64 llCycleCount=rdtsc() ;
				if(WANT_NO_FUNCTION_CALLEE_INFORMATION)
				{
					if(m_FilterModuleSet.size()>0)
						{
							ModuleID modID=NULL;
							ClassID classId=NULL;					
							m_pCorProfilerInfo2->GetFunctionInfo(functionId,&classId,&modID,NULL); 
							if(!CanContinue(classId,modID))
								{
									return S_OK;	
								}
						}
				}
				ThreadID tid;
				HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
				if(SUCCEEDED(hr)&& tid )
				{
					GetThreadInfo(tid)->GetStackInfo()->PopFunction(llCycleCount);
				}
			}
	}
	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					if ( reason == COR_PRF_TRANSITION_CALL)
						{
							if(m_FilterModuleSet.size()>0)
							{
								ModuleID modID=NULL;
								ClassID classId=NULL;					
								m_pCorProfilerInfo2->GetFunctionInfo(functionId,&classId,&modID,NULL); 
								if(!CanContinue(classId,modID))
									{
										return S_OK;	
									}
							}
							UINT64 llCycleCount=0;
							ThreadID tid;
							HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
							if(SUCCEEDED(hr)&& tid )
							{
								GetThreadInfo(tid)->GetStackInfo()->PopFunction(llCycleCount);
							}
						}
				}
		}


			return S_OK;
}

HRESULT __stdcall CSlogger::RuntimeSuspendStarted( COR_PRF_SUSPEND_REASON suspendReason )
		{
		if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
			{
				ThreadID tid;
				HRESULT hr=m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
				if(SUCCEEDED(hr) && tid)
				{
					GetThreadInfo(tid)->GetStackInfo()->SuspendFunction( rdtsc() );  
				}
			}
		
			m_mapSwapObjectAllocation.clear(); 
			m_mapTempObjectIDToObjectTag.clear();
			m_mapTempObjectIDToClassID.clear(); 
			return S_OK;
		}

		HRESULT __stdcall CSlogger::RuntimeResumeStarted()
		{
			
			//if filtered 
		if(CAN_PROFILE_FOR_OBJECTS && IS_PROFILEDPROCESSFOROBJECTS)
				{
					CAN_PROFILE_FOR_OBJECTS=false;					
				}
			
			if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
				{
					ThreadID tid;
					HRESULT hr=m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
					if(SUCCEEDED(hr) && tid)
					{
						GetThreadInfo(tid)->GetStackInfo()->ResumeFunction( rdtsc());  
					}
				}
		try
			{

			if(m_mapSwapObjectAllocation.size()>0)
				{
					m_mapObjectAllocation.clear(); 
					m_mapObjectAllocation.swap(m_mapSwapObjectAllocation); 
					m_mapSwapObjectAllocation.clear();
				}
			}catch(...){}

		try
			{
			if(m_mapTempObjectIDToObjectTag.size()>0)
			{
				m_mapObjectIDToObjectTag.clear();  
				m_mapObjectIDToObjectTag.swap(m_mapTempObjectIDToObjectTag); 
				m_mapTempObjectIDToObjectTag.clear();
			}
		}catch(...){}
			
	try{
			if(m_bIsTwoDotO && m_mapTempObjectIDToClassID.size()>0)
				{
					m_mapObjectIDToClassID.clear();  
					m_mapObjectIDToClassID.swap(m_mapTempObjectIDToClassID); 
					m_mapTempObjectIDToClassID.clear();
				}	
		}catch(...){}

			return S_OK;
		}

	HRESULT __stdcall CSlogger::RuntimeResumeFinished() 
		{
			if(CAN_PROFILE_FOR_OBJECTS && IS_PROFILEDPROCESSFOROBJECTS)
				{
					CAN_PROFILE_FOR_OBJECTS=false;
				}
		
			return S_OK;
		}

		HRESULT __stdcall CSlogger::ObjectAllocated( ObjectID objectID, ClassID classID )
		{				
				if(g_hWaitToDumpObjectData) 
				{
					WaitForSingleObject( g_hWaitToDumpObjectData,120*(1000));//Wait On..120 Secs is enough				
				}
				
				if(m_dwFunctionFilter==0)	//this means Function profiling is not started yet.As and when it starts, Memory Profiling in unwanted else till then assume memory profiling is on.
					{
							try
							{
								m_mapObjectIDToObjectTag[objectID]=++ui64objectIDDispenser;
							}catch(...){}
							try
								{
									if(m_bIsTwoDotO)
									{
										m_mapObjectIDToClassID[objectID]= classID;
									}
								} catch(...){}
					}
				
				if(WANT_OBJECT_ALLOCATION_DATA &&  IS_PROFILEDPROCESSFOROBJECTS)
					{
						/*if(m_mapObjectAllocation.find(objectID)!= m_mapObjectAllocation.end())
							return S_OK;*/

						ThreadID tid;
						StackInfo* pStackInfo=NULL;
						HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
						if(SUCCEEDED(hr)&& tid )
							{		
								pStackInfo=GetThreadInfo(tid)->GetStackInfo();
								if(!pStackInfo)
									return S_OK;								
								try
								{
									if(pStackInfo->_sFunctionStack.size()>0)
										{
											FunctionInfo* pFuncInfo=NULL;
											pFuncInfo =pStackInfo->_sFunctionStack.top().pFunctionInfo;
											if(pFuncInfo)
											{
												ULONG stackSize=pStackInfo->_sFunctionStack.size();	
												OBJECTALLOCATIONDATA allocData;
												allocData.allocFuncID=pFuncInfo->fid;
												allocData.allocThreadID =tid;
												for(ULONG x=0;x<stackSize;x++)
												{
													try
													{
														allocData.allocationStack.push(pStackInfo->_sFunctionStack.c.at(x).pFunctionInfo->fid);
													}
													catch(...)
													{
														//TRACE(ltoa(HRESULT_FROM_WIN32(GetLastError()),buffer,10));
													}															
												}
												m_mapObjectAllocation[objectID]=allocData;

											}
										}								
								}
								catch(...)
								{						
									AfxDebugBreak();
								}
								
							}

					}
					return S_OK;											
		}

		HRESULT __stdcall CSlogger::ExceptionUnwindFunctionLeave ()
		{
			 // Update the call stack as we leave
			if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
				{	
						ThreadID tid;
						HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
						if(SUCCEEDED(hr)&& tid )
						{
							try
							{
								GetThreadInfo(tid)->GetStackInfo()->PopFunction(rdtsc() );					
							}catch(...){}
						}
				}

			else if(IS_PROFILEDPROCESSFOROBJECTS )
				{
					if(WANT_OBJECT_ALLOCATION_DATA)
						{
							ThreadID tid;
							HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
							if(SUCCEEDED(hr)&& tid )
							{
								try
								{
									GetThreadInfo(tid)->GetStackInfo()->PopFunction(0 );					
								}catch(...){}
							}
						}
				}
		return S_OK;
		}


		HRESULT __stdcall  CSlogger::ExceptionSearchFunctionEnter( FunctionID functionID )
		{
			
			return S_OK;
		}


	HRESULT __stdcall  CSlogger::ExceptionThrown ( ObjectID thrownObjectID )
		{

		if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
			{
			if(WANT_FUNCTION_EXCEPTIONS)
				{
					if(!thrownObjectID)
					{
						return S_OK;
					}
					
						ThreadID tid;
						StackInfo* pStackInfo=NULL;
						HRESULT hr=	m_pCorProfilerInfo2->GetCurrentThreadID( &tid );
						if(SUCCEEDED(hr)&& tid )
							{		
								pStackInfo=GetThreadInfo(tid)->GetStackInfo();										
								if(!pStackInfo)
									return S_OK;							
								pStackInfo->SuspendFunction(rdtsc()); 
								try
								{
								
									USES_CONVERSION;									
									ClassID classID;
									hr=m_pCorProfilerInfo2->GetClassFromObject(thrownObjectID,&classID); 	
									if(m_bIsTwoDotO)
										{
										if(classID==NULL || FAILED(hr) )
											{	   
												hr=S_OK;
												classID=m_mapObjectIDToClassID[thrownObjectID]; 
											}
										}
									if(SUCCEEDED(hr) && classID)
									{	
										WCHAR wszClassName[MAX_LENGTH];
										wszClassName[0]=NULL; 
										hr=GetNameFromClassID(classID,wszClassName);
										if( wszClassName == NULL || (wcscmp(wszClassName,L"") == 0) )
											{
												lstrcpyW(wszClassName,L"Unknown Exception");
												hr=S_OK;
											}
											if(SUCCEEDED(hr) && wszClassName)
											{
												if(pStackInfo->_sFunctionStack.size()>0)
													{
														FunctionInfo* pFuncInfo=NULL;
														pFuncInfo =pStackInfo->_sFunctionStack.top().pFunctionInfo;
														if(pFuncInfo)
														{	
															PEXCEPTIONINFO pexceptionInfo=NULL;
															pexceptionInfo=  new EXCEPTIONINFO();
															pexceptionInfo->ExceptionClass=W2A(wszClassName);															

															if(WANT_FUNCTION_EXCEPTIONS_STACKTRACE)
															{

																ULONG stackSize=pStackInfo->_sFunctionStack.size();
			//													char buffer [16];
																for(ULONG x=0;x<stackSize;x++)
																{
																	try
																	{
																		pexceptionInfo->ExceptionStack.push(pStackInfo->_sFunctionStack.c.at(x).pFunctionInfo->fid);
																	}
																	catch(...)
																	{}
																}			
															}
																pFuncInfo->_mExceptionMap.push(pexceptionInfo);
														}
													}
											}
												
									}
								}							
								catch(...){}
								pStackInfo->ResumeFunction(rdtsc()); 

							}
				}
			}
			return S_OK;
		}

CalleeFunctionInfo * FunctionInfo::GetCalleeFunctionInfo( FunctionID fid )
	{		
		try
		{
			if ( _mCalleeInfo.find( fid ) == _mCalleeInfo.end() )
			{
				CalleeFunctionInfo* pCalleeFunctionInfo = NULL;
				pCalleeFunctionInfo=new CalleeFunctionInfo();
				_mCalleeInfo.insert( make_pair( fid, pCalleeFunctionInfo ) );
				return pCalleeFunctionInfo;
			}
				return _mCalleeInfo[ fid ];
		}
		catch(...)
		{	}
		return NULL;

	}

void FunctionInfo::Trace(CString* strFiles,ULONG32 spCount,ULONG32 *spLines,ULONG32 *spOffsets,ULONG32 *spStartCol,ULONG32 *spEndCol,char* szThreadID)
  { 
//#error (When u use rtdsc() in place of GetTickCount() ,pass int64 values directly to UI)

	 char buffer[256];
	 USES_CONVERSION;

	 if(llCycleCount==0)
		{
			llCycleCount=childCycleCount;
		}

	 if(WANT_NUMBER_OF_FUNCTION_CALLS)
		 {		
			sprintf(buffer, INT64_FORMAT, nCalls);
			TEXT_OUT(buffer);		
		 } 
	 else
		 {
			TEXT_OUT("0");
		 }
		 	TEXT_OUT(",");

		if(WANT_FUNCTION_TOTAL_CPU_TIME)
		 {
			 sprintf(buffer, INT64_FORMAT, llCycleCount);
			TEXT_OUT(buffer);	
		 }	 
		else
			{
				TEXT_OUT("0");
			}

			 TEXT_OUTLN("");	 
	 
	 //////start innner table Exceptions

	 char *strID=NULL;
	 strID=ltoa(fid,buffer,10);     

	 if(WANT_FUNCTION_EXCEPTIONS && !_mExceptionMap.empty())
		 {	
			 TEXT_OUTLN("<FunctionID,ThreadID,ExceptionName,ExceptionTrace>");
				while(!_mExceptionMap.empty() )
                    {									

					TEXT_OUT("'");
					TEXT_OUT(strID);
					TEXT_OUT("','");
					TEXT_OUT(szThreadID);
					TEXT_OUT("',");

						CString ExceptionString="'";					

										try
											{					

													
													try
													{
														CString strExceptionClass=((char*)(_mExceptionMap.top()->ExceptionClass.data()));							
														ExceptionString.Append(strExceptionClass);   
													}
													catch(...){}
													ExceptionString.Append("','");													
													/////dump Exception Trace//////////

													
													if(WANT_FUNCTION_EXCEPTIONS_STACKTRACE)
													{						
															UINT maxSize=_mExceptionMap.top()->ExceptionStack.size();
															while(!_mExceptionMap.top()->ExceptionStack.empty())
															{
																CString strExceptionTrace=g_Slogger->GetMinimumFunctionString(_mExceptionMap.top()->ExceptionStack.top());
				
																ExceptionString.Append(strExceptionTrace); 															

																if(_mExceptionMap.top()->ExceptionStack.size()>1)
																{									
																	ExceptionString.Append("|");																															
																}
																_mExceptionMap.top()->ExceptionStack.pop() ;
															}														
													}

													ExceptionString.Append("'");													
													TEXT_OUTLN(ExceptionString.GetBuffer());  
													ExceptionString.Empty();
													///////////////////CleanUp///////////						
													try
													{
													_mExceptionMap.top()->ExceptionClass.empty();
														while(!_mExceptionMap.top()->ExceptionStack.empty()) 
														{
																_mExceptionMap.top()->ExceptionStack.pop() ;
														}
													}
													catch(...){}
													if(_mExceptionMap.top())
														{
															delete _mExceptionMap.top();
															_mExceptionMap.top()=NULL;
														}
														
											}
											catch(...)
											{
												if(ExceptionString=="'")
												{
													ExceptionString.Append("',''"); 
													TEXT_OUTLN(ExceptionString.GetBuffer());  
													ExceptionString.Empty();
												}
											}						
						_mExceptionMap.pop(); 

					}
		 }
			 
 	 ///begin inner Callee Function Table
		
		 if(WANT_FUNCTION_CODE_VIEW && m_mapIPToDebug.size() > 0 )
		 {
			TEXT_OUTLN("<FunctionID,ThreadID,FileName,TimeConsumed,HitCount,FileOffset,StartColumn,EndColumn,CollectiveTime>");

						for(map <ULONG32,DEBUGINFO>::iterator it=m_mapIPToDebug.begin();it!=m_mapIPToDebug.end();it++)
						{
							TEXT_OUT("'");
							TEXT_OUT(ltoa(this->fid,buffer,10))  ;
							TEXT_OUT("','");
							TEXT_OUT(szThreadID);
							TEXT_OUT("','");

						/*if(g_Slogger->m_bIsTwoDotO)
							{
								
							}
						else*/
						{

							COR_DEBUG_IL_TO_NATIVE_MAP cor_map[2000];//2000 is more than enough
							ZeroMemory(cor_map,sizeof(COR_DEBUG_IL_TO_NATIVE_MAP)*2000);

							ULONG32 ulMapSize=0;
							HRESULT hr=g_Slogger->m_pCorProfilerInfo2->GetILToNativeMapping(this->fid,2000,&ulMapSize,cor_map);
							if(ulMapSize && SUCCEEDED(hr))
							{
								ULONG32 ilIP=0;												
								for(ULONG32 counter=0;counter<ulMapSize;counter++)
								{
									if (it->first >= cor_map[counter].nativeStartOffset && it->first <= cor_map[counter].nativeEndOffset)
									{
										ilIP=cor_map[counter].ilOffset ;
										break;
									} 									
								}

								if(spCount > 0 && spOffsets[0] <= ilIP)
									{
											unsigned int i;
											for (i = 0; i < spCount; i++)
											{
												if (spOffsets[i] >= ilIP )
												{
												break;
												}
											}
							
											if (((i == spCount) || (spOffsets[i] != ilIP)) && (i > 0))
												{
													i--;
												}
											
											/////
											try
												{
													if(strFiles && strFiles[i])
														{
															TEXT_OUT(strFiles[i].GetBuffer());              
														}
												}catch(...){}
											TEXT_OUT("',");
											/////////

											sprintf(buffer, INT64_FORMAT, it->second.consumedTime );
											TEXT_OUT(buffer);
											TEXT_OUT(",");

											////////
											sprintf(buffer, INT64_FORMAT, it->second.hitCount );
											TEXT_OUT(buffer);
											TEXT_OUT(",");

											TEXT_OUT(ltoa(spLines[i],buffer,10));
											TEXT_OUT(",");
											TEXT_OUT(ltoa(spStartCol[i],buffer,10));
											TEXT_OUT(",");
											TEXT_OUT(ltoa(spEndCol[i],buffer,10));											
											TEXT_OUT(",");
											TEXT_OUT(ltoa(llCycleCount,buffer,10));						
										
									}
								else
									{
										
											try
												{
													if(strFiles && strFiles[0])
														{
															TEXT_OUT(strFiles[0].GetBuffer());               
														}
													             
												}catch(...){}              
											TEXT_OUT("',");
											/////////

											sprintf(buffer, INT64_FORMAT, it->second.consumedTime );
											TEXT_OUT(buffer);
											TEXT_OUT(",");

											////////
											sprintf(buffer, INT64_FORMAT, it->second.hitCount );
											TEXT_OUT(buffer);
											TEXT_OUT(",");
											TEXT_OUT("0,0,0,");
											TEXT_OUT(ltoa(llCycleCount,buffer,10));	
									}
							}
							else
							{
										try
												{
													if(strFiles && strFiles[0])
														{
															TEXT_OUT(strFiles[0].GetBuffer());               
														}            
												}catch(...){}             
										TEXT_OUT("',");
										/////////

										sprintf(buffer, INT64_FORMAT, it->second.consumedTime );
										TEXT_OUT(buffer);
										TEXT_OUT(",");

										////////
										sprintf(buffer, INT64_FORMAT, it->second.hitCount );
										TEXT_OUT(buffer);
										TEXT_OUT(",");
										TEXT_OUT("0,0,0,");
										TEXT_OUT(ltoa(llCycleCount,buffer,10));	
							}
						}

								TEXT_OUTLN("");

							}
		 }		

		if(WANT_NO_FUNCTION_CALLEE_INFORMATION ||!WANT_FUNCTION_CALLEE_ID || _mCalleeInfo.size()==0  ) 
			{
				return;
			}

	TEXT_OUTLN("<CalleeFunctionID,CFCalls,CFCollectiveTime,CallerFunctionID,CFSignature,CFThreadID>");

	for ( map< FunctionID, CalleeFunctionInfo* >::iterator it = _mCalleeInfo.begin(); it != _mCalleeInfo.end(); it++ )
		{
					
		TEXT_OUT("'");
		try
		{					
					
					TEXT_OUT(ltoa(it->first,buffer,10));
					TEXT_OUT("',");//Callee FID


					if(WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS)
						{								
							sprintf(buffer, INT64_FORMAT, it->second->nCalls);
							TEXT_OUT(buffer);						
						}
					else
						{
							TEXT_OUT("0");
						}

						TEXT_OUT(",");

					////////////////////
					
					if(WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME)
						{
							sprintf(buffer, INT64_FORMAT, it->second->llCycleCount);
							TEXT_OUT(buffer);							
						}
					else
						{
							TEXT_OUT("0");
						}

						TEXT_OUT(",'");//Callee FID

					
					
					TEXT_OUT(ltoa(fid,buffer,10));
					TEXT_OUT("','" );								
					try
						{							
							CString strCalleeSignature=g_Slogger->GetMinimumFunctionString(it->first);							
							TEXT_OUT(strCalleeSignature.GetBuffer());						
						}
						catch(...){}
					TEXT_OUT("','" );

					if(WANT_FUNCTION_THREAD_ID)
					{
						TEXT_OUT(szThreadID);
					}

					TEXT_OUTLN("'");

					
			}
		catch(...){}		
		}	

		//end inner callee function table

  }

  CString CSlogger::GetMinimumFunctionString(const FunctionID functionID)//this function is called only for auxilliary function name resolution (like CalleeID or ParentID to functionName )
  {
			if(m_mapFuncIDToFuncName.find(functionID)!=m_mapFuncIDToFuncName.end())
				return m_mapFuncIDToFuncName[functionID];

			USES_CONVERSION; 
			HRESULT hr =S_OK;
			CString strFunctionSignature="UNKNOWN METHOD";
			ClassID classID=NULL;
			mdTypeDef classToken = mdTypeDefNil;
			try
			{		
					
					CComPtr<IMetaDataImport>  pMDImport;
					mdToken token=NULL;
					WCHAR funName[MAX_FUNCTION_LENGTH]=L"UNKNOWN METHOD"; 						

					try
					{
						ModuleID moduleID=0;
						hr=m_pCorProfilerInfo2->GetFunctionInfo(functionID,&classID,&moduleID,&token);
						if(FAILED(hr) || moduleID==0)
						{
							return strFunctionSignature;
						}

						hr=m_pCorProfilerInfo2->GetModuleMetaData(moduleID,ofRead,IID_IMetaDataImport,(IUnknown**)&pMDImport);
						
					}
					catch(...){}							

					if ( SUCCEEDED( hr )&&  token)
					{
						
						hr = pMDImport->GetMethodProps( token,&classToken,funName,MAX_FUNCTION_LENGTH,0,
																	0,
																	NULL,
																	NULL,
																	NULL, 
																	NULL );
						
					}
					
					if (m_bIsTwoDotO)
						{
						    hr = m_pCorProfilerInfo2->GetFunctionInfo2(functionID,0,&classID,NULL,NULL,0,
                                                            NULL,
                                                            NULL);
						  if (!SUCCEEDED(hr))
						     classID = 0;
						 }
					else
					{
						hr = m_pCorProfilerInfo2->GetFunctionInfo(functionID,
                                                          &classID,
                                                         NULL,NULL);
					}	

					WCHAR wstrClassName[MAX_LENGTH];
					wstrClassName[0]=NULL;
					if(SUCCEEDED(hr) && classID !=NULL)
					{
						if(GetNameFromClassID(classID,wstrClassName)==S_OK)
						{
							if(wstrClassName[0]=='\0')
								{	
									lstrcpyW(wstrClassName,L"UNKNOWN CLASS");
								}
						}
						else
						{
							lstrcpyW(wstrClassName,L"UNKNOWN CLASS");
						}
					}
					else if (classToken != mdTypeDefNil)
					{
						ULONG classGenericArgCount = 0;
						if(S_OK==this->GetClassName(pMDImport, classToken, wstrClassName, NULL, &classGenericArgCount))						
							{
								if(wstrClassName[0]=='\0')
									{
										swprintf(wstrClassName,L"UNKNOWN CLASS");
									}
							}
						else
						{
							lstrcpyW(wstrClassName,L"UNKNOWN CLASS");
						}  				
					}
					
					strFunctionSignature=wstrClassName;
					strFunctionSignature+="::";
					strFunctionSignature+=funName;

					m_mapFuncIDToFuncName[functionID]=strFunctionSignature;
				
			}
			catch(...){}
		return strFunctionSignature;	

  }

  void CSlogger::AppendTypeArgName(ULONG argIndex, ClassID *actualClassTypeArgs, ClassID *actualMethodTypeArgs, BOOL methodFormalArg, __out_ecount(cchBuffer) char *buffer, size_t cchBuffer)
{
    char argName[MAX_LENGTH];

    argName[0] = '\0';

    ClassID classId = 0;
    if (methodFormalArg && actualMethodTypeArgs != NULL)
        classId = actualMethodTypeArgs[argIndex];
    if (!methodFormalArg && actualClassTypeArgs != NULL)
        classId = actualClassTypeArgs[argIndex];

    if (classId != 0)
    {
        WCHAR className[MAX_LENGTH];

        HRESULT hr = g_Slogger->GetNameFromClassID(classId, className);
        if (SUCCEEDED(hr))
            _snprintf_s( argName, ARRAY_LEN(argName), ARRAY_LEN(argName)-1, "%S", className);
    }

    if (argName[0] == '\0')
    {
        char argStart = methodFormalArg ? 'H' : 'A';
        if (argIndex <= 5)
        {
            // the first 7 parameters are printed as A,B,C,D,E,F 
            // or as H,I,J,K,L
            sprintf_s( argName, ARRAY_LEN(argName), "%c", argIndex + argStart);
        }
        else
        {           
            sprintf_s( argName, ARRAY_LEN(argName), "%c%u", argStart, argIndex);
        }
    }

    StrAppend( buffer, argName, cchBuffer);
}


  void CSlogger::SetTimeResolutionFilter(void)
  {
	  CRegKey  trKey;
	  if(FAILED(HRESULT_FROM_WIN32(trKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  throw NULL;
	  }
	  DWORD trFlag=TR_CPU_CYCLES;
	  if(FAILED(HRESULT_FROM_WIN32(trKey.QueryDWORDValue("TRFlag",trFlag))))
	  {
		  trKey.Close(); 
		  throw NULL;
	  }
	  g_Slogger->m_dwTimeResolution=trFlag;//even if trFlag is not one of TR flags rdtsc() will profile for CPU cycles for default
	  trKey.Close ();

  }

  void CSlogger::ResolveFilters(void)
  {
	  CRegKey  keyFunctionClassFilter,keyObjectClassFilter,keyFunctionModuleFilter;//,keyObjectModuleFilter;
	  CRegKey  keyFunctionClassPassThrough,keyFunctionModulePassThrough,keyObjectPassThrough;

	  ////////////////////////////	  Clean Up First
	  g_Slogger->m_functionModuleFilter.c.clear();
	  g_Slogger->m_functionClassFilter.c.clear();
	  g_Slogger->m_objectClassFilter.c.clear();
	   /////////////////////////////////

	  if(FAILED(HRESULT_FROM_WIN32(keyFunctionClassPassThrough.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  return;
	  }
	  DWORD dwIsFunctionClassFilterPassthrough=1  ;
		if(FAILED(HRESULT_FROM_WIN32(keyFunctionClassPassThrough.QueryDWORDValue("FunctionClassPassthrough",dwIsFunctionClassFilterPassthrough))))
		{
			g_Slogger->m_bFunctionClassPassthrough=true; 
			keyFunctionClassPassThrough.Close();
			return;
		}

		g_Slogger->m_bFunctionClassPassthrough=(dwIsFunctionClassFilterPassthrough!=0)?true:false;
		keyFunctionClassPassThrough.Close();

		//////////////////////////////

		 /////////////////////////////////

	  if(FAILED(HRESULT_FROM_WIN32(keyFunctionModulePassThrough.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  return;
	  }
	  DWORD dwIsFunctionModuleFilterPassthrough=1  ;
		if(FAILED(HRESULT_FROM_WIN32(keyFunctionModulePassThrough.QueryDWORDValue("FunctionModulePassthrough",dwIsFunctionModuleFilterPassthrough))))
		{
			g_Slogger->m_bFunctionModulePassthrough=true; 
			keyFunctionModulePassThrough.Close();
			return;
		}

		g_Slogger->m_bFunctionModulePassthrough=(dwIsFunctionModuleFilterPassthrough!=0)?true:false;
		keyFunctionModulePassThrough.Close();

		//////////////////////////////

		if(FAILED(HRESULT_FROM_WIN32(keyObjectPassThrough.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  return;
	  }
	  DWORD dwIsObjectFilterPassthrough=1  ;
		if(FAILED(HRESULT_FROM_WIN32(keyObjectPassThrough.QueryDWORDValue("ObjectPassthrough",dwIsObjectFilterPassthrough))))
		{
			g_Slogger->m_bObjectPassthrough=true; 
			keyObjectPassThrough.Close();
			return;
		}

		g_Slogger->m_bObjectPassthrough=(dwIsObjectFilterPassthrough!=0)?true:false;
		keyObjectPassThrough.Close();

		/////////////////////////////////Filter Strings//////////////
		//keyFunctionClassFilter,keyObjectClassFilter,keyFunctionModuleFilter,keyObjectModuleFilter;	
		

		TCHAR strFunctionClassBuffer[MAX_LENGTH];//enough for filter string
		strFunctionClassBuffer[0]=NULL;

		TCHAR strFunctionModuleBuffer[MAX_LENGTH];//enough for filter string
		strFunctionModuleBuffer[0]=NULL;

		//TCHAR strObjectModuleBuffer[MAX_LENGTH];//enough for filter string
		//strObjectModuleBuffer[0]=NULL;

		TCHAR strObjectClassBuffer[MAX_LENGTH];//enough for filter string
		strObjectClassBuffer[0]=NULL;
		ULONG strSize=MAX_LENGTH;
        
		//////////////Function Class filter resolution
		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyFunctionClassFilter.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}
			  
				if(SUCCEEDED(HRESULT_FROM_WIN32(keyFunctionClassFilter.QueryStringValue("FunctionClassFilter",strFunctionClassBuffer,&strSize))))
				{	
					CString strFunctionClassFilterString;
					try
					{
						strFunctionClassFilterString=(strFunctionClassBuffer==NULL)?"":T2A(strFunctionClassBuffer) ;
					}
					catch(...){strFunctionClassFilterString="";}

					if(strFunctionClassFilterString.GetLength()>0)
					{
						strFunctionClassFilterString.Trim() ;
						while(strFunctionClassFilterString.Find("|")>=0)
							{
								int index=strFunctionClassFilterString.Find("|");
								if(index==0)
								{
									strFunctionClassFilterString=strFunctionClassFilterString.Right(strFunctionClassFilterString.GetLength()-(index+1)).Trim() ;  
									continue;
								}
								g_Slogger->m_functionClassFilter.push(strFunctionClassFilterString.Left(index).Trim());
								strFunctionClassFilterString=strFunctionClassFilterString.Right(strFunctionClassFilterString.GetLength()-(index+1)).Trim();  
							}
							if(strFunctionClassFilterString.GetLength()>0)
								{
									g_Slogger->m_functionClassFilter.push(strFunctionClassFilterString.Trim());

								}

					}	
					strFunctionClassFilterString.Empty(); 
				}

					keyFunctionClassFilter.Close(); 
		}
		catch(...){}

		/////////////////////////////////////function Module Filter Resolution/////////////

		strFunctionModuleBuffer[0]=NULL;
		strSize=MAX_LENGTH;
		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyFunctionModuleFilter.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}
			  
				if(SUCCEEDED(HRESULT_FROM_WIN32(keyFunctionModuleFilter.QueryStringValue("FunctionModuleFilter",strFunctionModuleBuffer,&strSize))))
				{	
					CString strFunctionModuleFilterString;
					try
					{
						strFunctionModuleFilterString=(strFunctionModuleBuffer==NULL)?"":T2A(strFunctionModuleBuffer) ;
					}
					catch(...){strFunctionModuleFilterString="";}

					if(strFunctionModuleFilterString.GetLength()>0)
					{
						strFunctionModuleFilterString.Trim() ;
						while(strFunctionModuleFilterString.Find("|")>=0)
							{
								int index=strFunctionModuleFilterString.Find("|");
								if(index==0)
								{									
									strFunctionModuleFilterString=strFunctionModuleFilterString.Right(strFunctionModuleFilterString.GetLength()-(index+1)).Trim();  //extract and trim
									continue;
								}
								CString strTemp=strFunctionModuleFilterString.Left(index).Trim();
								strTemp=strTemp.Right(strTemp.GetLength()- (strTemp.ReverseFind('\\')+1)).Trim() ;  //remove path UNCs
								g_Slogger->m_functionModuleFilter.push(strTemp);//push
								strFunctionModuleFilterString=strFunctionModuleFilterString.Right(strFunctionModuleFilterString.GetLength()-(index+1)).Trim();  //shorten
							}
							if(strFunctionModuleFilterString.GetLength()>0)
								{
									strFunctionModuleFilterString=strFunctionModuleFilterString.Right(strFunctionModuleFilterString.GetLength()- (strFunctionModuleFilterString.ReverseFind('\\')+1)).Trim() ;  
									g_Slogger->m_functionModuleFilter.push(strFunctionModuleFilterString.Trim());

								}

					}
					strFunctionModuleFilterString.Empty(); 
				}
					keyFunctionModuleFilter.Close(); 
		}
		catch(...){}

		//////////////////Object Module Filter/////////
		/*
		strObjectModuleBuffer[0]=NULL;
		strSize=MAX_LENGTH;
		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyObjectModuleFilter.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}
			  
				if(SUCCEEDED(HRESULT_FROM_WIN32(keyObjectModuleFilter.QueryStringValue("ObjectModuleFilter",strObjectModuleBuffer,&strSize))))
				{	
					CString strObjectModuleFilterString;
					try
					{
						strObjectModuleFilterString=(strObjectModuleBuffer==NULL)?"":T2A(strObjectModuleBuffer) ;
					}
					catch(...){strObjectModuleFilterString="";}

					if(strObjectModuleFilterString.GetLength()>0)
					{
						strObjectModuleFilterString.Trim() ;
						while(strObjectModuleFilterString.Find("|")>=0)
							{
								int index=strObjectModuleFilterString.Find("|");
								if(index==0)
								{									
									strObjectModuleFilterString=strObjectModuleFilterString.Right(strObjectModuleFilterString.GetLength()-(index+1)).Trim();  //extract and trim
									continue;
								}
								CString strTemp=strObjectModuleFilterString.Left(index).Trim();
								strTemp=strTemp.Right(strTemp.GetLength()- (strTemp.ReverseFind('\\')+1)).Trim() ;  //remove path UNCs
								g_Slogger->m_objectModuleFilter.push(strTemp);//push
								strObjectModuleFilterString=strObjectModuleFilterString.Right(strObjectModuleFilterString.GetLength()-(index+1)).Trim();  //shorten
							}
							if(strObjectModuleFilterString.GetLength()>0)
								{
									strObjectModuleFilterString=strObjectModuleFilterString.Right(strObjectModuleFilterString.GetLength()- (strObjectModuleFilterString.ReverseFind('\\')+1)).Trim() ;  
									g_Slogger->m_objectModuleFilter.push(strObjectModuleFilterString.Trim());

								}

					}
					strObjectModuleFilterString.Empty(); 
				}
					keyObjectModuleFilter.Close(); 
		}
		catch(...){}
		*/

	

	////////////////////////////////////////////

		//////////////////Object Class Filter////
		strObjectClassBuffer[0]=NULL;
		strSize=MAX_LENGTH;

		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyObjectClassFilter.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}

				HRESULT hr=HRESULT_FROM_WIN32(keyObjectClassFilter.QueryStringValue("ObjectClassFilter",strObjectClassBuffer,&strSize));
			  
				if(SUCCEEDED(hr))
				{	
					CString strObjectClassFilterString;
					try
					{
						strObjectClassFilterString=(strObjectClassBuffer==NULL)?"":T2A(strObjectClassBuffer) ;
					}
					catch(...){strObjectClassFilterString="";}

					if(strObjectClassFilterString.GetLength()>0)
					{
						strObjectClassFilterString.Trim() ;
						while(strObjectClassFilterString.Find("|")>=0)
							{
								int index=strObjectClassFilterString.Find("|");
								if(index==0)
								{
									strObjectClassFilterString=strObjectClassFilterString.Right(strObjectClassFilterString.GetLength()-(index+1)).Trim();  
									continue;
								}
								g_Slogger->m_objectClassFilter.push(strObjectClassFilterString.Left(index).Trim());
								strObjectClassFilterString=strObjectClassFilterString.Right(strObjectClassFilterString.GetLength()-(index+1)).Trim();  
							}
							if(strObjectClassFilterString.GetLength()>0)
								{
									g_Slogger->m_objectClassFilter.push(strObjectClassFilterString.Trim());

								}

					}	
					strObjectClassFilterString.Empty(); 
				}

					keyObjectClassFilter.Close(); 
		}
		catch(...){}


  }


  void CSlogger::GetLastSessionName(void)
  {
	  //////////////////Session Name////
		USES_CONVERSION ;
		TCHAR strLastSessionName[MAX_LENGTH];
		strLastSessionName[0]=NULL;
		DWORD strSize=MAX_LENGTH;
		CRegKey  keyLastSessionName;

		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyLastSessionName.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}

				HRESULT hr=HRESULT_FROM_WIN32(keyLastSessionName.QueryStringValue("LastSessionName",strLastSessionName,&strSize));
			  
				if(SUCCEEDED(hr))
				{					
					try
					{						
						m_csSessionName =(strLastSessionName==NULL)?m_csSessionName:T2A(strLastSessionName) ;
						keyLastSessionName.Close(); 
						return;
					}
					catch(...){}				
				}
					
		}
		catch(...){}
		try{keyLastSessionName.Close(); }catch(...){}		

  }

  void CSlogger::GetLastSessionPath(void)
  {
	   //////////////////Session Name////
		USES_CONVERSION ;
		TCHAR strLastSessionPath[MAX_LENGTH];
		strLastSessionPath[0]=NULL;
		DWORD strSize=MAX_LENGTH;
		CRegKey  keyLastSessionPath;

		try
		{

				if(FAILED(HRESULT_FROM_WIN32(keyLastSessionPath.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
				{
					return;
				}

				HRESULT hr=HRESULT_FROM_WIN32(keyLastSessionPath.QueryStringValue("LastSessionPath",strLastSessionPath,&strSize));
			  
				if(SUCCEEDED(hr))
				{					
					try
					{						
						m_csSessionPath =(strLastSessionPath==NULL)?m_csSessionPath:T2A(strLastSessionPath) ;
						keyLastSessionPath.Close(); 
						return;
					}
					catch(...){}				
				}
					
		}
		catch(...){}
		try{keyLastSessionPath.Close(); }catch(...){}		


  }

  void CSlogger::CanRunGCBeforeOC(void)
  {
	  CRegKey  gcKey;
	  if(FAILED(HRESULT_FROM_WIN32(gcKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  throw NULL;
	  }
	  DWORD gcFlag=1;
	  if(FAILED(HRESULT_FROM_WIN32(gcKey.QueryDWORDValue("RunGCBeforeObjectCollection",gcFlag))))
	  {
		  gcKey.Close(); 
		  throw NULL;
	  }
	  m_bRunGC_BOC = (gcFlag!=0)?true:false;
	  gcKey.Close ();
  }

  void CSlogger::IsProcessSuspended(void)
  {

	  USES_CONVERSION ;

	  CRegKey  suspendKey;
	  if(FAILED(HRESULT_FROM_WIN32(suspendKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
	  {
		  throw NULL;
	  }
	  DWORD suspendFlag=0;
	  if(FAILED(HRESULT_FROM_WIN32(suspendKey.QueryDWORDValue("SuspendOnStart",suspendFlag))))
	  {
		  suspendKey.Close(); 
		  throw NULL;
	  }
	  m_bSuspendOnStart = (suspendFlag!=0)?true:false;	 
	  ////
	   ///Bypass Immune app//////////
	  try
	  {
			TCHAR immuneApp[MAX_LENGTH];
			DWORD iAppSize=sizeof(immuneApp)/sizeof(TCHAR);
			immuneApp[0]=NULL;
			if(FAILED(HRESULT_FROM_WIN32(suspendKey.QueryStringValue("ImmuneApp",immuneApp,&iAppSize))))
			{				
				return;	  
			}
			if(immuneApp==NULL)
			{ 
				return;	   
			}

			CString strIApp=immuneApp;
			if(strIApp.GetLength()==0)
			{				
				return;	  
			}	  
				  
			strIApp=strIApp.Trim();				
			ZeroMemory(immuneApp,sizeof(immuneApp));
			immuneApp[0]=NULL;		
			GetModuleFileName(NULL,immuneApp,MAX_LENGTH * sizeof(TCHAR) ); 			
			if(immuneApp == NULL)
			{							
				return;	  
			}
			
			if(_stricmp(strIApp.GetBuffer(),T2A(immuneApp))==0)//immune this app from suspending on start
			{				
				m_bSuspendOnStart=false;
			}

	  }catch(...){}	 	 

  }

  bool CSlogger::IsInprocEnabled()
  {
	  try
	  {
		CRegKey  ipKey;
		if(FAILED(HRESULT_FROM_WIN32(ipKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
		{
			throw NULL;
		}
		DWORD InprocSwitch=1;
		if(FAILED(HRESULT_FROM_WIN32(ipKey.QueryDWORDValue("InprocSwitch",InprocSwitch))))
		{
			ipKey.Close(); 
			throw NULL;
		}
		ipKey.Close ();
		return ((InprocSwitch==0)?false:true);
	  }
	  catch(...)
	  {}
	  return true;

  }

   bool CSlogger::CanAutoAttach()
  {
	  try
	  {
		CRegKey  autoAttachKey;
		if(FAILED(HRESULT_FROM_WIN32(autoAttachKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
		{
			throw NULL;
		}
		DWORD dwAutoAttach=1;
		if(FAILED(HRESULT_FROM_WIN32(autoAttachKey.QueryDWORDValue("AutoAttach",dwAutoAttach))))
		{
			autoAttachKey.Close(); 
			throw NULL;
		}
		autoAttachKey.Close ();
		return ((dwAutoAttach==0)?false:true);
	  }
	  catch(...)
	  {}
	  return true;

  }
  bool CSlogger::IsObjectAllocationMonitored()
  {
	  try
	  {
		CRegKey  oaKey;
		if(FAILED(HRESULT_FROM_WIN32(oaKey.Open(HKEY_LOCAL_MACHINE,"Software\\Profiler#\\",KEY_READ))))
		{
			throw NULL;
		}
		DWORD oaSwitch=0;
		if(FAILED(HRESULT_FROM_WIN32(oaKey.QueryDWORDValue("IsObjectAllocationMonitored",oaSwitch))))
		{
			oaKey.Close(); 
			throw NULL;
		}
		oaKey.Close ();
		return ((oaSwitch==0)?false:true);
	  }
	  catch(...)
	  {}
	  return false;

  }


  HRESULT __stdcall CSlogger::JITCompilationStarted ( FunctionID functionID, BOOL fIsSafeToBlock )  
	   {
		   return S_OK;
		}