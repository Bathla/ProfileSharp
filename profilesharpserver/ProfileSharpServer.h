/* this ALWAYS GENERATED file contains the definitions for the interfaces */


/* File created by MIDL compiler version 5.01.0164 */
/* at Wed Nov 01 05:38:21 2006
 */
/* Compiler settings for F:\Development\ProfileSharpServer\ProfileSharpServer.idl:
    Oicf (OptLev=i2), W1, Zp8, env=Win32, ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __ProfileSharpServer_h__
#define __ProfileSharpServer_h__

#ifdef __cplusplus
extern "C"{
#endif 

/* Forward Declarations */ 

#ifndef __ISharpDelegator_FWD_DEFINED__
#define __ISharpDelegator_FWD_DEFINED__
typedef interface ISharpDelegator ISharpDelegator;
#endif 	/* __ISharpDelegator_FWD_DEFINED__ */


#ifndef __SharpDelegator_FWD_DEFINED__
#define __SharpDelegator_FWD_DEFINED__

#ifdef __cplusplus
typedef class SharpDelegator SharpDelegator;
#else
typedef struct SharpDelegator SharpDelegator;
#endif /* __cplusplus */

#endif 	/* __SharpDelegator_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

void __RPC_FAR * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void __RPC_FAR * ); 

#ifndef __ISharpDelegator_INTERFACE_DEFINED__
#define __ISharpDelegator_INTERFACE_DEFINED__

/* interface ISharpDelegator */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_ISharpDelegator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B68F231E-2261-4D7A-80B4-7D83C83CEDD3")
    ISharpDelegator : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE RefreshProfileeStatus( 
            /* [in] */ BSTR SystemName,
            /* [retval][out] */ BSTR __RPC_FAR *profileeStatus) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DeferMethodCall( 
            /* [in] */ LONG pid,
            /* [in] */ BSTR methodString,
            /* [in] */ LONG lParam) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_profilerName( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE InstallServiceEnvironment( 
            /* [in] */ BSTR EnvSetter,
            /* [in] */ BSTR targetProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ISharpDelegatorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *QueryInterface )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *AddRef )( 
            ISharpDelegator __RPC_FAR * This);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *Release )( 
            ISharpDelegator __RPC_FAR * This);
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *GetTypeInfoCount )( 
            ISharpDelegator __RPC_FAR * This,
            /* [out] */ UINT __RPC_FAR *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *GetTypeInfo )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo __RPC_FAR *__RPC_FAR *ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *GetIDsOfNames )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR __RPC_FAR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID __RPC_FAR *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Invoke )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS __RPC_FAR *pDispParams,
            /* [out] */ VARIANT __RPC_FAR *pVarResult,
            /* [out] */ EXCEPINFO __RPC_FAR *pExcepInfo,
            /* [out] */ UINT __RPC_FAR *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *RefreshProfileeStatus )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ BSTR SystemName,
            /* [retval][out] */ BSTR __RPC_FAR *profileeStatus);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *DeferMethodCall )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ LONG pid,
            /* [in] */ BSTR methodString,
            /* [in] */ LONG lParam);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *put_profilerName )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *InstallServiceEnvironment )( 
            ISharpDelegator __RPC_FAR * This,
            /* [in] */ BSTR EnvSetter,
            /* [in] */ BSTR targetProcess);
        
        END_INTERFACE
    } ISharpDelegatorVtbl;

    interface ISharpDelegator
    {
        CONST_VTBL struct ISharpDelegatorVtbl __RPC_FAR *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISharpDelegator_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ISharpDelegator_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ISharpDelegator_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ISharpDelegator_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define ISharpDelegator_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define ISharpDelegator_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define ISharpDelegator_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define ISharpDelegator_RefreshProfileeStatus(This,SystemName,profileeStatus)	\
    (This)->lpVtbl -> RefreshProfileeStatus(This,SystemName,profileeStatus)

#define ISharpDelegator_DeferMethodCall(This,pid,methodString,lParam)	\
    (This)->lpVtbl -> DeferMethodCall(This,pid,methodString,lParam)

#define ISharpDelegator_put_profilerName(This,newVal)	\
    (This)->lpVtbl -> put_profilerName(This,newVal)

#define ISharpDelegator_InstallServiceEnvironment(This,EnvSetter,targetProcess)	\
    (This)->lpVtbl -> InstallServiceEnvironment(This,EnvSetter,targetProcess)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ISharpDelegator_RefreshProfileeStatus_Proxy( 
    ISharpDelegator __RPC_FAR * This,
    /* [in] */ BSTR SystemName,
    /* [retval][out] */ BSTR __RPC_FAR *profileeStatus);


void __RPC_STUB ISharpDelegator_RefreshProfileeStatus_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ISharpDelegator_DeferMethodCall_Proxy( 
    ISharpDelegator __RPC_FAR * This,
    /* [in] */ LONG pid,
    /* [in] */ BSTR methodString,
    /* [in] */ LONG lParam);


void __RPC_STUB ISharpDelegator_DeferMethodCall_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ISharpDelegator_put_profilerName_Proxy( 
    ISharpDelegator __RPC_FAR * This,
    /* [in] */ BSTR newVal);


void __RPC_STUB ISharpDelegator_put_profilerName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ISharpDelegator_InstallServiceEnvironment_Proxy( 
    ISharpDelegator __RPC_FAR * This,
    /* [in] */ BSTR EnvSetter,
    /* [in] */ BSTR targetProcess);


void __RPC_STUB ISharpDelegator_InstallServiceEnvironment_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ISharpDelegator_INTERFACE_DEFINED__ */



#ifndef __PROFILESHARPSERVERLib_LIBRARY_DEFINED__
#define __PROFILESHARPSERVERLib_LIBRARY_DEFINED__

/* library PROFILESHARPSERVERLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_PROFILESHARPSERVERLib;

EXTERN_C const CLSID CLSID_SharpDelegator;

#ifdef __cplusplus

class DECLSPEC_UUID("94E4FC76-B39F-4CED-9CD6-59FE022E6139")
SharpDelegator;
#endif
#endif /* __PROFILESHARPSERVERLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long __RPC_FAR *, unsigned long            , BSTR __RPC_FAR * ); 
unsigned char __RPC_FAR * __RPC_USER  BSTR_UserMarshal(  unsigned long __RPC_FAR *, unsigned char __RPC_FAR *, BSTR __RPC_FAR * ); 
unsigned char __RPC_FAR * __RPC_USER  BSTR_UserUnmarshal(unsigned long __RPC_FAR *, unsigned char __RPC_FAR *, BSTR __RPC_FAR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long __RPC_FAR *, BSTR __RPC_FAR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif
