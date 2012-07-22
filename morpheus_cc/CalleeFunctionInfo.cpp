#include "StdAfx.h"
#include "calleefunctioninfo.h"

CalleeFunctionInfo::CalleeFunctionInfo()
{
  llCycleCount = 0;
  llRecursiveCycleCount = 0;
  nRecursiveCount = 0;
  nCalls = 0;
}

CalleeFunctionInfo::~CalleeFunctionInfo()
{

}

