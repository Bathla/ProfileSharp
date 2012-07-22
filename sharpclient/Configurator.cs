using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PSUI;
using System.Runtime.InteropServices ; 
using Microsoft.Win32 ;
using ProfileSharpServerLib;
namespace SharpClient
{
	/// <summary>
	/// Summary description for Configurator.
	/// </summary>
	/// 	

	public class Configurator : System.Windows.Forms.UserControl 
	{
		private System.Windows.Forms.Button acceptSettings;
		private System.Windows.Forms.Button rejectSettings;
		private System.ComponentModel.IContainer components;		
		private System.Windows.Forms.TabPage MemTab;
		private System.Windows.Forms.TabPage PerfTab;
		private System.Windows.Forms.TabControl tabConfig;
		private PSUI.PSPanelGroup psMemPanelGroup;
		private System.Windows.Forms.GroupBox ObjectClassFilterGroupBox;
		private System.Windows.Forms.ComboBox objClassFilter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioMemObjPassthrough;
		private System.Windows.Forms.RadioButton radioMemObjectBlock;
		private System.Windows.Forms.TabPage profileeTab;
		private PSUI.PSPanelGroup psPerfPanelGroup;
		private PSUI.PSPanel psMemFilterPanel;
		private PSUI.PSPanel psPerfFilterPanel;
		private System.Windows.Forms.TabControl psPerfFilterTab;
		private System.Windows.Forms.TabPage psPerfClassFilterTab;
		private System.Windows.Forms.TabPage psPerfModuleFilterTab;
		private System.Windows.Forms.GroupBox FunctionClassFilterGroupBox;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioFuncObjBlock;
		private System.Windows.Forms.RadioButton radioFuncObjPassthrough;
		private System.Windows.Forms.ComboBox funcClassFilter;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ComboBox funcModuleFilter;
		private System.Windows.Forms.RadioButton radioPerfModuleBlock;
		private System.Windows.Forms.RadioButton radioPerfModulePassthrough;
		private PSUI.PSPanel psPerfRuntimePanel;
		private System.Windows.Forms.CheckBox funcRuntimeFuncSignature;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox funcRuntimeModuleName;
		private System.Windows.Forms.CheckBox funcRuntimeThreadID;
		private System.Windows.Forms.CheckBox funcRuntimeNumberOfCalls;
		private System.Windows.Forms.CheckBox funcRuntimeCallTimes;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ComboBox funcRuntimeTimeResolution;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.CheckBox funcRuntimeExceptions;
		private System.Windows.Forms.CheckBox funcRuntimeExceptionStackTrace;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.CheckBox funcRuntimeManagedOnly;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.TreeView calleeFunctionTree;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.RadioButton profileeAlreadyRunning;
		private System.Windows.Forms.RadioButton radioProfileeSuspended;
		private System.Windows.Forms.RadioButton profileeInteractiveSuspended;
		private System.Windows.Forms.RadioButton profileeInteractive;							
		private readonly string configString;
		private System.Windows.Forms.OpenFileDialog openProfileeDialog;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Button browseProfileeApp;
		private System.Windows.Forms.TextBox ProfileeCmdlineParam;
		private System.Windows.Forms.TextBox ProfileeAppFolder;
		private System.Windows.Forms.TextBox ProfileeApp;
		private System.Windows.Forms.GroupBox newProfileeGroupBox;			
		public FunctionData functionDataObj;
		public ObjectData objectDataObj;
		private SharpDelegatorClass delegatorObj;
		private System.Windows.Forms.CheckBox funcRuntimeCodeView;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.CheckBox funcRuntimeCalleeFunctions;
		private System.Windows.Forms.GroupBox groupBox12;
		private PSUI.PSPanel psMemDataCollection;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox objGCBeforeObjectCollection;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.CheckBox objRuntimeObjectAllocation;
		private System.Windows.Forms.CheckBox objRuntimeObjectNameOnly;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox objRuntimeReferencedObjectData;
		private System.Windows.Forms.CheckBox objRuntimeObjectSize;
		private System.Windows.Forms.CheckBox objRuntimeObjectCount;
		private System.Windows.Forms.ListView profileeView;
		private System.Windows.Forms.ColumnHeader processID;
		private System.Windows.Forms.ColumnHeader processName;
		private System.Windows.Forms.ColumnHeader Status;
		private System.Windows.Forms.ToolTip filterToolTip;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox objRuntimeSrcCodeAnalysis;
		private System.Windows.Forms.ColumnHeader profilingType;
		private System.Windows.Forms.CheckBox defaultAttach;
		public static int g_ObjectFlag=0;
		public static int g_FunctionFlag=0;
		private System.Windows.Forms.Label label3;
		System.Threading.Thread profileeThread;		

		[DllImport("Kernel32.dll")]
		public static extern IntPtr LoadLibraryA(string lpFileName);

		[DllImport("Kernel32.dll" ,SetLastError=true)]
		public static extern int GetExitCodeThread(
			IntPtr hThread,      
			out int lpExitCode    
			);

		[DllImport("Kernel32.dll")]
		public static extern UIntPtr GetProcAddress(IntPtr hModule,string procAddress);		

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

		[DllImport("Kernel32.dll")]
		public static extern int SetEnvironmentVariable(string envName,string envVal);	
        
		public enum PROCESS_STATUS
		{
			PROFILED=0,
			RUNNING=1,
			SUSPENDED=2
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			struct STARTUPINFO
		{
			Int32 cb;
			string lpReserved;
			string lpDesktop;
			string lpTitle;
			Int32 dwX;
			Int32 dwY;
			Int32 dwXSize;
			Int32 dwYSize;
			Int32 dwXCountChars;
			Int32 dwYCountChars;
			Int32 dwFillAttribute;
			Int32 dwFlags;
			Int16 wShowWindow;
			Int16 cbReserved2;
			IntPtr lpReserved2;
			IntPtr hStdInput;
			IntPtr hStdOutput;
			IntPtr hStdError;
		}

		[DllImport("kernel32.dll",SetLastError=true)]
		static extern bool CreateProcess(string lpApplicationName,
			string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
			uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation);



		private bool WANT_FUNCTION_NAME;
		private bool WANT_FUNCTION_SIGNATURE ;
		private bool WANT_NUMBER_OF_FUNCTION_CALLS ;
		private bool WANT_FUNCTION_TOTAL_CPU_TIME ;
		private bool WANT_FUNCTION_MODULE ;
		public bool WANT_FUNCTION_EXCEPTIONS  ;
		private bool WANT_FUNCTION_EXCEPTIONS_STACKTRACE  ;
		private bool WANT_FUNCTION_CALLEE_ID  ;
		
		public bool WANT_FUNCTION_CODE_VIEW;
		public bool WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS  ;
		public bool WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME  ;
		private bool WANT_FUNCTION_THREAD_ID  ;
		private bool WANT_FUNCTION_MANAGED_ONLY  ;
		public bool WANT_NO_FUNCTION_CALLEE_INFORMATION  ;

		private bool WANT_OBJECT_NAME_ONLY ;
		private bool WANT_OBJECT_COUNT ;
		private bool WANT_OBJECT_SIZE ;
		public bool WANT_REFERENCED_OBJECTS ;
		public bool WANT_OBJECT_ALLOCATION_DATA;
		private bool WANT_OBJECT_ALL_DATA ;	
		private bool WANT_SRC_ANALYSIS_ONLY;
		private readonly bool IsDesktopApp;
		private readonly bool m_bExceptionTracing;
		
		public Configurator(string config,FunctionData funcObj,ObjectData objObj,bool bExceptionTracing,bool bConfigRequired,bool bDeskTopApp)
		{
			//
			// Required for Windows Form Designer support
			//
			
			WANT_FUNCTION_NAME=false;
			WANT_FUNCTION_SIGNATURE =false;
			WANT_NUMBER_OF_FUNCTION_CALLS =false;
			WANT_FUNCTION_TOTAL_CPU_TIME =false;
			WANT_FUNCTION_MODULE =false;
			WANT_FUNCTION_EXCEPTIONS  =false;
			WANT_FUNCTION_EXCEPTIONS_STACKTRACE  =false;
			WANT_FUNCTION_CALLEE_ID  =false;

			WANT_FUNCTION_CODE_VIEW=false;			
			WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS  =false;
			WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME  =false;
			WANT_FUNCTION_THREAD_ID  =false;
			WANT_FUNCTION_MANAGED_ONLY  =false;
			WANT_NO_FUNCTION_CALLEE_INFORMATION  =false;


			WANT_OBJECT_NAME_ONLY =false;
			WANT_OBJECT_COUNT =false;
			WANT_OBJECT_SIZE =false;
			WANT_REFERENCED_OBJECTS =false;
			WANT_OBJECT_ALLOCATION_DATA=false;
			WANT_SRC_ANALYSIS_ONLY=false;
			WANT_OBJECT_ALL_DATA =false;	

			///////////////////////
			///
			configString=config;	
			IsDesktopApp	=bDeskTopApp;	
			m_bExceptionTracing=bExceptionTracing;

			InitializeComponent();			

			if(IsDesktopApp)
			{
				profileeInteractive.Checked=true;  
				profileeView.Visible=false;  
				newProfileeGroupBox.Visible=true; 
				groupBox10.Visible=false; 
			}
			else
			{
				profileeAlreadyRunning.Checked=true;
				profileeView.Visible=true;  
				groupBox10.Visible=true;
				newProfileeGroupBox.Visible=false; 
			}
			Application.DoEvents();		
			
			if(configString=="MEMORY_ANALYSIS")
			{
				try
				{
					//if(!IsDesktopApp)
					{
						InstallProfilingEnvironment();
					}
				}
				catch(Exception ex)
				{
					HandleInstallMessage(ex.Message); 
					return;
				}

				this.tabConfig.TabPages.RemoveAt(1);  
				objectDataObj =new ObjectData(); 
				try
				{
					this.funcRuntimeCodeView.Checked=false;					 
					foreach(TreeNode node in calleeFunctionTree.Nodes )
					{
						node.Checked=false;
						foreach	(TreeNode subnode in node.Nodes)
						{
							subnode.Checked=false; 
						}
					}
					funcRuntimeCalleeFunctions.Checked=false;
				}catch{}
			}
			else if(configString=="PERFORMANCE_ANALYSIS")
			{
				try
				{
					//if(!IsDesktopApp)
					{
						InstallProfilingEnvironment();
					}
				}
				catch(Exception ex)
				{
					HandleInstallMessage(ex.Message); 
					return;									
				}

				this.tabConfig.TabPages.RemoveAt(0); 
				functionDataObj =new FunctionData(); 				
				if(bExceptionTracing)
				{					
					this.funcRuntimeExceptions.Checked=true;
					this.funcRuntimeExceptionStackTrace.Checked=true;					
					try
					{
						this.funcRuntimeCodeView.Checked=false;
						foreach(TreeNode node in calleeFunctionTree.Nodes )
						{
							node.Checked=false;
							foreach	(TreeNode subnode in node.Nodes)
							{
								subnode.Checked=false; 
							}
						}
						funcRuntimeCalleeFunctions.Checked=false;
					}
					catch{}
					
				}
				else
				{
					this.funcRuntimeCodeView.Checked=true;					
					this.funcRuntimeExceptions.Checked=false;
					this.funcRuntimeExceptionStackTrace.Checked=false;					
					foreach(TreeNode node in calleeFunctionTree.Nodes )
					{
						node.Checked=true;
						foreach	(TreeNode subnode in node.Nodes)
						{
							subnode.Checked=true; 
						}
					}
					funcRuntimeCalleeFunctions.Checked=true; 
				}
				
				try
				{
					objRuntimeSrcCodeAnalysis.Checked=false;
					objRuntimeObjectAllocation.Checked=false;
					objRuntimeReferencedObjectData.Checked=false; 
				}
				catch{}
				
			}			
			else if(configString=="READONLY")
			{
				this.Height -= (this.rejectSettings.Height+1) ;
				this.Controls.Remove(this.rejectSettings);
				this.Controls.Remove(this.acceptSettings);				
				this.tabConfig.TabPages.RemoveAt(2);

				if(funcObj!=null)
				{
					functionDataObj =new FunctionData(funcObj);					
				}
				else
				{
					this.tabConfig.TabPages.RemoveAt(1);   
				}
				if(objObj!=null)
				{
					objectDataObj =new ObjectData(objObj); 
				}
				else
				{
					this.tabConfig.TabPages.RemoveAt(0);
				}
				foreach(Control control in this.Controls)
				{
					StepDisable(control);
				}
			}
			else
			{
				try
				{
					//if(!IsDesktopApp)
					{
						InstallProfilingEnvironment();
					}
				}
				catch(Exception ex)
				{
					HandleInstallMessage(ex.Message); 
					return;
				}
				objectDataObj =new ObjectData(); 
				functionDataObj =new FunctionData(); 
			}	

			if(bConfigRequired==false)
			{				
				this.tabConfig.SelectedTab = profileeTab;			
				acceptSettings.Text="Accept";  
				Application.DoEvents();
				selectNewTab();
			}
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private void HandleInstallMessage(string exMessage)			
		{
			MessageBox.Show("The profiling environment could not be started.\n"+exMessage,"Critical Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);   										 
			this.Controls.Clear();  
			this.BackColor =Color.White;
			this.Enabled=false;
			Application.DoEvents();			
			throw new Exception(exMessage); 
		}
		private void StepDisable(Control control)
		{			
			string controlName=control.GetType().Name.ToLower();
			if(controlName.StartsWith("tab" ) ||controlName.StartsWith("ps" )) 
			{
				//Do nothing
			}
			else
			{
				if(control.Parent.Enabled==true)  
				{
					control.Enabled =false;
				}
			}
			if(control.Controls.Count>0)
			{
				foreach(Control childControl in control.Controls)
				{
					StepDisable(childControl);
				}
			}
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Configurator));
			this.acceptSettings = new System.Windows.Forms.Button();
			this.rejectSettings = new System.Windows.Forms.Button();
			this.MemTab = new System.Windows.Forms.TabPage();
			this.psMemPanelGroup = new PSUI.PSPanelGroup();
			this.psMemDataCollection = new PSUI.PSPanel(280);
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.objRuntimeSrcCodeAnalysis = new System.Windows.Forms.CheckBox();
			this.objGCBeforeObjectCollection = new System.Windows.Forms.CheckBox();
			this.label18 = new System.Windows.Forms.Label();
			this.objRuntimeObjectAllocation = new System.Windows.Forms.CheckBox();
			this.objRuntimeObjectNameOnly = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.objRuntimeReferencedObjectData = new System.Windows.Forms.CheckBox();
			this.objRuntimeObjectSize = new System.Windows.Forms.CheckBox();
			this.objRuntimeObjectCount = new System.Windows.Forms.CheckBox();
			this.psMemFilterPanel = new PSUI.PSPanel(280);
			this.ObjectClassFilterGroupBox = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioMemObjectBlock = new System.Windows.Forms.RadioButton();
			this.radioMemObjPassthrough = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.objClassFilter = new System.Windows.Forms.ComboBox();
			this.PerfTab = new System.Windows.Forms.TabPage();
			this.psPerfPanelGroup = new PSUI.PSPanelGroup();
			this.psPerfRuntimePanel = new PSUI.PSPanel(280);
			this.groupBox12 = new System.Windows.Forms.GroupBox();
			this.label17 = new System.Windows.Forms.Label();
			this.funcRuntimeCodeView = new System.Windows.Forms.CheckBox();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.funcRuntimeCalleeFunctions = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.calleeFunctionTree = new System.Windows.Forms.TreeView();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.funcRuntimeManagedOnly = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.funcRuntimeExceptionStackTrace = new System.Windows.Forms.CheckBox();
			this.funcRuntimeExceptions = new System.Windows.Forms.CheckBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.funcRuntimeTimeResolution = new System.Windows.Forms.ComboBox();
			this.funcRuntimeNumberOfCalls = new System.Windows.Forms.CheckBox();
			this.funcRuntimeThreadID = new System.Windows.Forms.CheckBox();
			this.funcRuntimeModuleName = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.funcRuntimeFuncSignature = new System.Windows.Forms.CheckBox();
			this.funcRuntimeCallTimes = new System.Windows.Forms.CheckBox();
			this.psPerfFilterPanel = new PSUI.PSPanel(280);
			this.psPerfFilterTab = new System.Windows.Forms.TabControl();
			this.psPerfClassFilterTab = new System.Windows.Forms.TabPage();
			this.FunctionClassFilterGroupBox = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioFuncObjBlock = new System.Windows.Forms.RadioButton();
			this.radioFuncObjPassthrough = new System.Windows.Forms.RadioButton();
			this.funcClassFilter = new System.Windows.Forms.ComboBox();
			this.psPerfModuleFilterTab = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.radioPerfModuleBlock = new System.Windows.Forms.RadioButton();
			this.radioPerfModulePassthrough = new System.Windows.Forms.RadioButton();
			this.funcModuleFilter = new System.Windows.Forms.ComboBox();
			this.tabConfig = new System.Windows.Forms.TabControl();
			this.profileeTab = new System.Windows.Forms.TabPage();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.profileeAlreadyRunning = new System.Windows.Forms.RadioButton();
			this.radioProfileeSuspended = new System.Windows.Forms.RadioButton();
			this.profileeInteractiveSuspended = new System.Windows.Forms.RadioButton();
			this.profileeInteractive = new System.Windows.Forms.RadioButton();
			this.newProfileeGroupBox = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.browseProfileeApp = new System.Windows.Forms.Button();
			this.ProfileeCmdlineParam = new System.Windows.Forms.TextBox();
			this.ProfileeAppFolder = new System.Windows.Forms.TextBox();
			this.ProfileeApp = new System.Windows.Forms.TextBox();
			this.profileeView = new System.Windows.Forms.ListView();
			this.processID = new System.Windows.Forms.ColumnHeader();
			this.processName = new System.Windows.Forms.ColumnHeader();
			this.Status = new System.Windows.Forms.ColumnHeader();
			this.profilingType = new System.Windows.Forms.ColumnHeader();
			this.defaultAttach = new System.Windows.Forms.CheckBox();
			this.openProfileeDialog = new System.Windows.Forms.OpenFileDialog();
			this.filterToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MemTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.psMemPanelGroup)).BeginInit();
			this.psMemPanelGroup.SuspendLayout();
			this.psMemDataCollection.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.psMemFilterPanel.SuspendLayout();
			this.ObjectClassFilterGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.PerfTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.psPerfPanelGroup)).BeginInit();
			this.psPerfPanelGroup.SuspendLayout();
			this.psPerfRuntimePanel.SuspendLayout();
			this.groupBox12.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.psPerfFilterPanel.SuspendLayout();
			this.psPerfFilterTab.SuspendLayout();
			this.psPerfClassFilterTab.SuspendLayout();
			this.FunctionClassFilterGroupBox.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.psPerfModuleFilterTab.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabConfig.SuspendLayout();
			this.profileeTab.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.newProfileeGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// acceptSettings
			// 
			this.acceptSettings.BackColor = System.Drawing.SystemColors.Control;
			this.acceptSettings.Cursor = System.Windows.Forms.Cursors.Hand;
			this.acceptSettings.Location = new System.Drawing.Point(376, 384);
			this.acceptSettings.Name = "acceptSettings";
			this.acceptSettings.Size = new System.Drawing.Size(152, 40);
			this.acceptSettings.TabIndex = 1;
			this.acceptSettings.Text = "Next >>";
			this.acceptSettings.Click += new System.EventHandler(this.acceptSettings_Click);
			// 
			// rejectSettings
			// 
			this.rejectSettings.BackColor = System.Drawing.SystemColors.Control;
			this.rejectSettings.Cursor = System.Windows.Forms.Cursors.Hand;
			this.rejectSettings.Location = new System.Drawing.Point(528, 384);
			this.rejectSettings.Name = "rejectSettings";
			this.rejectSettings.Size = new System.Drawing.Size(160, 40);
			this.rejectSettings.TabIndex = 2;
			this.rejectSettings.Text = "Cancel";
			this.rejectSettings.Click += new System.EventHandler(this.rejectSettings_Click);
			// 
			// MemTab
			// 
			this.MemTab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.MemTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MemTab.Controls.Add(this.psMemPanelGroup);
			this.MemTab.Location = new System.Drawing.Point(4, 28);
			this.MemTab.Name = "MemTab";
			this.MemTab.Size = new System.Drawing.Size(688, 352);
			this.MemTab.TabIndex = 0;
			this.MemTab.Text = "Memory Analysis Settings";
			// 
			// psMemPanelGroup
			// 
			this.psMemPanelGroup.AutoScroll = true;
			this.psMemPanelGroup.BackColor = System.Drawing.Color.Transparent;
			this.psMemPanelGroup.Controls.Add(this.psMemDataCollection);
			this.psMemPanelGroup.Controls.Add(this.psMemFilterPanel);
			this.psMemPanelGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.psMemPanelGroup.Location = new System.Drawing.Point(0, 0);
			this.psMemPanelGroup.Name = "psMemPanelGroup";
			this.psMemPanelGroup.PanelGradient = ((PSUI.GradientColor)(resources.GetObject("psMemPanelGroup.PanelGradient")));
			this.psMemPanelGroup.Size = new System.Drawing.Size(684, 348);
			this.psMemPanelGroup.TabIndex = 0;
			// 
			// psMemDataCollection
			// 
			this.psMemDataCollection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psMemDataCollection.AnimationRate = 0;
			this.psMemDataCollection.BackColor = System.Drawing.Color.Transparent;
			this.psMemDataCollection.Caption = "Advance Options";
			this.psMemDataCollection.CaptionCornerType = PSUI.CornerType.Top;
			this.psMemDataCollection.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.psMemDataCollection.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.psMemDataCollection.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psMemDataCollection.CaptionUnderline = System.Drawing.Color.Gray;
			this.psMemDataCollection.Controls.Add(this.groupBox2);
			this.psMemDataCollection.CurveRadius = 12;
			this.psMemDataCollection.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psMemDataCollection.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psMemDataCollection.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psMemDataCollection.ImageItems.Disabled = -1;
			this.psMemDataCollection.ImageItems.Highlight = -1;
			this.psMemDataCollection.ImageItems.Normal = -1;
			this.psMemDataCollection.ImageItems.Pressed = -1;
			this.psMemDataCollection.ImageItems.PSImgSet = null;
			this.psMemDataCollection.Location = new System.Drawing.Point(8, 296);
			this.psMemDataCollection.Name = "psMemDataCollection";
			this.psMemDataCollection.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psMemDataCollection.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psMemDataCollection.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psMemDataCollection.PanelState = PSUI.PSPanelState.Collapsed;
			this.psMemDataCollection.Size = new System.Drawing.Size(668, 33);
			this.psMemDataCollection.TabIndex = 6;
			this.psMemDataCollection.TextColors.Background = System.Drawing.Color.White;
			this.psMemDataCollection.TextColors.Foreground = System.Drawing.Color.Black;
			this.psMemDataCollection.TextHighlightColors.Background = System.Drawing.Color.Black;
			this.psMemDataCollection.TextHighlightColors.Foreground = System.Drawing.Color.White;
			this.psMemDataCollection.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psMemDataCollection.Expanded += new System.EventHandler(this.psMemPanel_Expanded);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.objRuntimeSrcCodeAnalysis);
			this.groupBox2.Controls.Add(this.objGCBeforeObjectCollection);
			this.groupBox2.Controls.Add(this.label18);
			this.groupBox2.Controls.Add(this.objRuntimeObjectAllocation);
			this.groupBox2.Controls.Add(this.objRuntimeObjectNameOnly);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.objRuntimeReferencedObjectData);
			this.groupBox2.Controls.Add(this.objRuntimeObjectSize);
			this.groupBox2.Controls.Add(this.objRuntimeObjectCount);
			this.groupBox2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(16, 64);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(624, 184);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Memory Profiling Advance Options";
			// 
			// objRuntimeSrcCodeAnalysis
			// 
			this.objRuntimeSrcCodeAnalysis.Checked = true;
			this.objRuntimeSrcCodeAnalysis.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeSrcCodeAnalysis.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.objRuntimeSrcCodeAnalysis.Location = new System.Drawing.Point(48, 120);
			this.objRuntimeSrcCodeAnalysis.Name = "objRuntimeSrcCodeAnalysis";
			this.objRuntimeSrcCodeAnalysis.Size = new System.Drawing.Size(248, 24);
			this.objRuntimeSrcCodeAnalysis.TabIndex = 9;
			this.objRuntimeSrcCodeAnalysis.Text = "Only profile objects allocated by source code";
			this.objRuntimeSrcCodeAnalysis.CheckedChanged += new System.EventHandler(this.objRuntimeSrcCodeAnalysis_CheckedChanged);
			// 
			// objGCBeforeObjectCollection
			// 
			this.objGCBeforeObjectCollection.Checked = true;
			this.objGCBeforeObjectCollection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objGCBeforeObjectCollection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.objGCBeforeObjectCollection.Location = new System.Drawing.Point(544, 16);
			this.objGCBeforeObjectCollection.Name = "objGCBeforeObjectCollection";
			this.objGCBeforeObjectCollection.Size = new System.Drawing.Size(16, 24);
			this.objGCBeforeObjectCollection.TabIndex = 8;
			this.objGCBeforeObjectCollection.Text = "Force .NET Garbage Collector before every memory snapshot.";
			this.objGCBeforeObjectCollection.Visible = false;
			// 
			// label18
			// 
			this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label18.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label18.Location = new System.Drawing.Point(320, 88);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(296, 56);
			this.label18.TabIndex = 7;
			this.label18.Text = "( *Select to determine each object\'s allocation pattern and heap generation data." +
				" It is a CPU intensive operation and requires you to profile the process immedia" +
				"tely and not later when the process has started.)";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// objRuntimeObjectAllocation
			// 
			this.objRuntimeObjectAllocation.Checked = true;
			this.objRuntimeObjectAllocation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeObjectAllocation.Location = new System.Drawing.Point(16, 96);
			this.objRuntimeObjectAllocation.Name = "objRuntimeObjectAllocation";
			this.objRuntimeObjectAllocation.Size = new System.Drawing.Size(272, 24);
			this.objRuntimeObjectAllocation.TabIndex = 6;
			this.objRuntimeObjectAllocation.Text = "Collect object allocation and generation data";
			this.objRuntimeObjectAllocation.CheckedChanged += new System.EventHandler(this.objRuntimeObjectAllocation_CheckedChanged);
			// 
			// objRuntimeObjectNameOnly
			// 
			this.objRuntimeObjectNameOnly.Checked = true;
			this.objRuntimeObjectNameOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeObjectNameOnly.Enabled = false;
			this.objRuntimeObjectNameOnly.Location = new System.Drawing.Point(560, 16);
			this.objRuntimeObjectNameOnly.Name = "objRuntimeObjectNameOnly";
			this.objRuntimeObjectNameOnly.Size = new System.Drawing.Size(16, 24);
			this.objRuntimeObjectNameOnly.TabIndex = 5;
			this.objRuntimeObjectNameOnly.Text = "Collect Object Class Name.";
			this.objRuntimeObjectNameOnly.Visible = false;
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label13.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label13.Location = new System.Drawing.Point(328, 32);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(232, 40);
			this.label13.TabIndex = 4;
			this.label13.Text = "( *Select to determine inter-object references. It is a memory intensive operatio" +
				"n. )";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// objRuntimeReferencedObjectData
			// 
			this.objRuntimeReferencedObjectData.Checked = true;
			this.objRuntimeReferencedObjectData.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeReferencedObjectData.Location = new System.Drawing.Point(16, 32);
			this.objRuntimeReferencedObjectData.Name = "objRuntimeReferencedObjectData";
			this.objRuntimeReferencedObjectData.Size = new System.Drawing.Size(256, 24);
			this.objRuntimeReferencedObjectData.TabIndex = 3;
			this.objRuntimeReferencedObjectData.Text = "Collect referenced objects for each object.";
			// 
			// objRuntimeObjectSize
			// 
			this.objRuntimeObjectSize.Checked = true;
			this.objRuntimeObjectSize.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeObjectSize.Location = new System.Drawing.Point(592, 16);
			this.objRuntimeObjectSize.Name = "objRuntimeObjectSize";
			this.objRuntimeObjectSize.Size = new System.Drawing.Size(16, 24);
			this.objRuntimeObjectSize.TabIndex = 2;
			this.objRuntimeObjectSize.Text = "Collect Objects\' in-memory size (in bytes).";
			this.objRuntimeObjectSize.Visible = false;
			// 
			// objRuntimeObjectCount
			// 
			this.objRuntimeObjectCount.Checked = true;
			this.objRuntimeObjectCount.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objRuntimeObjectCount.Location = new System.Drawing.Point(576, 16);
			this.objRuntimeObjectCount.Name = "objRuntimeObjectCount";
			this.objRuntimeObjectCount.Size = new System.Drawing.Size(16, 24);
			this.objRuntimeObjectCount.TabIndex = 1;
			this.objRuntimeObjectCount.Text = "Collect Object Count.";
			this.objRuntimeObjectCount.Visible = false;
			// 
			// psMemFilterPanel
			// 
			this.psMemFilterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psMemFilterPanel.AnimationRate = 0;
			this.psMemFilterPanel.BackColor = System.Drawing.Color.Transparent;
			this.psMemFilterPanel.Caption = "Filters";
			this.psMemFilterPanel.CaptionCornerType = PSUI.CornerType.Top;
			this.psMemFilterPanel.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.psMemFilterPanel.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.psMemFilterPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psMemFilterPanel.CaptionUnderline = System.Drawing.Color.DarkGray;
			this.psMemFilterPanel.Controls.Add(this.ObjectClassFilterGroupBox);
			this.psMemFilterPanel.CurveRadius = 12;
			this.psMemFilterPanel.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psMemFilterPanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psMemFilterPanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psMemFilterPanel.ImageItems.Disabled = -1;
			this.psMemFilterPanel.ImageItems.Highlight = -1;
			this.psMemFilterPanel.ImageItems.Normal = -1;
			this.psMemFilterPanel.ImageItems.Pressed = -1;
			this.psMemFilterPanel.ImageItems.PSImgSet = null;
			this.psMemFilterPanel.Location = new System.Drawing.Point(8, 8);
			this.psMemFilterPanel.Name = "psMemFilterPanel";
			this.psMemFilterPanel.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psMemFilterPanel.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psMemFilterPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psMemFilterPanel.Size = new System.Drawing.Size(668, 280);
			this.psMemFilterPanel.TabIndex = 1;
			this.psMemFilterPanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psMemFilterPanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psMemFilterPanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psMemFilterPanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.filterToolTip.SetToolTip(this.psMemFilterPanel, "Filters let you select which classes or namespaces or modules will (or will not) " +
				"be profiled.");
			this.psMemFilterPanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psMemFilterPanel.Expanded += new System.EventHandler(this.psMemPanel_Expanded);
			// 
			// ObjectClassFilterGroupBox
			// 
			this.ObjectClassFilterGroupBox.Controls.Add(this.groupBox1);
			this.ObjectClassFilterGroupBox.Controls.Add(this.label1);
			this.ObjectClassFilterGroupBox.Controls.Add(this.objClassFilter);
			this.ObjectClassFilterGroupBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ObjectClassFilterGroupBox.Location = new System.Drawing.Point(24, 40);
			this.ObjectClassFilterGroupBox.Name = "ObjectClassFilterGroupBox";
			this.ObjectClassFilterGroupBox.Size = new System.Drawing.Size(632, 240);
			this.ObjectClassFilterGroupBox.TabIndex = 0;
			this.ObjectClassFilterGroupBox.TabStop = false;
			this.ObjectClassFilterGroupBox.Text = "Apply Class Filter";
			this.filterToolTip.SetToolTip(this.ObjectClassFilterGroupBox, "Filters let you select which classes or namespaces or modules will (or will not) " +
				"be profiled.");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioMemObjectBlock);
			this.groupBox1.Controls.Add(this.radioMemObjPassthrough);
			this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(336, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 88);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter Option";
			// 
			// radioMemObjectBlock
			// 
			this.radioMemObjectBlock.Checked = true;
			this.radioMemObjectBlock.Location = new System.Drawing.Point(8, 56);
			this.radioMemObjectBlock.Name = "radioMemObjectBlock";
			this.radioMemObjectBlock.Size = new System.Drawing.Size(272, 16);
			this.radioMemObjectBlock.TabIndex = 1;
			this.radioMemObjectBlock.TabStop = true;
			this.radioMemObjectBlock.Text = "Ignore selected Classes / Namespaces";
			// 
			// radioMemObjPassthrough
			// 
			this.radioMemObjPassthrough.Location = new System.Drawing.Point(8, 32);
			this.radioMemObjPassthrough.Name = "radioMemObjPassthrough";
			this.radioMemObjPassthrough.Size = new System.Drawing.Size(280, 16);
			this.radioMemObjPassthrough.TabIndex = 0;
			this.radioMemObjPassthrough.Text = "Profile only selected Classes / Namespaces.";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(568, 32);
			this.label1.TabIndex = 1;
			this.label1.Text = "Class Filter lets you select which classes or namespaces will (or will not) be pr" +
				"ofiled. Type in fully-qualified .NET classes / namespaces to analyze for memory " +
				"analysis (Case-Sensitive) and select a Filter Option.";
			// 
			// objClassFilter
			// 
			this.objClassFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.objClassFilter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.objClassFilter.Items.AddRange(new object[] {
																"System",
																"Microsoft",
																"ASP"});
			this.objClassFilter.Location = new System.Drawing.Point(16, 56);
			this.objClassFilter.Name = "objClassFilter";
			this.objClassFilter.Size = new System.Drawing.Size(312, 176);
			this.objClassFilter.TabIndex = 0;
			this.filterToolTip.SetToolTip(this.objClassFilter, "To add a class / namespace, type and press Enter.To remove, select and press Dele" +
				"te.");
			this.objClassFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objClassFilter_KeyDown);
			this.objClassFilter.DoubleClick += new System.EventHandler(this.objClassFilter_DoubleClick);
			// 
			// PerfTab
			// 
			this.PerfTab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.PerfTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PerfTab.Controls.Add(this.psPerfPanelGroup);
			this.PerfTab.Location = new System.Drawing.Point(4, 28);
			this.PerfTab.Name = "PerfTab";
			this.PerfTab.Size = new System.Drawing.Size(688, 352);
			this.PerfTab.TabIndex = 1;
			this.PerfTab.Text = "Performance Analysis Settings";
			// 
			// psPerfPanelGroup
			// 
			this.psPerfPanelGroup.AutoScroll = true;
			this.psPerfPanelGroup.BackColor = System.Drawing.Color.Transparent;
			this.psPerfPanelGroup.Controls.Add(this.psPerfRuntimePanel);
			this.psPerfPanelGroup.Controls.Add(this.psPerfFilterPanel);
			this.psPerfPanelGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.psPerfPanelGroup.Location = new System.Drawing.Point(0, 0);
			this.psPerfPanelGroup.Name = "psPerfPanelGroup";
			this.psPerfPanelGroup.PanelGradient = ((PSUI.GradientColor)(resources.GetObject("psPerfPanelGroup.PanelGradient")));
			this.psPerfPanelGroup.Size = new System.Drawing.Size(684, 348);
			this.psPerfPanelGroup.TabIndex = 0;
			// 
			// psPerfRuntimePanel
			// 
			this.psPerfRuntimePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psPerfRuntimePanel.AnimationRate = 0;
			this.psPerfRuntimePanel.BackColor = System.Drawing.Color.Transparent;
			this.psPerfRuntimePanel.Caption = "Advance Options";
			this.psPerfRuntimePanel.CaptionCornerType = PSUI.CornerType.Top;
			this.psPerfRuntimePanel.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.psPerfRuntimePanel.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.psPerfRuntimePanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psPerfRuntimePanel.CaptionUnderline = System.Drawing.Color.Gray;
			this.psPerfRuntimePanel.Controls.Add(this.groupBox12);
			this.psPerfRuntimePanel.Controls.Add(this.groupBox11);
			this.psPerfRuntimePanel.Controls.Add(this.groupBox9);
			this.psPerfRuntimePanel.Controls.Add(this.groupBox8);
			this.psPerfRuntimePanel.Controls.Add(this.groupBox7);
			this.psPerfRuntimePanel.Controls.Add(this.groupBox6);
			this.psPerfRuntimePanel.Controls.Add(this.funcRuntimeNumberOfCalls);
			this.psPerfRuntimePanel.Controls.Add(this.funcRuntimeThreadID);
			this.psPerfRuntimePanel.Controls.Add(this.funcRuntimeModuleName);
			this.psPerfRuntimePanel.Controls.Add(this.label4);
			this.psPerfRuntimePanel.Controls.Add(this.funcRuntimeFuncSignature);
			this.psPerfRuntimePanel.Controls.Add(this.funcRuntimeCallTimes);
			this.psPerfRuntimePanel.CurveRadius = 12;
			this.psPerfRuntimePanel.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psPerfRuntimePanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psPerfRuntimePanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psPerfRuntimePanel.ImageItems.Disabled = -1;
			this.psPerfRuntimePanel.ImageItems.Highlight = -1;
			this.psPerfRuntimePanel.ImageItems.Normal = -1;
			this.psPerfRuntimePanel.ImageItems.Pressed = -1;
			this.psPerfRuntimePanel.ImageItems.PSImgSet = null;
			this.psPerfRuntimePanel.Location = new System.Drawing.Point(8, 296);
			this.psPerfRuntimePanel.Name = "psPerfRuntimePanel";
			this.psPerfRuntimePanel.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfRuntimePanel.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfRuntimePanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psPerfRuntimePanel.PanelState = PSUI.PSPanelState.Collapsed;
			this.psPerfRuntimePanel.Size = new System.Drawing.Size(200, 33);
			this.psPerfRuntimePanel.TabIndex = 1;
			this.psPerfRuntimePanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfRuntimePanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psPerfRuntimePanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psPerfRuntimePanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfRuntimePanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psPerfRuntimePanel.Expanded += new System.EventHandler(this.psPerfFilterPanel_Expanded);
			// 
			// groupBox12
			// 
			this.groupBox12.Controls.Add(this.label17);
			this.groupBox12.Controls.Add(this.funcRuntimeCodeView);
			this.groupBox12.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox12.Location = new System.Drawing.Point(136, 256);
			this.groupBox12.Name = "groupBox12";
			this.groupBox12.Size = new System.Drawing.Size(104, 16);
			this.groupBox12.TabIndex = 15;
			this.groupBox12.TabStop = false;
			this.groupBox12.Text = "Code-Profiling";
			this.groupBox12.Visible = false;
			// 
			// label17
			// 
			this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label17.Location = new System.Drawing.Point(24, 32);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(200, 16);
			this.label17.TabIndex = 12;
			this.label17.Text = "(Only for methods with source-code)";
			// 
			// funcRuntimeCodeView
			// 
			this.funcRuntimeCodeView.Checked = true;
			this.funcRuntimeCodeView.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeCodeView.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeCodeView.Location = new System.Drawing.Point(16, 16);
			this.funcRuntimeCodeView.Name = "funcRuntimeCodeView";
			this.funcRuntimeCodeView.Size = new System.Drawing.Size(160, 16);
			this.funcRuntimeCodeView.TabIndex = 11;
			this.funcRuntimeCodeView.Text = "Perform line-level profiling.";
			this.funcRuntimeCodeView.CheckedChanged += new System.EventHandler(this.funcRuntimeCodeView_CheckedChanged);
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.funcRuntimeCalleeFunctions);
			this.groupBox11.Controls.Add(this.label7);
			this.groupBox11.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
			this.groupBox11.Location = new System.Drawing.Point(40, 96);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(576, 72);
			this.groupBox11.TabIndex = 13;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Callee Function Profiling";
			// 
			// funcRuntimeCalleeFunctions
			// 
			this.funcRuntimeCalleeFunctions.Checked = true;
			this.funcRuntimeCalleeFunctions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeCalleeFunctions.Font = new System.Drawing.Font("Arial", 8.25F);
			this.funcRuntimeCalleeFunctions.Location = new System.Drawing.Point(8, 24);
			this.funcRuntimeCalleeFunctions.Name = "funcRuntimeCalleeFunctions";
			this.funcRuntimeCalleeFunctions.Size = new System.Drawing.Size(208, 32);
			this.funcRuntimeCalleeFunctions.TabIndex = 0;
			this.funcRuntimeCalleeFunctions.Text = "Collect  function-call hierarchy information.";
			this.funcRuntimeCalleeFunctions.CheckedChanged += new System.EventHandler(this.funcRuntimeCalleeFunctions_CheckedChanged);
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label7.Location = new System.Drawing.Point(232, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(304, 40);
			this.label7.TabIndex = 10;
			this.label7.Text = "( Select to profile which functions were called by a particular function and vice" +
				"-versa. It\'s a CPU intensive operation. Recommended for performance profiling)";
			// 
			// groupBox9
			// 
			this.groupBox9.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.groupBox9.Controls.Add(this.calleeFunctionTree);
			this.groupBox9.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox9.Location = new System.Drawing.Point(424, 264);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(72, 24);
			this.groupBox9.TabIndex = 9;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Callee functions";
			this.groupBox9.Visible = false;
			// 
			// calleeFunctionTree
			// 
			this.calleeFunctionTree.CheckBoxes = true;
			this.calleeFunctionTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.calleeFunctionTree.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.calleeFunctionTree.ImageIndex = -1;
			this.calleeFunctionTree.Indent = 19;
			this.calleeFunctionTree.ItemHeight = 18;
			this.calleeFunctionTree.Location = new System.Drawing.Point(3, 16);
			this.calleeFunctionTree.Name = "calleeFunctionTree";
			this.calleeFunctionTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
																						   new System.Windows.Forms.TreeNode("Profile Callee Functions [Names Only]", new System.Windows.Forms.TreeNode[] {
																																																			  new System.Windows.Forms.TreeNode("Number of Calls"),
																																																			  new System.Windows.Forms.TreeNode("Duration (*resolution same as parent)")})});
			this.calleeFunctionTree.SelectedImageIndex = -1;
			this.calleeFunctionTree.Size = new System.Drawing.Size(66, 5);
			this.calleeFunctionTree.TabIndex = 0;
			this.calleeFunctionTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.calleeFunctionTree_AfterCheck);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.funcRuntimeManagedOnly);
			this.groupBox8.Controls.Add(this.label6);
			this.groupBox8.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox8.Location = new System.Drawing.Point(336, 256);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(72, 16);
			this.groupBox8.TabIndex = 8;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Platform";
			this.groupBox8.Visible = false;
			// 
			// funcRuntimeManagedOnly
			// 
			this.funcRuntimeManagedOnly.Checked = true;
			this.funcRuntimeManagedOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeManagedOnly.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeManagedOnly.Location = new System.Drawing.Point(8, 24);
			this.funcRuntimeManagedOnly.Name = "funcRuntimeManagedOnly";
			this.funcRuntimeManagedOnly.Size = new System.Drawing.Size(208, 16);
			this.funcRuntimeManagedOnly.TabIndex = 0;
			this.funcRuntimeManagedOnly.Text = "Profile only managed functions.";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(8, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(200, 40);
			this.label6.TabIndex = 1;
			this.label6.Text = "(Select to ignore Native calls made or received by the CLR.)";
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.funcRuntimeExceptionStackTrace);
			this.groupBox7.Controls.Add(this.funcRuntimeExceptions);
			this.groupBox7.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox7.Location = new System.Drawing.Point(248, 256);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(80, 16);
			this.groupBox7.TabIndex = 7;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Exception-Tracing";
			this.groupBox7.Visible = false;
			// 
			// funcRuntimeExceptionStackTrace
			// 
			this.funcRuntimeExceptionStackTrace.Checked = true;
			this.funcRuntimeExceptionStackTrace.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeExceptionStackTrace.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeExceptionStackTrace.Location = new System.Drawing.Point(48, 24);
			this.funcRuntimeExceptionStackTrace.Name = "funcRuntimeExceptionStackTrace";
			this.funcRuntimeExceptionStackTrace.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeExceptionStackTrace.TabIndex = 1;
			this.funcRuntimeExceptionStackTrace.Text = "Exception StackTrace";
			this.funcRuntimeExceptionStackTrace.CheckedChanged += new System.EventHandler(this.funcRuntimeExceptionStackTrace_CheckedChanged);
			// 
			// funcRuntimeExceptions
			// 
			this.funcRuntimeExceptions.Checked = true;
			this.funcRuntimeExceptions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeExceptions.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeExceptions.Location = new System.Drawing.Point(16, 24);
			this.funcRuntimeExceptions.Name = "funcRuntimeExceptions";
			this.funcRuntimeExceptions.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeExceptions.TabIndex = 0;
			this.funcRuntimeExceptions.Text = "Collect  Exceptions information.";
			this.funcRuntimeExceptions.CheckedChanged += new System.EventHandler(this.funcRuntimeExceptions_CheckedChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.funcRuntimeTimeResolution);
			this.groupBox6.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox6.Location = new System.Drawing.Point(40, 176);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(360, 56);
			this.groupBox6.TabIndex = 6;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Performance measurement units";
			// 
			// funcRuntimeTimeResolution
			// 
			this.funcRuntimeTimeResolution.BackColor = System.Drawing.SystemColors.Window;
			this.funcRuntimeTimeResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.funcRuntimeTimeResolution.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeTimeResolution.ForeColor = System.Drawing.SystemColors.ControlText;
			this.funcRuntimeTimeResolution.Items.AddRange(new object[] {
																		   "CPU Cycles",
																		   "High resolution timer (in 1/10000 of Milliseconds)",
																		   "Low resolution timer (in Milliseconds)"});
			this.funcRuntimeTimeResolution.Location = new System.Drawing.Point(8, 24);
			this.funcRuntimeTimeResolution.MaxDropDownItems = 3;
			this.funcRuntimeTimeResolution.Name = "funcRuntimeTimeResolution";
			this.funcRuntimeTimeResolution.Size = new System.Drawing.Size(320, 24);
			this.funcRuntimeTimeResolution.TabIndex = 6;
			// 
			// funcRuntimeNumberOfCalls
			// 
			this.funcRuntimeNumberOfCalls.Checked = true;
			this.funcRuntimeNumberOfCalls.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeNumberOfCalls.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeNumberOfCalls.Location = new System.Drawing.Point(56, 256);
			this.funcRuntimeNumberOfCalls.Name = "funcRuntimeNumberOfCalls";
			this.funcRuntimeNumberOfCalls.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeNumberOfCalls.TabIndex = 4;
			this.funcRuntimeNumberOfCalls.Text = "Collect  number of times each function was invoked";
			this.funcRuntimeNumberOfCalls.Visible = false;
			this.funcRuntimeNumberOfCalls.CheckedChanged += new System.EventHandler(this.funcRuntimeNumberOfCalls_CheckedChanged);
			// 
			// funcRuntimeThreadID
			// 
			this.funcRuntimeThreadID.Checked = true;
			this.funcRuntimeThreadID.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeThreadID.Enabled = false;
			this.funcRuntimeThreadID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeThreadID.Location = new System.Drawing.Point(104, 256);
			this.funcRuntimeThreadID.Name = "funcRuntimeThreadID";
			this.funcRuntimeThreadID.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeThreadID.TabIndex = 3;
			this.funcRuntimeThreadID.Text = "Collect Thread ID ";
			this.funcRuntimeThreadID.Visible = false;
			// 
			// funcRuntimeModuleName
			// 
			this.funcRuntimeModuleName.Checked = true;
			this.funcRuntimeModuleName.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeModuleName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeModuleName.Location = new System.Drawing.Point(80, 256);
			this.funcRuntimeModuleName.Name = "funcRuntimeModuleName";
			this.funcRuntimeModuleName.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeModuleName.TabIndex = 2;
			this.funcRuntimeModuleName.Text = "Collect module name for each function.";
			this.funcRuntimeModuleName.Visible = false;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label4.Location = new System.Drawing.Point(376, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(256, 32);
			this.label4.TabIndex = 1;
			this.label4.Text = "( Select to infer fully qualified function signature. Otherwise only function-tit" +
				"le is collected)";
			// 
			// funcRuntimeFuncSignature
			// 
			this.funcRuntimeFuncSignature.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeFuncSignature.Location = new System.Drawing.Point(16, 56);
			this.funcRuntimeFuncSignature.Name = "funcRuntimeFuncSignature";
			this.funcRuntimeFuncSignature.Size = new System.Drawing.Size(352, 16);
			this.funcRuntimeFuncSignature.TabIndex = 0;
			this.funcRuntimeFuncSignature.Text = "Collect  complete  function-signature for each profiled function.";
			// 
			// funcRuntimeCallTimes
			// 
			this.funcRuntimeCallTimes.Checked = true;
			this.funcRuntimeCallTimes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.funcRuntimeCallTimes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcRuntimeCallTimes.Location = new System.Drawing.Point(32, 256);
			this.funcRuntimeCallTimes.Name = "funcRuntimeCallTimes";
			this.funcRuntimeCallTimes.Size = new System.Drawing.Size(16, 16);
			this.funcRuntimeCallTimes.TabIndex = 5;
			this.funcRuntimeCallTimes.Text = "Collect time consumed by each function.";
			this.funcRuntimeCallTimes.Visible = false;
			this.funcRuntimeCallTimes.CheckedChanged += new System.EventHandler(this.funcRuntimeCallTimes_CheckedChanged);
			// 
			// psPerfFilterPanel
			// 
			this.psPerfFilterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psPerfFilterPanel.AnimationRate = 0;
			this.psPerfFilterPanel.BackColor = System.Drawing.Color.Transparent;
			this.psPerfFilterPanel.Caption = "Filters";
			this.psPerfFilterPanel.CaptionCornerType = PSUI.CornerType.Top;
			this.psPerfFilterPanel.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.psPerfFilterPanel.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.psPerfFilterPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psPerfFilterPanel.CaptionUnderline = System.Drawing.Color.Gray;
			this.psPerfFilterPanel.Controls.Add(this.psPerfFilterTab);
			this.psPerfFilterPanel.CurveRadius = 12;
			this.psPerfFilterPanel.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psPerfFilterPanel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psPerfFilterPanel.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psPerfFilterPanel.ImageItems.Disabled = -1;
			this.psPerfFilterPanel.ImageItems.Highlight = -1;
			this.psPerfFilterPanel.ImageItems.Normal = -1;
			this.psPerfFilterPanel.ImageItems.Pressed = -1;
			this.psPerfFilterPanel.ImageItems.PSImgSet = null;
			this.psPerfFilterPanel.Location = new System.Drawing.Point(8, 8);
			this.psPerfFilterPanel.Name = "psPerfFilterPanel";
			this.psPerfFilterPanel.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfFilterPanel.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfFilterPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psPerfFilterPanel.Size = new System.Drawing.Size(668, 280);
			this.psPerfFilterPanel.TabIndex = 0;
			this.psPerfFilterPanel.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfFilterPanel.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psPerfFilterPanel.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psPerfFilterPanel.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.filterToolTip.SetToolTip(this.psPerfFilterPanel, "Filters let you select which classes or namespaces or modules will (or will not) " +
				"be profiled.");
			this.psPerfFilterPanel.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psPerfFilterPanel.Expanded += new System.EventHandler(this.psPerfFilterPanel_Expanded);
			// 
			// psPerfFilterTab
			// 
			this.psPerfFilterTab.Controls.Add(this.psPerfClassFilterTab);
			this.psPerfFilterTab.Controls.Add(this.psPerfModuleFilterTab);
			this.psPerfFilterTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.psPerfFilterTab.ItemSize = new System.Drawing.Size(62, 18);
			this.psPerfFilterTab.Location = new System.Drawing.Point(2, 35);
			this.psPerfFilterTab.Multiline = true;
			this.psPerfFilterTab.Name = "psPerfFilterTab";
			this.psPerfFilterTab.SelectedIndex = 0;
			this.psPerfFilterTab.Size = new System.Drawing.Size(664, 242);
			this.psPerfFilterTab.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.psPerfFilterTab.TabIndex = 0;
			// 
			// psPerfClassFilterTab
			// 
			this.psPerfClassFilterTab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfClassFilterTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.psPerfClassFilterTab.Controls.Add(this.FunctionClassFilterGroupBox);
			this.psPerfClassFilterTab.Location = new System.Drawing.Point(4, 22);
			this.psPerfClassFilterTab.Name = "psPerfClassFilterTab";
			this.psPerfClassFilterTab.Size = new System.Drawing.Size(656, 216);
			this.psPerfClassFilterTab.TabIndex = 0;
			this.psPerfClassFilterTab.Text = "Class Filter";
			// 
			// FunctionClassFilterGroupBox
			// 
			this.FunctionClassFilterGroupBox.Controls.Add(this.label9);
			this.FunctionClassFilterGroupBox.Controls.Add(this.groupBox4);
			this.FunctionClassFilterGroupBox.Controls.Add(this.funcClassFilter);
			this.FunctionClassFilterGroupBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FunctionClassFilterGroupBox.Location = new System.Drawing.Point(12, 0);
			this.FunctionClassFilterGroupBox.Name = "FunctionClassFilterGroupBox";
			this.FunctionClassFilterGroupBox.Size = new System.Drawing.Size(636, 208);
			this.FunctionClassFilterGroupBox.TabIndex = 3;
			this.FunctionClassFilterGroupBox.TabStop = false;
			this.FunctionClassFilterGroupBox.Text = "Apply Class Filter";
			this.filterToolTip.SetToolTip(this.FunctionClassFilterGroupBox, "Filters let you select which classes or namespaces or modules will (or will not) " +
				"be profiled.");
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label9.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label9.Location = new System.Drawing.Point(16, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(576, 32);
			this.label9.TabIndex = 3;
			this.label9.Text = "Class Filter lets you select which classes or namespaces will (or will not) be pr" +
				"ofiled. Type in fully-qualified .NET classes / namespaces to analyze for perform" +
				"ance analysis (Case-Sensitive) and select a Filter Option.";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioFuncObjBlock);
			this.groupBox4.Controls.Add(this.radioFuncObjPassthrough);
			this.groupBox4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox4.Location = new System.Drawing.Point(328, 112);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(296, 88);
			this.groupBox4.TabIndex = 2;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Filter Option";
			// 
			// radioFuncObjBlock
			// 
			this.radioFuncObjBlock.Checked = true;
			this.radioFuncObjBlock.Location = new System.Drawing.Point(8, 56);
			this.radioFuncObjBlock.Name = "radioFuncObjBlock";
			this.radioFuncObjBlock.Size = new System.Drawing.Size(272, 16);
			this.radioFuncObjBlock.TabIndex = 1;
			this.radioFuncObjBlock.TabStop = true;
			this.radioFuncObjBlock.Text = "Ignore selected Classes / Namespaces";
			// 
			// radioFuncObjPassthrough
			// 
			this.radioFuncObjPassthrough.Location = new System.Drawing.Point(8, 32);
			this.radioFuncObjPassthrough.Name = "radioFuncObjPassthrough";
			this.radioFuncObjPassthrough.Size = new System.Drawing.Size(280, 16);
			this.radioFuncObjPassthrough.TabIndex = 0;
			this.radioFuncObjPassthrough.Text = "Profile only selected Classes / Namespaces.";
			// 
			// funcClassFilter
			// 
			this.funcClassFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.funcClassFilter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcClassFilter.Items.AddRange(new object[] {
																 "System",
																 "Microsoft",
																 "ASP"});
			this.funcClassFilter.Location = new System.Drawing.Point(16, 56);
			this.funcClassFilter.Name = "funcClassFilter";
			this.funcClassFilter.Size = new System.Drawing.Size(304, 144);
			this.funcClassFilter.TabIndex = 0;
			this.filterToolTip.SetToolTip(this.funcClassFilter, "To add a class / namespace, type and press Enter.To remove, select and press Dele" +
				"te.");
			this.funcClassFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objClassFilter_KeyDown);
			this.funcClassFilter.DoubleClick += new System.EventHandler(this.objClassFilter_DoubleClick);
			// 
			// psPerfModuleFilterTab
			// 
			this.psPerfModuleFilterTab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.psPerfModuleFilterTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.psPerfModuleFilterTab.Controls.Add(this.groupBox3);
			this.psPerfModuleFilterTab.Location = new System.Drawing.Point(4, 22);
			this.psPerfModuleFilterTab.Name = "psPerfModuleFilterTab";
			this.psPerfModuleFilterTab.Size = new System.Drawing.Size(656, 216);
			this.psPerfModuleFilterTab.TabIndex = 1;
			this.psPerfModuleFilterTab.Text = "Module Filter";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.groupBox5);
			this.groupBox3.Controls.Add(this.funcModuleFilter);
			this.groupBox3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(13, 7);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(632, 240);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Apply Module Filter";
			this.filterToolTip.SetToolTip(this.groupBox3, "Filters let you select which classes or namespaces or modules will (or will not) " +
				"be profiled.");
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label2.Location = new System.Drawing.Point(16, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(568, 32);
			this.label2.TabIndex = 4;
			this.label2.Text = "Module Filter lets you select which .NET assembly / module will (or will not) be " +
				" profiled. Type in module names to analyze for performance analysis (Case-Insens" +
				"itive) and select a Filter Option.";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.radioPerfModuleBlock);
			this.groupBox5.Controls.Add(this.radioPerfModulePassthrough);
			this.groupBox5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox5.Location = new System.Drawing.Point(336, 112);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(288, 88);
			this.groupBox5.TabIndex = 2;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Filter Option";
			// 
			// radioPerfModuleBlock
			// 
			this.radioPerfModuleBlock.Location = new System.Drawing.Point(8, 56);
			this.radioPerfModuleBlock.Name = "radioPerfModuleBlock";
			this.radioPerfModuleBlock.Size = new System.Drawing.Size(272, 16);
			this.radioPerfModuleBlock.TabIndex = 1;
			this.radioPerfModuleBlock.Text = "Ignore selected modules";
			// 
			// radioPerfModulePassthrough
			// 
			this.radioPerfModulePassthrough.Checked = true;
			this.radioPerfModulePassthrough.Location = new System.Drawing.Point(8, 32);
			this.radioPerfModulePassthrough.Name = "radioPerfModulePassthrough";
			this.radioPerfModulePassthrough.Size = new System.Drawing.Size(272, 16);
			this.radioPerfModulePassthrough.TabIndex = 0;
			this.radioPerfModulePassthrough.TabStop = true;
			this.radioPerfModulePassthrough.Text = "Profile only selected modules";
			// 
			// funcModuleFilter
			// 
			this.funcModuleFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.funcModuleFilter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.funcModuleFilter.Location = new System.Drawing.Point(16, 56);
			this.funcModuleFilter.Name = "funcModuleFilter";
			this.funcModuleFilter.Size = new System.Drawing.Size(312, 144);
			this.funcModuleFilter.TabIndex = 0;
			this.filterToolTip.SetToolTip(this.funcModuleFilter, "To add a module / assembly name, type and press Enter.To remove, select and press" +
				" Delete.");
			this.funcModuleFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.funcModuleFilter_KeyDown);
			this.funcModuleFilter.DoubleClick += new System.EventHandler(this.objClassFilter_DoubleClick);
			// 
			// tabConfig
			// 
			this.tabConfig.Controls.Add(this.MemTab);
			this.tabConfig.Controls.Add(this.PerfTab);
			this.tabConfig.Controls.Add(this.profileeTab);
			this.tabConfig.ItemSize = new System.Drawing.Size(230, 24);
			this.tabConfig.Location = new System.Drawing.Point(0, 0);
			this.tabConfig.Multiline = true;
			this.tabConfig.Name = "tabConfig";
			this.tabConfig.SelectedIndex = 0;
			this.tabConfig.Size = new System.Drawing.Size(696, 384);
			this.tabConfig.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabConfig.TabIndex = 0;
			this.tabConfig.SelectedIndexChanged += new System.EventHandler(this.tabConfig_SelectedIndexChanged);
			// 
			// profileeTab
			// 
			this.profileeTab.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.profileeTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.profileeTab.Controls.Add(this.groupBox10);
			this.profileeTab.Controls.Add(this.newProfileeGroupBox);
			this.profileeTab.Controls.Add(this.profileeView);
			this.profileeTab.Controls.Add(this.defaultAttach);
			this.profileeTab.Location = new System.Drawing.Point(4, 28);
			this.profileeTab.Name = "profileeTab";
			this.profileeTab.Size = new System.Drawing.Size(688, 352);
			this.profileeTab.TabIndex = 2;
			this.profileeTab.Text = "Profilee Setup";
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.label8);
			this.groupBox10.Controls.Add(this.label5);
			this.groupBox10.Controls.Add(this.profileeAlreadyRunning);
			this.groupBox10.Controls.Add(this.radioProfileeSuspended);
			this.groupBox10.Controls.Add(this.profileeInteractiveSuspended);
			this.groupBox10.Controls.Add(this.profileeInteractive);
			this.groupBox10.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox10.Location = new System.Drawing.Point(0, 0);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(684, 136);
			this.groupBox10.TabIndex = 2;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Select a session type";
			this.groupBox10.Visible = false;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label8.Location = new System.Drawing.Point(48, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(344, 32);
			this.label8.TabIndex = 7;
			this.label8.Text = "(Suspends a process before the CLR is loaded so that you can start profiling it i" +
				"mmediately. Recommended for memory profiling.)";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.Red;
			this.label5.Location = new System.Drawing.Point(32, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(632, 32);
			this.label5.TabIndex = 6;
			this.label5.Text = "Please select a process to profile from the list-view below and click \'Accept\' to" +
				" start profiling it. If the process you want to profile is not listed, you may n" +
				"eed to restart the process without closing this dialog.";
			// 
			// profileeAlreadyRunning
			// 
			this.profileeAlreadyRunning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.profileeAlreadyRunning.Location = new System.Drawing.Point(24, 56);
			this.profileeAlreadyRunning.Name = "profileeAlreadyRunning";
			this.profileeAlreadyRunning.Size = new System.Drawing.Size(264, 16);
			this.profileeAlreadyRunning.TabIndex = 5;
			this.profileeAlreadyRunning.Text = "Select an already running  profilable  process.";
			this.profileeAlreadyRunning.CheckedChanged += new System.EventHandler(this.profileeAlreadyRunning_CheckedChanged);
			// 
			// radioProfileeSuspended
			// 
			this.radioProfileeSuspended.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioProfileeSuspended.Location = new System.Drawing.Point(24, 80);
			this.radioProfileeSuspended.Name = "radioProfileeSuspended";
			this.radioProfileeSuspended.Size = new System.Drawing.Size(640, 16);
			this.radioProfileeSuspended.TabIndex = 3;
			this.radioProfileeSuspended.Text = "Suspend all new  .NET processes and services that  start  now onwards. I  will se" +
				"lect  the process to profile and resume it.";
			this.radioProfileeSuspended.CheckedChanged += new System.EventHandler(this.profileeAlreadyRunning_CheckedChanged);
			// 
			// profileeInteractiveSuspended
			// 
			this.profileeInteractiveSuspended.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.profileeInteractiveSuspended.Location = new System.Drawing.Point(624, 8);
			this.profileeInteractiveSuspended.Name = "profileeInteractiveSuspended";
			this.profileeInteractiveSuspended.Size = new System.Drawing.Size(16, 16);
			this.profileeInteractiveSuspended.TabIndex = 1;
			this.profileeInteractiveSuspended.Text = "Start a new process and start profiling it right away.";
			this.profileeInteractiveSuspended.Visible = false;
			this.profileeInteractiveSuspended.CheckedChanged += new System.EventHandler(this.profileeInteractive_CheckedChanged);
			// 
			// profileeInteractive
			// 
			this.profileeInteractive.Checked = true;
			this.profileeInteractive.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.profileeInteractive.Location = new System.Drawing.Point(608, 8);
			this.profileeInteractive.Name = "profileeInteractive";
			this.profileeInteractive.Size = new System.Drawing.Size(16, 16);
			this.profileeInteractive.TabIndex = 0;
			this.profileeInteractive.TabStop = true;
			this.profileeInteractive.Text = "Start a new process. I will profile it after it has started.";
			this.profileeInteractive.Visible = false;
			this.profileeInteractive.CheckedChanged += new System.EventHandler(this.profileeInteractive_CheckedChanged);
			// 
			// newProfileeGroupBox
			// 
			this.newProfileeGroupBox.Controls.Add(this.label3);
			this.newProfileeGroupBox.Controls.Add(this.label16);
			this.newProfileeGroupBox.Controls.Add(this.label15);
			this.newProfileeGroupBox.Controls.Add(this.browseProfileeApp);
			this.newProfileeGroupBox.Controls.Add(this.ProfileeCmdlineParam);
			this.newProfileeGroupBox.Controls.Add(this.ProfileeAppFolder);
			this.newProfileeGroupBox.Controls.Add(this.ProfileeApp);
			this.newProfileeGroupBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.newProfileeGroupBox.Location = new System.Drawing.Point(16, 48);
			this.newProfileeGroupBox.Name = "newProfileeGroupBox";
			this.newProfileeGroupBox.Size = new System.Drawing.Size(656, 248);
			this.newProfileeGroupBox.TabIndex = 21;
			this.newProfileeGroupBox.TabStop = false;
			this.newProfileeGroupBox.Text = "Select a .NET Desktop Application to Profile";
			this.newProfileeGroupBox.Visible = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
				| System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(16, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Select application";
			// 
			// label16
			// 
			this.label16.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
				| System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label16.Location = new System.Drawing.Point(16, 176);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(112, 16);
			this.label16.TabIndex = 5;
			this.label16.Text = "Start parameters.";
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
				| System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label15.Location = new System.Drawing.Point(16, 104);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(104, 16);
			this.label15.TabIndex = 4;
			this.label15.Text = "Startup folder.";
			// 
			// browseProfileeApp
			// 
			this.browseProfileeApp.Cursor = System.Windows.Forms.Cursors.Hand;
			this.browseProfileeApp.Location = new System.Drawing.Point(576, 56);
			this.browseProfileeApp.Name = "browseProfileeApp";
			this.browseProfileeApp.Size = new System.Drawing.Size(40, 24);
			this.browseProfileeApp.TabIndex = 3;
			this.browseProfileeApp.Text = "...";
			this.filterToolTip.SetToolTip(this.browseProfileeApp, "Browse and select an application to profile.");
			this.browseProfileeApp.Click += new System.EventHandler(this.browseProfileeApp_Click);
			// 
			// ProfileeCmdlineParam
			// 
			this.ProfileeCmdlineParam.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ProfileeCmdlineParam.Location = new System.Drawing.Point(24, 200);
			this.ProfileeCmdlineParam.Name = "ProfileeCmdlineParam";
			this.ProfileeCmdlineParam.Size = new System.Drawing.Size(536, 20);
			this.ProfileeCmdlineParam.TabIndex = 2;
			this.ProfileeCmdlineParam.Text = "";
			// 
			// ProfileeAppFolder
			// 
			this.ProfileeAppFolder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ProfileeAppFolder.Location = new System.Drawing.Point(24, 128);
			this.ProfileeAppFolder.Name = "ProfileeAppFolder";
			this.ProfileeAppFolder.Size = new System.Drawing.Size(536, 20);
			this.ProfileeAppFolder.TabIndex = 1;
			this.ProfileeAppFolder.Text = "";
			// 
			// ProfileeApp
			// 
			this.ProfileeApp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ProfileeApp.Location = new System.Drawing.Point(24, 56);
			this.ProfileeApp.Name = "ProfileeApp";
			this.ProfileeApp.Size = new System.Drawing.Size(536, 20);
			this.ProfileeApp.TabIndex = 0;
			this.ProfileeApp.Text = "Application to run..";
			// 
			// profileeView
			// 
			this.profileeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.profileeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.processID,
																						   this.processName,
																						   this.Status,
																						   this.profilingType});
			this.profileeView.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.profileeView.FullRowSelect = true;
			this.profileeView.GridLines = true;
			this.profileeView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.profileeView.Location = new System.Drawing.Point(16, 152);
			this.profileeView.MultiSelect = false;
			this.profileeView.Name = "profileeView";
			this.profileeView.Size = new System.Drawing.Size(644, 160);
			this.profileeView.TabIndex = 20;
			this.profileeView.View = System.Windows.Forms.View.Details;
			this.profileeView.Visible = false;
			this.profileeView.VisibleChanged += new System.EventHandler(this.profileeView_VisibleChanged);
			this.profileeView.Leave += new System.EventHandler(this.profileeView_Leave);
			// 
			// processID
			// 
			this.processID.Text = "PID";
			// 
			// processName
			// 
			this.processName.Text = "Process Name";
			this.processName.Width = 260;
			// 
			// Status
			// 
			this.Status.Text = "Status";
			this.Status.Width = 115;
			// 
			// profilingType
			// 
			this.profilingType.Text = "Configured For";
			this.profilingType.Width = 200;
			// 
			// defaultAttach
			// 
			this.defaultAttach.Checked = true;
			this.defaultAttach.CheckState = System.Windows.Forms.CheckState.Checked;
			this.defaultAttach.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.defaultAttach.Location = new System.Drawing.Point(320, 320);
			this.defaultAttach.Name = "defaultAttach";
			this.defaultAttach.Size = new System.Drawing.Size(320, 16);
			this.defaultAttach.TabIndex = 8;
			this.defaultAttach.Text = "Automatically attach and start profiling new processes.";
			// 
			// openProfileeDialog
			// 
			this.openProfileeDialog.DefaultExt = "exe";
			this.openProfileeDialog.Filter = "Executable Files|*.exe";
			this.openProfileeDialog.FilterIndex = 0;
			this.openProfileeDialog.ReadOnlyChecked = true;
			this.openProfileeDialog.Title = "Select profilee";
			// 
			// filterToolTip
			// 
			this.filterToolTip.AutomaticDelay = 200;
			this.filterToolTip.AutoPopDelay = 8000;
			this.filterToolTip.InitialDelay = 200;
			this.filterToolTip.ReshowDelay = 40;
			this.filterToolTip.ShowAlways = true;
			// 
			// Configurator
			// 
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Controls.Add(this.rejectSettings);
			this.Controls.Add(this.acceptSettings);
			this.Controls.Add(this.tabConfig);
			this.Name = "Configurator";
			this.Size = new System.Drawing.Size(696, 424);
			this.Load += new System.EventHandler(this.Configurator_Load);
			this.MemTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.psMemPanelGroup)).EndInit();
			this.psMemPanelGroup.ResumeLayout(false);
			this.psMemDataCollection.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.psMemFilterPanel.ResumeLayout(false);
			this.ObjectClassFilterGroupBox.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.PerfTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.psPerfPanelGroup)).EndInit();
			this.psPerfPanelGroup.ResumeLayout(false);
			this.psPerfRuntimePanel.ResumeLayout(false);
			this.groupBox12.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.psPerfFilterPanel.ResumeLayout(false);
			this.psPerfFilterTab.ResumeLayout(false);
			this.psPerfClassFilterTab.ResumeLayout(false);
			this.FunctionClassFilterGroupBox.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.psPerfModuleFilterTab.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.tabConfig.ResumeLayout(false);
			this.profileeTab.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			this.newProfileeGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void objClassFilter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			ComboBox senderClassFilterObject=(ComboBox)sender;

			if(e.KeyValue==46)
			{
				try
				{					
					senderClassFilterObject.Items.RemoveAt(senderClassFilterObject.SelectedIndex);
					senderClassFilterObject.Refresh();  
				}
				catch{}

			}		
			

			else if(e.KeyValue==13 && senderClassFilterObject.Text!=null && senderClassFilterObject.Text.Length!=0  ) 
			{
				foreach (object obj  in senderClassFilterObject.Items )
				{
					if(obj.ToString().Trim()==senderClassFilterObject.Text.Trim() )
					{
						return;
					}
				}
				senderClassFilterObject.Items.Add(senderClassFilterObject.Text.Trim() );  
				senderClassFilterObject.Text=null;  
			}
			
		}

		private void psMemPanel_Expanded(object sender, System.EventArgs e)
		{
			foreach (Control psControl in psMemPanelGroup.Controls) 
			{
				if(psControl.GetType()==typeof(PSPanel) && psControl!=sender  )
				{
					((PSPanel)psControl).PanelState =PSPanelState.Collapsed;  

				}

			}
		}

		private void psPerfFilterPanel_Expanded(object sender, System.EventArgs e)
		{
			foreach (Control psControl in psPerfPanelGroup.Controls) 
			{
				if(psControl.GetType()==typeof(PSPanel)  )
				{
					if(psControl!=sender )
					{
						((PSPanel)psControl).PanelState =PSPanelState.Collapsed;  
					}
					else 
					{
//						if(((PSPanel)psControl).Caption.ToUpper()=="RUNTIME")
//						{
//								psControl.Size=new Size(672,367);  
//						}
//						else
//						{
//							psControl.Size=new Size(672,320);  
//						}
					}
					
				}
				Application.DoEvents(); 
			}

		}

		

		private void funcModuleFilter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			ComboBox senderModuleFilterObject=(ComboBox)sender;

			if(e.KeyValue==46)
			{
				try
				{					
					senderModuleFilterObject.Items.RemoveAt(senderModuleFilterObject.SelectedIndex);
					senderModuleFilterObject.Refresh();  
				}
				catch{}

			}		
			

			else if(e.KeyValue==13 && senderModuleFilterObject.Text!=null && senderModuleFilterObject.Text.Length!=0  ) 
			{
				foreach (object obj  in senderModuleFilterObject.Items )
				{
					if(obj.ToString().Trim().ToLower()== senderModuleFilterObject.Text.Trim().ToLower()  )
					{
						senderModuleFilterObject.Text=null;  
						MessageBox.Show("The module ["+obj.ToString()+"] has already been selected. (The comparison is case-insensitive)","Duplicate module!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation )  ;
						return;
					}					
				}

				senderModuleFilterObject.Items.Add(senderModuleFilterObject.Text.Trim() );  
				senderModuleFilterObject.Text=null;  
			}
		
		}

		private void Configurator_Load(object sender, System.EventArgs e)
		{			

			if(this.ParentForm!=null)
			{
				this.ParentForm.Closing += new System.ComponentModel.CancelEventHandler(this.Configurator_Closing);			
			}

			if(this.tabConfig.SelectedTab==profileeTab)
			{//Accept new settings if the configurations are to be set automatically
				AcceptData();
			}

			try
			{
				//V. IMP.
				RestoreDefaultConfiguration();//Clean Up all configurations before starting again				
			}
			catch{}
			///

			try
			{	
				if(PerfTab !=null)
				{
					calleeFunctionTree.ExpandAll();
					funcRuntimeTimeResolution.SelectedIndex=1;  					
					Application.DoEvents(); 					
					
				}
			}
			catch{}

				SyncUI();	
			try
			{
				filterToolTip.SetToolTip(defaultAttach,"Automatically attaches the profiler to any new process started by the selected application and starts profiling it with similar profiling configuration.\nRecommended for applications that call into ASP.NET, Web-Services, Windows Services or dllhost processes.");
			}
			catch{}
			Application.DoEvents();  
			
		}

		public void SyncUI()
		{
			if(objectDataObj !=null && MemTab!=null)
			{				
				SyncMemoryUI();

			}
			if(functionDataObj!=null && PerfTab!=null)
			{
                SyncPerformanceUI(); 
			}

		}

		private void SyncMemoryUI()
		{
			#region Memory UI Sync

			WANT_OBJECT_NAME_ONLY =( ((objectDataObj.m_iObjectsFlag) & 
				((int)OBJECT_FILTER.OF_OBJECT_NAME_ONLY))==(int)OBJECT_FILTER.OF_OBJECT_NAME_ONLY );
			WANT_OBJECT_COUNT= ( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_OBJECT_COUNT))==(int)OBJECT_FILTER.OF_OBJECT_COUNT );
			WANT_OBJECT_SIZE= ( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_OBJECT_SIZE))==(int)OBJECT_FILTER.OF_OBJECT_SIZE );
			WANT_REFERENCED_OBJECTS= ( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_REFERENCED_OBJECTS))==(int)OBJECT_FILTER.OF_REFERENCED_OBJECTS );
			WANT_OBJECT_ALLOCATION_DATA= ( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_OBJECT_ALLOCATION))==(int)OBJECT_FILTER.OF_OBJECT_ALLOCATION );	
			WANT_SRC_ANALYSIS_ONLY= ( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_OBJECT_SRC_ANALYSIS_ONLY ))==(int)OBJECT_FILTER.OF_OBJECT_SRC_ANALYSIS_ONLY );	
			WANT_OBJECT_ALL_DATA =( ((objectDataObj.m_iObjectsFlag) 
				&((int)OBJECT_FILTER.OF_OBJECT_ALL_DATA))==(int)OBJECT_FILTER.OF_OBJECT_ALL_DATA );

			if(objectDataObj.m_strObjectClassFilter!=null)
			{
				try
				{
					objClassFilter.Items.Clear();   
					foreach (string objClass in objectDataObj.m_strObjectClassFilter.Trim().Split(new char[]{'|'},25))
					{
						if(objClass.Length>0)  
							objClassFilter.Items.Add(objClass);   
					}
				}
				catch{}
			}

			if(objectDataObj.m_bObjectClassPassthrough)
			{
				radioMemObjPassthrough.Checked=true  ;
			}
			else
			{
				radioMemObjectBlock.Checked =true; 
			}
				
			if(configString=="READONLY" && objectDataObj.m_iObjectsFlag!=0  )
			{
				objRuntimeObjectCount.Checked =(WANT_OBJECT_COUNT)?true:false;						
				objRuntimeObjectSize.Checked=(WANT_OBJECT_SIZE)?true:false;
				objRuntimeReferencedObjectData.Checked =(WANT_REFERENCED_OBJECTS)?true:false;
				objGCBeforeObjectCollection.Checked =objectDataObj.m_bGCBeforeOC;  			
				objRuntimeObjectAllocation.Checked =(WANT_OBJECT_ALLOCATION_DATA)?true:false;	  
				objRuntimeSrcCodeAnalysis.Checked=(WANT_SRC_ANALYSIS_ONLY)?true:false;  
			}

			#endregion
		}

		private void SyncPerformanceUI()
		{
			#region Performance UI Sync

			WANT_FUNCTION_NAME = ( ((functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_NAME_ONLY))  == 
				(int)FUNCTION_FILTER.FF_FUNCTION_NAME_ONLY) ;
			WANT_FUNCTION_SIGNATURE= ( ((functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_SIGNATURE ))				==(int)FUNCTION_FILTER.FF_FUNCTION_SIGNATURE);
			WANT_NUMBER_OF_FUNCTION_CALLS= ( ((functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_NUMBER_OF_CALLS_ONLY )) ==(int)FUNCTION_FILTER.FF_FUNCTION_NUMBER_OF_CALLS_ONLY);
			WANT_FUNCTION_TOTAL_CPU_TIME =( ( (functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_TOTAL_CPU_TIME )) ==(int)FUNCTION_FILTER.FF_FUNCTION_TOTAL_CPU_TIME);
			WANT_FUNCTION_MODULE= ( ((functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_MODULE )) 				==(int)FUNCTION_FILTER.FF_FUNCTION_MODULE);
			WANT_FUNCTION_EXCEPTIONS=  ( ( (functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS )) 				==(int)FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS);
			WANT_FUNCTION_EXCEPTIONS_STACKTRACE=  ( ( (functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS_STACKTRACE )) ==(int)FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS_STACKTRACE);
			WANT_FUNCTION_CALLEE_ID=  (  (  (functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_ID )) 				==(int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_ID);

			WANT_FUNCTION_CODE_VIEW= (  (  (functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_CODE_VIEW )) 				==(int)FUNCTION_FILTER.FF_FUNCTION_CODE_VIEW );				
			WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS = ( ( (functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY )) 				==(int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY);
			WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME  =(  ((functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_TOTAL_CPU_TIME )) ==(int)FUNCTION_FILTER.FF_FUNCTION_CALLEE_TOTAL_CPU_TIME);
			WANT_FUNCTION_THREAD_ID=  ( ( (functionDataObj.m_iFunctionFlag ) & ((int)FUNCTION_FILTER.FF_FUNCTION_THREAD_ID)) 				==(int)FUNCTION_FILTER.FF_FUNCTION_THREAD_ID);
			WANT_FUNCTION_MANAGED_ONLY = ( ( (functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_FUNCTION_MANAGED_ONLY)) ==(int)FUNCTION_FILTER.FF_FUNCTION_MANAGED_ONLY);
			WANT_NO_FUNCTION_CALLEE_INFORMATION = ( ( (functionDataObj.m_iFunctionFlag ) & 				((int)FUNCTION_FILTER.FF_NO_FUNCTION_CALLEE_INFORMATION)) ==(int)FUNCTION_FILTER.FF_NO_FUNCTION_CALLEE_INFORMATION);


				
			//////////////////////////////
			///
			if(functionDataObj.m_strFunctionClassFilter!=null  )
			{					
				try
				{
					funcClassFilter.Items.Clear();   
					foreach (string fClass in functionDataObj.m_strFunctionClassFilter.Trim().Split(new char[]{'|'},25))
					{
						if(fClass.Length>0) 
							funcClassFilter.Items.Add(fClass);   
					}
				}
				catch{}
			}

			if(functionDataObj.m_strFunctionModuleFilter !=null  )
			{
				try
				{
					funcModuleFilter.Items.Clear();   
					foreach (string fModule in functionDataObj.m_strFunctionModuleFilter.Trim().Split(new char[]{'|'},25))
					{
						if(fModule.Length>0) 
							funcModuleFilter.Items.Add(fModule);   
					}
				}
				catch{}
			}

			if(functionDataObj.m_bFunctionClassPassthrough)
				radioFuncObjPassthrough.Checked=true;
			else			
				radioFuncObjBlock.Checked =true;				

			if(functionDataObj.m_bFunctionModulePassthrough)
				radioPerfModulePassthrough.Checked =true;
			else
				radioPerfModuleBlock.Checked=true;

			//////////////////////////
			///
			if(configString=="READONLY" && functionDataObj.m_iFunctionFlag!=0 )
			{
				funcRuntimeFuncSignature.Checked =WANT_FUNCTION_SIGNATURE;
				funcRuntimeModuleName.Checked =WANT_FUNCTION_MODULE;
				funcRuntimeThreadID.Checked  =WANT_FUNCTION_THREAD_ID;
				funcRuntimeNumberOfCalls.Checked=WANT_NUMBER_OF_FUNCTION_CALLS;
				funcRuntimeCallTimes.Checked=WANT_FUNCTION_TOTAL_CPU_TIME;
				funcRuntimeCodeView.Checked =WANT_FUNCTION_CODE_VIEW;

				try
				{
					if(functionDataObj.m_iTRFlag==2)  
					{
						funcRuntimeTimeResolution.SelectedIndex=1;
					}
					else if(functionDataObj.m_iTRFlag==4)
					{
						funcRuntimeTimeResolution.SelectedIndex=2;
					}
					else
					{
						funcRuntimeTimeResolution.SelectedIndex=0;
					}
				}
				catch{}
				funcRuntimeExceptions.Checked=WANT_FUNCTION_EXCEPTIONS;
				funcRuntimeExceptionStackTrace.Checked=WANT_FUNCTION_EXCEPTIONS_STACKTRACE;
				funcRuntimeManagedOnly.Checked =WANT_FUNCTION_MANAGED_ONLY;

				try
				{
					funcRuntimeCalleeFunctions.Checked=  WANT_FUNCTION_CALLEE_ID;
					calleeFunctionTree.Nodes[0].Checked=WANT_FUNCTION_CALLEE_ID;
					calleeFunctionTree.Nodes[0].Nodes[0].Checked=WANT_FUNCTION_CALLEE_NUMBER_OF_CALLS;
					calleeFunctionTree.Nodes[0].Nodes[1].Checked=WANT_FUNCTION_CALLEE_TOTAL_CPU_TIME; 					
				}
				catch{}

			}

			#endregion
		}

		private void objRuntimeObjectNameOnly_CheckedChanged(object sender, System.EventArgs e)
		{
			if(objRuntimeObjectNameOnly.Checked==false)
			{
				objRuntimeObjectCount.Checked =false;
				objRuntimeObjectSize.Checked =false;
				objRuntimeReferencedObjectData.Checked =false; 
 
			}
		}

		private void objRuntimeObjectCount_CheckedChanged(object sender, System.EventArgs e)
		{
			if(((CheckBox)sender).Checked==true )
			{
				objRuntimeObjectNameOnly.Checked=true;

			}
		}

		private void funcRuntimeExceptions_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeExceptions.Checked ==false)
			{
				funcRuntimeExceptionStackTrace.Checked =false; 
			}
			else
			{
				if(funcRuntimeCodeView.Checked)
				{
					funcRuntimeCodeView.Checked=false;
				}
			}

		}

		private void funcRuntimeExceptionStackTrace_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeExceptionStackTrace.Checked ==true)
			{
				funcRuntimeExceptions.Checked =true; 
			}
		
		}

		private void calleeFunctionTree_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if(e.Node.Parent==null)
			{
				if(e.Node.Checked==false)
				{
					foreach(TreeNode node in e.Node.Nodes)
					{
						node.Checked=false; 
					}
					return;

				}			
			}
			else
			{
				if(e.Node.Checked==true)
				{
					e.Node.Parent.Checked=true; 
					if(e.Node.Index ==0)//Callee number of calls
					{
						funcRuntimeNumberOfCalls.Checked= true; 						
					}			
					if(e.Node.Index ==1)//Callee TImes
					{
						funcRuntimeCallTimes.Checked=true;
					}				
				}
			}        
			Application.DoEvents();

		}


		public void RefreshProcessView()
		{
			refreshProcessView();
		}

		private void refreshProcessView()
		{
			bool bInteractive=true;

			while(true)
			{
				int pidProfilee=-1;
				if(profileeView.SelectedIndices.Count>0)  
				{
					pidProfilee=Convert.ToInt32(profileeView.SelectedItems[0].Text);    
				}

				this.Cursor =Cursors.WaitCursor;								
				Application.DoEvents();
				profileeView.Items.Clear(); 
				//profileeView.Enabled =false;
				Application.DoEvents();			

				try
				{
					if(bInteractive)
					{
						#region profilee enumerate interactive
 
						foreach(System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
						{	
					
							ListViewItem lvItem;	
							string strPID=process.Id.ToString();
							lvItem=new ListViewItem(strPID);
							string processName=process.MainModule.FileName;
							string exeName=processName.Substring(processName.LastIndexOf("\\")+1,processName.Length-processName.LastIndexOf("\\")-1);
							lvItem.SubItems.Add(exeName);

							try
							{
								IntPtr hMutex=System.IntPtr.Zero;													
								if( ( hMutex=OpenMutex(1,0,"Global\\"+strPID+"?TRUE"))!= System.IntPtr.Zero) 
								{			
									try
									{
										ReleaseMutex(hMutex);
										CloseHandle(hMutex);
									}catch{}
									hMutex=IntPtr.Zero ;

									if(SharpClientForm.profilerTable.ContainsKey(process.Id))
									{										
										if(Convert.ToString(SharpClientForm.profilerTable[process.Id]).ToUpper()== exeName.ToUpper() ) 
										{																		
											lvItem.SubItems.Add("Profiling");										
											lvItem.SubItems.Add(GetProfilingType(process.Id));
											goto A;

										}
									}
									
									lvItem.SubItems.Add("Suspended");
									lvItem.SubItems.Add(GetProfilingType(process.Id));
								}
								else if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED. THIS ITSELF ENSURES THAT THE MUTEX EXISTS						
								{
									if(hMutex!=IntPtr.Zero )
									{
										try
										{
											ReleaseMutex(hMutex);
											CloseHandle(hMutex);
										}
										catch{}
									}
									hMutex=IntPtr.Zero ;

									if(SharpClientForm.profilerTable.ContainsKey(process.Id))
									{																		
										lvItem.SubItems.Add("Profiling");
										lvItem.SubItems.Add(GetProfilingType(process.Id));
										goto A;										
									}			
						
									lvItem.SubItems.Add("Suspended");
									lvItem.SubItems.Add(GetProfilingType(process.Id));
								}
								else if( (hMutex=OpenMutex(1,0,"Global\\"+strPID+"?FALSE")) !=System.IntPtr.Zero)
								{	
									try
									{
										ReleaseMutex(hMutex);
										CloseHandle(hMutex);
									}
									catch{}
									hMutex=IntPtr.Zero ;

									if(SharpClientForm.profilerTable.ContainsKey(process.Id))
									{
										if(Convert.ToString(SharpClientForm.profilerTable[process.Id]).ToUpper()== exeName.ToUpper() ) 
										{																		
											lvItem.SubItems.Add("Profiling");
											lvItem.SubItems.Add(GetProfilingType(process.Id));
											goto A;

										}
									}														
									
									lvItem.SubItems.Add("Running");
									lvItem.SubItems.Add(GetProfilingType(process.Id));							
								}	
								else if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED. THIS ITSELF ENSURES THAT THE MUTEX EXISTS					
								{
									if(hMutex!=IntPtr.Zero )
									{
										try
										{
											ReleaseMutex(hMutex);
											CloseHandle(hMutex);
										}
										catch{}
									}
									hMutex=IntPtr.Zero ;

									if(SharpClientForm.profilerTable.ContainsKey(process.Id))
									{																		
										lvItem.SubItems.Add("Profiling");
										lvItem.SubItems.Add(GetProfilingType(process.Id));
										goto A;										
									}			
						
									lvItem.SubItems.Add("Running");
									lvItem.SubItems.Add(GetProfilingType(process.Id));
								}
								else
								{
									continue;
								}
							}
							catch
							{
								lvItem.SubItems.Add("N/A");		
								lvItem.SubItems.Add(GetProfilingType(process.Id));
							}

						A:										
							profileeView.Items.Add(lvItem);	
							
							if(Convert.ToInt32(lvItem.Text)==pidProfilee)
							{
								lvItem.Selected =true;
							}				
					
						}

						#endregion
					}
					else
					{
						#region profilee enumerate System
						if(delegatorObj==null)
						{
							throw new Exception("Unable to get profilee processes."); 
						}
						string computerName="localhost";
						try{computerName=SystemInformation.ComputerName;}
						catch{}
						string profileeStatusString=delegatorObj.RefreshProfileeStatus(computerName); 								

						foreach( string profileeString in profileeStatusString.Split(new char[]{'\n'},512))
						{	
							#region foreach
							if(profileeString.Length<8)
								continue;

							string[] profileeSubString = profileeString.Substring(7).Split(new char[]{'?'},3 ) ;
							ListViewItem lvItem=null;
							if(profileeSubString.Length>2)
							{
								lvItem=new ListViewItem(profileeSubString[0]);
								lvItem.SubItems.Add(profileeSubString[2]);

								//																foreach (Form frm in this.Owner.MdiChildren)  
								//																{
								//																	if(frm.Text.Split(new Char[] {':'},2)[0].Trim()==profileeSubString[0])
								//																	{										
								//																		lvItem.SubItems.Add("Profiling");
								//																		goto B;
								//																	}											  
								//																}
								int pID=0;
								try
								{
									pID=Convert.ToInt32 (profileeSubString[0]);
								}	
								catch{}								

								if(SharpClientForm.profilerTable.ContainsKey(pID))
								{
									if(Convert.ToString(SharpClientForm.profilerTable[pID]).ToUpper()== profileeSubString[2].ToUpper() ) 
									{
										lvItem.SubItems.Add("Profiling");										
										goto B;

									}
								}
								if(profileeSubString[1]=="FALSE")
								{
									lvItem.SubItems.Add("Running");
								}
								else
								{
									lvItem.SubItems.Add("Suspended");
								}					
							}
							else if (profileeSubString.Length==2)
							{
								lvItem=new ListViewItem(profileeSubString[0]);
								if(profileeSubString[1]=="FALSE" ||profileeSubString[1]=="TRUE")
								{	
									lvItem.SubItems.Add("N/A");									
									int pID=0;
									try
									{
										pID=Convert.ToInt32 (profileeSubString[0]);
									}	
									catch{}								

									if(SharpClientForm.profilerTable.ContainsKey(pID))
									{
										if(Convert.ToString(SharpClientForm.profilerTable[pID]).ToUpper().Trim() == "N/A" ) 
										{
											lvItem.SubItems.Add("Profiling");										
											goto B;

										}
									}
									if(profileeSubString[1]=="FALSE")
									{
										lvItem.SubItems.Add("Running");
									}
									else
									{
										lvItem.SubItems.Add("Suspended");
									}			
								}										
							}
							else if(profileeSubString.Length==1)
							{
								lvItem=new ListViewItem(profileeSubString[0]);
								lvItem.SubItems.Add("N/A");
								lvItem.SubItems.Add("N/A");

								int pID=0;
								try
								{
									pID=Convert.ToInt32 (profileeSubString[0]);
								}	
								catch{}								

								if(SharpClientForm.profilerTable.ContainsKey(pID))
								{
									if(Convert.ToString(SharpClientForm.profilerTable[pID]).ToUpper().Trim() =="N/A" ) 
									{
										lvItem.SubItems.Add("Profiling");										
										goto B;

									}
								}
							}
						B:	if(lvItem!=null)
							{
								int pID=0;
								try
								{
									pID=Convert.ToInt32 (profileeSubString[0]);
								}	
								catch{}				
								lvItem.SubItems.Add(GetProfilingType(pID));
								profileeView.Items.Add(lvItem);	
								if(Convert.ToInt32(lvItem.Text)==pidProfilee)
								{
									lvItem.Selected =true;
								}
							}
							#endregion
						}

						#endregion

					}
				}
				catch(Exception ex)
				{
					if(bInteractive)
					{	
						bInteractive=false;
						if(this.Cursor==Cursors.WaitCursor)
						{
							this.Cursor =Cursors.Arrow ;
							groupBox10.Enabled=true;  
							profileeView.Enabled =true;
							Application.DoEvents();  
						}
						System.Threading.Thread.Sleep(200) ;
						
						continue;
					}
					else
					{
						if(this.Cursor==Cursors.WaitCursor)
						{
							this.Cursor =Cursors.Arrow ;							
							profileeView.Enabled =false;
						}
						groupBox10.Enabled=true;  
						Application.DoEvents();  
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						
						break;
					}
				}

				if(this.Cursor==Cursors.WaitCursor)
				{
					this.Cursor =Cursors.Arrow ;					
					profileeView.Enabled =true;
				}
				groupBox10.Enabled=true;  				
				Application.DoEvents();
				System.Threading.Thread.Sleep(1700);  
			}
			groupBox10.Enabled=true; 
		}

		private string GetProfilingType(int pid)
		{
			try
			{				
				
				if(IsProcessOA(pid))
				{
					return "Memory Profiling";
				}
				else if (IsProcessInproc(pid))
				{
					return "Performance Profiling";
				}				
				else if(IsProcessExceptionTraced(pid))
				{
					return "Exception Tracing";
				}	
				else
				{
#warning "Note the extra space at the end of 'Memory Profiling' "
					return "Memory Profiling ";//Note the extra space
				}
			}
			catch{}
			return "N/A";
		}

		private void tabConfig_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			selectNewTab();
		}

		private void ChangeLabelColor()
		{
			for (int i=0;i<5;i++)
			{
				label5.ForeColor=Color.Red;				 
				Application.DoEvents();
				System.Threading.Thread.Sleep(500);
				label5.ForeColor= Color.FromKnownColor(System.Drawing.KnownColor.HotTrack);				
				Application.DoEvents();		
				System.Threading.Thread.Sleep(500);
			}
		}

		private void selectNewTab()
		{
			if (tabConfig.SelectedTab == profileeTab)
			{
				acceptSettings.Text = "Accept";
				try
				{
					profileeView.Focus();
				}
				catch { }
				try
				{
					if(IsDesktopApp==false && label5.Visible==true && label5.ForeColor==Color.Red)
					{
						System.Threading.Thread threadColor=new System.Threading.Thread(new System.Threading.ThreadStart(ChangeLabelColor));
						threadColor.Start(); 
					}
				}catch{}
				Application.DoEvents();

				//Very important to profile a process from suspended state when object allocation profiling is active
				if (configString == "MEMORY_ANALYSIS" && IsDesktopApp == false)
				{
					if (objRuntimeObjectAllocation.Checked)
					{
						radioProfileeSuspended.Checked = true;
						profileeAlreadyRunning.Checked = false;
						profileeAlreadyRunning.Enabled = false;
					}
					else
					{
						profileeAlreadyRunning.Checked = true;
						profileeAlreadyRunning.Enabled = true;
						radioProfileeSuspended.Checked = false;
					}
					Application.DoEvents();
				}

				try
				{
					AcceptData();//V. Imp.
				}
				catch { }

				if (profileeThread == null)
				{
					if (IsDesktopApp == false)//IsDesktopApp is Only Required here
					{
						#region Spawn the thread

						this.Cursor = Cursors.WaitCursor;
						Application.DoEvents();
						try
						{
							if (System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
							{
								throw new COMException(null, -2147023838);
							}

							if (delegatorObj == null)
							{
								delegatorObj = new SharpDelegatorClass();
								try
								{
									delegatorObj.profilerName = Application.StartupPath + @"\SoftProdigy.Core.dll";
								}
								catch (COMException _com_exc)
								{
									if (_com_exc.ErrorCode != -2147023649)
									{
										throw new COMException(_com_exc.Message, _com_exc.ErrorCode);
									}
								}
							}
						}
						catch (Exception exc)
						{
							int hr = System.Runtime.InteropServices.Marshal.GetHRForException(exc);
							if (hr == -2147023838 || hr == -2147221164)//SERVICE NOT AVAILABLE
							{
								SharpClientForm.scInstance.psStatusBar.Panels[0].Text = @"Profiling will be available only for desktop applications started by the profiler";
							}
							else
							{
								MessageBox.Show("Error initializing profiler settings.\n" + exc.Message, "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							this.Cursor = Cursors.Arrow;
							profileeView.Enabled = false;
							groupBox10.Enabled = true;
							radioProfileeSuspended.Enabled = false;
							profileeAlreadyRunning.Enabled = false;
							return;
						}

					Z: profileeThread = new System.Threading.Thread(new System.Threading.ThreadStart(RefreshProcessView));
						//profileeThread.ApartmentState =System.Threading.ApartmentState.MTA;   
						profileeView.Enabled = false;
						groupBox10.Enabled = false;
						profileeThread.Start();
						#endregion
					}
				}
				else
				{
					groupBox10.Enabled = true;
					if (profileeThread != null)
					{
						if ((profileeView.Visible == true) && (profileeThread.ThreadState == System.Threading.ThreadState.Suspended || (int)profileeThread.ThreadState == 96))   //96 is WaitSleepjoin and SuspendRequested
						{
							try
							{
								profileeThread.Resume();
							}
							catch { }
						}
					}

				}

			}
			else
			{
				acceptSettings.Text = "Next>>";
				if (profileeThread != null)
				{
					if (profileeThread.ThreadState == System.Threading.ThreadState.Running || profileeThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
					{
						try
						{
							profileeThread.Suspend();
						}
						catch { }
					}
				}

			}		
		}

		private void Configurator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(configString=="READONLY")
			{
				return;
			}

			this.Enabled=false;
			Application.DoEvents(); 
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

			if(delegatorObj!=null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(delegatorObj);						
			}

			delegatorObj =null;				
			//if(!IsDesktopApp)
			{
				if(defaultAttach.Checked)
				{
					SuspendKey=true;
					AutoAttach=true;

					try
					{
						if(functionDataObj!=null)
						{
							g_FunctionFlag=functionDataObj.m_iFunctionFlag;  
							g_ObjectFlag=0;
						}
						else if(objectDataObj!=null)
						{
							g_ObjectFlag=objectDataObj.m_iObjectsFlag;
							g_FunctionFlag=0;
						}
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
						RestoreDefaultConfiguration();
					}					
					//Suspend and carry on with the settings									
				}
				else
				{
					g_ObjectFlag=0;
					g_FunctionFlag=0;					
					UninstallProfilingEnvironment(false);//RestoreDefaultConfiguration called automatically
				}
			}
//			else
//			{
//				g_ObjectFlag=0;
//				g_FunctionFlag=0;
//				RestoreDefaultConfiguration();				
//			}
		}

		private void profileeView_VisibleChanged(object sender, System.EventArgs e)
		{
			if(profileeThread!=null)
			{
				if(profileeView.Visible==false)
				{
					if(profileeThread.ThreadState==System.Threading.ThreadState.Running ||profileeThread.ThreadState==System.Threading.ThreadState.WaitSleepJoin )
					{						
						profileeThread.Suspend(); 
					}
				}
				else
				{
					if(profileeThread.ThreadState==System.Threading.ThreadState.Suspended || (int)profileeThread.ThreadState == 96 )   //96 is WaitSleepjoin and SuspendRequested
					{	
						try
						{
							profileeThread.Resume(); 
						}
						catch{}
						
					}	

				}					
						
			}
			profileeView.Focus();
			Application.DoEvents();  		
		}

		private void browseProfileeApp_Click(object sender, System.EventArgs e)
		{
						
			if(openProfileeDialog.ShowDialog(this)==DialogResult.OK)
			{
				ProfileeApp.Text =openProfileeDialog.FileName;   
				ProfileeAppFolder.Text = ProfileeApp.Text.Substring(0,ProfileeApp.Text.LastIndexOf("\\"));
					
			}
		}

		private void profileeInteractive_CheckedChanged(object sender, System.EventArgs e)
		{
			if( ((RadioButton)sender).Checked==true  )
			{				
				if(newProfileeGroupBox.Visible==false)
				{
					profileeView.Visible =false;
					//psPanelProfileeSelection.PanelState =PSPanelState.Collapsed ;
					System.Threading.Thread.Sleep(400);   
					Application.DoEvents();

					newProfileeGroupBox.Visible =true;
					//psPanelProfileeSelection.PanelState =PSPanelState.Expanded  ;
				}
				Application.DoEvents();						
 
			}		
			
		}

		
		private void profileeAlreadyRunning_CheckedChanged(object sender, System.EventArgs e)
		{
			if( ((RadioButton)sender).Checked==true  )
			{
				if(profileeView.Visible==false)
				{
					newProfileeGroupBox.Visible =false;
					//psPanelProfileeSelection.PanelState =PSPanelState.Collapsed ;
					System.Threading.Thread.Sleep(400);   
					Application.DoEvents();

					profileeView.Visible=true;
					//psPanelProfileeSelection.PanelState =PSPanelState.Expanded  ;	
				}

				//////
				///Special case
				if(((RadioButton)sender).Name.ToUpper()  == "RADIOPROFILEESUSPENDED")
				{					
//					if(DialogResult.Cancel==MessageBox.Show("Suspend all new .NET processes for profiling?\n[See help for more information on this topic]","Suspend!" ,MessageBoxButtons.OKCancel,MessageBoxIcon.Question))  
//					{
//						profileeAlreadyRunning.Checked =true;
//						Application.DoEvents();
//						return;
//					}
//					else
					{
						if(delegatorObj!=null)
						{
							AcceptData(); 
						}
					}
				}
				else if (((RadioButton)sender).Name.ToUpper()  == "PROFILEEALREADYRUNNING")
				{
					if(delegatorObj!=null)
					{
						AcceptData(); 
					}
				}
				Application.DoEvents();	
			}	
			

		}

		private void profileeView_Leave(object sender, System.EventArgs e)
		{
			profileeView.Focus();
			Application.DoEvents();  
		}


		public void CloseConfigurator()
		{			
			try
			{
				this.Parent.Controls.Remove(this);    
			}
			catch{}
			try
			{
				this.ParentForm.Close();				
			}
			catch{}
			
		}

		private void acceptSettings_Click(object sender, System.EventArgs e)
		{			
			if(tabConfig.SelectedTab  != profileeTab)
			{
				tabConfig.SelectedTab=profileeTab;
				Application.DoEvents(); 
				return;
			}

			if(profileeInteractive.Checked  || profileeInteractiveSuspended.Checked)
			{
				#region Interactive process profiling

				if(configString =="MEMORY_ANALYSIS" && profileeInteractive.Checked  && objRuntimeObjectAllocation.Checked  )
				{
					//if(DialogResult.Yes  ==MessageBox.Show("To memory profile a process for object-allocations [see 'Memory Analysis Settings - Advance Options'],\nit is advised that you start profiling the process immediately and not later when the process has started.\nDo you want to start profiling the process immediately?","Memory Analysis",MessageBoxButtons.YesNo,MessageBoxIcon.Question))
					{
						profileeInteractiveSuspended.Checked=true; 
					}						
				}
				else
				{
					if(DialogResult.Yes  ==MessageBox.Show("Do you want to profile the process immediately?\nYou can also choose to profile it later when the process has started.","Start Profiling Now? ",MessageBoxButtons.YesNo,MessageBoxIcon.Question))
					{
						profileeInteractiveSuspended.Checked=true; 
					} 
					
				}

				Application.DoEvents();  
				try
				{
					AcceptData();
					UpdateEnvironmentAndStartProcess();				
				}
				finally
				{
					CloseConfigurator();					
				}
				return;
				#endregion 
			}

			else if (profileeAlreadyRunning.Checked || radioProfileeSuspended.Checked )
			{
				if(delegatorObj==null) 
				{
					MessageBox.Show("Profilee process information is incomplete","Error!",MessageBoxButtons.OK,MessageBoxIcon.Information ); 
					CloseConfigurator();
					return;
				}
				#region Already running process profiling

				
				if(configString =="MEMORY_ANALYSIS" && objRuntimeObjectAllocation.Checked)
				{	
  
					if(profileeView.SelectedItems.Count>0)
					{
						int pid=0;						
						try
						{
							pid=Convert.ToInt32(profileeView.SelectedItems[0].Text );							
						}
						catch{}
						if(pid!=0 )
						{	
							if(IsProcessOA(pid))
							{
								//do nothing
							}	
							else
							{
								MessageBox.Show("The process is not configured to be Memory Profiled for object-allocations\n[see 'Memory Analysis Settings -> Advance Options'].\nIt is advised that you restart the process WITHOUT CLOSING THIS DIALOG and then retry to Memory Profile it.","Memory Analysis",MessageBoxButtons.OK ,MessageBoxIcon.Stop); 								
								return;
							}
						}
					
					}
				}

				Application.DoEvents();  

				string pName,pStatus;
				pName="";
				pStatus ="";
				int pID=0;
				
				/////////////////
				///

				try
				{
					AcceptData();//first of all
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						 
					CloseConfigurator();
					return;
				}
				
				//////////////
				///
				if(profileeView.Visible==false)
				{
					try
					{
						profileeTab.Select();
						tabConfig.SelectedTab=profileeTab;
					}
					catch{}
					Application.DoEvents();
					if(profileeView.Visible==false)
					{
						MessageBox.Show("No process selected!\nSelect a process to profile","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						
						return;
					}
				}

				if(profileeView.SelectedItems.Count>0)
				{
					try
					{					
						pID=Convert.ToInt32(profileeView.SelectedItems[0].Text )  ;
						pName=profileeView.SelectedItems[0].SubItems[1].Text.Trim() ;
						pStatus=profileeView.SelectedItems[0].SubItems[2].Text.Trim().ToUpper();
						if(pStatus=="PROFILING")
						{
							throw new Exception("The process "+pName+" is already being profiled.\nYou may need to stop the process, choose a profiling type and then restart the process to profile it again."); 
						}						
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						 
						return;
					}
    
				}
				else
				{
					MessageBox.Show("No process selected!\nSelect a process to profile","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);					
					return;
				}

				bool bIsSuspended=(pStatus=="SUSPENDED")?true:false;
				////////////////
				///
				int intFFlag=0;
				int intOFlag=0;
				if(configString=="PERFORMANCE_ANALYSIS")
				{
					try
					{
						if(functionDataObj!=null )
							intFFlag=functionDataObj.m_iFunctionFlag;							
						if(IsProcessOA(pID))
						{
							if(m_bExceptionTracing)
							{
								throw new Exception ("The process is not configured to be profiled for Exception-Tracing as it was initially started with Memory Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Exception-Trace it.");
							}
							else
							{
								throw new Exception ("The process is not configured to be Performance Profiled as it was initially started with Memory Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Performance Profile it.");
							}
						}
						bool b=IsProcessInproc(pID);
						if( b &&  funcRuntimeExceptions.Checked )  
						{							
							throw new Exception ("The process is not configured to be profiled for Exception-Tracing as it was initially started with Performance Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Exception-Trace it.");
						}
						if(!b)
						{
							if(funcRuntimeCodeView.Checked)
							{
								if(IsProcessExceptionTraced(pID))
								{
									throw new Exception ("The process is not configured to be Performance Profiled as it was initially started with Exception-Tracing.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Performance Profile it.");
								}
								else
								{
									throw new Exception ("The process is not configured to be Performance Profiled as it was initially started with Memory Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Performance Profile it.");
								}
							}
							else if(funcRuntimeExceptions.Checked)
							{
								if(IsProcessExceptionTraced(pID))
								{
									//Continue;
								}
								else//means the process is basic memory profiled
								{
									throw new Exception ("The process is not configured to be profiled for Exception-Tracing as it was initially started with Memory Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Exception-Trace it.");
								}
							}
						}
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						
						return;
					}					
				}
				else if (configString=="MEMORY_ANALYSIS")
				{
					try
					{
						if(objectDataObj!=null) 
							intOFlag=objectDataObj.m_iObjectsFlag;	
						if(IsProcessInproc(pID) )
						{
							throw new Exception ("The process is not configured to be Memory Profiled as it was initially started with Performance Profiling.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Memory Profile it.");
						}	
						if(IsProcessExceptionTraced(pID))
						{
							throw new Exception ("The process is not configured to be Memory Profiled as it was initially started with Exception-Tracing.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Memory Profile it.");
						}
						bool b=IsProcessOA(pID);
						if( !b &&  objRuntimeObjectAllocation.Checked  )  
						{
							throw new Exception ("The process is not configured to be Memory Profiled.\nPlease restart the process WITHOUT CLOSING THIS DIALOG and then retry to Memory Profile it.");
						}						
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);						
						return;
					}					
				}
				else
				{
					return;
				}				

				HandOverProcess(pID ,pName,bIsSuspended,intFFlag ,intOFlag ,configString);	
				CloseConfigurator();
				return;
				#endregion 

			}		
			

		}

		private void AcceptData()
		{
			if(configString=="MEMORY_ANALYSIS")
			{
				AcceptMemoryAndProfileeData();
				objectDataObj.UpdateRegistry();  
			}
			else if(configString=="PERFORMANCE_ANALYSIS")
			{
				AcceptPerformanceAndProfileeData();
				functionDataObj.UpdateRegistry();  
			}
//			else if(configString=="BOTH")
//			{
//				AcceptAllData();
//				functionDataObj.UpdateRegistry();  
//				objectDataObj.UpdateRegistry();
//			}

		}

		private void AcceptPerformanceAndProfileeData()
		{
			AcceptPerformanceData();
			AcceptProfileeData();

		}
		private void AcceptMemoryAndProfileeData()
		{
			AcceptMemoryData();
			AcceptProfileeData();

		}

		private void AcceptAllData()
		{
			AcceptMemoryData();
			AcceptPerformanceData();
			AcceptProfileeData();
		}

		private void AcceptMemoryData()
		{
			//Class Filter
			if(objClassFilter.Items.Count>0)
			{
				string classFilterString="";
				
				foreach(object data in objClassFilter.Items)
				{
					if(Convert.ToString(data).Length>0)
					{
						classFilterString+=Convert.ToString(data);
						classFilterString+="|";
					}
				}
				objectDataObj.m_strObjectClassFilter=classFilterString;  

			}
			else
			{
				objectDataObj.m_strObjectClassFilter="";
			}
			//Passthrough flag

			if(radioMemObjPassthrough.Checked)
			{
				objectDataObj.m_bObjectClassPassthrough=true; 
			}
			else
			{
				objectDataObj.m_bObjectClassPassthrough=false;
			}


			///////////////Parse Flag
			///			
			int objectFlag=((int)(OBJECT_FILTER.OF_OBJECT_NAME_ONLY)) ; //initialize

			if(objRuntimeObjectCount.Checked)
			{
				objectFlag|=((int)(OBJECT_FILTER.OF_OBJECT_COUNT)) ;
			}
			if(objRuntimeObjectSize.Checked)
			{
				objectFlag|=((int)(OBJECT_FILTER.OF_OBJECT_SIZE)) ;

			}
			if(objRuntimeReferencedObjectData.Checked) 
			{
				objectFlag|=((int)(OBJECT_FILTER.OF_REFERENCED_OBJECTS)) ;

			}			
#warning "objRuntimeObjectAllocation is the object-alloc control's name"

			try
			{
				if(objRuntimeObjectAllocation.Checked) 
				{
					objectFlag|=((int)(OBJECT_FILTER.OF_OBJECT_ALLOCATION)) ;
					IsObjectAllocationMonitored=true;
				}	
				else
				{
					IsObjectAllocationMonitored=false;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error setting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  		
				return;
			}

			if(objRuntimeSrcCodeAnalysis.Checked) 
			{
				objectFlag|=((int)(OBJECT_FILTER.OF_OBJECT_SRC_ANALYSIS_ONLY)) ;
			}			



			objectDataObj.m_iObjectsFlag =objectFlag; 
		
			////////////GC
			///
//			if(objGCBeforeObjectCollection.Checked)
//			{
//				objectDataObj.m_bGCBeforeOC =true; 
//			}
//			else
//			{
//				objectDataObj.m_bGCBeforeOC =false; 
//			}			

			objectDataObj.m_bGCBeforeOC =true; 
		}

		private void AcceptPerformanceData()
		{
			//Class Filter
			if(funcClassFilter.Items.Count>0)
			{	
				string classFilterString="";
				
				foreach(object data in funcClassFilter.Items)
				{
					if(Convert.ToString(data).Length>0) 				
					{
						classFilterString+=Convert.ToString(data);
						classFilterString+="|"; 
					}
				}
				functionDataObj.m_strFunctionClassFilter=classFilterString;							
			}
			else
			{
				functionDataObj.m_strFunctionClassFilter="";
			}

			///////////Class Passthrough Flag
			
			if(radioFuncObjPassthrough.Checked)
			{
				functionDataObj.m_bFunctionClassPassthrough=true; 
			}
			else
			{
				functionDataObj.m_bFunctionClassPassthrough=false;
			}
			////////////////////
			///
			//Module Filter
			if(funcModuleFilter.Items.Count>0)
			{	
				string moduleFilterString="";
				
				foreach(object data in funcModuleFilter.Items)
				{
					if(Convert.ToString(data).Length>0) 
					{
						moduleFilterString+=Convert.ToString(data);
						moduleFilterString+="|"; 
					}
				}
				functionDataObj.m_strFunctionModuleFilter =	moduleFilterString;					
			}
			else
			{
				functionDataObj.m_strFunctionModuleFilter =	"";
			}

			///////////Module Passthrough Flag
			
			if(radioPerfModulePassthrough.Checked)
			{
				functionDataObj.m_bFunctionModulePassthrough=true; 
			}
			else
			{
				functionDataObj.m_bFunctionModulePassthrough=false;
			}
			////////////////////
			///
			//Parse filter Flag

			int functionFlag=((int)(FUNCTION_FILTER.FF_FUNCTION_NAME_ONLY));//initialize
			if(funcRuntimeFuncSignature.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_SIGNATURE));  
			}
			if(funcRuntimeModuleName.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_MODULE)); 
			}

			if(funcRuntimeThreadID.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_THREAD_ID)); 
			}

			if(funcRuntimeCodeView.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_CODE_VIEW)); 
			}

			if(funcRuntimeNumberOfCalls.Checked) 
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_NUMBER_OF_CALLS_ONLY)); 
			}

			if(funcRuntimeCallTimes.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_TOTAL_CPU_TIME)); 
				switch(funcRuntimeTimeResolution.SelectedIndex)
				{
					case  0:
					{
						functionDataObj.m_iTRFlag=((int)(TIME_RESOLUTION.TR_CPU_CYCLES)) ;  
						break;
					}
					case 1:
					{
						functionDataObj.m_iTRFlag=((int)(TIME_RESOLUTION.TR_HIGH_RESOLUTION)) ;  
						break;
					}
					case 2:
					{
						functionDataObj.m_iTRFlag=((int)(TIME_RESOLUTION.TR_LOW_RESOLUTION)) ;  
						break;
					}

				}
			}

			if(funcRuntimeExceptions.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS)); 
				if(funcRuntimeExceptionStackTrace.Checked)
				{
					functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_EXCEPTIONS_STACKTRACE)); 
				}
			}

			if(funcRuntimeManagedOnly.Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_MANAGED_ONLY)); 
			}

			if(!calleeFunctionTree.Nodes[0].Checked)
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_NO_FUNCTION_CALLEE_INFORMATION));
			}
			else
			{
				functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_CALLEE_ID));
				if(calleeFunctionTree.Nodes[0].Nodes[0].Checked)
				{
					functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY));				
				}
				if(calleeFunctionTree.Nodes[0].Nodes[1].Checked)
				{
					functionFlag |=((int)(FUNCTION_FILTER.FF_FUNCTION_CALLEE_TOTAL_CPU_TIME));
				
				}

			}
			/////////////Function filter Flag parsed
			///
			functionDataObj.m_iFunctionFlag =functionFlag ; 

		}
		public void AcceptProfileeData()
		{
			//Suspend ??
			RegistryKey keySuspend;
			try
			{
				keySuspend=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error getting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  				
				return;
			}			
			try
			{
				keySuspend.SetValue("SuspendOnStart",(radioProfileeSuspended.Checked || profileeInteractiveSuspended.Checked )?1:0);							
			}
			catch(Exception exc)
			{
				MessageBox.Show("Error setting configuration values:\n"+exc.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  		
				try
				{
					keySuspend.Close();  
				}
				catch{}
				return;
			}

			try
			{
				if(configString=="PERFORMANCE_ANALYSIS")
				{
					if(funcRuntimeCodeView.Checked)  
					{
						IsInproc=true; 	
					}
					else
					{
						IsInproc=false;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error setting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  		
			}

			try
			{
				if(configString=="MEMORY_ANALYSIS")
				{
					if(objRuntimeObjectAllocation.Checked)  
					{
						IsObjectAllocationMonitored=true; 						
					}
					else
					{
						IsObjectAllocationMonitored=false;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error setting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  		
			}


		}

		public static bool Suspended
		{
			get
			{
				RegistryKey keySuspend;
				try
				{
					keySuspend=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
				}
				catch
				{
					return true;
				}			
				bool isSuspended=true;
				try
				{
					isSuspended=Convert.ToInt32(keySuspend.GetValue("SuspendOnStart"))==0?false:true;
				}
				catch{}
				try
				{
					keySuspend.Close();  
				}
				catch{}			
				return isSuspended;
			}			
		}

		public static bool SuspendKey
		{
			set
			{
				RegistryKey keySuspend=null;
				try
				{								
					keySuspend=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);
					int bSuspended=	(value==true)?1:0;			
					keySuspend.SetValue("SuspendOnStart",bSuspended);
					keySuspend.Close();  
				}
				catch(Exception ex)
				{
					MessageBox.Show("The profiler was unable to restore CLR settings for the system.\nAll .NET processes may be rendered in suspeneded state.\nRead the documentation to rescue.\n"+ex.Message,"Critical Error!!!!",MessageBoxButtons.OK,MessageBoxIcon.Error);  								
				}
				if(keySuspend!=null)
				{
					try	{keySuspend.Close();}
					catch{}
				}		
			}			
		}

		public static bool AutoAttach
		{
			set
			{
				RegistryKey keyAutoAttach=null;
				try
				{								
					keyAutoAttach=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);
					int bAutoAttach=(value==true)?1:0;			
					keyAutoAttach.SetValue("AutoAttach",bAutoAttach);
					keyAutoAttach.Close();  
				}
				catch(Exception ex)
				{
					MessageBox.Show("The profiler was unable to restore CLR settings for the system.\nAll .NET processes may be rendered in suspeneded state.\nRead the documentation to rescue.\n"+ex.Message,"Critical Error!!!!",MessageBoxButtons.OK,MessageBoxIcon.Error);  								
				}
				if(keyAutoAttach!=null)
				{
					try	{keyAutoAttach.Close();}
					catch{}
				}
			}			
		}


		public static bool IsProfiling()
		{
			RegistryKey keyProfiling;
			try
			{
				keyProfiling=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
			}
			catch
			{
				return true;
			}
			bool isSuspended=true;
			try
			{
				isSuspended=Convert.ToInt32(keyProfiling.GetValue("EnvSetup"))==0?false:true;
			}
			catch{}
			try
			{
				keyProfiling.Close();  
			}
			catch{}			
			return isSuspended;
		}

		private static bool IsInproc
		{			
//			get
//			{
//				RegistryKey keyInproc;
//				try
//				{
//					keyInproc=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
//				}
//				catch
//				{
//					return true;
//				}
//				bool isInproc=true;
//				try
//				{
//					isInproc=Convert.ToInt32(keyInproc.GetValue("InprocSwitch"))==0?false:true;
//				}
//				catch{}
//				try
//				{
//					keyInproc.Close();  
//				}
//				catch{}								
//				return isInproc; 
//			}
			set
			{
				RegistryKey keyInproc;				
				keyInproc=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);				
				int iIP=(value==true)?1:0;
				keyInproc.SetValue("InprocSwitch",iIP);	
			
#warning "Excetion Tracing is set here"				
				keyInproc.SetValue("RunGCBeforeObjectCollection",0);//V. Imp
				keyInproc.Close();  
			}
		}


		private static bool IsObjectAllocationMonitored
		{			
//			get
//			{
//				RegistryKey keyObjAlloc=null;
//				bool isObjAlloc=true;
//				try
//				{
//					keyObjAlloc=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\");
//					isObjAlloc=Convert.ToInt32(keyObjAlloc.GetValue("IsObjectAllocationMonitored"))==0?false:true;
//				}
//				catch{	}				
//				finally
//				{
//					if(keyObjAlloc!=null)
//					keyObjAlloc.Close();  
//				}				
//				return isObjAlloc; 
//			}
			set
			{
				RegistryKey keyObjAlloc=null;
				int intObjAlloc=(value==true)?1:0;
				try
				{
					keyObjAlloc=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);
					keyObjAlloc.SetValue("IsObjectAllocationMonitored",intObjAlloc);					
				}
				catch{}				
				finally
				{
					if(keyObjAlloc!=null)
					{
						keyObjAlloc.Close();  
					}
				}
			}
		}

		public static bool IsProcessExceptionTraced(int pid)
		{
			IntPtr hMutex=System.IntPtr.Zero;													
			if( ( hMutex=OpenMutex(1,0,"Global\\"+Convert.ToString(pid)+"?EXCEPTIONS"))!= System.IntPtr.Zero)
			{
				try{ReleaseMutex(hMutex);}
				catch{}
				try{CloseHandle(hMutex);}
				catch{}
				hMutex=System.IntPtr.Zero;
				return true;
			}
			else if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED.
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		public static bool IsProcessInproc(int pid)
		{
			IntPtr hMutex=System.IntPtr.Zero;													
			if( ( hMutex=OpenMutex(1,0,"Global\\"+Convert.ToString(pid)+"?INPROC"))!= System.IntPtr.Zero)
			{
				try{ReleaseMutex(hMutex);}
				catch{}
				try{CloseHandle(hMutex);}
				catch{}
				hMutex=System.IntPtr.Zero;
				return true;
			}
			else if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED.
			{
				return true;
			}
			else
			{
				return false;
			}

		}		

		public static bool IsProcessOA(int pid)
		{
			IntPtr hMutex=System.IntPtr.Zero;													
			if( ( hMutex=OpenMutex(1,0,"Global\\"+Convert.ToString(pid)+"?OA"))!= System.IntPtr.Zero)
			{
				try{ReleaseMutex(hMutex);}
				catch{}
				try{CloseHandle(hMutex);}
				catch{}
				hMutex=System.IntPtr.Zero;
				return true;
			}
			else if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)//ACCESS DENIED.
			{
				return true;
			}
			else
			{
				return false;
			}

		}		

		private int UpdateEnvironmentAndStartProcess()
		{
			if(newProfileeGroupBox.Visible==false)
			{
				try
				{
					profileeTab.Select();
					tabConfig.SelectedTab=profileeTab;
				}
				catch{}
				Application.DoEvents();
				if(newProfileeGroupBox.Visible==false)
				{
					MessageBox.Show("Invalid process!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  				
					return 0;
				}
			}
			else
			{
				try
				{
					UpdateEnvironment();
				}				
				catch(Exception ex)
				{
					MessageBox.Show("Error starting process.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  				
					return 0;
				}
			}

			PROCESS_INFORMATION pInfo= new PROCESS_INFORMATION();
			try
			{				
				const uint NORMAL_PRIORITY_CLASS = 0x0020;
				bool retValue;
				string CommandLine = " \""+ProfileeCmdlineParam.Text+"\"" ; 					
				
				STARTUPINFO sInfo = new STARTUPINFO();
				SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
				SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
				pSec.nLength = Marshal.SizeOf(pSec);
				tSec.nLength = Marshal.SizeOf(tSec);

				retValue = CreateProcess(ProfileeApp.Text ,CommandLine,
					ref pSec,ref tSec,true,NORMAL_PRIORITY_CLASS,
					IntPtr.Zero,ProfileeAppFolder.Text,ref sInfo,out pInfo);

				if(retValue==false)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); 				
				}			
				
				if(pInfo.dwProcessId!=0)
				{
				
					bool bIsSuspended=Suspended;
					////////////////
					///
					int intFFlag=0;
					int intOFlag=0;
					if(configString=="PERFORMANCE_ANALYSIS")
					{
						if(functionDataObj!=null )
						{
							intFFlag=functionDataObj.m_iFunctionFlag;
							intOFlag=0;							
						}
				
					}
					else if (configString=="MEMORY_ANALYSIS")
					{
						if(objectDataObj!=null) 
						{
							intOFlag=objectDataObj.m_iObjectsFlag;
							intFFlag=0;							
						}							
					}
					else
					{						
						if(functionDataObj!=null )
							intFFlag=functionDataObj.m_iFunctionFlag;
						if(objectDataObj!=null) 
							intOFlag=objectDataObj.m_iObjectsFlag;
					}
					
					string processName=ProfileeApp.Text;
					string exeName=processName.Substring(processName.LastIndexOf("\\")+1,processName.Length-processName.LastIndexOf("\\")-1);
					GetProfilingType(pInfo.dwProcessId); 
					HandOverProcess(pInfo.dwProcessId,exeName,bIsSuspended,intFFlag ,intOFlag ,configString);

					return pInfo.dwProcessId;
				}
				else
				{
					throw new Exception("Try again with valid process information.");
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error starting process.\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  												
			}
			finally
			{
				if(pInfo.dwProcessId!=0)
				{
					try
					{
						CloseHandle(pInfo.hThread);
					}
					catch{}
					try
					{
						CloseHandle(pInfo.hProcess);					
					}
					catch{}
				}
			}
			
			
			return 0;//should not be here
		}

		private void HandOverProcess(int PID,string processName,bool bIsSuspended,int intFFlag,int intOFlag,string configString)
		{
			ProfilerControl perfControl=new ProfilerControl(PID,processName,bIsSuspended,intFFlag ,intOFlag ,configString,this);					
			perfControl.Text =Convert.ToString(PID)+ ":"+ processName;	//V.Imp.
			perfControl.Show(); 
			Application.DoEvents(); 			
		}




		private void UpdateEnvironment()
		{
			if(SetEnvironmentVariable("COR_PROFILER","{383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D}")==0)
			{
				//use Runtime.InteropServices.Marshal.GetHRForLastWin32Error();
				throw new Exception("Environment setup failure!");				
			}
			if(SetEnvironmentVariable("COR_ENABLE_PROFILING","0x1")==0)
			{
				//use Runtime.InteropServices.Marshal.GetHRForLastWin32Error();
				throw new Exception("Environment setup failure!");
			}
		}

		private void funcRuntimeNumberOfCalls_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeNumberOfCalls.Checked==false)
			{
				calleeFunctionTree.ExpandAll();				
				calleeFunctionTree.Nodes[0].Nodes[0].Checked =false;
				Application.DoEvents();
   
			}
		}

		private void funcRuntimeCallTimes_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeCallTimes.Checked==false)
			{				
				calleeFunctionTree.ExpandAll();				
				calleeFunctionTree.Nodes[0].Nodes[1].Checked =false;
				Application.DoEvents();
   
			}
		
		}


		private void rejectSettings_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Abort Profiling?","Abort!",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
			{

				try
				{
					this.ParentForm.Close();  
					return;
				}
				catch{	}
				try
				{
					this.Parent.Controls.Remove(this);    
				}
				catch{}
			}
		}

		public static void RestoreDefaultConfiguration()
		{
			UncheckSuspend();			
			IsInproc=false;		
			IsObjectAllocationMonitored=false;
			AutoAttach=false;//Start with no attachment. V. Imp.
		}

		private static  void UncheckSuspend()
		{
			if(!Suspended)
				return;
			RegistryKey keySuspend=null;
			try
			{
				
				keySuspend=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);   
				keySuspend.SetValue("SuspendOnStart",0);  
				
			}
			catch(Exception ex)
			{
				MessageBox.Show("The profiler was unable to restore CLR settings for the system.\nAll .NET processes may be rendered in suspeneded state.\nRead the documentation to rescue.\n"+ex.Message,"Critical Error!!!!",MessageBoxButtons.OK,MessageBoxIcon.Error);  								
			}
			if(keySuspend!=null)
			{
				try	{keySuspend.Close();}
				catch{}
			}		
		}

		private static void pvtInstallProfilingEnvironment()
		{
			SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text="Initializing.Please wait..";
			SharpClient.SharpClientForm.scInstance.Cursor =Cursors.WaitCursor ;
			Application.DoEvents(); 			
			ProfileSharpServerLib.SharpDelegatorClass delegator=null; 
			object shellObject=null;
			object envProp=null;
			try
			{
				Configurator.ToggleProfiling(1); 
				if(!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
				{
					shellObject=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell",true));  
					envProp=shellObject.GetType().InvokeMember("Environment",System.Reflection.BindingFlags.GetProperty,null,shellObject,null);                    					
					envProp.GetType().InvokeMember("Item",System.Reflection.BindingFlags.SetProperty,null,envProp,new object[]{"COR_ENABLE_PROFILING","0x1"});     
					envProp.GetType().InvokeMember("Item",System.Reflection.BindingFlags.SetProperty,null,envProp,new object[]{"COR_PROFILER","{383227C2-1C12-4C3B-90E7-EF5BCCBEBD7D}"});     
					delegator=new ProfileSharpServerLib.SharpDelegatorClass();
					delegator.InstallServiceEnvironment(Application.StartupPath+"\\EnvSetter.dll",null );  
				}
			}
			catch(Exception ex)
			{
				if(Marshal.GetHRForException(ex)==-2147023838 || Marshal.GetHRForException(ex)==-2147221164 ||Marshal.GetHRForException(ex)==-2147024893 )
				{
					SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text="Ready to profile - Desktop applications only.";  
				}
				else
				{
					SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text="The profiling environment could not be started.";
					Application.DoEvents();
					throw ex;
				}				
				//throw ex;
			}
			finally
			{
				if(delegator!=null)
				{
					System.Runtime.InteropServices.Marshal.ReleaseComObject(delegator);
					delegator =null;
				}
				if(shellObject!=null)
				{
					Marshal.ReleaseComObject(shellObject);
					shellObject=null;
				}
				if(envProp !=null)
				{
					Marshal.ReleaseComObject(envProp);
					envProp=null;
				}
				SharpClient.SharpClientForm.scInstance.Cursor=Cursors.Arrow ;  
				if(SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text=="Initializing.Please wait.." )
				{
					SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text="Ready to profile";				
				}
			}
		}


		public static void InstallProfilingEnvironment()
		{
			 pvtInstallProfilingEnvironment();
		}


		private static void pvtUninstallProfilingEnvironment(bool bShowMsg)
		{
			ProfileSharpServerLib.SharpDelegatorClass delegator=null; 
			object shellObject=null;
			object envProp=null;	
			try
			{				
				try
				{
					Configurator.RestoreDefaultConfiguration();  //V. Imp.
				}
				catch{}				
				SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text ="Please wait.Shutting down profiling environment...";
				SharpClient.SharpClientForm.scInstance.Cursor =Cursors.WaitCursor;
				Application.DoEvents();  
				Configurator.ToggleProfiling(0);  
				if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
				{
					throw new COMException(null,-2147023838); 
				}	
				try
				{
					shellObject=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell",true));  
					envProp=shellObject.GetType().InvokeMember("Environment",System.Reflection.BindingFlags.GetProperty,null,shellObject,null);                    					
					envProp.GetType().InvokeMember("Remove",System.Reflection.BindingFlags.InvokeMethod,null,envProp,new object[]{"COR_ENABLE_PROFILING"});     
					envProp.GetType().InvokeMember("Remove",System.Reflection.BindingFlags.InvokeMethod,null,envProp,new object[]{"COR_PROFILER"});     					
				}
				catch{}
				delegator=new ProfileSharpServerLib.SharpDelegatorClass();
				delegator.InstallServiceEnvironment(Application.StartupPath+"\\EnvSetter.dll",null);  

			}		
			catch(Exception ex)
			{	
				SharpClient.SharpClientForm.scInstance.Cursor=Cursors.Arrow ; 	
				if(bShowMsg)
				{
					if(Marshal.GetHRForException(ex)==-2147023838 || Marshal.GetHRForException(ex)==-2147221164 || Marshal.GetHRForException(ex)==-2147024893 )//Service not available
					{
						SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text="Interactive Profiling Only";
					}
					else
					{
						MessageBox.Show("The profiling environment could not be restored.\nSome .NET applications may not respond.Refer documentation for recovery!.","Critical Error!",MessageBoxButtons.OK ,MessageBoxIcon.Error);  
					}
				}
				Application.DoEvents(); 
			}
			finally
			{
				SharpClient.SharpClientForm.scInstance.Cursor=Cursors.Arrow ; 
				
				if(delegator!=null)
				{
					System.Runtime.InteropServices.Marshal.ReleaseComObject(delegator);
					delegator=null;
				}
				if(shellObject!=null)
				{
					Marshal.ReleaseComObject(shellObject);
					shellObject=null;
				}
				if(envProp !=null)
				{
					Marshal.ReleaseComObject(envProp);
					envProp=null;
				}
				SharpClient.SharpClientForm.scInstance.psStatusBar.Panels[0].Text ="Ready to profile";

			}
		}

		public static void UninstallProfilingEnvironment(bool bShowMsg)
		{
			pvtUninstallProfilingEnvironment(bShowMsg);
		}

		public static void ToggleProfiling(int state)
		{
			RegistryKey keyProfiling=null;			
			try
			{
				
				keyProfiling=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);   
				keyProfiling.SetValue("EnvSetup",state);				
			}
			catch(Exception ex)
			{
				MessageBox.Show("The profiler was unable to restore CLR settings for the system.\nSome .NET processes may be not respond.Read the documentation to rescue.\n"+ex.Message,"Critical Error!!!!",MessageBoxButtons.OK,MessageBoxIcon.Error);  								
			}
			if(keyProfiling!=null)
			{
				try	{keyProfiling.Close();}
				catch{}
			}		
		}

		private void funcRuntimeCodeView_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeCodeView.Checked)
			{
				funcRuntimeExceptions.Checked=false;  
			}
		}

		private void funcRuntimeCalleeFunctions_CheckedChanged(object sender, System.EventArgs e)
		{
			if(funcRuntimeCalleeFunctions.Checked)
			{
				foreach(TreeNode node in calleeFunctionTree.Nodes )
				{
					node.Checked=true;
					foreach	(TreeNode subnode in node.Nodes)
					{
						subnode.Checked=true; 
					}
				}
			}
			else
			{
				
				foreach(TreeNode node in calleeFunctionTree.Nodes )
				{
					node.Checked=false; 
				}
					
			}	
			Application.DoEvents();  
			
		}

		private void objClassFilter_DoubleClick(object sender, System.EventArgs e)
		{
//			ComboBox senderClassFilterObject=(ComboBox)sender;
//			try
//			{					
//				senderClassFilterObject.Items.RemoveAt(senderClassFilterObject.SelectedIndex);
//				senderClassFilterObject.Refresh();  
//			}
//			catch{}
		}

		private void objRuntimeObjectAllocation_CheckedChanged(object sender, System.EventArgs e)
		{
			if(objRuntimeObjectAllocation.Checked==false )
			{
				objRuntimeSrcCodeAnalysis.Checked=false; 
			}
			Application.DoEvents(); 
		
		}

		private void objRuntimeSrcCodeAnalysis_CheckedChanged(object sender, System.EventArgs e)
		{
			if(objRuntimeSrcCodeAnalysis.Checked )
			{
				objRuntimeObjectAllocation.Checked=true; 
			}
			Application.DoEvents(); 
		
		}

		
	}

	public enum FUNCTION_FILTER
	{
		FF_FUNCTION_NAME_ONLY=1,
		FF_FUNCTION_SIGNATURE=FF_FUNCTION_NAME_ONLY | 2 ,
		FF_FUNCTION_CODE_VIEW=4,
		FF_FUNCTION_NUMBER_OF_CALLS_ONLY=8 | FF_FUNCTION_NAME_ONLY,
		FF_FUNCTION_TOTAL_CPU_TIME=16 | FF_FUNCTION_NAME_ONLY,
		FF_FUNCTION_MODULE=FF_FUNCTION_NAME_ONLY   | 32,
		FF_FUNCTION_EXCEPTIONS=FF_FUNCTION_NAME_ONLY  | 64,
		FF_FUNCTION_STATISTICS=FF_FUNCTION_NUMBER_OF_CALLS_ONLY |
			FF_FUNCTION_TOTAL_CPU_TIME ,
		FF_FUNCTION_CALLEE_ID=128 | FF_FUNCTION_NAME_ONLY, 
		FF_FUNCTION_EXCEPTIONS_STACKTRACE=256 |FF_FUNCTION_EXCEPTIONS,
		FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY= FF_FUNCTION_CALLEE_ID  | FF_FUNCTION_NUMBER_OF_CALLS_ONLY | 512,		
		FF_FUNCTION_CALLEE_TOTAL_CPU_TIME=FF_FUNCTION_CALLEE_ID  | FF_FUNCTION_TOTAL_CPU_TIME | 1024,		
		FF_FUNCTION_CALLEE_STATISTICS=FF_FUNCTION_CALLEE_NUMBER_OF_CALLS_ONLY |
			FF_FUNCTION_CALLEE_TOTAL_CPU_TIME ,
		FF_FUNCTION_THREAD_ID=FF_FUNCTION_NAME_ONLY|4096,
		FF_FUNCTION_MANAGED_ONLY=FF_FUNCTION_NAME_ONLY|8192 ,
		FF_NO_FUNCTION_CALLEE_INFORMATION=FF_FUNCTION_NAME_ONLY|16384	

	}


	public enum OBJECT_FILTER
	{
		OF_OBJECT_NAME_ONLY=1,
		OF_OBJECT_COUNT=OF_OBJECT_NAME_ONLY|2,
		OF_OBJECT_SIZE=OF_OBJECT_NAME_ONLY| 4,
		OF_REFERENCED_OBJECTS=OF_OBJECT_NAME_ONLY|8,	
		OF_OBJECT_ALLOCATION=OF_OBJECT_NAME_ONLY|16,
		OF_OBJECT_SRC_ANALYSIS_ONLY=OF_OBJECT_NAME_ONLY|32,
		OF_OBJECT_ALL_DATA=OF_OBJECT_SIZE |OF_OBJECT_COUNT
			|OF_REFERENCED_OBJECTS|OF_OBJECT_ALLOCATION
	}


	public enum TIME_RESOLUTION
	{
		TR_CPU_CYCLES=1,
		TR_HIGH_RESOLUTION=2,
		TR_LOW_RESOLUTION=4
	}

	public class FunctionData
	{
		public int m_iFunctionFlag;
		public string m_strFunctionClassFilter;
		public string m_strFunctionModuleFilter;
		public bool m_bFunctionClassPassthrough;
		public bool m_bFunctionModulePassthrough;
		public int m_iTRFlag;
		
		private RegistryKey keyFunctionData;		

		public FunctionData(int iFunctionFlag,string strFunctionClassFilter,string strFunctionModuleFilter,
			bool bFunctionClassPassthrough,bool bFunctionModulePassthrough,int iTRFlag)
		{
			m_iFunctionFlag=iFunctionFlag;
			m_strFunctionClassFilter=strFunctionClassFilter;
			m_strFunctionModuleFilter =strFunctionModuleFilter;
			m_bFunctionClassPassthrough=bFunctionClassPassthrough ;
			m_bFunctionModulePassthrough=bFunctionModulePassthrough;
			m_iTRFlag=iTRFlag;
		}

		public FunctionData(FunctionData fObject)
		{
			if(fObject!=null)
			{
				this.m_iFunctionFlag=fObject.m_iFunctionFlag;
				this.m_strFunctionClassFilter=fObject.m_strFunctionClassFilter;
				this.m_strFunctionModuleFilter =fObject.m_strFunctionModuleFilter;
				this.m_bFunctionClassPassthrough=fObject.m_bFunctionClassPassthrough ;
				this.m_bFunctionModulePassthrough=fObject.m_bFunctionModulePassthrough;
				this.m_iTRFlag=fObject.m_iTRFlag;	
			}
		}

		public void Fill(FunctionData fObject)
		{
			if(fObject!=null)
			{
				this.m_iFunctionFlag=fObject.m_iFunctionFlag;
				this.m_strFunctionClassFilter=fObject.m_strFunctionClassFilter;
				this.m_strFunctionModuleFilter =fObject.m_strFunctionModuleFilter;
				this.m_bFunctionClassPassthrough=fObject.m_bFunctionClassPassthrough ;
				this.m_bFunctionModulePassthrough=fObject.m_bFunctionModulePassthrough;
				this.m_iTRFlag=fObject.m_iTRFlag;	
			}
		}	

		public FunctionData()
		{	
			m_strFunctionClassFilter="";
			m_strFunctionModuleFilter="";
			m_bFunctionClassPassthrough =false;
			m_bFunctionModulePassthrough =false;
			m_iTRFlag=1;	
			keyFunctionData=null;
			try
			{
				keyFunctionData=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",false);   
				m_strFunctionClassFilter =Convert.ToString( keyFunctionData.GetValue("FunctionClassFilter"));				 
				m_strFunctionModuleFilter =Convert.ToString(keyFunctionData.GetValue("FunctionModuleFilter"));
				m_bFunctionClassPassthrough =(Convert.ToInt32(keyFunctionData.GetValue("FunctionClassPassthrough"))==0)?false:true;				
				m_bFunctionModulePassthrough =(Convert.ToInt32(keyFunctionData.GetValue("FunctionModulePassthrough"))==0)?false:true;
				m_iTRFlag =Convert.ToInt32(keyFunctionData.GetValue("TRFlag"));
			} 
			catch(Exception ex)
			{
				MessageBox.Show("Error getting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);  								
			}	
			finally
			{
				if(keyFunctionData!=null)
				{
					try
					{
						keyFunctionData.Close(); 					
					}
					catch{}
				}
				keyFunctionData=null;

			}

		}

		public void UpdateRegistry()
		{			
			try
			{
				keyFunctionData=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);   
				keyFunctionData.SetValue("FunctionClassFilter",m_strFunctionClassFilter);
				keyFunctionData.SetValue("FunctionModuleFilter",m_strFunctionModuleFilter);
				keyFunctionData.SetValue("FunctionClassPassthrough",m_bFunctionClassPassthrough==true?1:0);				
				keyFunctionData.SetValue("FunctionModulePassthrough",m_bFunctionModulePassthrough==true?1:0);
				keyFunctionData.SetValue("TRFlag",m_iTRFlag);	
			}
			catch(Exception ex)
			{
				MessageBox.Show("New performance analysis configuration values could not be updated:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);  									
			}	
			finally
			{
				if(keyFunctionData!=null)
				{
					try
					{
						keyFunctionData.Close(); 					
					}
					catch{}
				}
				keyFunctionData=null;

			}
		}	
		
	}

	public class ObjectData
	{
		public int m_iObjectsFlag;
		public string m_strObjectClassFilter;
		public bool m_bObjectClassPassthrough;
		public bool m_bGCBeforeOC;
		public  bool m_bObjectCollection;		

		private RegistryKey keyObjectData;	


		public ObjectData(int iObjectsFlag,string strObjectClassFilter,bool bObjectClassPassthrough,bool bGCBeforeOC)
		{ 
			m_iObjectsFlag=iObjectsFlag;
			m_strObjectClassFilter=strObjectClassFilter;
			m_bObjectClassPassthrough =bObjectClassPassthrough;
			m_bGCBeforeOC=bGCBeforeOC;	
			
		}
		public ObjectData(ObjectData oObject)
		{
			if(oObject!=null)
			{
				this.m_iObjectsFlag=oObject.m_iObjectsFlag;
				this.m_strObjectClassFilter=oObject.m_strObjectClassFilter;
				this.m_bObjectClassPassthrough =oObject.m_bObjectClassPassthrough;
				this.m_bGCBeforeOC=oObject.m_bGCBeforeOC;				
			}
		}

		public void Fill(ObjectData oObject)
		{
			if(oObject!=null)
			{
				this.m_iObjectsFlag=oObject.m_iObjectsFlag;
				this.m_strObjectClassFilter=oObject.m_strObjectClassFilter;
				this.m_bObjectClassPassthrough =oObject.m_bObjectClassPassthrough;
				this.m_bGCBeforeOC=oObject.m_bGCBeforeOC;				
			}
		}



		public ObjectData()
		{
			m_strObjectClassFilter="";
			m_bObjectClassPassthrough=false;
			m_bGCBeforeOC=true;
			keyObjectData=null;	
			
			try
			{
				keyObjectData=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",false);   
				m_strObjectClassFilter =Convert.ToString( keyObjectData.GetValue("ObjectClassFilter"));				 				
				m_bObjectClassPassthrough  =(Convert.ToInt32(keyObjectData.GetValue("ObjectPassthrough"))==0)?false:true;
				m_bGCBeforeOC =(Convert.ToInt32(keyObjectData.GetValue("RunGCBeforeObjectCollection"))==0)?false:true;				
			} 
			catch(Exception ex)
			{
				MessageBox.Show("Error getting configuration values:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);				
			}
			finally
			{
				if(keyObjectData!=null)
				{
					try
					{
						keyObjectData.Close();
					}catch{}				
				}
				keyObjectData=null;					
			}
		}

		public void UpdateRegistry()
		{			
			try
			{
				keyObjectData=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);   
				keyObjectData.SetValue("ObjectClassFilter",m_strObjectClassFilter);
				keyObjectData.SetValue("ObjectPassthrough",m_bObjectClassPassthrough==true?1:0);				
				keyObjectData.SetValue("RunGCBeforeObjectCollection",m_bGCBeforeOC==true?1:0);				
			}
			catch(Exception ex)
			{
				MessageBox.Show("New memory analysis configuration values could not be updated:\n"+ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);  									
			}	
			finally
			{
				if(keyObjectData!=null)
				{
					try
					{
						keyObjectData.Close();
					}
					catch{}				
				}
				keyObjectData=null;					
			}
				
		}
		}

		
	}
    		


