using System;
using System.Windows.Forms;  
using SharpClient.UI ;
using PSUI;
using System.Xml ;
using PGRptControl; 
using System.Data.OleDb;  
using System.Drawing;
using System.Data ;
using System.Collections ;
using System.Reflection; 
namespace SharpClient
{
	/// <summary>
	/// Summary description for CompareTabPage.
	/// </summary>
	public class CompareTabPage:SharpClient.UI.Controls.TabPage
	{
		private System.ComponentModel.IContainer components;
		private delegateToolbarEvent subscriberEvent;
		private delegateMenuEvent menuSubscriberEvent;
		private string m_session1;
		private string m_session2;
		private string m_profileeName;

		public enum DataFormat
		{
			XML,
			CSV
		}
	
			
		private PGRptControl.PGUserControl MorphReportControl;
		private System.Windows.Forms.SaveFileDialog saveSessionDlg;

		private void SaveData(DataFormat dataFormat)
		{
			try
			{
				if(this.saveSessionDlg==null)
				{
					this.saveSessionDlg = new System.Windows.Forms.SaveFileDialog();			
					this.saveSessionDlg.RestoreDirectory = true;
					this.saveSessionDlg.Title = "Export Comparison Data";
				}

				if(dataFormat==DataFormat.XML)
				{
					saveSessionDlg.RestoreDirectory=true;
					saveSessionDlg.Filter ="XML Files(*.xml)|*.xml";
					if(saveSessionDlg.ShowDialog ()==DialogResult.OK)
					{
						if(this.MorphReportControl.ReportGrid.DataSource!=null)
						{
							((DataSet)this.MorphReportControl.ReportGrid.DataSource).WriteXml(saveSessionDlg.FileName) ;
						}
						else
						{
							throw new Exception("Currently there is no data to save"); 
						}
					}	
					else
					{
						throw new Exception("No file selected"); 
					}
				}
				else if(dataFormat==DataFormat.CSV)
				{
					saveSessionDlg.RestoreDirectory=true;
					saveSessionDlg.Filter ="CSV Files(*.csv)|*.csv";

					if(this.MorphReportControl.ReportGrid.DataSource==null)
					{
						throw new Exception("Currently there is no data to save"); 
					}

					string fileToSave=Application.StartupPath+ @"\Sessions\SessionData_"+DateTime.Now.ToLongDateString()+".csv";

					foreach(DataTable table in ((DataSet)this.MorphReportControl.ReportGrid.DataSource).Tables)
					{
						saveSessionDlg.FileName=table.TableName+".csv" ;

						if(saveSessionDlg.ShowDialog ()==DialogResult.OK)
						{
							fileToSave=saveSessionDlg.FileName;
							System.IO.StreamWriter strWriter=null;

							try
							{
								strWriter=	new System.IO.StreamWriter(fileToSave,false,System.Text.Encoding.ASCII);    
								for(int i=0;i<table.Columns.Count;i++)  
								{	
									DataColumn col=table.Columns[i];
									if(i==table.Columns.Count-1)
									{
										strWriter.Write(col.ColumnName);
									}
									else
									{
										strWriter.Write(col.ColumnName+",");
									}
								}
								strWriter.Write("\n"); 

								foreach(DataRow row in table.Rows)  
								{	
									for(int i=0;i<table.Columns.Count;i++)  
									{
										DataColumn col=table.Columns[i];
										if(i==table.Columns.Count-1)
										{
											strWriter.Write(Convert.ToString(row[col]));
										}
										else
										{
											strWriter.Write(Convert.ToString(row[col])+",");
										}
									}

									strWriter.Write("\n"); 
								}								
							}
							catch(Exception ex)
							{
								if(strWriter!=null)
								{
									strWriter.Close();
									strWriter=null;									
								}
								throw ex;
							}
							finally
							{
								if(strWriter!=null)
								{
									strWriter.Close();
									strWriter=null;									
								}
							}
						}
						else
						{
							throw new Exception("Save action cancelled"); 
						}
					}
					


				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Unable to save session.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;
			}
		}


		private void InitializeComponent()
		{
			// 
			// CompareTabPage
			// 
			this.Dock = System.Windows.Forms.DockStyle.Fill;

		}
	
		public CompareTabPage(string profileeName,string firstSession,string secondSession)
		{
			this.m_session1=firstSession;
			this.m_session2=secondSession; 
			this.m_profileeName=profileeName; 
			Init();
			subscriberEvent=new delegateToolbarEvent(ToolBarButton_Clicked); 
			menuSubscriberEvent=new delegateMenuEvent(MenuItem_Clicked); 			
		}

		public System.Data.DataSet GetResultSetCopy()
		{
			try
			{
				System.Data.DataSet dsTemp=((System.Data.DataSet)(this.MorphReportControl.ReportGrid.DataSource)).Copy();
				dsTemp.DataSetName =Convert.ToString(this.Tag).Replace("∆","-") .Split(new char[]{':'},2)[1] ; 
				return  dsTemp;   
			}
			catch
			{
			}
			return null;
		}


		private void Init()
		{
			this.Title = m_session1+" ∆ " +m_session2;
			this.Tag= m_profileeName+":"+m_session1+" ∆ " +m_session2;
			this.VisibleChanged += new System.EventHandler(this.CompareTabPage_VisibleChanged);

			this.MorphReportControl=new PGUserControl();
			MorphReportControl.TotallingEnabled = true;
			MorphReportControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;					
			MorphReportControl.Dock=DockStyle.Fill; 
			MorphReportControl.Location = new System.Drawing.Point(2, 35);
			MorphReportControl.DBCon=new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;"); 
			MorphReportControl.ConfigFile =Application.StartupPath+@"\MemoryReportDefs\MemoryComparison.xml";

			if(MorphReportControl.Parent!=this)
			{
				this.Controls.Add(MorphReportControl);
			}
			MorphReportControl.Search_Clicked +=new Search_ClickedHandler(MorphReportControl_Search_Clicked);
			MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(MorphReportControl_SelectedIndex_Changed); 

			
			MorphReportControl.FixedPropertyTable["Numerator-SessionID"]= m_session1;	
			MorphReportControl.FixedPropertyTable["Denominator-SessionID"]= m_session2;	
			MorphReportControl.Show();		
	
		}


		private void MenuItem_Clicked(object sender)
		{
			try
			{
				System.Windows.Forms.MenuItem m=(System.Windows.Forms.MenuItem)sender;
				if(m==SharpClientForm.scInstance.menuGraph)
				{
					CreateGraph();
					return;
				}
				else if(m==SharpClientForm.scInstance.menuPrintPreview || m==SharpClientForm.scInstance.menuWebPreview)
				{
					CreatePreview();
					return;
				}
				else if(m==SharpClientForm.scInstance.menuExport_XML )
				{
					SaveData(DataFormat.XML); 
					return;
				}
				else if(m==SharpClientForm.scInstance.menuExport_CSV )
				{
					SaveData(DataFormat.CSV)  ;
					return;
				}

			}
			catch(Exception ex)
			{
				MessageBox.Show("Unable to perform the required action.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;
			}  
		}

		private void CreateGraph()
		{
			GraphTabPage graphPage=null;				
			try
			{
				graphPage=new GraphTabPage();
				graphPage.Title="Graph "+ this.Title  ;  
 				graphPage.SourceData=this.GetResultSetCopy();
				SharpClientForm.scInstance.sharpClientMDITab.TabPages.Add(graphPage);
				SharpClientForm.scInstance.sharpClientMDITab.SelectedTab =graphPage ;
				graphPage.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show("An Error occured while drawing the graph.\n"+ex.Message,"Error!") ;
				if(graphPage!=null)
				{
					foreach(Control control in graphPage.Controls)
					{
						control.Enabled=false; 
					}
					SharpClient.SharpClientForm.scInstance.sharpClientMDITab.TabPages.Remove(this);
				}
			}
		}

		private void ToolBarButton_Clicked(object arg)
		{
			System.Windows.Forms.ToolBarButtonClickEventArgs e=null;
			try
			{
				e=(System.Windows.Forms.ToolBarButtonClickEventArgs)arg;
			}
			catch{}
			if(e==null)
			{
				return;
			}

			if(e.Button.Text=="CG")
			{
				CreateGraph();
			}
			else if(e.Button.Text=="WP")  
			{
				CreatePreview();
				return;
			}

		}

		private void CreatePreview()
		{	
			PrintTabPage printPage=null;
			try
			{
				printPage=new PrintTabPage();
				printPage.Title="Preview "+this.Title ;  
 				printPage.SourceData=this.GetResultSetCopy();
				SharpClientForm.scInstance.sharpClientMDITab.TabPages.Add(printPage);
				SharpClientForm.scInstance.sharpClientMDITab.SelectedTab =printPage ;
				printPage.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show("An Error occured while creating preview.\n"+ex.Message,"Error!") ;
				if(printPage!=null)
				{
					foreach(Control control in printPage.Controls)
					{
						control.Enabled=false; 
					}
					SharpClient.SharpClientForm.scInstance.sharpClientMDITab.TabPages.Remove(this);      

				}
			}
		}

		
		protected override void Dispose( bool disposing )
		{
			if(MorphReportControl!=null)
			{
				if(MorphReportControl.DBCon!=null)
				{
					MorphReportControl.DBCon.Close();
					MorphReportControl.DBCon.Dispose();
					MorphReportControl.DBCon=null;
					try
					{
						MorphReportControl.Dispose();
					}
					catch{}				
 
				}
			}
			
			if( disposing )
			{			

				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public PGRptControl.PGUserControl GetReportControl()
		{
			return this.MorphReportControl ;
		}


		private void CompareTabPage_VisibleChanged(object sender, System.EventArgs e)
		{
			
			
			if(this.Visible==true)
			{
				SharpClientForm.scInstance.SubscribeToControl("CG",this.subscriberEvent);  
				SharpClientForm.scInstance.SubscribeToControl("WP",this.subscriberEvent);  
				SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuGraph,menuSubscriberEvent);
				SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuPrintPreview,menuSubscriberEvent);
				SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuWebPreview,menuSubscriberEvent);
				SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuExport_CSV,menuSubscriberEvent);
				SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuExport_XML,menuSubscriberEvent);				
 

			}
			else
			{
				SharpClientForm.scInstance.UnsubscribeControl("CG"); 
				SharpClientForm.scInstance.UnsubscribeControl("WP"); 

				SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuGraph);
				SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuPrintPreview);
				SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuWebPreview);
				SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuExport_CSV);
				SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuExport_XML);
					
			}
				
			
		}

		private void MorphReportControl_Search_Clicked(long RowsReturned, string MasterQuery)
		{
			if(SharpClientForm.CanShowQry())
			{
				MessageBox.Show(MasterQuery);  
			}
		}

		private int getColumnIndex(string colName)
		{
			return ((DataSet)(MorphReportControl.ReportGrid.DataSource )).Tables[0].Columns[colName].Ordinal;  
		}		

		private void MorphReportControl_SelectedIndex_Changed(object sender, EventArgs e)
		{
			switch(Convert.ToString(MorphReportControl.CriteriaBox.SelectedItem).ToUpper())
			{
				case "COMPARE MEMORY BY OBJECT ALLOCATION":
				{
					DisplayAllocationData();
					break;
				}
				case "COMPARE MEMORY BY OBJECTS REFERENCED (IF ANY)":
				{
					DisplayReferencedData();
					break;
				}
				case "COMPARE MEMORY BY OBJECTS REFERENCING (IF ANY)":
				{
					DisplayReferencingData();
					break;
				}
			}
		}

		private void DisplayReferencingData()
		{
			try
			{

				OleDbCommand cmd=null;
				System.Text.StringBuilder stackString=new System.Text.StringBuilder("") ;
				string detailString="";
				OleDbDataReader reader=null;
				try
				{
					
					int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
					string objID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ObjectID")]);									
					string objClass=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object-Class")]);									
					string objSize=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Size(Bytes)")]);									
					
					detailString="Object-Class: "+objClass+"\n"+"Object-Size: "+objSize+" (bytes)\n"+"ObjectID: "+objID+", is referenced by :-";

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					cmd=new OleDbCommand("Select ObjectID,ObjectName,ObjectSize,IsRootObject from LiveObjects where SessionID='"+m_session1 +"' and ObjectID in (Select distinct ParentObjectID from RefObjects where SessionID='"+m_session1+"' and RefObjectID='"+objID+"') "); 
					cmd.Connection =MorphReportControl.DBCon ;
					if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
					{
						cmd.Connection.Open();  
					}

					reader=cmd.ExecuteReader();
					if(reader.HasRows)					
					{
						while(reader.Read())
						{
							stackString.Append(Convert.ToString( reader.GetValue(0))+"^" +Convert.ToString( reader.GetValue(1))+"^"+Convert.ToString( reader.GetValue(2))+"^"+Convert.ToString( reader.GetValue(3))+"|");
   
						}						
					}
										
				}
				catch(Exception ex)
				{					
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;						
				}
				finally
				{
					if(reader!=null)
					{
						try
						{
							reader.Close();							
						}
						catch{}
						reader=null;
					}
					if(cmd!=null)
					{
						try
						{
							cmd.Connection.Close();  
							cmd.Dispose();
						}
						catch{}					
						cmd=null;
					}
					
				}
				SharpClientForm.scInstance.AddStackControlTab("Referencing Objects",m_session1,8,SharpClient.UI.Docking.State.DockBottom,"ObjectID|Object-Class|Object-Size(Byte)|Root-Object",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}
		private void DisplayReferencedData()
		{
			try
			{

				OleDbCommand cmd=null;
				System.Text.StringBuilder stackString=new System.Text.StringBuilder("") ;
				string detailString="";
				OleDbDataReader reader=null;
				try
				{
					
					int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
					string objID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ObjectID")]);				
					string objClass=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object-Class")]);									
					string objSize=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Size(Bytes)")]);									

					detailString="Object-Class: "+objClass+"\n"+"Object-Size: "+objSize+" (bytes)\n"+"ObjectID: "+objID+", refers to :- ";

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					cmd=new OleDbCommand("Select RefObjectID,RefObjectName,RefObjectCount,RefObjectSize from RefObjects where SessionID='"+m_session1+"' and ParentObjectID='"+objID+"' "); 
					cmd.Connection =MorphReportControl.DBCon ;
					if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
					{
						cmd.Connection.Open();  
					}

					reader=cmd.ExecuteReader();
					if(reader.HasRows)					
					{
						while(reader.Read())
						{
							stackString.Append(Convert.ToString( reader.GetValue(0))+"^" +Convert.ToString( reader.GetValue(1))+"^"+Convert.ToString( reader.GetValue(2))+"^"+Convert.ToString( reader.GetValue(3))+"|");
   
						}						
					}
										
				}
				catch(Exception ex)
				{					
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;						
				}
				finally
				{
					if(reader!=null)
					{
						try
						{
							reader.Close();							
						}
						catch{}
						reader=null;
					}
					if(cmd!=null)
					{
						try
						{
							cmd.Connection.Close();  
							cmd.Dispose();
						}
						catch{}					
						cmd=null;
					}
					
				}
				SharpClientForm.scInstance.AddStackControlTab("Referenced Objects",m_session1,7,SharpClient.UI.Docking.State.DockTop,"ObjectID|Object-Class|Reference-Count|Object-Size(Byte)",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}

		private void DisplayAllocationData()
		{
			try
			{

				OleDbCommand cmd=null;
				System.Text.StringBuilder stackString=new System.Text.StringBuilder("N:A") ;
				string detailString="";
				try
				{
					int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
					string objID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ObjectID")]);				
					string objClass=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object-Class")]);				
					string objThreadID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Thread ID")]);				
					string objSize=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Size(Bytes)")]);									
					string objAge=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Age")]);
					string objGen=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Generation")]);

					detailString="Object-Class: "+objClass+"\n"+"Object-Size: "+objSize+" bytes\n"+"ObjectID: "+objID+"\n"+"ThreadID: "+objThreadID+"  Object Age: "+objAge+ "\nObject Generation: "+objGen;

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					cmd=new OleDbCommand("Select top 1 AllocFunctionIDStack from ObjectAllocation where SessionID='"+m_session1+"' and ObjectID='"+objID+"' "); 
					cmd.Connection =MorphReportControl.DBCon ;
					if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
					{
						cmd.Connection.Open();  
					}
					string stackIDString=Convert.ToString(cmd.ExecuteScalar()).Trim()  ;
					string[] stackIDs=stackIDString.Split (new char[]{'|'}); 
					stackString.Replace("N:A","");
					for(int x=0 ;x<stackIDs.Length;x++)
					{
						cmd.CommandText="SELECT top 1 AllocFunctionName from AllocationFunction where AllocFunctionID='"+stackIDs[x]+"' and SessionID='"+m_session1+"'";
						string allocFunc=Convert.ToString(cmd.ExecuteScalar()).Trim()  ;
						stackString.Append(allocFunc+"|"); 
					}
				}
				catch(Exception ex)
				{					
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;						
				}
				finally
				{
					if(cmd!=null)
					{
						try
						{
							cmd.Connection.Close();  
							cmd.Dispose();
						}
						catch{}					
						cmd=null;
					}
				}
				
				SharpClientForm.scInstance.AddStackControlTab("Allocation-Stack",m_session1,6,SharpClient.UI.Docking.State.DockRight,"Allocation-Stack",stackString.ToString() ,detailString,-1,false,false);
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		
		}

	}
}
