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
	/// Summary description for SharpClientTabPage.
	/// </summary>
	/// 	
	public class SharpClientTabPage:SharpClient.UI.Controls.TabPage
	{	
		public enum DataFormat
		{
			XML,
			CSV
		}

		private System.Threading.Thread m_tThread; 
		private string m_fileName;				
		private Configurator cnfgSettings; 		
		private FunctionData funcObj;
		public PSUI.PSPanelGroup AnalysisPanelGroup;
		private PSUI.PSPanel settingsPanel;
		private PSUI.PSPanel analysisPanel;		
		private System.Windows.Forms.TreeView ReportsNavigation;		
		private System.ComponentModel.IContainer components;
		private ObjectData  objObj;		
		private PGRptControl.PGUserControl MorphReportControl;
		private string m_Session;
		private ArrayList fileNameArray;
		delegateToolbarEvent subscriberEvent;
		delegateMenuEvent menuSubscriberEvent;
		private System.Windows.Forms.SaveFileDialog saveSessionDlg;
		private System.Windows.Forms.OpenFileDialog openSrcFileDlg;
		private DataTable srcTable;
		bool bIsDirty=true;
		private readonly bool bIsMemorySession;


		public bool IsMemorySession
		{
			get
			{
				
				return bIsMemorySession;
				
			}
		}

		public SharpClientTabPage(string title,string fileName )
		{
			this.Title =title;
			this.m_fileName=fileName;

			if(fileName.ToLower().EndsWith(".fxml"))
			{
				bIsMemorySession=false;
				m_tThread=new System.Threading.Thread(new System.Threading.ThreadStart(functionProc));  				
			}
			else if(fileName.ToLower().EndsWith(".oxml"))
			{
				bIsMemorySession=true;
				m_tThread=new System.Threading.Thread(new System.Threading.ThreadStart(objectProc));  			

			}
			else
			{
				bIsMemorySession=false;
				return;
			}
			if(m_tThread!=null)
			{
				m_tThread.ApartmentState =System.Threading.ApartmentState.MTA   ;				
				this.VisibleChanged+=new EventHandler(SharpClientTabPage_VisibleChanged); 			
			}
			
			this.EnabledChanged +=new EventHandler(SharpClientTabPage_EnabledChanged);
			subscriberEvent=new delegateToolbarEvent( Toolbarbutton_Clicked);	
			menuSubscriberEvent=new delegateMenuEvent(MenuItem_Clicked); 
			m_Session="SessionID";
			fileNameArray=new ArrayList(); 		
				
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
				if(m==SharpClientForm.scInstance.menuPrintPreview || m==SharpClientForm.scInstance.menuWebPreview)
				{
					CreatePreview();
					return;
				}

				if(m==SharpClientForm.scInstance.menuExport_XML )
				{
					SaveData(DataFormat.XML); 
					return;
				}
				if(m==SharpClientForm.scInstance.menuExport_CSV )
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

		private void SaveData(DataFormat dataFormat)
		{
			try
			{				
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

		private void CreateGraph()
		{
			GraphTabPage graphPage=null;
			try
			{
				graphPage=new GraphTabPage();
				graphPage.Title="Graph "+this.Title;  
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

		private void Toolbarbutton_Clicked(object arg)
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

			if(e.Button.Text=="TS")
			{
				if(this.Controls.Contains(AnalysisPanelGroup)  )
				{
					if(this.settingsPanel.Visible==false)
					{
						this.settingsPanel.Visible=true;  
						this.analysisPanel.Height =540;
					}
					else
					{
						this.settingsPanel.Visible=false;  
						this.analysisPanel.Height =580;						
					}
				}
				else
				{					
					System.Windows.Forms.Form cnfgForm=new Form();
					cnfgForm.AutoScaleBaseSize = new System.Drawing.Size(5, 13);					
					cnfgForm.BackColor = System.Drawing.SystemColors.AppWorkspace;
					cnfgForm.ClientSize = new System.Drawing.Size(696,458);	
					cnfgForm.Icon=SharpClientForm.scInstance.Icon;   				
					cnfgForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle ;
					cnfgForm.MaximizeBox = false;
					cnfgForm.MinimizeBox = false;
					cnfgForm.Name = "cnfgForm";
					cnfgForm.ShowInTaskbar = false;
					cnfgForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
					cnfgForm.Text = "Configurator settings for "+this.Title.Replace(":"," : ") ;
					cnfgForm.Controls.Add(cnfgSettings);	
					cnfgForm.Height=410; 			
					cnfgSettings.Dock=DockStyle.Fill;					
					cnfgForm.ShowDialog(this);
					
				}
			}
			else if(e.Button.Text=="CG")
			{
				CreateGraph();				
				return;
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
				printPage.Title="Preview "+this.Title;  
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
		

		private void functionProc()
		{	
			this.Cursor=Cursors.WaitCursor ;
			this.Enabled=false;
			Application.DoEvents();  

			FunctionImporter funImporter=null;
			try
			{	
				funImporter=new FunctionImporter( m_fileName,true,"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;",true);
				funImporter.loadSession();
				funImporter.updateFunctions();
				this.funcObj=new FunctionData(funImporter.getFunctionData); 								
			}

			catch(Exception ex)
			{
				this.Cursor=Cursors.Arrow; 
				Application.DoEvents() ; 
				MessageBox.Show("Session could not be loaded.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				this.Title ="Error!";
				try
				{
					SharpClientForm.scInstance.sharpClientMDITab.TabPages.Remove(this);    
				}
				catch{}
			}
			finally
			{
				if(this.Cursor==Cursors.WaitCursor)  
				{
					this.Cursor=Cursors.Arrow; 
					this.Enabled=true;
					PruneReportTree(true);					
					ReportsNavigation.ExpandAll(); 
					Application.DoEvents();  
					
				}
				if(funImporter!=null)
				{	
					this.Title =funImporter.profileeName+":"+ funImporter.profileeSessionName; 
					m_Session=funImporter.profileeSessionName;
					funImporter.Dispose();
					funImporter=null;
				}
				
			
			}			

		}
		private void PruneReportTree(bool bFunctionProfiling)
		{
			try
			{
				if(bFunctionProfiling)
				{
					if(!cnfgSettings.WANT_NO_FUNCTION_CALLEE_INFORMATION || cnfgSettings.WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS || cnfgSettings.WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME)
					{
						//Do Nothing
					}
					else
					{
						TreeNode node=GetNode(ReportsNavigation   ,"View Parent Function Information");
						if(node !=null)
						{
							node.Remove(); 
						}
						node=null;
						node=GetNode(ReportsNavigation   ,"View Children Function Information");
						if(node !=null)
						{
							node.Remove(); 
						}
					}
					if(cnfgSettings.WANT_FUNCTION_CODE_VIEW)
					{
						//Do Nothing
					}
					else
					{
						TreeNode node=GetNode(ReportsNavigation   ,"Code Analysis");
						if(node !=null)
						{
							node.Remove(); 
						}
						
					}
					if(cnfgSettings.WANT_FUNCTION_EXCEPTIONS)
					{
						//Do Nothing
					}
					else
					{
						TreeNode node=GetNode(ReportsNavigation   ,"Exception Analysis");
						if(node !=null)
						{
							node.Remove(); 
						}
						
					}
				}
				else //Object Profiling
				{
					if(cnfgSettings.WANT_REFERENCED_OBJECTS )
					{
						//Do Nothing
					}
					else
					{
						TreeNode node=GetNode(ReportsNavigation   ,"Memory Analysis(Object-References)");
						if(node !=null)
						{
							node.Remove(); 
						}
						 
					}

					if(cnfgSettings.WANT_OBJECT_ALLOCATION_DATA)
					{
						//Do Nothing
					}
					else
					{
						TreeNode node=GetNode(ReportsNavigation   ,"Memory Analysis(Object-Allocation)");
						if(node !=null)
						{
							node.Remove(); 
						}	 
						
					}

				}
			}
			catch{}
		}

		private TreeNode  GetNode(object  treeControl,string nodeText)
		{
			if(treeControl==null)
			{
				return null;
			}
			else if(treeControl.GetType() ==typeof(TreeView))
			{
				TreeView treeView=treeControl as TreeView;
				foreach (TreeNode node in treeView.Nodes)
				{
					if(node.Text.Trim().ToUpper() ==nodeText.Trim().ToUpper())
					{
						return  node;
					}
					else
					{
						TreeNode childNode=GetNode(node as object,nodeText);
						if(childNode!=null)
						{
							return childNode;
						}
					}
				}
				return null;
			}
			else if(treeControl.GetType() ==typeof(TreeNode))
			{
				TreeNode treeNode=treeControl as TreeNode;
				if(treeNode.Text.Trim().ToUpper() ==nodeText.Trim().ToUpper())
				{
					return treeNode;
				}
				foreach (TreeNode node in treeNode.Nodes)
				{
					if(node.Text.Trim().ToUpper() ==nodeText.Trim().ToUpper())
					{
						return  node;
					}
					else
					{
						TreeNode childNode=GetNode(node as object,nodeText);
						if(childNode!=null)
						{
							return childNode;
						}
					}
				}
			}
			
			return null;	
			

		}
		private void objectProc()
		{	
			this.Cursor=Cursors.WaitCursor ;
			this.Enabled=false;
			Application.DoEvents();  

			ObjectImporter objImporter=null;
			try
			{				
				objImporter=new ObjectImporter( m_fileName,true,"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;",true);
				objImporter.loadSession();
				objImporter.updateObjects();
				this.objObj=new ObjectData( objImporter.getObjectData);  				
 
			}
			catch(Exception ex)
			{
				this.Cursor=Cursors.Arrow; 
				Application.DoEvents() ; 
				MessageBox.Show("Session could not be loaded.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				this.Title ="Error!";
				try
				{
					SharpClientForm.scInstance.sharpClientMDITab.TabPages.Remove(this);    
				}
				catch{}
			}
			finally
			{
				if(this.Cursor==Cursors.WaitCursor)  
				{
					this.Cursor=Cursors.Arrow; 
					this.Enabled=true;
					PruneReportTree(false);
					ReportsNavigation.ExpandAll(); 
					Application.DoEvents();  
					
				}
				if(objImporter!=null)
				{
					this.Title =objImporter.profileeName+":"+ objImporter.profileeSessionName;  
					m_Session=objImporter.profileeSessionName;  
					objImporter.Dispose();
					objImporter=null;
				}				
			
			}			

		}
		

		private void SharpClientTabPage_VisibleChanged(object sender, EventArgs e)
		{
			if(m_tThread!=null && this.Visible==true)
			{
				if(m_tThread.ThreadState==System.Threading.ThreadState.Unstarted)
				{	
					try
					{	
						Init();						
						m_tThread.Start();							
					}
					catch
					{
						this.Enabled=false; 
						this.Title ="Error!";						
					}
				}
			}

			if(this.Enabled==true)
			{
				if(this.Visible==true)
				{
					SharpClientForm.scInstance.SubscribeToControl("TS",subscriberEvent);  
					SharpClientForm.scInstance.SubscribeToControl("CG",subscriberEvent);  
					SharpClientForm.scInstance.SubscribeToControl("WP",subscriberEvent);  
					SharpClientForm.scInstance.SubscribeToControl("CP",subscriberEvent);  
					//Menu Subscribers
					SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuExport_CSV,menuSubscriberEvent);
					SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuExport_XML,menuSubscriberEvent);
					SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuGraph,menuSubscriberEvent);
					SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuPrintPreview,menuSubscriberEvent);
					SharpClientForm.scInstance.SubscribeToMenu(SharpClientForm.scInstance.menuWebPreview,menuSubscriberEvent); 
					
					//SharpClientForm.scInstance.ShowCodeControl(true);

				}
				else
				{
					SharpClientForm.scInstance.UnsubscribeControl("TS"); 
					SharpClientForm.scInstance.UnsubscribeControl("CG"); 
					SharpClientForm.scInstance.UnsubscribeControl("WP");					
					SharpClientForm.scInstance.UnsubscribeControl("CP");					
					//SharpClientForm.scInstance.ShowCodeControl(false);
					SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuExport_CSV);
					SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuExport_XML);
					SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuGraph);
					SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuPrintPreview);
					SharpClientForm.scInstance.UnsubscribeMenu(SharpClientForm.scInstance.menuWebPreview);

				}
			}

		}

		private void settingsPanel_Expanded(object sender, EventArgs e)
		{
			if(AnalysisPanelGroup!=null)
			{
				foreach (Control psControl in AnalysisPanelGroup.Controls) 
				{
					if(psControl.GetType()==typeof(PSPanel) && psControl!=sender  )
					{
						((PSPanel)psControl).PanelState =PSPanelState.Collapsed; 

					}
				}
				if(sender==settingsPanel)
				{
					settingsPanel.Height = 425;
				}
				else if(sender==analysisPanel )
				{
					analysisPanel.Height =540;
				}
			}
			
		}

		private void LoadReportList(string reportList)
		{
			try
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(reportList); 
				foreach (XmlNode node in doc.DocumentElement.ChildNodes)
				{
					bool isNodeAlreadyAdded=false;
					foreach(TreeNode treeNode in ReportsNavigation.Nodes)
					{
						if(treeNode.Text == node.Attributes["Group"].Value)
						{
							isNodeAlreadyAdded =true;
							((TreeNode)(treeNode.Nodes.Add(node.Attributes["HelpString"].Value))).Tag =node.Attributes["Name"].Value   ;							
							break;
						}

					}	
					if(isNodeAlreadyAdded==false)
					{
						TreeNode treeNode=ReportsNavigation.Nodes.Add(node.Attributes["Group"].Value);
						((TreeNode)(treeNode.Nodes.Add(node.Attributes["HelpString"].Value))).Tag =node.Attributes["Name"].Value   ;							

					}				

				}				

			}
			catch(Exception ex)
			{
				ReportsNavigation.Enabled =false;
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error); 

			}			

		}

		private void Init()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SharpClientTabPage));			
			this.AnalysisPanelGroup = new PSUI.PSPanelGroup();
			this.analysisPanel = new PSUI.PSPanel(540);
			this.settingsPanel = new PSUI.PSPanel(425);
			this.ReportsNavigation = new System.Windows.Forms.TreeView();			
			this.saveSessionDlg = new System.Windows.Forms.SaveFileDialog();			
			this.saveSessionDlg.RestoreDirectory = true;
			this.saveSessionDlg.Title = "Export Session Data";

			MorphReportControl=new PGUserControl();
			MorphReportControl.TotallingEnabled = true;
			MorphReportControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			MorphReportControl.DBCon =null;
			MorphReportControl.ConfigFile ="";			
			MorphReportControl.Dock=DockStyle.Fill; 
			MorphReportControl.Location = new System.Drawing.Point(2, 35);
			MorphReportControl.Visible =false; 	


			Application.DoEvents();   

			///////////
			((System.ComponentModel.ISupportInitialize)(this.AnalysisPanelGroup)).BeginInit();
			this.AnalysisPanelGroup.SuspendLayout();

			if(m_fileName.ToLower().EndsWith(".fxml"))
			{				
				cnfgSettings=new Configurator ("READONLY",new FunctionData(0,"","",false,false,1),null,false,true,false); 
			}
			else if(m_fileName.ToLower().EndsWith(".oxml"))
			{				
				cnfgSettings=new Configurator ("READONLY",null,new ObjectData( 0,"",false,true),false,true,false); 
			}
			else
			{
				cnfgSettings=new Configurator ("READONLY",null,null,false,true,false); 
			}					
			settingsPanel.Controls.Add(cnfgSettings);
			cnfgSettings.Dock =DockStyle.Fill ;  
			cnfgSettings.Visible =false;
			// 
			// AnalysisPanelGroup
			// 
			this.AnalysisPanelGroup.AutoScroll = false;
			this.AnalysisPanelGroup.BackColor = System.Drawing.Color.Transparent;
			this.AnalysisPanelGroup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AnalysisPanelGroup.Controls.Add(this.analysisPanel);
			this.AnalysisPanelGroup.Controls.Add(this.settingsPanel);
			//this.AnalysisPanelGroup.Dock = System.Windows.Forms.DockStyle.Top; 
			this.AnalysisPanelGroup.Dock = System.Windows.Forms.DockStyle.Fill; 
			this.AnalysisPanelGroup.Location = new System.Drawing.Point(17, 17);
			this.AnalysisPanelGroup.Name = "AnalysisPanelGroup";
			this.AnalysisPanelGroup.PanelGradient = ((PSUI.GradientColor)(resources.GetObject("AnalysisPanelGroup.PanelGradient")));
			//this.AnalysisPanelGroup.Size = new System.Drawing.Size(728, 581);						
			// 
			// analysisPanel
			// 
			this.analysisPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.analysisPanel.AnimationRate = 0;
			this.analysisPanel.BackColor = System.Drawing.Color.Transparent;
			this.analysisPanel.Caption = "Analyze";
			this.analysisPanel.CaptionCornerType = PSUI.CornerType.Top;
			this.analysisPanel.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.analysisPanel.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.analysisPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.analysisPanel.CaptionUnderline = System.Drawing.Color.Gray;
			this.analysisPanel.CurveRadius = 12;
			this.analysisPanel.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.analysisPanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.analysisPanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.analysisPanel.ImageItems.PSImgSet = null;
			this.analysisPanel.Location = new System.Drawing.Point(517, 17);
			this.analysisPanel.Name = "analysisPanel";
			this.analysisPanel.PanelGradient.End = System.Drawing.Color.White;
			this.analysisPanel.PanelGradient.Start = System.Drawing.Color.White;
			this.analysisPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.analysisPanel.Size = new System.Drawing.Size(720, 540);
			this.analysisPanel.TabIndex = 1;
			this.analysisPanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.analysisPanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.analysisPanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.analysisPanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.analysisPanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.analysisPanel.Expanded += new System.EventHandler(this.settingsPanel_Expanded);
			// 
			// settingsPanel
			// 
			this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsPanel.AnimationRate = 0;
			this.settingsPanel.BackColor = System.Drawing.Color.Transparent;
			this.settingsPanel.Caption = "Session Settings";
			this.settingsPanel.CaptionCornerType = PSUI.CornerType.Top;
			this.settingsPanel.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.settingsPanel.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.settingsPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.settingsPanel.CaptionUnderline = System.Drawing.Color.Gray;
			this.settingsPanel.CurveRadius = 12;
			this.settingsPanel.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.settingsPanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.settingsPanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.settingsPanel.ImageItems.PSImgSet = null;
			this.settingsPanel.Location = new System.Drawing.Point(166, 17);
			this.settingsPanel.Name = "settingsPanel";
			this.settingsPanel.PanelGradient.End = System.Drawing.Color.White;
			this.settingsPanel.PanelGradient.Start = System.Drawing.Color.White;
			this.settingsPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.settingsPanel.PanelState = PSUI.PSPanelState.Collapsed;
			this.settingsPanel.Size = new System.Drawing.Size(720, 425);
			this.settingsPanel.TabIndex = 0;
			this.settingsPanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.settingsPanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.settingsPanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.settingsPanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.settingsPanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.settingsPanel.Expanded += new System.EventHandler(this.settingsPanel_Expanded);
			// 
			// ReportsNavigation
			// 
			this.ReportsNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ReportsNavigation.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ReportsNavigation.FullRowSelect = true;			
			this.ReportsNavigation.Location = new System.Drawing.Point(776, 17);
			this.ReportsNavigation.Name = "ReportsNavigation";
			this.ReportsNavigation.SelectedImageIndex = -1;
			this.ReportsNavigation.Size = new System.Drawing.Size(708, 474);
			this.ReportsNavigation.TabIndex = 1;
			this.ReportsNavigation.ImageList=SharpClientForm.scInstance.imageList2;
			this.ReportsNavigation.ImageIndex=1; 		

			// 
			// reportsImgList
			// 
			((System.ComponentModel.ISupportInitialize)(this.AnalysisPanelGroup)).EndInit();
			this.AnalysisPanelGroup.ResumeLayout(false);

			this.analysisPanel.Controls.Add(ReportsNavigation);    			
			this.Controls.Add(AnalysisPanelGroup);
		
			
			if(m_fileName.ToLower().EndsWith(".fxml"))
			{
				LoadReportList(Application.StartupPath+ @"\PerformanceReportDefs\PerformanceReportCollection.xml");
			}
			else if(m_fileName.ToLower().EndsWith(".oxml"))
			{
				LoadReportList(Application.StartupPath+@".\MemoryReportDefs\MemoryReportCollection.xml");
			}				

			ReportsNavigation.DoubleClick +=new EventHandler(ReportsNavigation_DoubleClick); 

		}
		

		private void SharpClientTabPage_EnabledChanged(object sender, EventArgs e)
		{
			if(this.Enabled==true)
			{
				if(funcObj!=null)
				{					
					cnfgSettings.functionDataObj.Fill(funcObj); 
					cnfgSettings.SyncUI();  					
					funcObj=null;
					cnfgSettings.Dock =DockStyle.Fill ;
					cnfgSettings.Visible =true;
					cnfgSettings.Show(); 						   
				}
				else if(objObj!=null)
				{
					cnfgSettings.objectDataObj.Fill(objObj);  
					cnfgSettings.SyncUI();					
					objObj=null;
					if(cnfgSettings.Parent!= settingsPanel)
					{
						settingsPanel.Controls.Add(cnfgSettings);
					}
					cnfgSettings.Dock =DockStyle.Fill ;
					cnfgSettings.Visible =true;
					cnfgSettings.Show(); 					
				}
				
			}

		}

		private void ReportsNavigation_DoubleClick(object sender, EventArgs e)
		{
			if(ReportsNavigation.SelectedNode.Parent!=null)  
			{
				string folderToSeek;
				if(m_fileName.ToLower().EndsWith(".fxml"))
				{
					folderToSeek=Application.StartupPath+ @"\PerformanceReportDefs\";
					if(ReportsNavigation.SelectedNode.Parent.Text=="Code Analysis")
					{
						if(ReportsNavigation.SelectedNode.Text=="Analyze Overall Code Performance")
						{
							this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_CodeAnalysis);
							this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Line-Level Performance.");
							Application.DoEvents(); 
						}
						else if (ReportsNavigation.SelectedNode.Text=="Analyze Code Performance by Individual Thread-IDs")
						{
							this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ThreadCodeAnalysis);
							this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Line-Level Performance for the Thread.");
							Application.DoEvents(); 

						}
					}
					else if(ReportsNavigation.SelectedNode.Text=="Analyze Exceptions")
					{
						//this.MorphReportControl.ReportGrid.Navigate+=new NavigateSelectedIndexChanged_Handler(ReportGrid_Navigate); 
						//DataGridTableStyle exceptionStyle=new DataGridTableStyle();
						//exceptionStyle.MappingName="Exceptions" ;
						//this.MorphReportControl.ReportGrid.TableStyles.Add(exceptionStyle);						


						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ExceptionAnalysis);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Exception Stack-Trace.");
						Application.DoEvents(); 

					}
					else if(ReportsNavigation.SelectedNode.Text=="View Parent Function Information")
					{						
						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ParentFunction);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Parent Functions.");
						Application.DoEvents(); 
					}
					else if(ReportsNavigation.SelectedNode.Text=="View Children Function Information")
					{						
						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ChildrenFunction);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Children Functions.");
						Application.DoEvents(); 
					}

				}
				else if	(m_fileName.ToLower().EndsWith(".oxml"))
				{
					folderToSeek=Application.StartupPath+@"\MemoryReportDefs\";
					if(ReportsNavigation.SelectedNode.Text=="Analyze Objects by allocation")
					{	
						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ObjectAllocationAnalysis);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Object-Allocation Stack.");
						Application.DoEvents(); 
					}
					else if(ReportsNavigation.SelectedNode.Text=="Analyze In-Memory Referenced Objects")
					{
						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ReferencedObjects);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Referenced Objects.");
						Application.DoEvents(); 
					}
					else if(ReportsNavigation.SelectedNode.Text=="Analyze In-Memory Referencing Objects")
					{
						this.MorphReportControl.SelectedIndex_Changed+=new SelectedIndexChanged_Handler(SelectedIndex_Changed_ReferencingObjects);
						this.MorphReportControl.ReportGridToolTip.SetToolTip(this.MorphReportControl.ReportGrid,"Click on a row to view Referencing Objects.");
						Application.DoEvents(); 
					}
				}
				else
				{
					return;
				}
				System.IO.FileInfo fileInfo=null;
				try
				{
					fileInfo =new System.IO.FileInfo(folderToSeek+@ReportsNavigation.SelectedNode.Tag+@".xml");
				}
				catch{}
				if(fileInfo==null)
				{
					MessageBox.Show("The file for this report does not exist!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);                     					 
					return;
				}
				if(!fileInfo.Exists) 
				{
					MessageBox.Show("The file for this report does not exist!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);                     					 
					return;
				}
				
				Application.DoEvents();
				
				MorphReportControl.DBCon=new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;"); 
				MorphReportControl.ConfigFile =fileInfo.FullName ;
				MorphReportControl.Search_Clicked+=new Search_ClickedHandler(MorphReportControl_Search_Clicked); 
				MorphReportControl.Parameter_Changed +=new ParameterChanged_Handler(MorphReportControl_Parameter_Changed);

				MorphReportControl.FixedPropertyTable["SessionID"]= m_Session;

				//////////////////Imp Change///
				///

				if(MorphReportControl.Parent!=this)
				{
					this.Controls.Add(MorphReportControl);
				}
				MorphReportControl.Parent=this; 

				if(settingsPanel.Controls.Contains (cnfgSettings))
				{
					this.settingsPanel.Controls.Remove(cnfgSettings);				
				}
				if(this.Controls.Contains (AnalysisPanelGroup))
				{
					this.Controls.Remove (AnalysisPanelGroup); 	
				}

				MorphReportControl.Visible=true;				   
				
				MorphReportControl.Show();
				Application.DoEvents();

			}
		}		
		
		protected override void Dispose( bool disposing )
		{
			//SharpClientForm.scInstance.UnsubscribeControl("TS");
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
			if(srcTable!=null)
			{
				try
				{
				
					srcTable.Clear();
					srcTable.Dispose();
				}
				catch{}
			}
			srcTable=null;
			fileNameArray.Clear(); 
			if( disposing )
			{			

				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		
		public System.Data.DataSet GetResultSetCopy()
		{
			try
			{
				System.Data.DataSet dsTemp=((System.Data.DataSet)(this.MorphReportControl.ReportGrid.DataSource)).Copy();
				dsTemp.DataSetName =this.Title.Split(new char[]{':'},2)[1] ; 
				return  dsTemp;   
			}
			catch
			{

			}
			return null;
		}

		public PGRptControl.PGUserControl GetReportControl()
		{
			return this.MorphReportControl ;
		}


		private int getColumnIndex(string colName)
		{
			return ((DataSet)(MorphReportControl.ReportGrid.DataSource )).Tables[0].Columns[colName].Ordinal;  
		}		

		private void MorphReportControl_Search_Clicked(long RowsReturned, string MasterQuery)
		{			
			if(bIsDirty)
			{
				try
				{
					if(SharpClientForm.scInstance.codeTabControl.TabPages.Count>0)
					{
						SharpClientForm.scInstance.codeTabControl.TabPages.Clear(); 
					}
				}
				catch{}
			}
			bIsDirty=false;

			if(SharpClientForm.CanShowQry())
			{
				MessageBox.Show(MasterQuery);  
			}
		}

		

		private void MorphReportControl_Parameter_Changed(object sender)
		{
			bIsDirty=true;			
			
		}

		
		private void SelectedIndex_Changed_ThreadCodeAnalysis(object sender, EventArgs e)
		{
			OleDbCommand cmd=null;
			try
			{				
				this.Enabled=false;
				this.Cursor=Cursors.WaitCursor;
				Application.DoEvents(); 

				int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
				string fID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("FunctionID")]);				
				string timeConsumed=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Time Consumed(units)")]);								
				string threadID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ThreadID")]);								

				cmd=new OleDbCommand("Select top 1 FileName from DebugCode where SessionID='"+m_Session+"' and FunctionID='"+fID+"' and FileOffset <> 0 "); 
				cmd.Connection =MorphReportControl.DBCon ;
				if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
				{
					cmd.Connection.Open();  
				}

				string fileName=Convert.ToString(cmd.ExecuteScalar()).Trim()  ;
				if(!System.IO.File.Exists(fileName))
				{
					if(fileName==null || fileName.LastIndexOf(".")==-1 || fileName.LastIndexOf("\\") ==-1)
					{
						throw new Exception("Unable to retrieve source code document.\nNo profiling data could be collected for this function."); 
					}

					if(openSrcFileDlg==null)
					{
						openSrcFileDlg=new OpenFileDialog();
						openSrcFileDlg.Title ="Select Source Code File";			
						openSrcFileDlg.RestoreDirectory =true;
						openSrcFileDlg.Multiselect=false; 
					}

					string ext=fileName.Substring(fileName.LastIndexOf("."));
					string srcFile=fileName.Substring(fileName.LastIndexOf("\\")+1);
					openSrcFileDlg.Filter ="Source Code File(*"+ext+")|"+srcFile;					
					openSrcFileDlg.FileName=srcFile;

					if(openSrcFileDlg.ShowDialog() ==DialogResult.OK)
					{
						string newFile=openSrcFileDlg.FileName;
						if(!System.IO.File.Exists(newFile) || newFile==null || newFile.LastIndexOf(".")==-1)
						{
							throw new Exception("Unable to retrieve source code document"); 							
						}

						cmd.CommandText="UPDATE DebugCode SET FileName='"+newFile+"' where SessionID='"+m_Session+"' AND FileName='"+fileName+"'";
						cmd.ExecuteNonQuery();
						Application.DoEvents(); 						
						fileName=newFile;
						MorphReportControl.DoRefresh();
						Application.DoEvents();  						
					}
					else
					{
						throw new Exception("Unable to retrieve source code performance data for the specified function.\nEither the source code document is unavailable or no profiling data could be collected for this function."); 
					}	
				}

				int fileOffset=0;

				//				if(!fileNameArray.Contains(fileName.ToLower()))
				//				{
				//					fileNameArray.Add(fileName.ToLower()); 

				string codeQry=@"SELECT Sum([CollectiveTime]) AS [CollectiveTime], Sum([DebugCode.HitCount]) as [Hit Count], Sum([DebugCode.TimeConsumed]) as [Time Consumed],[DebugCode.FileOffset],[DebugCode.FileName] FROM DEBUGCODE WHERE [DebugCode.SessionID]= '"+m_Session+@"' and [DebugCode.FileName]='"+fileName+@"' and ThreadID='"+threadID+@"' GROUP BY [DebugCode.FileName],[DebugCode.FileOffset],[DebugCode.ThreadID] ORDER BY [DebugCode.FileName], [DebugCode.FileOffset]";
				
				if(srcTable!=null)
				{
					srcTable.Dispose();
					srcTable=null;
				}

				srcTable=new DataTable();
				OleDbDataAdapter ada=new OleDbDataAdapter(codeQry,MorphReportControl.DBCon);
				ada.Fill(srcTable);
				ada.Dispose();
				ada=null;
				//				}
				cmd.CommandText ="SELECT TOP 1 FileOffset FROM DebugCode WHERE SessionID='"+m_Session+"' AND FunctionID= '"+fID+"' and ThreadID='"+ threadID +"' and FileOffset <> 0 ORDER BY FileOffset "; 
				try
				{
					fileOffset=Convert.ToInt32(cmd.ExecuteScalar());  
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message+"\n Code view is unavailable"); 
					return;
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
			
				SharpClientForm.scInstance.AddCodeControlTab(m_Session,fileName,threadID,ref srcTable,0,0);
				SharpClientForm.scInstance.scrollToCodeLine(fileOffset);
				return;
			}			
			catch(Exception ex)
			{				
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
			finally
			{
				if(this.Enabled==false )
				{
					this.Enabled=true;
				}
				if(this.Cursor==Cursors.WaitCursor)
				{
					this.Cursor=Cursors.Arrow;  
				}

				Application.DoEvents(); 	

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
		}

		private void SelectedIndex_Changed_CodeAnalysis(object sender, EventArgs e)
		{
			OleDbCommand cmd=null;
			try
			{				
				this.Enabled=false;
				this.Cursor=Cursors.WaitCursor;
				Application.DoEvents(); 

				int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
				string fID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("FunctionID")]);				
				string timeConsumed=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Time Consumed(units)")]);								

				cmd=new OleDbCommand("Select top 1 FileName from DebugCode where SessionID='"+m_Session+"' and FunctionID='"+fID+"' and FileOffset <> 0 "); 
				cmd.Connection =MorphReportControl.DBCon ;
				if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
				{
					cmd.Connection.Open();  
				}

				string fileName=Convert.ToString(cmd.ExecuteScalar()).Trim()  ;
				if(!System.IO.File.Exists(fileName))
				{
					if(fileName==null || fileName.LastIndexOf(".")==-1 || fileName.LastIndexOf("\\") ==-1)
					{
						throw new Exception("Unable to retrieve source code document.\nNo profiling data could be collected for this function."); 
					}

					if(openSrcFileDlg==null)
					{
						openSrcFileDlg=new OpenFileDialog();
						openSrcFileDlg.Title ="Select Source Code File";			
						openSrcFileDlg.RestoreDirectory =true;
						openSrcFileDlg.Multiselect=false; 
					}

					string ext=fileName.Substring(fileName.LastIndexOf("."));
					string srcFile=fileName.Substring(fileName.LastIndexOf("\\")+1);
					openSrcFileDlg.Filter ="Source Code File(*"+ext+")|"+srcFile;					
					openSrcFileDlg.FileName=srcFile;

					if(openSrcFileDlg.ShowDialog() ==DialogResult.OK)
					{
						string newFile=openSrcFileDlg.FileName;
						if(!System.IO.File.Exists(newFile) || newFile==null || newFile.LastIndexOf(".")==-1)
						{
							throw new Exception("Unable to retrieve source code document"); 							
						}

						cmd.CommandText="UPDATE DebugCode SET FileName='"+newFile+"' where SessionID='"+m_Session+"' AND FileName='"+fileName+"'";
						cmd.ExecuteNonQuery();
						Application.DoEvents(); 						
						fileName=newFile;
						MorphReportControl.DoRefresh();
						Application.DoEvents();  						
					}
					else
					{
						throw new Exception("Unable to retrieve source code performance data for the specified function.\nEither the source code document is unavailable or no profiling data could be collected for this function."); 
					}	
				}

				int fileOffset=0;

//				if(!fileNameArray.Contains(fileName.ToLower()))
//				{
//					fileNameArray.Add(fileName.ToLower()); 

					string codeQry=@"SELECT Sum([CollectiveTime]) AS [CollectiveTime], Sum([DebugCode.HitCount]) as [Hit Count], Sum([DebugCode.TimeConsumed]) as [Time Consumed],[DebugCode.FileOffset],[DebugCode.FileName] FROM DEBUGCODE WHERE [DebugCode.SessionID]= '"+m_Session+@"' and [DebugCode.FileName]='"+fileName+@"' GROUP BY [DebugCode.FileName],[DebugCode.FileOffset] ORDER BY [DebugCode.FileName], [DebugCode.FileOffset]";
					
					if(srcTable!=null)
					{
						srcTable.Dispose();
						srcTable=null;
					}

					srcTable=new DataTable();
					OleDbDataAdapter ada=new OleDbDataAdapter(codeQry,MorphReportControl.DBCon);
					ada.Fill(srcTable);
					ada.Dispose();
					ada=null;
//				}
				cmd.CommandText ="SELECT TOP 1 FileOffset FROM DebugCode WHERE SessionID='"+m_Session+"' AND FunctionID= '"+fID+"' and FileOffset <> 0 ORDER BY FileOffset "; 
				try
				{
					fileOffset=Convert.ToInt32(cmd.ExecuteScalar());  
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message+"\n Code view is unavailable"); 
					return;
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
				
				SharpClientForm.scInstance.AddCodeControlTab(m_Session,fileName,ref srcTable,0,0);
				SharpClientForm.scInstance.scrollToCodeLine(fileOffset);
				return;
			}			
			catch(Exception ex)
			{				
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
			finally
			{
				if(this.Enabled==false )
				{
					this.Enabled=true;
				}
				if(this.Cursor==Cursors.WaitCursor)
				{
					this.Cursor=Cursors.Arrow;  
				}

				Application.DoEvents(); 	

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
		}


		private void SelectedIndex_Changed_ParentFunction(object sender, EventArgs e)
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
					string functionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("FunctionID")]);				
					string parentFunctionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Parent-FunctionID")]);				
					string functionName=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Function Name")]);									
					string hitCount=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Hit Count")]);									
					string threadID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Thread ID")]);
					string timeConsumed=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Time Consumed(%)")]);									

					if(threadID==null | threadID.Length==0)
					{
						detailString="Function: "+functionName+"\n"+"Hit Count: "+hitCount+"\n"+"Time Consumed(%): "+timeConsumed+"\nFunctionID: "+functionID+", called by :-";
					}
					else
					{
						detailString="Function: "+functionName+"\n"+"Hit Count: "+hitCount+"    Thread ID: "+threadID+"\n"+"Time Consumed(%): "+timeConsumed+"\nFunctionID: "+functionID+", called by :-";
					}

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					if(threadID==null | threadID.Length==0)
					{
						cmd=new OleDbCommand("Select top 1 FunctionID,FSignature,Calls,CollectiveTime,ThreadID,ModuleName from FTable where SessionID='"+m_Session+"' and FunctionID='"+parentFunctionID+"'"); 
					}
					else
					{
						cmd=new OleDbCommand("Select top 1 FunctionID,FSignature,Calls,CollectiveTime,ThreadID,ModuleName from FTable where SessionID='"+m_Session+"' and FunctionID='"+parentFunctionID+"' and ThreadID='"+threadID+"'"); 
					}
					
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
							string tId=Convert.ToString( reader.GetValue(4));
							tId=(tId==null||tId.Length==0)?"N:A":tId;
							string modId=Convert.ToString( reader.GetValue(5));
							modId=(modId==null||modId.Length==0)?"N:A":modId; 
							stackString.Append(Convert.ToString( reader.GetValue(0))+"^" +Convert.ToString( reader.GetValue(1))+"^"+Convert.ToString( reader.GetValue(2))+"^"+Convert.ToString( reader.GetValue(3))+"^"+tId+"^"+modId+"|");
   
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
				SharpClientForm.scInstance.AddStackControlTab("Parent Functions",m_Session,10,SharpClient.UI.Docking.State.DockTop,"FunctionID|Function|Total-Calls|Total Time-Consumed(Units)|ThreadID|Module",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}


		private void SelectedIndex_Changed_ChildrenFunction(object sender, EventArgs e)
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
					string functionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("FunctionID")]);									
					string functionName=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Function Name")]);									
					string hitCount=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Hit Count")]);									
					string percentTimeConsumed=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Time Consumed(%)")]);									
					string threadID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Thread ID")]);									
					UInt64 ui64TimeConsumed=Convert.ToUInt64(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Time Consumed(units)")]);									


					if(threadID==null | threadID.Length==0)
					{
						detailString="Function: "+functionName+"\n"+"Hit Count: "+hitCount+"\n"+"Time Consumed(%): "+percentTimeConsumed+"\nFunctionID: "+functionID+", called :-";
					}
					else
					{
						detailString="Function: "+functionName+"\n"+"Hit Count: "+hitCount+"   Thread ID: "+threadID+"\n"+"Time Consumed(%): "+percentTimeConsumed+"\nFunctionID: "+functionID+", called :-";
					}

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					if(threadID==null | threadID.Length==0)
					{
						cmd=new OleDbCommand("Select CalleeFunctionID,CFSignature,CFCalls,CFCollectiveTime,CFThreadID from CFTable where SessionID='"+m_Session+"' and CallerFunctionID='"+functionID+"' "); 
					}
					else
					{
						cmd=new OleDbCommand("Select CalleeFunctionID,CFSignature,CFCalls,CFCollectiveTime,CFThreadID from CFTable where SessionID='"+m_Session+"' and CallerFunctionID='"+functionID+"' and CFThreadID='"+threadID+"'"); 
					}
					cmd.Connection =MorphReportControl.DBCon ;
					if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
					{
						cmd.Connection.Open();  
					}

					reader=cmd.ExecuteReader();
					if(reader.HasRows)					
					{
						UInt64 percentOfParent=0;
						while(reader.Read())
						{
							percentOfParent=(ui64TimeConsumed==0)?0:(Convert.ToUInt64(reader.GetValue(3))*100)/ui64TimeConsumed;
							stackString.Append(Convert.ToString( reader.GetValue(0))+"^" +Convert.ToString( reader.GetValue(1))+"^"+Convert.ToString( reader.GetValue(2))+"^"+Convert.ToString(percentOfParent)+"^"+Convert.ToString(reader.GetValue(4))+"|");   
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

				SharpClientForm.scInstance.AddStackControlTab("Child Functions",m_Session,9,SharpClient.UI.Docking.State.DockTop,"FunctionID|Function|Total-Calls|Total Time-Consumed (% of parent)|Thread ID",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}


		private void SelectedIndex_Changed_ExceptionAnalysis(object sender, EventArgs e)
		{
			#region Exception Analysis Code (Old)
//			try
//			{
//				if(MorphReportControl.ReportGrid.DataMember.ToUpper()!="FUNCTIONS"    )
//				{
//					if(MorphReportControl.ReportGrid.CurrentCell.RowNumber!=curRowIndex)
//					{	
//						SizeF size=MorphReportControl.ReportGrid.CreateGraphics().MeasureString(Convert.ToString(MorphReportControl.ReportGrid[MorphReportControl.ReportGrid.CurrentRowIndex,2]),MorphReportControl.ReportGrid.Font);
//						ResizeRows(size);						
//						MorphReportControl.ReportGrid.TableStyles[0].GridColumnStyles["Exception Trace"].Width= Convert.ToInt32(size.Width)+12;                        
//						curRowIndex =MorphReportControl.ReportGrid.CurrentCell.RowNumber;
//					}
//				}
//			}
//			catch{}
			#endregion


			try
			{
				OleDbCommand cmd=null;
				System.Text.StringBuilder stackString=new System.Text.StringBuilder("") ;
				string detailString="";
				OleDbDataReader reader=null;
				try
				{	
					int rowIndex=MorphReportControl.ReportGrid.CurrentRowIndex ;
					string functionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("FunctionID")]);									
					string functionName=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Function Name")]);									
					string exceptionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ExceptionID")]);
					string sessionID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("SessionID")]);
					string exceptionType=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Exception Type")]);
					string moduleName=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Module")]);
					string threadID=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("ThreadID")]);

					cmd=new OleDbCommand();
					cmd.Connection =MorphReportControl.DBCon ;

					try
					{
						cmd.CommandText="Select Sum(Calls),Sum(CollectiveTime) from FTable where SessionID='"+m_Session+"' and FunctionID='"+functionID+"'"; 											
						if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
						{
							cmd.Connection.Open();  
						}
						reader=cmd.ExecuteReader();
						if(reader.HasRows)						
						{
							reader.Read();
							detailString="Exception: "+exceptionType+"\nRaised by function:  " +functionName+"    [ FunctionID: "+functionID+"  (Total-Calls: "+Convert.ToString(reader.GetValue(0))+", Total-Time Consumed: "+Convert.ToString(reader.GetValue(1))+" units) ]\nModule:  "+moduleName+", ThreadID: "+threadID; 
						}
						else
						{
							detailString="Exception: "+exceptionType+"\nRaised by function:  " +functionName+"    [ FunctionID: "+functionID+" ]\nModule:  "+moduleName+", ThreadID: "+threadID; 
						}
					}
					catch
					{
						detailString="Exception: "+exceptionType+"\nRaised by function:  " +functionName+"    [ FunctionID: "+functionID+" ]\nModule:  "+moduleName+", ThreadID: "+threadID; 
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

					}
					
					cmd.CommandText="Select ExceptionTrace from Exceptions where SessionID='"+m_Session+"' and ExceptionID='"+exceptionID+"'"; 											
					if(cmd.Connection.State== ConnectionState.Closed ||  cmd.Connection.State==ConnectionState.Broken)
					{
						cmd.Connection.Open();  
					}			

					reader=cmd.ExecuteReader();
					if(reader.HasRows)										
					{						
						while(reader.Read())
						{							
							stackString.Append(Convert.ToString( reader.GetValue(0)));
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

				SharpClientForm.scInstance.AddStackControlTab("Exception-Stack",m_Session,11,SharpClient.UI.Docking.State.DockRight,"Exception Stack-Trace",stackString.ToString() ,detailString,-1,false,false);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}

		private void SelectedIndex_Changed_ObjectAllocationAnalysis(object sender, EventArgs e)
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
					string objAge=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Age")]);
					string objSize=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Size(Bytes)")]);									
					string objGen=Convert.ToString(MorphReportControl.ReportGrid[rowIndex,getColumnIndex("Object Generation")]);


					detailString="Object-Class: "+objClass+"\n"+"Object-Size: "+objSize+" bytes\n"+"ObjectID: "+objID+"\n"+"ThreadID: "+objThreadID+"  Object Age: "+ objAge + "\nObject Generation: "+objGen;;

					//USE TOP 1 WHEN RESULT COUNT IS EXPECTED TO BE ONE
					cmd=new OleDbCommand("Select top 1 AllocFunctionIDStack from ObjectAllocation where SessionID='"+m_Session+"' and ObjectID='"+objID+"' "); 
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
						cmd.CommandText="SELECT top 1 AllocFunctionName from AllocationFunction where AllocFunctionID='"+stackIDs[x]+"' and SessionID='"+m_Session+"'";
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
				
				SharpClientForm.scInstance.AddStackControlTab("Allocation-Stack",m_Session,6,SharpClient.UI.Docking.State.DockRight,"Allocation-Stack",stackString.ToString() ,detailString,-1,false,false);
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}
		}
	

		private void SelectedIndex_Changed_ReferencedObjects(object sender, EventArgs e)
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
					cmd=new OleDbCommand("Select RefObjectID,RefObjectName,RefObjectCount,RefObjectSize from RefObjects where SessionID='"+m_Session+"' and ParentObjectID='"+objID+"' "); 
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
				SharpClientForm.scInstance.AddStackControlTab("Referenced Objects",m_Session,7,SharpClient.UI.Docking.State.DockTop,"ObjectID|Object-Class|Reference-Count|Object-Size(Byte)",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}

		}

		private void SelectedIndex_Changed_ReferencingObjects(object sender, EventArgs e)
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
					cmd=new OleDbCommand("Select ObjectID,ObjectName,ObjectSize,IsRootObject from LiveObjects where SessionID='"+m_Session+"' and ObjectID in (Select distinct ParentObjectID from RefObjects where SessionID='"+m_Session+"' and RefObjectID='"+objID+"') "); 
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
				SharpClientForm.scInstance.AddStackControlTab("Referencing Objects",m_Session,8,SharpClient.UI.Docking.State.DockTop,"ObjectID|Object-Class|Object-Size(Byte)|Root-Object",stackString.ToString() ,detailString,-2,false,true);    				
				
			}
			catch(Exception ex)
			{			
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ;				
			}

		}


		#region Exception Analysis Code (Old)
//		private void ResizeRows(SizeF size)
//		{
//			try
//			{
//				if(excRowArray==null)
//				{
//					MethodInfo mi = MorphReportControl.ReportGrid.GetType().GetMethod("get_DataGridRows", 
// 
//						BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance 
// 
//						| BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static); 
//
//					excRowArray = (object[])mi.Invoke(MorphReportControl.ReportGrid,null); 
//				}
//
//				if(curRowIndex !=-1)
//				{
//					try
//					{
//						excRowArray[curRowIndex].GetType().GetProperty("Height").SetValue(excRowArray[curRowIndex],18,null);
//						//Height 18 is hardcoded
//					}
//					catch{}
//
//				}
//				int i=MorphReportControl.ReportGrid.CurrentCell.RowNumber;  
//				PropertyInfo pi = excRowArray[i].GetType().GetProperty("Height");  
//				pi.SetValue(excRowArray[i],Convert.ToInt32(size.Height)+8 ,null); 
//			}
//			catch{}
//		}
//
//		private void ReportGrid_Navigate(object sender, NavigateEventArgs ne)
//		{
//			curRowIndex=-1;
//			excRowArray=null;
//		}
//		
#endregion

		private void InitializeComponent()
		{

		}

		
	}
}
