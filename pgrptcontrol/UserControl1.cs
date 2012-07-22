using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml; 
using System.Data.OleDb;
using System.Reflection;  
namespace PGRptControl
{

	/// <summary>
	/// Search Button Clicked Event Handler
	/// </summary>
	/// 	
	public delegate void Search_ClickedHandler(long RowsReturned,string MasterQuery);
	public delegate bool BeforeSearch_ClickedHandler();
	public delegate void Total_ClickedHandler(int ColumnIndex,double Total);
	public delegate bool QueryParsed_Handler(string parsedQuery);
	public delegate void DataSetFormed_Handler(DataSet GridDataSource,string BoundObject);
	public delegate void ParameterChanged_Handler(object sender);

	public delegate void SelectedIndexChanged_Handler(object sender,System.EventArgs e);



	/// <summary>
	/// PGUserControl For Report Generation
	/// </summary>
	/// 	

	public class PGUserControl : System.Windows.Forms.UserControl
	{		
		private String cnfgFileName;		
		private OleDbConnection ActiveCon;
		private XmlDocument cnfgFile;
		private string CriteriaID;
		private Hashtable CriteriaTable;
		private Hashtable ConditionTable;
		private Hashtable fixedPropTable;
		private System.Windows.Forms.ComboBox SearchCriteria;
		private System.Windows.Forms.Panel ContainerPanel;
		private string BindableObject=null;
		private string MasterTable=null;
		private System.Windows.Forms.ContextMenu GridColumnAggregateMenu;
		private System.Windows.Forms.MenuItem AggregateMenuItem;
		private int AggregatableColumnNumber;
		private bool IsTotallingEnabled;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.DataGrid ParentGrid;
		private System.Windows.Forms.Button ExecQuery;
		private System.Windows.Forms.Label StatusLabel;
		public event BeforeSearch_ClickedHandler BeforeSearch_Clicked;
		public event Search_ClickedHandler Search_Clicked;
		public event Total_ClickedHandler Total_Clicked;
		public event QueryParsed_Handler QueryParsed;
		public event DataSetFormed_Handler DataSetFormed;
		public event ParameterChanged_Handler Parameter_Changed;  
		public event SelectedIndexChanged_Handler SelectedIndex_Changed;
		private int selIndex;
		public System.Windows.Forms.ToolTip ReportGridToolTip;
		private System.ComponentModel.IContainer components;

		public PGUserControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();				
			fixedPropTable=new Hashtable ();
			selIndex=-1;
						
		}

		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();


				fixedPropTable.Clear();				
				Events.Dispose();

				if(cnfgFile !=null)
				{
					cnfgFile.RemoveAll();				
					cnfgFile =null;
				}

				if (CriteriaTable!=null) 				
				{
					CriteriaTable.Clear();
					CriteriaTable =null;
				}

				if(ConditionTable !=null)
				{
					ConditionTable.Clear(); 
					ConditionTable=null;
				}
				if(!ParentGrid.IsDisposed )
				{
					ParentGrid.DataBindings.Clear();
					if( ((DataSet)ParentGrid.DataSource)!=null )
					{
						try
						{
							((DataSet)ParentGrid.DataSource).Clear();
							((DataSet)ParentGrid.DataSource).Dispose(); 
						}
						catch(Exception ex){}
					}
 


					ParentGrid.DataSource=null;
					ParentGrid.Dispose();  
				}


			}
			
 
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.GridColumnAggregateMenu = new System.Windows.Forms.ContextMenu();
			this.AggregateMenuItem = new System.Windows.Forms.MenuItem();
			this.SearchCriteria = new System.Windows.Forms.ComboBox();
			this.ContainerPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.StatusLabel = new System.Windows.Forms.Label();
			this.ExecQuery = new System.Windows.Forms.Button();
			this.ParentGrid = new System.Windows.Forms.DataGrid();
			this.ReportGridToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ParentGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// GridColumnAggregateMenu
			// 
			this.GridColumnAggregateMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									this.AggregateMenuItem});
			// 
			// AggregateMenuItem
			// 
			this.AggregateMenuItem.Index = 0;
			this.AggregateMenuItem.Text = "Total";
			this.AggregateMenuItem.Click += new System.EventHandler(this.AggregateMenuItem_Click);
			// 
			// SearchCriteria
			// 
			this.SearchCriteria.Dock = System.Windows.Forms.DockStyle.Top;
			this.SearchCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SearchCriteria.Location = new System.Drawing.Point(10, 10);
			this.SearchCriteria.Name = "SearchCriteria";
			this.SearchCriteria.Size = new System.Drawing.Size(584, 21);
			this.SearchCriteria.TabIndex = 3;
			this.SearchCriteria.SelectedIndexChanged += new System.EventHandler(this.SearchCriteria_SelectedIndexChanged);
			// 
			// ContainerPanel
			// 
			this.ContainerPanel.AutoScroll = true;
			this.ContainerPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ContainerPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ContainerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ContainerPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ContainerPanel.Location = new System.Drawing.Point(10, 31);
			this.ContainerPanel.Name = "ContainerPanel";
			this.ContainerPanel.Size = new System.Drawing.Size(584, 112);
			this.ContainerPanel.TabIndex = 4;
			this.ContainerPanel.TabStop = true;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.StatusLabel);
			this.panel1.Controls.Add(this.ExecQuery);
			this.panel1.Controls.Add(this.ContainerPanel);
			this.panel1.Controls.Add(this.SearchCriteria);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.DockPadding.All = 10;
			this.panel1.Location = new System.Drawing.Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(608, 192);
			this.panel1.TabIndex = 11;
			// 
			// StatusLabel
			// 
			this.StatusLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.StatusLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.StatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.StatusLabel.Location = new System.Drawing.Point(10, 143);
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(256, 35);
			this.StatusLabel.TabIndex = 15;
			this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ExecQuery
			// 
			this.ExecQuery.BackColor = System.Drawing.SystemColors.Control;
			this.ExecQuery.Cursor = System.Windows.Forms.Cursors.Hand;
			this.ExecQuery.Dock = System.Windows.Forms.DockStyle.Right;
			this.ExecQuery.ForeColor = System.Drawing.SystemColors.ControlText;
			this.ExecQuery.Location = new System.Drawing.Point(404, 143);
			this.ExecQuery.Name = "ExecQuery";
			this.ExecQuery.Size = new System.Drawing.Size(190, 35);
			this.ExecQuery.TabIndex = 13;
			this.ExecQuery.Text = "Search...";
			this.ExecQuery.Click += new System.EventHandler(this.ExecQuery_Click);
			// 
			// ParentGrid
			// 
			this.ParentGrid.AlternatingBackColor = System.Drawing.Color.FromArgb(((System.Byte)(238)), ((System.Byte)(238)), ((System.Byte)(238)));
			this.ParentGrid.BackgroundColor = System.Drawing.SystemColors.AppWorkspace;
			this.ParentGrid.CaptionBackColor = System.Drawing.SystemColors.ControlText;
			this.ParentGrid.DataMember = "";
			this.ParentGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ParentGrid.GridLineColor = System.Drawing.SystemColors.Control;
			this.ParentGrid.HeaderBackColor = System.Drawing.SystemColors.Control;
			this.ParentGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.ParentGrid.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.ParentGrid.Location = new System.Drawing.Point(8, 200);
			this.ParentGrid.Name = "ParentGrid";
			this.ParentGrid.PreferredColumnWidth = 140;
			this.ParentGrid.PreferredRowHeight = 18;
			this.ParentGrid.ReadOnly = true;
			this.ParentGrid.RowHeaderWidth = 70;
			this.ParentGrid.SelectionBackColor = System.Drawing.SystemColors.ControlText;
			this.ParentGrid.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ParentGrid.Size = new System.Drawing.Size(608, 416);
			this.ParentGrid.TabIndex = 12;
			this.ParentGrid.CurrentCellChanged += new System.EventHandler(this.ParentGrid_CurrentCellChanged);
			this.ParentGrid.Leave += new System.EventHandler(this.ParentGrid_Leave);
			this.ParentGrid.Scroll += new System.EventHandler(this.ParentGrid_Scroll);
			// 
			// ReportGridToolTip
			// 
			this.ReportGridToolTip.ShowAlways = true;
			// 
			// PGUserControl
			// 
			this.Controls.Add(this.ParentGrid);
			this.Controls.Add(this.panel1);
			this.DockPadding.All = 8;
			this.Name = "PGUserControl";
			this.Size = new System.Drawing.Size(624, 624);
			this.Load += new System.EventHandler(this.PGUserControl_Load);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ParentGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		
		private void PGUserControl_Load(object sender, System.EventArgs e)
		{				
			//ParentGrid.Height = (this.Height*16)/31;
			//ParentGrid.Width = (this.Width*20)/21;
			//ExecQuery.Location =new Point(this.Width-(ExecQuery.Width + (this.Width-ParentGrid.Width)-10), ExecQuery.Location.Y ) ;
			//StatusLabel.Location=  new Point( ContainerPanel.Location.X,ContainerPanel.Location.Y+ContainerPanel.Height+6  );      

			cnfgFile =new XmlDocument ();
			CriteriaTable=new Hashtable();    
			ConditionTable=new Hashtable(); 			
			AggregatableColumnNumber =-1;

 
			if (cnfgFileName == null || cnfgFileName.Length==0  )
			{
				cnfgFileName=Application.StartupPath +"\\"+this.Name+".xml" ;
			}			
				try
				{					
					cnfgFile.Load(cnfgFileName); 
				}
				catch(System.Exception ex)
				{
                    MessageBox.Show(ex.Message);
					this.Enabled =false;
					this.ConfigFile ="";
					return;
				}        
       		
			//Doc is loaded				
			foreach (XmlNode SCNode in cnfgFile.DocumentElement.ChildNodes )
			{
                SearchCriteria.Items.Add(SCNode.Attributes["Value"].Value )  ;

			}

			if (SearchCriteria.Items.Count>0)
			{
				SearchCriteria.SelectedIndex =0;

			}
           
		}

		private void SearchCriteria_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			         
			try
			{
				if(CriteriaID!=(string)SearchCriteria.Text)
				{
					/////Clear Grid

					try
					{
						ParentGrid.DataBindings.Clear();  
						ParentGrid.DataSource =null;
						ParentGrid.Refresh ();   
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message);    
					}

					//////
					CriteriaID =(string)SearchCriteria.Text;
					CriteriaTable.Clear();  
				
					foreach (XmlNode SCNode in cnfgFile.DocumentElement.ChildNodes.Item(SearchCriteria.SelectedIndex))
					{
						if (SCNode !=null && SCNode.Name == "ReportCondition" )
						{
							CriteriaTable[SCNode.Attributes["ID"].Value]=SCNode ; 								
							//Form Criteria Hashtable
						}

					}
					
					loadReportQueries(); 
					ContainerPanel.Focus();   
				}					
			}
			catch(Exception ex)
			{
                MessageBox.Show (ex.Message);
			}
            
		}

		private void loadReportQueries()
		{		
			ContainerPanel.Controls.Clear();

			Label[] queryLabel=new Label[CriteriaTable.Keys.Count];	
			ComboBox[] queryCondition=new ComboBox[CriteriaTable.Keys.Count];	
			ComboBox[] queryDomain=new ComboBox[CriteriaTable.Keys.Count];	
			ConditionTable.Clear(); 
		
			int x=0;	 
			foreach(string QueryItem in CriteriaTable.Keys)
			{	
				//////////////
				string tempQueryItem=QueryItem.Replace(" ","_"); 

				queryLabel[x]=new Label();
				queryLabel[x].Font=new Font(SearchCriteria.Font,FontStyle.Underline);  
				queryLabel[x].Name =tempQueryItem.ToString();  
				queryLabel[x].Text =QueryItem.ToString();  
				queryLabel[x].Width =110;
				queryLabel[x].BackColor =ContainerPanel.BackColor ;
				queryLabel[x].ForeColor =ContainerPanel.ForeColor ;
				queryLabel[x].TextAlign =ContentAlignment.MiddleLeft; 
				queryLabel[x].Location =new Point( 10,(x)*30 + 15);	
 
				ContainerPanel.Controls.Add(queryLabel[x]);					
				///////////
				queryCondition[x]=new ComboBox();
				queryCondition[x].Name =tempQueryItem+"_Condition" ;

				queryCondition[x].DropDownStyle =ComboBoxStyle.DropDownList ;
				queryCondition[x].Width =190;
				queryCondition[x].BackColor =ContainerPanel.BackColor ;
				queryCondition[x].ForeColor =ContainerPanel.ForeColor ;
				queryCondition[x].Location =new Point(140,(x)*30 + 15) ;				
				ContainerPanel.Controls.Add(queryCondition[x]);					
				queryCondition[x].SelectedIndexChanged +=new System.EventHandler(this.queryCondition_SelectedIndexChanged);

				///////////////////
				queryDomain[x]=new ComboBox();
				queryDomain[x].Name =tempQueryItem+"_Domain" ;
				queryDomain[x].Width =(ContainerPanel.Width*4)/5 ;
				queryDomain[x].Width =400;
				queryDomain[x].BackColor =ContainerPanel.BackColor ;
				queryDomain[x].ForeColor =ContainerPanel.ForeColor ;
				queryDomain[x].Location =new Point(380,(x)*30 + 15) ;   
				queryDomain[x].DropDownStyle  =ComboBoxStyle.Simple;
				queryDomain[x].Height =25;	
				queryDomain[x].TabIndex= queryCondition[x].TabIndex +1;//V.Imp.

				queryDomain[x].KeyPress+=new System.Windows.Forms.KeyPressEventHandler(this.queryDomain_KeyPressed); 
				queryDomain[x].TextChanged+=new EventHandler(queryDomain_TextChanged);  				
				//Parameter_Changed

				ContainerPanel.Controls.Add(queryDomain[x]); 
				
                
				////////////////

				XmlNode  SCNode=(XmlNode)(CriteriaTable[QueryItem]);

				
//				queryDomain[x].Tag = SCNode.FirstChild.Attributes["DataType"].Value ;
//				Data type identification and Tagging

				foreach(XmlNode SCSubNode in SCNode.FirstChild.FirstChild.ChildNodes)				
				{	
					if (SCSubNode.Attributes.Count==2)  
					{
						queryCondition[x].Items.Add(SCSubNode.Attributes.Item(1).Value); 					
						ConditionTable[tempQueryItem+"_Condition_"+SCSubNode.Attributes.Item(1).Value]=SCSubNode.FirstChild;
						//Form Condition Hashtable
					}
					else
					{
						queryCondition[x].Items.Add("N/A"); 

					}	

				}
				try
				{
					if(queryCondition[x].Items.Count>0)
					{
                        queryCondition[x].SelectedIndex =0;

					}
				}
				catch(Exception e){MessageBox.Show(e.Message);}

				x++;
			}            
			
		}

		private void queryDomain_KeyPressed(object sender,KeyPressEventArgs e)
		{
			if((int)e.KeyChar==13)
			{
                ExecuteQueryAndFillGrid();				
			}

		}


		private void queryCondition_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComboBox tempCB=(ComboBox)sender;
			XmlNode DomainNode=(XmlNode)ConditionTable[tempCB.Name+"_"+ tempCB.Text] ;
			//Uses Condition Hashtable
			ComboBox tempDomainCB= (ComboBox)ContainerPanel.GetNextControl(tempCB,true);
			if (tempDomainCB==null)
			{
				MessageBox.Show ("Domain Box is null");
				return;

			}
			//Clear;
			tempDomainCB.Items.Clear();  

			//Get next Control	

			if(DomainNode.Attributes["Fixed"]!=null)
			{
				if(DomainNode.Attributes["Fixed"].Value=="True")
				{
					tempDomainCB.Text= Convert.ToString(fixedPropTable[((Label)(ContainerPanel.GetNextControl(tempCB,false))).Text]); 
					tempDomainCB.Enabled=false;
				}
			}
		
			switch(DomainNode.Attributes["Type"].Value)
			{
				case "TextBox":
				{
					tempDomainCB.DropDownStyle =ComboBoxStyle.Simple ;
					break;
				}
				case "ComboBox":
				{
					tempDomainCB.DropDownStyle =ComboBoxStyle.DropDown ;
					tempDomainCB.Sorted =true;

					break;
				}
				case "ListBox":
				{
					tempDomainCB.DropDownStyle =ComboBoxStyle.DropDownList ;				
					tempDomainCB.Sorted =true;
					break;
				}
				default:
				{
					break;
				}
			}
			if(DomainNode.HasChildNodes)
			{
				if(DomainNode.FirstChild.Name=="Value"  )
				{
					foreach (XmlNode ValueNode in DomainNode.ChildNodes )
					{
						tempDomainCB.Items.Add(ValueNode.InnerText);   
					}

				}
				else if(DomainNode.FirstChild.Name=="Query"  )
				{
					//Run the associated query on the DataSet,fetch row and populate ComboBox					
					try
					{
						if (ActiveCon !=null)
						{	
							DataSet QueryDS=new DataSet(); 
							string domainQry=DomainNode.FirstChild.InnerText;
							if(domainQry.IndexOf("{")>0)
							{
								string [] domainQryParts=domainQry.Split(new char[]{'{','}'}); 	
								domainQry=""; 
								for(int p=0;p<domainQryParts.Length;p++)
								{
									if(p%2==0)
									{
										domainQry+= domainQryParts[p];
									}
									else
									{
										domainQry+="'"+Convert.ToString(fixedPropTable[domainQryParts[p]])+"'";
									}
										
								}

							}
							OleDbDataAdapter QueryDA=new OleDbDataAdapter (domainQry,ActiveCon);
							if (QueryDA !=null)
							{
								int RowsFilled=QueryDA.Fill(QueryDS) ; 
								if (RowsFilled>0 && QueryDS !=null)
								{
									foreach (DataRow QueryRow in QueryDS.Tables[0].Rows)
									{
										if(!QueryRow.IsNull(0)) 
										{
											tempDomainCB.Items.Add(QueryRow[0])  ;
										}

									}
  
									QueryDS.Clear();
								}								
								QueryDA.Dispose(); 
							}						

						}
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message);  

					}

				}

				if (tempDomainCB.Items.Count>0 && tempDomainCB.DropDownStyle ==ComboBoxStyle.DropDownList )
				{
					tempDomainCB.SelectedIndex =0;

				}
				

			}

			
		}

		private void ExecQuery_Click(object sender, System.EventArgs e)
		{
			DoRefresh();

		}
		public void DoRefresh()
		{
			try
			{
				this.Cursor=Cursors.WaitCursor;
				this.StatusLabel.Text="Please wait. Retrieving.."; 
				Application.DoEvents();  
				selIndex=-1;
				if(this.BeforeSearch_Clicked==null)
				{
					ExecuteQueryAndFillGrid();
				}
				else
				{
					if(BeforeSearch_Clicked())
					{ExecuteQueryAndFillGrid();}
					else
					{
						return ;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("An exception has occured.\n"+ex.Message+"\nPlease Retry again.","Exception!",MessageBoxButtons.OK,MessageBoxIcon.Error ) ;
			}
			finally
			{
				this.Cursor=Cursors.Arrow;				
				Application.DoEvents();  
			}
		
		}

		public double getTotal(int colIndex)
		{
			double Aggregate=0;
			if(colIndex != -1)			
			{	
				             
				int rMax=ParentGrid.BindingContext[ParentGrid.DataSource,ParentGrid.DataMember].Count  ;
                
				try
				{

					for(int rCount=0;rCount<rMax;rCount++)         
					{						
						double temp=ParentGrid[rCount,colIndex]==null?0:Convert.ToDouble(ParentGrid[rCount,colIndex]);
						Aggregate+=temp;
					}	
				}
				catch(Exception ex)
				{					
					return 0.0;
				}
                                
			}
			return Aggregate ;


		}


		private void ExecuteQueryAndFillGrid()
		{
			string DGSourceQuery=null;
			DataSet ds=new DataSet();
			
			ds.Relations.Clear();
			ds.Tables.Clear();  
			ds.Clear(); 
			
			ParentGrid.DataBindings.Clear();  
			ParentGrid.DataSource =null;
			ParentGrid.Refresh ();            
			
			try
			{
				DGSourceQuery=GenerateQueryFromControl();		
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);		
				ds.Clear();
				ds.Dispose(); 
				return;

			}

			try
			{
				if(QueryParsed!=null)
				{
					if(QueryParsed(DGSourceQuery)==false)
					{
						ds.Clear();
						ds.Dispose(); 
						return;						
					}

				}
			}
			catch(Exception ex){}
			
			
			int RowsFetched=0;		
			StatusLabel.Text =""; 

			if (ActiveCon !=null)
			{				
				try
				{
					OleDbDataAdapter dbSrcAdapter;
					dbSrcAdapter=new OleDbDataAdapter(DGSourceQuery,ActiveCon);										
					RowsFetched=dbSrcAdapter.Fill(ds,MasterTable);	
					dbSrcAdapter.Dispose();
					
				}
				catch(Exception ex)	
				{
					MessageBox.Show(ex.Message);	
				}		

				foreach(XmlNode DetailSetNode in cnfgFile.DocumentElement.ChildNodes.Item(SearchCriteria.SelectedIndex).SelectSingleNode("ResultSet").SelectNodes("DetailSet") )
				{
					try
					{
						string DetailSetQuery=ParseQuery(DetailSetNode.InnerText); 
						OleDbDataAdapter dbAdaTemp=new OleDbDataAdapter(DetailSetQuery,ActiveCon);
						dbAdaTemp.Fill(ds,DetailSetNode.Attributes["TableName"].Value);
						dbAdaTemp.Dispose(); 
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message);  
					}
					
				}


				foreach(XmlNode RelationNode in cnfgFile.DocumentElement.ChildNodes.Item(SearchCriteria.SelectedIndex).SelectSingleNode("ResultSet").SelectNodes("Relation") )
				{
					try
					{	
						DataRelation myDataRelation;
						string strMasterTable=RelationNode.Attributes["Master"].Value;
						string strChildTable=RelationNode.Attributes["Details"].Value;

						string strParentBindingColumn=null;
						string strChildBindingColumn=null;
						if(RelationNode.Attributes["BindingColumn"]!=null)
						{							
							strParentBindingColumn=RelationNode.Attributes["BindingColumn"].Value;							
							strChildBindingColumn=strParentBindingColumn;
						}
						else
						{
							strParentBindingColumn=RelationNode.Attributes["ParentBindingColumn"].Value;
							strChildBindingColumn=RelationNode.Attributes["ChildBindingColumn"].Value;

						}

						string strRelationName=RelationNode.Attributes["Name"].Value;

						DataColumn colParent=ds.Tables[strMasterTable].Columns[strParentBindingColumn];
						DataColumn colChild=ds.Tables[strChildTable].Columns[strChildBindingColumn];

						myDataRelation = new DataRelation							
							(
							strRelationName							
							,colParent
							,colChild
							,false
							);

						ds.Relations.Add(myDataRelation);
					
					}
					catch(Exception ex)	
					{
						MessageBox.Show(ex.Message);  
					}
					
				}
				try
				{
					if(DataSetFormed!=null)
					{
						DataSetFormed(ds ,BindableObject); 
					}
				}
				catch(Exception ex){}

				try
				{
					ParentGrid.SetDataBinding(ds,BindableObject);				

				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);  
				} 

			}
			 
			StatusLabel.Text =RowsFetched.ToString()+" rows fetched" ;     
     
			if (this.Search_Clicked!=null)
			{
				this.Search_Clicked(RowsFetched,DGSourceQuery); 
                
			}
			
			ds.Dispose(); 

		}

	
		private string GenerateQueryFromControl()
		{  
			string UnparsedQuery=null;
			try
			{
				XmlNode MasterNode=cnfgFile.DocumentElement.ChildNodes.Item(SearchCriteria.SelectedIndex).SelectSingleNode("ResultSet").FirstChild;
				if(MasterNode!=null)
				{
					if(MasterNode.Name=="MasterSet")
					{
						BindableObject =MasterNode.ParentNode.Attributes["BindTo"].Value; 
						MasterTable=MasterNode.Attributes["TableName"].Value ; 
						UnparsedQuery=MasterNode.InnerText ;    
					}

				}				

			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				return null;
			}			
			return ParseQuery(UnparsedQuery) ;

		}

		private string ParseQuery(string QueryToBind)
		{		

			string[] QueryParts=QueryToBind.Split(new char[]{'{','}'}); 			
			
			foreach(string QueryPart in QueryParts)
			{
				if (CriteriaTable.Contains(QueryPart) )
				{
					XmlNode node=(XmlNode)CriteriaTable[QueryPart];
					string strComparable=null,strCondition=null,strDomain=null; 


					//////////////////////
					strComparable =node.FirstChild.Attributes["Mapsto"].Value;

					///////////////////
					string tempQueryPart=QueryPart.Replace(" ","_");
					string ConditionControlSelectedText=null;
					string DomainControlSelectedText="%";
					foreach (Control tempConditionCB in ContainerPanel.Controls)
					{
						if (tempConditionCB.GetType().Name=="ComboBox" &&  tempConditionCB.Name.Length>0)
						{
							if( tempConditionCB.Name==tempQueryPart+"_Condition")
							{
								ConditionControlSelectedText=((ComboBox)tempConditionCB).Text.ToString ();
								ComboBox tempDomainCB=((ComboBox)ContainerPanel.GetNextControl(tempConditionCB,true));

								if(tempDomainCB!=null)
								{
									DomainControlSelectedText=(string)tempDomainCB.Text;
									try
									{
										DomainControlSelectedText=(DomainControlSelectedText.Length>0)? DomainControlSelectedText:"%";
									}
									catch(Exception ex)
									{										
										DomainControlSelectedText="%";
									}
									break;
								}
							}

						}
					}
					XmlNode DomainNode=null;
					try
					{
						DomainNode=((XmlNode)ConditionTable[tempQueryPart+"_Condition_"+ConditionControlSelectedText]);
					}
					catch(Exception e){MessageBox.Show(e.Message);}

					
					/////////////////////////	
					
					strCondition=" Like ";	//Default
                    

//					if(node.FirstChild.Attributes["DataType"].Value =="String")
//					{			
					if (DomainNode.Attributes["Key"]!=null )
					{					
						if(DomainNode.Attributes["Key"].Value =="%")
						{							
							
							strDomain=" '"+DomainControlSelectedText+"%' ";							
							
                                
						}
						else if(DomainNode.Attributes["Key"].Value =="%%")
						{
							
							strDomain=" '%"+DomainControlSelectedText+"%' ";							

						}
						else
						{
							if(DomainControlSelectedText!="%")
							{
								strCondition=" "+ DomainNode.ParentNode.Attributes["Value"].Value+" ";
								string delimiter=DomainNode.Attributes["Key"].Value.Substring(0,1); 

								strDomain=delimiter+DomainControlSelectedText+delimiter;
							}
							else
							{
								//strDomain=" '"+DomainControlSelectedText+"' or "+strComparable+" is null ";
								strComparable=" 1";
								strCondition="=";
								strDomain="1 ";
								//exhaustive search scenario with delimiter
							}

						}

					}
					else
					{
						if(DomainControlSelectedText!="%")
						{
							//if(DomainNode.ParentNode.Attributes["Value"].Value!= "=" )	//We want '=' to be converted to 'like' too
							
							strCondition=" "+ DomainNode.ParentNode.Attributes["Value"].Value+" ";
							if(node.FirstChild.Attributes["DataType"].Value =="String")
							{
								strDomain=" '"+DomainControlSelectedText+"' ";
							}
							else
							{
								string delimiter=" ";
								strDomain=delimiter+DomainControlSelectedText+delimiter;//no-string comparison scenario

							}
                                
						}
						else
						{
							//strDomain=" '"+DomainControlSelectedText+"' or "+strComparable+" is null ";
							strComparable=" 1";
							strCondition="=";
							strDomain="1 ";
							//exhaustive search scenario
						}
					}
                        				
	//				}

					QueryToBind=QueryToBind.Replace("{" +QueryPart+"}",strComparable+strCondition+strDomain);

				}
			}
						

			return QueryToBind;



		}

		private void AggregateMenuItem_Click(object sender, System.EventArgs e)
		{
			double Aggregate=0;
			if(AggregatableColumnNumber != -1)			
			{	
				             
			int rMax=ParentGrid.BindingContext[ParentGrid.DataSource,ParentGrid.DataMember].Count  ;
                
				try
				{

					for(int rCount=0;rCount<rMax;rCount++)         
					{						
						double temp=ParentGrid[rCount,AggregatableColumnNumber]==null?0:Convert.ToDouble(ParentGrid[rCount,AggregatableColumnNumber]);
                        Aggregate+=temp;
					}	
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
					return;
				}
                                
			}

			if (this.Total_Clicked!=null)
			{
				this.Total_Clicked(AggregatableColumnNumber,Aggregate); 
                
			}
		}

		private void ParentGrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{			
			if(IsTotallingEnabled)
			{
				if(e.Button.ToString()=="Right"  )
				{

					System.Windows.Forms.DataGrid.HitTestInfo hti;			
					hti = ParentGrid.HitTest(e.X, e.Y);		

					if(hti.Type== System.Windows.Forms.DataGrid.HitTestType.ColumnHeader )
					{	
						AggregatableColumnNumber =hti.Column ;
						GridColumnAggregateMenu.Show(ParentGrid ,new Point( e.X,e.Y));  									
				
					}
				}
			}			

		}

		

		
				/// <summary>
		/// //xml file to generate Report Controls and Conditions
		/// </summary>
		/// 
		[Browsable(true)]
		public String ConfigFile
		{
			get
			{
				return cnfgFileName;
			}
			set
			{
				cnfgFileName=value;

			}
		}		


		[Browsable(true)]
		public bool TotallingEnabled
		{
			get
			{
				return IsTotallingEnabled;
			}
			set
			{
				IsTotallingEnabled=value;

			}
		}

		/// <summary>
		/// //Active Connection
		/// </summary>
		 [Browsable(false)]
		public OleDbConnection DBCon
		{
			get
			{
				return ActiveCon ;
			}
			set
			{
				ActiveCon=value;
			}
		}


		[Browsable(false)]
		public ComboBox CriteriaBox
		{
			get
			{
				return this.SearchCriteria; 
			}
			
		}


		[Browsable(false)]
		public Panel QueryPanel
		{
			get
			{
				return this.ContainerPanel ;
			}
			
		}

		[Browsable(false)]
		public DataGrid ReportGrid
		{
			get
			{
				return this.ParentGrid; 
			}
			
		}

		[Browsable(false)]		
		public Label ResultStatus
		{
			get
			{
				return this.StatusLabel; 
			}
			
		}

		[Browsable(false)]		
		public Button SearchButton
		{
			get
			{
				return this.ExecQuery ;
			}
			
		}

		public Hashtable FixedPropertyTable
		{
			get
			{
				return fixedPropTable;
			}			
		}

		private void queryDomain_TextChanged(object sender, EventArgs e)
		{
			if(Parameter_Changed!=null)
			{
				Parameter_Changed(sender);
			}
			
		}

		private void ParentGrid_Scroll(object sender, System.EventArgs e)
		{
			ParentGrid.Focus();
			Application.DoEvents();  
		}

		private void ParentGrid_CurrentCellChanged(object sender, System.EventArgs e)
		{

			int i=this.ReportGrid.CurrentRowIndex  ;
			if(i!=selIndex && SelectedIndex_Changed!=null)
			{
				SelectedIndex_Changed(sender,e); 
			}
			selIndex=i; 

		
		}

		private void ParentGrid_Leave(object sender, System.EventArgs e)
		{
			selIndex=-1;
		}

		
	}
}
