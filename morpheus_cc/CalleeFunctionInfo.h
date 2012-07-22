#pragma once

class CalleeFunctionInfo {
public: 
	CalleeFunctionInfo();
	~CalleeFunctionInfo();

  UINT64 llCycleCount;
  UINT64 llRecursiveCycleCount;
  UINT64 nCalls;
  UINT64 nRecursiveCount;

};