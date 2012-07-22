/* this file contains the actual definitions of */
/* the IIDs and CLSIDs */

/* link this file in with the server and any clients */


/* File created by MIDL compiler version 5.01.0164 */
/* at Wed Nov 01 05:38:21 2006
 */
/* Compiler settings for F:\Development\ProfileSharpServer\ProfileSharpServer.idl:
    Oicf (OptLev=i2), W1, Zp8, env=Win32, ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
*/
//@@MIDL_FILE_HEADING(  )
#ifdef __cplusplus
extern "C"{
#endif 


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

const IID IID_ISharpDelegator = {0xB68F231E,0x2261,0x4D7A,{0x80,0xB4,0x7D,0x83,0xC8,0x3C,0xED,0xD3}};


const IID LIBID_PROFILESHARPSERVERLib = {0xD21E0CFC,0xE036,0x4B91,{0xA4,0xE8,0xBF,0xDC,0xBA,0xF5,0x43,0x1E}};


const CLSID CLSID_SharpDelegator = {0x94E4FC76,0xB39F,0x4CED,{0x9C,0xD6,0x59,0xFE,0x02,0x2E,0x61,0x39}};


#ifdef __cplusplus
}
#endif

