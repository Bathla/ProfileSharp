#pragma once
class ThreadInfo;

class StackInfo
{
public:
	StackInfo( ThreadInfo* pThreadInfo );
	 UINT64 PopFunction(UINT64 llCycleCount  );
	 void PushFunction( FunctionInfo* pFunctionInfo);	  
	  void SuspendFunction( UINT64 llCycleCount );
	  void ResumeFunction( UINT64 llCycleCount );
	  stack< StackEntryInfo > _sFunctionStack;
	  bool IsDebugFunction(FunctionID functionID);
	   void GetDebugThread(ICorDebugThread** pCorDebugThread);
	  bool RetryGetInfo(FunctionInfo* _parentFunctionInfo,UINT64 llElapsed);
private:
	UINT64 _llSuspendStart;  
  ThreadInfo* _pThreadInfo;
};

