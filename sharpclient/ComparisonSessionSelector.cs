using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpClient
{
	/// <summary>
	/// Summary description for ComparisonSessionSelector.
	/// </summary>
	public class ComparisonSessionSelector : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbMemSession1;
		private System.Windows.Forms.ComboBox cbMemSession2;
		private System.Windows.Forms.Button btnStartCompare;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label numeratorlabel;
		private System.Windows.Forms.Label denominatorlabel;
		private System.Windows.Forms.Panel panel1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ComparisonSessionSelector()
		{
			//
			// Required for Windows Form Designer support
			//
			try
			{
				InitializeComponent();
			}
			catch{}
			this.cbMemSession1.SelectedIndexChanged+=new EventHandler(cbMemSession1_SelectedIndexChanged);  
			this.cbMemSession2.SelectedIndexChanged+=new EventHandler(cbMemSession2_SelectedIndexChanged); 

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
			this.cbMemSession1 = new System.Windows.Forms.ComboBox();
			this.cbMemSession2 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnStartCompare = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numeratorlabel = new System.Windows.Forms.Label();
			this.denominatorlabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbMemSession1
			// 
			this.cbMemSession1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMemSession1.Location = new System.Drawing.Point(8, 16);
			this.cbMemSession1.Name = "cbMemSession1";
			this.cbMemSession1.Size = new System.Drawing.Size(264, 21);
			this.cbMemSession1.TabIndex = 0;
			// 
			// cbMemSession2
			// 
			this.cbMemSession2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMemSession2.Location = new System.Drawing.Point(368, 16);
			this.cbMemSession2.Name = "cbMemSession2";
			this.cbMemSession2.Size = new System.Drawing.Size(248, 21);
			this.cbMemSession2.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Location = new System.Drawing.Point(280, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "compared to";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnStartCompare
			// 
			this.btnStartCompare.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.btnStartCompare.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnStartCompare.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.btnStartCompare.Location = new System.Drawing.Point(640, 16);
			this.btnStartCompare.Name = "btnStartCompare";
			this.btnStartCompare.Size = new System.Drawing.Size(72, 24);
			this.btnStartCompare.TabIndex = 3;
			this.btnStartCompare.Text = "Compare..";
			this.btnStartCompare.Click += new System.EventHandler(this.btnStartCompare_Click);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label2.Location = new System.Drawing.Point(128, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(464, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "\'Memory-Comparison Results\' show you the details of the objects that are in  :-";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(280, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "but not in";
			// 
			// numeratorlabel
			// 
			this.numeratorlabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.numeratorlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.numeratorlabel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.numeratorlabel.Location = new System.Drawing.Point(16, 32);
			this.numeratorlabel.Name = "numeratorlabel";
			this.numeratorlabel.Size = new System.Drawing.Size(248, 16);
			this.numeratorlabel.TabIndex = 6;
			this.numeratorlabel.Text = "Session1";
			this.numeratorlabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// denominatorlabel
			// 
			this.denominatorlabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.denominatorlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.denominatorlabel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.denominatorlabel.Location = new System.Drawing.Point(368, 32);
			this.denominatorlabel.Name = "denominatorlabel";
			this.denominatorlabel.Size = new System.Drawing.Size(240, 16);
			this.denominatorlabel.TabIndex = 7;
			this.denominatorlabel.Text = "Session2";
			this.denominatorlabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.cbMemSession1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.cbMemSession2);
			this.panel1.Controls.Add(this.btnStartCompare);
			this.panel1.Location = new System.Drawing.Point(0, 56);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(736, 56);
			this.panel1.TabIndex = 8;
			// 
			// ComparisonSessionSelector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ClientSize = new System.Drawing.Size(730, 114);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.denominatorlabel);
			this.Controls.Add(this.numeratorlabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ComparisonSessionSelector";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select memory sessions  to compare.";
			this.Load += new System.EventHandler(this.ComparisonSessionSelector_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ComparisonSessionSelector_Load(object sender, System.EventArgs e)
		{
			try
			{
				int memSessionCount=0;
				foreach (SharpClient.UI.Controls.TabPage tPage in SharpClientForm.scInstance.sharpClientMDITab.TabPages)
				{
					if(tPage.GetType()==typeof(SharpClientTabPage))
					{
						SharpClientTabPage memTabPage=tPage as SharpClientTabPage;
						if(memTabPage.IsMemorySession && memTabPage.Title!="Loading..." )
						{
							if(cbMemSession1.Items.Contains(memTabPage.Title) ||  cbMemSession2.Items.Contains(memTabPage.Title))
							{
								continue;
							}
							cbMemSession1.Items.Add(memTabPage.Title);
							cbMemSession2.Items.Add(memTabPage.Title); 
							++memSessionCount;
						}					
					}
				}	
				if(memSessionCount<2)
				{
					MessageBox.Show("You need to load atleast 2 memory sessions in the profiler first, to compare them.\nPlease open 2 or more memory sessions for analysis in the profiler and then retry.","Insufficient number of memory sessions." ,MessageBoxButtons.OK,MessageBoxIcon.Stop );
					this.Close(); 
				}
			
				try
				{
					cbMemSession1.SelectedIndex=cbMemSession1.Items.Count-1; 
					cbMemSession2.SelectedIndex=cbMemSession1.Items.Count-2; 
				}
				catch{}
				Application.DoEvents();
			}			
			catch
			{
				MessageBox.Show("An error has occured during enumeration of loaded memory sessiosn.\n Please retry. ","Error!." ,MessageBoxButtons.OK,MessageBoxIcon.Error );
				this.Close();
			}
			
		}

		private void btnStartCompare_Click(object sender, System.EventArgs e)
		{
			try
			{
				CompareTabPage compareTab=new CompareTabPage(Convert.ToString(cbMemSession1.SelectedItem).Split(new char[]{':'})[0],Convert.ToString(cbMemSession1.SelectedItem).Split(new char[]{':'})[1],Convert.ToString(cbMemSession2.SelectedItem).Split(new char[]{':'})[1] );
				compareTab.Dock =DockStyle.Fill ;
				Application.DoEvents(); 
				SharpClientForm.scInstance.sharpClientMDITab.TabPages.Add(compareTab);
				Application.DoEvents(); 
				SharpClientForm.scInstance.sharpClientMDITab.SelectedTab =compareTab ;
				compareTab.Show(); 
				Application.DoEvents(); 
				this.Close(); 
			}
			catch{}
		}

		private void cbMemSession1_SelectedIndexChanged(object sender, EventArgs e)
		{
			numeratorlabel.Text=cbMemSession1.Text;   
			Application.DoEvents();  
		}

		private void cbMemSession2_SelectedIndexChanged(object sender, EventArgs e)
		{
			denominatorlabel.Text=cbMemSession2.Text;   
			Application.DoEvents();
		}
	}
}
