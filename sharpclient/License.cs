using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpClient
{
	/// <summary>
	/// Summary description for License.
	/// </summary>
	public class License : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RichTextBox LicenseBox;
		private System.Windows.Forms.Button btnClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public License()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Icon=SharpClientForm.scInstance.Icon;   

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
			this.btnClose = new System.Windows.Forms.Button();
			this.LicenseBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.SystemColors.Control;
			this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnClose.Location = new System.Drawing.Point(272, 376);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(88, 23);
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "OK";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// LicenseBox
			// 
			this.LicenseBox.AutoSize = true;
			this.LicenseBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.LicenseBox.Location = new System.Drawing.Point(0, 0);
			this.LicenseBox.Name = "LicenseBox";
			this.LicenseBox.ReadOnly = true;
			this.LicenseBox.Size = new System.Drawing.Size(624, 368);
			this.LicenseBox.TabIndex = 1;
			this.LicenseBox.Text = "";
			// 
			// License
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ClientSize = new System.Drawing.Size(622, 403);
			this.Controls.Add(this.LicenseBox);
			this.Controls.Add(this.btnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "License";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProfileSharp License Agreement";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.License_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void License_Load(object sender, System.EventArgs e)
		{
			LicenseBox.Text="PLEASE SEE THE LICENSE.TXT FILE ACCOMPANYING THIS SOFTWARE AT https://github.com/Bathla/ProfileSharp/blob/master/LICENSE.txt";

		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close(); 
		}
		
	}
}
