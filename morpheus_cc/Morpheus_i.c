

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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


#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, IID_ISlogger,0x9BD5EAA3,0xEC8A,0x42ED,0xA4,0x7A,0xF2,0x5B,0xB0,0xDD,0x12,0xC7);


MIDL_DEFINE_GUID(IID, LIBID_MorpheusLib,0x6C3C7899,0xA265,0x4091,0xB6,0x69,0x56,0x42,0xD1,0xB0,0xD7,0x73);


MIDL_DEFINE_GUID(IID, DIID__ISloggerEvents,0xE8733137,0xC913,0x4923,0xA5,0x95,0xD6,0xBE,0x61,0x3D,0xC8,0x97);


MIDL_DEFINE_GUID(CLSID, CLSID_Slogger,0x383227C2,0x1C12,0x4C3B,0x90,0xE7,0xEF,0x5B,0xCC,0xBE,0xBD,0x7D);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



