using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpClient
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.LinkLabel homePage;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Panel logoPanel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.homePage = new System.Windows.Forms.LinkLabel();
			this.closeButton = new System.Windows.Forms.Button();
			this.logoPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// homePage
			// 
			this.homePage.Cursor = System.Windows.Forms.Cursors.Hand;
			this.homePage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.homePage.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this.homePage.Location = new System.Drawing.Point(160, 216);
			this.homePage.Name = "homePage";
			this.homePage.Size = new System.Drawing.Size(128, 16);
			this.homePage.TabIndex = 0;
			this.homePage.TabStop = true;
			this.homePage.Text = "www.softprodigy.com";
			this.homePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.homePage_LinkClicked);
			// 
			// closeButton
			// 
			this.closeButton.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.closeButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.closeButton.Location = new System.Drawing.Point(160, 240);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(120, 32);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// logoPanel
			// 
			this.logoPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.logoPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("logoPanel.BackgroundImage")));
			this.logoPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.logoPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.logoPanel.Location = new System.Drawing.Point(0, 0);
			this.logoPanel.Name = "logoPanel";
			this.logoPanel.Size = new System.Drawing.Size(454, 216);
			this.logoPanel.TabIndex = 2;
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ClientSize = new System.Drawing.Size(454, 283);
			this.Controls.Add(this.logoPanel);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.homePage);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProfileSharp Enterprise Edition v1.3";
			this.ResumeLayout(false);

		}
		#endregion

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			this.Close(); 
		}

		private void homePage_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start( "iexplore.exe","\""+homePage.Text+"\"");
			}
			catch
			{
				try
				{
					string strExplorer=Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer\\iexplore.exe";				
					System.Diagnostics.ProcessStartInfo pInfo=new System.Diagnostics.ProcessStartInfo(strExplorer,"\""+homePage.Text+"\"");				
					pInfo.WorkingDirectory =Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)+"\\Internet Explorer";
					pInfo.UseShellExecute =false;
					System.Diagnostics.Process.Start(pInfo);			
				}
				catch(Exception ex)
				{
					MessageBox.Show("Unable to connect to the website\n"+ex.Message+"\n You may need to visit the indicated link manually.","Error!");				
				}
			}
		}
	}
}
