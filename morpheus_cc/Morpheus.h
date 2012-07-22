

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0366 */
/* at Sun May 06 09:13:02 2007
 */
/* Compiler settings for .\Morpheus.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
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

#ifndef __Morpheus_h__
#define __Morpheus_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ISlogger_FWD_DEFINED__
#define __ISlogger_FWD_DEFINED__
typedef interface ISlogger ISlogger;
#endif 	/* __ISlogger_FWD_DEFINED__ */


#ifndef ___ISloggerEvents_FWD_DEFINED__
#define ___ISloggerEvents_FWD_DEFINED__
typedef interface _ISloggerEvents _ISloggerEvents;
#endif 	/* ___ISloggerEvents_FWD_DEFINED__ */


#ifndef __Slogger_FWD_DEFINED__
#define __Slogger_FWD_DEFINED__

#ifdef __cplusplus
typedef class Slogger Slogger;
#else
typedef struct Slogger Slogger;
#endif /* __cplusplus */

#endif 	/* __Slogger_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __ISlogger_INTERFACE_DEFINED__
#define __ISlogger_INTERFACE_DEFINED__

/* interface ISlogger */
/* [unique][helpstring][nonextensible][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISlogger;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9BD5EAA3-EC8A-42ED-A47A-F25BB0DD12C7")
    ISlogger : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct ISloggerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISlogger * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISlogger * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISlogger * This);
        
        END_INTERFACE
    } ISloggerVtbl;

    interface ISlogger
    {
        CONST_VTBL struct ISloggerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISlogger_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ISlogger_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ISlogger_Release(This)	\
    (This)->lpVtbl -> Release(This)


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISlogger_INTERFACE_DEFINED__ */



#ifndef __MorpheusLib_LIBRARY_DEFINED__
#define __MorpheusLib_LIBRARY_DEFINED__

/* library MorpheusLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_MorpheusLib;

#ifndef ___ISloggerEvents_DISPINTERFACE_DEFINED__
#define ___ISloggerEvents_DISPINTERFACE_DEFINED__

/* dispinterface _ISloggerEvents */
/* [helpstring][uuid] */ 


EXTERN_C const IID DIID__ISloggerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("E8733137-C913-4923-A595-D6BE613DC897")
    _ISloggerEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _ISloggerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _ISloggerEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _ISloggerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _ISloggerEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _ISloggerEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _ISloggerEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _ISloggerEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _ISloggerEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _ISloggerEventsVtbl;

    interface _ISloggerEvents
    {
        CONST_VTBL struct _ISloggerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _ISloggerEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _ISloggerEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _ISloggerEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _ISloggerEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _ISloggerEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _ISloggerEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _ISloggerEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___ISloggerEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_Slogger;

#ifdef __cplusplus

class DECLSPEC_UUID("383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D")
Slogger;
#endif
#endif /* __MorpheusLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


