// Slogger.h : Declaration of the CSlogger

#pragma once
#include "resource.h"       // main symbols

#include "Morpheus.h"
#include "_ISloggerEvents_CP.h"

//#include <windows.h>
#include <wincrypt.h>
#include <mmsystem.h>
#include <psapi.h>
#include <comdef.h>
#include <corerror.h>
#include <cor.h>
#include <corprof.h>
#include <corsym.h>
#include <cordebug.h>
#include <hash_map>
#include <map>
#include <stack>
#include <queue>
#include <set>
//#include "FunctionModelStore.h"
//#include "MarkerByteManager.h"

//#include <iostream>
using namespace std;

//#include <comutil.h>

#define MAX_LENGTH 1024
#define HRESULTTOWIN32(hres)                                \
            ((HRESULT_FACILITY(hres) == FACILITY_WIN32)     \
                ? HRESULT_CODE(hres)                        \
                : (hres))

#define ARRAY_LEN(a) (sizeof(a) / sizeof((a)[0]))

#define E_ADS_BAD_PATHNAME _HRESULT_TYPEDEF_(0x80005000L)

bool g_bUpdate=false;	//OBJECT PROFILING GATE
bool g_bCapture=false;	//FUNCTION PROFILING GATE

#define CAN_PROFILE_FOR_OBJECTS (g_bUpdate)
#define CAN_PROFILE_FOR_FUNCTIONS (g_bCapture)
									  
#define IS_PROFILEDPROCESSFOROBJECTS (g_pid==g_Slogger->m_ProfileePID)//same
#define IS_PROFILEDPROCESSFORFUNCTIONS (g_pid==g_Slogger->m_ProfileePID)//same	 

enum TIME_RESOLUTION
{
	TR_CPU_CYCLES=1,
	TR_HIGH_RESOLUTION=2,
	TR_LOW_RESOLUTION=4
};


#define WANT_CPU_CYCLES (((g_Slogger->m_dwTimeResolution) & (TR_CPU_CYCLES))== TR_CPU_CYCLES)
#define WANT_HIGH_RESOLUTION (((g_Slogger->m_dwTimeResolution) & (TR_HIGH_RESOLUTION))== TR_HIGH_RESOLUTION)
#define WANT_LOW_RESOLUTION (((g_Slogger->m_dwTimeResolution) & (TR_LOW_RESOLUTION))== TR_LOW_RESOLUTION)

void StrAppend(__out_ecount(cchBuffer) char *buffer, const char *str, size_t cchBuffer)
{
    size_t bufLen = strlen(buffer) + 1;
    if (bufLen <= cchBuffer)
        strncat_s(buffer, cchBuffer, str, cchBuffer-bufLen);
}

const char* GetErrorText(HRESULT hr)
{
	switch(hr)
	{
	case CORDBG_E_UNRECOVERABLE_ERROR: 
		return "Unrecoverable API error.";
	case CORDBG_E_PROCESS_TERMINATED: 
		return "Process was terminated.";
	case CORDBG_E_PROCESS_NOT_SYNCHRONIZED: 
		return"Process not synchronized.";
	case CORDBG_E_CLASS_NOT_LOADED: 
		return "A class is not loaded.";
	case CORDBG_E_IL_VAR_NOT_AVAILABLE: 
		return "An IL variable is not available at the current native IP.";
	case CORDBG_E_BAD_REFERENCE_VALUE: 
		return "A reference value was found to be bad during dereferencing.";
	case CORDBG_E_FIELD_NOT_AVAILABLE: 
		return "A field in a class is not available, because the runtime optimized it away.";
	case CORDBG_E_NON_NATIVE_FRAME: 
		return "'Native frame only' operation on non-native frame";
	case CORDBG_E_NONCONTINUABLE_EXCEPTION:
		return "Continue on non-continuable exception";
	case CORDBG_E_CODE_NOT_AVAILABLE: 
		return "The code is currently unavailable";
	case CORDBG_E_FUNCTION_NOT_IL: 
		return "Attempt to get a ICorDebugFunction for";
		// a function that is not IL.
	case CORDBG_S_BAD_START_SEQUENCE_POINT: 
		return "Attempt to SetIP not at a sequence point";
	case CORDBG_S_BAD_END_SEQUENCE_POINT: 
		return "Attempt to SetIP when not going to a sequence point.  If both this and CORDBG_E_BAD_START_SEQUENCE_POINT are true, only CORDBG_E_BAD_START_SEQUENCE_POINT will be reported.";
	case CORDBG_S_INSUFFICIENT_INFO_FOR_SET_IP: 
		return "SetIP is possible, but the debugger doesn't have enough info to fix variable locations, GC refs, or anything else. Use at your own risk.";
	case CORDBG_E_CANT_SET_IP_INTO_FINALLY:
		return "SetIP isn't possible, because SetIP would move EIP from outside of an exception handling finally clause to a point inside of one.";
	case CORDBG_E_CANT_SET_IP_OUT_OF_FINALLY: 
		return "SetIP isn�t possible because it would move EIP from within an exception handling finally clause to a point outside of one.";
	case CORDBG_E_CANT_SET_IP_INTO_CATCH: 
		return "SetIP isn't possible, because SetIP would move EIP from outside of an exception handling catch clause to a point inside of one.";
	case CORDBG_E_SET_IP_NOT_ALLOWED_ON_NONLEAF_FRAME: 
		return "Setip cannot be done on any frame except the leaf frame.";
	case CORDBG_E_SET_IP_IMPOSSIBLE: 
		return "SetIP isn't allowed. For example, there is insufficient memory to perform SetIP.";
	case CORDBG_E_FUNC_EVAL_BAD_START_POINT:
		return "Func eval can't work if we're, for example, not stopped at a GC safe point.";
	case CORDBG_E_INVALID_OBJECT: 
		return "This object value is no longer valid.";
	case CORDBG_E_FUNC_EVAL_NOT_COMPLETE: 
		return "If you call CordbEval::GetResult before the func eval has finished, you'll get this result.";
	case CORDBG_S_FUNC_EVAL_HAS_NO_RESULT:
		return "Some Func evals will lack a return value, such as those whose return type is void.";
	case CORDBG_S_VALUE_POINTS_TO_VOID: 
		return " The Debugging API doesn't support dereferencing pointers of type void.";
	case CORDBG_E_INPROC_NOT_IMPL: 
		return "The inproc version of the debugging API doesn't implement this function.";
	case CORDBG_S_FUNC_EVAL_ABORTED: 
		return "The func eval completed, but was aborted.";
	case CORDBG_E_STATIC_VAR_NOT_AVAILABLE:
		return "A static variable isn't available because it hasn't been initialized yet.";
	case CORDBG_E_OBJECT_IS_NOT_COPYABLE_VALUE_CLASS: 
		return "Can't copy a VC with object refs in it.";
	case CORDBG_E_CANT_SETIP_INTO_OR_OUT_OF_FILTER: 
		return "SetIP can't leave or enter a filter";
	case CORDBG_E_CANT_CHANGE_JIT_SETTING_FOR_ZAP_MODULE: 
		return "You can't change JIT settings for ZAP modules.";
	case CORDBG_E_REMOTE_CONNECTION_CONN_RESET: 
		return "The remote device closed the connection.";
	case CORDBG_E_REMOTE_CONNECTION_KEEP_ALIVE: 
		return "The connection was closed due to a keep alive failure.";
	case CORDBG_E_REMOTE_CONNECTION_FATAL_ERROR: 
		return "Generic error that the device connection has been broken with no chance for recovery.";
	default:
		{
			USES_CONVERSION;
			_com_error e(hr);
			const char* ret = T2CA(e.ErrorMessage());
			return ret;
		}	
	};

}



enum FUNCTION_FILTER
	{
		FF_FUNCTION_NAME_ONLY=1,
		FF_FUNCTION_SIGNATURE=FF_FUNCTION_NAME_ONLY | 2 ,
		FF_FUNCTION_CODE_VIEW=4,
		FF_FUNCTION_NUMBER_OF_CALLS_ONLY=8 | FF_FUNCTION_NAME_ONLY,
		FF_FUNCTION_TOTAL_CPU_TIME=16 | FF_FUNCTION_NAME_ONLY,
		FF_FUNCTION_MODULE=FF_FUNCTION_NAME_ONLY   | 32,
		FF_FUNCTION_EXCEPTIONS=FF_FUNCTION_NAME_ONLY  | 64,
		FF_FUNCTION_STATISTICS=FF_FUNCTION_NUMBER_OF_CALLS_ONLY |
			FF_FUNCTION_TOTAL_CPU_TIME ,
		FF_FUNCTION_CALLEE_ID=128 | FF_FUNCTION_NAME_ONLY, 
		FF_FUNCTION_EXCEPTIONS_STACKTRACE=256 |FF_FUNCTION_EXCEPTIONS,
		FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY= FF_FUNCTION_CALLEE_ID  | FF_FUNCTION_NUMBER_OF_CALLS_ONLY | 512,		
		FF_FUNCTION_CALLEE_TOTAL_CPU_TIME=FF_FUNCTION_CALLEE_ID  | FF_FUNCTION_TOTAL_CPU_TIME | 1024,
		//FF_FUNCTION_FAST_PROFILING=2048,
		FF_FUNCTION_CALLEE_STATISTICS=FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY |
				FF_FUNCTION_CALLEE_TOTAL_CPU_TIME ,
		FF_FUNCTION_THREAD_ID=FF_FUNCTION_NAME_ONLY|4096,
		FF_FUNCTION_MANAGED_ONLY=FF_FUNCTION_NAME_ONLY|8192 ,
		FF_NO_FUNCTION_CALLEE_INFORMATION=FF_FUNCTION_NAME_ONLY|16384
	};

#define WANT_FUNCTION_NAME ( ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_NAME_ONLY))  == FF_FUNCTION_NAME_ONLY)
#define WANT_FUNCTION_SIGNATURE ( ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_SIGNATURE )) ==FF_FUNCTION_SIGNATURE)
#define WANT_NUMBER_OF_FUNCTION_CALLS ( ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_NUMBER_OF_CALLS_ONLY )) ==FF_FUNCTION_NUMBER_OF_CALLS_ONLY)
#define WANT_FUNCTION_TOTAL_CPU_TIME ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_TOTAL_CPU_TIME )) ==FF_FUNCTION_TOTAL_CPU_TIME)
#define WANT_FUNCTION_MODULE ( ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_MODULE )) ==FF_FUNCTION_MODULE)
#define WANT_FUNCTION_EXCEPTIONS  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_EXCEPTIONS )) ==FF_FUNCTION_EXCEPTIONS)
#define WANT_FUNCTION_EXCEPTIONS_STACKTRACE  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_EXCEPTIONS_STACKTRACE )) ==FF_FUNCTION_EXCEPTIONS_STACKTRACE)
#define WANT_FUNCTION_CALLEE_ID  (  (  (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CALLEE_ID )) ==FF_FUNCTION_CALLEE_ID)
#define WANT_FUNCTION_CODE_VIEW  (  (  (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CODE_VIEW )) ==FF_FUNCTION_CODE_VIEW)

//#define WANT_FUNCTION_PARENT_ID WANT_FUNCTION_CALLEE_ID
//NOTE :- WANT_FUNCTION_CALLEE_ID  ALSO INCLUDES FUNCTION-PARENT(CALLER) FUNCTIONID 

//#define WANT_FUNCTION_CALLEE_SIGNATURE  (  ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CALLEE_SIGNATURE ) ) ==FF_FUNCTION_CALLEE_SIGNATURE)
#define WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY )) ==FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY)
#define WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME  (  ((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CALLEE_TOTAL_CPU_TIME )) ==FF_FUNCTION_CALLEE_TOTAL_CPU_TIME)
//#define WANT_FUNCTION_CALLEE_AVERAGE_CPU_TIME  (((g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_CALLEE_AVERAGE_CPU_TIME )) ==FF_FUNCTION_CALLEE_AVERAGE_CPU_TIME)
#define WANT_FUNCTION_THREAD_ID  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_THREAD_ID)) ==FF_FUNCTION_THREAD_ID)
#define WANT_FUNCTION_MANAGED_ONLY  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_FUNCTION_MANAGED_ONLY)) ==FF_FUNCTION_MANAGED_ONLY)
#define WANT_NO_FUNCTION_CALLEE_INFORMATION  ( ( (g_Slogger->m_dwFunctionFilter) & (FF_NO_FUNCTION_CALLEE_INFORMATION)) ==FF_NO_FUNCTION_CALLEE_INFORMATION)
HANDLE hGlobalDebugCheck=NULL;
HANDLE hGlobalDebugCheckInit=NULL;
//IS_PROFILEDPROCESSFORFUNCTIONS && CAN_PROFILE_FOR_FUNCTIONS

enum OBJECT_FILTER
	{
		OF_OBJECT_NAME_ONLY=1,
		OF_OBJECT_COUNT=OF_OBJECT_NAME_ONLY|2,
		OF_OBJECT_SIZE=OF_OBJECT_NAME_ONLY| 4,
		OF_REFERENCED_OBJECTS=OF_OBJECT_NAME_ONLY|8,	
		OF_OBJECT_ALLOCATION=OF_OBJECT_NAME_ONLY|16,
		OF_OBJECT_SRC_ANALYSIS_ONLY=OF_OBJECT_NAME_ONLY|32,
		OF_OBJECT_ALL_DATA=OF_OBJECT_SIZE |OF_OBJECT_COUNT
		|OF_REFERENCED_OBJECTS|OF_OBJECT_ALLOCATION|OF_OBJECT_SRC_ANALYSIS_ONLY
	};

#define WANT_OBJECT_NAME_ONLY ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_NAME_ONLY))==OF_OBJECT_NAME_ONLY )
#define WANT_OBJECT_COUNT ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_COUNT))==OF_OBJECT_COUNT )
#define WANT_OBJECT_SIZE ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_SIZE))==OF_OBJECT_SIZE )
#define WANT_REFERENCED_OBJECTS ( ((g_Slogger->m_dwObjectFilter) &(OF_REFERENCED_OBJECTS))==OF_REFERENCED_OBJECTS )
#define WANT_OBJECT_ALLOCATION_DATA ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_ALLOCATION))==OF_OBJECT_ALLOCATION )
#define WANT_OBJECT_ALL_DATA ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_ALL_DATA))==OF_OBJECT_ALL_DATA )
#define WANT_SRC_ANALYSIS_ONLY ( ((g_Slogger->m_dwObjectFilter) &(OF_OBJECT_SRC_ANALYSIS_ONLY))==OF_OBJECT_SRC_ANALYSIS_ONLY )

LRESULT WINAPI startCheck(LPVOID param);

#include "ThreadInfo.h"
class StackInfo ;

void LogFile(const char*);
void LogFileSingleLine(const char*);
UINT BeginTimer();
void EndTimer(UINT wTimerRes);
UINT64 rdtsc();

long g_pid=-1;
HANDLE g_hWaitToDumpObjectData=NULL;
HANDLE g_hCLRHandle=NULL;

#define TEXT_OUT( message ) LogFileSingleLine(message ) 
//#define TEXT_OUTLN( message ) printf( "%s\n", message )
#define TEXT_OUTLN( message ) LogFile(message) 

#define _DIMOF( Array ) (sizeof(Array) / sizeof(Array[0]))
#define g_wszParamSeparator	L"+"

#define INT64_FORMAT "%I64u"
#define MAX_FUNCTION_LENGTH 1024

extern CRITICAL_SECTION g_csParamList;
extern CRITICAL_SECTION g_csReturnTypeList;
extern CRITICAL_SECTION g_csMethod;

CRITICAL_SECTION g_csFunction;

FILE* g_LogFile=NULL;

typedef class OBJECTALLOCATIONDATA
{
public:
	FunctionID allocFuncID;
	stack<FunctionID> allocationStack;
	ThreadID allocThreadID;
	ULONG lAge;

	OBJECTALLOCATIONDATA()
	{
		allocFuncID=0;
		allocThreadID=0;
		lAge=0;
	}
} *POBJECTALLOCATIONDATA;

typedef class OBJECTDATA
{
public:
	ULONG objCount;
	ULONG objSize;
	CString objName;	
	
OBJECTDATA()
{
	objCount=1;
	objSize=0;
	objName="";	
}
} *POBJECTDATA;



class CLock
{
public:	
	CLock( CRITICAL_SECTION& cs );
	~CLock();
private:
	CRITICAL_SECTION& m_cs;			
}; 
  
class CLockObjectDump
{
public:	
	CLockObjectDump(  )
	{
				
	if(g_hWaitToDumpObjectData==NULL)
		{
			g_hWaitToDumpObjectData=CreateEvent(NULL,TRUE,FALSE,"SYNC_DUMP");	
		}

	}
	~CLockObjectDump( )
	{
		if(g_hWaitToDumpObjectData)
		{
			try
			{
				SetEvent(g_hWaitToDumpObjectData);
			}
			catch(...){}			
			try
			{	if(g_hWaitToDumpObjectData)
				{
					CloseHandle(g_hWaitToDumpObjectData);					
				}
			}catch(...){}
		}
		g_hWaitToDumpObjectData=NULL; 	
		
	}
	
}; 
  __forceinline CLock::CLock(CRITICAL_SECTION &cs):m_cs(cs)
	  {			
	  try
		  {
			InitializeCriticalSection(&cs);
			EnterCriticalSection(&cs); 
		  }catch(...){}
	  }

  __forceinline CLock::~CLock ()
	  {
	  try
		  {
			LeaveCriticalSection(&m_cs);
			DeleteCriticalSection(&m_cs);
			}catch(...){}
	  }

class FunctionInfo;
class CSlogger;
class CalleeFunctionInfo;


typedef class EXCEPTIONINFO
{
public:	
	string ExceptionClass;
	stack <FunctionID> ExceptionStack;		
	
	 EXCEPTIONINFO()
	 {
		 ExceptionClass="";			
	 }

} *PEXCEPTIONINFO;

typedef class DEBUGINFO
{
public :
	UINT64 hitCount;	
	UINT64 consumedTime;

	DEBUGINFO()
	{
		hitCount=0;
		consumedTime=0;
	}

} *PDEBUGINFO;

typedef class PERFORMANCEDATA
{
public:
	UINT64 cycleCounts;
	FunctionInfo* pFuncInfo;
	
PERFORMANCEDATA()
{
	cycleCounts=0;
	pFuncInfo=NULL;	
}
} *PPERFORMANCEDATA;

class FunctionInfo
{
public: 
	FunctionInfo( FunctionID fid )
	{		
		llCycleCount = 0;
		llRecursiveCycleCount = 0;
		llSuspendCycleCount = 0;
		nCalls = 0;
		nRecursiveCount = 0;		
		this->fid = fid;
		baseIP=0;
		childCycleCount=0;
		/*this->parentFid =fid;*/
	}


	CalleeFunctionInfo * GetCalleeFunctionInfo( FunctionID fid );

	~FunctionInfo()
		{		
		try
			{
			while(!_mExceptionMap.empty() )
				{
					_mExceptionMap.top()->ExceptionClass.empty();
					while(!_mExceptionMap.top()->ExceptionStack.empty()) 
					{
						_mExceptionMap.top()->ExceptionStack.pop() ;
					}
                    try
					{	
						if(_mExceptionMap.top())
						{
							delete _mExceptionMap.top();
							_mExceptionMap.top()=NULL;
						}
					}
					catch(...){}
					_mExceptionMap.pop(); 
				}
			}
		catch(...){}
		///////////
		m_mapIPToDebug.clear(); 
		 
		};

	void Trace(CString* strFiles,ULONG32 spCount,ULONG32 *spLines,ULONG32 *spOffsets,ULONG32 *spStartCol,ULONG32 *spEndCol,char * szThreadID);

  UINT64 nCalls;
  UINT64 nRecursiveCount;
  UINT64 llCycleCount;
  UINT64 llRecursiveCycleCount;
  UINT64 llSuspendCycleCount;
  UINT64 childCycleCount;
  FunctionID fid;
  UINT_PTR baseIP;
  map <ULONG32,DEBUGINFO> m_mapIPToDebug;
  /*FunctionID parentFid;*/
  map< FunctionID, CalleeFunctionInfo* > _mCalleeInfo;
  stack <PEXCEPTIONINFO> _mExceptionMap;


};




// CSlogger
CSlogger * g_Slogger=NULL;

class ATL_NO_VTABLE CSlogger : 
	public CComObjectRootEx<CComMultiThreadModel>,
	//public ICorDebugManagedCallback,
	public CComCoClass<CSlogger, &CLSID_Slogger>,
	public IConnectionPointContainerImpl<CSlogger>,
	public CProxy_ISloggerEvents<CSlogger>, 
	public ISlogger,
	public ICorProfilerCallback2
{
public:
	CSlogger()
	{		
		m_bIsTwoDotO=false;
		char buffer[32];
		CLock lock(g_csFunction); 
		g_LogFile=NULL;		
		ui64objectIDDispenser=0;//v. imp.

		g_hWaitToDumpObjectData=NULL;
		g_hCLRHandle=NULL;
		g_Slogger=this;		
		m_bSuspendOnStart=false;//v .imp.		
		m_hSuspensionMutexFlag=NULL;//v .imp.
		m_hInprocMutex=NULL;
		m_hOAMutex=NULL;
		m_ProfileePID =-1;		
		m_dwFunctionFilter=0;
		m_dwObjectFilter=0;//CHANGE (not -1 but 0)
		m_dwTimeResolution=TR_CPU_CYCLES;//begin with CPU_CYCLES by default
		m_uiTR=BeginTimer();
		m_bFunctionClassPassthrough=true;
		m_bFunctionModulePassthrough=true;
		m_bObjectPassthrough=true;		
		m_bRunGC_BOC=true;
		m_bObjectAllocation=false;
		m_csSessionName="ProfilerSession";
			m_csSessionName.Append(itoa(rand(),buffer,10));
			m_csSessionPath ="C:\\";		
	}


	~CSlogger()
	{	
		

		if(hGlobalDebugCheck)
		{
			try
			{
				CloseHandle(hGlobalDebugCheck);
			}catch(...){}
			hGlobalDebugCheck=NULL;
		}

		if(hGlobalDebugCheckInit)
		{
			try
			{
				CloseHandle(hGlobalDebugCheckInit);
			}catch(...){}
			hGlobalDebugCheckInit=NULL;
		}

		CLock lock(g_csFunction); 

		m_csSessionName.Empty();
		m_csSessionPath.Empty(); 		
		try
		{
			if(m_hSuspensionMutexFlag)
			{
				ReleaseMutex(m_hSuspensionMutexFlag);				
			}
		}
		catch(...){}
		if(m_hSuspensionMutexFlag){CloseHandle(m_hSuspensionMutexFlag);}
		m_hSuspensionMutexFlag=NULL;
		m_bSuspendOnStart=false;

		try
		{
			if(m_hInprocMutex)
			{
				ReleaseMutex(m_hInprocMutex);			
			}
		}
		catch(...){}
		if(m_hInprocMutex)
		{
			CloseHandle(m_hInprocMutex);
		}
		m_hInprocMutex=NULL;

		try
		{
			if(m_hOAMutex)
			{
				ReleaseMutex(m_hOAMutex);			
			}
		}
		catch(...){}
		if(m_hOAMutex)
		{
			CloseHandle(m_hOAMutex);
		}
		m_hOAMutex=NULL;

		///////////////

		try
		{	
			if(g_hCLRHandle)
			{
				CloseHandle(g_hCLRHandle);					
			}
		}catch(...){}
		g_hCLRHandle=NULL; 	
		EndTimer(m_uiTR);
		g_Slogger=NULL;		
	if(m_pCorProfilerInfo2)
		m_pCorProfilerInfo2.Release();

		while(!m_objectClassFilter.empty ())
		{
			m_objectClassFilter.top().Empty();
			m_objectClassFilter.pop();  
		}
		/*
		while(!m_objectModuleFilter.empty ())
		{
			m_objectModuleFilter.top().Empty();
			m_objectModuleFilter.pop();  
		}
		*/	

		while(!m_functionClassFilter.empty ())
		{
			m_functionClassFilter.top().Empty();
			m_functionClassFilter.pop();  
		}
		while(!m_functionModuleFilter.empty ())
		{
			m_functionModuleFilter.top().Empty();
			m_functionModuleFilter.pop();  
		}

		try
		{
			map<FunctionID,CString>::iterator it=m_mapFuncIDToFuncName.begin();
			while(it!=m_mapFuncIDToFuncName.end())
			{
				(it->second).Empty();  
				it++;
			}
			m_mapFuncIDToFuncName.clear(); 
		}
		catch(...){}

	}

DECLARE_REGISTRY_RESOURCEID(IDR_SLOGGER)

BEGIN_COM_MAP(CSlogger)
	COM_INTERFACE_ENTRY(ISlogger)
	COM_INTERFACE_ENTRY(IConnectionPointContainer)	
	COM_INTERFACE_ENTRY(ICorProfilerCallback)
	COM_INTERFACE_ENTRY(ICorProfilerCallback2)	
END_COM_MAP()

BEGIN_CONNECTION_POINT_MAP(CSlogger)
	CONNECTION_POINT_ENTRY(__uuidof(_ISloggerEvents))
END_CONNECTION_POINT_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}
public:	
	map<ThreadID, ThreadInfo* > _mThreadInfo;
	DWORD m_dwFunctionFilter;
	DWORD m_dwObjectFilter;
	DWORD m_dwTimeResolution;
	UINT m_uiTR;
map<UINT_PTR,POBJECTDATA>  m_mapLiveObjects;
map<UINT_PTR,POBJECTDATA> m_mapRootObjects;
map<UINT_PTR,map<UINT_PTR,POBJECTDATA> > m_mapReferencedObjects;
map<FunctionID,CString> m_mapFuncIDToFuncName;///filters
map<ObjectID,OBJECTALLOCATIONDATA> m_mapObjectAllocation;
map<ObjectID,OBJECTALLOCATIONDATA> m_mapSwapObjectAllocation;
map<ObjectID,OBJECTALLOCATIONDATA> m_mapTempObjectAllocation;
map<ObjectID,ClassID> m_mapObjectIDToClassID;
map<ObjectID,ClassID> m_mapTempObjectIDToClassID;

///////////////
ObjectID ui64objectIDDispenser;
map<ObjectID,ObjectID> m_mapObjectIDToObjectTag;
map<ObjectID,ObjectID> m_mapTempObjectIDToObjectTag;
//stack<CString> m_objectModuleFilter;
stack<CString> m_functionModuleFilter;
stack<CString> m_objectClassFilter;
stack<CString> m_functionClassFilter;

map<ThreadID,DWORD> m_mapDebugMap; 

CString m_csSessionName;
CString m_csSessionPath;

set <ULONG> m_debugModuleSet;
set <ULONG> m_FilterModuleSet;

bool m_bFunctionClassPassthrough;
bool m_bFunctionModulePassthrough;
bool m_bObjectPassthrough;
bool m_bObjectAllocation;
bool m_bRunGC_BOC;

CComPtr<ISymUnmanagedBinder> binder;								

private :
	bool IsInprocEnabled();	 
	bool CanAutoAttach();
	HANDLE m_hOAMutex;
	
	bool IsObjectAllocationMonitored();	
public :
	bool m_bIsTwoDotO;
	HANDLE m_hInprocMutex;

void Trace()
{
	
	if(!WANT_FUNCTION_NAME) 
		{
			return;			
		}	
	
	
  for ( map< ThreadID, ThreadInfo* >::iterator it = _mThreadInfo.begin(); it != _mThreadInfo.end(); it++ )
  {
	char threadBuffer[32];
	it->second->Trace(_ultoa(it->first,threadBuffer,10));	
  }
}

bool IsDebugFunction(FunctionID functionID)
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


bool CanContinue(ClassID classID,ModuleID moduleID)
	{
		if(/*m_FilterModuleSet.size()>0 && */moduleID )
			{	 
				if(m_FilterModuleSet.find(moduleID)!=m_FilterModuleSet.end())
					{
						return true;
					}
				else
					{
						return false;
					}
			}
		else
			{
				return true;
			}
	}


bool IsModuleCodeCoverable(ModuleID moduleID)
	{
		if(m_FilterModuleSet.size()>0 && moduleID )
			{	 
				if(m_FilterModuleSet.find(moduleID)!=m_FilterModuleSet.end())
					{
						return true;
					}
				else
					{
						return false;
					}
			}
		else
			{
				return false;
			}
	}


//bool CanContinue(ClassID classID,ModuleID moduleID )
//	{
//		if(moduleID!=NULL)
//			{	 
//				if(m_FilterModuleSet.find(moduleID)!=m_FilterModuleSet.end())
//					{
//						goto A;
//					}
//				else if(m_FilterAntiModuleSet.find(moduleID)!=m_FilterAntiModuleSet.end())
//					{
//						return false;
//					}
//				else
//					{
//						if(m_functionModuleFilter.size()>0)  
//							{ 
//								WCHAR moduleName[MAX_LENGTH];
//								moduleName[0]=NULL;
//								HRESULT hr=m_pCorProfilerInfo2->GetModuleInfo( moduleID,NULL,MAX_LENGTH,NULL,moduleName,NULL);
//								if(FAILED(hr) || !moduleName)
//								{
//									m_FilterModuleSet.insert(moduleID);
//									goto A;
//								}
//								CString cstrModule(moduleName);
//								cstrModule=cstrModule.Right(cstrModule.GetLength()- (cstrModule.ReverseFind('\\')+1)); 
//								if(m_bFunctionModulePassthrough )
//								{
//									for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
//									{	
//										if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
//											continue;
//
//										if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0) 											
//											{
//												m_FilterModuleSet.insert(moduleID);
//												goto A;
//											}
//									}
//
//										m_FilterAntiModuleSet.insert(moduleID);
//										return false;
//										
//								}
//								else
//								{
//									block these 
//									for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
//									{	
//										if(g_Slogger->m_functionModuleFilter.c.at(x).GetLength() > cstrModule.GetLength())
//											{
//												m_FilterModuleSet.insert(moduleID);
//												goto A;
//											}
//
//										if(_strnicmp(g_Slogger->m_functionModuleFilter.c.at(x).GetBuffer(),cstrModule.GetBuffer(),g_Slogger->m_functionModuleFilter.c.at(x).GetLength() )==0)
//											{
//												m_FilterAntiModuleSet.insert(moduleID);
//												return false;
//											}
//											
//									 }
//											Passed ..Allow to move on
//											m_FilterModuleSet.insert(moduleID);
//												goto A;
//
//								}
//
//							}
//						else
//							{
//								m_FilterModuleSet.insert(moduleID);
//									goto A;
//							}
//
//					}
//					
//			}
//		else
//			{
//				goto A;
//			}
//		///////////////////////////////////////////////////////////////////
//A:
//	
//		if(classID!=NULL)
//			{	 
//				if(m_FilterClassSet.find(classID)!=m_FilterClassSet.end())
//					{
//						return true;
//					}
//				else if(m_FilterAntiClassSet.find(classID)!=m_FilterAntiClassSet.end())
//					{
//						return false;
//					}
//				else
//					{
//						if(m_functionClassFilter.size()>0)  
//							{ 
//								WCHAR className[MAX_LENGTH];
//								className[0]=NULL;
//								HRESULT hr=GetNameFromClassID(classID,className);
//								if(FAILED(hr) || !className)
//								{
//									m_FilterClassSet.insert(classID);
//									return true;
//								}
//								CString cstrClassName(className);
//
//									if(g_Slogger->m_bFunctionClassPassthrough )
//									{
//										for(ULONG x=0;x<g_Slogger->m_functionClassFilter.size();x++)
//										{	
//											if(g_Slogger->m_functionClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
//												continue;
//
//											if(strncmp(g_Slogger->m_functionClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_functionClassFilter.c.at(x).GetLength() )==0) 
//												{
//													m_FilterClassSet.insert(classID);
//													return true;
//												}
//												
//										}
//
//										failed .. the Class is not desired
//										m_FilterAntiClassSet.insert(classID);  
//										return false;								
//											
//									}
//									else
//									{
//										block these 
//										for(ULONG x=0;x<g_Slogger->m_functionClassFilter.size();x++)
//										{	
//											if(g_Slogger->m_functionClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
//												{
//													m_FilterClassSet.insert(classID);
//													return true;
//												}
//
//											if(strncmp(g_Slogger->m_functionClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_functionClassFilter.c.at(x).GetLength() )==0)
//												{
//													m_FilterAntiClassSet.insert(classID);  
//													return false;		
//												}
//
//												
//										}
//												Passed ..Allow to move on
//											m_FilterClassSet.insert(classID);
//											return true;
//
//									}
//								}
//							else
//								{
//									m_FilterClassSet.insert(classID);
//										return true;
//								}
//					}
//					
//			}
//		else
//			{
//				return true;
//			}
//
//	}

							  
void AppendTypeArgName(ULONG argIndex, ClassID *actualClassTypeArgs, ClassID *actualMethodTypeArgs, BOOL methodFormalArg, __out_ecount(cchBuffer) char *buffer, size_t cchBuffer);

void DumpObjectAllocationData()
	{
		char buffer[32];
		CString strAlloc;
		TEXT_OUTLN("<ObjectID,ThreadID,AllocFunctionID,AllocFunctionIDStack,ObjectAge,ObjectGeneration>");		//always emitted
		if(m_mapTempObjectAllocation.size()>0 && (WANT_OBJECT_ALLOCATION_DATA))
			{
				map<UINT_PTR,OBJECTALLOCATIONDATA>::iterator ObjectIterator=m_mapTempObjectAllocation.begin();														
				//ULONG32 lNotReached=0;
				while(m_mapTempObjectAllocation.size()>0)
					{
					try
						{
							strAlloc="'";
							strAlloc+=ultoa(g_Slogger->m_mapTempObjectIDToObjectTag[ObjectIterator->first],buffer,10);
							strAlloc+="','";
							strAlloc+=ltoa(ObjectIterator->second.allocThreadID,buffer,10);
							strAlloc+="','";
							strAlloc+=ltoa(ObjectIterator->second.allocFuncID,buffer,10);
							strAlloc+="','";

							//never use unsigned integers in inverse loops
							for(long x=ObjectIterator->second.allocationStack.size()-1;x>=0;x--)
							{	
								FunctionID fidAlloc=ObjectIterator->second.allocationStack.c.at(x);							
								if(fidAlloc)
								{
									//build up the cache
									GetMinimumFunctionString(fidAlloc);
									strAlloc+=ltoa(fidAlloc,buffer,10);
									strAlloc+="|";
								}
							}
							strAlloc+="','";
							strAlloc+=ltoa(ObjectIterator->second.lAge,buffer,10);
							strAlloc+="','";
							if(m_bIsTwoDotO)
								{
									COR_PRF_GC_GENERATION_RANGE genRange;								
									HRESULT hr=m_pCorProfilerInfo2->GetObjectGeneration(ObjectIterator->first,&genRange);
									if(SUCCEEDED(hr))
										{
											strAlloc+=ltoa(genRange.generation,buffer,10);
										}
									else
										{ 
											strAlloc+="N:A";
										}
								}
							else
								{ 
									strAlloc+="N:A";
								}
							strAlloc+="'";						
							TEXT_OUTLN(strAlloc.GetBuffer());
							strAlloc.Empty();
						}catch(...){}

						/*try	{
							m_mapTempObjectIDToObjectTag.erase(ObjectIterator->first);							
						}
						catch(...)
						{
							TRACE0("Temp ObjectTag map error");
						}	*/

					try	{
							m_mapTempObjectAllocation.erase(ObjectIterator);
							ObjectIterator=m_mapTempObjectAllocation.begin();
						}catch(...){goto A;}			
					}	

			m_mapTempObjectAllocation.clear();			
			}

			////////////////////////////
//			g_Slogger->m_mapTempObjectIDToObjectTag.clear();
			//////////////////////////

A:			TEXT_OUTLN("<AllocFunctionID,AllocFunctionName>");	//always emitted
			if(m_mapFuncIDToFuncName.size () && (WANT_OBJECT_ALLOCATION_DATA))
			{					
				map<FunctionID,CString>::iterator funcIterator=m_mapFuncIDToFuncName.begin();
				while(m_mapFuncIDToFuncName.size ()>0)
				{

					strAlloc="'";
					strAlloc+=ltoa(funcIterator->first,buffer,10);
					strAlloc+="','";
					strAlloc+=funcIterator->second.GetBuffer();
					strAlloc+="'";
					TEXT_OUTLN(strAlloc.GetBuffer());
					strAlloc.Empty(); 
					try	{
							m_mapFuncIDToFuncName.erase(funcIterator);
							funcIterator=m_mapFuncIDToFuncName.begin();
						}catch(...){return;}			

				}
				m_mapFuncIDToFuncName.clear(); 
			}
		

	}

void DumpStringAndStructMapForLiveObjects(map<UINT_PTR,POBJECTDATA> &mapObj)
{
	if(mapObj.size()<1 /*|| m_mapRootObjects.size()<1 */) //m_mapRootObjects is critical but it can be zero size with filtering on
		return;	

	USES_CONVERSION;	
	
		try
		{
			 
			map<UINT_PTR,POBJECTDATA>::const_iterator ObjectIterator=mapObj.begin();			
			while(ObjectIterator != mapObj.end ())
			{
				//IS THIS LIVE OBJECT A ROOT OBJECT				 
				if(m_mapRootObjects.find(ObjectIterator->first)!=m_mapRootObjects.end())
				{
					m_mapRootObjects.erase(ObjectIterator->first);// This entry is done with.So clear it to decrease the root objects' map size as well as the live 
					//objects' map size.This will decrease the search time for next iteration
					//mapObj.erase(ObjectIterator->first); 
					//ObjectIterator=mapObj.begin();
					try	{ObjectIterator++;}catch(...){return;}
					continue;
					
				}

					char firstBuffer[32];
					char secondBuffer[32];
					char thirdBuffer[32];

					CString objString="'";					
												
						/////////////////
					//ObjectID
					ObjectID objID=g_Slogger->m_mapTempObjectIDToObjectTag[ObjectIterator->first];
					if(objID)
					{
						objString.Append(ultoa(objID,firstBuffer,10));	
					}
					else
					{
						objString.Append("N:A");	
					}										
					objString.Append("','");
						//////////

						//objectName

							try
						{
							CString strObj="";
							strObj=ObjectIterator->second->objName ;
							if(strObj)
							{
								//strObj.Replace("<","[");//validate for xml
								//strObj.Replace(">","]");

								//strObj.Replace("&","&amp;");//validate for xml
								//strObj.Replace("'","&apos;");//validate for xml
								//strObj.Replace("\"","&quot;");//validate for xml

								objString.Append(strObj);
							}
							strObj.Empty(); 
						}
						catch(...){}					

						objString.Append("',0");	

						/////////////////////
						//////////////////					
						
							if(WANT_OBJECT_SIZE)
							{
								
								objString.Append(ltoa(ObjectIterator->second->objSize,thirdBuffer,10));
								
							}								
							objString.Append(",0");
					
					//////////////////
						
							if(WANT_OBJECT_COUNT)
							{								
									objString.Append(ltoa(ObjectIterator->second->objCount,secondBuffer,10));
							
							}							
								objString.Append(",'0'");//Root Object?	   							

						TEXT_OUTLN(objString.GetBuffer());
						
						objString.Empty(); 
						
						////////////////////////
						try
						{
							ObjectIterator++;			
						}
						catch(...)
						{							
							return;

						}				
			}
		}
		catch(...)
		{	
			return;
		}

}


void DumpStringAndStructMapForRootObjects(map<UINT_PTR,POBJECTDATA> &mapObj)
{	
	if(mapObj.size()<1)
		return;	

	USES_CONVERSION;	
	
		try
		{
			 
			map<UINT_PTR,POBJECTDATA>::const_iterator RootIterator=mapObj.begin();			
			while(RootIterator != mapObj.end ())
			{	
					
					char firstBuffer[16];
					char secondBuffer[16];
					char thirdBuffer[16];
					
					CString objString="'";
						/////////////////
						//ObjectID
					ObjectID objID=g_Slogger->m_mapTempObjectIDToObjectTag[RootIterator->first];
					if(objID)
					{
						objString.Append(ultoa(objID,firstBuffer,10));	
					}
					else
					{
						objString.Append("N:A");	
					}
						
						//////////

						objString.Append("','");

						//ObjectName

						try
						{
							CString strObj="";								
							strObj=RootIterator->second->objName ;
							if(strObj)
							{
								//strObj.Replace("<","[");//validate for xml
								//strObj.Replace(">","]");

								//strObj.Replace("&","&amp;");//validate for xml
								//strObj.Replace("'","&apos;");//validate for xml
								//strObj.Replace("\"","&quot;");//validate for xml

								
								objString.Append(strObj);								
							}
							strObj.Empty(); 
						}
						catch(...){}	
						objString.Append("',0");


						
						//////////								
						
							if(WANT_OBJECT_SIZE)
							{										
								objString.Append(ltoa(RootIterator->second->objSize,thirdBuffer,10));
								
							}
							objString.Append(",0");//smart ;-)
					//////////////////

							/////////////////////
						
							if(WANT_OBJECT_COUNT)
							{
								objString.Append(ltoa(RootIterator->second->objCount,secondBuffer,10));
							}						

					//////////////////					
							//IsRootObject
								objString.Append(",'1'");//1 as it is a root object							
					/*
					//////////////////PercentSize
								if(WANT_OBJECT_SIZE && g_Slogger->m_ui64ObjectsSize !=0)
							{
								try
								{
									int dec,sign;
									CString strDec=_ecvt((RootIterator->second->objSize*100.00000000)/g_Slogger->m_ui64ObjectsSize,8,&dec,&sign);
									if(dec<=0)
										{
											objString.Append("0.") ;
											for(int x=0;x<abs(dec);x++)
											{
												objString+="0";
											}
											objString.Append(strDec);
										}
										else
										{
											objString.Append(strDec.Left(dec));
											objString.Append(".");
											objString.Append(strDec.Right(strDec.GetLength()-dec));  
										}												
								}catch(...){}
							}							
					//////////////////
							objString.Append(",");

							/////////////////////
						
							if(WANT_OBJECT_COUNT && g_Slogger->m_ui64ObjectCount !=0)
							{
								try
								{
									int dec,sign;
									CString strDec=_ecvt((RootIterator->second->objCount * 100.00000000)/g_Slogger->m_ui64ObjectCount,8,&dec,&sign);
									if(dec<=0)
										{
											objString.Append("0.") ;
											for(int x=0;x<abs(dec);x++)
											{
												objString+="0";
											}
											objString.Append(strDec);
										}
										else
										{
											objString.Append(strDec.Left(dec));
											objString.Append(".");
											objString.Append(strDec.Right(strDec.GetLength()-dec));  
										}									

								}catch(...){}								
							}						
				*/

						TEXT_OUTLN(objString.GetBuffer());
						
						objString.Empty(); 						
						////////////////////////

						try
						{
							RootIterator++;			
						}
						catch(...)
						{							
							return;
						}						
			}
		}
		catch(...)
		{	
			return;
		}
			
}

void DumpStringAndStructMapForReferencedObjects(map<UINT_PTR, map<UINT_PTR,POBJECTDATA> > &mapObj)
{	
	
	if(!WANT_REFERENCED_OBJECTS  || mapObj.size()<1 )
		return;

	USES_CONVERSION;
	
	try
	{
	map<UINT_PTR, map<UINT_PTR,POBJECTDATA> >::const_iterator ObjectIterator=mapObj.begin();	
	while(ObjectIterator != mapObj.end ())
		{		
				//Dump Referenced Objects							
				map<UINT_PTR,POBJECTDATA>::const_iterator RefIterator=ObjectIterator->second.begin() ; 
				while(RefIterator != ObjectIterator->second.end())								
				{
						char firstBuffer[32];
						char secondBuffer[32];

						CString strRefString="'";				

						
						//RefObjectID	
					ObjectID refObjID=g_Slogger->m_mapTempObjectIDToObjectTag[RefIterator->first];
					if(refObjID)
					{						
						strRefString.Append(ultoa(refObjID,secondBuffer,10));	
					}
					else
					{
						refObjID=g_Slogger->m_mapObjectIDToObjectTag[RefIterator->first];
						if(refObjID)
						{			
							
							if(g_Slogger->m_bIsTwoDotO)
								{
									g_Slogger->m_mapTempObjectIDToClassID[RefIterator->first]=g_Slogger->m_mapObjectIDToClassID[RefIterator->first];
									try
										{ 
											g_Slogger->m_mapObjectIDToClassID.erase(RefIterator->first);
										}
									catch(...){}

								}								
							g_Slogger->m_mapTempObjectIDToObjectTag[RefIterator->first]=refObjID; 
							try
							{
								g_Slogger->m_mapObjectIDToObjectTag.erase(RefIterator->first);
							}catch(...){}
							strRefString.Append(ultoa(refObjID,secondBuffer,10));	
						}
						else
						{
							strRefString.Append("N:A");	
						}
					}							
				
					strRefString.Append("','");	
						//ParentObjectID

					ObjectID objID=g_Slogger->m_mapTempObjectIDToObjectTag[ObjectIterator->first];
					if(objID)
					{
						strRefString.Append(ultoa(objID,firstBuffer,10));	
					}
					else
					{
						strRefString.Append("N:A");	
					}

						strRefString.Append("','"); 						
						try
						{
							CString strObj=RefIterator->second->objName;
							if(strObj)
							{
								//strObj.Replace("<","[");//validate for xml
								//strObj.Replace(">","]");

								//strObj.Replace("&","&amp;");//validate for xml
								//strObj.Replace("'","&apos;");//validate for xml
								//strObj.Replace("\"","&quot;");//validate for xml
							}

							strRefString.Append(strObj);   
						}
						catch(...){}	

						strRefString.Append("',0");

//						/*

						
						//if(WANT_REFERENCED_OBJECTS_COUNT)
						{	
							strRefString.Append(ltoa(RefIterator->second->objCount,firstBuffer,10));
						}
						
						//if(WANT_REFERENCED_OBJECTS_SIZE)
						{
							strRefString.Append(",0");
							strRefString.Append(ltoa(RefIterator->second->objSize,firstBuffer,10));
						}

					
						TEXT_OUTLN(strRefString.GetBuffer());						
						strRefString.Empty();						
						try
						{
							RefIterator++;
						}
						catch(...)			
						{	
							return;											
						}	
							
				}

				try	{ObjectIterator++;}	catch(...){return;}	
		}				
	}
	catch(...){}
}


static HRESULT __stdcall StackTracer(FunctionID funcId, 
	UINT_PTR ip, // instruction pointer for next instruction in frame
	COR_PRF_FRAME_INFO frameInfo,ULONG32 contextSize,BYTE byteContext[],void* clientData)	
	{
			if(funcId==0)
				{
					return S_OK;
				}                            
			else			  
				{
					FunctionInfo* _parentFunctionInfo=NULL;
					UINT64 cycles=0;
					PPERFORMANCEDATA perfData=NULL;					
					perfData=(PPERFORMANCEDATA)clientData;
					try
						{
							 _parentFunctionInfo=perfData->pFuncInfo;
							 cycles=perfData->cycleCounts;
						}catch(...){}
					
					try
						{ 
							if(_parentFunctionInfo && funcId==_parentFunctionInfo->fid)
								{	
									UINT_PTR IP=0;
									if(_parentFunctionInfo->baseIP)
										{
											IP=_parentFunctionInfo->baseIP;
										}
									else
										{
											ULONG codeSize=0;
											HRESULT hr=g_Slogger->m_pCorProfilerInfo2->GetCodeInfo(funcId,(LPCBYTE*)(&IP),&codeSize); 
											if(SUCCEEDED(hr))
												{
													_parentFunctionInfo->baseIP=IP; 
												}
											else
												{
													_parentFunctionInfo->baseIP=ip;
												}
										}
									
									IP=ip-IP;
									_parentFunctionInfo->m_mapIPToDebug[IP].consumedTime+=cycles;
									_parentFunctionInfo->m_mapIPToDebug[IP].hitCount+=1;
									return E_FAIL;
								}							
						}catch(...)
						{}
					return S_OK;
				}
			
	}

private:

ThreadInfo* GetThreadInfo( ThreadID tid )
{
  if ( _mThreadInfo.find( tid ) == _mThreadInfo.end() )
  {
    ThreadInfo* pThreadInfo = new ThreadInfo();
    _mThreadInfo.insert( make_pair( tid, pThreadInfo ) );
    return pThreadInfo;
  }
  
  return _mThreadInfo[ tid ];
}

void EndAll(  )
{
  for ( map< ThreadID, ThreadInfo* >::iterator it = _mThreadInfo.begin(); it != _mThreadInfo.end(); it++ )
  {
    if ( it->second->IsRunning() )
	{
      ThreadInfo* pThreadInfo = GetThreadInfo(it->first );
		pThreadInfo->End();
	}
  }
}

public:

        STDMETHOD( Initialize )( IUnknown *pICorProfilerInfoUnk );
        STDMETHOD( Shutdown )();

		// APPLICATION DOMAIN EVENTS
	   	STDMETHOD( AppDomainCreationStarted )( AppDomainID appDomainID )
		{
			return S_OK;
		}

        STDMETHOD( AppDomainCreationFinished )( AppDomainID appDomainID, HRESULT hrStatus )
		{
			return S_OK;
		}

        STDMETHOD( AppDomainShutdownStarted )( AppDomainID appDomainID )
		{
			return S_OK;
		}

		STDMETHOD( AppDomainShutdownFinished )( AppDomainID appDomainID, HRESULT hrStatus )
		{
			return S_OK;
		}

	 	// ASSEMBLY EVENTS
	   	STDMETHOD( AssemblyLoadStarted )( AssemblyID assemblyID )
		{
			return S_OK;
		}
		STDMETHOD( AssemblyLoadFinished )( AssemblyID assemblyID, HRESULT hrStatus )
		{
			return S_OK;
		}
        STDMETHOD( AssemblyUnloadStarted )( AssemblyID assemblyID )
		{
			return S_OK;
		}

		STDMETHOD( AssemblyUnloadFinished )( AssemblyID assemblyID, HRESULT hrStatus )
		{
			return S_OK;
		}

		// MODULE EVENTS

		STDMETHOD( ModuleLoadStarted)( ModuleID moduleID )
		{
			return S_OK;
		}
	   
    	STDMETHOD( ModuleLoadFinished )( ModuleID moduleID, HRESULT hrStatus );

		STDMETHOD( GarbageCollectionStarted)( 
            int cGenerations,
            BOOL generationCollected[  ],
            COR_PRF_GC_REASON reason);

		STDMETHOD (ThreadNameChanged)( 
            ThreadID threadId,
             ULONG cchName,
             WCHAR name[  ]);
		
		STDMETHOD (SurvivingReferences)(
                unsigned long cSurvivingObjectIDRanges, 
                UINT_PTR* objectIDRangeStart, 
                unsigned long* cObjectIDRangeLength);
		
		STDMETHOD (GarbageCollectionFinished)();
			
		STDMETHOD (FinalizeableObjectQueued)(
                unsigned long finalizerFlags, 
                UINT_PTR objectId);
			
		STDMETHOD (RootReferences2)(
                unsigned long cRootRefs, 
                UINT_PTR* rootRefIds, 
                COR_PRF_GC_ROOT_KIND* rootKinds, 
                COR_PRF_GC_ROOT_FLAGS* rootFlags, 
                UINT_PTR* rootIds);
				
		STDMETHOD (HandleCreated)(
                UINT_PTR handleId, 
                UINT_PTR initialObjectId);
		
		STDMETHOD (HandleDestroyed)(UINT_PTR handleId);
		
        STDMETHOD( ModuleUnloadStarted )( ModuleID moduleID )
		{
			return S_OK;
		}
		STDMETHOD( ModuleUnloadFinished )( ModuleID moduleID, HRESULT hrStatus );
		

		STDMETHOD( ModuleAttachedToAssembly )( ModuleID moduleID, AssemblyID assemblyID )
		{
			return S_OK;
		}


		// CLASS EVENTS
        STDMETHOD( ClassLoadStarted )( ClassID classID )
		{
			return S_OK;
		}

        STDMETHOD( ClassLoadFinished )( ClassID classID, HRESULT hrStatus )
		{
			return S_OK;
		}

     	STDMETHOD( ClassUnloadStarted )( ClassID classID )
		{
			return S_OK;
		}
		STDMETHOD( ClassUnloadFinished )( ClassID classID, HRESULT hrStatus )
		{
			return S_OK;
		}
		STDMETHOD( FunctionUnloadStarted )( FunctionID functionID )
		{
			return S_OK;
		}

		// JIT EVENTS
        STDMETHOD( JITCompilationStarted )( FunctionID functionID, BOOL fIsSafeToBlock )  ; 
		
        STDMETHOD( JITCompilationFinished )( FunctionID functionID, HRESULT hrStatus, BOOL fIsSafeToBlock )
		{
			return S_OK;
		}

        STDMETHOD( JITCachedFunctionSearchStarted )( FunctionID functionID, BOOL *pbUseCachedFunction )
		{
			*pbUseCachedFunction=TRUE;
			return S_OK;
		}

		STDMETHOD( JITCachedFunctionSearchFinished )( FunctionID functionID, COR_PRF_JIT_CACHE result )
		{
			return S_OK;
		}
        STDMETHOD( JITFunctionPitched )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( JITInlining )( FunctionID callerID, FunctionID calleeID, BOOL *pfShouldInline )
		{
			*pfShouldInline=false;
			return S_OK;

		}

		// THREAD EVENTS
        STDMETHOD( ThreadCreated )( ThreadID threadID )
		{   

			//try
			//{	
			//	if(m_bSuspendOnStart)
			//	{
			//		HANDLE m_hCLRThread=NULL;
			//		HRESULT hr=m_pCorProfilerInfo2->GetHandleFromThread(threadID,&m_hCLRThread);
			//		if(SUCCEEDED(hr) && m_hCLRThread)
			//		{	
			//			m_qCLRHandle.push(m_hCLRThread);
			//			DWORD dw=SuspendThread(m_hCLRThread); 	//as this is the first thread to be created						
			//		//it is the main thread.So suspend it
			//		}
			//	}
			//}
			//catch(...){}
					
		HRESULT hr=S_OK;
				
#pragma warning ("The code below is errorsome")
				//try
				//{
				//	if( m_hInprocMutex && !m_bIsTwoDotO)
				//		{
				//			hr=m_pCorProfilerInfo2->BeginInprocDebugging(TRUE,&(m_mapDebugMap[threadID]));	
				//		}
				//}catch(...){}

		///////////////////////////
				
			try
				{
					if((IS_PROFILEDPROCESSFORFUNCTIONS) ||(IS_PROFILEDPROCESSFOROBJECTS) )  //this line will resume only if ResumeCLR has been called once
					{
						m_bSuspendOnStart=false;//now no more threads will be suspended    
						ThreadInfo* pThreadInfo = NULL;
						pThreadInfo=GetThreadInfo( threadID );
						if(pThreadInfo)
						pThreadInfo->Start();
					}
				}
					catch(...){	}
			return S_OK;	

		}

        STDMETHOD( ThreadDestroyed )( ThreadID threadID )
		{
		#pragma warning ("The code below is errorsome")
			/*try
				{
				if(m_hInprocMutex && !m_bIsTwoDotO)
					{
						m_pCorProfilerInfo2->EndInprocDebugging(m_mapDebugMap[threadID]);					
					}
				}
				catch(...){}*/
		////////////////////////////////////////


			try
				{
					m_mapDebugMap.erase(threadID);
				}catch(...){}

			if((IS_PROFILEDPROCESSFORFUNCTIONS) || (IS_PROFILEDPROCESSFOROBJECTS) )  
			{				
				try
				{
					ThreadInfo* pThreadInfo = NULL;
					pThreadInfo=GetThreadInfo( threadID );
					if(pThreadInfo) 
					pThreadInfo->End();
				}catch(...){}
			}
				return S_OK;
		}

        STDMETHOD( ThreadAssignedToOSThread )( ThreadID managedThreadID, DWORD osThreadID )
		{
			return S_OK;
		}
    
        // REMOTING EVENTS
        // Client-side events
        STDMETHOD( RemotingClientInvocationStarted )()
		{
			return S_OK;
		}
        STDMETHOD( RemotingClientSendingMessage )( GUID *pCookie, BOOL fIsAsync )
		{
			return S_OK;
		}
        STDMETHOD( RemotingClientReceivingReply )( GUID *pCookie, BOOL fIsAsync )
		{
			return S_OK;
		}
        STDMETHOD( RemotingClientInvocationFinished )()
		{
			return S_OK;
		}
        // Server-side events
        STDMETHOD( RemotingServerReceivingMessage )( GUID *pCookie, BOOL fIsAsync )
		{
			return S_OK;
		}
        STDMETHOD( RemotingServerInvocationStarted )()
		{
			return S_OK;
		}
        STDMETHOD( RemotingServerInvocationReturned )()
		{
			return S_OK;
		}
        STDMETHOD( RemotingServerSendingReply )( GUID *pCookie, BOOL fIsAsync )
		{
			return S_OK;
		}

        // CONTEXT EVENTS
    	STDMETHOD( UnmanagedToManagedTransition )( FunctionID functionID, COR_PRF_TRANSITION_REASON reason );
		

        STDMETHOD( ManagedToUnmanagedTransition )( FunctionID functionID, COR_PRF_TRANSITION_REASON reason );
		
                                                                  
        // SUSPENSION EVENTS
        STDMETHOD( RuntimeSuspendStarted )( COR_PRF_SUSPEND_REASON suspendReason );
		
        STDMETHOD( RuntimeSuspendFinished )()
		{				
			return S_OK;
		}

        STDMETHOD( RuntimeSuspendAborted )()
		{
			return S_OK;
		}

        STDMETHOD( RuntimeResumeStarted )();		

        STDMETHOD( RuntimeResumeFinished )() ;

        STDMETHOD( RuntimeThreadSuspended )( ThreadID threadid )
		{
			return S_OK;
		}
        STDMETHOD( RuntimeThreadResumed )( ThreadID threadid )
		{
			return S_OK;
		}
		// GC EVENTS
		STDMETHOD( MovedReferences )( ULONG cmovedObjectIDRanges,
                                      ObjectID oldObjectIDRangeStart[],
                                      ObjectID newObjectIDRangeStart[],
                                      ULONG cObjectIDRangeLength[] )
		{

		
			if(g_Slogger)
			{ 
					map<ObjectID,ObjectID>::iterator ObjectIterator=m_mapObjectIDToObjectTag.begin(); 																	
						ULONG32	i=0;
						ObjectID ObjectSlabInfra=oldObjectIDRangeStart[0];
						ObjectID ObjectSlabUltra=oldObjectIDRangeStart[cmovedObjectIDRanges-1] + cObjectIDRangeLength[cmovedObjectIDRanges-1];
						ObjectID ObjectSlabMovedMinSentinel=oldObjectIDRangeStart[i];
						ObjectID ObjectSlabMovedMaxSentinel=oldObjectIDRangeStart[i]+ cObjectIDRangeLength[i];
						ObjectID ObjectIDInSlab=oldObjectIDRangeStart[i] - newObjectIDRangeStart[i];

						while(m_mapObjectIDToObjectTag.size())
						{	
							try
							{
								if(ObjectIterator->first < ObjectSlabInfra  || ObjectIterator->first >	(ObjectSlabUltra))
								{
									m_mapTempObjectIDToObjectTag[ObjectIterator->first]=ObjectIterator->second;
									if(m_bIsTwoDotO)
										{
											m_mapTempObjectIDToClassID[ObjectIterator->first]=m_mapObjectIDToClassID[ObjectIterator->first];
										}
									goto A;
								}
								else if((ObjectSlabMovedMinSentinel<= ObjectIterator->first) && (ObjectIterator->first	<= (ObjectSlabMovedMaxSentinel)))
								{
									m_mapTempObjectIDToObjectTag[ObjectIterator->first - (ObjectIDInSlab)]=ObjectIterator->second;
									if(m_bIsTwoDotO)
										{
											m_mapTempObjectIDToClassID[ObjectIterator->first - (ObjectIDInSlab)]=m_mapObjectIDToClassID[ObjectIterator->first];
										}
									goto A;									
								}
								else
									{									
										m_mapTempObjectIDToObjectTag[ObjectIterator->first]=ObjectIterator->second;
											if(m_bIsTwoDotO)
												{
													m_mapTempObjectIDToClassID[ObjectIterator->first]=m_mapObjectIDToClassID[ObjectIterator->first];
												}
										goto A;
									}
							}
							catch(...){}
A:						
						if(m_bIsTwoDotO)
							{
								try
									{
									if(m_mapObjectIDToClassID.size())
										{
											m_mapObjectIDToClassID.erase(ObjectIterator->first);										
										}
									}catch(...){}
							}
						try
							{
							if(m_mapObjectIDToObjectTag.size())
								{
									m_mapObjectIDToObjectTag.erase(ObjectIterator);
									if(m_mapObjectIDToObjectTag.size())
									{
										ObjectIterator=m_mapObjectIDToObjectTag.begin ();
									}
									else
										{
											break;
										}
								}								
								else
									{
										break;
									}
							}catch(...){}

						try
							{
								if( m_mapObjectIDToObjectTag.size() && ObjectIterator->first > (ObjectSlabMovedMaxSentinel))
										{
											i++;
											if(i<cmovedObjectIDRanges)
												{
													ObjectSlabMovedMinSentinel=oldObjectIDRangeStart[i];
													ObjectSlabMovedMaxSentinel=oldObjectIDRangeStart[i]	+ cObjectIDRangeLength[i];									
													ObjectIDInSlab=oldObjectIDRangeStart[i] - newObjectIDRangeStart[i];											
												}
											else
												{
													break;
												}									
											
										}								

							} catch(...){ break;}
						}

						
					while(m_mapObjectIDToObjectTag.size())
						{
							map<ObjectID,ObjectID>::iterator	ObjectLastIterator=m_mapObjectIDToObjectTag.begin(); 	
							m_mapTempObjectIDToObjectTag[ObjectLastIterator->first]=ObjectLastIterator->second;
									if(m_bIsTwoDotO)
										{
											m_mapTempObjectIDToClassID[ObjectLastIterator->first]=m_mapObjectIDToClassID[ObjectLastIterator->first];
										}
							if(m_bIsTwoDotO)
							{
								try
									{
										m_mapObjectIDToClassID.erase(ObjectLastIterator->first);										
									}catch(...){}
							}
							try
							{
								m_mapObjectIDToObjectTag.erase(ObjectLastIterator);
								if(m_mapObjectIDToObjectTag.size())
									{
										ObjectLastIterator=m_mapObjectIDToObjectTag.begin ();	
									}
								else
									{
										break;
									}
								
							}catch(...)	
								{
									break;
								}
						}

						m_mapObjectIDToObjectTag.clear();
						m_mapObjectIDToObjectTag.swap(m_mapTempObjectIDToObjectTag);//V. Imp.	to make	sure that the map is not size==0 for multiple MovedReferences callbacks
						m_mapTempObjectIDToObjectTag.clear();
						if(m_bIsTwoDotO)
							{
								m_mapObjectIDToClassID.clear();
								m_mapObjectIDToClassID.swap(m_mapTempObjectIDToClassID);
								m_mapTempObjectIDToClassID.clear() ;
							}

				}


			if(WANT_OBJECT_ALLOCATION_DATA &&  IS_PROFILEDPROCESSFOROBJECTS)
					{						
					if(g_Slogger)
						{
							map<UINT_PTR,OBJECTALLOCATIONDATA>::iterator ObjectIterator=m_mapObjectAllocation.begin	();																		
							ULONG32	i=0;							
							ObjectID ObjectSlabInfra=oldObjectIDRangeStart[0];
							ObjectID ObjectSlabUltra=oldObjectIDRangeStart[cmovedObjectIDRanges-1] + cObjectIDRangeLength[cmovedObjectIDRanges-1];
							ObjectID ObjectSlabMovedMinSentinel=oldObjectIDRangeStart[i];
							ObjectID ObjectSlabMovedMaxSentinel=oldObjectIDRangeStart[i]	+ cObjectIDRangeLength[i];
							ObjectID ObjectIDInSlab=oldObjectIDRangeStart[i] - newObjectIDRangeStart[i];

							while(m_mapObjectAllocation.size())
							{
								try
								{
									++(ObjectIterator->second.lAge);	
									if(ObjectIterator->first < ObjectSlabInfra	|| ObjectIterator->first >	(ObjectSlabUltra))
									{
										m_mapSwapObjectAllocation[ObjectIterator->first]=ObjectIterator->second;
										goto B;
									}
									if((ObjectSlabMovedMinSentinel<= ObjectIterator->first) && (ObjectIterator->first	<= (ObjectSlabMovedMaxSentinel)))
									{
										m_mapSwapObjectAllocation[ObjectIterator->first	- ObjectIDInSlab]=ObjectIterator->second;
										goto B;										
									}									
									else
									{
										m_mapSwapObjectAllocation[ObjectIterator->first]=ObjectIterator->second;
										goto B;
									}
								}
								catch(...)
									{	}
B:								try
								{
									if(m_mapObjectAllocation.size())
										{
											m_mapObjectAllocation.erase(ObjectIterator);
											if(m_mapObjectAllocation.size())
												{
													ObjectIterator=m_mapObjectAllocation.begin ();	
												}
											else
												{
													break;
												}
										}
										else
										{
											break;
										}									
								}catch(...)	{}

							try
								{
									if(m_mapObjectAllocation.size() && ObjectIterator->first > (ObjectSlabMovedMaxSentinel))
									{
										i++;
										if(i<cmovedObjectIDRanges)
											{
												ObjectSlabMovedMinSentinel=oldObjectIDRangeStart[i];
												ObjectSlabMovedMaxSentinel=oldObjectIDRangeStart[i]	+ cObjectIDRangeLength[i];									
												ObjectIDInSlab=oldObjectIDRangeStart[i] - newObjectIDRangeStart[i];											
											}
										else
											{
												break;
											}
									}	
								}  
									catch(...){break;}

							}

							while(m_mapObjectAllocation.size())
								{
									map<ObjectID,OBJECTALLOCATIONDATA>::iterator ObjectLastIterator=m_mapObjectAllocation.begin(); 	
									m_mapSwapObjectAllocation[ObjectLastIterator->first]=ObjectLastIterator->second;
									try
									{
										m_mapObjectAllocation.erase(ObjectLastIterator);
										if(m_mapObjectAllocation.size())
											{
												ObjectLastIterator=m_mapObjectAllocation.begin ();													
											}
										else
											{
												break;
											}
										
									}catch(...)	
										{
											break;
										}
								} 
							
							m_mapObjectAllocation.clear();
							m_mapObjectAllocation.swap(m_mapSwapObjectAllocation);
							m_mapSwapObjectAllocation.clear();
						}		
				}
				
return S_OK;
				
		}


void GetFunctionSignature( 
  FunctionID fid,
  DWORD& dwMethodAttributes,
  string& strRet, 
  string& strClassName,
  string& strFnName,
  string& strParameters,
  string& strModuleName ,
  CString** strFiles,
  ULONG32 &spCount,
  ULONG32 **spLines,
  ULONG32 **spOffsets,
  ULONG32** spStartColumn,
  ULONG32** spEndColumn
  )
{
	USES_CONVERSION ;
    ULONG ulArgs;
    WCHAR wszRet[ MAX_FUNCTION_LENGTH ];
	wszRet[0]=NULL;
    WCHAR wszParams[ MAX_FUNCTION_LENGTH ];
	wszParams[0]=NULL;
    WCHAR wszFnName[ MAX_FUNCTION_LENGTH ];
	wszFnName[0]=NULL;
    WCHAR wszClassName[ MAX_FUNCTION_LENGTH ];
	wszClassName[0]=NULL;
	WCHAR wszModuleName[MAX_FUNCTION_LENGTH];
	wszModuleName[0]=NULL;
	*strFiles=NULL;
	spCount=0;
	*spLines=NULL;
	*spOffsets=NULL;
	*spStartColumn=NULL;
	*spEndColumn=NULL;
	try
		{

			GetFunctionProperties( fid, &dwMethodAttributes, &ulArgs, wszRet, wszParams, wszClassName, wszFnName,wszModuleName,strFiles,spCount,spLines,spOffsets,spStartColumn,spEndColumn);
		}catch(...){}

	strRet = (wszRet==NULL)?"":W2A( wszRet );
    strParameters = (wszParams==NULL)?"":W2A( wszParams );
    strClassName = (wszClassName==NULL)?"":W2A( wszClassName );
    strFnName = (wszFnName==NULL)?"":W2A( wszFnName );
	strModuleName = (wszModuleName==NULL)?"":W2A(wszModuleName);	

}
private:
	PCCOR_SIGNATURE ParseElementType( IMetaDataImport *pMDImport,
											  PCCOR_SIGNATURE signature, 
											  char *buffer )
{	
	switch ( *signature++ ) 
	{	
		case ELEMENT_TYPE_VOID:
        	strcat( buffer, "void" );	
			break;					
		
        
		case ELEMENT_TYPE_BOOLEAN:	
			strcat( buffer, "bool" );	
			break;	
		
        
		case ELEMENT_TYPE_CHAR:
        	strcat( buffer, "wchar" );	
			break;		
					
        
		case ELEMENT_TYPE_I1:
        	strcat( buffer, "int8" );	
			break;		
 		
        
		case ELEMENT_TYPE_U1:
        	strcat( buffer, "unsigned int8" );	
			break;		
		
        
		case ELEMENT_TYPE_I2:
        	strcat( buffer, "int16" );	
			break;		
		
        
		case ELEMENT_TYPE_U2:
        	strcat( buffer, "unsigned int16" );	
			break;			
		
        
		case ELEMENT_TYPE_I4:
        	strcat( buffer, "int32" );	
			break;
            
        
		case ELEMENT_TYPE_U4:
        	strcat( buffer, "unsigned int32" );	
			break;		
		
        
		case ELEMENT_TYPE_I8:
        	strcat( buffer, "int64" );	
			break;		
		
        
		case ELEMENT_TYPE_U8:
        	strcat( buffer, "unsigned int64" );	
			break;		
		
        
		case ELEMENT_TYPE_R4:
        	strcat( buffer, "float32" );	
			break;			
		
        
		case ELEMENT_TYPE_R8:
        	strcat( buffer, "float64" );	
			break;		
		
        
		case ELEMENT_TYPE_U:
        	strcat( buffer, "unsigned int" );	
			break;		 
		
        
		case ELEMENT_TYPE_I:
        	strcat( buffer, "int" );	
			break;			  
		
        
		case ELEMENT_TYPE_OBJECT:
        	strcat( buffer, "Object" );	
			break;		 
		
        
		case ELEMENT_TYPE_STRING:
        	strcat( buffer, "String" );	
			break;		 
		
        
		case ELEMENT_TYPE_TYPEDBYREF:
        	strcat( buffer, "refany" );	
			break;				       

		case ELEMENT_TYPE_CLASS:	
		case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_CMOD_OPT:
        	{	
				mdToken	token;	
				char classname[MAX_FUNCTION_LENGTH];


				classname[0] = '\0';
			   	signature += CorSigUncompressToken( signature, &token ); 
				if ( TypeFromToken( token ) != mdtTypeRef )
				{
                	HRESULT	hr;
					WCHAR zName[MAX_FUNCTION_LENGTH];
					
					
					hr = pMDImport->GetTypeDefProps( token, 
													 zName,
													 MAX_FUNCTION_LENGTH,
													 NULL,
													 NULL,
													 NULL );
					if ( SUCCEEDED( hr ) )
						wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
                }
                    
				strcat( buffer, classname );		
			}
            break;	
		
        
		case ELEMENT_TYPE_SZARRAY:	 
			signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "[]" );
			break;		
		
        
		case ELEMENT_TYPE_ARRAY:	
			{	
				ULONG rank;
                

				signature = ParseElementType( pMDImport, signature, buffer );                 
				rank = CorSigUncompressData( signature );													
				if ( rank == 0 ) 
					strcat( buffer, "[?]" );

				else 
				{
					ULONG *lower;	
					ULONG *sizes; 	
                    ULONG numsizes; 
					ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
                    
                                         
					lower = (ULONG *)_alloca( arraysize );                                                        
					memset( lower, 0, arraysize ); 
                    sizes = &lower[rank];

					numsizes = CorSigUncompressData( signature );	
					if ( numsizes <= rank )
					{
                    	ULONG numlower;
                        
                        
						for ( ULONG i = 0; i < numsizes; i++ )	
							sizes[i] = CorSigUncompressData( signature );	
						
                        
						numlower = CorSigUncompressData( signature );	
						if ( numlower <= rank )
						{
							for (int  i = 0; i < numlower; i++)	
								lower[i] = CorSigUncompressData( signature ); 
							
                            
							strcat( buffer, "[" );	
							for (int i = 0; i < rank; i++ )	
							{	
								if ( (sizes[i] != 0) && (lower[i] != 0) )	
								{	
									if ( lower[i] == 0 )	
										sprintf ( buffer, "%d", sizes[i] );	

									else	
									{	
										sprintf( buffer, "%d", lower[i] );	
										strcat( buffer, "..." );	
										
										if ( sizes[i] != 0 )	
											sprintf( buffer, "%d", (lower[i] + sizes[i] + 1) );	
									}	
								}
                                	
								if ( i < (rank - 1) ) 
									strcat( buffer, "," );	
							}	
                            
							strcat( buffer, "]" );  
						}						
					}
				}
			} 
			break;	

		
		case ELEMENT_TYPE_PINNED:
			signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "pinned" );	
			break;	
         
        
        case ELEMENT_TYPE_PTR:   
            signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "*" );	
			break;   
        
        
        case ELEMENT_TYPE_BYREF:   
            signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "& " );	//xml notation
			break;  		    


		default:	
		case ELEMENT_TYPE_END:	
		case ELEMENT_TYPE_SENTINEL:	
			strcat( buffer, "UNKNOWN" );	
			break;	
                        	
	}     
	
	return signature;

}

	
public:
	
		
	HRESULT CSlogger::GetClassName(IMetaDataImport *pMDImport, mdToken classToken, WCHAR className[], ClassID *classTypeArgs, ULONG *totalGenericArgCount)
{
    DWORD dwTypeDefFlags = 0;
    HRESULT hr = S_OK;
	className[0]='\0';
    hr = pMDImport->GetTypeDefProps( classToken, 
                                     className, 
                                     MAX_LENGTH,
                                     NULL, 
                                     &dwTypeDefFlags, 
                                     NULL ); 
		if ( FAILED( hr ) )
		{
			return hr;
		}
		*totalGenericArgCount = 0;
		if (IsTdNested(dwTypeDefFlags))
		{
			mdToken enclosingClass = mdTokenNil;
			hr = pMDImport->GetNestedClassProps(classToken, &enclosingClass);
			if ( FAILED( hr ) )
			{
				return hr;
			}
			hr = GetClassName(pMDImport, enclosingClass, className, classTypeArgs, totalGenericArgCount);
			if (FAILED(hr))
				return hr;
			size_t length = wcslen(className);
			if (length + 2 < MAX_LENGTH)
			{
				className[length++] = '.';
				hr = pMDImport->GetTypeDefProps( classToken, 
												className + length, 
												(ULONG)(MAX_LENGTH - length),
												NULL, 
												NULL, 
												NULL );
				if ( FAILED( hr ) )
				{
					return hr;
				}
			}
		}

		WCHAR *backTick = wcschr(className, L'`');
		if (backTick != NULL)
		{
			*backTick = L'\0';
			ULONG genericArgCount = wcstoul(backTick+1, NULL, 10);

			if (genericArgCount >0)
			{
				char typeArgText[MAX_LENGTH];
				typeArgText[0] = '\0';

				StrAppend(typeArgText, "<", MAX_LENGTH);
				for (ULONG i = *totalGenericArgCount; i < *totalGenericArgCount + genericArgCount; i++)
				{
					if (i != *totalGenericArgCount)
						StrAppend(typeArgText, ",", MAX_LENGTH);
					AppendTypeArgName(i, classTypeArgs, NULL, FALSE, typeArgText, MAX_LENGTH);
				}
				StrAppend(typeArgText, ">", MAX_LENGTH);

				*totalGenericArgCount += genericArgCount;    
				_snwprintf_s(className, MAX_LENGTH, MAX_LENGTH-1, L"%s%S", className, typeArgText);
			}
		}
    return hr;
}

bool GetClassNameFromClassID(ClassID classId,
                                                LPWSTR wszClass )
{
    ModuleID moduleId;
    mdTypeDef typeDef=0;

    wszClass[0] = 0;

    HRESULT hr = g_Slogger->m_pCorProfilerInfo2->GetClassIDInfo( classId, &moduleId,
                                                        &typeDef );
	if(moduleId==0 || typeDef==mdTypeDefNil)
			{
				GetArrayClassName(classId,wszClass);
				if(wszClass==NULL)
					{
						wcscpy_s(wszClass, MAX_LENGTH, L"UNKNOWN CLASS");
					}
				return true;
			}

    if ( FAILED(hr) || typeDef == 0  )
        return false;

    
    CComPtr<IMetaDataImport> pIMetaDataImport = 0;
    hr = g_Slogger->m_pCorProfilerInfo2->GetModuleMetaData( moduleId, ofRead,
                                        IID_IMetaDataImport,
                                        (LPUNKNOWN *)&pIMetaDataImport );
    if ( FAILED(hr) || !pIMetaDataImport)
        return false;

   
    WCHAR wszTypeDef[MAX_LENGTH];
    DWORD cchTypeDef = ARRAY_LEN(wszTypeDef);
	wszTypeDef[0]='\0';

    hr = pIMetaDataImport->GetTypeDefProps( typeDef, wszTypeDef, cchTypeDef,
                                            &cchTypeDef, 0, 0 );
    if ( FAILED(hr) )
        return false;

    lstrcpyW( wszClass, wszTypeDef );   
    pIMetaDataImport.Release();

    return true;
}

HRESULT CSlogger::_GetNameFromElementType( CorElementType elementType, __out_ecount(buflen) WCHAR *buffer, size_t buflen )
{
    HRESULT hr = S_OK;

    switch ( elementType )
    {
        case ELEMENT_TYPE_BOOLEAN:
             wcscpy_s( buffer, buflen, L"System.Boolean" );
             break;

        case ELEMENT_TYPE_CHAR:
             wcscpy_s( buffer, buflen, L"System.Char" );
             break;

        case ELEMENT_TYPE_I1:
             wcscpy_s( buffer, buflen, L"System.SByte" );
             break;

        case ELEMENT_TYPE_U1:
             wcscpy_s( buffer, buflen, L"System.Byte" );
             break;

        case ELEMENT_TYPE_I2:
             wcscpy_s( buffer, buflen, L"System.Int16" );
             break;

        case ELEMENT_TYPE_U2:
             wcscpy_s( buffer, buflen, L"System.UInt16" );
             break;

        case ELEMENT_TYPE_I4:
             wcscpy_s( buffer, buflen, L"System.Int32" );
             break;

        case ELEMENT_TYPE_U4:
             wcscpy_s( buffer, buflen, L"System.UInt32" );
             break;

        case ELEMENT_TYPE_I8:
             wcscpy_s( buffer, buflen, L"System.Int64" );
             break;

        case ELEMENT_TYPE_U8:
             wcscpy_s( buffer, buflen, L"System.UInt64" );
             break;

        case ELEMENT_TYPE_R4:
             wcscpy_s( buffer, buflen, L"System.Single" );
             break;

        case ELEMENT_TYPE_R8:
             wcscpy_s( buffer, buflen, L"System.Double" );
             break;

        case ELEMENT_TYPE_STRING:
             wcscpy_s( buffer, buflen, L"System.String" );
             break;

        case ELEMENT_TYPE_PTR:
             wcscpy_s( buffer, buflen, L"System.IntPtr" );
             break;

        case ELEMENT_TYPE_VALUETYPE:
             wcscpy_s( buffer, buflen, L"struct" );
             break;

        case ELEMENT_TYPE_CLASS:
             wcscpy_s( buffer, buflen, L"class" );
             break;

        case ELEMENT_TYPE_ARRAY:
             wcscpy_s( buffer, buflen, L"System.Array" );
             break;

        case ELEMENT_TYPE_I:
             wcscpy_s( buffer, buflen, L"int_ptr" );
             break;

        case ELEMENT_TYPE_U:
             wcscpy_s( buffer, buflen, L"unsigned int_ptr" );
             break;

        case ELEMENT_TYPE_OBJECT:
             wcscpy_s( buffer, buflen, L"System.Object" );
             break;

        case ELEMENT_TYPE_SZARRAY:
             wcscpy_s( buffer, buflen, L"System.Array" );
             break;

        case ELEMENT_TYPE_MAX:
        case ELEMENT_TYPE_END:
        case ELEMENT_TYPE_VOID:
        case ELEMENT_TYPE_FNPTR:
        case ELEMENT_TYPE_BYREF:
        case ELEMENT_TYPE_PINNED:
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_MODIFIER:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_TYPEDBYREF:
        default:
			wcscpy_s( buffer, buflen, L"{UNKNOWN}" );
             break;
    }

    return hr;
} 


    
HRESULT CSlogger::GetNameFromClassID( ClassID classID, WCHAR className[] )
{
      if ( m_pCorProfilerInfo2 == NULL )
		  {
			return E_FAIL;
		  }
 		if(!m_bIsTwoDotO)
			{
				GetClassNameFromClassID(classID,className);
				return S_OK;
			}
	
	//////////////////////////////////

        ModuleID moduleID=0;
        mdTypeDef classToken=0;                                                                                                               
        CComPtr<IMetaDataImport> pMDImport=NULL ; 
		ClassID *classTypeArgs = NULL;
        ULONG32 classTypeArgCount = 0;
		DWORD dwTypeDefFlags = 0;
		ULONG genericArgCount = 0;
		HRESULT hr=S_OK;
		hr = g_Slogger->m_pCorProfilerInfo2->GetClassIDInfo( classID, &moduleID,
                                                        &classToken );
		if(moduleID==0 || classToken==mdTypeDefNil)
			{
				GetArrayClassName(classID,className);
				if(className==NULL)
					{
						wcscpy_s(className, MAX_LENGTH, L"UNKNOWN CLASS");
					}
				return S_OK;
			}
		
		if(SUCCEEDED(hr))
			{
				hr= m_pCorProfilerInfo2->GetModuleMetaData( moduleID, 
															(ofRead),
															IID_IMetaDataImport, 
															(IUnknown **)&pMDImport );

				if ( pMDImport && classToken != mdTypeDefNil )
					{
						 hr = m_pCorProfilerInfo2->GetClassIDInfo2(classID,
                                                               NULL,
                                                               NULL,
                                                               NULL,
                                                               0,
                                                               &classTypeArgCount,
                                                               NULL);
						if (SUCCEEDED(hr) && classTypeArgCount > 0)
							{
								classTypeArgs = (ClassID *)_alloca(classTypeArgCount*sizeof(classTypeArgs[0]));
								hr = m_pCorProfilerInfo2->GetClassIDInfo2(classID,
																	NULL,
																	NULL,
																	NULL,
																	classTypeArgCount,
																	&classTypeArgCount,
																	classTypeArgs);
							}
						else
							{
								classTypeArgs = NULL;
							}

					hr = GetClassName(pMDImport, classToken, className, classTypeArgs, &genericArgCount);

					}
				else
					{
						wcscpy_s(className, MAX_LENGTH, L"UNKNOWN CLASS");
						return S_OK;
					}
			}
         else
			 {
				wcscpy_s(className, MAX_LENGTH, L"UNMANAGED FRAME");
				return S_OK;
		   }
    return hr;
} 




HRESULT CSlogger::GetArrayClassName( ClassID classID, WCHAR className[] )
	{
		ULONG rank = 0;
		CorElementType elementType;
		ClassID realClassID = NULL;
		WCHAR ranks[256];
		ranks[0] = '\0';
		className[0] = '\0';
		HRESULT hr=E_FAIL;
		try
			{

				hr = m_pCorProfilerInfo2->IsArrayClass( classID, &elementType, &realClassID, &rank );
				if ( hr == S_OK )
				{
					ClassID prevClassID; 
					do
					{
						prevClassID = realClassID;
						const size_t len = ARRAY_LEN(ranks);
						_snwprintf_s( ranks, ARRAY_LEN(ranks), ARRAY_LEN(ranks)-1, L"%s[]", ranks);
						hr = m_pCorProfilerInfo2->IsArrayClass( prevClassID, &elementType, &realClassID, &rank );
						if ( (hr == S_FALSE) || (FAILED(hr)) || (realClassID == NULL) )
						{
						realClassID = prevClassID;                        
							break;
						}
					}
					while ( TRUE );
		            
					if ( SUCCEEDED( hr ) )
					{
						className[0] = '\0';
						if ( realClassID != NULL )
							hr = GetNameFromClassID( realClassID, className );
						else
							hr = _GetNameFromElementType( elementType, className, MAX_LENGTH );
		        
						if ( SUCCEEDED( hr ) && className[0]!='\0')
						{                       
							_snwprintf_s( className, MAX_LENGTH, MAX_LENGTH-1, L"%s %s",className, ranks  );
						}
						else
							{	
								className[0] = '\0';
								return hr;
							}
					}
					else                    
					{
						className[0] = '\0';
							return hr;
					}
				}
				else
					{	
						className[0] = '\0';
						return hr;
					}
			}catch(...)
			{			
				hr=S_OK;
			}
			return S_OK;
	}

HRESULT CSlogger::GetFunctionProperties( 
			FunctionID functionID,   DWORD* pdwMethodAttr,
								  ULONG *argCount,
									   WCHAR *returnTypeStr, 
									   WCHAR *functionParameters,
									   WCHAR *className, WCHAR *funName,WCHAR* moduleName,
									   CString** fileNames,ULONG32 &spCount,
									   ULONG32 **spLines,ULONG32 **spOffsets,ULONG32** spStartColumn,
									   ULONG32** spEndColumn
										   )
{	
    
	HRESULT hr = E_FAIL;

	// init return values
	*argCount = 0;	returnTypeStr[0] = NULL; functionParameters[0] = NULL;funName[0] = NULL;
	className[0] = NULL; *pdwMethodAttr=0;moduleName[0]=NULL;*spLines=NULL;*fileNames=NULL;
	*spOffsets=NULL;*spStartColumn=NULL; *spEndColumn=NULL;	spCount=0; swprintf( funName, L"UNMANAGED FRAME" );
	swprintf( className, L"" );

	///////////////

    if ( functionID == NULL ||m_pCorProfilerInfo2 ==NULL )
		{
			return E_POINTER;
		}	   	

	if (WANT_FUNCTION_NAME /*|| WANT_OBJECT_ALLOCATION_DATA*/ )
		{
			mdToken	funcToken=0;
			ClassID classId=0;
			ModuleID moduleID=0;
			CComPtr<IMetaDataImport> pMDImport=NULL;			
			mdTypeDef classToken = mdTypeDefNil;
            DWORD methodAttr = 0;
            PCCOR_SIGNATURE sigBlob = NULL;

			try
			{
				if (m_bIsTwoDotO)
                {
                    hr = m_pCorProfilerInfo2->GetFunctionInfo2( functionID,0,NULL,&moduleID,&funcToken,0,
                                                                 NULL,NULL);              
                }
				else
				{
					hr=m_pCorProfilerInfo2->GetFunctionInfo(functionID,NULL,&moduleID,&funcToken);
				}
				
				if(FAILED(hr) || funcToken==0 || moduleID==0)
				{
					return E_FAIL;
				}

				hr=m_pCorProfilerInfo2->GetModuleMetaData(moduleID,ofRead,IID_IMetaDataImport,(IUnknown**)&pMDImport);
				
			}
			catch(...)
			{
				hr=E_FAIL; 
			}
		    
			if(FAILED(hr) || funcToken==0 || pMDImport==NULL)
				{
					return E_FAIL;
				}

			hr = pMDImport->GetMethodProps( funcToken,
                                            &classToken,
                                            funName,
                                            MAX_LENGTH,
                                            0,
                                            &methodAttr,
                                            &sigBlob,
                                            NULL,
                                            NULL, 
                                            NULL );
			*pdwMethodAttr=methodAttr;

			if(FAILED(hr))
				{
					return hr;
				}
		
		if (m_bIsTwoDotO)
                {
                    hr = m_pCorProfilerInfo2->GetFunctionInfo2( functionID,0,&classId,&moduleID,&funcToken,0,
                                                                 NULL,NULL);
                    if (!SUCCEEDED(hr))
                        classId = 0;
                }
		else
				{
					hr = m_pCorProfilerInfo2->GetFunctionInfo(functionID,
                                                          &classId,
                                                         &moduleID,&funcToken);
				}

				if (SUCCEEDED(hr) && classId != 0)
					{
						 hr = GetNameFromClassID(classId, className);
						 if(className[0]=='\0')
							 {
								swprintf(className,L"UNKNOWN CLASS");
							 }
					}
				else if (classToken != mdTypeDefNil)
					{
						 ULONG classGenericArgCount = 0;
						hr = GetClassName(pMDImport, classToken, className, NULL, &classGenericArgCount);						
						if(className[0]=='\0')
							 {
								swprintf(className,L"UNKNOWN CLASS");
							 }
					}
					try
						{
						if(moduleID)
								{
									hr=m_pCorProfilerInfo2->GetModuleInfo( moduleID,NULL,MAX_LENGTH,NULL,moduleName,NULL);
									if(FAILED(hr) || !moduleName)
										swprintf( moduleName, L"UNKNOWN" );														
								}
						}
					catch(...)
						{
							swprintf( moduleName, L"UNKNOWN" );					
						}						


				if((WANT_FUNCTION_CODE_VIEW /*||WANT_OBJECT_ALLOCATION_DATA*/ )&& g_Slogger->m_debugModuleSet.find(moduleID)!=g_Slogger->m_debugModuleSet.end())
							{
								if(g_Slogger->binder==NULL)
										{
											try
											{
												hr=g_Slogger->binder.CoCreateInstance(L"CorSymBinder_SxS",NULL,CLSCTX_INPROC_SERVER);  												
												ASSERT(SUCCEEDED(hr));

											}catch(...){}
										}
								
									try
									{	
										if(g_Slogger->binder!=NULL)
										{											
											CComPtr<ISymUnmanagedReader> reader;											
											hr = binder->GetReaderForFile(pMDImport,
															moduleName,L".",&reader);
											if(reader.p)
											{
												CComPtr<ISymUnmanagedMethod> method;
												hr=reader->GetMethod(funcToken, &method);
												if(method.p)
												{	
													spCount=0;														
													//if(WANT_OBJECT_ALLOCATION_DATA)
													//	{
													//		spCount=1;//No more needed , only the first line of the allocating method
													//	}
													//else
														{
															method->GetSequencePointCount(&spCount);
														}
													
												if(spCount > 0)
															{
																	*spOffsets=new ULONG32[spCount];
																	ISymUnmanagedDocument**spDocuments=new ISymUnmanagedDocument*[spCount];
																	*spLines=new ULONG32[spCount];
																	*spStartColumn=new ULONG32[spCount];
																	*spEndColumn=new ULONG32[spCount];
																	*fileNames=new CString[spCount];

																	ULONG32 actualCount = 0;																																
																	hr = method->GetSequencePoints(spCount, &actualCount, *spOffsets, spDocuments, *spLines, *spStartColumn, NULL, *spEndColumn);
																
																	if(SUCCEEDED(hr))
																	{
																		WCHAR fileName[MAX_LENGTH];
																		for(ULONG32 x=0;x<spCount;x++)
																		{
																				fileName[0]=NULL;
																				ULONG32 length = 0;
																				hr=E_FAIL;
																				try
																					{
																						hr=spDocuments[x]->GetURL(MAX_LENGTH, &length, fileName);
																					}catch(...){} 
																				spDocuments[x]->Release();//Do not forget																				
																				spDocuments[x]=NULL;
																					(*fileNames)[x]=fileName;																				

																		}
																	}	

																	try
																	{
																		if(spDocuments)
																		{
																			bool IsValid=true;
																			#ifdef _DEBUG
																				IsValid=_CrtIsValidHeapPointer(spDocuments);
																			#endif		
																			if(IsValid)
																			{
																				delete [] spDocuments;
																			}																			
																		}
																	}  catch(...){}	
																	spDocuments=NULL;
																
															}
													 
													method.Release(); 
												}
												reader.Release(); 

											}											
										}
									}catch(...){}								
								
					}	
				
				if(WANT_FUNCTION_SIGNATURE)
					{
						 if(!m_bIsTwoDotO)
							 {
								ULONG callConv;
								char buffer[MAX_FUNCTION_LENGTH];	
								sigBlob += CorSigUncompressData( sigBlob, &callConv );
								if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
								{
										static WCHAR* callConvNames[8] = 
										{	
											L"", 
											L"unmanaged cdecl ", 
											L"unmanaged stdcall ",	
											L"unmanaged thiscall ",	
											L"unmanaged fastcall ",	
											L"vararg ",	 
											L"<error> "	 
											L"<error> "	 
										};	
										buffer[0] = '\0';
										if ( (callConv & 7) != 0 )
											sprintf( buffer, "%s ", callConvNames[callConv & 7]);
										
										sigBlob += CorSigUncompressData( sigBlob, argCount );

										sigBlob = ParseElementType( pMDImport, sigBlob, buffer );

										if ( buffer[0] == '\0' )
											sprintf( buffer, "void" );

										swprintf( returnTypeStr, L"%S",buffer );
														
										for ( ULONG i = 0;(SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount)));i++ )
												{
													buffer[0] = '\0';

													sigBlob = ParseElementType( pMDImport, sigBlob, buffer );									
													if ( i == 0 )
														swprintf( functionParameters, L"%S", buffer );

													else if ( sigBlob != NULL )
														swprintf( functionParameters, L"%s, %S", functionParameters, buffer );
													
													else
														hr = E_FAIL;
												}							    								
									}
								else
								{
									buffer[0] = '\0';
									sigBlob = ParseElementType( pMDImport, sigBlob, buffer );
									swprintf( returnTypeStr, L"%s %S",returnTypeStr, buffer );
								}
							 }
							else
							{
								
								ULONG callConv;
								char buffer[MAX_FUNCTION_LENGTH];
                
								sigBlob += CorSigUncompressData( sigBlob, &callConv );
								if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
									{
										 static char* callConvNames[8] = 
										{   
											"", 
											"unmanaged cdecl ", 
											"unmanaged stdcall ",  
											"unmanaged thiscall ", 
											"unmanaged fastcall ", 
											"vararg ",  
											"<error> "  
											"<error> "  
										};  
											buffer[0] = '\0';
											if ( (callConv & 7) != 0 )
												sprintf_s( buffer, ARRAY_LEN(buffer), "%s ", callConvNames[callConv & 7]);   
						                    
											ULONG genericArgCount = 0;
											ClassID *methodTypeArgs = NULL;
											UINT32 methodTypeArgCount = 0;
											ClassID *classTypeArgs = NULL;
											ULONG32 classTypeArgCount = 0;

											if ((callConv & IMAGE_CEE_CS_CALLCONV_GENERIC) != 0)
											{
											sigBlob += CorSigUncompressData( sigBlob, &genericArgCount );
											}
                    
											methodTypeArgs = (ClassID *)_alloca(genericArgCount*sizeof(methodTypeArgs[0]));

											hr = m_pCorProfilerInfo2->GetFunctionInfo2( functionID,
																					0,
																					&classId,
																					NULL,
																					NULL,
																					genericArgCount,
																					&methodTypeArgCount,
																					methodTypeArgs);

                        
											if (!SUCCEEDED(hr))
												methodTypeArgs = NULL;
											else
											{
												hr = m_pCorProfilerInfo2->GetClassIDInfo2(classId,
																					NULL,
																					NULL,
																					NULL,
																					0,
																					&classTypeArgCount,
																					NULL);
											}
                        
											if (SUCCEEDED(hr) && classTypeArgCount > 0)
											{
												classTypeArgs = (ClassID *)_alloca(classTypeArgCount*sizeof(classTypeArgs[0]));

												hr = m_pCorProfilerInfo2->GetClassIDInfo2(classId,
																					NULL,
																					NULL,
																					NULL,
																					classTypeArgCount,
																					&classTypeArgCount,
																					classTypeArgs);
											}
											if (!SUCCEEDED(hr))
												classTypeArgs = NULL;
											
											sigBlob += CorSigUncompressData( sigBlob, argCount );

                   
											sigBlob = ParseElementType( pMDImport, sigBlob, classTypeArgs, methodTypeArgs, buffer, ARRAY_LEN(buffer) );
	                
											if (genericArgCount != 0 && genericArgCount < 65536)
											{
												StrAppend(buffer, " <", ARRAY_LEN(buffer));
												for (ULONG i = 0; i < genericArgCount; i++)
												{
													if (i != 0)
														StrAppend(buffer, ",", ARRAY_LEN(buffer));
													AppendTypeArgName(i, classTypeArgs, methodTypeArgs, TRUE, buffer, ARRAY_LEN(buffer));
												}
												StrAppend(buffer, ">", ARRAY_LEN(buffer));
											}
                   
											if ( buffer[0] == '\0' )
												sprintf_s( buffer, ARRAY_LEN(buffer), "void" );

											_snwprintf_s( returnTypeStr, MAX_FUNCTION_LENGTH, MAX_FUNCTION_LENGTH-1, L"%S",buffer );
                
                                              
											for ( ULONG i = 0;(SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount)));i++ )
											{
												buffer[0] = '\0';

												sigBlob = ParseElementType( pMDImport, sigBlob, classTypeArgs, methodTypeArgs, buffer, ARRAY_LEN(buffer)-1 );
												buffer[ARRAY_LEN(buffer)-1] = '\0';

												if ( i == 0 ) 
												{
													_snwprintf_s( functionParameters, MAX_FUNCTION_LENGTH, MAX_FUNCTION_LENGTH-1, L"%S", buffer );
												}
												else if ( sigBlob != NULL )
												{
													_snwprintf_s( functionParameters, MAX_FUNCTION_LENGTH, MAX_FUNCTION_LENGTH-1, L"%s,%S", functionParameters, buffer );
												}
						                    
												else
													hr = E_FAIL;
											}
									}
									else
									{ 
										buffer[0] = '\0';
										sigBlob = ParseElementType( pMDImport, sigBlob, NULL, NULL, buffer, ARRAY_LEN(buffer)-1 );
										buffer[ARRAY_LEN(buffer)-1] = L'\0';
										_snwprintf_s( returnTypeStr, MAX_FUNCTION_LENGTH, MAX_FUNCTION_LENGTH-1, L"%s %S",returnTypeStr, buffer );
									}
														
							}
						
					}
		}
				   return hr;
}


PCCOR_SIGNATURE CSlogger::ParseElementType( IMetaDataImport *pMDImport,
                                           PCCOR_SIGNATURE signature,
                                           ClassID *classTypeArgs,
                                           ClassID *methodTypeArgs,
                                           __out_ecount(cchBuffer) char *buffer,
                                           size_t cchBuffer)
{   
    switch ( *signature++ ) 
    {   
        case ELEMENT_TYPE_VOID:
            StrAppend( buffer, "void", cchBuffer);   
            break;                  
        
        
        case ELEMENT_TYPE_BOOLEAN:  
            StrAppend( buffer, "bool", cchBuffer);   
            break;  
        
        
        case ELEMENT_TYPE_CHAR:
            StrAppend( buffer, "wchar", cchBuffer);  
            break;      
                    
        
        case ELEMENT_TYPE_I1:
            StrAppend( buffer, "int8", cchBuffer );   
            break;      
        
        
        case ELEMENT_TYPE_U1:
            StrAppend( buffer, "unsigned int8", cchBuffer );  
            break;      
        
        
        case ELEMENT_TYPE_I2:
            StrAppend( buffer, "int16", cchBuffer );  
            break;      
        
        
        case ELEMENT_TYPE_U2:
            StrAppend( buffer, "unsigned int16", cchBuffer ); 
            break;          
        
        
        case ELEMENT_TYPE_I4:
            StrAppend( buffer, "int32", cchBuffer );  
            break;
            
        
        case ELEMENT_TYPE_U4:
            StrAppend( buffer, "unsigned int32", cchBuffer ); 
            break;      
        
        
        case ELEMENT_TYPE_I8:
            StrAppend( buffer, "int64", cchBuffer );  
            break;      
        
        
        case ELEMENT_TYPE_U8:
            StrAppend( buffer, "unsigned int64", cchBuffer ); 
            break;      
        
        
        case ELEMENT_TYPE_R4:
            StrAppend( buffer, "float32", cchBuffer );    
            break;          
        
        
        case ELEMENT_TYPE_R8:
            StrAppend( buffer, "float64", cchBuffer );    
            break;      
        
        
        case ELEMENT_TYPE_U:
            StrAppend( buffer, "unsigned int_ptr", cchBuffer );   
            break;       
        
        
        case ELEMENT_TYPE_I:
            StrAppend( buffer, "int_ptr", cchBuffer );    
            break;            
        
        
        case ELEMENT_TYPE_OBJECT:
            StrAppend( buffer, "Object", cchBuffer ); 
            break;       
        
        
        case ELEMENT_TYPE_STRING:
            StrAppend( buffer, "String", cchBuffer ); 
            break;       
        
        
        case ELEMENT_TYPE_TYPEDBYREF:
            StrAppend( buffer, "refany", cchBuffer ); 
            break;                     

        case ELEMENT_TYPE_CLASS:    
        case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_CMOD_OPT:
            {   
                mdToken token;  
                char classname[MAX_LENGTH];


                classname[0] = '\0';
                signature += CorSigUncompressToken( signature, &token ); 
                if ( TypeFromToken( token ) != mdtTypeRef )
                {
                    HRESULT hr;
                    WCHAR zName[MAX_LENGTH];
                    
                    
                    hr = pMDImport->GetTypeDefProps( token, 
                                                     zName,
                                                     MAX_LENGTH,
                                                     NULL,
                                                     NULL,
                                                     NULL );
                    if ( SUCCEEDED( hr ) )
                    {
                        size_t convertedChars;
                        wcstombs_s( &convertedChars, classname, ARRAY_LEN(classname), zName, ARRAY_LEN(zName) );
                    }
                }
                    
                StrAppend( buffer, classname, cchBuffer );        
            }
            break;  
        
        
        case ELEMENT_TYPE_SZARRAY:   
            signature = ParseElementType( pMDImport, signature, classTypeArgs, methodTypeArgs, buffer, cchBuffer ); 
            StrAppend( buffer, "[]", cchBuffer );
            break;      
        
        
        case ELEMENT_TYPE_ARRAY:    
            {   
                ULONG rank;
                

                signature = ParseElementType( pMDImport, signature, classTypeArgs, methodTypeArgs, buffer, cchBuffer );                 
                rank = CorSigUncompressData( signature );  
                
                // The second condition is to guard against overflow bugs & shut up PREFAST
                if ( rank == 0 || rank >= 65536 ) 
                    StrAppend( buffer, "[?]", cchBuffer );

                else 
                {
                    ULONG *lower;   
                    ULONG *sizes;   
                    ULONG numsizes; 
                    ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
                    
                                         
                    lower = (ULONG *)_alloca( arraysize );                                                        
                    memset( lower, 0, arraysize ); 
                    sizes = &lower[rank];

                    numsizes = CorSigUncompressData( signature );   
                    if ( numsizes <= rank )
                    {
                        ULONG numlower;
                        ULONG i;                        
                        
                        for (i = 0; i < numsizes; i++ )  
                            sizes[i] = CorSigUncompressData( signature );   
                        
                        
                        numlower = CorSigUncompressData( signature );   
                        if ( numlower <= rank )
                        {
                            for ( i = 0; i < numlower; i++) 
                                lower[i] = CorSigUncompressData( signature ); 
                            
                            
                            StrAppend( buffer, "[", cchBuffer );  
                            for ( i = 0; i < rank; i++ )    
                            {   
                                if ( (sizes[i] != 0) && (lower[i] != 0) )   
                                {   
                                    char sizeBuffer[100];
                                    if ( lower[i] == 0 )
                                        sprintf_s ( sizeBuffer, ARRAY_LEN(sizeBuffer), "%d", sizes[i] ); 
                                    else    
                                    {   
                                        sprintf_s( sizeBuffer, ARRAY_LEN(sizeBuffer), "%d...", lower[i] );  
                                        
                                        if ( sizes[i] != 0 )    
                                            sprintf_s( sizeBuffer, ARRAY_LEN(sizeBuffer), "%d...%d", lower[i], (lower[i] + sizes[i] + 1) ); 
                                    }   
                                    StrAppend( buffer, sizeBuffer, cchBuffer );    
                                }
                                    
                                if ( i < (rank - 1) ) 
                                    StrAppend( buffer, ",", cchBuffer );  
                            }   
                            
                            StrAppend( buffer, "]", cchBuffer );  
                        }                       
                    }
                }
            } 
            break;  

        
        case ELEMENT_TYPE_PINNED:
            signature = ParseElementType( pMDImport, signature, classTypeArgs, methodTypeArgs, buffer, cchBuffer ); 
            StrAppend( buffer, "pinned", cchBuffer ); 
            break;  
         
        
        case ELEMENT_TYPE_PTR:   
            signature = ParseElementType( pMDImport, signature, classTypeArgs, methodTypeArgs, buffer, cchBuffer ); 
            StrAppend( buffer, "*", cchBuffer );  
            break;   
        
        
        case ELEMENT_TYPE_BYREF:   
            signature = ParseElementType( pMDImport, signature, classTypeArgs, methodTypeArgs, buffer, cchBuffer ); 
            StrAppend( buffer, "&", cchBuffer );  
            break;              

		case ELEMENT_TYPE_VAR:
            AppendTypeArgName(CorSigUncompressData(signature), classTypeArgs, methodTypeArgs, FALSE, buffer, cchBuffer);
            break;

        case ELEMENT_TYPE_MVAR:
            AppendTypeArgName(CorSigUncompressData(signature), classTypeArgs, methodTypeArgs, TRUE, buffer, cchBuffer);
            break;

        default:    
        case ELEMENT_TYPE_END:  
        case ELEMENT_TYPE_SENTINEL: 
            StrAppend( buffer, "<UNKNOWN>", cchBuffer );  
            break;                                                              
                            
    } // switch

    return signature;

} 

        STDMETHOD( ObjectAllocated )( ObjectID objectID, ClassID classID );		

        STDMETHOD( ObjectsAllocatedByClass )( ULONG classCount, ClassID classIDs[], ULONG objects[] )
		{
			return S_OK;
		}

		private:

			void PopulateReferencedObjects(ObjectID objectID,ULONG cObjRefs,ObjectID objectRefIDs[])
			{	
				USES_CONVERSION;	
				try
				{
					if(CAN_PROFILE_FOR_OBJECTS  && IS_PROFILEDPROCESSFOROBJECTS) 
					{
						if(!WANT_REFERENCED_OBJECTS )
						return;											
						
						for(ULONG x=0;x<cObjRefs;x++)
						{	
								WCHAR className[MAX_LENGTH];
								wcscpy(className,L"UNKNOWN CLASS");//init
								//if(WANT_REFERENCED_OBJECTS_NAME)
								{
									ClassID classID;	
									HRESULT hr=S_OK;
									if(!m_bIsTwoDotO)
										{
											hr=m_pCorProfilerInfo2->GetClassFromObject(objectRefIDs[x],&classID);																										
										}
									else
										{
											hr=S_OK;
											classID=m_mapObjectIDToClassID[objectRefIDs[x]];
										}
									if(SUCCEEDED(hr) && classID)													
									{						
										GetNameFromClassID(classID,className);	
										if( className == NULL || (wcscmp(className,L"") == 0) )
										{
											lstrcpyW(className,L"UNKNOWN CLASS");
										}
									}

								}

								ULONG objSize=0,objCount=0;
								//if(WANT_REFERENCED_OBJECTS_SIZE)
								{
									m_pCorProfilerInfo2->GetObjectSize(objectRefIDs[x],&objSize); 					
								}							
								
									if(m_mapReferencedObjects.find(objectID)==m_mapReferencedObjects.end())
									{
										m_mapReferencedObjects[objectID][objectRefIDs[x]]=NULL;
										m_mapReferencedObjects[objectID][objectRefIDs[x]]=new OBJECTDATA(); 

										//if(WANT_REFERENCED_OBJECTS_COUNT) 
										//m_mapReferencedObjects[objectID][objectRefIDs[x]]->objCount=1;//1 by default

										//if(WANT_REFERENCED_OBJECTS_SIZE) 
										m_mapReferencedObjects[objectID][objectRefIDs[x]]->objSize=objSize;

										//if(WANT_REFERENCED_OBJECTS_NAME)
										m_mapReferencedObjects[objectID][objectRefIDs[x]]->objName=W2A(className);//W2A IS ERROR SOME
											
									}								
									else
									{
										if(m_mapReferencedObjects[objectID].find(objectRefIDs[x])==m_mapReferencedObjects[objectID].end())
										{
											m_mapReferencedObjects[objectID][objectRefIDs[x]]=NULL;
											m_mapReferencedObjects[objectID][objectRefIDs[x]]=new OBJECTDATA(); 

											//if(WANT_REFERENCED_OBJECTS_COUNT)
											//m_mapReferencedObjects[objectID][objectRefIDs[x]]->objCount=1;//1 is by default

											//if(WANT_REFERENCED_OBJECTS_SIZE) 
											m_mapReferencedObjects[objectID][objectRefIDs[x]]->objSize=objSize;																				

											//if(WANT_REFERENCED_OBJECTS_NAME)
											m_mapReferencedObjects[objectID][objectRefIDs[x]]->objName=W2A(className);//W2A IS ERROR SOME

										}
										else
										{
											//if(WANT_REFERENCED_OBJECTS_COUNT) 
											{
												if(m_mapReferencedObjects[objectID][objectRefIDs[x]])
												{
													m_mapReferencedObjects[objectID][objectRefIDs[x]]->objCount=m_mapReferencedObjects[objectID][objectRefIDs[x]]->objCount+1;
												}
											}

											//if(WANT_REFERENCED_OBJECTS_SIZE) 
											{
												/*if(m_mapReferencedObjects[objectID][objectRefIDs[x]])
												{
													m_mapReferencedObjects[objectID][objectRefIDs[x]]->objSize=m_mapReferencedObjects[objectID][objectRefIDs[x]]->objSize+objSize;
												}*/
											}

										}
									}	
									
						}
						
					}
				}
				catch(...){}

			}
			


		public:


        STDMETHOD( ObjectReferences )( ObjectID objectID,
                                       ClassID classID,
                                       ULONG cObjectRefs,
                                       ObjectID objectRefIDs[] )
		{
            try
			{				
				if(CAN_PROFILE_FOR_OBJECTS  && IS_PROFILEDPROCESSFOROBJECTS) 
					{					
					if(WANT_OBJECT_NAME_ONLY)
					{
							USES_CONVERSION;
							WCHAR className[MAX_LENGTH];
							className[0]=NULL;
							CString cstrClassName;
							//ZeroMemory((void*)className,sizeof(className));

							GetNameFromClassID(classID,className) ;	
							
							ULONG objSize=0;

							if( className == NULL || wcscmp(className,L"") == 0 )
							{
								lstrcpyW(className,L"UNKNOWN CLASS");
								cstrClassName=W2A(className);	
								if(g_Slogger->m_objectClassFilter.size()>0)
									goto C;
								else
									goto B;

							}
							////Class Filter/////////////	

												cstrClassName=W2A(className);
										
												if(g_Slogger->m_objectClassFilter.size()>0 && cstrClassName.GetLength()>0 )  
												{    								
													if(g_Slogger->m_bObjectPassthrough )
													{
														for(ULONG x=0;x<g_Slogger->m_objectClassFilter.size();x++)
														{	
															if(g_Slogger->m_objectClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
																continue;

															char* classFilter=g_Slogger->m_objectClassFilter.c.at(x).GetBuffer();

															if(strncmp(classFilter,cstrClassName.GetBuffer(),strlen(classFilter) )==0) 
																goto B;//Move On
														}

														//failed .. the Class is not desired
														goto C;//get next function ID									
															
													}
													else
													{
														///block these 
														for(ULONG x=0;x<g_Slogger->m_objectClassFilter.size();x++)
														{	
															if(g_Slogger->m_objectClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
																goto B;//Move On

															if(strncmp(g_Slogger->m_objectClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_objectClassFilter.c.at(x).GetLength() )==0)
																goto C;

																
														}
																//Passed ..Allow to move on
																goto B;

													}

												}

												//////////////////////
B:											try{cstrClassName.Empty();}catch(...){}			


							if(WANT_OBJECT_SIZE)
							{
								m_pCorProfilerInfo2->GetObjectSize(objectID,&objSize);
							}
//CHANGE
//							if(m_mapLiveObjects.find(objectID)== m_mapLiveObjects.end())
							{
								m_mapLiveObjects[objectID]=NULL;
								m_mapLiveObjects[objectID]=new OBJECTDATA();

								//if(WANT_OBJECT_COUNT)
								//{									
								//	m_mapLiveObjects[objectID]->objCount=1;
								//	//m_ui64ObjectCount +=1;
								//}

								if(WANT_OBJECT_SIZE)
								{
									m_mapLiveObjects[objectID]->objSize=objSize;
									//m_ui64ObjectsSize+=objSize;
								}

								if(m_mapLiveObjects[objectID]->objName=="")
								m_mapLiveObjects[objectID]->objName=W2A(className);
								
							}
							
							if(WANT_REFERENCED_OBJECTS)//minimum requirement.
							{
								PopulateReferencedObjects(objectID,cObjectRefs,objectRefIDs);
							}
					}
				}
			}
			catch(...){	}			
C:			return S_OK;//must return S_OK to continue receiving callbacks
			//Also include RootRefrences

		}
    
		//Note RootReferences may be called multiple times for a
		// particular GC
        STDMETHOD( RootReferences )( ULONG cRootRefs, ObjectID rootRefIDs[] )
		{
			USES_CONVERSION ;
			 try
			{
				if(CAN_PROFILE_FOR_OBJECTS  && IS_PROFILEDPROCESSFOROBJECTS) 
				{					
					if(!WANT_OBJECT_NAME_ONLY)
						return S_OK;

					HRESULT hr=E_FAIL;						
					for(ULONG x=0;x<cRootRefs;x++)
					{
						try
						{	
							ClassID classID=NULL;
							WCHAR className[MAX_LENGTH];
							CString cstrClassName="UNKNOWN CLASS";
							ZeroMemory((void*)className,sizeof(className));
							hr=m_pCorProfilerInfo2->GetClassFromObject(rootRefIDs[x],&classID);
							if(m_bIsTwoDotO)
								{
								if(classID==NULL || FAILED(hr) )
									{	   
										hr=S_OK;
										classID=m_mapObjectIDToClassID[rootRefIDs[x]]; 
									}
								}
							if(SUCCEEDED(hr) && classID)
							{
								if(GetNameFromClassID(classID,className )==S_OK)
								{

									if( className == NULL || wcscmp(className,L"") == 0 )
										{
												lstrcpyW(className,L"UNKNOWN CLASS");
												if(g_Slogger->m_objectClassFilter.size()>0)
													goto C;
												else
													goto B;												
										}
										/////////////Class Filter///////
								
										cstrClassName=W2A(className);//V.Imp
									if(g_Slogger->m_objectClassFilter.size()>0 && cstrClassName.GetLength()>0 )  
									{    								
										if(g_Slogger->m_bObjectPassthrough )
										{
											for(ULONG x=0;x<g_Slogger->m_objectClassFilter.size();x++)
											{	
												if(g_Slogger->m_objectClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
													continue;

												char * classFilter=g_Slogger->m_objectClassFilter.c.at(x).GetBuffer();

												if(strncmp(classFilter,cstrClassName.GetBuffer(),strlen(classFilter) )==0) 
													goto B;//Move On
											}

											//failed .. the Class is not desired
											goto C;//get next function ID									
												
										}
										else
										{
											///block these 
											for(ULONG x=0;x<g_Slogger->m_objectClassFilter.size();x++)
											{	
												if(g_Slogger->m_objectClassFilter.c.at(x).GetLength() > cstrClassName.GetLength())
													goto B;//Move On

												if(strncmp(g_Slogger->m_objectClassFilter.c.at(x).GetBuffer(),cstrClassName.GetBuffer(),g_Slogger->m_objectClassFilter.c.at(x).GetLength() )==0)
													goto C;

													
											}
													//Passed ..Allow to move on
													goto B;

										}

									}

									//////////////////////
		B:							try{cstrClassName.Empty();}catch(...){}

									/////////////////
									ULONG objSize=0;
									if(WANT_OBJECT_SIZE) 
									m_pCorProfilerInfo2->GetObjectSize(rootRefIDs[x],&objSize); 									

										if(m_mapRootObjects.find(rootRefIDs[x])== m_mapRootObjects.end())
											{
												m_mapRootObjects[rootRefIDs[x]]=NULL;
												m_mapRootObjects[rootRefIDs[x]]=new OBJECTDATA();

												if(WANT_OBJECT_COUNT) 
												{
													m_mapRootObjects[rootRefIDs[x]]->objCount=1;
												}				

												if(WANT_OBJECT_SIZE) 
												{
													m_mapRootObjects[rootRefIDs[x]]->objSize=objSize;
												}
												else
												{
													m_mapRootObjects[rootRefIDs[x]]->objSize=0;
												}				
												
												m_mapRootObjects[rootRefIDs[x]]->objName = W2A(className);//needed always

											}
											/*
										else
										{ 
											if(WANT_OBJECT_COUNT) 
											{
												if(m_mapRootObjects[rootRefIDs[x]])
												m_mapRootObjects[rootRefIDs[x]]->objCount=m_mapRootObjects[rootRefIDs[x]]->objCount+1;
											}											

											if(WANT_OBJECT_SIZE) 
											{
												if(m_mapRootObjects[rootRefIDs[x]])
												m_mapRootObjects[rootRefIDs[x]]->objSize=m_mapRootObjects[rootRefIDs[x]]->objSize+objSize;
											}

										}
										*/

								}
							}
                            
						}
						catch(...){}

C:						continue;
					}				
					
				}
			}
			catch(...){}			
			return S_OK;
		}
            
		// EXCEPTION EVENTS
        // Exception creation
        STDMETHOD( ExceptionThrown )( ObjectID thrownObjectID );
		
        // Search phase
        STDMETHOD( ExceptionSearchFunctionEnter )( FunctionID functionID );
		
        STDMETHOD( ExceptionSearchFunctionLeave )()
		{
			return S_OK;
		}
        STDMETHOD( ExceptionSearchFilterEnter )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionSearchFilterLeave )()
		{
			return S_OK;
		}
        STDMETHOD( ExceptionSearchCatcherFound )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionCLRCatcherFound )()
		{
			return S_OK;
		}
			

        STDMETHOD( ExceptionCLRCatcherExecute )()
		{
			return S_OK;
		}
        STDMETHOD( ExceptionOSHandlerEnter )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionOSHandlerLeave )( FunctionID functionID )
		{
			return S_OK;
		}
        // Unwind phase
        STDMETHOD( ExceptionUnwindFunctionEnter )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionUnwindFunctionLeave )();
		
        STDMETHOD( ExceptionUnwindFinallyEnter )( FunctionID functionID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionUnwindFinallyLeave )()
		{
			return S_OK;
		}
        STDMETHOD( ExceptionCatcherEnter )( FunctionID functionID, ObjectID objectID )
		{
			return S_OK;
		}
        STDMETHOD( ExceptionCatcherLeave )()
		{
			return S_OK;
		}



		// COM CLASSIC VTable
        STDMETHOD( COMClassicVTableCreated )( ClassID wrappedClassID,
                                              REFGUID implementedIID,
                                              void *pVTable,
                                              ULONG cSlots )											  
		{
			return S_OK;
		}
        STDMETHOD( COMClassicVTableDestroyed )( ClassID wrappedClassID,
                                                REFGUID implementedIID,
                                                void *pVTable )
		{
			return S_OK;
		}
		

		////Other public Members
		CComPtr<ICorProfilerInfo2> m_pCorProfilerInfo2;
		long m_ProfileePID;
		bool m_bSuspendOnStart;
		HANDLE m_hSuspensionMutexFlag;

		public:
		DWORD m_dwEventMask;
		//queue <HANDLE> m_qCLRHandle;	

		void Enter(FunctionID funcID);
		void Leave(FunctionID funcID);
		void Tailcall(FunctionID funcID);

		CString GetMinimumFunctionString(const FunctionID functionID);
		void SetTimeResolutionFilter(void);
		void ResolveFilters(void);		
		void GetLastSessionName(void);
		void GetLastSessionPath(void);
		void CanRunGCBeforeOC(void);
private:
	void IsProcessSuspended(void);
};


class StackEntryInfo
{
public: 
	StackEntryInfo( FunctionInfo* pFunctionInfo )
	{
		if(IS_PROFILEDPROCESSFOROBJECTS && WANT_OBJECT_ALLOCATION_DATA)
		{
			this->llCycleStart = 0;			
		}
		else
		{
			this->llCycleStart = rdtsc();//call rdtsc() as late as possible when pushing
		}
		this->pFunctionInfo = pFunctionInfo;				
	}

	StackEntryInfo( const StackEntryInfo& rhs )
	{
		pFunctionInfo = rhs.pFunctionInfo;		
		llCycleStart = rhs.llCycleStart;
	}

	~StackEntryInfo()
	{}
  
  UINT64 llCycleStart;
  FunctionInfo* pFunctionInfo;
};
//void SubmitError(char* message)
//{
//	if ( message == NULL )     
//		message = "#########CRITICAL ERROR###########";      
//  
//    TEXT_OUTLN( message );
//	if(g_Slogger->m_pCorProfilerInfo2 && g_Slogger)
//	{
//		g_Slogger->m_pCorProfilerInfo2->SetEventMask (g_Slogger->m_dwEventMask=COR_PRF_MONITOR_NONE );
//	}
//
//}


void __stdcall EnterStub( FunctionID functionID )
{
    g_Slogger->Enter( functionID );
    
} // EnterStub


void __stdcall LeaveStub( FunctionID functionID )
{
     g_Slogger->Leave( functionID );
    
} // LeaveStub

void __stdcall TailcallStub( FunctionID functionID )
{
    g_Slogger->Tailcall( functionID );
    
} // TailcallStub

void __declspec( naked ) EnterNaked2(FunctionID funcId, 
                                     UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO func, 
                                     COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call EnterStub
        pop edx
        pop ecx
        pop eax
        ret 16
    }
} 

void __declspec( naked ) LeaveNaked2(FunctionID funcId, 
                                     UINT_PTR clientData, 
                                     COR_PRF_FRAME_INFO func, 
                                     COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange)
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call LeaveStub
        pop edx
        pop ecx
        pop eax
        ret 16
    }
} 

void __declspec( naked ) EnterNaked()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call EnterStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }
    
} 

void __declspec( naked ) TailcallNaked2(FunctionID funcId, 
                                        UINT_PTR clientData, 
                                        COR_PRF_FRAME_INFO func)
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call TailcallStub
        pop edx
        pop ecx
        pop eax
        ret 12
    }
} 

void __declspec( naked ) LeaveNaked()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call LeaveStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }
    
} 
 
void __declspec( naked ) TailcallNaked()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call TailcallStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }
    
} // TailcallNaked


#ifdef WIN32
  __declspec(naked) UINT64 __fastcall getCPU_CYCLES()
  {	 
	 		__asm
			{
				rdtsc;
				ret;
			}    

  }
#else
 __inline__ unsigned UINT64 int getCPU_CYCLES(void)
  {	  
		unsigned long long int x;
		__asm__ volatile (".byte 0x0f, 0x31" : "=A" (x));
		return x;
		//CPUID: __asm __volatile(".byte 0x0f,0xa2");
	
  }
#endif


UINT64 getHRTC()
{
	FILETIME fileTime;
	ZeroMemory(&fileTime,sizeof(FILETIME));

	GetSystemTimeAsFileTime(&fileTime);	
	ULARGE_INTEGER  i64;
	
	i64.HighPart =fileTime.dwHighDateTime ;
	i64.LowPart =fileTime.dwLowDateTime ;
    return i64.QuadPart ;
}

UINT64 rdtsc()
{///can also return 0 here if WANT_OBJECT_ALLOCATION
	if(WANT_CPU_CYCLES)
		return getCPU_CYCLES();
	else if(WANT_LOW_RESOLUTION)
		return timeGetTime();//GetTickCount();
	else if (WANT_HIGH_RESOLUTION)	
		return getHRTC();
	else	
		return getCPU_CYCLES();//default
	
}



UINT BeginTimer()
{
	const UINT TARGET_RESOLUTION = 1;         // 1-millisecond target resolution

	TIMECAPS tc;
	UINT     wTimerRes;

	if (timeGetDevCaps(&tc, sizeof(TIMECAPS)) != TIMERR_NOERROR) 
	{
		return 0;
	}

	wTimerRes = min(max(tc.wPeriodMin, TARGET_RESOLUTION), tc.wPeriodMax);
	timeBeginPeriod(wTimerRes);

	return wTimerRes;
}

void EndTimer(UINT wTimerRes)
{
	if (wTimerRes != 0)
		timeEndPeriod(wTimerRes);
}



CRITICAL_SECTION CS_WFILE;

void LogFile(const char *szLine)
{	
	if ( (g_LogFile != NULL) && (szLine) )
	{		
		fputs( szLine, g_LogFile );	
		fputs( "\n", g_LogFile );		
		

	}	

}

void LogFileSingleLine(const char *szLine)
{	
//	CLock lock(CS_WFILE); 	
	if ( (g_LogFile != NULL) && (szLine) )
	{		
		fputs( szLine, g_LogFile );	

	}
}

CRITICAL_SECTION g_csUpdate;

void ClearFunctionNames()
{
	if(!g_Slogger)
		return;
	
				try
				{
					map<FunctionID,CString>::iterator it=g_Slogger->m_mapFuncIDToFuncName.begin();
					while(it!=g_Slogger->m_mapFuncIDToFuncName.end())
					{
						(it->second).Empty();  
						it++;
					}			
				}
				catch(...){}		
				g_Slogger->m_mapFuncIDToFuncName.clear(); 

}

void ClearFunctionCache(bool bClearFunctionNames)
{
	if(!g_Slogger )
		return;

			for ( map< ThreadID, ThreadInfo* >::iterator it = g_Slogger->_mThreadInfo.begin(); it != g_Slogger->_mThreadInfo.end(); it++ )
			{
				for ( map< FunctionID, FunctionInfo* >::iterator funcIT = it->second->_mFunctionInfo.begin() ; funcIT != it->second->_mFunctionInfo.end(); funcIT++ )
				{
					if(funcIT->second->_mCalleeInfo[funcIT->first]) 
					{
						try
						{
							delete funcIT->second->_mCalleeInfo[funcIT->first] ;							

						}	
						catch(...)
						{
							//char buffer[32];
							//fputs(ltoa(HRESULT_FROM_WIN32(GetLastError()),buffer,10),file); 
							
						}
						funcIT->second->_mCalleeInfo[funcIT->first]=NULL;
					}
					if(it->second->_mFunctionInfo[funcIT->first])
					{
						try
						{
							delete it->second->_mFunctionInfo[funcIT->first];							
						}
						catch(...)
						{
							/*char buffer[32];
							fputs(ltoa(HRESULT_FROM_WIN32(GetLastError()),buffer,10),file); */
						}
						it->second->_mFunctionInfo[funcIT->first]=NULL;
					}
				
				}
					it->second->_mFunctionInfo.clear();		
					///////////////////////////////////

					if(g_Slogger->_mThreadInfo[it->first])
					{
						try
						{
							delete g_Slogger->_mThreadInfo[it->first];							
						}
						catch(...)
						{
							/*char buffer[32];
							fputs(ltoa(HRESULT_FROM_WIN32(GetLastError()),buffer,10),file); */
						}
						g_Slogger->_mThreadInfo[it->first]=NULL;
					}
							    							
			}
//			fclose(file);
			
			g_Slogger->_mThreadInfo.clear();  
		//clear function ID to Function Name map. Good to Clear b'coz FunctionIDs can expire on module unloads
			if(bClearFunctionNames)
			{
				ClearFunctionNames(); 

			}
			
}


STDAPI CFC()//ClearFunctionCache
{
	CLock lock(g_csFunction);
	CLockObjectDump lockDump;
	if( g_Slogger)
		{
		if(!WANT_FUNCTION_NAME) 
					return S_OK;

		if(CAN_PROFILE_FOR_FUNCTIONS)
			{
				SetLastError(HRESULTTOWIN32(E_PENDING));  
				return E_PENDING ;
			}//FUNCTION CAPTURING IS ON
		//STOP IT FIRST TO PROCEED 			
			ClearFunctionCache(true);

		}

	return S_OK;

}

	void ClearRootObjCache()
	{
			//////////////////////////
		if(!g_Slogger)	
			return;

					map<UINT_PTR,POBJECTDATA>::iterator RootObjIterator=g_Slogger->m_mapRootObjects.begin();
					while(RootObjIterator!=g_Slogger->m_mapRootObjects.end()) 
				{
					if(RootObjIterator->second)
					{
						try
						{
							RootObjIterator->second->objName.Empty(); 
							delete RootObjIterator->second;
							RootObjIterator->second=NULL;

						}
						catch(...)
						{
							//TEXT_OUTLN("[Root delete ERROR]");
						}
					}
					RootObjIterator++;
				}
				g_Slogger->m_mapRootObjects.clear();

	}

	void ClearLiveObjCache()
	{
		if(!g_Slogger)	
			return;

		map<UINT_PTR,POBJECTDATA>::iterator LiveObjIterator= g_Slogger->m_mapLiveObjects.begin();
					while(LiveObjIterator!=g_Slogger->m_mapLiveObjects.end()) 
				{
					if(LiveObjIterator->second)
					{
						try
						{	
							LiveObjIterator->second->objName.Empty();  
							delete LiveObjIterator->second;
							LiveObjIterator->second=NULL;
						}
						catch(...)
						{
							//TEXT_OUTLN("[Live Object delete ERROR]");
						}
					}
					LiveObjIterator++;

				}
					g_Slogger->m_mapLiveObjects.clear();


	}	

	void ClearRefObjCache()
	{
		if(!g_Slogger)	
			return;

		map<UINT_PTR, map<UINT_PTR,POBJECTDATA> >::iterator Iterator=g_Slogger->m_mapReferencedObjects.begin();
				
				while(Iterator != g_Slogger->m_mapReferencedObjects.end())
				{					
					map<UINT_PTR,POBJECTDATA>::iterator refIterator=Iterator->second.begin();
					while(refIterator!=Iterator->second.end())
					{   
						if(refIterator->second)
						{
							try
							{
								refIterator->second->objName.Empty();  
                                delete refIterator->second;
								refIterator->second=NULL;
							}
							catch(...)
							{
//								TEXT_OUTLN("[Ref Object delete ERROR]");
							}
						}
						refIterator++;
					}
					Iterator->second.clear();
					Iterator++;
				}
				g_Slogger->m_mapReferencedObjects.clear();  
	}




	__forceinline bool IsBPX(void *address)
	{
	__asm {
		mov esi, address	// load function address
		mov al, [esi]		// load the opcode
		cmp al, 0xCC		// check if the opcode is CCh
		je BPXed		// yes, there is a breakpoint

		// jump to return true
		xor eax, eax 		// false,
		jmp NOBPX 		// no breakpoint
	BPXed:
		mov eax, 1		// breakpoint found
	NOBPX:
	}
}
BOOL IsODBGLoaded()
	{
		CString OllyDbg="DA";
		OllyDbg+="EM";//ON";
		OllyDbg+="ON";
		char * caption=OllyDbg.GetBuffer() ;
		__asm
		{
			push 0x00
			push caption

			mov eax, fs:[30h] 		// pointer to PEB
			movzx eax, byte ptr[eax+0x2]
			or al,al
			jz normal_
			jmp out_
		normal_:
			xor eax, eax
			leave
			ret
		out_:
			mov eax, 0x1
			leave
			ret
		}
	}




STDAPI BFC(DWORD dwFunctionFilter)//BeginFunctionCapturing
	{
		CLock lock(g_csFunction);	
		
		Sleep (50);//can wait slightly
		if(g_Slogger)
			{
				/*if(!WANT_FUNCTION_NAME) 
					return S_OK;		*/			
				try
				{
					g_Slogger->m_mapObjectIDToObjectTag.clear();
					g_Slogger->m_mapObjectIDToClassID.clear(); 
				}catch(...){}
				try
				{
					g_Slogger->m_mapTempObjectIDToObjectTag.clear();  
					g_Slogger->m_mapTempObjectIDToClassID.clear(); 
				}catch(...){}

				if(!CAN_PROFILE_FOR_FUNCTIONS)
					{
						if(hGlobalDebugCheck==NULL)
						{
							try
							{
								DWORD dwID;
								hGlobalDebugCheck=CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
							}catch(...)
							{
								return S_OK;
							}
						}
						CAN_PROFILE_FOR_FUNCTIONS=true;
						
					}
				if(g_Slogger->m_dwFunctionFilter == 0)
					{
						g_Slogger->m_dwFunctionFilter =dwFunctionFilter;						
					}	
			}
		return S_OK;
	}

STDAPI IP(DWORD PID)//InitializeProfiling
	{
		CLock lock(g_csFunction);			
		try
		{
			DWORD dwID;
			hGlobalDebugCheckInit=CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
		}catch(...)
		{
			return S_OK;
		}

		Sleep (150);//can wait slightly		
		if(g_Slogger)
		{				
			if(g_Slogger->m_ProfileePID == -1) 
					{
						DWORD hashVal=GetCurrentProcessId();
						char hashBuffer[32];
						CString strHash=ltoa(hashVal,hashBuffer,10);
						LPSTR lpstrHash=strHash.GetBuffer(); 
						
						for(int i = 0 ; i < strHash.GetLength() ; i++)
						{
							hashVal^=(DWORD)(lpstrHash[i]);
						}			

						g_Slogger->m_ProfileePID =(PID-hashVal!=0)?PID:GetCurrentProcessId() ;

						try
						{	
							g_Slogger->CanRunGCBeforeOC(); 
							g_Slogger->GetLastSessionName();
							g_Slogger->GetLastSessionPath();
							
						}
						catch(...)
						{
							g_Slogger->m_bRunGC_BOC=true;					
							char buffer[32];
							g_Slogger->m_csSessionName="ProfilerSession";
							g_Slogger->m_csSessionName.Append(itoa(rand(),buffer,10));

							////////////////////
							g_Slogger->m_csSessionPath="C:\\";					
						}

						try
						{
							g_Slogger->SetTimeResolutionFilter();					
						}
						catch(...)
						{
							g_Slogger->m_dwTimeResolution=TR_HIGH_RESOLUTION;
						}
						try
						{
							g_Slogger->ResolveFilters();
						}
						catch(...){}
							return S_OK;
					}
			}

		SetLastError(ERROR_ALREADY_INITIALIZED);   
		return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);

	}
			
			
void RestoreCLR()
	{
	if(!g_Slogger)
		{
			return;
		}
		g_Slogger->m_bSuspendOnStart=false;                    					
							try
							{
								if(g_hCLRHandle)
									{
										SetEvent(g_hCLRHandle);
									}
							}
							catch(...){}
							Sleep(150);
							try
							{	if(g_hCLRHandle)
								{
									CloseHandle(g_hCLRHandle);					
								}
							}catch(...){}
							g_hCLRHandle=NULL; 	

					try
					{
						if(g_Slogger->m_hSuspensionMutexFlag)
						{
							ReleaseMutex(g_Slogger->m_hSuspensionMutexFlag);				
						}
					}
					catch(...){}
					if(g_Slogger->m_hSuspensionMutexFlag){CloseHandle(g_Slogger->m_hSuspensionMutexFlag);}
					g_Slogger->m_hSuspensionMutexFlag=NULL;//V. Imp.

					CString strMutexString="Global\\";
					g_pid=GetCurrentProcessId();//re-affirm
					char buffer[16];
					strMutexString.Append(ltoa(g_pid,buffer,10));
					strMutexString.Append("?");
					strMutexString +="FALSE";
					
					g_Slogger->m_hSuspensionMutexFlag=CreateMutex(NULL,false,strMutexString);				

					g_Slogger->m_bSuspendOnStart=false;		
	}
						
STDAPI RCLR(DWORD PID)//ResumeCLR
{
	CLock lock(g_csFunction);	
	Sleep (150);//can wait slightly	
	try
		{
			DWORD dwID;
			hGlobalDebugCheckInit=CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
		}catch(...)
		{
			return S_OK;
		}

				DWORD hashVal=GetCurrentProcessId();
				char hashBuffer[32];
				CString strHash=ltoa(hashVal,hashBuffer,10);
				LPSTR lpstrHash=strHash.GetBuffer(); 
				
				for(int i = 0 ; i < strHash.GetLength() ; i++)
				{
					hashVal^=(DWORD)(lpstrHash[i]);
				}

	if(g_Slogger!=NULL)	{

		g_Slogger->m_ProfileePID =(PID-hashVal!=0)?PID:GetCurrentProcessId();
				
//		if( (g_Slogger->m_ProfileePID > 0   )  &&  (PID == g_Slogger->m_ProfileePID) )	//no need for this as we also will want to resume non profilee applications that have been suspended on startup
		{
			if((g_hCLRHandle)  && (g_Slogger->m_bSuspendOnStart)  &&  (g_Slogger->m_hSuspensionMutexFlag)) 
			{     
					RestoreCLR();
					HRESULT hr=HRESULT_FROM_WIN32(GetLastError());
					return  hr;
			}
			else
			{
				SetLastError(ERROR_ALREADY_INITIALIZED);   
				return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);

			}
		}
	}

		SetLastError(E_FAIL);   
		return HRESULT_FROM_WIN32(E_FAIL);	
}



STDAPI SFC()//StopFunctionCapturing
	{
		CLock lock(g_csFunction); 		
		if(CAN_PROFILE_FOR_FUNCTIONS)
			{
				CAN_PROFILE_FOR_FUNCTIONS=false;//no need to stop the process on SFC				
			}
		
		return S_OK;
	}

STDAPI DFD(/*get session from reg*/)//DumpFunctionData
	{

	CLock lock(g_csFunction);
	CLockObjectDump lockDump;
	try
				{
					g_Slogger->m_mapObjectIDToObjectTag.clear();
					g_Slogger->m_mapObjectIDToClassID.clear(); 
				}catch(...){}
				try
				{
					g_Slogger->m_mapTempObjectIDToObjectTag.clear();  
					g_Slogger->m_mapTempObjectIDToClassID.clear(); 
				}catch(...){}

	USES_CONVERSION ;
	char buffer[256];

	g_Slogger->GetLastSessionName();
	g_Slogger->GetLastSessionPath();

	if(g_Slogger->m_csSessionName.GetLength()==0 || g_Slogger->m_csSessionPath.GetLength()==0   )  
	{
			SetLastError(HRESULTTOWIN32(E_ADS_BAD_PATHNAME ));
			return  E_ADS_BAD_PATHNAME ;
	}



	if(!g_Slogger  ||  IsBadReadPtr(g_Slogger,sizeof(g_Slogger)))// ||  !strSessionID ||  strcmp(strSessionID,"")==0 )
		{
			SetLastError(HRESULTTOWIN32(E_FAIL));
			return E_FAIL; 
		}

	if(!WANT_FUNCTION_NAME) 
					return S_OK;		

		if(g_LogFile)
			{	
				SetLastError(HRESULTTOWIN32(E_PENDING)); 
				return E_PENDING;
			}	
	
	g_LogFile = NULL;
	CAN_PROFILE_FOR_FUNCTIONS=false;	//must stop capturing data before dumping
	try
		{			
			CString csSessionUNC=(g_Slogger->m_csSessionPath.Right(1)=="\\")?g_Slogger->m_csSessionPath:g_Slogger->m_csSessionPath + "\\"   ;
			csSessionUNC.Append(g_Slogger->m_csSessionName);
			csSessionUNC.Append(".fxml");
			g_LogFile =fopen( csSessionUNC.GetBuffer() , "wt");

		}
	catch(...)		
		{
			g_LogFile=NULL;
		}

	if(!g_LogFile)
		{	
			return HRESULT_FROM_WIN32(GetLastError()); 
		}
	
	try
		{		
		
		
		TEXT_OUTLN("<FunctionDataSet>") ;

		///////////File Specific Data Attributes

				TEXT_OUTLN("<DataAttributes>");			
				try
				{
										
						CString dataAttributes="<Version>1.3</Version>";
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();
                        //Time resolution Flag
						dataAttributes="<TRFlag>"; 
							dataAttributes.Append(ltoa(g_Slogger->m_dwTimeResolution,buffer,10));  
						dataAttributes.Append("</TRFlag>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();
						///////////Function Filter Flag///
						dataAttributes="<FunctionFlag>"; 
							dataAttributes.Append(ltoa(g_Slogger->m_dwFunctionFilter,buffer,10));  
						dataAttributes.Append("</FunctionFlag>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();

						///////////Function Class Filter  
						dataAttributes="<FunctionClassFilter>"; 
						for(ULONG x=0;x<g_Slogger->m_functionClassFilter.size();x++)
						{
							dataAttributes.Append(g_Slogger->m_functionClassFilter.c.at(x));
							dataAttributes.Append("|"); 
						}						
						dataAttributes.Append("</FunctionClassFilter>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();

						/////////////Function Module Filter 

						dataAttributes="<FunctionModuleFilter>"; 
						for(ULONG x=0;x<g_Slogger->m_functionModuleFilter.size();x++)
						{
							dataAttributes.Append(g_Slogger->m_functionModuleFilter.c.at(x));
							dataAttributes.Append("|"); 
						}
						dataAttributes.Append("</FunctionModuleFilter>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();

						///////////FunctionClassPassthrough Flag//
						dataAttributes="<FunctionClassPassthrough>"; 	
						dataAttributes.Append((g_Slogger->m_bFunctionClassPassthrough==true)?"1":"0") ;
						dataAttributes.Append("</FunctionClassPassthrough>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();	

						///////////FunctionModulePassthrough Flag//
						dataAttributes="<FunctionModulePassthrough>"; 	
						dataAttributes.Append((g_Slogger->m_bFunctionModulePassthrough==true)?"1":"0") ;
						dataAttributes.Append("</FunctionModulePassthrough>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();	


						//////////////Process Name//////////////						
						dataAttributes="<ProfileeAppName>"; 	
						try
						{
							TCHAR strAppName[MAX_LENGTH];							
							if(GetModuleBaseName(GetCurrentProcess(),NULL,strAppName,sizeof(strAppName)))
							{
								dataAttributes.Append(T2A(strAppName));
							}

						}
						catch(...){}
						dataAttributes.Append("</ProfileeAppName>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();	
						
						/////Session ID/////////////
						dataAttributes="<SessionID>"; 	
						try
						{
							dataAttributes.Append(g_Slogger->m_csSessionName);
						}
						catch(...){}
						dataAttributes.Append("</SessionID>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();

				}
				catch(...){}
				TEXT_OUTLN("</DataAttributes>");
		TEXT_OUTLN("</FunctionDataSet>") ;
			//////////////////
		try
			{ 
				if(g_Slogger)
					g_Slogger->Trace(); //Dump!
			}
		catch(...)
			{}
			
		}
	catch(...)
		{	}
	
	if (g_LogFile )
		{			
			fclose( g_LogFile );			
		}
		g_LogFile = NULL;				
		
	return S_OK;	
	}

	STDAPI COC()//ClearObjectCache
	{
		CLock lock(g_csUpdate); 
		CLockObjectDump lockDump;		//CHANGE
		
		if(CAN_PROFILE_FOR_OBJECTS )
			{			
				SetLastError(HRESULTTOWIN32(E_PENDING));
				return E_PENDING;
			}
			//should let the profiler 
		//collect objects first

		if( g_Slogger)	
			{	
				/*g_Slogger->m_ui64ObjectCount=0;
				g_Slogger->m_ui64ObjectsSize=0;*/
				ClearLiveObjCache();
				ClearRootObjCache ();
				ClearRefObjCache();				
				//ClearObjAllocCache();//We ought to clear this cache only when the profiler exits!!(Why?)

			}
		return S_OK;

    }

STDAPI CO(DWORD dwObjectFilter)//CollectObjects
{
	CLock lock(g_csUpdate); 	
	Sleep(50);	//can wait slightly
	if(!g_Slogger)
		{
			SetLastError(HRESULTTOWIN32(E_FAIL));
			return E_FAIL;	
		}
	//make the calling of GC before data collection optional

	CAN_PROFILE_FOR_OBJECTS=false; //stop profiling first
	COC();//Clear cache 

	//if(g_Slogger->m_bRunGC_BOC)  //Lets Always run GC first
	{
		g_Slogger->m_pCorProfilerInfo2->ForceGC();  //we can call ForceGC because it is 	
		//called in a different thread from the App's main thread.
		//We are using CreateRemoteThread for this
	}
	

	Sleep(50);		//can wait slightly
	if(hGlobalDebugCheck==NULL)
		{
			try
			{
				DWORD dwID;
				hGlobalDebugCheck=CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
			}catch(...){}
		}

if(g_Slogger->m_dwObjectFilter == 0 )
	{
		g_Slogger->m_dwObjectFilter =dwObjectFilter;
		if(WANT_OBJECT_ALLOCATION_DATA)
			{				
				return S_OK;
			}
	}			
				
				//////////////////////////////////////////////////
				CAN_PROFILE_FOR_OBJECTS=true;	//start profiling;	
				g_Slogger->m_pCorProfilerInfo2->ForceGC(); 				
				//CAN_PROFILE_FOR_OBJECTS is turned off 
				//automatically at runtime resumption after GC
				//
	if(g_hWaitToDumpObjectData==NULL)
					{
						g_hWaitToDumpObjectData=CreateEvent(NULL,TRUE,FALSE,"SYNC_DUMP");	
					}
	
	return S_OK;
}
   

STDAPI DOD(/* Get session from reg*/)//DumpObjectData
	{
	CLock lock(g_csFunction); 	
	CLockObjectDump lockDump; 

	if(!g_Slogger  ||  IsBadReadPtr(g_Slogger,sizeof(g_Slogger)))
		{
			SetLastError(HRESULTTOWIN32(E_POINTER));			
			return E_FAIL;
		}

	if(g_LogFile || CAN_PROFILE_FOR_OBJECTS )
		{
			SetLastError(HRESULTTOWIN32(E_PENDING));
			return E_PENDING;
		}

	g_Slogger->GetLastSessionName();//update Session variables
	g_Slogger->GetLastSessionPath();


	if(g_Slogger->m_csSessionName.GetLength()==0 || g_Slogger->m_csSessionPath.GetLength()==0   )  
	{
			SetLastError(HRESULTTOWIN32(E_ADS_BAD_PATHNAME ));
			return  E_ADS_BAD_PATHNAME ;
	}
	//either logfile is being written to or objects are being collected
	// So wait...

		if(!WANT_OBJECT_NAME_ONLY)
			return E_UNEXPECTED ;

	//////////
	char buffer[256];	
	
	g_LogFile = NULL;
	try
		{
			CString csSessionUNC=(g_Slogger->m_csSessionPath.Right(1)=="\\")?g_Slogger->m_csSessionPath:g_Slogger->m_csSessionPath + "\\"   ;
			csSessionUNC.Append(g_Slogger->m_csSessionName);
			csSessionUNC.Append(".oxml");
			g_LogFile =fopen( csSessionUNC.GetBuffer() , "wt");

		}
	catch(...)		
		{
			g_LogFile=NULL;
		}

	if(!g_LogFile)
		{
			return HRESULT_FROM_WIN32(GetLastError()); 
		}
	

		TEXT_OUT("<ObjectDataSet>") ;	

		/////////////////////Object Attributes//////////////
				TEXT_OUTLN("<DataAttributes>");			
				try
				{										
						CString dataAttributes="<Version>1.3</Version>";
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();  
						//////////Was GC Run before Object Collection//
						dataAttributes="<GCBeforeOC>"; 
						dataAttributes.Append( g_Slogger->m_bRunGC_BOC==true?"1":"0" );  
						dataAttributes.Append("</GCBeforeOC>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();  

                        //////////Object Filter flag//
						dataAttributes="<ObjectFlag>"; 
							dataAttributes.Append(ltoa(g_Slogger->m_dwObjectFilter,buffer,10));  
						dataAttributes.Append("</ObjectFlag>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();   

						
						///////////Object Class Filter  
						dataAttributes="<ObjectClassFilter>"; 
						for(ULONG x=0;x<g_Slogger->m_objectClassFilter.size();x++)
						{
							dataAttributes.Append(g_Slogger->m_objectClassFilter.c.at(x));
							dataAttributes.Append("|"); 
						}
						dataAttributes.Append("</ObjectClassFilter>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();

						/////////////Object Module Filter
						/*

						dataAttributes="<ObjectModuleFilter>"; 
						for(ULONG x=0;x<g_Slogger->m_objectModuleFilter.size();x++)
						{
							dataAttributes.Append(g_Slogger->m_objectModuleFilter.c.at(x));
							dataAttributes.Append("|"); 
						}
						dataAttributes.Append("</ObjectModuleFilter>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();						*/

						///////////ObjectPassthrough Flag//
						dataAttributes="<ObjectPassthrough>"; 	
						dataAttributes.Append((g_Slogger->m_bObjectPassthrough==true)?"1":"0") ;
						dataAttributes.Append("</ObjectPassthrough>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();


						//////////////Process Name//////////////						
						dataAttributes="<ProfileeAppName>"; 	
						try
						{
							TCHAR strAppName[MAX_LENGTH];							
							if(GetModuleBaseName(GetCurrentProcess(),NULL,strAppName,MAX_LENGTH))
							{
								dataAttributes.Append(T2A(strAppName));
							} 							

						}
						catch(...){}
						dataAttributes.Append("</ProfileeAppName>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();	

						/////Session ID/////////////
						dataAttributes="<SessionID>"; 	
						try
						{
							dataAttributes.Append(g_Slogger->m_csSessionName); 
						}
						catch(...){}
						dataAttributes.Append("</SessionID>"); 
						TEXT_OUTLN(dataAttributes.GetBuffer());
						dataAttributes.Empty();	

				}
				catch(...){}
				TEXT_OUTLN("</DataAttributes>");
				TEXT_OUTLN("</ObjectDataSet>") ;
				//////////////////////////////////////////////////////
                
				TEXT_OUTLN("<ObjectID,ObjectName,ObjectSize,ObjectCount,IsRootObject>");	//always emitted	

				///////////////////////	
								//insert clean up code
//NOTE: NOW MAKE SURE THAT THERE IS NO MovedReferences CALLED B/W CO AND DOD.SO CALL DOD FROM CO AND NOT
				//EXPLICITLY FROM SHARPCLIENT.OR SUSPEND THE MOVED REFERENCE TILL DUMP
			

			if(WANT_OBJECT_ALLOCATION_DATA && g_Slogger->m_mapObjectAllocation.size()>0   )
				{
					g_Slogger->m_mapSwapObjectAllocation.clear();
					g_Slogger->m_mapTempObjectAllocation.clear ();
					for(map<UINT_PTR,POBJECTDATA>::const_iterator liveObjectDataIter=g_Slogger->m_mapLiveObjects.begin() ;liveObjectDataIter!=g_Slogger->m_mapLiveObjects.end();liveObjectDataIter++)
						{
							if(g_Slogger->m_mapObjectAllocation.find(liveObjectDataIter->first)!=g_Slogger->m_mapObjectAllocation.end() )
								{
									g_Slogger->m_mapTempObjectAllocation[liveObjectDataIter->first]=g_Slogger->m_mapObjectAllocation[liveObjectDataIter->first]; 
									g_Slogger->m_mapObjectAllocation.erase(liveObjectDataIter->first);
								}			
						}

					g_Slogger->m_mapObjectAllocation.clear();
					g_Slogger->m_mapObjectAllocation=g_Slogger->m_mapTempObjectAllocation;
				}

				if(g_Slogger->m_mapObjectIDToObjectTag.size()>0)
				{
					g_Slogger->m_mapTempObjectIDToObjectTag.clear(); 
					g_Slogger->m_mapTempObjectIDToClassID.clear(); 
					for(map<UINT_PTR,POBJECTDATA>::const_iterator liveObjectDataIter=g_Slogger->m_mapLiveObjects.begin() ;liveObjectDataIter!=g_Slogger->m_mapLiveObjects.end();liveObjectDataIter++)
						{
							if(g_Slogger->m_mapObjectIDToObjectTag.find(liveObjectDataIter->first)!=g_Slogger->m_mapObjectIDToObjectTag.end() )
								{									
									if(g_Slogger->m_bIsTwoDotO)
									{
										g_Slogger->m_mapTempObjectIDToClassID[liveObjectDataIter->first]=g_Slogger->m_mapObjectIDToClassID[liveObjectDataIter->first]; 
										try
										{
											g_Slogger->m_mapObjectIDToClassID.erase(liveObjectDataIter->first);
										}catch(...){}
									}

									g_Slogger->m_mapTempObjectIDToObjectTag[liveObjectDataIter->first]=g_Slogger->m_mapObjectIDToObjectTag[liveObjectDataIter->first]; 
									try
									{
										g_Slogger->m_mapObjectIDToObjectTag.erase(liveObjectDataIter->first);
									}catch(...){}
								
								}			
						}
					
				}

				try
				{
					//root objects	
					g_Slogger->DumpStringAndStructMapForRootObjects(g_Slogger->m_mapRootObjects); 
				}
				catch(...){}	
				/////////////////////Live Instances//////////////				
										
				try
				{					
						//Live Objects
					g_Slogger->DumpStringAndStructMapForLiveObjects(g_Slogger->m_mapLiveObjects);					
					
				}
				catch(...){}

				try
				{			
					ClearRootObjCache();
				}catch(...)	{}
				try
				{
				ClearLiveObjCache();
				}catch(...)	{}

				try
				{						
					g_Slogger->DumpObjectAllocationData();  //always emitted								
				}
				catch(...){}

				
				////REFERENCED_OBJECTS/////////////
				try
				{
					TEXT_OUTLN("<RefObjectID,ParentObjectID,RefObjectName,RefObjectCount,RefObjectSize>");//always emitted
					if(WANT_REFERENCED_OBJECTS)
					{
						//Referenced Objects
						g_Slogger->DumpStringAndStructMapForReferencedObjects(g_Slogger->m_mapReferencedObjects); 
						
					}
				}
				catch(...){}
				try
				{
					ClearRefObjCache();	
				}
				catch(...){}

				g_Slogger->m_mapObjectIDToObjectTag.clear();
				g_Slogger->m_mapObjectIDToObjectTag.swap(g_Slogger->m_mapTempObjectIDToObjectTag);
				g_Slogger->m_mapTempObjectIDToObjectTag.clear(); 			
							
				/////////////////


				if(g_Slogger->m_bIsTwoDotO)
					{
						g_Slogger->m_mapObjectIDToClassID.clear();
						g_Slogger->m_mapObjectIDToClassID.swap(g_Slogger->m_mapTempObjectIDToClassID);
						g_Slogger->m_mapTempObjectIDToClassID.clear(); 	
					}
				///////////////

					///////CLEAR FUNCTION CACHE
					try
					{
						ClearFunctionCache(true) ;
					}
					catch(...){}
							

	if (g_LogFile != NULL )
		{
			
			fclose( g_LogFile );	
			
		}
		g_LogFile = NULL;		

	return S_OK;
			
	}

__forceinline bool IsBeingDebugged() 
{
	try
	{
		CString NTICE="\\\\.\\";
		NTICE+="NT";
		NTICE+="IC";
		NTICE+="E";
		HANDLE hFile=CreateFile( NTICE.GetBuffer() ,
					GENERIC_READ | GENERIC_WRITE,
					FILE_SHARE_READ | FILE_SHARE_WRITE,
					NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

		if(hFile!=INVALID_HANDLE_VALUE || GetLastError()==ERROR_ALREADY_EXISTS)
		{
			try
			{
				CloseHandle(hFile);
			}catch(...){}
			return true; 
		}

		if(IsODBGLoaded())
		{
			return true;
		}

		if(IsDebuggerPresent())
		{
			return true;
		}
		
	__asm
	{
		mov eax, fs:[0x30]
		mov eax, [eax+0xC]
		mov eax, [eax+0xC]
		add dword ptr [eax+0x20], 0x2000	// increase size variable
	}

	if( IsBPX(&IsBPX) || IsBPX(&IsBeingDebugged) || IsBPX(&IsDebuggerPresent) || IsBPX(&BFC) || IsBPX (&CO))
	{
		return true;
	}

	}
	catch(...)
	{
		return true;
	}

	return false;

}

__forceinline LRESULT WINAPI startCheck(LPVOID param)//To avoid debuggers from hacking in a profilee process ;-)
{
	CLock lock(g_csFunction);	

#ifdef _DEBUG
	while(true)	
#else
	while (!IsBeingDebugged())	
#endif
	{
		Sleep( (( rand()%6 )+ 3 )*500) ;
	}
	
	exit(0);
}

OBJECT_ENTRY_AUTO(__uuidof(Slogger), CSlogger)

