using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpClient
{
	/// <summary>
	/// Summary description for ConfiguratorForm.
	/// </summary>
	public class ConfiguratorForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label errorLabel;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private Configurator cnfg; 

		public ConfiguratorForm(string config,FunctionData funcObj,ObjectData objObj,bool exceptionTracing,bool bConfigRequired,bool bInteractive)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Icon=SharpClientForm.scInstance.Icon; 
			try
			{
				cnfg=new Configurator(config,funcObj,objObj,exceptionTracing,bConfigRequired,bInteractive); 			
			}
			catch(Exception ex)
			{
				this.Text="ProfileSharp Exception!" ;
				this.Size=new Size(this.Width/2,this.Height/2);    
				this.errorLabel.Text="\nAn exception has occured while trying to initialize profiling environment on your system.\n"  
					+"\n Exception details are :- \n\""+ex.Message+"\"\n\n Please contact SoftProdigy for support on this issue.";
				Application.DoEvents(); 
				this.Resize+=new EventHandler(ConfiguratorForm_Resize); 
				Application.DoEvents();
				return;
			}			

			this.Controls.Clear();
			Application.DoEvents(); 
			cnfg.Parent =this;
			this.Controls.Add(cnfg);
			cnfg.Dock =DockStyle.Fill ;
			this.ControlRemoved+=new ControlEventHandler(ConfiguratorForm_ControlRemoved);						
			this.Resize+=new EventHandler(ConfiguratorForm_Resize); 
			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfiguratorForm));
			this.errorLabel = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// errorLabel
			// 
			this.errorLabel.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.errorLabel.ForeColor = System.Drawing.Color.DarkRed;
			this.errorLabel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.errorLabel.Location = new System.Drawing.Point(64, 0);
			this.errorLabel.Name = "errorLabel";
			this.errorLabel.Size = new System.Drawing.Size(264, 168);
			this.errorLabel.TabIndex = 0;
			this.errorLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this.linkLabel1.Location = new System.Drawing.Point(40, 176);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(304, 23);
			this.linkLabel1.TabIndex = 1;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Report Error";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(64, 64);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// ConfiguratorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ClientSize = new System.Drawing.Size(698, 425);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.errorLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ConfiguratorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Configurator";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void ConfiguratorForm_ControlRemoved(object sender, ControlEventArgs e)
		{
			if(e.Control==cnfg)
			{
				SharpClientForm.scInstance.psStatusBar.Panels[0].Text="Ready to profile";				
				this.Close();
				try
				{
					this.Dispose(true); 
				}
				catch{} 
				System.GC.Collect();  				
			}
		}

		private void ConfiguratorForm_Resize(object sender, System.EventArgs e)
		{		
			SharpClientForm.scInstance.WindowState=this.WindowState;  						
		}

		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start( "iexplore.exe","\"www.SoftProdigy.com\\Contact.aspx\"");
			}
			catch
			{
				try
				{
					string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
					System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\"www.SoftProdigy.com\\Contact.aspx\"");				
					pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
					pInfo.UseShellExecute =false;
					System.Diagnostics.Process.Start(pInfo);			
				}
				catch(Exception ex)
				{
					MessageBox.Show("Unable to connect to the website\n"+ex.Message+"\n Please send an email regarding this issue to helpdesk@softprodigy.com\n Your patience is appreciated. Thank you.","Error!");				
				}
			}
		}

		
	}
}
