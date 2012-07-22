using System;
using System.Data;
using System.Data.OleDb; 
using System.Windows.Forms;
using System.Text; 
using System.Runtime.InteropServices;  
using System.ComponentModel; 
namespace PGBusinessLogic
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class BusinessLogic
	{
		[DllImport( "advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		private static extern bool LookupAccountSid(
			[In,MarshalAs(UnmanagedType.LPTStr)] string systemName,
			IntPtr sid,
			[Out,MarshalAs(UnmanagedType.LPTStr)] StringBuilder name,
			ref int cbName,
			StringBuilder referencedDomainName,
			ref int cbReferencedDomainName,
			out int use );

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern bool LookupAccountName(
			[In,MarshalAs(UnmanagedType.LPTStr)] string systemName,
			[In,MarshalAs(UnmanagedType.LPTStr)] string accountName,
			IntPtr sid,
			ref int cbSid,
			StringBuilder referencedDomainName,
			ref int cbReferencedDomainName,
			out int use);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool ConvertSidToStringSid(
			IntPtr sid,
			[In,Out,MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool ConvertStringSidToSid(
			[In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid,
			ref IntPtr sid);

		public BusinessLogic()
		{
			
		}

		public static Exception ModifyStoreHouse(OleDbConnection Con,string QueryToRun,string QueryToValidate)
		{
			if (Con==null)
			{
				return new Exception("Connection is not a valid object"); 				

			}


			////////////Begin Transaction//////////			
			
			OleDbCommand myCommand = new OleDbCommand();
			OleDbTransaction myTrans=null;

			try
			{
				myTrans = Con.BeginTransaction(IsolationLevel.ReadCommitted);						
			
				myCommand.Connection = Con;
				myCommand.Transaction =myTrans;
			}

			catch(Exception ex)
			{			
				if(myTrans!=null)
					myTrans.Rollback(); 
				return ex;
			}

			try
			{
				if(QueryToValidate!=null)
				{
					myCommand.CommandText = QueryToValidate ;
					int iResult=(int)myCommand.ExecuteScalar();
					if(iResult>0)
					{
						throw new Exception("Validation Query Failed.Account is in use.") ;

					}
				}
			}
			catch(Exception ex)
			{
				myTrans.Rollback();	
				return ex;							
			}		
			//////////Validated////////////
			try
			{
				myCommand.CommandText = QueryToRun ;                
				int rAffected=myCommand.ExecuteNonQuery();				
				/*
				if(rAffected==0)
				{
					throw new Exception(" The record for this entry does not exist! Can not update.") ;

				}
				*/

				
			}
			catch(Exception ex)
			{
				myTrans.Rollback();	
				return ex;	
			}

			try
			{
				myTrans.Commit();
			}
			catch(Exception ex)
			{
				return ex;
			}

			return null;
		
		}



		public static Exception PerformMultipleQueriesWithoutValidation(OleDbConnection Con,string[] QueriesToRun)
		{
			if (Con==null || QueriesToRun==null)
			{
				return new Exception("Either Connection or Query Object is null"); 				

			}

			////////////Begin Transaction//////////			
			
			OleDbCommand myCommand = new OleDbCommand();
			OleDbTransaction myTrans=null;

			try
			{
				myTrans = Con.BeginTransaction(IsolationLevel.ReadCommitted);						
			
				myCommand.Connection = Con;
				myCommand.Transaction =myTrans;
			}

			catch(Exception ex)
			{			
				if(myTrans!=null)
					myTrans.Rollback(); 
				return ex;
			}			
			
			try
			{
				for(int x=0;x<QueriesToRun.Length;x++)
				{
					string QueryToRun=QueriesToRun[x];
				
					myCommand.CommandText = QueryToRun ;                
					int rAffected=myCommand.ExecuteNonQuery();				
					if(rAffected==0)
					{
						throw new Exception(" No record affected ! Can not execute.") ;

					}
				}
				
			}
			catch(Exception ex)
			{
				myTrans.Rollback();	
				Exception newEx=new Exception(ex.Message+"\nError Executing Query :- "+myCommand.CommandText) ;
				return newEx;							
			}

			try
			{
				myTrans.Commit();
			}
			catch(Exception ex)
			{
				return ex;
			}

			return null;
		
		}

		
		public static DataSet FlipDataSet(DataSet my_DataSet)
		{
			DataSet ds = new DataSet();
			foreach(DataTable dt in my_DataSet.Tables)
			{
				DataTable table = new DataTable();
				for(int i=0; i<=dt.Rows.Count; i++)
				{
					table.Columns.Add(Convert.ToString(i));
				}
				DataRow r;
				for(int k=0; k<dt.Columns.Count; k++)
				{
					r = table.NewRow();
					r[0] = dt.Columns[k].ToString();
					for(int j=1; j<=dt.Rows.Count; j++)
						r[j] = dt.Rows[j-1][k];

					table.Rows.Add(r);

				}				
				ds.Tables.Add(table);
			}	
		
			return ds;
		}

		public static string GetSid(string name)
		{
			IntPtr _sid = IntPtr.Zero;    //pointer to binary form of SID string.
			int _sidLength = 0;            //size of SID buffer.
			int _domainLength = 0;        //size of domain name buffer.
			int _use;                    //type of object.
			StringBuilder _domain = new StringBuilder();    //stringBuilder for domain name.
			int _error = 0;                
			string _sidString = "";

			//first call of the function only returns the size of buffers (SID, domain name)
			LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
			_error = Marshal.GetLastWin32Error();

			if (_error != 122) //error 122 (The data area passed to a system call is too small) - normal behaviour.
			{
				throw(new Exception(new Win32Exception(_error).Message));
			}
			else
			{
				_domain = new StringBuilder(_domainLength); //allocates memory for domain name
				_sid = Marshal.AllocHGlobal(_sidLength);    //allocates memory for SID
				bool _rc = LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);

				if (_rc == false)
				{
					_error = Marshal.GetLastWin32Error();
					Marshal.FreeHGlobal(_sid);
					throw(new Exception(new Win32Exception(_error).Message));
				}
				else
				{
					// converts binary SID into string
					_rc = ConvertSidToStringSid(_sid, ref _sidString);

					if (_rc == false)
					{
						_error = Marshal.GetLastWin32Error();
						Marshal.FreeHGlobal(_sid);
						throw(new Exception(new Win32Exception(_error).Message));
					}
					else
					{
						Marshal.FreeHGlobal(_sid);
						return _sidString;
					}
				}
			}

		}
		
	}
}

