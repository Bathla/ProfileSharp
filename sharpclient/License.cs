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
			LicenseBox.Text="LICENSE AGREEMENT\n\nDisclaimer\nTHIS SOFTWARE IS PROVIDED 'AS IS', WITHOUT A WARRENTY OF ANY KIND.\nSOFTPRODIGY DON'T TAKE ANY RESPONSIBILITY FOR ANY DAMAGES SUFFERED AS A RESULT OF USING,MODIFYING OR DISTRIBUTING THE SOFTWARE.\n IN NO EVENT WILL SOFTPRODIGY BE LIABLE FOR ANY LOST REVENUE,PROFIT OR DATA, \nOR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES,\nHOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY,\n ARISING OUT OF THE USE OF OR INABILITY TO USE THE SOFTWARE, EVEN IF SOFTPRODIGY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. \nTHE AUTHOR DOES NOT WARRANT THAT SOFTPRODIGY'S 'ProfileSharp' IS FREE FROM BUGS, ERRORS,\nOR OTHER PROGRAM LIMITATIONS.\n\nLIMITATION OF LIABILITY AND DAMAGES\n"
				+"TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THE AUTHOR IS NOT LIABLE FOR ANY INDIRECT,\nSPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO: DAMAGES FOR LOSS OF BUSINESS,\nLOSS OF PROFITS OR INVESTMENT, OR THE LIKE), WHETHER BASED ON BREACH OF CONTRACT, BREACH OF WARRANTY,\nTORT (INCLUDING NEGLIGENCE), PRODUCT LIABILITY OR OTHERWISE, EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE\nPOSSIBILITY OF SUCH DAMAGES, AND EVEN IF A REMEDY SET FORTH HEREIN IS FOUND TO HAVE FAILED OF ITS\nESSENTIAL PURPOSE. THE AUTHOR WILL NOT BE SUBJECT TO LIABILITY FOR ANY BUGS OR DAMAGES CAUSED\nBY ANY OF THE SOFTPRODIGY'S 'ProfileSharp' FEATURES.\n\nGrant of License\n"
+"Installation and Use\nYou are granted a right to install the SOFTWARE and use it in accordance with the terms of this EULA, in order to process your own applications with the SOFTWARE.\n\nTrial version: \nUse of SOFTWARE without purchase of a License shall be limited to evaluation purposes only. Redistribution of RUNTIME is strictly prohibited in all forms. Programs created using a trial license must be destroyed when the trial period has expired. Software compiled using a trial license cannot be bought, sold, licensed, copied, traded or otherwise used by other individuals. \n\nRegistered version: \nEach SOFTWARE License shall be assigned to one CUSTOMER at the time of purchase and is non-transferable to other individuals. All use of SOFTWARE must be preformed by said Licensed CUSTOMER. Use of SOFTWARE by other individuals is permitted only if said other individual has been licensed to use SOFTWARE.  \n"
+"Except as permitted above, Customer shall not, nor allow others to copy, in whole or in part, emulate, sub-licence, distribute, sell, transfer, exploit, alter, modify, adapt or translate the Software nor decompile, disassemble or reverse engineer the same nor attempt to do such thing. Redistribution of the RUNTIME by unlicensed 3rd parties is strictly prohibited.\n\nEND-USER LICENSE AGREEMENT FOR SoftProdigy's 'ProfileSharp'\n\nThis license agreement ('EULA') is a legal agreement between you, either an individual or a single entity, and SoftProdigy.com concerning the software identified above, which includes executable files and accompanying printed, on-line or other electronic documentation ('SOFTWARE'). The SOFTWARE also includes any updates and supplements to original SOFTWARE provided to you. \nBy installing, copying, running, or otherwise using the SOFTWARE, you agree to be bound by the terms of this license. If you do not agree to the terms of this license, you must remove all 'ProfileSharp' files from your computer. \n"
+"All copyrights to the SOFTWARE are exclusively owned by SoftProdigy.com. All rights not explicitly granted here are reserved by SoftProdigy.com. \nThank you for using the SOFTWARE in accordance with the terms of this EULA.";

		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close(); 
		}
		
	}
}
