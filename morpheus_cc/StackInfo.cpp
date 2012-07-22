/****************************************************************************************
 * Copyright (c) Bathla [bathla.tech@gmail.com] All Rights Reserved.
 * Licensed under Apache License 2
 *
 * File: StackInfo.cpp
 *
 * Description:
 *	
 *
 *
 ***************************************************************************************/

#include "StdAfx.h"
#include "Slogger.h"
#include "Stackinfo.h"
#include "ThreadInfo.h"
#include "CalleeFunctionInfo.h"

StackInfo::StackInfo( ThreadInfo* pThreadInfo )
	{
		_pThreadInfo = pThreadInfo;
	}

bool StackInfo::IsDebugFunction(FunctionID functionID)
	{
		ModuleID moduleID;		

		HRESULT hr=	g_Slogger->m_pCorProfilerInfo2->GetFunctionInfo( functionID,NULL,&moduleID,NULL);		
		if(FAILED(hr))
			return false;
		if(	g_Slogger->m_debugModuleSet.find(moduleID)!=g_Slogger->m_debugModuleSet.end()) 
		{
			return true;
		}
		else
		{
			return false;
		}						
	}

	bool StackInfo::RetryGetInfo(FunctionInfo* _parentFunctionInfo,UINT64 llElapsed)
	{
			if(!g_Slogger->m_bIsTwoDotO)
				{
						CComPtr<ICorDebugThread> _pDebugThread;
						GetDebugThread(&_pDebugThread);
						if(!_pDebugThread)
							return false;
						CComPtr<ICorDebugFrame> frame;
						HRESULT hr=_pDebugThread->GetActiveFrame(&frame);										
						_pDebugThread.Release();									
						if(!frame)
						{
							return false;
						}				

						CComPtr<ICorDebugNativeFrame> nActiveFrame;								

						frame->QueryInterface(__uuidof(ICorDebugNativeFrame), (void**)&nActiveFrame);
						frame.Release(); 									
						if(!nActiveFrame)
						{
							return false;
						}
						nActiveFrame->GetCaller(&frame);
						nActiveFrame.Release();
						if(!frame)
						{					
							return false;
						}

						CComPtr<ICorDebugNativeFrame> nFrame;
						frame->QueryInterface(__uuidof(ICorDebugNativeFrame), (void**)&nFrame);
						frame.Release();
						if(!nFrame)
						{
							return false;
						}
						ULONG32 ip = 0;	
						nFrame->GetIP(&ip); 
						nFrame.Release();			
						if(ip)
						{	
							_parentFunctionInfo->m_mapIPToDebug[ip].consumedTime+=llElapsed;
							_parentFunctionInfo->m_mapIPToDebug[ip].hitCount+=1;
							return true;
						}


					return false;
				}
			else
				{
				try
					{
						PERFORMANCEDATA perfData;
						perfData.cycleCounts=llElapsed;
						perfData.pFuncInfo=_parentFunctionInfo;

						g_Slogger->m_pCorProfilerInfo2->DoStackSnapshot( NULL,CSlogger::StackTracer,COR_PRF_SNAPSHOT_DEFAULT,(LPVOID)&perfData,NULL,0);					   
					}catch(...){}
					return true;
				}

	}

	 void StackInfo::GetDebugThread(ICorDebugThread** pCorDebugThread)
	 {
		
			*pCorDebugThread=NULL;
			 if(!g_Slogger->m_bIsTwoDotO && g_Slogger->m_hInprocMutex)
			 {			
				CComPtr<IUnknown> punkDebugThread;
				HRESULT hr;			
				ThreadID threadId=0;
				g_Slogger->m_pCorProfilerInfo2->GetCurrentThreadID(&threadId);
					try
					{	
						if(threadId)
						hr=g_Slogger->m_pCorProfilerInfo2->EndInprocDebugging(g_Slogger->m_mapDebugMap[threadId]);					
					}catch(...){}

					hr=g_Slogger->m_pCorProfilerInfo2->BeginInprocDebugging(TRUE,&(g_Slogger->m_mapDebugMap[threadId]));  
					hr=g_Slogger->m_pCorProfilerInfo2->GetInprocInspectionIThisThread(&punkDebugThread);
						if(punkDebugThread)
						{
							punkDebugThread->QueryInterface(__uuidof(ICorDebugThread),(void**)pCorDebugThread);
							punkDebugThread.Release();
							return;
						}
			 }
			

	 }

  
void StackInfo::PushFunction( FunctionInfo* pFunctionInfo )
{
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
		{
			try
			{
					if ( _sFunctionStack.size() > 0 )
					{ 
							if(WANT_NO_FUNCTION_CALLEE_INFORMATION || ! WANT_FUNCTION_CALLEE_ID)
							{
								goto A;
							}

							FunctionInfo *_pFunctionInfo=_sFunctionStack.top().pFunctionInfo;
							CalleeFunctionInfo* pCalleeFunctionInfo=_pFunctionInfo->GetCalleeFunctionInfo( pFunctionInfo->fid);
							if(pCalleeFunctionInfo)
								{
									pCalleeFunctionInfo->nRecursiveCount++;
									pCalleeFunctionInfo->nCalls++;
								}				
					}	
			            
						A:
							pFunctionInfo->nCalls++;
							pFunctionInfo->nRecursiveCount++;
							_sFunctionStack.push( StackEntryInfo( pFunctionInfo ) );
							
			}
			catch(...)
			{				
			}
				
		}

	else if(IS_PROFILEDPROCESSFOROBJECTS )
		{
			if(WANT_OBJECT_ALLOCATION_DATA)
				{
					try
					{
						/*if ( _sFunctionStack.size() > 0 )*/
						{ 
							pFunctionInfo->nCalls++;
							pFunctionInfo->nRecursiveCount++;
							if(pFunctionInfo->nRecursiveCount < 2)
							{
								_sFunctionStack.push( StackEntryInfo( pFunctionInfo ) );
							}
						}
					}
					catch(...){}
				}
		}
}


UINT64 StackInfo::PopFunction(UINT64 llCycleCount )
{
	UINT64 llElapsed=0;
	if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
			{				
				try
				{
					if ( _sFunctionStack.size() > 0 )
					{
						llElapsed = llCycleCount - _sFunctionStack.top().llCycleStart;
						FunctionInfo* pFunctionInfo = _sFunctionStack.top().pFunctionInfo;
						FunctionID fidCallee = pFunctionInfo->fid;

						pFunctionInfo->nRecursiveCount--;
						if ( pFunctionInfo->nRecursiveCount == 0 )
							pFunctionInfo->llCycleCount += llElapsed;
						else
							pFunctionInfo->llRecursiveCycleCount += llElapsed; 
						
						_sFunctionStack.pop();

							FunctionInfo* _parentFunctionInfo=NULL;	//V. Imp

							if(_sFunctionStack.size() > 0)
									{
										_parentFunctionInfo=_sFunctionStack.top().pFunctionInfo;									
										_parentFunctionInfo->childCycleCount+=llElapsed; 										
									}

							if(WANT_FUNCTION_CODE_VIEW)
								{
									if(_parentFunctionInfo)
										{
											if(!IsDebugFunction(_parentFunctionInfo->fid))
												goto A;
											if(!RetryGetInfo(_parentFunctionInfo,llElapsed))
											{
												RetryGetInfo(_parentFunctionInfo,llElapsed);
											}
										}
								}

A:
							if(WANT_NO_FUNCTION_CALLEE_INFORMATION  || ! WANT_FUNCTION_CALLEE_ID )
								{
									goto B;
								}
								if(_parentFunctionInfo)
									{
										CalleeFunctionInfo* pCalleeFunctionInfo = _parentFunctionInfo->GetCalleeFunctionInfo( fidCallee );
											pCalleeFunctionInfo->nRecursiveCount--;
										if ( pCalleeFunctionInfo->nRecursiveCount == 0 )
											pCalleeFunctionInfo->llCycleCount += llElapsed;
										else
											pCalleeFunctionInfo->llRecursiveCycleCount += llElapsed;
									}
						}

					B:	

						return llElapsed;
					}
					catch(...)
					{					
					}
				}		

	else if(IS_PROFILEDPROCESSFOROBJECTS )
			{
				if(WANT_OBJECT_ALLOCATION_DATA)
					{
						try
						{
							if(_sFunctionStack.size()>0)
							{
								FunctionInfo* pFunctionInfo = _sFunctionStack.top().pFunctionInfo;						
								pFunctionInfo->nRecursiveCount--;
								if(pFunctionInfo->nRecursiveCount==0)
								{
									_sFunctionStack.pop();							
								}
							}
						}
						catch(...)
						{					
						}
					}
			}

	return llElapsed;
}

  
  void StackInfo::SuspendFunction( UINT64 llCycleCount )
  {
  if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
	  {
			_llSuspendStart = llCycleCount;
			if ( _sFunctionStack.size() == 0 ) 
			{
				return;
			}
	  }
  }

  void StackInfo::ResumeFunction( UINT64 llCycleCount )
  {
  if(CAN_PROFILE_FOR_FUNCTIONS && IS_PROFILEDPROCESSFORFUNCTIONS)  
	  {
			UINT64 llElapsed = llCycleCount - _llSuspendStart;	
			if ( _sFunctionStack.size() == 0 ) 
			{
				return;
			}
			_sFunctionStack.top().pFunctionInfo->llSuspendCycleCount += llElapsed;
			_pThreadInfo->_llSuspendTime += llElapsed;
	  }
  }

