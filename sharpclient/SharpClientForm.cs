using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using SharpClient.UI ;
using SharpClient.UI.Docking; 
using System.Runtime.InteropServices;  
using System.IO; 
using System.Security.Cryptography; 
using System.Text;
namespace SharpClient
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	/// 
	public delegate void delegateToolbarEvent(object objClicked);
	public delegate void delegateMenuEvent(object objClicked);

	public class SharpClientForm : System.Windows.Forms.Form
	{
		
		public System.Windows.Forms.StatusBar psStatusBar;
		private SplashForm splashForm;
		private System.Windows.Forms.ToolBar psToolBar;
		private System.Windows.Forms.MainMenu psMainMenu;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.MenuItem menuOpen;
		private System.Windows.Forms.MenuItem menuClose;
		private System.Windows.Forms.MenuItem menuExport;
		private System.Windows.Forms.MenuItem menuNew;
		private System.Windows.Forms.MenuItem menuOpen_MemAnalysis;
		private System.Windows.Forms.MenuItem menuOpen_PerfAnalysis;
		public System.Windows.Forms.MenuItem menuExport_XML;
		public System.Windows.Forms.MenuItem menuExport_CSV;
		private System.Windows.Forms.MenuItem menuItem4;
		private Hashtable subscribersTable;
		private Hashtable subscribersMenuTable;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.StatusBarPanel panelSharpClientStatus;
		private System.Windows.Forms.OpenFileDialog openProfilerSession;
		public SharpClient.UI.Controls.TabControl codeTabControl;		
		public static Hashtable profilerTable=new Hashtable() ;
		private System.Windows.Forms.ToolBarButton toggleSettings;
		private System.Windows.Forms.MenuItem menuOpen_CompareAnalysis;
		private System.Windows.Forms.MenuItem menuView;
		public System.Windows.Forms.MenuItem menuGraph;
		private System.Windows.Forms.ToolBarButton createGraph;
		private System.Windows.Forms.ToolBarButton webPrintPreview;		
		public static SharpClientForm scInstance;
		private System.Windows.Forms.ToolBarButton codePreview;
		private System.Windows.Forms.ContextMenu openContextMenu;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.ToolBarButton openSession;
		private System.Windows.Forms.ToolBarButton newSession;
		private System.Windows.Forms.ContextMenu newContextMenu;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		public System.Windows.Forms.MenuItem menuPrintPreview;
		public System.Windows.Forms.MenuItem menuWebPreview;
		public System.Windows.Forms.ImageList imageList1;
		public System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuAbout;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private DockingManager _manager;
		private DockingManager _stackManager;
		private System.Windows.Forms.MenuItem menuItem11;
		private static string[] arguments;
		private IntPtr handle=IntPtr.Zero;
		private System.Windows.Forms.ToolBarButton openProfilerOptions;
		private System.Windows.Forms.MenuItem menuProfilerOptions;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuCustomization;
		public SharpClient.UI.Controls.TabControl sharpClientMDITab;
		//private ObjectMonitorClass objMonitor;
		System.Threading.Thread profileeThread;		
	
		[DllImport("Shell32.dll")]
		private static extern int IsUserAnAdmin();

		[DllImport("Kernel32.dll", SetLastError=true)]
		public static extern IntPtr OpenMutex(uint dwAccess,int Inherit,string lpName);

		[DllImport("Kernel32.dll")]
		public static extern int CloseHandle(IntPtr handle);	
	
		[DllImport("Kernel32.dll")]
		public static extern int ReleaseMutex(IntPtr handle);	

		[DllImport("Kernel32.dll", SetLastError=true)]
		public static extern IntPtr CreateMutex(ref SECURITY_ATTRIBUTES tSec,bool bInitialOwner,string lpName);

		[DllImport("Kernel32.dll", SetLastError=true)]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess,IntPtr lpThreadAttributes,uint dwStackSize,UIntPtr lpStartAddress,IntPtr lpParameter,uint dwCreationFlags,out int ThreadId);

		[StructLayout(LayoutKind.Sequential)]
			public struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

//		public ObjectMonitorClass ObjectMonitor
//		{
//			get
//			{
//				return objMonitor;
//			}
//		}

		public SharpClientForm()
		{
			//
			// Required for Windows Form Designer support
			//

			try
			{
				if(!Convert.ToBoolean(IsUserAnAdmin()))
				{
					if(DialogResult.No==MessageBox.Show("ProfileSharp has detected that you are running the profiler in a non-administrative account.\nThis can make the profiler unreponsive.Do you still want to continue?"  ,"Non-Administrator",MessageBoxButtons.YesNo,MessageBoxIcon.Warning))
					{
						System.Diagnostics.Process.GetCurrentProcess().Kill();    
					}
				}
			}
			catch
			{}

			try
			{
				if(IsProfilerAlreadyRunning())
				{

//					if(objMonitor!=null)
//					{
//						try
//						{
//							Marshal.ReleaseComObject(objMonitor);
//							objMonitor=null;
//						}
//						catch{}
//					}
					MessageBox.Show("ProfileSharp could not be initialized on your system.\nPlease contact SoftProdigy for support on this issue." ,"ProfileSharp Error!",MessageBoxButtons.OK,MessageBoxIcon.Stop);
					System.Diagnostics.Process.GetCurrentProcess().Kill();	
					return;
				}
			}
			catch(Exception except){
				MessageBox.Show("An Exception occured\n"+except.Message ,"ProfileSharp Error!.",MessageBoxButtons.OK,MessageBoxIcon.Stop);
				System.Diagnostics.Process.GetCurrentProcess().Kill();		
				return;
			}
			finally
			{
//				if(objMonitor!=null)
//				{
//					try
//					{
//						Marshal.ReleaseComObject(objMonitor);
//						objMonitor=null;
//					}
//					catch{}
//				}				
			}			

			
			Application.DoEvents(); 
			InitializeComponent();
			if(SharpClientForm.scInstance ==null)
			{
				SharpClientForm.scInstance=this;
			}
			sharpClientMDITab.Appearance=SharpClient.UI.Controls.TabControl.VisualAppearance.MultiDocument;

			subscribersTable=new Hashtable();   
			subscribersMenuTable =new Hashtable ();

			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			codeTabControl=new SharpClient.UI.Controls.TabControl(); 
			this.Controls.Add(codeTabControl);   

			_manager=new DockingManager(this,SharpClient.UI.Common.VisualStyle.IDE);			
			this.codeTabControl.Appearance = SharpClient.UI.Controls.TabControl.VisualAppearance.MultiDocument;
			this.codeTabControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.codeTabControl.ButtonActiveColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.codeTabControl.ButtonInactiveColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.codeTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeTabControl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.codeTabControl.HideTabsMode = SharpClient.UI.Controls.TabControl.HideTabsModes.ShowAlways;
			this.codeTabControl.HotTextColor = System.Drawing.SystemColors.Desktop;
			this.codeTabControl.IDEPixelBorder = false;
			this.codeTabControl.Location = new System.Drawing.Point(0, 31);
			this.codeTabControl.Name = "codeTabControl";
			this.codeTabControl.Size = new System.Drawing.Size(790, 484);
			this.codeTabControl.TabIndex = 4;
			this.codeTabControl.ClosePressed+=new EventHandler(codeTabControl_ClosePressed); 			
			
			_manager.InnerControl=sharpClientMDITab; 
			_manager.OuterControl= psStatusBar;				
			
			Content c=_manager.Contents.Add(codeTabControl,"Source Code",imageList1,4);
			_manager.AddContentWithState(c,SharpClient.UI.Docking.State.DockBottom);     
			_manager.HideAllContents();	

			///////////////Stack Control///////
			///

			_stackManager=new DockingManager(this,SharpClient.UI.Common.VisualStyle.IDE);			
			_stackManager.InnerControl=sharpClientMDITab;
			_stackManager.OuterControl= psStatusBar;	
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private SharpClient.UI.Controls.TabControl AddPanelContents(string ContentTitle,int imageListIndex,SharpClient.UI.Docking.State _dockstate,bool bShow)
		{
			SharpClient.UI.Controls.TabControl stackTabControl=null;
			Content cStack=null;
			if(_stackManager==null)
			{
				_stackManager=new DockingManager(this,SharpClient.UI.Common.VisualStyle.IDE);
				_stackManager.InnerControl=sharpClientMDITab;
				_stackManager.OuterControl= psStatusBar;	
			}

			if(_stackManager.Contents[ContentTitle]==null)
			{
				stackTabControl=new SharpClient.UI.Controls.TabControl();				
				stackTabControl.Appearance = SharpClient.UI.Controls.TabControl.VisualAppearance.MultiDocument;
				stackTabControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
				stackTabControl.ButtonActiveColor = System.Drawing.SystemColors.ActiveCaptionText;
				stackTabControl.ButtonInactiveColor = System.Drawing.SystemColors.InactiveCaptionText;
				stackTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
				stackTabControl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
				stackTabControl.HideTabsMode = SharpClient.UI.Controls.TabControl.HideTabsModes.ShowAlways;
				stackTabControl.HotTextColor = System.Drawing.SystemColors.Desktop;
				stackTabControl.IDEPixelBorder = false;
				stackTabControl.Location = new System.Drawing.Point(0, 31);				
				stackTabControl.Size = new System.Drawing.Size(790, 484);			
				stackTabControl.ClosePressed+=new EventHandler(stackTabControl_ClosePressed); 
				cStack=_stackManager.Contents.Add(stackTabControl,ContentTitle,imageList1,imageListIndex );								
				_stackManager.AddContentWithState(cStack,_dockstate);
								
			}
			else
			{
				cStack=_stackManager.Contents[ContentTitle];
				stackTabControl=cStack.Control as SharpClient.UI.Controls.TabControl;
			}

			ShowStackControl(bShow,cStack);	
			return stackTabControl;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SharpClientForm));
			this.psStatusBar = new System.Windows.Forms.StatusBar();
			this.panelSharpClientStatus = new System.Windows.Forms.StatusBarPanel();
			this.psToolBar = new System.Windows.Forms.ToolBar();
			this.newSession = new System.Windows.Forms.ToolBarButton();
			this.newContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem17 = new System.Windows.Forms.MenuItem();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.openSession = new System.Windows.Forms.ToolBarButton();
			this.openContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.openProfilerOptions = new System.Windows.Forms.ToolBarButton();
			this.toggleSettings = new System.Windows.Forms.ToolBarButton();
			this.createGraph = new System.Windows.Forms.ToolBarButton();
			this.webPrintPreview = new System.Windows.Forms.ToolBarButton();
			this.codePreview = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.psMainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuNew = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.menuItem16 = new System.Windows.Forms.MenuItem();
			this.menuOpen = new System.Windows.Forms.MenuItem();
			this.menuOpen_MemAnalysis = new System.Windows.Forms.MenuItem();
			this.menuOpen_PerfAnalysis = new System.Windows.Forms.MenuItem();
			this.menuOpen_CompareAnalysis = new System.Windows.Forms.MenuItem();
			this.menuExport = new System.Windows.Forms.MenuItem();
			this.menuExport_XML = new System.Windows.Forms.MenuItem();
			this.menuExport_CSV = new System.Windows.Forms.MenuItem();
			this.menuClose = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.menuView = new System.Windows.Forms.MenuItem();
			this.menuProfilerOptions = new System.Windows.Forms.MenuItem();
			this.menuGraph = new System.Windows.Forms.MenuItem();
			this.menuPrintPreview = new System.Windows.Forms.MenuItem();
			this.menuWebPreview = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuCustomization = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuAbout = new System.Windows.Forms.MenuItem();
			this.openProfilerSession = new System.Windows.Forms.OpenFileDialog();
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.sharpClientMDITab = new SharpClient.UI.Controls.TabControl();
			((System.ComponentModel.ISupportInitialize)(this.panelSharpClientStatus)).BeginInit();
			this.SuspendLayout();
			// 
			// psStatusBar
			// 
			this.psStatusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psStatusBar.Location = new System.Drawing.Point(0, 511);
			this.psStatusBar.Name = "psStatusBar";
			this.psStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						   this.panelSharpClientStatus});
			this.psStatusBar.ShowPanels = true;
			this.psStatusBar.Size = new System.Drawing.Size(790, 20);
			this.psStatusBar.TabIndex = 1;
			// 
			// panelSharpClientStatus
			// 
			this.panelSharpClientStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.panelSharpClientStatus.Width = 10;
			// 
			// psToolBar
			// 
			this.psToolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.psToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						 this.newSession,
																						 this.openSession,
																						 this.openProfilerOptions,
																						 this.toggleSettings,
																						 this.createGraph,
																						 this.webPrintPreview,
																						 this.codePreview});
			this.psToolBar.ButtonSize = new System.Drawing.Size(24, 24);
			this.psToolBar.DropDownArrows = true;
			this.psToolBar.ImageList = this.imageList1;
			this.psToolBar.Location = new System.Drawing.Point(0, 0);
			this.psToolBar.Name = "psToolBar";
			this.psToolBar.ShowToolTips = true;
			this.psToolBar.Size = new System.Drawing.Size(790, 37);
			this.psToolBar.TabIndex = 2;
			this.psToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.psToolBar_ButtonClick);
			// 
			// newSession
			// 
			this.newSession.DropDownMenu = this.newContextMenu;
			this.newSession.ImageIndex = 0;
			this.newSession.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			this.newSession.Text = "NS";
			this.newSession.ToolTipText = "Start New Profiling Session.";
			// 
			// newContextMenu
			// 
			this.newContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItem7,
																						   this.menuItem8,
																						   this.menuItem17,
																						   this.menuItem18,
																						   this.menuItem19});
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 0;
			this.menuItem7.Text = "Desktop Application..";
			this.menuItem7.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 1;
			this.menuItem8.Text = "Windows Service..";
			this.menuItem8.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem17
			// 
			this.menuItem17.Index = 2;
			this.menuItem17.Text = "COM+ Application..";
			this.menuItem17.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 3;
			this.menuItem18.Text = "ASP.NET..";
			this.menuItem18.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem19
			// 
			this.menuItem19.Index = 4;
			this.menuItem19.Text = "Any .NET Process";
			this.menuItem19.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// openSession
			// 
			this.openSession.DropDownMenu = this.openContextMenu;
			this.openSession.ImageIndex = 1;
			this.openSession.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			this.openSession.Text = "OS";
			this.openSession.ToolTipText = "Open Profiling Results.";
			// 
			// openContextMenu
			// 
			this.openContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItem5,
																							this.menuItem6,
																							this.menuItem11});
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 0;
			this.menuItem5.Text = "Memory Profiling Results..";
			this.menuItem5.Click += new System.EventHandler(this.menuOpen_MemAnalysis_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.Text = "Performance Profiling Results..";
			this.menuItem6.Click += new System.EventHandler(this.menuOpen_PerfAnalysis_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 2;
			this.menuItem11.Text = "Memory Comparison Results..";
			this.menuItem11.Click += new System.EventHandler(this.menuOpen_CompareAnalysis_Click);
			// 
			// openProfilerOptions
			// 
			this.openProfilerOptions.ImageIndex = 12;
			this.openProfilerOptions.Text = "OPO";
			this.openProfilerOptions.ToolTipText = "Show Profiler Options";
			// 
			// toggleSettings
			// 
			this.toggleSettings.Enabled = false;
			this.toggleSettings.ImageIndex = 2;
			this.toggleSettings.Text = "TS";
			this.toggleSettings.ToolTipText = "Hide/Show Settings";
			// 
			// createGraph
			// 
			this.createGraph.Enabled = false;
			this.createGraph.ImageIndex = 3;
			this.createGraph.Text = "CG";
			this.createGraph.ToolTipText = "Create Graph";
			// 
			// webPrintPreview
			// 
			this.webPrintPreview.Enabled = false;
			this.webPrintPreview.ImageIndex = 5;
			this.webPrintPreview.Text = "WP";
			this.webPrintPreview.ToolTipText = "Web/Print Preview";
			// 
			// codePreview
			// 
			this.codePreview.Enabled = false;
			this.codePreview.ImageIndex = 4;
			this.codePreview.Text = "CP";
			this.codePreview.ToolTipText = "Hide/Show Source Code";
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(24, 24);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// psMainMenu
			// 
			this.psMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuFile,
																					   this.menuView,
																					   this.menuItem1});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuNew,
																					 this.menuOpen,
																					 this.menuExport,
																					 this.menuClose,
																					 this.menuItem4,
																					 this.menuExit});
			this.menuFile.Text = "File";
			// 
			// menuNew
			// 
			this.menuNew.Index = 0;
			this.menuNew.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem12,
																					this.menuItem13,
																					this.menuItem14,
																					this.menuItem15,
																					this.menuItem16});
			this.menuNew.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuNew.Text = "Profile                    ";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 0;
			this.menuItem12.Text = "Desktop Application..";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 1;
			this.menuItem13.Text = "Windows Service..";
			this.menuItem13.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 2;
			this.menuItem14.Text = "COM+ Application..";
			this.menuItem14.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 3;
			this.menuItem15.Text = "ASP.NET..";
			this.menuItem15.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem16
			// 
			this.menuItem16.Index = 4;
			this.menuItem16.Text = "Any .NET Process..";
			this.menuItem16.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuOpen
			// 
			this.menuOpen.Index = 1;
			this.menuOpen.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuOpen_MemAnalysis,
																					 this.menuOpen_PerfAnalysis,
																					 this.menuOpen_CompareAnalysis});
			this.menuOpen.Text = "Open";
			// 
			// menuOpen_MemAnalysis
			// 
			this.menuOpen_MemAnalysis.Index = 0;
			this.menuOpen_MemAnalysis.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftM;
			this.menuOpen_MemAnalysis.Text = "Memory Profiling Results..";
			this.menuOpen_MemAnalysis.Click += new System.EventHandler(this.menuOpen_MemAnalysis_Click);
			// 
			// menuOpen_PerfAnalysis
			// 
			this.menuOpen_PerfAnalysis.Index = 1;
			this.menuOpen_PerfAnalysis.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftP;
			this.menuOpen_PerfAnalysis.Text = "Performance Profiling Results..";
			this.menuOpen_PerfAnalysis.Click += new System.EventHandler(this.menuOpen_PerfAnalysis_Click);
			// 
			// menuOpen_CompareAnalysis
			// 
			this.menuOpen_CompareAnalysis.Index = 2;
			this.menuOpen_CompareAnalysis.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftC;
			this.menuOpen_CompareAnalysis.Text = "Memory Comparison Results..";
			this.menuOpen_CompareAnalysis.Click += new System.EventHandler(this.menuOpen_CompareAnalysis_Click);
			// 
			// menuExport
			// 
			this.menuExport.Index = 2;
			this.menuExport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuExport_XML,
																					   this.menuExport_CSV});
			this.menuExport.Text = "Export";
			// 
			// menuExport_XML
			// 
			this.menuExport_XML.Enabled = false;
			this.menuExport_XML.Index = 0;
			this.menuExport_XML.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftX;
			this.menuExport_XML.Text = "Export as XML..";
			this.menuExport_XML.Click += new System.EventHandler(this.menuItem_Click);
			// 
			// menuExport_CSV
			// 
			this.menuExport_CSV.Enabled = false;
			this.menuExport_CSV.Index = 1;
			this.menuExport_CSV.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftV;
			this.menuExport_CSV.Text = "Export as CSV..";
			this.menuExport_CSV.Click += new System.EventHandler(this.menuItem_Click);
			// 
			// menuClose
			// 
			this.menuClose.Index = 3;
			this.menuClose.Shortcut = System.Windows.Forms.Shortcut.CtrlF4;
			this.menuClose.Text = "Close";
			this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "-";
			// 
			// menuExit
			// 
			this.menuExit.Index = 5;
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// menuView
			// 
			this.menuView.Index = 1;
			this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuProfilerOptions,
																					 this.menuGraph,
																					 this.menuPrintPreview,
																					 this.menuWebPreview});
			this.menuView.Text = "View";
			// 
			// menuProfilerOptions
			// 
			this.menuProfilerOptions.Index = 0;
			this.menuProfilerOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuProfilerOptions.Text = "Profiler Options";
			this.menuProfilerOptions.Click += new System.EventHandler(this.menuProfilerOptions_Click);
			// 
			// menuGraph
			// 
			this.menuGraph.Enabled = false;
			this.menuGraph.Index = 1;
			this.menuGraph.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.menuGraph.Text = "Graph..";
			this.menuGraph.Click += new System.EventHandler(this.menuItem_Click);
			// 
			// menuPrintPreview
			// 
			this.menuPrintPreview.Enabled = false;
			this.menuPrintPreview.Index = 2;
			this.menuPrintPreview.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.menuPrintPreview.Text = "Print Preview";
			this.menuPrintPreview.Click += new System.EventHandler(this.menuItem_Click);
			// 
			// menuWebPreview
			// 
			this.menuWebPreview.Enabled = false;
			this.menuWebPreview.Index = 3;
			this.menuWebPreview.Text = "Web Preview";
			this.menuWebPreview.Click += new System.EventHandler(this.menuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem9,
																					  this.menuItem2,
																					  this.menuItem10,
																					  this.menuCustomization,
																					  this.menuItem3,
																					  this.menuAbout});
			this.menuItem1.Text = "Help";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 0;
			this.menuItem9.Text = "Contact..";
			this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuItem2.Text = "Product Documentation..";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 2;
			this.menuItem10.Text = "License..";
			this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
			// 
			// menuCustomization
			// 
			this.menuCustomization.Index = 3;
			this.menuCustomization.Text = "Order Product Customization..";
			this.menuCustomization.Click += new System.EventHandler(this.menuCustomization_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// menuAbout
			// 
			this.menuAbout.Index = 5;
			this.menuAbout.Text = "About Profile#";
			this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
			// 
			// openProfilerSession
			// 
			this.openProfilerSession.AddExtension = false;
			this.openProfilerSession.RestoreDirectory = true;
			// 
			// imageList2
			// 
			this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// sharpClientMDITab
			// 
			this.sharpClientMDITab.AllowDrop = true;
			this.sharpClientMDITab.Appearance = SharpClient.UI.Controls.TabControl.VisualAppearance.MultiDocument;
			this.sharpClientMDITab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.sharpClientMDITab.ButtonActiveColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.sharpClientMDITab.ButtonInactiveColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.sharpClientMDITab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sharpClientMDITab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.sharpClientMDITab.HideTabsMode = SharpClient.UI.Controls.TabControl.HideTabsModes.ShowAlways;
			this.sharpClientMDITab.HotTextColor = System.Drawing.SystemColors.Desktop;
			this.sharpClientMDITab.IDEPixelBorder = false;
			this.sharpClientMDITab.Location = new System.Drawing.Point(0, 37);
			this.sharpClientMDITab.Name = "sharpClientMDITab";
			this.sharpClientMDITab.Size = new System.Drawing.Size(790, 474);
			this.sharpClientMDITab.TabIndex = 6;
			this.sharpClientMDITab.TextInactiveColor = System.Drawing.Color.Gray;
			this.sharpClientMDITab.DragDrop += new System.Windows.Forms.DragEventHandler(this.SharpClientForm_DragDrop);
			this.sharpClientMDITab.DragEnter += new System.Windows.Forms.DragEventHandler(this.SharpClientForm_DragEnter);
			this.sharpClientMDITab.ClosePressed += new System.EventHandler(this.sharpClientMDITab_ClosePressed);
			// 
			// SharpClientForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(790, 531);
			this.Controls.Add(this.sharpClientMDITab);
			this.Controls.Add(this.psToolBar);
			this.Controls.Add(this.psStatusBar);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.Menu = this.psMainMenu;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "SharpClientForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " ProfileSharp Enterprise Edition";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SharpClientForm_Closing);
			this.Load += new System.EventHandler(this.SharpClientForm_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SharpClientForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SharpClientForm_DragEnter);
			((System.ComponentModel.ISupportInitialize)(this.panelSharpClientStatus)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			SharpClientForm.arguments=args; 
			System.Threading.Thread mainThread=new System.Threading.Thread(new System.Threading.ThreadStart(MainApp));
			mainThread.ApartmentState=System.Threading.ApartmentState.STA ;
			mainThread.Start(); 
			mainThread.Join();
		}

		[STAThread]
		static void MainApp()
		{
			Application.Run(new SharpClientForm());				
		}

		private bool IsProfilerAlreadyRunning()
		{		
#warning "Unusable Code. Commented Out"/////////////UNUSABLE CODE///////////

//			int hr=0;
//			try
//			{
//				System.Diagnostics.Process[] processes= System.Diagnostics.Process.GetProcessesByName("ProfileSharpUpdater.dat");
//				foreach(System.Diagnostics.Process process in processes)
//				{
//					try
//					{
//						process.Kill(); 
//						System.Threading.Thread.Sleep(500);
//					}
//					catch(Exception ex) 
//					{
//						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//						return true;
//					}
//				}				
//			}			
//			catch(Exception ex) 
//			{
//				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//				return true;
//			}
//			
//			try
//			{
//				System.Diagnostics.Process[] processes= System.Diagnostics.Process.GetProcessesByName("ProfileSharpUpdater");
//				foreach(System.Diagnostics.Process process in processes)
//				{
//					try
//					{
//						process.Kill(); 
//						System.Threading.Thread.Sleep(500);
//					}
//					catch(Exception ex) 
//					{
//						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//						return true;
//					}
//				}				
//			}			
//			catch(Exception ex) 
//			{
//				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//				return true;
//			}
//			
//			try
//			{
//				string curProcName=System.Diagnostics.Process.GetCurrentProcess().ProcessName;
//				System.Diagnostics.Process[] processes= System.Diagnostics.Process.GetProcessesByName(curProcName);
//				foreach(System.Diagnostics.Process process in processes)
//				{
//					try
//					{
//						if(process.Id!=System.Diagnostics.Process.GetCurrentProcess().Id)
//						{
//							process.Kill(); 
//							System.Threading.Thread.Sleep(500);
//						}
//					}
//					catch(Exception ex) 
//					{
//						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//						return true;
//					}
//				}				
//			}			
//			catch(Exception ex) 
//			{
//				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error) ; 
//				return true;
//			}
//			
//			try
//			{
//				System.Threading.Thread.Sleep(500);
//			}
//			catch{}
//			
//			try
//			{
//				Configurator.SECURITY_ATTRIBUTES  tSec = new Configurator.SECURITY_ATTRIBUTES();			
//				tSec.nLength = Marshal.SizeOf(tSec);
//				handle=Configurator.CreateMutex(ref tSec,false,"ABATHLA");
//				hr=System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error();	
//				System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);				
//			}
//			catch(COMException comExc)
//			{
//				if(comExc.ErrorCode==-2147024713)	//already created			
//				{
//					return true;
//				}
//			}
//			catch{}
//						
//			#warning "Important Code"
//						Microsoft.Win32.RegistryKey key=null;
//			try
//			{
//				key=Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"CLSID\{3AD05148-1BFC-4A9E-BACE-0F216D5EF32C}\LocalServer32");
//				string fileName=Convert.ToString (key.GetValue(null)).Replace("\"","") ;
//				string hashVal=SharpClientForm.hashCheck(fileName);
//				if(hashVal!="wj3yj3zkUfLoTTuJ9+E7AXO7Jhw=")
//				{
//					throw new IndexOutOfRangeException();
//				}	
//			}
//			catch(Exception exc)
//			{
//				exc.Source=null;
//				return true;  
//			}
//			finally
//			{
//				if(key!=null)
//				{
//					key.Close(); 
//					key=null;
//				}
//			}
//			
//			try
//			{
//				objMonitor=Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("{3AD05148-1BFC-4A9E-BACE-0F216D5EF32C}"),null,true)) as ProfileSharpUpdater.ObjectMonitorClass  ;
//			}
//			catch
//			{
//				return true;
//			}
//			if(objMonitor==null)
//			{
//				return true;  
//			}
//			else
//			{		
//				try
//				{
//					Marshal.ReleaseComObject(objMonitor);
//				}
//				catch{}
//				objMonitor=null;
//				Microsoft.Win32.RegistryKey key2=null;	//check again after successful creation
//				try
//				{
//					key2=Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"CLSID\{3AD05148-1BFC-4A9E-BACE-0F216D5EF32C}\LocalServer32");
//					string fileName=Convert.ToString (key2.GetValue(null)).Replace("\"","") ;
//					string hashVal=SharpClientForm.hashCheck(fileName);
//					if(hashVal!="wj3yj3zkUfLoTTuJ9+E7AXO7Jhw=")
//					{
//						throw new IndexOutOfRangeException();
//					}	
//				}
//				catch
//				{
//					return true;
//				}
//				finally
//				{
//					if(key2!=null)
//					{
//						key2.Close(); 
//						key2=null;
//					}
//					
//				}
//			}					

			return false;
			
		}

		private static string hashCheck(string fileName)
		{				
			StreamReader fStream=null;
			string fileData=null;
			string hashString=null;
			try
			{
				System.IO.FileInfo fileCore=new System.IO.FileInfo(fileName);
				fStream= new StreamReader(fileName);
				fileData=fStream.ReadToEnd();

				ASCIIEncoding encCur = new ASCIIEncoding();
				byte[] inputData = encCur.GetBytes(fileData);
				SHA1CryptoServiceProvider SHA1Obj = new SHA1CryptoServiceProvider(); 
				byte[]  hashedPassword = SHA1Obj.ComputeHash(inputData);						
				hashString=Convert.ToBase64String(hashedPassword);
			}
			catch(Exception ex) 
			{
				ex.Source=null; 
				throw new Exception("The profiler component failed to initialize."); 
			}
			finally
			{
				if(fStream!=null)
					fStream.Close(); 
			}

			return hashString;

		}

		public void AddCodeControlTab(string sessionId,string srcFile,ref DataTable srcTable,UInt64 threshHoldHitCount,UInt64 threshHoldPercentTimeConsumed)
		{
			try
			{
				ShowCodeControl(true);	
				string id=sessionId+":"+srcFile;

				if(_manager.Contents.Count==0)//Not possible
				{
					Content c=_manager.Contents.Add(codeTabControl,"Source Code",imageList1,4 );
					_manager.AddContentWithState(c,SharpClient.UI.Docking.State.DockBottom);     
				}

				foreach(SharpClient.UI.Controls.TabPage  t in codeTabControl.TabPages)
				{
					if(Convert.ToString(t.Tag).ToLower()==id.ToLower() )
					{
						codeTabControl.SelectedTab=t;
						t.Show();
						return;
					}
				}

				System.IO.FileInfo srcFileFullName=new System.IO.FileInfo(srcFile);
				SharpClient.UI.Controls.TabPage tPage=new SharpClient.UI.Controls.TabPage(sessionId+": "+srcFileFullName.Name); 
				tPage.Tag=id;
				tPage.Dock=DockStyle.Fill;
				codeTabControl.TabPages.Add(tPage);
				PGRptControl.CodeControl codeControl=new PGRptControl.CodeControl();
				codeControl.SourceFile=srcFileFullName.FullName;//always first
				codeControl.SourceTable=srcTable;
				codeControl.Dock=DockStyle.Fill;
				tPage.Controls.Add(codeControl); 
				codeControl.PreparePageEx(); 
				codeTabControl.SelectedTab=tPage;
				tPage.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
			}
		}

		public void AddCodeControlTab(string sessionId,string srcFile,string threadID,ref DataTable srcTable,UInt64 threshHoldHitCount,UInt64 threshHoldPercentTimeConsumed)
		{
			try
			{
				ShowCodeControl(true);	
				string id=sessionId+":"+srcFile+":"+threadID;

				if(_manager.Contents.Count==0)//Not possible
				{
					Content c=_manager.Contents.Add(codeTabControl,"Source Code",imageList1,4 );
					_manager.AddContentWithState(c,SharpClient.UI.Docking.State.DockBottom);     
				}

				foreach(SharpClient.UI.Controls.TabPage  t in codeTabControl.TabPages)
				{
					if(Convert.ToString(t.Tag).ToLower()==id.ToLower() )
					{
						codeTabControl.SelectedTab=t;
						t.Show();
						return;
					}
				}

				System.IO.FileInfo srcFileFullName=new System.IO.FileInfo(srcFile);
				SharpClient.UI.Controls.TabPage tPage=new SharpClient.UI.Controls.TabPage(sessionId+": "+srcFileFullName.Name+" ("+threadID+")"); 
				tPage.Tag=id;
				tPage.Dock=DockStyle.Fill;
				codeTabControl.TabPages.Add(tPage);
				PGRptControl.CodeControl codeControl=new PGRptControl.CodeControl();
				codeControl.SourceFile=srcFileFullName.FullName;//always first
				codeControl.SourceTable=srcTable;
				codeControl.Dock=DockStyle.Fill;
				tPage.Controls.Add(codeControl); 
				codeControl.PreparePageEx(); 
				codeTabControl.SelectedTab=tPage;
				tPage.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
			}
		}



//		public void AddStackControlTab(string ContentTitle,string uniqueTitle,int imageindex,SharpClient.UI.Docking.State _docState,string stackHeader,string stackData,string details,int ColumnHeaderValue,bool sortable)
//		{				
//			try
//			{
//				SharpClient.UI.Controls.TabControl stackTabControl=AddPanelContents(ContentTitle,imageindex,_docState,true);   
//
//				foreach(SharpClient.UI.Controls.TabPage  t in stackTabControl.TabPages)
//				{
//					if(Convert.ToString(t.Tag).ToLower()==uniqueTitle.ToLower() )
//					{						
//						foreach(Control control in t.Controls)
//						{
//							if(control.GetType()==typeof(PGRptControl.StackControl))
//							{
//								((PGRptControl.StackControl)control).RefreshData(stackHeader,stackData,details,ColumnHeaderValue);
//								break;
//							}
//						}
//						stackTabControl.SelectedTab=t;
//						t.Show();
//						return;
//					}
//				}
//			
//				SharpClient.UI.Controls.TabPage tPage=new SharpClient.UI.Controls.TabPage(uniqueTitle); 
//				tPage.Tag=uniqueTitle;
//				tPage.Dock=DockStyle.Fill;
//				stackTabControl.TabPages.Add(tPage);
//				PGRptControl.StackControl stackControl=new PGRptControl.StackControl(sortable); 				
//				stackControl.RefreshData(stackHeader,stackData,details,ColumnHeaderValue);					
//				stackControl.Dock=DockStyle.Fill;
//				tPage.Controls.Add(stackControl); 							
//				stackTabControl.SelectedTab=tPage;
//				tPage.Show();
//			}
//			catch(Exception ex)
//			{
//				MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
//			}
//		}

		public void AddStackControlTab(string ContentTitle,string uniqueTitle,int imageindex,SharpClient.UI.Docking.State _docState,string stackHeader,string stackData,string details,int ColumnHeaderValue,bool wizard,bool sortable)
		{				
			try
			{
				SharpClient.UI.Controls.TabControl stackTabControl=AddPanelContents(ContentTitle,imageindex,_docState,true);   
				
				foreach(SharpClient.UI.Controls.TabPage  t in stackTabControl.TabPages)
				{
					if(Convert.ToString(t.Tag).ToLower()==uniqueTitle.ToLower() )
					{						
						foreach(Control control in t.Controls)
						{
							if(control.GetType()==typeof(PGRptControl.StackControl))
							{
								((PGRptControl.StackControl)control).RefreshData(stackHeader,stackData,details,ColumnHeaderValue);
								break;
							}
						}
						stackTabControl.SelectedTab=t;
						t.Show();
						return;
					}
				}
			
				stackTabControl.ClosePressed-=new EventHandler(stackTabControl_ClosePressed); 
				//Remove the closing option
				stackTabControl.ShowArrows=false;
				stackTabControl.ShowClose=false; 
				SharpClient.UI.Controls.TabPage tPage=new SharpClient.UI.Controls.TabPage(uniqueTitle); 
				tPage.Tag=uniqueTitle;
				tPage.Dock=DockStyle.Fill;
				stackTabControl.TabPages.Add(tPage);
				PGRptControl.StackControl stackControl=new PGRptControl.StackControl(sortable); 
				stackControl.Wizard= wizard;
				stackControl.StackControl_ItemSelected+=new PGRptControl.StackControl_ItemSelectedHandler(stackControl_StackControl_ItemSelected); 
				stackControl.RefreshData(stackHeader,stackData,details,ColumnHeaderValue);					
				stackControl.Dock=DockStyle.Fill;
				tPage.Controls.Add(stackControl); 							
				stackTabControl.SelectedTab=tPage;
				tPage.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
			}
		}

		public void scrollToCodeLine(int lineNumber)
		{
			if(codeTabControl.SelectedTab!=null)
			{
				foreach(Control control in codeTabControl.SelectedTab.Controls)
				{
					if(control.GetType()== typeof(PGRptControl.CodeControl))
					{						
						((PGRptControl.CodeControl)control).ScrollToLine(lineNumber);
						return;
					}
				}
			}
		}

		public static bool CanShowQry()
		{
			Microsoft.Win32.RegistryKey keyQry;
			try
			{
				keyQry=Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
			}
			catch
			{
				return false;
			}
			bool bCanShow=false;
			try
			{
				bCanShow=Convert.ToInt32( keyQry.GetValue("ShowQry"))==0?false:true;
			}
			catch{}
			try
			{
				keyQry.Close();  
			}
			catch{}			
			return bCanShow;
		}

		public static bool CanCompact()
		{
			Microsoft.Win32.RegistryKey keyCompact=null;
			try
			{
				keyCompact=Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
			}
			catch
			{
				return false;
			}
			bool bCanCompact=false;
			try
			{
				bCanCompact=Convert.ToInt32( keyCompact.GetValue("AutoCompactDB"))==0?false:true;
			}
			catch{}
			try
			{
				if(keyCompact!=null)
				{
					keyCompact.Close();  
				}
			}
			catch{}			
			return bCanCompact;
		}

		private void ShowCodeControl(bool bShow)
		{
			if(_manager.Contents!=null && _manager.Contents.Count>0)
			{
				if(bShow)
				{					
					_manager.BringAutoHideIntoView(_manager.Contents[0]);  
					_manager.ShowAllContents();  
				}
				else
				{
					_manager.HideAllContents(); 
				}
			}
		}

		private void ShowStackControl(bool bShow,Content content)
		{
			if(_stackManager.Contents!=null && _stackManager.Contents.Count>0)
			{
				if(bShow)
				{				
					_stackManager.ShowContent(content); 
				}
				else
				{
					_stackManager.HideContent(content); 
				}
			}
		}

		
		public bool SubscribeToControl(string controlText,object delegateFunction)
		{
			if(subscribersTable[controlText]!=null) 
			{
				return false;
			}
			else
			{
				
				foreach (System.Windows.Forms.ToolBarButton tb in this.psToolBar.Buttons )
				{
					if(tb.Text==controlText)
					{
						tb.Enabled=true; 
						subscribersTable[controlText]=delegateFunction;  
						return true;
					}
				}
				return false;
			}
		}

		public void UnsubscribeControl(string controlText)
		{		
			try
			{
				foreach (System.Windows.Forms.ToolBarButton tb in this.psToolBar.Buttons )
				{
					if(tb.Text==controlText)
					{
						tb.Enabled=false; 
						subscribersTable.Remove(controlText);  						
					}
				}
			}
			catch{}
				
		}

		public bool SubscribeToMenu(MenuItem menuItem,object delegateFunction)
		{
			if(subscribersMenuTable[menuItem]!=null) 
			{
				return false;
			}
			else
			{
				
				menuItem.Enabled=true; 
				subscribersMenuTable[menuItem]=delegateFunction;  
				return true;
					
			}
			
		}
		

		public void UnsubscribeMenu(MenuItem menuItem)
		{		
			try
			{
				if(menuItem!=null)
				{
					menuItem.Enabled=false;  
					subscribersMenuTable.Remove(menuItem);  										
				}
			}
			catch{}				
		}

		private void CompactDB()
		{
			Application.DoEvents();  
			object jetObject=null;
			try
			{
				string srcString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;";
				string destString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\Temp.mdb;";
				jetObject=Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine",true));  
				jetObject.GetType().InvokeMember( "CompactDatabase",System.Reflection.BindingFlags.InvokeMethod|System.Reflection.BindingFlags.Public,null,jetObject,new object[]{srcString ,destString});                    					
			}
			catch{}
			finally
			{
				if(jetObject!=null)
				{
					Marshal.ReleaseComObject(jetObject);
					jetObject=null;
				}
				try
				{
					if(System.IO.File.Exists(Application.StartupPath+@"\Temp.mdb"))
					{
						System.IO.File.Copy(Application.StartupPath+@"\Temp.mdb",Application.StartupPath+@"\SharpBase.mdb",true);													
						System.IO.File.Delete(Application.StartupPath+@"\Temp.mdb");
					}
				}
				catch{}
			}
		}
		

		private void SharpClientForm_Load(object sender, System.EventArgs e)
		{	
			
			try
			{

				psStatusBar.Panels[0].Icon=this.Icon;  
				psStatusBar.Panels[0].Text="Welcome to ProfileSharp. The .NET Code, Performance and Memory Profiler by SoftProdigy.";
				Application.DoEvents();  
				splashForm=new SplashForm();
				splashForm.Show();  
				Application.DoEvents(); 		
				this.Cursor =Cursors.WaitCursor ;
				Application.DoEvents(); 
				
				try
				{
					Configurator.RestoreDefaultConfiguration();
				}
				catch{}			
											
				try
				{
					FunctionImporter.DeleteCache("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;",true); 
				}
				catch{}
				try
				{
					ObjectImporter.DeleteCache("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Application.StartupPath+@"\SharpBase.mdb;Mode=ReadWrite|Share Deny None;Persist Security Info=False;",true); 
				}
				catch{}				
				Application.DoEvents();  
				if(CanCompact())
				{
					CompactDB(); 
				}
				System.Threading.Thread.Sleep(3000);   
				
			}
			catch
			{
				if(splashForm!=null)
				{
					try
					{
						splashForm.Close();
						splashForm.Dispose();
						splashForm=null;
						Application.DoEvents();  
					}
					catch{}
				}				
			}
			finally
			{			
				this.Cursor=Cursors.Arrow ;  
				SharpClientForm.scInstance.AddStackControlTab("Profiler-Options","Profiler-Options",12,SharpClient.UI.Docking.State.DockLeft,"Choose an option","Profile desktop application.||Profile windows service.||Profile ASP.NET||Profile COM+ application.||Open performance results.||Open memory results.||Compare two memory snapshots.||Report a bug.||Suggest a feature.||Ask for a customization.||View help file.||ProfileSharp license agreement.||Check for updates.||About us.","\nPlease choose an action to perform\nwith ProfileSharp." ,-1,true,false);
				
				if(splashForm!=null)
				{
					try
					{
						splashForm.Close();
						splashForm.Dispose();
						splashForm=null;
						Application.DoEvents();  
					}
					catch{}
				}
 
			}
			/////////Auto Attach Start Thread
			Application.DoEvents();  
			profileeThread = new System.Threading.Thread(new System.Threading.ThreadStart(refreshProcessView));
			profileeThread.Start();

			//////////////////
		
			if (SharpClientForm.arguments!=null &&   SharpClientForm.arguments.Length>0)
			{
				try
				{
					foreach (string fileName in SharpClientForm.arguments)
					{
						Application.DoEvents();
						if(fileName.ToLower().EndsWith(".oxml"))
						{							
							SharpClientTabPage objectTab=new SharpClientTabPage("Loading...",fileName ); 				
							objectTab.Dock =DockStyle.Fill ;
							this.sharpClientMDITab.TabPages.Add(objectTab); 
							this.sharpClientMDITab.SelectedTab =objectTab ;
							objectTab.Show();   
						}
						else if(fileName.ToLower().EndsWith(".fxml"))
						{
							SharpClientTabPage functionTab=new SharpClientTabPage("Loading...",fileName  ); 				
							functionTab.Dock =DockStyle.Fill ;
							this.sharpClientMDITab.TabPages.Add(functionTab); 
							this.sharpClientMDITab.SelectedTab =functionTab ;
							functionTab.Show(); 
						}
						Application.DoEvents();
						break;//we only want to open 1 file at a time.

					}
				}										
				catch(Exception except)
				{					
					MessageBox.Show("ProfileSharp was unable to open one or more files specified.\n"+except.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error)   ;
				}			

			}
			
		}


		private void refreshProcessView()
		{			
			while(true)
			{				
				try
				{ 
					foreach(System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
					{	
						string strPID=process.Id.ToString();
						try
						{
							IntPtr hMutex=System.IntPtr.Zero;							
							if( ( hMutex=OpenMutex(1,0,"Global\\"+strPID+"?AUTO_ATTACH"))!= System.IntPtr.Zero)
							{
								ReleaseMutex(hMutex);
								CloseHandle(hMutex);
								hMutex=IntPtr.Zero ;
								if(!SharpClientForm.profilerTable.ContainsKey(process.Id))
								{
									AttachProfiler( process.Id);//Replace With SendMessage (WM_USER+ 911)
									continue;
								}
							}							
							if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED
							{
								AttachProfiler( process.Id);//Replace With SendMessage (WM_USER+ 911)
								continue;
							}
						}								
						catch{}
					}				
				}
			catch(Exception ex)
					{
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error)   ;
					}
				System.Threading.Thread.Sleep(500);   
				Application.DoEvents();
			}
		}
		

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{			
			if(m.Msg==(0x0400+ 911))//0x400=WM_USER for once
			{
				int PID=((int)m.WParam);
				AttachProfiler(PID);
			}
			base.WndProc(ref m);
		}

		private void AttachProfiler(int PID)
		{
			//Check for already profiling
			if(profilerTable==null){return;	}
			if(profilerTable.ContainsKey(PID)){return;}

			string processName=null;
			try
			{
				processName=System.Diagnostics.Process.GetProcessById(PID).MainModule.ModuleName ;
			}
			catch
			{		
				try
				{
					processName=System.Diagnostics.Process.GetProcessById(PID).ProcessName;
					if(processName!=null)  
					{
						if(!processName.ToLower().EndsWith(".exe"))
						{
							processName+=".exe";
						}
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error)   ;
					return;	
				}				
			}
			
			string configString=null;
			if(Configurator.g_FunctionFlag!=0)
			{
				configString="PERFORMANCE_ANALYSIS";
			}
			else if(Configurator.g_ObjectFlag!=0)
			{
				configString="MEMORY_ANALYSIS";
			}
			if(Configurator.g_FunctionFlag==0 && Configurator.g_ObjectFlag==0)
			{
				//Do Nothing
			}
			else
			{
				try
				{
					ProfilerControl perfControl=new ProfilerControl(PID,processName,true,Configurator.g_FunctionFlag ,Configurator.g_ObjectFlag ,configString,null);					
					perfControl.Text =Convert.ToString(PID)+ ":"+ processName;	//V.Imp.
					perfControl.Show(); 
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error)   ;
					return;	
				}				
			}
			Application.DoEvents(); 
		}

		private void SharpClientForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{			
			if(MessageBox.Show("Exit Profiler?","Exit?",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)==DialogResult.Cancel)
			{
				e.Cancel=true;
				return;
			}				
			if(profileeThread!=null)
			{
				try
				{
					profileeThread.Resume(); 
				}
				catch{}
				try
				{
					profileeThread.Abort(); 					
				}
				catch{}
			}
			try
			{
//				if(objMonitor!=null)
//				{
//					Marshal.ReleaseComObject(objMonitor);
//					objMonitor=null;
//				}
				Configurator.UninstallProfilingEnvironment(false);
			}
			catch{}		
				
			finally
			{					
				if(handle!=IntPtr.Zero)
				{
					try
					{
						Configurator.ReleaseMutex(handle);  
						Configurator.CloseHandle(handle);  
					}
					catch{}
					handle=IntPtr.Zero;
				}

				try
				{
					System.Diagnostics.Process.GetCurrentProcess().Kill();   
				}
				catch{}
			}			
		
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.Close(); 
		}

		private void menuClose_Click(object sender, System.EventArgs e)
		{
			if(this.sharpClientMDITab.SelectedTab!=null )
			{
				this.sharpClientMDITab.TabPages.Remove(sharpClientMDITab.SelectedTab);
			}
		}

		private void menuOpen_MemAnalysis_Click(object sender, System.EventArgs e)
		{
			OpenMemorySession();
		
		}

		private void OpenMemorySession()
		{
			openProfilerSession.Title ="Open Memory Analysis Session File";
			openProfilerSession.Filter ="Memory Analysis files(*.oxml)|*.oxml";
			openProfilerSession.RestoreDirectory =true;
			if(openProfilerSession.ShowDialog() ==DialogResult.OK)
			{
				SharpClientTabPage objectTab=new SharpClientTabPage("Loading...",openProfilerSession.FileName ); 				
				objectTab.Dock =DockStyle.Fill ;
				this.sharpClientMDITab.TabPages.Add(objectTab); 
				this.sharpClientMDITab.SelectedTab =objectTab ;
				objectTab.Show();   
			}
		}

		private void menuOpen_PerfAnalysis_Click(object sender, System.EventArgs e)
		{
			OpenPerformanceSession();		
		}		

		private void OpenPerformanceSession()
		{
			openProfilerSession.Title ="Open Performance Analysis Session File";
			openProfilerSession.Filter ="Performance Analysis files(*.fxml)|*.fxml";
			openProfilerSession.RestoreDirectory =true;
			if(openProfilerSession.ShowDialog() ==DialogResult.OK)
			{				
				SharpClientTabPage functionTab=new SharpClientTabPage("Loading...",openProfilerSession.FileName ); 				
				functionTab.Dock =DockStyle.Fill ;
				this.sharpClientMDITab.TabPages.Add(functionTab); 
				this.sharpClientMDITab.SelectedTab =functionTab ;
				functionTab.Show(); 
			}	
		}
		

		private void sharpClientMDITab_ClosePressed(object sender, System.EventArgs e)
		{			
			if(this.sharpClientMDITab.SelectedTab!=null)  
			{
				this.sharpClientMDITab.TabPages.Remove(sharpClientMDITab.SelectedTab);			

			}
		}

		private void psToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(subscribersTable[e.Button.Text]!=null)
			{
				try
				{
					delegateToolbarEvent d=(delegateToolbarEvent)subscribersTable[e.Button.Text];
					d(e);
				}
				catch{}
			}
			if(e.Button.Text=="CP"  )
			{
				if(_manager.Contents[0].Visible==false) 
				{
					_manager.ShowAllContents(); 
				}
				else					
				{
					_manager.HideAllContents(); 
				}
			}		
			if(e.Button.Text=="OPO")
			{
				SharpClientForm.scInstance.AddStackControlTab("Profiler-Options","Profiler-Options",12,SharpClient.UI.Docking.State.DockLeft,"Choose an option","Profile desktop application.||Profile windows service.||Profile ASP.NET||Profile COM+ application.||Open performance results.||Open memory results.||Compare two memory snapshots.||Report a bug.||Suggest a feature.||Ask for a customization.||View help file.||ProfileSharp license agreement.||Check for updates.||About us.","\nPlease choose an action to perform\nwith ProfileSharp." ,-1,true,false);
				
			}
		}

		
		private void menuOpen_CompareAnalysis_Click(object sender, System.EventArgs e)
		{			
			ComparisonSessionSelector comparisonDialog=new ComparisonSessionSelector();			
			comparisonDialog.ShowDialog(this);  		
		}

		private void codeTabControl_ClosePressed(object sender, EventArgs e)
		{
			try
			{
				if(this.codeTabControl.SelectedTab!=null)  
				{
					this.codeTabControl.TabPages.Remove(this.codeTabControl.SelectedTab);			
				}
			}
			catch{}

		}

		private void stackTabControl_ClosePressed(object sender, EventArgs e)
		{
			try
			{
				SharpClient.UI.Controls.TabControl stackTabControl= sender as SharpClient.UI.Controls.TabControl;
				if(stackTabControl.SelectedTab!=null)  
				{
					stackTabControl.TabPages.Remove(stackTabControl.SelectedTab);			
				}
			}
			catch{}

		}

		private void menuItem_Click(object sender, System.EventArgs e)
		{
			if(subscribersMenuTable[sender]!=null)
			{
				try
				{
					delegateMenuEvent d=(delegateMenuEvent)subscribersMenuTable[sender];
					d(sender);
				}
				catch{}
			}
		
		}

		private void menuAbout_Click(object sender, System.EventArgs e)
		{
			About a=new About();
			a.ShowDialog(this);
		
		}

		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
				System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+"www.SoftProdigy.com"+"\"");				
				pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
				pInfo.UseShellExecute =false;
				System.Diagnostics.Process.Start(pInfo);			
			}
			catch(Exception ex)
			{
				MessageBox.Show("Unable to connect to the website\n"+ex.Message+"\n You may need to visit the indicated link manually.","Error!");				
			}
		
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			OpenProductDocumentation();
		}
		private void OpenProductDocumentation()
		{
			try
			{
				System.Diagnostics.Process.Start("AcroRd32.exe",Application.StartupPath+@"\Help\Documentation.pdf");  
			}
			catch
			{
				try
				{
					Microsoft.Win32.RegistryKey regAcroReader=  Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Classes\\Software\\Adobe\\Acrobat\\Exe",false);
					string acroExe=Convert.ToString(regAcroReader.GetValue(null)) ;					
					acroExe=acroExe.Trim(new char[]{'\"'}); 
					System.IO.FileInfo acroFile=new System.IO.FileInfo(acroExe);   
					System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(acroExe,Application.StartupPath+@"\Help\Documentation.pdf");
					pInfo.WorkingDirectory=acroFile.DirectoryName; 
					pInfo.UseShellExecute =false;
					System.Diagnostics.Process.Start(pInfo);			
				}
				catch
				{
					MessageBox.Show("Unable to show documentation.Please make sure 'Adobe Acrobat Reader' is installed on your system.\nIf it is,please make sure its path lies in the 'PATH' system-variable of the system.","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error ) ;
				}
			}
		}

		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			
			try
			{
				License license=new License();
				license.ShowDialog(this); 
			}
			catch(Exception ex)
			{
				MessageBox.Show("Unable to show the 'License-Agreement'\n"+ex.Message+"\n You may need to visit SoftProdigy's official website to read the 'License-Agreement'. ","Error!");				
			}
		}

		private void SharpClientForm_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop) )
			{
				string[] files=e.Data.GetData(DataFormats.FileDrop) as string[] ;
				try
				{
					foreach (string fileName in files)
					{
						Application.DoEvents();
						if(fileName.ToLower().EndsWith(".oxml"))
						{							
							SharpClientTabPage objectTab=new SharpClientTabPage("Loading...",fileName ); 				
							objectTab.Dock =DockStyle.Fill ;
							this.sharpClientMDITab.TabPages.Add(objectTab); 
							this.sharpClientMDITab.SelectedTab =objectTab ;
							objectTab.Show();   
						}
						else if(fileName.ToLower().EndsWith(".fxml"))
						{
							SharpClientTabPage functionTab=new SharpClientTabPage("Loading...",fileName  ); 				
							functionTab.Dock =DockStyle.Fill ;
							this.sharpClientMDITab.TabPages.Add(functionTab); 
							this.sharpClientMDITab.SelectedTab =functionTab ;
							functionTab.Show(); 
						}
						else
						{
							throw new Exception("Unrecognized file format."); 
						}
						Application.DoEvents();
						break;//we only want to open 1 file at a time.

					}
				}
				catch(Exception except)
				{
					MessageBox.Show("ProfileSharp was unable to open one or more files specified.\n"+except.Message,"Error in opening file!",MessageBoxButtons.OK,MessageBoxIcon.Error)   ;
				}
			}
			
		}

		private void SharpClientForm_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop) )
			{
				e.Effect=DragDropEffects.All;  
			}
		}

		private void stackControl_StackControl_ItemSelected(string itemClicked)
		{
			switch (itemClicked.Trim().ToUpper() )
			{
				case "PROFILE DESKTOP APPLICATION.":				
				{					
					ProfilingSelector perfSelector=new ProfilingSelector(PROFILEE_TYPE.PROFILE_DESKTOP_APP);
					perfSelector.Show();
					break;
				}
				case "PROFILE WINDOWS SERVICE.":
				case "PROFILE ASP.NET":
				case "PROFILE COM+ APPLICATION.":
				{
					ProfilingSelector perfSelector=new ProfilingSelector(PROFILEE_TYPE.PROFILE_BACKGROUND_PROCESS);
					perfSelector.Show(); 
					break;
				}
				case "OPEN PERFORMANCE RESULTS.":
				{
					OpenPerformanceSession();
					break;
				}
				case "OPEN MEMORY RESULTS.":
				{
					OpenMemorySession();
					break;
				}
				case "COMPARE TWO MEMORY SNAPSHOTS.":
				{
					ComparisonSessionSelector comparisonDialog=new ComparisonSessionSelector();			
					comparisonDialog.ShowDialog(this);  	
					break;
				}				
				case "SUGGEST A FEATURE.":
				case "REPORT A BUG.":
				{
					try
					{
						string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
						System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+"www.SoftProdigy.com\\Contact.aspx"+"\"");				
						pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
						pInfo.UseShellExecute =false;
						System.Diagnostics.Process.Start(pInfo);			
					}
					catch(Exception ex)
					{
						MessageBox.Show("Unable to connect to the website\n"+ex.Message+"\n You may need to visit www.SoftProdigy.com manually.","Error!");				
					}
		
					break;
				}
				case "ASK FOR A CUSTOMIZATION.":
				{
					try
					{
						string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
						System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+"mailto:Products@SoftProdigy.com?Subject=I need ProfileSharp to be customized to my requirements.Please do it so for me."+"\"");				
						pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
						pInfo.UseShellExecute =false;
						System.Diagnostics.Process.Start(pInfo);			
					}
					catch(Exception ex)
					{
						MessageBox.Show("Unable to connect to SoftProdigy.\n"+ex.Message+"\n Please drop in a mail to Products@SoftProdigy.com with 'Product Customization Required' as subject.","Error!");				
					}
		
					break;
				}
				case "VIEW HELP FILE.":
				{
					OpenProductDocumentation();
					break;
				}
				case "PROFILESHARP LICENSE AGREEMENT.":
				{
					try
					{
						License license=new License();
						license.ShowDialog(this); 
					}
					catch(Exception ex)
					{
						MessageBox.Show("Unable to show the 'License-Agreement'\n"+ex.Message+"\n You may need to visit SoftProdigy's official website to acknowledge the 'License-Agreement'. ","Error!");				
					}
					break;
				}
				case "CHECK FOR UPDATES.":
				{
					try
					{
						string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
						System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+"www.SoftProdigy.com"+"\"");				
						pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
						pInfo.UseShellExecute =false;
						System.Diagnostics.Process.Start(pInfo);			
					}
					catch(Exception ex)
					{
						MessageBox.Show("Unable to connect to the website\n"+ex.Message+"\n You may need to visit www.SoftProdigy.com manually.","Error!");				
					}
		
					break;
				}
				case "ABOUT US.":
				{
					About a=new About();
					a.ShowDialog(this);
					break;
				}
			}
		}

		
		private void menuProfilerOptions_Click(object sender, System.EventArgs e)
		{
			SharpClientForm.scInstance.AddStackControlTab("Profiler-Options","Profiler-Options",12,SharpClient.UI.Docking.State.DockLeft,"Choose an option","Profile desktop application.||Profile windows service.||Profile ASP.NET||Profile COM+ application.||Open performance results.||Open memory results.||Compare two memory snapshots.||Report a bug.||Suggest a feature.||Ask for a customization.||View help file.||ProfileSharp license agreement.||Check for updates.||About us.","\nPlease choose an action to perform\nwith ProfileSharp." ,-1,true,false);
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			ProfilingSelector perfSelector=new ProfilingSelector(PROFILEE_TYPE.PROFILE_DESKTOP_APP);
			perfSelector.Show();
		}

		private void menuItem13_Click(object sender, System.EventArgs e)
		{
			ProfilingSelector perfSelector=new ProfilingSelector(PROFILEE_TYPE.PROFILE_BACKGROUND_PROCESS);
			perfSelector.Show(); 
		}

		private void menuCustomization_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
				System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+"mailto:Products@SoftProdigy.com?Subject=I need ProfileSharp to be customized to my requirements.Please do it so for me."+"\"");				
				pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
				pInfo.UseShellExecute =false;
				System.Diagnostics.Process.Start(pInfo);			
			}
			catch(Exception ex)
			{
				MessageBox.Show("Unable to connect to SoftProdigy.\n"+ex.Message+"\n Please drop in a mail to Products@SoftProdigy.com with 'Product Customization Required' as subject.","Error!");				
			}
		
		}

		
	}
}
