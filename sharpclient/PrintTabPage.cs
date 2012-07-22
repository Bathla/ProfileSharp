using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data ;  
using System.Xml ;
using System.IO ;
using System.Runtime.InteropServices ; 
namespace SharpClient
{
	/// <summary>
	/// Summary description for PrintTabPage.
	/// </summary>
	public class PrintTabPage:SharpClient.UI.Controls.TabPage
	{
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TreeView treePreview;
		private System.Windows.Forms.GroupBox groupPreviewSettings;
		private System.Windows.Forms.ComboBox templateBox;
		private System.Windows.Forms.Button webPreview;
		private System.Windows.Forms.Button printPreview;
		private AxSHDocVw.AxWebBrowser axPreviewBrowser;
		private System.ComponentModel.IContainer components;	
		private System.Data.DataSet  ds;
		private ToolTip treeToolTip;
		private TrackBar zoomTracker;
		private object missing;
		private int zoomIndex;
		private System.Windows.Forms.Label label1;

		public PrintTabPage()
		{
			Init();			
			this.Enabled=false; 			
			missing=System.Type.Missing ;
			zoomIndex=100;
		}

		private void Init()
		{			
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PrintTabPage));
			this.treePreview = new System.Windows.Forms.TreeView();
			treeToolTip=new ToolTip(); 
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.groupPreviewSettings = new System.Windows.Forms.GroupBox();
			this.printPreview = new System.Windows.Forms.Button();
			this.webPreview = new System.Windows.Forms.Button();
			this.templateBox = new System.Windows.Forms.ComboBox();
			this.axPreviewBrowser = new AxSHDocVw.AxWebBrowser();
			this.groupPreviewSettings.SuspendLayout();
			this.zoomTracker=new TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.axPreviewBrowser)).BeginInit();
			this.SuspendLayout();
            treeToolTip.AutoPopDelay = 5000;
			treeToolTip.InitialDelay = 700;
			treeToolTip.ReshowDelay = 500;
			treeToolTip.ShowAlways=true; 
			treeToolTip.SetToolTip(this.treePreview,"Select columns to preview."  );

			// 
			// treePreview
			// 
			this.treePreview.CheckBoxes = true;
			this.treePreview.Dock = System.Windows.Forms.DockStyle.Left;
			this.treePreview.ImageIndex = -1;
			this.treePreview.Location = new System.Drawing.Point(0, 0);
			this.treePreview.Name = "treePreview";
			this.treePreview.SelectedImageIndex = -1;
			this.treePreview.Size = new System.Drawing.Size(184, 621);
			this.treePreview.TabIndex = 8;
			this.treePreview.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treePreview_BeforeCheck);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(184, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 621);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// groupPreviewSettings
			// 
			this.groupPreviewSettings.Controls.Add(this.printPreview);
			this.groupPreviewSettings.Controls.Add(this.webPreview);
			this.groupPreviewSettings.Controls.Add(this.templateBox);
			this.groupPreviewSettings.Controls.Add(this.zoomTracker);
			this.groupPreviewSettings.Controls.Add(this.label1);
			this.groupPreviewSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupPreviewSettings.Location = new System.Drawing.Point(192, 0);
			this.groupPreviewSettings.Name = "groupPreviewSettings";
			this.groupPreviewSettings.Size = new System.Drawing.Size(696, 64);
			this.groupPreviewSettings.TabIndex = 10;
			this.groupPreviewSettings.TabStop = false;
			this.groupPreviewSettings.Text = "Choose a template";
			// 
			// printPreview
			// 
			this.printPreview.Cursor = System.Windows.Forms.Cursors.Hand;
			this.printPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.printPreview.Location = new System.Drawing.Point(268, 16);
			this.printPreview.Name = "printPreview";
			this.printPreview.Size = new System.Drawing.Size(112, 40);
			this.printPreview.TabIndex = 2;
			this.printPreview.Text = "Print Preview";
			this.printPreview.Click += new System.EventHandler(this.printPreview_Click);
			// 
			// webPreview
			// 
			this.webPreview.Cursor = System.Windows.Forms.Cursors.Hand;
			this.webPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.webPreview.Location = new System.Drawing.Point(148, 16);
			this.webPreview.Name = "webPreview";
			this.webPreview.Size = new System.Drawing.Size(112, 40);
			this.webPreview.TabIndex = 1;
			this.webPreview.Text = "Web Preview";
			this.webPreview.Click += new System.EventHandler(this.webPreview_Click);
			// 
			// templateBox
			// 
			this.templateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.templateBox.Location = new System.Drawing.Point(16, 24);
			this.templateBox.Name = "templateBox";
			this.templateBox.Size = new System.Drawing.Size(116, 21);
			this.templateBox.TabIndex = 0;
			// 
			// axPreviewBrowser
			// 
			this.axPreviewBrowser.Dock = System.Windows.Forms.DockStyle.Top;
			this.axPreviewBrowser.Enabled = true;
			this.axPreviewBrowser.Location = new System.Drawing.Point(192, 64);
			this.axPreviewBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPreviewBrowser.OcxState")));
			this.axPreviewBrowser.Size = new System.Drawing.Size(696, 557);
			this.axPreviewBrowser.TabIndex = 11;
			this.axPreviewBrowser.DocumentComplete+=new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(axPreviewBrowser_DocumentComplete); 

			//this.axPreviewBrowser.DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(this.axPreviewBrowser_DocumentComplete);
			// 
			// TestingForm
			// 			
			this.zoomTracker.LargeChange = 2;
			this.zoomTracker.Location = new System.Drawing.Point(452, 16);
			this.zoomTracker.Maximum = 100;
			this.zoomTracker.Minimum = 40;
			this.zoomTracker.Name = "zoomTracker";
			this.zoomTracker.Size = new System.Drawing.Size(80, 42);
			this.zoomTracker.TabIndex = 5;
			this.zoomTracker.TickFrequency = 10;
			this.zoomTracker.Value = 100;
			this.zoomTracker.Scroll+=new EventHandler(zoomTracker_Scroll);  
			

			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(404, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 23);
			this.label1.TabIndex = 6;
			this.label1.Text = "Zoom";
			this.label1.ForeColor=Color.Black;  


			this.ClientSize = new System.Drawing.Size(888, 621);
			this.Controls.Add(this.axPreviewBrowser);
			this.Controls.Add(this.groupPreviewSettings);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.treePreview);
			this.Name = "PrintTabPage";
			this.Text = "Web/Print Preview";
			this.VisibleChanged += new System.EventHandler(this.PrintTabPage_VisibleChanged);
			this.groupPreviewSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.axPreviewBrowser)).EndInit();
			this.ResumeLayout(false);
		}


		
		protected override void Dispose( bool disposing )
		{			
			if(ds!=null)
			{
				try
				{
					ds.Clear();
					ds.Dispose();
				}
				catch{}
				ds=null;
			}
			
			if( disposing )
			{				
				this.Events.Dispose();  
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void GetTemplates()
		{
			try
			{
				DirectoryInfo templateDir=new DirectoryInfo(Application.StartupPath+@"\Print Templates");
				if(templateDir==null)
				{
					throw new Exception("The print-template directory is not accessible!"); 
				}

				foreach(FileInfo templateFileInfo in templateDir.GetFiles("*.html") )
				{
					templateBox.Items.Add(templateFileInfo.Name);
				}
				if(templateBox.Items.Count>0)
				{
					templateBox.SelectedIndex=0; 
				}
				else
				{
					throw new Exception("No templates available"); 
				}
			}			
			catch(Exception ex)
			{				
				throw new Exception("There was an error enumerating print templates!\n"+ex.Message);				
			}
		}
		
		private void PrintTabPage_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible==true )
			{
				if(this.Enabled==false)
				{
					try
					{
						if(ds!=null)
						{
							TreeNode parentNode=treePreview.Nodes.Add(ds.DataSetName);   
							foreach(DataTable dTable in ds.Tables)
							{
								TreeNode dtNode=parentNode.Nodes.Add(dTable.TableName);
								foreach(DataColumn col in dTable.Columns )
								{		
									dtNode.Nodes.Add(col.ColumnName); 
								}				
							}
							treePreview.ExpandAll(); 
							GetTemplates();	
							object objFrame=missing;
							this.templateBox.SelectedIndexChanged += new System.EventHandler(this.templateBox_SelectedIndexChanged);
							//axPreviewBrowser.Navigate(Application.StartupPath+@"\Print Templates\" +Convert.ToString(templateBox.SelectedItem),ref missing,ref objFrame,ref missing,ref missing);																					
						}
					}
					catch(Exception ex)
					{
						MessageBox.Show("Preview failed!\n"+ex.Message,"Error!");
						foreach(Control control in this.Controls)
						{
							control.Enabled=false; 
						}
						SharpClient.SharpClientForm.scInstance.sharpClientMDITab.TabPages.Remove(this);      
					}
					this.Enabled=true; 
				}
			}
		}

		[Browsable(false)]		
		public object SourceData
		{
			get
			{
				return ds ;
			}
			set
			{
				if(value.GetType()==typeof(DataSet) )
				{
					if(ds!=null)
					{
						try
						{
							ds.Clear();
							ds.Dispose();
						}
						catch{}
						ds=null;
					}
					ds=(DataSet)value;
				}				
				else
				{
					throw new Exception("Invalid Data Source"); 
				}
			}
		}

		private void treePreview_BeforeCheck(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			TreeView parentTree=(TreeView)sender;
			foreach(TreeNode node in parentTree.Nodes)
			{
				if(e.Node==node)
				{
					MessageBox.Show("Please select a column node.");
					e.Cancel =true;
					return;
				}
				if(ds!=null)
				{
					foreach(TreeNode tNode in node.Nodes)
					{
						if(tNode==e.Node)
						{
							MessageBox.Show("Please select a column node.");
							e.Cancel =true;
							return;							
						}		
				
						foreach(TreeNode selNode in tNode.Nodes)
						{
							if(selNode.Checked && selNode.Parent !=e.Node.Parent )
							{
								selNode.Checked=false; 												
							}
						}
					}		
					
					
				}
			}
		
		}

		private void axPreviewBrowser_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
		{
			try
			{
				if(axPreviewBrowser.Document !=null)
				{
					object bodyObj=axPreviewBrowser.Document.GetType().InvokeMember("body" ,System.Reflection.BindingFlags.GetProperty,null,axPreviewBrowser.Document,null); 
					if(bodyObj!=null)
					{
						object styleObj=bodyObj.GetType().InvokeMember("style" ,System.Reflection.BindingFlags.GetProperty,null,bodyObj,null); 
						if(styleObj!=null)
						{							
							object []zoomVal=new object[1];
							zoomVal[0]=Convert.ToString(zoomIndex)+"%" ;
							styleObj.GetType().InvokeMember("zoom" ,System.Reflection.BindingFlags.SetProperty,null,styleObj,zoomVal); 

						}

					}

				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message,"Error!"); 
			}
		}


		private string ApplyTemplate(XmlDocument doc)
		{
			if(axPreviewBrowser.Document !=null)
			{
				object bodyObj=axPreviewBrowser.Document.GetType().InvokeMember("body" ,System.Reflection.BindingFlags.GetProperty,null,axPreviewBrowser.Document,null); 
				if(bodyObj!=null)
				{
					string outerHTML=Convert.ToString(bodyObj.GetType().InvokeMember("outerHTML" ,System.Reflection.BindingFlags.GetProperty,null,bodyObj,null)); 				

					if(outerHTML!=null)
					{
						try
						{
							outerHTML=outerHTML.Replace("Sample Data" ,treePreview.Nodes[0].Text );
						}
						catch{}

						int startIndex=outerHTML.ToUpper().IndexOf("<DATA>");
						int endIndex=outerHTML.ToUpper().IndexOf("</DATA>");
						if(startIndex <1 || endIndex <1)
						{
							return doc.OuterXml ;
						}

						string strToBeReplaced=outerHTML.Substring(startIndex,(endIndex - startIndex)+7);
						outerHTML=outerHTML.Replace(strToBeReplaced,doc.OuterXml);
						return outerHTML;                        
					}
				}
			}
			return doc.OuterXml;	

		}

		private string FormDocument()
		{			
			ArrayList nodeArray=new ArrayList() ;
			nodeArray.Clear();  
			
			foreach(TreeNode node in treePreview.Nodes[0].Nodes)
			{
				foreach(TreeNode cNode in node.Nodes)
				{
					if(cNode.Checked)
					{	
						nodeArray.Add(cNode.Parent.Text+"."+cNode.Text);						
					}
				}					
			}

			if( nodeArray.Count ==0)
				throw new Exception("No columns selected!"); 			
				

			XmlDocument doc=new XmlDocument ();
			int percent=100/(nodeArray.Count+1); 
			string strTotalWidth=Convert.ToString(percent*nodeArray.Count);
			doc.LoadXml("<center><table border=\"1\" width=\""+strTotalWidth+"%\"></table></center>"); 

			XmlNode rowHeaderNode= doc.DocumentElement.FirstChild.AppendChild(doc.CreateElement("tr"));   

			foreach(object obj in nodeArray)
			{
				XmlNode headerNode=rowHeaderNode.AppendChild(doc.CreateElement("td")); 
				XmlNode widthHeaderAttrNode = doc.CreateNode(XmlNodeType.Attribute, "width",null);
				widthHeaderAttrNode.Value  = Convert.ToString(percent)+"%";	

				headerNode.Attributes.SetNamedItem(widthHeaderAttrNode); 
				XmlNode boldNode=headerNode.AppendChild(doc.CreateElement("b"));
				boldNode.InnerText=Convert.ToString(obj).Split(new char[]{'.'} ,2)[1];
			}	
				
			string nodeString=Convert.ToString(nodeArray[0]);
			string nodeTable=nodeString.Split(new char[]{'.'} ,2)[0];
					
			foreach (System.Data.DataRow row in ds.Tables[nodeTable].Rows  )
			{						
				XmlNode rowNode= doc.DocumentElement.FirstChild.AppendChild(doc.CreateElement("tr"));   

				foreach(System.Data.DataColumn col in ds.Tables[nodeTable].Columns)
				{
					if(nodeArray.Contains(nodeTable+"."+col.ColumnName)) 					
					{	
						XmlNode colNode=rowNode.AppendChild(doc.CreateElement("td"));
						XmlNode widthColAttrNode = doc.CreateNode(XmlNodeType.Attribute, "width",null);
						widthColAttrNode.Value  = Convert.ToString(percent)+"%";	
						colNode.Attributes.SetNamedItem(widthColAttrNode);
						colNode.InnerText=(row[col]==DBNull.Value||row[col]==null)?"": Convert.ToString (row[col]) ; 	
					}					
				}	
			}

			string docStringToSave=ApplyTemplate(doc);  
			doc.RemoveAll();				
			return docStringToSave;
			
		}

		private void webPreview_Click(object sender, System.EventArgs e)
		{
			System.IO.TextWriter docWriter=null;	
			try
			{
				if(templateBox.SelectedItem==null)
				{
					MessageBox.Show("Please first select a template for printing from the [Choose Template] section");  
					return;
				}					
				string docStringToSave=FormDocument();
				if(docStringToSave==null)
					throw new Exception("Invalid/Incomplete data"); 
				docWriter=new StreamWriter(Application.StartupPath+ @"\PrintCache\PrintDocument.html" ,false,System.Text.Encoding.ASCII);
				docWriter.Write(docStringToSave); 
				docWriter.Close();	
				docWriter=null;	
				//System.Diagnostics.Process.Start("iexplore.exe","\""+Application.StartupPath+@"\PrintCache\PrintDocument.html" +"\"");
				string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
				System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+Application.StartupPath+@"\PrintCache\PrintDocument.html" +"\"");				
				pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
				pInfo.UseShellExecute =false;
				System.Diagnostics.Process.Start(pInfo);	
			}
			catch(Exception ex)
			{
				if(docWriter!=null)
				{
					docWriter.Close(); 
					docWriter=null;
				}				
				MessageBox.Show("Document for preview could not be formed!\n"+ex.Message,"Error!");				
			}
		}

		private void printPreview_Click(object sender, System.EventArgs e)
		{
			System.IO.TextWriter docWriter=null;	
			try
			{
				if(templateBox.SelectedItem==null)
				{
					MessageBox.Show("Please first select a template for printing from the [Choose Template] section");  
					return;
				}					
				string docStringToSave=FormDocument();
				if(docStringToSave==null)
					throw new Exception("Invalid/Incomplete data"); 
				docWriter=new StreamWriter(Application.StartupPath+ @"\PrintCache\PrintDocument.html" ,false,System.Text.Encoding.ASCII);
				docWriter.Write(docStringToSave); 
				docWriter.WriteLine("<script language=\"javascript\">");
				docWriter.WriteLine("	var WebBrowser='<OBJECT ID=WebBrowser1 WIDTH=0 HEIGHT=0 CLASSID=CLSID:8856F961-340A-11D0-A96B-00C04FD705A2></OBJECT>';");
				docWriter.WriteLine("	document.write(WebBrowser);");
				docWriter.WriteLine("	WebBrowser1.ExecWB(7,0);");
				docWriter.WriteLine("</script>");
				docWriter.Close();	
				docWriter=null;	
				//System.Diagnostics.Process.Start("iexplore.exe","\""+Application.StartupPath+@"\PrintCache\PrintDocument.html" +"\"");
				string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
				System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+Application.StartupPath+@"\PrintCache\PrintDocument.html" +"\"");				
				pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
				pInfo.UseShellExecute =false;
				System.Diagnostics.Process.Start(pInfo);	
			}
			catch(Exception ex)
			{
				if(docWriter!=null)
				{
					docWriter.Close(); 
					docWriter=null;
				}				
				MessageBox.Show("Document for preview could not be formed!\n"+ex.Message,"Error!");				
			}		
		}

		private void templateBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			object objFrame=missing;
			axPreviewBrowser.Navigate(Application.StartupPath+"\\Print Templates\\" +Convert.ToString(templateBox.SelectedItem),ref missing,ref objFrame,ref missing,ref missing);																
		}

		private void zoomTracker_Scroll(object sender, EventArgs e)
		{
			zoomIndex=zoomTracker.Value ;
			object objFrame=missing;
			axPreviewBrowser.Navigate(Application.StartupPath+"\\Print Templates\\" +Convert.ToString(templateBox.SelectedItem),ref missing,ref objFrame,ref missing,ref missing);

		}
	}
}
