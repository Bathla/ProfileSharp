// SharpDelegator.h : Declaration of the CSharpDelegator

#ifndef __SHARPDELEGATOR_H_
#define __SHARPDELEGATOR_H_

#include "resource.h"       // main symbols
#include "ProfileSharpServer.h"

class CLock
{
public:	
	CLock( CRITICAL_SECTION& cs );
	~CLock();
private:
	CRITICAL_SECTION& m_cs;			
}; 
  
  __forceinline CLock::CLock(CRITICAL_SECTION &cs):m_cs(cs)
	  {			
			InitializeCriticalSection(&cs);
			EnterCriticalSection(&cs); 
	  }

  __forceinline CLock::~CLock ()
	  {
			LeaveCriticalSection(&m_cs);
			DeleteCriticalSection(&m_cs);
	  }


CRITICAL_SECTION g_cs;

class CLock;

/////////////////////////////////////////////////////////////////////////////
// CSharpDelegator
class ATL_NO_VTABLE CSharpDelegator : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CSharpDelegator, &CLSID_SharpDelegator>,
	public IDispatchImpl<ISharpDelegator, &IID_ISharpDelegator, &LIBID_PROFILESHARPSERVERLib>
{
private:
	static HMODULE hProfiler;
public:
	CSharpDelegator()
	{
	}

	STDMETHOD(RefreshProfileeStatus)(BSTR SystemName, BSTR* profileeStatus);

	STDMETHOD(DeferMethodCall)(LONG pid, BSTR methodString, LONG lParam);	
	STDMETHOD(put_profilerName)(BSTR newVal);
	STDMETHOD(InstallServiceEnvironment)(BSTR EnvSetter,BSTR targetProcess);

DECLARE_REGISTRY_RESOURCEID(IDR_SHARPDELEGATOR)
DECLARE_NOT_AGGREGATABLE(CSharpDelegator)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CSharpDelegator)
	COM_INTERFACE_ENTRY(ISharpDelegator)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// ISharpDelegator
public:
};

HMODULE CSharpDelegator::hProfiler=NULL; 
#endif //__SHARPDELEGATOR_H_
