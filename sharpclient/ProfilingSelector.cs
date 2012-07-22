using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpClient
{
	/// <summary>
	/// Summary description for ProfilingSelector.
	/// </summary>
	/// 
	public enum PROFILEE_TYPE
	{
		PROFILE_DESKTOP_APP,
		PROFILE_BACKGROUND_PROCESS		
	}

	public class ProfilingSelector : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox typeSelector;
		private System.Windows.Forms.RadioButton memProfilingOption;
		private System.Windows.Forms.RadioButton perfProfilingOption;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button Next;
		private System.Windows.Forms.Button btnQuit;
		private System.Windows.Forms.RadioButton excProfilingOption;
		private PROFILEE_TYPE profileeType;
		private System.Windows.Forms.PictureBox pictureBox1;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		

		public ProfilingSelector(PROFILEE_TYPE profType)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.ProfileeType=profType;
			this.Icon=SharpClient.SharpClientForm.scInstance.Icon;    
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProfilingSelector));
			this.typeSelector = new System.Windows.Forms.GroupBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.excProfilingOption = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.perfProfilingOption = new System.Windows.Forms.RadioButton();
			this.memProfilingOption = new System.Windows.Forms.RadioButton();
			this.Next = new System.Windows.Forms.Button();
			this.btnQuit = new System.Windows.Forms.Button();
			this.typeSelector.SuspendLayout();
			this.SuspendLayout();
			// 
			// typeSelector
			// 
			this.typeSelector.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.typeSelector.Controls.Add(this.pictureBox1);
			this.typeSelector.Controls.Add(this.label3);
			this.typeSelector.Controls.Add(this.excProfilingOption);
			this.typeSelector.Controls.Add(this.label2);
			this.typeSelector.Controls.Add(this.label1);
			this.typeSelector.Controls.Add(this.perfProfilingOption);
			this.typeSelector.Controls.Add(this.memProfilingOption);
			this.typeSelector.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.typeSelector.Location = new System.Drawing.Point(0, 0);
			this.typeSelector.Name = "typeSelector";
			this.typeSelector.Size = new System.Drawing.Size(544, 288);
			this.typeSelector.TabIndex = 0;
			this.typeSelector.TabStop = false;
			this.typeSelector.Text = "Select Type Of Profiling.";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(24, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(472, 64);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(208, 216);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(272, 32);
			this.label3.TabIndex = 5;
			this.label3.Text = "Exception-Tracing identifies exceptions occuring in your .NET application.";
			// 
			// excProfilingOption
			// 
			this.excProfilingOption.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.excProfilingOption.Location = new System.Drawing.Point(24, 216);
			this.excProfilingOption.Name = "excProfilingOption";
			this.excProfilingOption.Size = new System.Drawing.Size(160, 32);
			this.excProfilingOption.TabIndex = 3;
			this.excProfilingOption.Text = "Trace Exceptions.";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(208, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(272, 32);
			this.label2.TabIndex = 3;
			this.label2.Text = "Performance profiling helps you discover slow lines of code in your .NET applicat" +
				"ion.";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(208, 152);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 32);
			this.label1.TabIndex = 2;
			this.label1.Text = "Memory profiling helps you detect memory leaks in your .NET application.";
			// 
			// perfProfilingOption
			// 
			this.perfProfilingOption.Checked = true;
			this.perfProfilingOption.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.perfProfilingOption.Location = new System.Drawing.Point(24, 88);
			this.perfProfilingOption.Name = "perfProfilingOption";
			this.perfProfilingOption.Size = new System.Drawing.Size(168, 32);
			this.perfProfilingOption.TabIndex = 1;
			this.perfProfilingOption.TabStop = true;
			this.perfProfilingOption.Text = "Profile Performance.";
			// 
			// memProfilingOption
			// 
			this.memProfilingOption.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.memProfilingOption.Location = new System.Drawing.Point(24, 152);
			this.memProfilingOption.Name = "memProfilingOption";
			this.memProfilingOption.Size = new System.Drawing.Size(136, 32);
			this.memProfilingOption.TabIndex = 2;
			this.memProfilingOption.Text = "Profile Memory. ";
			// 
			// Next
			// 
			this.Next.BackColor = System.Drawing.SystemColors.Control;
			this.Next.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Next.Location = new System.Drawing.Point(360, 288);
			this.Next.Name = "Next";
			this.Next.Size = new System.Drawing.Size(88, 40);
			this.Next.TabIndex = 4;
			this.Next.Text = "Next>>";
			this.Next.Click += new System.EventHandler(this.Next_Click);
			// 
			// btnQuit
			// 
			this.btnQuit.BackColor = System.Drawing.SystemColors.Control;
			this.btnQuit.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnQuit.Location = new System.Drawing.Point(448, 288);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(88, 40);
			this.btnQuit.TabIndex = 5;
			this.btnQuit.Text = "Cancel";
			this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
			// 
			// ProfilingSelector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ClientSize = new System.Drawing.Size(546, 327);
			this.ControlBox = false;
			this.Controls.Add(this.btnQuit);
			this.Controls.Add(this.Next);
			this.Controls.Add(this.typeSelector);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfilingSelector";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Profiling Type";
			this.TopMost = true;
			this.typeSelector.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnQuit_Click(object sender, System.EventArgs e)
		{
			if(DialogResult.Yes==MessageBox.Show("Are you sure you want to cancel profiling?","Cancel?" ,MessageBoxButtons.YesNo ,MessageBoxIcon.Question ))
			{
				this.Close(); 
			}
		}

		private void Next_Click(object sender, System.EventArgs e)
		{
			if(perfProfilingOption.Checked)
			{
				this.Visible=false;
				Application.DoEvents(); 
				ConfiguratorForm cnfg=null;
				if(MessageBox.Show("Do you want to configure what information the profiler should collect for this profiling session?","Configure?",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,false,true,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,false,true,false);
					}					
				}
				else
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,false,false,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,false,false,false);
					}
				}
				
				cnfg.ShowDialog(this); 					
				this.Close(); 
			}

			else if( memProfilingOption.Checked)
			{
				this.Visible=false;
				Application.DoEvents(); 
				ConfiguratorForm cnfg=null;
				if(MessageBox.Show("Do you want to configure what information the profiler should collect for this profiling session?","Configure?",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "MEMORY_ANALYSIS",null,null,false,true,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "MEMORY_ANALYSIS",null,null,false,true,false);
					}					
				}
				else
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "MEMORY_ANALYSIS",null,null,false,false,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "MEMORY_ANALYSIS",null,null,false,false,false);
					}
				}
				cnfg.ShowDialog(this); 
				this.Close(); 
			}
			else if(excProfilingOption.Checked)  
			{
				this.Visible=false;
				Application.DoEvents(); 
				ConfiguratorForm cnfg=null;
				if(MessageBox.Show("Do you want to configure what information the profiler should collect for this profiling session?","Configure?",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,true,true,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,true,true,false);
					}					
				}
				else
				{					
					if(ProfileeType==PROFILEE_TYPE.PROFILE_DESKTOP_APP)
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,true,false,true);
					}
					else
					{
						cnfg =new ConfiguratorForm( "PERFORMANCE_ANALYSIS",null,null,true,false,false);
					}
				}
				cnfg.ShowDialog(this); 
				this.Close(); 

			}
		}

		public PROFILEE_TYPE ProfileeType
		{
			get
			{
				return profileeType; 
			
			}
			set
			{
				profileeType=value;
			}
		}
	}
}
