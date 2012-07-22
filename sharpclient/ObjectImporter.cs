using System;
using System.Data;
using System.Data.OleDb;  
using PGBusinessLogic;
using System.Windows.Forms ;
 
namespace SharpClient
{
	/// <summary>
	/// Summary description for ObjectImporter.
	/// </summary>
	/// 


	public class ObjectImporter:IDisposable
	{
		private OleDbConnection con;		

//		private bool WANT_OBJECT_NAME_ONLY ;
//		private bool WANT_OBJECT_COUNT ;
//		private bool WANT_OBJECT_SIZE ;
//		private bool WANT_REFERENCED_OBJECTS ;
//		private bool WANT_OBJECT_ALL_DATA ;	

		private int m_dwObjectFilter;

		private bool m_obj_GCBeforeOC;		
		private bool m_obj_ObjectPassthrough;
		private string m_obj_ObjectClassFilter;
		private string m_obj_ObjectModuleFilter;
		private string m_obj_ProfileeAppName;
		private string m_obj_SessionID;		

		private bool bIsAlreadyLoaded;
		private string m_SessionFile;
	
		private bool m_bCleanCache;
		private bool m_bContinueOnCacheError;		
		
		public ObjectImporter(string sessionFile,bool bCleanCache,string connectionString,bool bContinueOnCacheError)
		{
//			WANT_OBJECT_NAME_ONLY =false;
//			WANT_OBJECT_COUNT =false;
//			WANT_OBJECT_SIZE =false;
//			WANT_REFERENCED_OBJECTS =false;
//			WANT_OBJECT_ALL_DATA =false;	

			m_SessionFile=sessionFile;

			bIsAlreadyLoaded=false;

			m_bCleanCache=bCleanCache;
			m_bContinueOnCacheError=bContinueOnCacheError;

			m_dwObjectFilter=0; 
			m_obj_GCBeforeOC=true;			
			m_obj_ObjectPassthrough=true;
			m_obj_ObjectClassFilter=null;
			m_obj_ObjectModuleFilter=null;
			m_obj_ProfileeAppName=null;
			m_obj_SessionID=null;			


			con=new OleDbConnection(connectionString); 
			con.Open() ;

//			if(m_bCleanCache)
//				DeleteCache();
			
		}


		public ObjectData getObjectData
		{
			get
			{
				return new ObjectData(m_dwObjectFilter,m_obj_ObjectClassFilter,m_obj_ObjectPassthrough,m_obj_GCBeforeOC);    
			}
			
		}		

		private void DeleteCache()
		{		
			if(con==null)
				throw new Exception("The cache could not be retrieved."); 
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from LiveObjects",null);
			}
			catch(Exception ex)
			{	
				if(!m_bContinueOnCacheError )
					throw new Exception("Live Objects Cache could not be cleared.\n"+ ex.Message);					
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from RefObjects",null);
			}
			catch(Exception ex)
			{
				if(!m_bContinueOnCacheError )
					throw new Exception("Referenced Objects cache could not be cleared.\n"+ ex.Message);					
			}

			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from ObjectAllocation",null);
			}
			catch(Exception ex)
			{
				if(!m_bContinueOnCacheError )
					throw new Exception("Allocation Data cache could not be cleared.\n"+ ex.Message);					
			}

			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(con,"Delete from AllocationFunction",null);
			}
			catch(Exception ex)
			{
				if(!m_bContinueOnCacheError )
					throw new Exception("Allocation Data cache could not be cleared.\n"+ ex.Message);					
			}
			
		}

		private void CleanOff()
		{
			if(con!=null)
			{
				con.Close();
				con.Dispose(); 
			}					

		}

		public static void DeleteCache(string connectionString,bool bContinueOnCacheError)
		{
			OleDbConnection myCon=new OleDbConnection(connectionString);
			myCon.Open ();
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from LiveObjects",null);
			}
			catch(Exception ex)
			{	
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
				{					
					throw new Exception("Live Objects Cache could not be cleared.\n"+ ex.Message);					
				}
			}
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from RefObjects",null);
			}
			catch(Exception ex)
			{
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
					throw new Exception("Referenced Objects cache could not be cleared.\n"+ ex.Message);					
			}			
			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from ObjectAllocation",null);
			}
			catch(Exception ex)
			{	
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
				{					
					throw new Exception("Object Allocation Cache could not be cleared.\n"+ ex.Message);					
				}
			}

			try
			{
				PGBusinessLogic.BusinessLogic.ModifyStoreHouse(myCon,"Delete from AllocationFunction",null);
			}
			catch(Exception ex)
			{	
				if(myCon!=null)
					try	{myCon.Close() ;}	
					catch{}
				myCon=null;

				if(!bContinueOnCacheError )
				{					
					throw new Exception("Object Allocation Cache could not be cleared.\n"+ ex.Message);					
				}
			}

			if(myCon!=null)
				try	{myCon.Close() ;}	
				catch{}
			myCon=null;			
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
			System.IO.StreamReader objImporter=null;
				
			//do not dispose
			objImporter=new System.IO.StreamReader(m_SessionFile,System.Text.Encoding.ASCII);			
   
			string daString="";
						
			while(!(daString+=objImporter.ReadLine()).EndsWith("</ObjectDataSet>"))
			{
			}
		
			DataSet objectDS=new DataSet ();	
			string excString=null;
			try
			{
				objectDS.ReadXml(new System.IO.StringReader(daString),XmlReadMode.Auto); 		

				if(objectDS.Tables["DataAttributes"].Columns.Contains("Version") && (Convert.ToString(objectDS.Tables["DataAttributes"].Rows[0]["Version"])=="1.3")) 
				{

				}
				else
				{
					throw new Exception("The file version you are trying to open is obsolete.\nYou may need to re-create the profiling session."); 
				}

				m_obj_SessionID=Convert.ToString(objectDS.Tables["DataAttributes"].Rows[0]["SessionID"]); 
				OleDbCommand bVerifyCommand = new OleDbCommand("SELECT COUNT(*) FROM LIVEOBJECTS WHERE SESSIONID='"+m_obj_SessionID+"'",con);	
				bIsAlreadyLoaded=( Convert.ToInt32(bVerifyCommand.ExecuteScalar()) > 0 )?true:false ;				
				try
				{
					bVerifyCommand.Dispose(); 
				}
				catch{}

				
				m_dwObjectFilter =Convert.ToInt32 (objectDS.Tables["DataAttributes"].Rows[0]["ObjectFlag"]);   	
				m_obj_GCBeforeOC=(Convert.ToInt32(objectDS.Tables["DataAttributes"].Rows[0]["GCBeforeOC"]))==0?false:true;
				m_obj_ObjectClassFilter=Convert.ToString(objectDS.Tables["DataAttributes"].Rows[0]["ObjectClassFilter"]);   	
				//m_obj_ObjectModuleFilter=Convert.ToString(objectDS.Tables["DataAttributes"].Rows[0]["ObjectModuleFilter"]);   	
				m_obj_ObjectPassthrough=(Convert.ToInt32(objectDS.Tables["DataAttributes"].Rows[0]["ObjectPassthrough"]))==0?false:true;
				m_obj_ProfileeAppName=Convert.ToString(objectDS.Tables["DataAttributes"].Rows[0]["ProfileeAppName"]);   	
				
			}
			catch(Exception ex)
			{
				excString="Data Attributes import for [Object] tables failed.\n"+ex.Message  ;								
			}
			finally
			{
				if(objectDS!=null)
				{
					objectDS.Relations.Clear();
					objectDS.Tables.Clear();
					objectDS.Clear();
					objectDS.Dispose();
				}
				if(objImporter!=null)
				{
					objImporter.Close();
					objImporter=null;
				}
				if(excString!=null)
					throw new Exception(excString);
			}

			//			WANT_OBJECT_NAME_ONLY =( ((m_dwObjectFilter) & ((int)OBJECT_FILTER.OF_OBJECT_NAME_ONLY))==(int)OBJECT_FILTER.OF_OBJECT_NAME_ONLY );
			//			WANT_OBJECT_COUNT= ( ((m_dwObjectFilter) &((int)OBJECT_FILTER.OF_OBJECT_COUNT))==(int)OBJECT_FILTER.OF_OBJECT_COUNT );
			//			WANT_OBJECT_SIZE= ( ((m_dwObjectFilter) &((int)OBJECT_FILTER.OF_OBJECT_SIZE))==(int)OBJECT_FILTER.OF_OBJECT_SIZE );
			//			WANT_REFERENCED_OBJECTS= ( ((m_dwObjectFilter) &((int)OBJECT_FILTER.OF_REFERENCED_OBJECTS))==(int)OBJECT_FILTER.OF_REFERENCED_OBJECTS );
			//			WANT_OBJECT_ALL_DATA =( ((m_dwObjectFilter) &((int)OBJECT_FILTER.OF_OBJECT_ALL_DATA))==(int)OBJECT_FILTER.OF_OBJECT_ALL_DATA );		
		}
		
		public void updateObjects()
		{					
			updatePrivateObjects();
			
		}

		private void updatePrivateObjects()
		{
			if(bIsAlreadyLoaded)
				return;

			System.IO.StreamReader objImporter=null;		
			OleDbCommand myCommand=null;
			try
			{
				objImporter=new System.IO.StreamReader(m_SessionFile,System.Text.Encoding.ASCII);
				while(!objImporter.ReadLine().EndsWith("<ObjectID,ObjectName,ObjectSize,ObjectCount,IsRootObject>"))
				{
				}      
				string objectString="";
				myCommand = new OleDbCommand(null,con);	
				string strObjectQry="INSERT INTO LIVEOBJECTS(ObjectID,ObjectName,ObjectSize,ObjectCount,IsRootObject,SessionID) VALUES(";
                        
				objectString=objImporter.ReadLine();
				while(!(objectString.EndsWith("<ObjectID,ThreadID,AllocFunctionID,AllocFunctionIDStack,ObjectAge,ObjectGeneration>"))) 
				{
					try
					{						
						myCommand.CommandText =strObjectQry+objectString+",'" + m_obj_SessionID +"')";
						myCommand.ExecuteNonQuery();
					}
					catch
					{}
					objectString=objImporter.ReadLine();
					if(objectString==null)
					{
						break;
					}					 
				}

				///////////////////////////////////////////////////////////////
				///
				string strObjectAllocQry="INSERT INTO OBJECTALLOCATION(ObjectID,ThreadID,AllocFunctionID,AllocFunctionIDStack,ObjectAge,ObjectGeneration,SessionID) VALUES(";
				objectString=objImporter.ReadLine();				
				while(!(objectString.EndsWith("<AllocFunctionID,AllocFunctionName>")))
				{
					try
					{
						myCommand.CommandText =strObjectAllocQry+objectString+",'" + m_obj_SessionID +"')";
						myCommand.ExecuteNonQuery();
					}
					catch
					{}

					objectString=objImporter.ReadLine();
					if(objectString==null)
					{
						break;
					}
				}

				string strObjectFunctionAllocQry="INSERT INTO ALLOCATIONFUNCTION(AllocFunctionID,AllocFunctionName,SessionID) VALUES(";
				objectString=objImporter.ReadLine();
				while(!(objectString.EndsWith("<RefObjectID,ParentObjectID,RefObjectName,RefObjectCount,RefObjectSize>")))
				{
					try
					{
						myCommand.CommandText =strObjectFunctionAllocQry+objectString+",'" + m_obj_SessionID +"')";
						myCommand.ExecuteNonQuery();
					}
					catch
					{}
					objectString=objImporter.ReadLine();
					if(objectString==null)
					{
						break;
					}
				}


#warning "Should also collect all RefObjects' Size and Count irrespective of Filter(Filter should ideally apply only to Parent objects)"

				string strRefObjectQry="INSERT INTO REFOBJECTS(RefObjectID,ParentObjectID,RefObjectName,RefObjectCount,RefObjectSize,SessionID) VALUES(";
				objectString=objImporter.ReadLine();				
				if(objectString!=null)
				{
					while(objectString!=null)
					{
						try
						{
							myCommand.CommandText =strRefObjectQry+objectString+",'" + m_obj_SessionID +"')";
							myCommand.ExecuteNonQuery();
						}
						catch
						{}

						objectString=objImporter.ReadLine();
						if(objectString==null)
						{
							break;
						}
					}	
				}
				
			}
			catch(Exception exc)
			{
				MessageBox.Show(exc.Message);					
			}
			finally
			{
				if(objImporter!=null)
				{
					objImporter.Close();
					objImporter=null;
				}
				if(myCommand!=null)
				{
					myCommand.Dispose();
					myCommand=null;
				}
			}
		}

		public string profileeName
		{
			get
			{
				return m_obj_ProfileeAppName;
			}
		}

		public string profileeSessionName
		{
			get
			{
				return m_obj_SessionID;
			}
		}
	}
}
