using System;
using System.Data;
using System.Data.OleDb;  
using PGBusinessLogic;
using System.Windows.Forms ;
 
namespace SharpClient
{
	/// <summary>
	/// 
	/// </summary>
	public class FunctionImporter : System.IDisposable
	{

		private OleDbConnection con;
		private int m_dwFunctionFilter;
		private int m_func_TRFlag;
		private string m_func_FunctionClassFilter;
		private string m_func_FunctionModuleFilter;
		private bool m_func_FunctionClassPassthrough;
		private bool m_func_FunctionModulePassthrough;
		private string m_func_ProfileeAppName;
		private string m_func_SessionID;


		private string m_SessionFile;

		private bool bIsAlreadyLoaded;
	
		private bool m_bCleanCache;
		private bool m_bContinueOnCacheError;	


		public FunctionImporter(string sessionFile,bool bCleanCache,string connectionString,bool bContinueOnCacheError)
		{

			bIsAlreadyLoaded=false;

			m_dwFunctionFilter=0;
			m_func_TRFlag=0;
			m_func_FunctionClassFilter=null;
			m_func_FunctionModuleFilter=null;
			m_func_FunctionClassPassthrough=true;
			m_func_FunctionModulePassthrough=true;
			m_func_ProfileeAppName=null;
			m_func_SessionID=null;	

			m_SessionFile=sessionFile;

			m_bCleanCache=bCleanCache;
			m_bContinueOnCacheError=bContinueOnCacheError;		


			con=new OleDbConnection(connectionString); 
			con.Open() ;

//			if(m_bCleanCache)
//				DeleteCache();			
			
		}

		public FunctionData getFunctionData
		{
			get
			{
				return new FunctionData(m_dwFunctionFilter,m_func_FunctionClassFilter,m_func_FunctionModuleFilter,m_func_FunctionClassPassthrough, 
					m_func_FunctionModulePassthrough,m_func_TRFlag);  
			}
		}


		public string profileeName
		{
			get
			{
				return m_func_ProfileeAppName;
			}
		}

		public string profileeSessionName
		{
			get
			{
				return m_func_SessionID;
			}
		}

		private void DeleteCache()
		{
			if(con==null)
				throw new Exception("The cache could not be retrieved."); 
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from FTable",null);
			}
			catch(Exception ex)
			{	
				if(!m_bContinueOnCacheError )
					throw new Exception("Function Cache could not be cleared.\n"+ ex.Message);					
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from DebugCode",null);
			}
			catch(Exception ex)
			{	
				if(!m_bContinueOnCacheError )
					throw new Exception("Code Cache could not be cleared.\n"+ ex.Message);					
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from CFTable",null);
			}
			catch(Exception ex)
			{
				if(!m_bContinueOnCacheError )
					throw new Exception("Callee Function cache could not be cleared.\n"+ ex.Message);					
			}

			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from Exceptions",null);
			}
			catch(Exception ex)
			{
				if(!m_bContinueOnCacheError )
					throw new Exception("Exceptions cache could not be cleared.\n"+ ex.Message);			
			}
			
		}

		public static void DeleteCache(string connectionString,bool bContinueOnCacheError)
		{
			OleDbConnection myCon=new OleDbConnection(connectionString);
			myCon.Open ();
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from FTable",null);
			}
			catch(Exception ex)
			{	
				if(myCon!=null)
					try	{myCon.Close() ;}	catch{}
				myCon=null;

				if(!bContinueOnCacheError )
				{					
					throw new Exception("Function Cache could not be cleared.\n"+ ex.Message);					
				}
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from DebugCode",null);
			}
			catch(Exception ex)
			{	
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
				{					
					throw new Exception("Code Cache could not be cleared.\n"+ ex.Message);					
				}
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from CFTable",null);
			}
			catch(Exception ex)
			{
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
					throw new Exception("Callee Function cache could not be cleared.\n"+ ex.Message);					
			}

			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from Exceptions",null);
			}
			catch(Exception ex)
			{
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
					throw new Exception("Exceptions cache could not be cleared.\n"+ ex.Message);			
			}

			if(myCon!=null)
				try	{myCon.Close() ;}	
				catch{}
			myCon=null;

			
		}

		private void CleanOff()
		{
			if(con!=null)
			{
				con.Close();
				con.Dispose(); 
				con=null;
			}		

		}


		public void Dispose()
		{
			try
			{
				CleanOff();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error while cleaning up resources :- \n" +ex.Message);  
			}		

		}

		public void loadSession()
		{
			loadPrivateSession();
		}

		private void loadPrivateSession()
		{
			System.IO.StreamReader funcImporter=null;
				
			//do not dispose
			funcImporter=new System.IO.StreamReader(m_SessionFile,System.Text.Encoding.ASCII);			
   
			string faString="";
			while(!(faString+=funcImporter.ReadLine()).EndsWith("</FunctionDataSet>"))
			{
			}

			string excString=null;
			DataSet functionDS=new DataSet();		
			try
			{
				functionDS.ReadXml(new System.IO.StringReader(faString),XmlReadMode.Auto);
				if(functionDS.Tables["DataAttributes"].Columns.Contains("Version") && (Convert.ToString(functionDS.Tables["DataAttributes"].Rows[0]["Version"])=="1.3")) 
				{
				}
				else
				{
					throw new Exception("The file version you are trying to open is obsolete.\nYou may need to re-create the profiling session."); 
				}

				m_func_SessionID=Convert.ToString (functionDS.Tables["DataAttributes"].Rows[0]["SessionID"]);  					
				OleDbCommand bVerifyCommand = new OleDbCommand("SELECT COUNT(*) FROM FTABLE WHERE SESSIONID='"+m_func_SessionID+"'",con);	
				bIsAlreadyLoaded=( Convert.ToInt32(bVerifyCommand.ExecuteScalar()) > 0 )?true:false ;				
				try
				{
					bVerifyCommand.Dispose(); 
				}
				catch{}


				m_dwFunctionFilter =Convert.ToInt32 (functionDS.Tables["DataAttributes"].Rows[0]["FunctionFlag"]);   	
				m_func_TRFlag=Convert.ToInt32 (functionDS.Tables["DataAttributes"].Rows[0]["TRFlag"]);  
				m_func_FunctionClassFilter=Convert.ToString (functionDS.Tables["DataAttributes"].Rows[0]["FunctionClassFilter"]);  
				m_func_FunctionModuleFilter=Convert.ToString (functionDS.Tables["DataAttributes"].Rows[0]["FunctionModuleFilter"]);  
				m_func_FunctionClassPassthrough=(Convert.ToInt32(functionDS.Tables["DataAttributes"].Rows[0]["FunctionClassPassthrough"]))==0?false:true;
				m_func_FunctionModulePassthrough=(Convert.ToInt32(functionDS.Tables["DataAttributes"].Rows[0]["FunctionModulePassthrough"]))==0?false:true;
				m_func_ProfileeAppName=Convert.ToString (functionDS.Tables["DataAttributes"].Rows[0]["ProfileeAppName"]);  
				

			}
			catch(Exception ex)
			{ 
				excString="Data Attributes import for [Performance] tables failed.\n"+ex.Message  ;				
			}	
			finally
			{
				if(functionDS!=null)
				{				
					functionDS.Relations.Clear();
					functionDS.Tables.Clear();
					functionDS.Clear();
					functionDS.Dispose();
				}
				if(funcImporter!=null)
				{
					funcImporter.Close();
					funcImporter=null;
				}

				if(excString!=null)
					throw new Exception(excString);
			}		
		}


		public void updateFunctions()
		{
			updatePrivateFunctions();
			
		}

		private void updatePrivateFunctions()
		{
			if(bIsAlreadyLoaded)
				return;

			System.IO.StreamReader funcImporter=null;		
			OleDbCommand myCommand=null;	

			try
			{
				myCommand = new OleDbCommand(null,con);	
				funcImporter=new System.IO.StreamReader(m_SessionFile,System.Text.Encoding.ASCII);

				while(!funcImporter.ReadLine().EndsWith("<FunctionID,FSignature,ThreadID,ModuleName,Calls,CollectiveTime>"))
				{

				}      
				string functionString=funcImporter.ReadLine();  //initialize
				
				string strFunctionQry="INSERT INTO FTABLE (FunctionID,FSignature,ThreadID,ModuleName,Calls,CollectiveTime,SessionID) VALUES(";
				string strExceptionsQry="INSERT INTO EXCEPTIONS (FunctionID,ThreadID,ExceptionName,ExceptionTrace,ExceptionID,SessionID) VALUES(";
				string strDebugCodeQry="INSERT INTO DEBUGCODE (FunctionID,ThreadID,FileName,TimeConsumed,HitCount,FileOffset,StartColumn,EndColumn,CollectiveTime,SessionID) VALUES(";
				string strRefFunctionQry="INSERT INTO CFTABLE (CalleeFunctionID,CFCalls,CFCollectiveTime,CallerFunctionID,CFSignature,CFThreadID,SessionID) VALUES(";				
				UInt64 ui64ExceptionID=0;

				while(functionString!=null)
				{
					myCommand.CommandText= strFunctionQry+functionString+",'"+m_func_SessionID+"')";
					myCommand.ExecuteNonQuery ();

					functionString=funcImporter.ReadLine();
					if(functionString!=null)
					{
						if(functionString.EndsWith("<FunctionID,FSignature,ThreadID,ModuleName,Calls,CollectiveTime>"))
						{
							functionString=funcImporter.ReadLine();
							continue;
						}
						if(functionString.EndsWith("<FunctionID,ThreadID,ExceptionName,ExceptionTrace>"))
						{
							functionString=funcImporter.ReadLine();
							string cumulativeExceptionString="";							
							while(true)
							{	
								if(functionString==null)
								{
									goto A;// I dare use "goto" ;
								}
								if(functionString.EndsWith(">"))
									break;		
								cumulativeExceptionString+=functionString+@"
";
								if(functionString.EndsWith("'"))
								{
									myCommand.CommandText= strExceptionsQry+cumulativeExceptionString+",'"+ (++ui64ExceptionID).ToString() +"','"+m_func_SessionID+"')";
									myCommand.ExecuteNonQuery ();
									cumulativeExceptionString="";							
								}
								functionString=funcImporter.ReadLine();								
							}
						}

						if(functionString.EndsWith("<FunctionID,ThreadID,FileName,TimeConsumed,HitCount,FileOffset,StartColumn,EndColumn,CollectiveTime>"))
						{
							functionString=funcImporter.ReadLine();
							while(!functionString.EndsWith(">"))
							{
								myCommand.CommandText= strDebugCodeQry+functionString+",'"+m_func_SessionID+"')";
								myCommand.ExecuteNonQuery ();
								functionString=funcImporter.ReadLine();
								if(functionString==null)
								{
									goto A;//I dare again :-)
								}
							}
						}

						if(functionString.EndsWith("<CalleeFunctionID,CFCalls,CFCollectiveTime,CallerFunctionID,CFSignature,CFThreadID>"))
						{
							functionString=funcImporter.ReadLine();
							while(!functionString.EndsWith(">"))
							{
								myCommand.CommandText= strRefFunctionQry+functionString+",'"+m_func_SessionID+"')";
								myCommand.ExecuteNonQuery ();
								functionString=funcImporter.ReadLine();
								if(functionString==null)
								{
									goto A;//and again.
								}
							}
						}

						functionString=funcImporter.ReadLine();
						continue;
					}

				A:	break;
				}
			}
			catch(Exception exc)
			{
				MessageBox.Show(exc.Message);					
			}
			finally
			{
				if(funcImporter!=null)
				{
					funcImporter.Close();
					funcImporter=null;
				}
				if(myCommand!=null)
				{
					myCommand.Dispose();
					myCommand=null;
				}
			}
		}



		
	}
}
