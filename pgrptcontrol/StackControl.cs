using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PGRptControl
{
	/// <summary>
	/// Summary description for StackControl.
	/// </summary>
	/// 

	
	public delegate void StackControl_ItemSelectedHandler(string itemClicked);
		
	public class StackControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label entryDetail;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView StackListView;
		private bool bWizard;
		private System.ComponentModel.IContainer components;
		public event StackControl_ItemSelectedHandler StackControl_ItemSelected;
		private ColumnSorter TheColumnSorter = new ColumnSorter();
		private bool bSort=false;

		public StackControl(bool sort)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			bWizard=false;	
			bSort=sort;
			if(bSort==false)
			{
				StackListView.HeaderStyle =ColumnHeaderStyle.Nonclickable; 
				StackListView.Sorting=SortOrder.None;   
			}
			else
			{
				StackListView.HeaderStyle =ColumnHeaderStyle.Clickable; 
				StackListView.Sorting=SortOrder.Descending ;
			}
		}


		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			this.Events.Dispose();  
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.entryDetail = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.StackListView = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// entryDetail
			// 
			this.entryDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.entryDetail.Dock = System.Windows.Forms.DockStyle.Top;
			this.entryDetail.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.entryDetail.Location = new System.Drawing.Point(5, 5);
			this.entryDetail.Name = "entryDetail";
			this.entryDetail.Size = new System.Drawing.Size(694, 83);
			this.entryDetail.TabIndex = 4;
			// 
			// splitter1
			// 
			this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(5, 88);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(694, 5);
			this.splitter1.TabIndex = 5;
			this.splitter1.TabStop = false;
			// 
			// StackListView
			// 
			this.StackListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StackListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.StackListView.FullRowSelect = true;
			this.StackListView.GridLines = true;
			this.StackListView.HideSelection = false;
			this.StackListView.Location = new System.Drawing.Point(5, 93);
			this.StackListView.MultiSelect = false;
			this.StackListView.Name = "StackListView";
			this.StackListView.Size = new System.Drawing.Size(694, 390);
			this.StackListView.TabIndex = 6;
			this.StackListView.View = System.Windows.Forms.View.Details;
			this.StackListView.ItemActivate += new System.EventHandler(this.StackListView_ItemActivate);
			this.StackListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.StackListView_ColumnClick);
			// 
			// StackControl
			// 
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Controls.Add(this.StackListView);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.entryDetail);
			this.DockPadding.All = 5;
			this.Name = "StackControl";
			this.Size = new System.Drawing.Size(704, 488);
			this.ResumeLayout(false);

		}
		#endregion

		//Format:  "AllocationStack|Module","Function1^My1.dll|Function2^My2.dll|Function2^N:A"
		public void RefreshData(string headerList,string listData,string details,int ColumnHeaderValue)
		{			
			entryDetail.Text =details;
			StackListView.Items.Clear();
			Application.DoEvents(); 			

			if(StackListView.Columns.Count==0)
			{
				if(headerList!=null)
				{					
					string[] headerItems=headerList.Trim().Split(new char[]{'|'});
					foreach(string headerItem in headerItems)
					{
						StackListView.Columns.Add(headerItem,ColumnHeaderValue,System.Windows.Forms.HorizontalAlignment.Left );
					}
				}
			}
			

			if(listData!=null)
			{				
				string[] listItems=listData.Trim().Split(new char[]{'|'});
				for(int i=0;i<listItems.Length;i++)
				{	
					string[] listEntries=listItems[i].Trim().Split(new char[]{'^'});	
					ListViewItem liView=StackListView.Items.Add(listEntries[0]);
					for(int j=1;j<listEntries.Length;j++)
					{		
						liView.SubItems.Add(listEntries[j]);  
					}	
					
				}			
			}
						
		}

//		public void RefreshData(string headerList,string listData,int ColumnHeaderValue)
//		{
//			if(this.Controls.Contains(entryDetail))
//			{
//				this.Controls.Remove(entryDetail); 
//				StackListView.Dock=DockStyle.Fill ;
//			}
//		
//			
//			StackListView.Items.Clear();
//			Application.DoEvents();  
//			if(StackListView.Columns.Count==0)
//			{
//				if(headerList!=null)
//				{
//					string[] headerItems=headerList.Trim().Split(new char[]{'|'});
//					foreach(string headerItem in headerItems)
//					{						
//						StackListView.Columns.Add(headerItem,ColumnHeaderValue,System.Windows.Forms.HorizontalAlignment.Left );
//					}
//				}
//			}
//			
//
//			if(listData!=null)
//			{
//				string[] listItems=listData.Trim().Split(new char[]{'|'});
//				for(int i=0;i<listItems.Length;i++)
//				{					
//					string[] listEntries=listItems[i].Trim().Split(new char[]{','});	
//					ListViewItem liView=StackListView.Items.Add(listEntries[0]);
//					for(int j=1;j<listEntries.Length;j++)
//					{
//						if(listEntries[j]==null || listEntries[j].Length==0)
//						{
//							continue;
//						}
//						liView.SubItems.Add(listEntries[j]);  
//					}		
//					
//				}			
//			}
//
//			
//		}


		private void StackListView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			if(CanSort)
			{
				
				if(StackListView.ListViewItemSorter!=TheColumnSorter)
				{
					TheColumnSorter.sortOrder=SortOrder.Descending ;  
					StackListView.ListViewItemSorter = TheColumnSorter;					
				}
				else
				{
					TheColumnSorter.sortOrder=(TheColumnSorter.sortOrder==SortOrder.Descending)? SortOrder.Ascending:SortOrder.Descending  ;					
				}
				TheColumnSorter.CurrentColumn  = e.Column;				
				StackListView.Sort();
			}
		}

		
		private void StackListView_ItemActivate(object sender, System.EventArgs e)
		{
			if(bWizard)
			{
				if(this.StackControl_ItemSelected!=null && this.StackListView.SelectedItems.Count>0)
				{
					string strParam=this.StackListView.SelectedItems[0].Text;
					if(strParam!=null && strParam.Length>0)
					{
						this.StackControl_ItemSelected(strParam);
					}
				}

			}
		}

		public bool CanSort
		{
			get
			{
				return bSort;
			}			
		}		

		public bool Wizard
		{
			get
			{
				return bWizard;
			}
			set
			{
				if(value==true)
				{
					entryDetail.Font=new Font( "Verdana", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0))); 
					entryDetail.Height=60;				
					StackListView.MultiSelect=false;

					//Actually not needed, as we assume that sortable and wizard are mutually exclusive
					StackListView.HeaderStyle =ColumnHeaderStyle.Nonclickable; 
					StackListView.Sorting=SortOrder.None;   
					///////////////					

					StackListView.Activation=ItemActivation.OneClick;   
					StackListView.Font =new Font( "Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0))); 
					StackListView.ForeColor=System.Drawing.SystemColors.HotTrack;
					StackListView.GridLines=false; 
					StackListView.FullRowSelect=false;						
//					wizardToolTip.SetToolTip(StackListView,"Please choose an action to perform with ProfileSharp"); 
					bWizard=value;
					Application.DoEvents();  
				}
			}
		}		
	}

	public class ColumnSorter : IComparer
	{
		public int CurrentColumn = 0;
		public SortOrder sortOrder=SortOrder.Descending; 
		private CaseInsensitiveComparer objComparer=new CaseInsensitiveComparer();  
		public int Compare(object x, object y)
		{
			int sortResult=0;
			try
			{
				ListViewItem columnA=null;
				ListViewItem columnB=null;
								
				columnA = (ListViewItem)x;
				columnB = (ListViewItem)y;	
				
				if(columnA.SubItems.Count-1<CurrentColumn || columnB.SubItems.Count-1<CurrentColumn )
				{
					if(sortOrder==SortOrder.Ascending)   
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}				
				try	
				{					
					sortResult=objComparer.Compare(Convert.ToInt32(columnA.SubItems[CurrentColumn].Text.Trim()),
						Convert.ToInt32(columnB.SubItems[CurrentColumn].Text.Trim())); 
				}
				catch
				{
					sortResult=String.Compare(columnA.SubItems[CurrentColumn].Text.Trim() ,
						columnB.SubItems[CurrentColumn].Text.Trim() );
				}
				if(sortOrder==SortOrder.Descending)
				{
					return (-1 * sortResult);
				}				
			}
			catch{}	
			return sortResult;		
		}

		public ColumnSorter()
		{
			
		}
	}

	

}
