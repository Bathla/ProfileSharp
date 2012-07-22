#include "StdAfx.h"
#include "Slogger.h"
#include "threadinfo.h"
#include "StackInfo.h"
ThreadInfo::ThreadInfo()
{
		_bRunning = false;
		_llSuspendTime = 0;
		_pStackInfo=NULL;
		_pStackInfo = new StackInfo( this );
}
ThreadInfo::~ThreadInfo()
	{
	if(_pStackInfo)
		delete _pStackInfo;
	}

 void ThreadInfo::Start()
  {
	   _llStartTime = rdtsc();
		_bRunning = true;
  }

  void ThreadInfo::End()
 {
	_llEndTime = rdtsc();
	_bRunning = false;
  }
  
  bool ThreadInfo::IsRunning()
	 {
	  return _bRunning;
	}

  StackInfo* ThreadInfo::GetStackInfo()
  {
		return _pStackInfo;
  }

  FunctionInfo* ThreadInfo::GetFunctionInfo( FunctionID fid )
  {
	   if ( _mFunctionInfo.find( fid ) == _mFunctionInfo.end() )
		{
			FunctionInfo* pFunctionInfo = NULL;
			pFunctionInfo=new FunctionInfo( fid );
			_mFunctionInfo.insert( make_pair( fid, pFunctionInfo ) );
			return pFunctionInfo;
		}
		return _mFunctionInfo[ fid ];
  }

  void ThreadInfo::Trace(char * szThreadID )
  {
		for ( map< FunctionID, FunctionInfo* >::iterator it = _mFunctionInfo.begin(); it != _mFunctionInfo.end(); it++ )
				{
					//
								DWORD methodAttr=0;
								std::string strRet,strClassName,strFuncName,strParameters,strModule;
								ULONG32 *spLines=NULL;
								ULONG32 *spStartCol=NULL;
								ULONG32 *spEndCol=NULL;
								ULONG32 spCount=0;
								ULONG32 *spOffsets=NULL;
								CString * strFiles=NULL;

								//module function filtering has been removed.so it is always returned
								try
								{									
									g_Slogger->GetFunctionSignature(it->first,methodAttr,strRet,strClassName,strFuncName,strParameters,strModule,&strFiles,spCount,&spLines,&spOffsets,&spStartCol,&spEndCol);					
								}
							catch(...){}

								CString cstrModule=strModule.data();
								CString cstrClassName=strClassName.data();

								///////////////Function Module Filtering///////////	
							
								cstrModule=cstrModule.Right(cstrModule.GetLength()- (cstrModule.ReverseFind('\\')+1));  
								if( !cstrModule ||  cstrModule.GetLength()==0)//Very Rare
									goto A;//Goto Next filter
								//Continuing to next filter even if the module string is unavailable
								//Extra data is always better than no data						

							
							if(g_Slogger->m_functionModuleFilter.size()>0 && strModule.length()>0 )  
							{ 
								if(g_Slogger->m_bFunctionModulePassthrough )
								{
									for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
									{	
										if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
											continue;

										if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0) 
											goto A;//Goto Next filter
									}

									//failed .. the module is not desired
									goto C;//get next function ID
										
								}
								else
								{
									///block these 
									for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
									{	
										if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
											goto A;//Goto Next filter

										if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0)
											goto C;
											
									}
											//Passed ..Allow to move on
											goto A;

								}

							}

							//////////////////////
A:							
							
							//try{cstrModule.Empty();}catch(...){}	//Do Not Empty,it is required later
							///Function Class filter///////////							
														
							if( !cstrClassName ||  cstrClassName.GetLength()==0)//Very Rare
									goto B;//move on

							////Function Class Filtering///////////							


							if(g_Slogger->m_functionClassFilter.size()>0 && strClassName.length()>0 )  
							{    								
								if(g_Slogger->m_bFunctionClassPassthrough )
								{
									for(ULONG x=0;x<g_Slogger->m_functionClassFilter.size();x++)
									{	
										if(g_Slogger->m_functionClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
											continue;

										if(strncmp(g_Slogger->m_functionClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_functionClassFilter.c.at(x).GetLength() )==0) 
											goto B;//Move On
									}

									//failed .. the Class is not desired
									goto C;//get next function ID									
										
								}
								else
								{
									///block these 
									for(ULONG x=0;x<g_Slogger->m_functionClassFilter.size();x++)
									{	
										if(g_Slogger->m_functionClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
											goto B;//Move On

										if(strncmp(g_Slogger->m_functionClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_functionClassFilter.c.at(x).GetLength() )==0)
											goto C;

											
									}
											//Passed ..Allow to move on
											goto B;

								}

							}
							
							//////////////////////
B:					//TEXT_OUTLN("<FunctionID,FSignature,ThreadID,ModuleName,Calls,CollectiveTime,ParentID,ParentName,PercentTimes,PercentCalls>"); 
					TEXT_OUTLN("<FunctionID,FSignature,ThreadID,ModuleName,Calls,CollectiveTime>"); 					

					try{cstrClassName.Empty();}catch(...){}
							//////////////////////////////
					try
						{
								//FunctionID
								char buffer [64];						
								TEXT_OUT("'");
								TEXT_OUT(ltoa(it->first,buffer,10));//always wanted
								TEXT_OUT("','");


								//FSignature
								CString strFunctionSignature="";								
								try
								{
										if(WANT_FUNCTION_SIGNATURE && methodAttr )
											{
											try
												{
													if(IsMdPrivate(methodAttr))
														strFunctionSignature.Append("private ");
													if(IsMdPublic(methodAttr))
														strFunctionSignature.Append("public ");
													if(IsMdStatic(methodAttr))
														strFunctionSignature .Append("static ");
													if(IsMdPinvokeImpl(methodAttr))
														strFunctionSignature.Append("pinvoke ");
													if(IsMdFinal(methodAttr))
														strFunctionSignature.Append("final ");										
													if(IsMdAbstract(methodAttr))
														strFunctionSignature.Append("abstract ");
													if(IsMdVirtual(methodAttr))
														strFunctionSignature.Append("virtual ");
													if(strRet.data())
														strFunctionSignature.Append(strRet.data());
													strFunctionSignature.Append(" ");
												}catch(...){}
											}
											
										try
											{
												if(strClassName.data() )
												strFunctionSignature.Append(strClassName.data());		
											}
										catch(...)
											{
												strFunctionSignature.Append("UNKNOWN_CLASS");
											}
										strFunctionSignature.Append("::");
										///////////////////		

										try
											{
												if(strFuncName.data())
												strFunctionSignature.Append(strFuncName.data());
											}
										catch(...)
											{
												strFunctionSignature.Append("UNKNOWN_METHOD");
											}
											

										
										/////////
										if(WANT_FUNCTION_SIGNATURE)
											{
												strFunctionSignature.Append("(");
												try
													{
														strFunctionSignature.Append(strParameters.data());
													}
												catch(...)
													{
														strFunctionSignature.Append("UNKNOWN_PARAM");
													}

												strFunctionSignature.Append(")");												
											} 
									}
								catch(...)
								{}

														

								TEXT_OUT(strFunctionSignature.GetBuffer());
								TEXT_OUT("','");
								strFuncName.clear();
								strClassName.clear();
								strRet.clear ();
								strFunctionSignature.Empty();
								/////////

								
								
								if(WANT_FUNCTION_THREAD_ID)
									{									
										TEXT_OUT(szThreadID);											
									}		

									TEXT_OUT("','");
								
								if(WANT_FUNCTION_MODULE)
									{										
											
									TEXT_OUT(cstrModule.GetLength()==0?"N:A":cstrModule.GetBuffer() );										
									}
									TEXT_OUT("',");	
									try{cstrModule.Empty();}catch(...){}


									/////////////////////Gather File Offset///////
								
								
								it->second->Trace(strFiles,spCount,spLines,spOffsets,spStartCol,spEndCol,szThreadID);
								
					}
					catch(...){}

C:
										
						try
						{						
						if(strFiles)
							delete [] strFiles;							
						}catch(...){}
						strFiles=NULL;

						try
						{	 if(spLines)
							delete [] spLines;
						}catch(...){}
						spLines=NULL;

						try
						{
						if(spOffsets)
							delete [] spOffsets;
						}catch(...){}
						spOffsets=NULL;
						try
						{
						if(spStartCol)
							delete [] spStartCol;
						}catch(...){}
						spStartCol=NULL;

						try
						{
						if(spEndCol)
							delete [] spEndCol;
						}catch(...){}
					spEndCol=NULL;

					
					continue;
					
					
				}	
  }

