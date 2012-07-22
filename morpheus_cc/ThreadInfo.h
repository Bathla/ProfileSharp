#pragma once

class StackInfo;
class FunctionInfo;

class ThreadInfo
{
public:
	ThreadInfo(void);
	~ThreadInfo(void);
	void Start();
	void End();
	bool IsRunning();
	StackInfo* GetStackInfo();
	FunctionInfo* GetFunctionInfo( FunctionID fid );
	void Trace(char * szThreadID);
public:
	UINT64 _llStartTime;
	UINT64 _llEndTime;
	UINT64 _llSuspendTime;  
private:
  bool  _bRunning;
  StackInfo* _pStackInfo;
public:
  map< FunctionID, FunctionInfo* > _mFunctionInfo;
};
