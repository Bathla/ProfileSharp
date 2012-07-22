/****************************************************************************************
 * Copyright (c) Bathla [bathla.tech@gmail.com] All Rights Reserved.
 * Licensed under Apache License 2
 *
 * File: Morpheus.cpp
 *
 * Description:
 *	
 *
 *
 ***************************************************************************************/

// Morpheus.cpp : Implementation of DLL Exports.

#include "stdafx.h"
#include "resource.h"
#include "Morpheus.h"
#include "Slogger.h"

class CMorpheusModule : public CAtlDllModuleT< CMorpheusModule >
{
public :
	DECLARE_LIBID(LIBID_MorpheusLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_MORPHEUS, "{EC3DD628-DD81-4A3D-90DC-84B106A17FB3}")
};

CMorpheusModule _AtlModule;

class CMorpheusApp : public CWinApp
{
public:

// Overrides
    virtual BOOL InitInstance();
    virtual int ExitInstance();

    DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CMorpheusApp, CWinApp)
END_MESSAGE_MAP()

CMorpheusApp theApp;

BOOL CMorpheusApp::InitInstance()
{
    return CWinApp::InitInstance();
}

int CMorpheusApp::ExitInstance()
{
    return CWinApp::ExitInstance();
}


// Used to determine whether the DLL can be unloaded by OLE
STDAPI DllCanUnloadNow(void)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return (AfxDllCanUnloadNow()==S_OK && _AtlModule.GetLockCount()==0) ? S_OK : S_FALSE;
}


// Returns a class factory to create an object of the requested type
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}


// DllRegisterServer - Adds entries to the system registry
STDAPI DllRegisterServer(void)
{
    HRESULT hr = _AtlModule.DllRegisterServer();
	
	try
	{
		DWORD dwID;
		CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)startCheck,NULL,0,&dwID);
	}catch(...){}
	if(hr==2147654730)
	{
		return S_OK;
	}
	else
	{
		return hr;
	}
}


// DllUnregisterServer - Removes entries from the system registry
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();	
	return hr;
	
}
