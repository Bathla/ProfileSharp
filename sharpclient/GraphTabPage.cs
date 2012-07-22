using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml; 
using System.Data ;
using SharpClient.UI.Charting ; 
using PSUI;
namespace SharpClient
{
	/// <summary>
	/// Summary description for GraphTabPage.
	/// </summary>
	public class GraphTabPage:SharpClient.UI.Controls.TabPage
	{	
				/// <summary>
		/// ///////////////
		/// 
		/// </summary>
		/// 

		private System.ComponentModel.IContainer components;		
		private AbstractChart mChartToDraw;	
		private DataSet ds;
		private DataTable dt;
		private static Random randomizer = new Random();
		private DefaultDataSource m_source;
		private Hashtable m_hashTable;
		private TreeNode m_selNode;
		private PSUI.PSPanelGroup panelGroup;
		private System.Windows.Forms.Splitter splitter1;
		private PSUI.PSPanel psTreePanel;
		private PSUI.PSPanel psLegendPanel;
		private System.Windows.Forms.TreeView graphTree;
		private System.Windows.Forms.Panel legendPanel;
		private System.Windows.Forms.Panel panelCanvas;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox GroupByBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox BarGraph;
		private System.Windows.Forms.CheckBox PieGraph;
		//private System.Windows.Forms.CheckBox LineGraph;
		private System.Windows.Forms.Button btnAverages;
		private System.Windows.Forms.Panel canvasPanel;		

		private AbstractChart ChartToDraw 
		{
			set 
			{
				mChartToDraw = value;
			}
			get 
			{
				return mChartToDraw;
			}
		}

		protected override void Dispose( bool disposing )
		{
			this.Events.Dispose();  
			mChartToDraw=null;
			m_hashTable.Clear();
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
			if(dt!=null)
			{
				try
				{
					dt.Clear(); 
					dt.Dispose();
				}
				catch{}
				dt=null;				
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


		private void Init()
		{
			// 
			// GraphTabPage
			// 
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GraphTabPage));			
			this.panelGroup = new PSUI.PSPanelGroup();
			this.psLegendPanel = new PSUI.PSPanel(551);
			this.psTreePanel = new PSUI.PSPanel(552);
			this.graphTree = new System.Windows.Forms.TreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.legendPanel = new System.Windows.Forms.Panel();
			this.panelCanvas = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.GroupByBox = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.BarGraph = new System.Windows.Forms.CheckBox();
			this.PieGraph = new System.Windows.Forms.CheckBox();
//			this.LineGraph = new System.Windows.Forms.CheckBox();
			this.btnAverages = new System.Windows.Forms.Button();
			this.canvasPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.panelGroup)).BeginInit();
			this.panelGroup.SuspendLayout();
			this.psLegendPanel.SuspendLayout();
			this.psTreePanel.SuspendLayout();
			this.panelCanvas.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			
			// 
			// panelGroup
			// 
			this.panelGroup.AutoScroll = true;
			this.panelGroup.BackColor = System.Drawing.Color.Transparent;
			this.panelGroup.Controls.Add(this.psLegendPanel);
			this.panelGroup.Controls.Add(this.psTreePanel);
			this.panelGroup.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelGroup.Location = new System.Drawing.Point(0, 0);
			this.panelGroup.Name = "panelGroup";
			this.panelGroup.PanelGradient = ((PSUI.GradientColor)(resources.GetObject("panelGroup.PanelGradient")));
			this.panelGroup.Size = new System.Drawing.Size(216, 621);
			this.panelGroup.TabIndex = 8;
			// 
			// psLegendPanel
			// 
			this.psLegendPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psLegendPanel.BackColor = System.Drawing.Color.Transparent;
			this.psLegendPanel.Caption = "Legends";
			this.psLegendPanel.CaptionCornerType = PSUI.CornerType.Top;
			this.psLegendPanel.CaptionGradient.End = System.Drawing.Color.Gray;
			this.psLegendPanel.CaptionGradient.Start = System.Drawing.Color.Gray;
			this.psLegendPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psLegendPanel.CaptionUnderline = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(64)), ((System.Byte)(64)));
			this.psLegendPanel.Controls.Add(this.legendPanel);
			this.psLegendPanel.CurveRadius = 12;
			this.psLegendPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psLegendPanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psLegendPanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psLegendPanel.ImageItems.PSImgSet = null;
			this.psLegendPanel.Location = new System.Drawing.Point(8, 568);
			this.psLegendPanel.Name = "psLegendPanel";
			this.psLegendPanel.PanelGradient.End = System.Drawing.Color.White;
			this.psLegendPanel.PanelGradient.Start = System.Drawing.Color.White;
			this.psLegendPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psLegendPanel.PanelState = PSUI.PSPanelState.Collapsed;
			this.psLegendPanel.Size = new System.Drawing.Size(200, 33);
			this.psLegendPanel.TabIndex = 1;
			this.psLegendPanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psLegendPanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psLegendPanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psLegendPanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.psLegendPanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psLegendPanel.AnimationRate=0;  
			// 
			// psTreePanel
			// 
			this.psTreePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psTreePanel.BackColor = System.Drawing.Color.Transparent;
			this.psTreePanel.Caption = "Select column to graph";
			this.psTreePanel.CaptionCornerType = PSUI.CornerType.Top;
			this.psTreePanel.CaptionGradient.End = System.Drawing.Color.Gray;
			this.psTreePanel.CaptionGradient.Start = System.Drawing.Color.Gray;
			this.psTreePanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psTreePanel.CaptionUnderline = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(64)), ((System.Byte)(64)));
			this.psTreePanel.Controls.Add(this.graphTree);
			this.psTreePanel.CurveRadius = 12;
			this.psTreePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psTreePanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psTreePanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psTreePanel.ImageItems.PSImgSet = null;
			this.psTreePanel.Location = new System.Drawing.Point(8, 8);
			this.psTreePanel.Name = "psTreePanel";
			this.psTreePanel.PanelGradient.End = System.Drawing.Color.White;
			this.psTreePanel.PanelGradient.Start = System.Drawing.Color.White;
			this.psTreePanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psTreePanel.Size = new System.Drawing.Size(200, 552);
			this.psTreePanel.TabIndex = 0;
			this.psTreePanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psTreePanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psTreePanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psTreePanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.psTreePanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psTreePanel.AnimationRate =0;

			// 
			// graphTree
			// 
			this.graphTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphTree.ImageIndex = -1;
			this.graphTree.Location = new System.Drawing.Point(2, 35);
			this.graphTree.Name = "graphTree";
			this.graphTree.SelectedImageIndex = -1;
			this.graphTree.Size = new System.Drawing.Size(196, 514);
			this.graphTree.TabIndex = 0;
			this.graphTree.CheckBoxes =true; 
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.splitter1.Location = new System.Drawing.Point(216, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(2, 621);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// legendPanel
			// 
			this.legendPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.legendPanel.Location = new System.Drawing.Point(2, 35);
			this.legendPanel.Name = "legendPanel";
			this.legendPanel.Size = new System.Drawing.Size( 550,1000 );
			this.legendPanel.TabIndex = 0;
			// 
			// panelCanvas
			// 
			this.panelCanvas.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.panelCanvas.Controls.Add(this.canvasPanel);
			this.panelCanvas.Controls.Add(this.groupBox1);
			this.panelCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelCanvas.Location = new System.Drawing.Point(221, 0);
			this.panelCanvas.Name = "panelCanvas";
			this.panelCanvas.Size = new System.Drawing.Size(723, 621);
			this.panelCanvas.TabIndex = 10;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.GroupByBox);
			this.groupBox1.Controls.Add(this.btnAverages);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(723, 88);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group By";
			// 
			// GroupByBox
			// 
			this.GroupByBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.GroupByBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.GroupByBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.GroupByBox.Location = new System.Drawing.Point(3, 16);
			this.GroupByBox.Name = "GroupByBox";
			this.GroupByBox.Size = new System.Drawing.Size(293, 22);
			this.GroupByBox.TabIndex = 0;
			// 
			// groupBox2
			// 
//			this.groupBox2.Controls.Add(this.LineGraph);
			this.groupBox2.Controls.Add(this.PieGraph);
			this.groupBox2.Controls.Add(this.BarGraph);
			this.groupBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(304, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 64);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Select Representation";
			// 
			// PieGraph
			// 
			this.PieGraph.Location = new System.Drawing.Point(128, 16);
			this.PieGraph.Name = "PieGraph";
			this.PieGraph.Size = new System.Drawing.Size(96, 45);
			this.PieGraph.TabIndex = 10;
			this.PieGraph.Text = "Pie Graph";
			// 
			// BarGraph
			// 
			this.BarGraph.Location = new System.Drawing.Point(16, 16);
			this.BarGraph.Name = "BarGraph";
			this.BarGraph.Size = new System.Drawing.Size(101, 45);
			this.BarGraph.TabIndex = 9;
			this.BarGraph.Text = "Bar Graph";
			// LineGraph
			// 
//			this.LineGraph.Dock = System.Windows.Forms.DockStyle.Left;
//			this.LineGraph.Location = new System.Drawing.Point(200, 16);
//			this.LineGraph.Name = "LineGraph";
//			this.LineGraph.Size = new System.Drawing.Size(100, 45);
//			this.LineGraph.TabIndex = 2;
//			this.LineGraph.Text = "Line Graph";
			// 
			// btnAverages
			// 
			this.btnAverages.Location = new System.Drawing.Point(568, 24);
			this.btnAverages.Name = "btnAverages";
			this.btnAverages.Size = new System.Drawing.Size(128, 48);
			this.btnAverages.TabIndex = 11;
			this.btnAverages.Text = "Show Averages";
			this.btnAverages.Cursor=Cursors.Hand ;  
			// 
			// canvasPanel
			// 
			this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.canvasPanel.Location = new System.Drawing.Point(0, 88);
			this.canvasPanel.Name = "canvasPanel";
			this.canvasPanel.Size = new System.Drawing.Size(723, 533);
			this.canvasPanel.TabIndex = 1;
			// 
			// Test
						
			this.ClientSize = new System.Drawing.Size(944, 621);
			this.Controls.Add(this.panelCanvas);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panelGroup);
			this.Name = "GrapgTabPage";
			this.Text = "Graph";
			((System.ComponentModel.ISupportInitialize)(this.panelGroup)).EndInit();
			this.panelGroup.ResumeLayout(false);
			this.psLegendPanel.ResumeLayout(false);
			this.psTreePanel.ResumeLayout(false);
			this.panelCanvas.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);


			this.psTreePanel.Expanded+=new EventHandler(psTreePanel_Expanded);  
			this.psLegendPanel.Expanded+=  new EventHandler(psTreePanel_Expanded);  			
			this.graphTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.graphTree_AfterCheck);
			this.graphTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.graphTree_BeforeCheck);			
			this.btnAverages.Click += new System.EventHandler(this.btnAverages_Click);			
//			this.LineGraph.CheckedChanged += new System.EventHandler(this.LineGraph_CheckedChanged);			
			this.PieGraph.CheckedChanged += new System.EventHandler(this.PieGraph_CheckedChanged);
			this.BarGraph.CheckedChanged += new System.EventHandler(this.BarGraph_CheckedChanged);
			this.GroupByBox.SelectedIndexChanged +=new EventHandler(GroupByBox_SelectedIndexChanged);
			this.canvasPanel.SizeChanged += new System.EventHandler(this.canvasPanel_SizeChanged);
			this.canvasPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasPanel_Paint);
			this.ClientSize = new System.Drawing.Size(888, 621);			
			this.VisibleChanged += new System.EventHandler(this.GraphTabPage_VisibleChanged);
			this.legendPanel.Paint+=new PaintEventHandler(legendPanel_Paint);  			

		}
	
		public GraphTabPage()
		{
			Init(); 	
			m_hashTable=new Hashtable();
			this.Enabled=false; 			
		}
		
		private void canvasPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			try
			{				
				if(mChartToDraw != null) 
				{
					mChartToDraw.draw(e.Graphics);					
					legendPanel.Refresh(); 
				}
			}
			catch{	}		
		}

		private void canvasPanel_SizeChanged(object sender, System.EventArgs e)
		{	
			if(mChartToDraw != null) 
			{
				mChartToDraw.Size = canvasPanel.Size;  
			}
			canvasPanel.Refresh();
			legendPanel.Refresh(); 
		
		}

		private void initializeClientChart(AbstractChart chart) 
		{
			mChartToDraw = chart;
			chart.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText ; 
			if(chart is CartesianChart) 
			{
				((CartesianChart)chart).IsShowLabel = true;
			}
			chart.DataSource = m_source; 
			chart.Enable3D = true;
			canvasPanel.Refresh();  
			legendPanel.Refresh(); 
		}

		private void graphTree_BeforeCheck(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
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
							if(selNode.Checked && selNode!=e.Node)
							{
								selNode.Checked=false; 												
							}
						}
					}
					return;
				}
				if(dt!=null)
				{
					foreach(TreeNode selNode in node.Nodes)
					{
						if(selNode.Checked && selNode!=e.Node)
						{
							selNode.Checked=false; 												
						}	
					}
				}
			}
		}	

		private DefaultDataSource fillDataSource()
		{			
			DefaultDataSource source = new DefaultDataSource();
			Series series = new Series();
			source.add(series);
			updateCumulativeData();
			if(m_hashTable.Count>26)
			{	
				psLegendPanel.PanelState =PSUI.PSPanelState.Expanded ;
				double sizeMark=m_hashTable.Count/26.000  ;			
				psLegendPanel.Height=Convert.ToInt32(System.Math.Ceiling(sizeMark)*550);          
				psLegendPanel.Refresh(); 				
			}
 
			foreach(object key in m_hashTable.Keys)
			{
				series.add(new DataPoint(Convert.ToString(key),(float)Convert.ToDouble(m_hashTable[key]),Color.FromArgb(randomizer.Next(255),randomizer.Next(255),randomizer.Next(255)))); 
			}
			return source;
		}

		private void updateCumulativeData()
		{
			
			m_hashTable.Clear();
			Hashtable hashTableCount=new Hashtable ();
			hashTableCount.Clear();
			string gtable=Convert.ToString(GroupByBox.SelectedItem).Split(new char[]{'.'},2)[0];
			string gcolumn=Convert.ToString(GroupByBox.SelectedItem).Split(new char[]{'.'},2)[1];
			string table=m_selNode.Parent.Text ;
			string column=m_selNode.Text;
			if(ds!=null)
			{
				foreach(DataRow row in ds.Tables[gtable].Rows)
				{
					if(!m_hashTable.ContainsKey(row[gcolumn]))
					{
						m_hashTable[row[gcolumn]]=Convert.ToDouble(row[column]); 
					}
					else
					{
						m_hashTable[row[gcolumn]]=Convert.ToDouble(m_hashTable[row[gcolumn]])+Convert.ToDouble(row[column]); 
					}
					if(btnAverages.FlatStyle==FlatStyle.Flat   )
					{
						if(hashTableCount.ContainsKey(row[gcolumn]))
						{
							hashTableCount[row[gcolumn]]=Convert.ToInt32(hashTableCount[row[gcolumn]])+1;
						}
						else
						{
							hashTableCount[row[gcolumn]]=1;
						}
					}
				}

				if(btnAverages.FlatStyle==FlatStyle.Flat   )
				{
					foreach(object key in hashTableCount.Keys)
					{
						m_hashTable[key]=Convert.ToInt32(hashTableCount[key])==0?0:Convert.ToDouble(m_hashTable[key])/Convert.ToInt32(hashTableCount[key]);
					}
				}

				return;
			}
			if(dt!=null)
			{
				foreach(DataRow row in dt.Rows)
				{
					if(!m_hashTable.ContainsKey(row[gcolumn]))
					{
						m_hashTable[row[gcolumn]]=Convert.ToDouble(row[column]); 
					}
					else
					{
						m_hashTable[row[gcolumn]]=Convert.ToDouble(m_hashTable[row[gcolumn]])+Convert.ToDouble(row[column]); 
					}
					if(btnAverages.FlatStyle==FlatStyle.Flat   )
					{
						if(hashTableCount.ContainsKey(row[gcolumn]))
						{
							hashTableCount[row[gcolumn]]=Convert.ToInt32(hashTableCount[row[gcolumn]])+1;
						}
						else
						{
							hashTableCount[row[gcolumn]]=1;
						}
					}
				}

				if(btnAverages.FlatStyle==FlatStyle.Flat   )
				{
					foreach(object key in hashTableCount.Keys)
					{
						m_hashTable[key]=Convert.ToInt32(hashTableCount[key])==0?0:Convert.ToDouble(m_hashTable[key])/Convert.ToInt32(hashTableCount[key]);
					}
				}
			}				
		}	

	
		private void graphTree_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{			
			if(e.Node==null ||e.Node.Parent==null )
				return;
			GroupByBox.Items.Clear();  
			btnAverages.Enabled=false; 
			if(e.Node.Checked)
			{
				foreach(TreeNode node in e.Node.Parent.Nodes)
				{
					GroupByBox.Items.Add(e.Node.Parent.Text+"." +node.Text); 
				}
				if(GroupByBox.Items.Count>0)
				{
					GroupByBox.SelectedIndex=0;   
					btnAverages.Enabled=true; 
				}
			}

			m_selNode=e.Node; 
		}	
		
		private void btnAverages_Click(object sender, System.EventArgs e)
		{
			if(btnAverages.FlatStyle==FlatStyle.Flat   )
			{
				btnAverages.FlatStyle =FlatStyle.Standard;	
				btnAverages.Text ="Show Averages";
			}
			else
			{
				btnAverages.FlatStyle =FlatStyle.Flat; 				
				btnAverages.Text ="Show Discrete";			
			}
			if(BarGraph.Checked)
			{				
				BarGraph.Checked=false;  
				BarGraph.Checked=true;
			}
			else if(PieGraph.Checked)
			{
				PieGraph.Checked=false;  
				PieGraph.Checked=true;  
			}
//			else if(LineGraph.Checked)
//			{				
//				LineGraph.Checked=false;  
//				LineGraph.Checked=true;  
//			}
		}		

		private void BarGraph_CheckedChanged(object sender, System.EventArgs e)
		{
			if(BarGraph.Checked)
			{
				if(PieGraph.Checked)
				{
					PieGraph.Checked=false;  
				}

				if(btnAverages.Enabled)
				{
					if(m_source!=null)
					{
						m_source=null;
					}
					try
					{
						m_source=fillDataSource();						
						psLegendPanel.Refresh(); 
						initializeClientChart(new Barchart(canvasPanel.Size));
						
					}
					catch(Exception ex)
					{
						MessageBox.Show ("Unable to draw the chart.The selected column may not have numeric values.\n"+ex.Message,"Error!");
					}
				}
				m_hashTable.Clear();
			}			
		}

		private void PieGraph_CheckedChanged(object sender, System.EventArgs e)
		{
			if(PieGraph.Checked)
			{
				if(BarGraph.Checked)
				{
					BarGraph.Checked=false;
				}

				if(btnAverages.Enabled)
				{
					if(m_source!=null)
					{
						m_source=null;
					}
					try
					{
						m_source=fillDataSource();					
						psLegendPanel.Refresh(); 
						initializeClientChart(new Piechart(canvasPanel.Size));
						
					}
					catch(Exception ex)
					{
						MessageBox.Show ("Unable to draw the chart.The selected column may not have numeric values.\n"+ex.Message,"Error!");
					}
				}
				m_hashTable.Clear();	
			}
		
		}

//		private void LineGraph_CheckedChanged(object sender, System.EventArgs e)
//		{
//			if(LineGraph.Checked )
//			{
//				if(BarGraph.Checked)
//				{
//					BarGraph.Checked=false;
//				}
//				if(btnAverages.Enabled)
//				{
//					if(m_source!=null)
//					{
//						m_source=null;
//					}
//					try
//					{
//						m_source=fillDataSource();					
//						initializeClientChart(new LineChart(canvasPanel.Size));
//					}
//					catch(Exception ex)
//					{
//						MessageBox.Show ("Unable to draw the chart.The selected column may not have numeric values.\n"+ex.Message,"Error!");
//					}
//					m_hashTable.Clear();
//				}
//			}		
//		}

		private void GraphTabPage_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible==true )
			{
				if(this.Enabled==false)
				{
					try
					{
						if(ds!=null)
						{
							TreeNode parentNode=graphTree.Nodes.Add(ds.DataSetName);  
 
#warning "Add this feature" {
//							foreach(DataTable dTable in ds.Tables)
//							{
//								TreeNode dtNode=parentNode.Nodes.Add(dTable.TableName);
//								
//								foreach(DataColumn col in dTable.Columns )
//								{		
//									dtNode.Nodes.Add(col.ColumnName);  
// 
//								}				
//							}
#warning "Add this feature" }
								

								DataTable dTable=ds.Tables[0]; 
								TreeNode dtNode=parentNode.Nodes.Add(dTable.TableName);								
								foreach(DataColumn col in dTable.Columns )
								{		
									dtNode.Nodes.Add(col.ColumnName);   
								}											

							goto A;
						}
						if(dt!=null)
						{
							TreeNode parentNode=graphTree.Nodes.Add(dt.TableName);
							foreach(DataColumn col in dt.Columns )
							{					
								parentNode.Nodes.Add(col.ColumnName);   
							}
						}

					A:						graphTree.ExpandAll(); 			
					}
					catch
					{
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
				if(ds!=null)
				{
					return ds ;
				}
				else if(dt!=null)
				{
					return dt;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if(value.GetType()==typeof(DataSet) )
				{
					ds=(DataSet)value;
				}
				else
					if(value.GetType()==typeof(DataTable ) )
				{
					dt=(DataTable)value;
				}
				else
				{
					throw new Exception("Invalid Data Source"); 
				}
			}
		}

		private void GroupByBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(GroupByBox.Items.Count>0)
			{
				if(BarGraph.Checked)
				{
					BarGraph.Checked=false;  
					BarGraph.Checked=true;
				}
				else if(PieGraph.Checked)
				{
					PieGraph.Checked=false;  
					PieGraph.Checked=true;  
				}
//				else if(LineGraph.Checked)
//				{
//					LineGraph.Checked=false;  
//					LineGraph.Checked=true;  
//				}

			}
		}

		private void psTreePanel_Expanded(object sender, EventArgs e)
		{
			foreach (Control psControl in panelGroup.Controls) 
			{
				if(psControl.GetType()==typeof(PSPanel) && psControl!=sender  )
				{
					((PSPanel)psControl).PanelState =PSPanelState.Collapsed;  

				}

			}
		}

		private void legendPanel_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				if(mChartToDraw!=null)
				{
					mChartToDraw.drawLegend(e.Graphics);   
				}
			}
			catch{}
			base.OnPaint(e); 
		}	
	
		
	}
}
