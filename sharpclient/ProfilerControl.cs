using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProfileSharpServerLib;
using System.Runtime.InteropServices ; 
using PSUI;
using Microsoft.Win32 ;
using System.Security.Cryptography ; 
using System.Text; 
using System.IO; 
namespace SharpClient
{
	/// <summary>
	/// Summary description for ProfilerControl.
	/// </summary>
	public class ProfilerControl : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private int pid;
		private string processName;
		private readonly bool bSuspended;
		private ProfileSharpServerLib.SharpDelegatorClass delegatorObj;
		private int m_functionFlag;
		private int m_objectFlag;
		private string configString;
		private System.Windows.Forms.SaveFileDialog saveSessionDlg;
		private PSUI.PSPanel psProfilePerformance;
		private System.Windows.Forms.Button Init;
		private System.Windows.Forms.Button Start;
		private System.Windows.Forms.Button Stop;
		private System.Windows.Forms.Button Clear;
		private System.Windows.Forms.Button Save;
		private PSUI.PSPanel psProfileMemory;
		private System.Windows.Forms.Button Dump;
		private System.Windows.Forms.Button Flush;
		private bool bDefer;
		private PSUI.PSPanelGroup psProfileControlPanelGroup;
		private System.Windows.Forms.Button Collect;
		private System.Windows.Forms.StatusBar profilerControlStausBar;
		private bool bIsCapturingStartedForOnce;
		private string cor="Cor";
		private System.Windows.Forms.ToolTip profilerControlToolTip;
		private bool bQuit=false;
		private string lastFile;

		[DllImport("Kernel32.dll",SetLastError=true)]
		public static extern IntPtr LoadLibraryA(string lpFileName);

		[DllImport("Kernel32.dll" ,SetLastError=true)]
		public static extern int GetExitCodeThread(
			IntPtr hThread,      
			out int lpExitCode    
			);

		[DllImport("Kernel32.dll",SetLastError=true)]
		public static extern UIntPtr GetProcAddress(IntPtr hModule,string procName);		

		[DllImport("Kernel32.dll", SetLastError=true)]
		public static extern IntPtr OpenMutex(uint dwAccess,int Inherit,string lpName);

		[DllImport("Kernel32.dll",SetLastError=true)]
		public static extern int CloseHandle(IntPtr handle);	
	
		[DllImport("Kernel32.dll",SetLastError=true)]
		public static extern int ReleaseMutex(IntPtr handle);	

		[DllImport("Kernel32.dll", SetLastError=true)]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess,IntPtr lpThreadAttributes,uint dwStackSize,UIntPtr lpStartAddress,IntPtr lpParameter,uint dwCreationFlags,out int ThreadId);

		[DllImport("Kernel32.dll", SetLastError=true)]
		private static extern int ResumeThread(IntPtr hThread);

		[DllImport("SoftProdigy.Core.dll", SetLastError=true)]
		private static extern int DllRegisterServer();
        
		private static UIntPtr BFC;
		private static UIntPtr CFC;
		private static UIntPtr COC;
		private static UIntPtr CO;
		private static UIntPtr DFD;
		private static UIntPtr DOD;
		private static UIntPtr SFC;
		private static UIntPtr IP;
		private static UIntPtr RCLR;

		#warning "Error prone Code"			
		private string dll=Application.CompanyName.Split(new char[]{' '})[0]+"."; 
		private Configurator m_cnfg;
		
		public ProfilerControl(int PID,string PName,bool suspended,int functionFlag,int objectFlag,string cnfgString,Configurator cnfg)
		{
			//
			// Required for Windows Form Designer support
			//			

			this.pid =PID;
			processName=PName;
			bSuspended=suspended ;			
			m_functionFlag =functionFlag ;
			m_objectFlag =objectFlag ;
			configString =cnfgString;
			bIsCapturingStartedForOnce=false;
			bQuit=false;
			InitializeComponent();
			this.m_cnfg=cnfg;			
			this.HandleCreated +=new EventHandler(ProfilerControl_HandleCreated);		 

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProfilerControl));
			this.psProfileControlPanelGroup = new PSUI.PSPanelGroup();
			this.psProfileMemory = new PSUI.PSPanel(280);
			this.Collect = new System.Windows.Forms.Button();
			this.Dump = new System.Windows.Forms.Button();
			this.Flush = new System.Windows.Forms.Button();
			this.psProfilePerformance = new PSUI.PSPanel(80);
			this.Start = new System.Windows.Forms.Button();
			this.Stop = new System.Windows.Forms.Button();
			this.Clear = new System.Windows.Forms.Button();
			this.Save = new System.Windows.Forms.Button();
			this.Init = new System.Windows.Forms.Button();
			this.saveSessionDlg = new System.Windows.Forms.SaveFileDialog();
			this.profilerControlStausBar = new System.Windows.Forms.StatusBar();
			this.profilerControlToolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.psProfileControlPanelGroup)).BeginInit();
			this.psProfileControlPanelGroup.SuspendLayout();
			this.psProfileMemory.SuspendLayout();
			this.psProfilePerformance.SuspendLayout();
			this.SuspendLayout();
			// 
			// psProfileControlPanelGroup
			// 
			this.psProfileControlPanelGroup.AutoScroll = true;
			this.psProfileControlPanelGroup.BackColor = System.Drawing.Color.Transparent;
			this.psProfileControlPanelGroup.Controls.Add(this.psProfileMemory);
			this.psProfileControlPanelGroup.Controls.Add(this.psProfilePerformance);
			this.psProfileControlPanelGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.psProfileControlPanelGroup.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psProfileControlPanelGroup.Location = new System.Drawing.Point(0, 0);
			this.psProfileControlPanelGroup.Name = "psProfileControlPanelGroup";
			this.psProfileControlPanelGroup.PanelGradient = ((PSUI.GradientColor)(resources.GetObject("psProfileControlPanelGroup.PanelGradient")));
			this.psProfileControlPanelGroup.Size = new System.Drawing.Size(282, 167);
			this.psProfileControlPanelGroup.TabIndex = 0;
			// 
			// psProfileMemory
			// 
			this.psProfileMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psProfileMemory.AnimationRate = 0;
			this.psProfileMemory.BackColor = System.Drawing.Color.Transparent;
			this.psProfileMemory.Caption = "Profile Memory";
			this.psProfileMemory.CaptionCornerType = PSUI.CornerType.Top;
			this.psProfileMemory.CaptionGradient.End = System.Drawing.Color.Gray;
			this.psProfileMemory.CaptionGradient.Start = System.Drawing.Color.Gray;
			this.psProfileMemory.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psProfileMemory.CaptionUnderline = System.Drawing.Color.Gray;
			this.psProfileMemory.Controls.Add(this.Collect);
			this.psProfileMemory.Controls.Add(this.Dump);
			this.psProfileMemory.Controls.Add(this.Flush);
			this.psProfileMemory.CurveRadius = 12;
			this.psProfileMemory.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psProfileMemory.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psProfileMemory.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psProfileMemory.ImageItems.Disabled = -1;
			this.psProfileMemory.ImageItems.Highlight = -1;
			this.psProfileMemory.ImageItems.Normal = -1;
			this.psProfileMemory.ImageItems.Pressed = -1;
			this.psProfileMemory.ImageItems.PSImgSet = null;
			this.psProfileMemory.Location = new System.Drawing.Point(8, 96);
			this.psProfileMemory.Name = "psProfileMemory";
			this.psProfileMemory.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfileMemory.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfileMemory.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psProfileMemory.PanelState = PSUI.PSPanelState.Collapsed;
			this.psProfileMemory.Size = new System.Drawing.Size(266, 33);
			this.psProfileMemory.TabIndex = 1;
			this.psProfileMemory.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfileMemory.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psProfileMemory.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psProfileMemory.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfileMemory.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psProfileMemory.Collapsed += new System.EventHandler(this.psProfilePerformance_Collapsed);
			this.psProfileMemory.Expanded += new System.EventHandler(this.psProfilePerformance_Expanded);
			// 
			// Collect
			// 
			this.Collect.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Collect.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Collect.Enabled = false;
			this.Collect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Collect.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Collect.Location = new System.Drawing.Point(24, 40);
			this.Collect.Name = "Collect";
			this.Collect.Size = new System.Drawing.Size(56, 32);
			this.Collect.TabIndex = 41;
			this.Collect.Text = "Capture";
			this.profilerControlToolTip.SetToolTip(this.Collect, "Take a Memory Snapshot of the .NET Heap.");
			this.Collect.Click += new System.EventHandler(this.Collect_Click);
			// 
			// Dump
			// 
			this.Dump.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Dump.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Dump.Enabled = false;
			this.Dump.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Dump.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Dump.Location = new System.Drawing.Point(88, 40);
			this.Dump.Name = "Dump";
			this.Dump.Size = new System.Drawing.Size(64, 32);
			this.Dump.TabIndex = 39;
			this.Dump.Text = "Save";
			this.profilerControlToolTip.SetToolTip(this.Dump, "Save the memory-profiling data collected after clicking \'Capture\'");
			this.Dump.Click += new System.EventHandler(this.Dump_Click);
			// 
			// Flush
			// 
			this.Flush.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Flush.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Flush.Enabled = false;
			this.Flush.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Flush.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Flush.Location = new System.Drawing.Point(160, 40);
			this.Flush.Name = "Flush";
			this.Flush.Size = new System.Drawing.Size(64, 32);
			this.Flush.TabIndex = 40;
			this.Flush.Text = "Clear";
			this.profilerControlToolTip.SetToolTip(this.Flush, "Clear the Profiler\'s Memory. After saving the profiled information, click Clear.");
			this.Flush.Click += new System.EventHandler(this.Flush_Click);
			// 
			// psProfilePerformance
			// 
			this.psProfilePerformance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.psProfilePerformance.AnimationRate = 0;
			this.psProfilePerformance.BackColor = System.Drawing.Color.Transparent;
			this.psProfilePerformance.Caption = "Profile Performance";
			this.psProfilePerformance.CaptionCornerType = PSUI.CornerType.Top;
			this.psProfilePerformance.CaptionGradient.End = System.Drawing.SystemColors.AppWorkspace;
			this.psProfilePerformance.CaptionGradient.Start = System.Drawing.SystemColors.AppWorkspace;
			this.psProfilePerformance.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psProfilePerformance.CaptionUnderline = System.Drawing.SystemColors.ControlDark;
			this.psProfilePerformance.Controls.Add(this.Start);
			this.psProfilePerformance.Controls.Add(this.Stop);
			this.psProfilePerformance.Controls.Add(this.Clear);
			this.psProfilePerformance.Controls.Add(this.Save);
			this.psProfilePerformance.CurveRadius = 12;
			this.psProfilePerformance.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.psProfilePerformance.ForeColor = System.Drawing.SystemColors.WindowText;
			this.psProfilePerformance.HorzAlignment = System.Drawing.StringAlignment.Near;
			this.psProfilePerformance.ImageItems.Disabled = -1;
			this.psProfilePerformance.ImageItems.Highlight = -1;
			this.psProfilePerformance.ImageItems.Normal = -1;
			this.psProfilePerformance.ImageItems.Pressed = -1;
			this.psProfilePerformance.ImageItems.PSImgSet = null;
			this.psProfilePerformance.Location = new System.Drawing.Point(8, 8);
			this.psProfilePerformance.Name = "psProfilePerformance";
			this.psProfilePerformance.PanelGradient.End = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfilePerformance.PanelGradient.Start = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfilePerformance.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			this.psProfilePerformance.Size = new System.Drawing.Size(266, 80);
			this.psProfilePerformance.TabIndex = 0;
			this.psProfilePerformance.TextColors.Background = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfilePerformance.TextColors.Foreground = System.Drawing.SystemColors.ControlText;
			this.psProfilePerformance.TextHighlightColors.Background = System.Drawing.SystemColors.ControlText;
			this.psProfilePerformance.TextHighlightColors.Foreground = System.Drawing.SystemColors.ActiveCaptionText;
			this.psProfilePerformance.VertAlignment = System.Drawing.StringAlignment.Center;
			this.psProfilePerformance.Collapsed += new System.EventHandler(this.psProfilePerformance_Collapsed);
			this.psProfilePerformance.Expanded += new System.EventHandler(this.psProfilePerformance_Expanded);
			// 
			// Start
			// 
			this.Start.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Start.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Start.Enabled = false;
			this.Start.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Start.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Start.Location = new System.Drawing.Point(16, 40);
			this.Start.Name = "Start";
			this.Start.Size = new System.Drawing.Size(56, 32);
			this.Start.TabIndex = 34;
			this.Start.Text = "Start";
			this.profilerControlToolTip.SetToolTip(this.Start, "Start Performance Profiling the Target Process.");
			this.Start.Click += new System.EventHandler(this.Start_Click);
			// 
			// Stop
			// 
			this.Stop.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Stop.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Stop.Enabled = false;
			this.Stop.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Stop.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Stop.Location = new System.Drawing.Point(72, 40);
			this.Stop.Name = "Stop";
			this.Stop.Size = new System.Drawing.Size(56, 32);
			this.Stop.TabIndex = 33;
			this.Stop.Text = "Stop";
			this.profilerControlToolTip.SetToolTip(this.Stop, "Stop Performance Profiling the Target Process.");
			this.Stop.Click += new System.EventHandler(this.Stop_Click);
			// 
			// Clear
			// 
			this.Clear.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Clear.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Clear.Enabled = false;
			this.Clear.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Clear.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Clear.Location = new System.Drawing.Point(184, 40);
			this.Clear.Name = "Clear";
			this.Clear.Size = new System.Drawing.Size(64, 32);
			this.Clear.TabIndex = 32;
			this.Clear.Text = "Clear";
			this.profilerControlToolTip.SetToolTip(this.Clear, "Clear the Profiler\'s Memory. After saving the profiled information, click Clear.");
			this.Clear.Click += new System.EventHandler(this.Clear_Click);
			// 
			// Save
			// 
			this.Save.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Save.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Save.Enabled = false;
			this.Save.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Save.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Save.Location = new System.Drawing.Point(128, 40);
			this.Save.Name = "Save";
			this.Save.Size = new System.Drawing.Size(56, 32);
			this.Save.TabIndex = 31;
			this.Save.Text = "Save";
			this.profilerControlToolTip.SetToolTip(this.Save, "Save the Performance Data Collected.");
			this.Save.Click += new System.EventHandler(this.Save_Click);
			// 
			// Init
			// 
			this.Init.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Init.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Init.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Init.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Init.Location = new System.Drawing.Point(0, 0);
			this.Init.Name = "Init";
			this.Init.Size = new System.Drawing.Size(272, 5);
			this.Init.TabIndex = 35;
			this.Init.Text = "Initialize";
			this.profilerControlToolTip.SetToolTip(this.Init, "Initialize the Profiler for Profiling.");
			this.Init.Visible = false;
			// 
			// saveSessionDlg
			// 
			this.saveSessionDlg.DefaultExt = "P#Session";
			this.saveSessionDlg.Filter = "All Files(*.*)|*.*";
			// 
			// profilerControlStausBar
			// 
			this.profilerControlStausBar.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.profilerControlStausBar.Location = new System.Drawing.Point(0, 151);
			this.profilerControlStausBar.Name = "profilerControlStausBar";
			this.profilerControlStausBar.Size = new System.Drawing.Size(282, 16);
			this.profilerControlStausBar.TabIndex = 36;
			this.profilerControlStausBar.Text = "Ready";
			// 
			// profilerControlToolTip
			// 
			this.profilerControlToolTip.AutomaticDelay = 300;
			this.profilerControlToolTip.AutoPopDelay = 6000;
			this.profilerControlToolTip.InitialDelay = 300;
			this.profilerControlToolTip.ReshowDelay = 60;
			this.profilerControlToolTip.ShowAlways = true;
			// 
			// ProfilerControl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Desktop;
			this.ClientSize = new System.Drawing.Size(282, 167);
			this.Controls.Add(this.profilerControlStausBar);
			this.Controls.Add(this.psProfileControlPanelGroup);
			this.Controls.Add(this.Init);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(600, 0);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(315, 230);
			this.Name = "ProfilerControl";
			this.Opacity = 0.8;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.TopMost = true;
			this.Closed += new System.EventHandler(this.ProfilerControl_Closed);
			this.Activated += new System.EventHandler(this.ProfilerControl_Activated);
			((System.ComponentModel.ISupportInitialize)(this.psProfileControlPanelGroup)).EndInit();
			this.psProfileControlPanelGroup.ResumeLayout(false);
			this.psProfileMemory.ResumeLayout(false);
			this.psProfilePerformance.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void ProfilerControl_HandleCreated(object sender, EventArgs e)
		{

			try
			{
				if(this.pid ==0 ||  processName==null  ||configString==null)
					throw new Exception ("Invalid profilee information passed to the profiler.");

				if(configString=="MEMORY_ANALYSIS" && m_objectFlag ==0)
					throw new Exception ("Invalid memory analysis information passed to the profiler.");

				if(configString=="PERFORMANCE_ANALYSIS" && m_functionFlag==0)
					throw new Exception ("Invalid performance analysis information passed to the profiler.");

				if(configString=="BOTH" && (m_functionFlag==0 ||m_objectFlag==0))
					throw new Exception ("Invalid information passed to the profiler.");
	

				/////////////
				///
				if(!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
				{

					if(DllRegisterServer()!=0)
					{
						throw new Exception("The profiler component failed to load.\nPlease try re-registering the profiler or reinstall the profiler.");
					}

					string ext="dll";
					IntPtr hMod=LoadLibraryA(string.Concat(dll+cor+"e",".",ext));					
					//					IntPtr hMod=(IntPtr)1342177280;
					//					BFC=(UIntPtr)1342182960;
					//					CFC=(UIntPtr)1342216656;
					//					COC=(UIntPtr)1342218496;
					//					CO=(UIntPtr)1342218624;
					//					DFD=(UIntPtr)1342213328;
					//					DOD=(UIntPtr)1342218944;
					//					SFC=(UIntPtr)1342183120;
					//					IP=(UIntPtr)1342221232;
					//					RCLR=(UIntPtr)1342221840;
					BFC=GetProcAddress(hMod,"BFC");
					if(BFC==(System.UIntPtr.Zero)) throw new Exception("BFC is null"); 
					CFC=GetProcAddress(hMod,"CFC");
					if(CFC==(System.UIntPtr.Zero)) throw new Exception("CFC is null"); 
					COC=GetProcAddress(hMod,"COC");
					if(COC==( System.UIntPtr.Zero)) throw new Exception("COC is null"); 
					CO=GetProcAddress(hMod,"CO");
					if(CO==( System.UIntPtr.Zero)) throw new Exception("CO is null"); 
					DFD=GetProcAddress(hMod,"DFD");
					if(DFD==( System.UIntPtr.Zero)) throw new Exception("DFD is null"); 
					DOD=GetProcAddress(hMod,"DOD");
					if(DOD==( System.UIntPtr.Zero)) throw new Exception("DOD is null"); 
					SFC=GetProcAddress(hMod,"SFC");
					if(SFC==(System.UIntPtr.Zero)) throw new Exception("SFC is null"); 
					IP=GetProcAddress(hMod,"IP");
					if(IP==(System.UIntPtr.Zero)) throw new Exception("IP is null"); 
					RCLR=GetProcAddress(hMod,"RCLR");
					if(RCLR==System.UIntPtr.Zero ) throw new Exception("RCLR is null"); 
				}
	
				this.ControlRemoved+=new ControlEventHandler(ProfilerControl_ControlRemoved); 
				this.psProfileControlPanelGroup.ControlRemoved +=new ControlEventHandler(psProfileControlPanelGroup_ControlRemoved); 

				if(m_functionFlag==0)
				{
					try{this.psProfileControlPanelGroup.Controls.Remove(psProfilePerformance);   }
					catch{}
				}
				else if(m_objectFlag==0)
				{
					try{this.psProfileControlPanelGroup.Controls.Remove(psProfileMemory);   }
					catch{}
				}
				

				if(Convert.ToString(SharpClientForm.profilerTable[this.pid]).ToUpper().Trim() != this.processName.ToUpper().Trim() )
				{
					SharpClientForm.profilerTable[this.pid]=this.processName;
				}		

			}
			catch(Exception ex)
			{
				
				MessageBox.Show("Unable to start the profiler.\n"+ex.Message, "Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);								
				foreach (Control control in this.Controls )
				{
					control.Enabled=false; 
				}

				TryKill();				
				return;	
			}
			

		}

		private void TryKill()
		{
			try
			{
				if(configString=="PERFORMANCE_ANALYSIS" && m_functionFlag!=0)
				{				
					if(SFC!=UIntPtr.Zero && (int)SFC!=0 )
					{
						HandleMethodCall(true,SFC,"SFC",0,"Profiling Stopped.","Unable to stop!");						 
					}
				}				
			}
			catch{}
			Application.DoEvents();			
			if(pid!=0 && MessageBox.Show("Terminate profilee process?","Terminate?",MessageBoxButtons.YesNo,MessageBoxIcon.Question) ==DialogResult.Yes ) 
			{
				try
				{
					System.Diagnostics.Process.GetProcessById(pid).Kill(); 
				}
				catch(Exception exc)
				{
					MessageBox.Show("The process could not be killed.\n"+exc.Message,"!!!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
 
				}				
			}
		}
		
		private void InitializeProfiler()
		{
			bDefer=true;
			try
			{
				System.Diagnostics.Process process=System.Diagnostics.Process.GetProcessById(pid)   ;
				IntPtr hProcess=process.Handle ;
				if(hProcess==IntPtr.Zero)
				{
					throw new Exception("Invalid Process Handle"); 
				}
				else
				{
					int dwThreadId;	
					if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
					{
						profilerControlStausBar.Text =  "Profiler Initialization Successful";
						return ;
					}		
					int pidParam=pid;
					string strParam=pidParam.ToString();
					ASCIIEncoding encCur = new ASCIIEncoding();
					byte[] encodedPassword = encCur.GetBytes(strParam);//					

					for(int x=0;x<encodedPassword.Length;x++)
					{
						pidParam^=Convert.ToInt32(encodedPassword[x]); 
					}

					IntPtr hThread=CreateRemoteThread(hProcess,IntPtr.Zero,0,IP,(IntPtr)pidParam,4,out dwThreadId);					//Suspended					
					if(System.Runtime.InteropServices.Marshal.GetLastWin32Error()==5)
					{
						bDefer=true;
						throw new System.Security.SecurityException("Access Denied");  
					}
			
					if( hThread==IntPtr.Zero)
					{
						throw new Exception("Invalid Thread Handle"); 
					}

					int result=ResumeThread(hThread);
					if(result==-1)
					{
						throw new Exception("Initialization failed with thread abortion"); 
					}
					if(result==1 || result==0)
					{
						//ok
					}
					else
					{
						throw new Exception("Initialization failed with thread count error"); 
					}

					int dwStatus;
					int success;
					while(true)
					{
						success=GetExitCodeThread(hThread,out dwStatus);
						if(success==0 || dwStatus !=0x00000103L)//0x00000103L :-  status_running
							break;
						System.Threading.Thread.Sleep(100);
					} 
					try{CloseHandle(hThread);}catch{}	
					try{CloseHandle(hProcess);}catch{}		

					if(dwStatus==0)
					{
						//MessageBox.Show("Profiler Initialization Successful"); 
						bDefer=false;
						profilerControlStausBar.Text =  "Profiler Initialization Successful";
						Application.DoEvents(); 
						if(Configurator.IsProcessOA(pid))
						{
							HandleMethodCall(false,CO,"CO" ,m_objectFlag," Profiler Initialization Successful","Initialization failed");
						} 						
						return;
					}
					else if(dwStatus==-2147023649)	//Already initialized
					{
						//MessageBox.Show("Profiler has already been initialized for this process."); 
						profilerControlStausBar.Text =  "Profiler already initialized.";
						Application.DoEvents(); 
						bDefer=false;
						return;
					}
					else throw new COMException("Initialization failed with error :- "+dwStatus.ToString() ,dwStatus);
				}				
			}
			catch{}	
				bDefer=true;
			try
			{
				if(!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)     
				{				
					if(delegatorObj == null || !System.Runtime.InteropServices.Marshal.IsComObject(delegatorObj) )					
					{
						try
						{
							delegatorObj = new SharpDelegatorClass();					
							delegatorObj.profilerName=Application.StartupPath+@"\SoftProdigy.Core.dll";
						}
						catch(COMException com_Error)
						{
							if(com_Error.ErrorCode==-2147023649)	//Already initialized
							{
								profilerControlStausBar.Text =  "Profiler already initialized.";
								Application.DoEvents(); 						
								return;
							}
								//else throw new COMException("Initialization failed with error :- "+com_Error.ErrorCode.ToString()  ,com_Error.ErrorCode);
							else
							{
									throw com_Error;
							}
						}
					}

					int pidParam=pid;
					string strParam=pidParam.ToString();
					ASCIIEncoding encCur = new ASCIIEncoding();
					byte[] encodedPassword = encCur.GetBytes(strParam);//					

					for(int x=0;x<encodedPassword.Length;x++)
					{
						pidParam^=Convert.ToInt32(encodedPassword[x]); 
					}

					delegatorObj.DeferMethodCall(pid,"IP",pidParam);	
					profilerControlStausBar.Text =  "Profiler Initialization Successful";
					Application.DoEvents(); 
					if(Configurator.IsProcessOA(pid))
					{
						HandleMethodCall(false,CO,"CO" ,m_objectFlag," Profiler Initialization Successful","Initialization failed");
					}  				
				}
			}
			catch(COMException com_Error)
			{
				MessageBox.Show(" Profiler Initialization Failed.\n"+com_Error.Message,"Exception!",MessageBoxButtons.OK ,MessageBoxIcon.Error) ;
				try
				{
					profilerControlStausBar.Text =  "Initialization failed :- "+com_Error.ErrorCode.ToString()  ;				
				}
				catch{}
				foreach (Control control in this.Controls )
				{
					control.Enabled=false; 
				}
				Application.DoEvents();

				BFC=System.UIntPtr.Zero  ;
				CFC=System.UIntPtr.Zero;
				COC=System.UIntPtr.Zero;
				CO=System.UIntPtr.Zero;
				DFD=System.UIntPtr.Zero;
				DOD=System.UIntPtr.Zero;
				SFC=System.UIntPtr.Zero;				
				RCLR=System.UIntPtr.Zero; 
				throw com_Error;
			}
			catch(Exception except)
			{				
				try
				{
					profilerControlStausBar.Text =  " Profiler Initialization failed :- "+Marshal.GetHRForException(except).ToString()  ;				
				}
				catch{}
				foreach (Control control in this.Controls )
				{
					control.Enabled=false; 
				}
				Application.DoEvents();

				BFC=System.UIntPtr.Zero  ;
				CFC=System.UIntPtr.Zero;
				COC=System.UIntPtr.Zero;
				CO=System.UIntPtr.Zero;
				DFD=System.UIntPtr.Zero;
				DOD=System.UIntPtr.Zero;
				SFC=System.UIntPtr.Zero;				
				RCLR=System.UIntPtr.Zero;
				throw except;

			}			
		
		}		

		
		

		private void ResumeProcess()
		{
			if(!bSuspended)
				return;
			if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
			{
				profilerControlStausBar.Text =  "Profiling Already Started";
				return ;
			}
			try
			{	
				if(m_functionFlag!=0)
				{
					if(HandleMethodCall(true,BFC,"BFC",m_functionFlag,"Profiling Started","Unable to start!"))
						bIsCapturingStartedForOnce=true;
					else
						bIsCapturingStartedForOnce=false;
				}

				if(!bDefer)
				{
					IntPtr hProcess=System.Diagnostics.Process.GetProcessById(pid).Handle ;  
					if(hProcess==IntPtr.Zero)
						throw new Exception("Invalid Process Handle"); 

					int dwThreadId;	
					int pidParam=pid;
					string strParam=pidParam.ToString();
					ASCIIEncoding encCur = new ASCIIEncoding();
					byte[] encodedPassword = encCur.GetBytes(strParam);									
					for(int x=0;x<encodedPassword.Length;x++)
					{
						pidParam^=Convert.ToInt32(encodedPassword[x]); 
					}

					IntPtr hThread=CreateRemoteThread(hProcess,IntPtr.Zero,0,RCLR,(IntPtr)pidParam,0,out dwThreadId);					
					if( hThread==IntPtr.Zero)
						throw new Exception("Invalid Thread Handle"); 		
					int dwStatus;
					int success;
					while(true)
					{
						success=GetExitCodeThread(hThread,out dwStatus);
						if(success==0 || dwStatus !=0x00000103L)//0x00000103L :-  status_running
							break;
						System.Threading.Thread.Sleep(100);
					} 
					try{CloseHandle(hThread);}
					catch{}	
					try{CloseHandle(hProcess);}
					catch{}

					if(dwStatus!=0)
						Marshal.ThrowExceptionForHR(dwStatus);
					else
					{						
						profilerControlStausBar.Text =  "Profiling Already Started.Process Running Now.";
						Application.DoEvents(); 						

						try{this.Controls.Remove(Init);   }
						catch{} 
						
					}
					return;
				}
				else
				{
					try
					{
						if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
							throw new Exception("Invalid Configuration");  

						if(delegatorObj == null ||  !System.Runtime.InteropServices.Marshal.IsComObject(delegatorObj) )
							delegatorObj = new SharpDelegatorClass();
						try
						{
							delegatorObj.profilerName=Application.StartupPath+@"\SoftProdigy.Core.dll";
						}
						catch(COMException com_Error)
						{
							if(com_Error.ErrorCode==-2147023649)	//Already initialized
							{
								//Do nothing
							}
							else throw com_Error;//new COMException("Resumption failed with error :- "+com_Error.ErrorCode.ToString()  ,com_Error.ErrorCode);
						}

						int pidParam=pid;
						string strParam=pidParam.ToString();
						ASCIIEncoding encCur = new ASCIIEncoding();
						byte[] encodedPassword = encCur.GetBytes(strParam);									
						for(int x=0;x<encodedPassword.Length;x++)
						{
							pidParam^=Convert.ToInt32(encodedPassword[x]); 
						}

						delegatorObj.DeferMethodCall(pid,"RCLR", pidParam);						
						profilerControlStausBar.Text =  "Profiling Already Started.Process Running Now.";
						Application.DoEvents(); 
						
						//bSuspended=false;//This is another option						
						try{this.Controls.Remove(Init);   }
						catch{}
  
					}
					catch(COMException com_Error)
					{
						throw com_Error;
					}
					catch(Exception except)
					{				
						throw except;
					}

				}
				
			}
			catch(COMException com_Error)
			{
				MessageBox.Show(" Resumption Failed.\n"+com_Error.Message,"Exception!",MessageBoxButtons.OK ,MessageBoxIcon.Error) ;
				profilerControlStausBar.Text =  "Resumption failed :- "+com_Error.ErrorCode.ToString()  ;				
				foreach (Control control in this.Controls )
				{
					control.Enabled=false; 
				}
				Application.DoEvents();

				BFC=System.UIntPtr.Zero  ;
				CFC=System.UIntPtr.Zero;
				COC=System.UIntPtr.Zero;
				CO=System.UIntPtr.Zero;
				DFD=System.UIntPtr.Zero;
				DOD=System.UIntPtr.Zero;
				SFC=System.UIntPtr.Zero;				
				RCLR=System.UIntPtr.Zero; 				
			}
			catch(Exception except)
			{			
				MessageBox.Show(" Resumption Failed.\n"+except.Message,"Exception!",MessageBoxButtons.OK ,MessageBoxIcon.Error) ;
				try
				{
					profilerControlStausBar.Text =  " Resumption Failed :- "+Marshal.GetHRForException(except).ToString()  ;				
				}
				catch{}
				foreach (Control control in this.Controls )
				{
					control.Enabled=false; 
				}
				Application.DoEvents();

				BFC=System.UIntPtr.Zero  ;
				CFC=System.UIntPtr.Zero;
				COC=System.UIntPtr.Zero;
				CO=System.UIntPtr.Zero;
				DFD=System.UIntPtr.Zero;
				DOD=System.UIntPtr.Zero;
				SFC=System.UIntPtr.Zero;				
				RCLR=System.UIntPtr.Zero;
			}	
		}

		private void Start_Click(object sender, System.EventArgs e)
		{
			if(HandleMethodCall(true,BFC,"BFC",m_functionFlag,"Profiling Started","Unable to start!"))
				bIsCapturingStartedForOnce=true;
			else
				bIsCapturingStartedForOnce=false;

		}

		private void Stop_Click(object sender, System.EventArgs e)
		{
			HandleMethodCall(true,SFC,"SFC",0,"Stopped","Unable to stop!");
		
		}

		private void Clear_Click(object sender, System.EventArgs e)
		{
			HandleMethodCall(true,CFC,"CFC",0,"Cleared","Unable to clear performance data cache.");
		
		}

		private void Save_Click(object sender, System.EventArgs e)
		{
			try
			{		
		
				saveSessionDlg.Title ="Save Performance Session";
				saveSessionDlg.RestoreDirectory =true;

				if(saveSessionDlg.ShowDialog ()==DialogResult.OK)
				{			
					if(!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
					{
						SetLastSession(saveSessionDlg.FileName);					
						#region Save Performance Data
						profilerControlStausBar.Text =  "Please wait.Saving..";
						Application.DoEvents(); 	   
						HandleMethodCall(true,DFD,"DFD",0,"Saved","Unable to save performance data!");
						lastFile=saveSessionDlg.FileName ;

						#endregion
					}

				}	
				else
				{
					throw new Exception("No session data saved!"); 
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);   
			}

		}


		private void Dump_Click(object sender, System.EventArgs e)
		{
			
			try
			{		
				saveSessionDlg.Title ="Save Memory Analysis Session";
				saveSessionDlg.RestoreDirectory =true;
				if(saveSessionDlg.ShowDialog ()==DialogResult.OK)
				{			
					if(!System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
					{					
						SetLastSession(saveSessionDlg.FileName);					
						#region Save Memory Data
						profilerControlStausBar.Text =  "Please wait.Saving..";
						Application.DoEvents(); 
						HandleMethodCall(false,DOD,"DOD",0,"Saved","Unable to save memory analysis data!");
						lastFile=saveSessionDlg.FileName;
						#endregion
					}

				}	
				else
				{
					throw new Exception("No session data saved!"); 
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);   
			}

		}

		private void SetLastSession(string session)
		{
			try
			{
				string path;
				string sessionName;
				int iDiv=session.LastIndexOf("\\");
				path=session.Substring(0,iDiv );   
				sessionName=session.Substring(iDiv+1,session.Length-(iDiv+1));
			
				RegistryKey keySession=null;							
				keySession=Registry.LocalMachine.OpenSubKey("Software\\Profiler#\\",true);   
				keySession.SetValue("LastSessionPath",path);				
				keySession.SetValue("LastSessionName",sessionName);							
				if(keySession!=null)
				{
					try{keySession.Close();}
					catch{}
				}
			}
			catch(Exception exc)
			{			
				throw new Exception("Unable to set session configuration values.\n"+exc.Message) ;
			}

		}

	

		private void InitProfiler()
		{
			try
			{

				Init.Enabled=false;
				this.profilerControlStausBar.Text="Waiting for CLR to be loaded in the process..";				
				this.Cursor =Cursors.WaitCursor ;	
			
				//SharpClientForm.scInstance.Enabled =false;//We do not want the process configuration 
				//changed by another configurator before it has started				

				Application.DoEvents();
				try
				{					
					if(bSuspended)
					{
						while(true)
						{							
							Application.DoEvents();  
							if(bQuit)
							{
								return;
							}
							IntPtr hMutex=System.IntPtr.Zero;													
							hMutex=OpenMutex(1,0,"Global\\"+this.pid.ToString() +"?TRUE");
							int hr=Marshal.GetLastWin32Error();
							
							if( hMutex!= System.IntPtr.Zero) 
							{
								try{ReleaseMutex(hMutex);}
								catch{}
								try{CloseHandle(hMutex);}
								catch{}
								hMutex=System.IntPtr.Zero;
								break;
							}
							else if (hr==5)//ACCESS_DENIED
							{
								hMutex=System.IntPtr.Zero;
								break;
							}
							System.Threading.Thread.Sleep(200);   
						}
					}
					else
					{											
						while(true)
						{							
							Application.DoEvents();  
							if(bQuit)
							{
								return;
							}
							IntPtr hMutex=System.IntPtr.Zero;													
							hMutex=OpenMutex(1,0,"Global\\"+this.pid.ToString()+"?FALSE");
							int hr=Marshal.GetLastWin32Error();
							if( hMutex!= System.IntPtr.Zero) 
							{
								try{ReleaseMutex(hMutex);}
								catch{}
								try{CloseHandle(hMutex);}
								catch{}
								hMutex=System.IntPtr.Zero;
								break;
							}
							else if (hr==5)//ACCESS_DENIED
							{
								hMutex=System.IntPtr.Zero;
								break;
							}
							System.Threading.Thread.Sleep(200);   
						}					
					}
				}
				catch{}
				finally
				{
					Init.Enabled=true;
					SharpClientForm.scInstance.Enabled =true;
					this.profilerControlStausBar.Text="Ready";				  
					this.Cursor=Cursors.Arrow;  
					Application.DoEvents();					 
				}
			
				/////////////////////////////////
			
				try
				{
					InitializeProfiler(); 
				}
				catch
				{					
					foreach (Control control in this.Controls )
					{
						control.Enabled=false; 
					}				
					return;
				}
						

				if(m_functionFlag!=0 && (configString== "PERFORMANCE_ANALYSIS" || configString =="BOTH") && psProfilePerformance!=null )
				{
					Start.Enabled =true;
					Stop.Enabled =true;
					Clear.Enabled =true;
					Save.Enabled =true;								
				}
				if(m_objectFlag !=0 && (configString== "MEMORY_ANALYSIS" || configString =="BOTH") && psProfileMemory!=null )
				{
					Collect.Enabled=true;
					Flush.Enabled =true;
					Dump.Enabled =true;								
				}  

				Init.Enabled =false;
			
				if(bSuspended)
				{
					ResumeProcess();
				}

				Application.DoEvents(); 
				
			}
			finally
			{
				try
				{
					this.Controls.Remove(Init);   
				}
				catch{}
			}
		}

		private void psProfilePerformance_Expanded(object sender, System.EventArgs e)
		{
			foreach (Control psControl in psProfileControlPanelGroup.Controls) 
			{
				if(psControl.GetType()==typeof(PSPanel)  )
				{
					if(psControl!=sender )
					{
						((PSPanel)psControl).PanelState =PSPanelState.Collapsed;  
					}
					else
					{
						psControl.Size=new Size(296,80);
						this.Height+=30;  //33 is the height of caption.We will ignore it
						this.psProfileControlPanelGroup.Height+=30 ;  

					}
				}				
			}


			Application.DoEvents ();
		}

		private void Collect_Click(object sender, System.EventArgs e)
		{
			profilerControlStausBar.Text =  "Please wait.Capturing..";
			Application.DoEvents(); 
			HandleMethodCall(false,CO,"CO" ,m_objectFlag,"Captured.Please save it now.","Unable to analyze memory");
		}

		private void Flush_Click(object sender, System.EventArgs e)
		{
			HandleMethodCall(false,COC,"COC",0,"Cleared","Unable to clear the memory cache.");
		
		}		
		
		private bool HandleMethodCall(bool isFunctionCall,UIntPtr methodPtr,string methodName,int param,string successMessage,string failMessage)
		{

			if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
			{
				profilerControlStausBar.Text =  successMessage;
				return true;
			}
			try
			{
				if(isFunctionCall)
				{
					if(m_functionFlag!=0 && (configString=="PERFORMANCE_ANALYSIS"  || configString=="BOTH"))
					{
						goto A;
					}
					else
					{
						MessageBox.Show("This action is not valid for this process.", "Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);										
						return false;
					}
				}
				else
				{
					if(m_objectFlag !=0 && (configString=="MEMORY_ANALYSIS"  || configString=="BOTH"))
					{
						goto A;
					}
					else
					{
						MessageBox.Show("This action is not valid for this process.", "Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);										
						return false;
					}

				}
				
			A:

				if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
				{
					profilerControlStausBar.Text =  successMessage;
					return true;
				}

				if(!bDefer)									
				{				
					//if(System.DateTime.Now.Second%2==0)					
					if(true)					
					{
						IntPtr hProcess=System.Diagnostics.Process.GetProcessById(pid).Handle;   
						if(hProcess==IntPtr.Zero)
							throw new Exception("Invalid Process Handle"); 

						int dwThreadId=0;
						IntPtr hThread=IntPtr.Zero;
						if(param==0)
						{
							if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
							{
								hThread=IntPtr.Zero ;
							}
							else
							{
								hThread=CreateRemoteThread(hProcess,IntPtr.Zero,0,methodPtr,IntPtr.Zero,0,out dwThreadId);			
							}
						}
						else
						{
							if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
							{
								hThread=(IntPtr)200 ;
							}
							else
							{
								hThread=CreateRemoteThread(hProcess,IntPtr.Zero,0,methodPtr,(IntPtr)param,0,out dwThreadId);			
							}
						}

						if( hThread==IntPtr.Zero)						
							throw new Exception("Invalid Thread Handle:-"+ System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error().ToString()); 				

						int dwStatus;
						int success;
						while(true)
						{
							success=GetExitCodeThread(hThread,out dwStatus);
							if(success==0 || dwStatus !=0x00000103L)//0x00000103L :-  status_running
								break;
							System.Threading.Thread.Sleep(100);
						} 
						try{CloseHandle(hThread);}
						catch{}	
						try{CloseHandle(hProcess);}
						catch{}

						if(dwStatus!=0)
						{
							Marshal.ThrowExceptionForHR  (dwStatus);
						}
						else
						{	
							profilerControlStausBar.Text =  successMessage;
							Application.DoEvents(); 
					  
						}
					}					
					return true;
				}
				else
				{
					try
					{
						if(System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
						{
							return true;
						}
						if(delegatorObj == null || !System.Runtime.InteropServices.Marshal.IsComObject(delegatorObj) )
							delegatorObj = new SharpDelegatorClass();
						try
						{
							delegatorObj.profilerName=Application.StartupPath+@"\SoftProdigy.Core.dll";
						}
						catch(COMException com_Error)
						{
							if(com_Error.ErrorCode==-2147023649)	//Already initialized
							{
								//Do nothing
							}
							else throw com_Error;
						}

						delegatorObj.DeferMethodCall(pid,methodName,param);					  				
						profilerControlStausBar.Text = successMessage;
						Application.DoEvents(); 					  
					}
					catch(COMException com_Error)
					{
						throw com_Error;
					}
					catch(Exception exc)
					{
						throw exc;
					}
					return true;					  

				}
			}
			catch(COMException _com_error)
			{
				profilerControlStausBar.Text = failMessage;
				Application.DoEvents(); 
				MessageBox.Show(failMessage+"\n"+_com_error.Message, "Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			catch(Exception ex)
			{
				profilerControlStausBar.Text = failMessage;
				Application.DoEvents(); 
				MessageBox.Show(failMessage+"\n"+ex.Message, "Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);															 
			}

			return false;

		}

		private void ProfilerControl_ControlRemoved(object sender, ControlEventArgs e)
		{
			if(e.Control.Name.ToUpper()=="INIT")
			{
				try
				{
					this.Height -= (Init.Height +2);
					this.psProfileControlPanelGroup.Location=new Point(0,0); 
				}
				catch{}
			}
			Application.DoEvents();  
					
		}

		private void psProfileControlPanelGroup_ControlRemoved(object sender, ControlEventArgs e)
		{
			if(e.Control.Name.ToUpper()=="PSPROFILEPERFORMANCE")
			{
				psProfileControlPanelGroup.Height-=(psProfilePerformance.Height+5); 
				this.Height -=(e.Control.Height+ 5) ;
				Application.DoEvents();  				
			}
			else if(e.Control.Name.ToUpper() == "PSPROFILEMEMORY"  )
			{
				psProfileControlPanelGroup.Height-=(psProfileMemory.Height+15);   
				this.Height -=e.Control.Height+ 10 ;
 
			}

			psProfileMemory.Location =new Point(6,6); 			
 
			Application.DoEvents();  
		}

		private void psProfilePerformance_Collapsed(object sender, System.EventArgs e)
		{
			if(sender.GetType()==typeof(PSPanel)  )
			{
				this.Height-=30 ;
				this.psProfileControlPanelGroup.Height-=30 ;  
				
			}				
		}

		private void ProfilerControl_Closed(object sender, EventArgs e)
		{
			if(delegatorObj!=null)
				System.Runtime.InteropServices.Marshal.ReleaseComObject(delegatorObj);
			try
			{
				SharpClientForm.profilerTable.Remove(this.pid);   
			}
			catch{}
			finally
			{
				bQuit=true;
			}
			TryKill();	
			try
			{
				try
				{
					this.Visible=false;					
				}
				catch{}
				Application.DoEvents();
				if(lastFile!=null && MessageBox.Show("Do you want to open the last saved profiling results now?","Open Last Results?",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
				{					 
					if(configString=="PERFORMANCE_ANALYSIS")
					{
						if(!lastFile.Trim().ToLower().EndsWith(".fxml"))
						{
							lastFile+=".fxml";
						}
					}
					else if(configString=="MEMORY_ANALYSIS")
					{
						if(!lastFile.Trim().ToLower().EndsWith(".oxml"))
						{
							lastFile+=".oxml";
						}
					}
					if(System.IO.File.Exists(lastFile))
					{
						SharpClientTabPage newResultsTab=new SharpClientTabPage("Loading...",lastFile ); 				
						newResultsTab.Dock =DockStyle.Fill ;
						SharpClientForm.scInstance.sharpClientMDITab.TabPages.Add(newResultsTab); 
						SharpClientForm.scInstance.sharpClientMDITab.SelectedTab =newResultsTab ;
						newResultsTab.Show();   
					}
				}
			}
			catch{}

		}
		

		
		private void ProfilerControl_Activated(object sender, System.EventArgs e)
		{			
			if(this.Controls.Contains(Init))
			{				
				this.Show(); 				
				if(m_cnfg!=null)//V. Imp.
				{
					try
					{						
						m_cnfg.CloseConfigurator(); 
						m_cnfg.Dispose(); 
					}
					catch{}
				}
				m_cnfg=null;			
				System.GC.Collect();  
				InitProfiler();	
				try
				{
					this.Controls.Remove(Init);   
				}
				catch{}
			}		
		}
	}
}



