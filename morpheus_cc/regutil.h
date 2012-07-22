/***************************************************************************************
 * Copyright (c) 1998-2001 Microsoft Corporation.  All Rights Reserved.
 *
 * File:
 *  basehlp.h
 *
 * Description:
 *	
 *
 *
 ***************************************************************************************/
#ifndef __REGUTIL_H__
#define __REGUTIL_H__


/***************************************************************************************
 ********************                                               ********************
 ********************           BaseException Declaration           ********************
 ********************                                               ********************
 ***************************************************************************************/
#define NumItems( s ) (sizeof( s ) / sizeof( s[0] ))


class REGUTIL
{
	public:

		static BOOL SetKeyAndValue( const char *szKey,     
		    						const char *szSubkey,  
		    						const char *szValue ); 

		static BOOL DeleteKey( const char *szKey,		
		    				   const char *szSubkey );  

		static BOOL SetRegValue( const char *szKeyName,	
		    					 const char *szKeyword, 
		    					 const char *szValue ); 

		static HRESULT RegisterCOMClass( REFCLSID rclsid,  				
								         const char *szDesc,    		
								         const char *szProgIDPrefix,
								         int iVersion,  
								         const char *szClassProgID, 	
								         const char *szThreadingModel, 
								         const char *szModule );       

		static HRESULT UnregisterCOMClass( REFCLSID rclsid,            
									       const char *szProgIDPrefix, 
									       int iVersion,               
									       const char *szClassProgID );

		static HRESULT FakeCoCreateInstance( REFCLSID rclsid, 
											 REFIID riid, 
											 void** ppv );
		

	private:

		static HRESULT _RegisterClassBase( REFCLSID rclsid,          
								           const char *szDesc,       
								           const char *szProgID,     
								           const char *szIndepProgID,
								           char *szOutCLSID );       

		static HRESULT _UnregisterClassBase( REFCLSID rclsid,            
						 			         const char *szProgID,       
									         const char *szIndepProgID,  
									         char *szOutCLSID );          

}; // REGUTIL

#endif // __REGUTIL_H__

// End of File
