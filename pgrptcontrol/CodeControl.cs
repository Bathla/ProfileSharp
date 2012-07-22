using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace PGRptControl
{
	/// <summary>
	/// Summary description for CodeControl.
	/// </summary>
	/// 

	public class CodeControl : System.Windows.Forms.UserControl
	{
		private const int WM_VSCROLL = 0x115;
		private const int SBS_VERT = 1;
		private const int SB_THUMBPOSITION = 4;

		private Subclass sClassCodeHighlighter;
		private Subclass sClassLinesRTB;		
		private Subclass sClassMilliSecondDisplayRTB;
		private DataTable srcDT;
		private string srcFile;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button LineTitle;
		private System.Windows.Forms.RichTextBox LinesRTB;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.RichTextBox MilliSecondDisplayRTB;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.RichTextBox CodeHighLighter;
		private Hashtable m_CharPosToLine;
		private UInt64 m_i64MaxHitCount;
		private decimal m_decMaxTimeConsumed;
		private bool isDirty;
		private int iFirstLine;	
		private StringBuilder srcRtf;
		private System.Windows.Forms.ToolTip saveRTFToolTip;
		private System.ComponentModel.IContainer components;

		public CodeControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			iFirstLine=0;
			m_CharPosToLine=new Hashtable();
			m_decMaxTimeConsumed=0;
			m_i64MaxHitCount=0;
			isDirty=false;
			sClassCodeHighlighter = new Subclass(CodeHighLighter.Handle);
			sClassCodeHighlighter.WindowProcedure += new PGRptControl.Subclass.WindowProcedureEventHandler(sClass_WindowProcedure);
			sClassLinesRTB = new Subclass(LinesRTB.Handle);			
			sClassLinesRTB.WindowProcedure += new PGRptControl.Subclass.WindowProcedureEventHandler(sClass_WindowProcedure);						

			sClassMilliSecondDisplayRTB= new Subclass(MilliSecondDisplayRTB.Handle);
			sClassMilliSecondDisplayRTB.WindowProcedure += new PGRptControl.Subclass.WindowProcedureEventHandler(sClass_WindowProcedure);
			
			LinesRTB.GotFocus+=new EventHandler(LinesRTB_GotFocus); 	
			MilliSecondDisplayRTB.GotFocus+=new EventHandler(LinesRTB_GotFocus); 						
			this.Resize+=new EventHandler(CodeControl_Resize); 	
			CodeHighLighter.VScroll +=new EventHandler(CodeHighLighter_VScroll);
			PreparePageEx();			

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(srcDT!=null)
			{
				srcDT.Clear(); 
				srcDT.Dispose();
				srcDT=null;
				m_CharPosToLine.Clear();
			}			
			if( disposing )
			{
				this.Events.Dispose();  
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.LineTitle = new System.Windows.Forms.Button();
			this.LinesRTB = new System.Windows.Forms.RichTextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.MilliSecondDisplayRTB = new System.Windows.Forms.RichTextBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.CodeHighLighter = new System.Windows.Forms.RichTextBox();
			this.saveRTFToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.LineTitle);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(912, 32);
			this.panel1.TabIndex = 0;
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button3.Location = new System.Drawing.Point(368, 0);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(544, 32);
			this.button3.TabIndex = 23;
			this.button3.Text = "Source";
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Location = new System.Drawing.Point(56, 0);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(128, 32);
			this.button2.TabIndex = 22;
			this.button2.Text = "Hit-Count";
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(184, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(184, 32);
			this.button1.TabIndex = 21;
			this.button1.Text = "Time consumed";
			// 
			// LineTitle
			// 
			this.LineTitle.Enabled = false;
			this.LineTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LineTitle.Location = new System.Drawing.Point(0, 0);
			this.LineTitle.Name = "LineTitle";
			this.LineTitle.Size = new System.Drawing.Size(56, 32);
			this.LineTitle.TabIndex = 20;
			this.LineTitle.Text = "Line No.";
			// 
			// LinesRTB
			// 
			this.LinesRTB.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.LinesRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.LinesRTB.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.LinesRTB.DetectUrls = false;
			this.LinesRTB.Dock = System.Windows.Forms.DockStyle.Left;
			this.LinesRTB.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.LinesRTB.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.LinesRTB.Location = new System.Drawing.Point(0, 32);
			this.LinesRTB.Name = "LinesRTB";
			this.LinesRTB.ReadOnly = true;
			this.LinesRTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
			this.LinesRTB.Size = new System.Drawing.Size(55, 512);
			this.LinesRTB.TabIndex = 7;
			this.LinesRTB.Text = "";
			this.saveRTFToolTip.SetToolTip(this.LinesRTB, "Press \'Ctrl+S\' to save and open with Wordpad to view.");
			this.LinesRTB.WordWrap = false;
			// 
			// splitter1
			// 
			this.splitter1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.splitter1.Location = new System.Drawing.Point(55, 32);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(1, 512);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// MilliSecondDisplayRTB
			// 
			this.MilliSecondDisplayRTB.AutoSize = true;
			this.MilliSecondDisplayRTB.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.MilliSecondDisplayRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.MilliSecondDisplayRTB.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.MilliSecondDisplayRTB.DetectUrls = false;
			this.MilliSecondDisplayRTB.Dock = System.Windows.Forms.DockStyle.Left;
			this.MilliSecondDisplayRTB.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.MilliSecondDisplayRTB.ForeColor = System.Drawing.SystemColors.WindowText;
			this.MilliSecondDisplayRTB.HideSelection = false;
			this.MilliSecondDisplayRTB.Location = new System.Drawing.Point(56, 32);
			this.MilliSecondDisplayRTB.Name = "MilliSecondDisplayRTB";
			this.MilliSecondDisplayRTB.ReadOnly = true;
			this.MilliSecondDisplayRTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
			this.MilliSecondDisplayRTB.Size = new System.Drawing.Size(310, 512);
			this.MilliSecondDisplayRTB.TabIndex = 22;
			this.MilliSecondDisplayRTB.Text = "";
			this.saveRTFToolTip.SetToolTip(this.MilliSecondDisplayRTB, "Press \'Ctrl+S\' to save and open with Wordpad to view.");
			this.MilliSecondDisplayRTB.WordWrap = false;
			// 
			// splitter2
			// 
			this.splitter2.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.splitter2.Location = new System.Drawing.Point(366, 32);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(1, 512);
			this.splitter2.TabIndex = 23;
			this.splitter2.TabStop = false;
			// 
			// CodeHighLighter
			// 
			this.CodeHighLighter.AutoSize = true;
			this.CodeHighLighter.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.CodeHighLighter.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.CodeHighLighter.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.CodeHighLighter.DetectUrls = false;
			this.CodeHighLighter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CodeHighLighter.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.CodeHighLighter.HideSelection = false;
			this.CodeHighLighter.Location = new System.Drawing.Point(367, 32);
			this.CodeHighLighter.Name = "CodeHighLighter";
			this.CodeHighLighter.ReadOnly = true;
			this.CodeHighLighter.Size = new System.Drawing.Size(545, 512);
			this.CodeHighLighter.TabIndex = 24;
			this.CodeHighLighter.Text = "";
			this.saveRTFToolTip.SetToolTip(this.CodeHighLighter, "Press \'Ctrl+S\' to save and open with Wordpad to view.");
			this.CodeHighLighter.WordWrap = false;
			this.CodeHighLighter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeHighLighter_KeyDown);
			// 
			// saveRTFToolTip
			// 
			this.saveRTFToolTip.AutoPopDelay = 6000;
			this.saveRTFToolTip.InitialDelay = 500;
			this.saveRTFToolTip.ReshowDelay = 100;
			this.saveRTFToolTip.ShowAlways = true;
			// 
			// CodeControl
			// 
			this.Controls.Add(this.CodeHighLighter);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.MilliSecondDisplayRTB);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.LinesRTB);
			this.Controls.Add(this.panel1);
			this.Name = "CodeControl";
			this.Size = new System.Drawing.Size(912, 544);
			this.saveRTFToolTip.SetToolTip(this, "Press \'Ctrl+S\' to save and open with Wordpad to view.");
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeHighLighter_KeyDown);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void PreparePageEx()
			{		
				preparePrivatePageEx();
									
			}

		public void SaveSourceDocument(string targetRTF)
		{
			if(srcRtf==null)
				throw new Exception("No source-code information available for saving."); 
			System.IO.StreamWriter rtfWriter=null;
			try
			{
				rtfWriter=new StreamWriter(targetRTF,false,System.Text.Encoding.ASCII);
				rtfWriter.Write(srcRtf);
			}
			finally
			{
				if(rtfWriter!=null)
				{
					rtfWriter.Close();
					rtfWriter=null;
				}
			}
		}

		private void preparePrivatePageEx()
		{
			if(!isDirty)
				return;			
				
			if(srcDT==null)
			{
				throw new Exception("Invalid data");
			}

			if(srcFile==null)
			{
				throw new Exception("Invalid File");
			}
				
			DataRow [] rowCollection=srcDT.Select("DebugCode.FileName='"+SourceFile+"' AND DebugCode.FileOffset <> 0 ","DebugCode.FileOffset",DataViewRowState.CurrentRows);			

			if(rowCollection.Length<1)
			{
				throw new Exception("There is no line of code in ["+srcFile+"] matching this criteria."); 
			}

			CodeHighLighter.Clear(); 
			MilliSecondDisplayRTB.Clear(); 

			ArrayList strSourceArray=new ArrayList(); 				
			m_CharPosToLine.Clear();			
			System.IO.StreamReader streamReader=new StreamReader(srcFile,System.Text.Encoding.ASCII);    
			string strLine=streamReader.ReadLine();			
			m_CharPosToLine[1]=0;
			int i=1;
			int nextOffset=0;
			while (strLine!=null)
			{
				LinesRTB.Text+=i.ToString()+"\n";
				++i;
				strSourceArray.Add(strLine.Replace("\\","\\\\") +"\n");					
				nextOffset+=strLine.Length+1; 
				m_CharPosToLine[i]=nextOffset;
				strLine=streamReader.ReadLine();
			}
			LinesRTB.Text+=i.ToString()+"\n";
				
			streamReader.Close();
			streamReader=null;

			ArrayList strHCArray=new ArrayList(); 
			ArrayList strTCArray=new ArrayList();	


			if(srcRtf==null)
			{
				srcRtf=new StringBuilder(@"{\rtf1\ansi\ {\colortbl ;\red255\green255\blue0;\red255\green0\blue0;}"); 				
			}

			foreach (DataRow row in rowCollection)
			{
				try
				{
					
					int lineNumber=Convert.ToInt32(row["DebugCode.FileOffset"])-1;
					if(lineNumber<0)
					{
						continue;
					}

					if(iFirstLine==0)
					{
						iFirstLine=lineNumber+1;
					}

					string selectedText=strSourceArray[lineNumber].ToString(); 				
					selectedText=selectedText.Replace("{","\\{");
					selectedText=selectedText.Replace("}","\\}");
					selectedText=selectedText.Replace("\n","\\par ");

					selectedText="{\\Highlight1"+selectedText+"}";
					strSourceArray[lineNumber]=selectedText;

					strHCArray.Add(Convert.ToUInt64(row["Hit Count"]));					
					UInt64 i64CollectiveTime=Convert.ToUInt64(row["CollectiveTime"]);
					Decimal iTCPercent=0;
					if(i64CollectiveTime!=0)
					{					
						decimal fraction=(Convert.ToDecimal(row["Time Consumed"]))/ ((Convert.ToDecimal(i64CollectiveTime)));
						fraction*=100;
						iTCPercent=Decimal.Round(fraction,2);						
					}
					strTCArray.Add(iTCPercent);
				}			
				catch(Exception ex)
				{
					if(ex.GetType()==typeof(ArgumentOutOfRangeException))
					{
						continue;
					}					
					if(MessageBox.Show("An exception occured while retrieving the source code for the file.\n Do you want to continue any way?","Exception!",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
					{
						continue;
					}
					else
					{
						throw ex;
					}
				}
			}
			
			StringBuilder strBuilder=new StringBuilder(null); 
			StringBuilder strInfoBuilder=new StringBuilder(null); 
			int iInfoCount=0;	
			
			for(int x=0;x<strSourceArray.Count;x++)
			{						
				string strToAdd=strSourceArray[x].ToString(); 
				if(!strToAdd.StartsWith("{\\Highlight1" ))
				{
					strToAdd=strToAdd.Replace("{","\\{");
					strToAdd=strToAdd.Replace("}","\\}");
					strToAdd=strToAdd.Replace("\n","\\par ");
					srcRtf.Append("\\tab \\tab ");														
				}
				else
				{
					try
					{
						Decimal iTC=Convert.ToDecimal(strTCArray[iInfoCount]);
						string spaceString="";
						for (int j=0;j<=Convert.ToInt32(Decimal.Round(iTC,0)/10);j++)
						{
							spaceString+=" ";
						}
						string infoString=@"\u9689? {\Highlight1 "+Convert.ToString(strHCArray[iInfoCount]).PadRight(16)+@"} \u9658?{\Highlight1 "+(Convert.ToString(iTC)+"%").PadRight(7)+@"} {\Highlight2"+spaceString+"}" ;
						strInfoBuilder.Append(infoString);
						srcRtf.Append(infoString);
						++iInfoCount;
					}
					catch
					{
					}
				}			
				
				strToAdd=strToAdd.Replace("\t","\\tab ");				
				strBuilder.Append(strToAdd);
				srcRtf.Append(strToAdd.Replace("\\par ",""));  
				strInfoBuilder.Append("\\par ");
				srcRtf.Append("\\par ");
				
			}
			strInfoBuilder.Append("\\par "); 

			strSourceArray.Clear(); 
			strTCArray.Clear();
			strHCArray.Clear();								
			CodeHighLighter.Rtf =@"{\rtf1\ansi\ {\colortbl ;\red255\green255\blue0;}"+strBuilder.ToString(); 				
			MilliSecondDisplayRTB.Rtf= @"{\rtf1\ansi\ {\colortbl ;\red255\green255\blue0;\red255\green0\blue0;}"+strInfoBuilder.ToString();
			MilliSecondDisplayRTB.Show();
			CodeHighLighter.Show(); 
			Application .DoEvents(); 

			isDirty=false;	
		}
		

		private string MakePrettyPercent(string selectedText)
		{
			return @"{\rtf1\ansi\ {\colortbl ;\red255\green0\blue0;}{\Highlight1 "+selectedText+@"}";
		}

		private string MakePretty(string selectedText)
		{
			return @"{\rtf1\ansi\ {\colortbl;\red255\green255\blue0;}{\Highlight1 "+selectedText+"}";
		}
		
		
		private string MakePrettyIcon(string selectedText)
		{
			return @"{\rtf1\ansi {\fonttbl{\f0\fswiss\fprq2\fcharset178 Arial;}{\f1\fmodern\fprq1\fcharset0 Courier;}{\f2\fswiss\fprq2\fcharset0 Arial;}}{\colortbl ;\red0\green0\blue0;\red255\green255\blue0;}\viewkind4\uc1\pard\ltrpar\nowidctlpar\tx4140\ul\f0\rtlch\fs18\u1758?\ulnone\f1\ltrch\fs26  \cf1\highlight2\fs18 "+selectedText+"} ";
		}
			
		public void ScrollToLine(int lineNumber)
		{
			CodeHighLighter.Select(Convert.ToInt32(m_CharPosToLine[lineNumber]),0);						
		}
			
		private void sClass_WindowProcedure (ref Message uMsg)
		{
			if(uMsg.Msg == WM_VSCROLL &&  uMsg.HWnd.Equals(CodeHighLighter.Handle))// WM_VSCROLL Message's for RTB's
			{			

				Message msgLinesRTB;
				msgLinesRTB = System.Windows.Forms.Message.Create(LinesRTB.Handle, uMsg.Msg, uMsg.WParam, uMsg.LParam);
				sClassLinesRTB.SendWndProc(ref msgLinesRTB);							
				
				Message msgMilliSecondDisplayRTB;
				msgMilliSecondDisplayRTB = System.Windows.Forms.Message.Create(MilliSecondDisplayRTB.Handle, uMsg.Msg, uMsg.WParam, uMsg.LParam);
				sClassMilliSecondDisplayRTB.SendWndProc(ref msgMilliSecondDisplayRTB);											
			}		
			
		}		
		
		
		
		private void CodeHighLighter_VScroll (object sender, System.EventArgs e)
		{			
			int RTBScrollPos;			
			RTBScrollPos = GetScrollPos(CodeHighLighter.Handle, SBS_VERT);
			PostMessageA(LinesRTB.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * RTBScrollPos, 0);
			PostMessageA(MilliSecondDisplayRTB.Handle, WM_VSCROLL,SB_THUMBPOSITION + 0x10000 * RTBScrollPos, 0);			
		}

		
		[DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Ansi, SetLastError=true)]
		private static extern int GetScrollPos(IntPtr hWnd, int nBar);
		
		[DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Ansi, SetLastError=true)]
		private static extern bool PostMessageA(IntPtr hwnd, int wMsg, int wParam, int lParam);

		private void LinesRTB_GotFocus(object sender, EventArgs e)
		{			
			CodeHighLighter.Focus();  
		}
			
		public DataTable SourceTable
		{
			get
			{
				return this.srcDT ;
			}
			set
			{
				if(this.srcDT!=null)
				{
					try
					{
						srcDT.Clear();
						srcDT.Dispose();
					}
					catch{}
					srcDT=null;
				}

				this.srcDT=value; 				
				isDirty=true;
			}
		}

		public string SourceFile
		{
			get
			{
				return this.srcFile ;
			}
			set
			{
				this.srcFile=value; 
				isDirty=true;
			}
		}

		public UInt64 ThreshholdHitCount
		{
			get
			{
				return m_i64MaxHitCount ;
			}
			set
			{
				m_i64MaxHitCount=value;
				isDirty=true;
			}
		}

		public decimal ThreshholdPercentTimeConsumed
		{
			get
			{
				return m_decMaxTimeConsumed;
			}
			set
			{
				m_decMaxTimeConsumed=value;
				isDirty=true;
			}
		}

		private void CodeControl_Resize(object sender, EventArgs e)
		{
			button3.Width =CodeHighLighter.Width ; 
		}

		private void CodeHighLighter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			try

			{
				if(e.Control)
				{
					if(e.KeyValue==83 || e.KeyValue==115 )
					{
						SaveFileDialog saveRtfDlg=new SaveFileDialog();
						saveRtfDlg.Title="Save Source-Code Performance Data"; 
						saveRtfDlg.RestoreDirectory=true;
						saveRtfDlg.Filter ="Rich-Text File(*.rtf)|*.rtf";
						saveRtfDlg.OverwritePrompt=true;						
						string rtfFile="Code-Performance";
						try
						{
							rtfFile=srcFile.Substring(srcFile.LastIndexOf ("\\")+1); 
						}
						catch{}
						saveRtfDlg.FileName=rtfFile+".rtf";
						if(saveRtfDlg.ShowDialog()==DialogResult.OK)
						{
							SaveSourceDocument(saveRtfDlg.FileName); 						
						}
						else throw new Exception("The action was cancelled by the user."); 
					}				
				}	
			}
			catch(Exception ex)
			{
				MessageBox.Show("No performance data saved.\n"+ex.Message,"Document not saved."  ,MessageBoxButtons.OK,MessageBoxIcon.Error);  
			}
		}			
		
	}


	public class Subclass : System.Windows.Forms.NativeWindow
	{
		//===================================================================
		// NativeWindow Subclassing
		//===================================================================
		public delegate void WindowProcedureEventHandler(ref Message uMsg);
		private WindowProcedureEventHandler WindowProcedureEvent;
		
		public event WindowProcedureEventHandler WindowProcedure
		{
			add
			{
				WindowProcedureEvent = (WindowProcedureEventHandler) System.Delegate.Combine(WindowProcedureEvent, value);
			}
			remove
			{
				WindowProcedureEvent = (WindowProcedureEventHandler) System.Delegate.Remove(WindowProcedureEvent, value);
			}
		}
		
		
		public Subclass(IntPtr pWindowHandle) 
		{
			base.AssignHandle(pWindowHandle);
		}
		
		protected override void WndProc (ref System.Windows.Forms.Message uMsg)
		{
			base.WndProc(ref uMsg);
			if (WindowProcedureEvent != null)
				WindowProcedureEvent(ref uMsg);
		}
		
		public void SendWndProc (ref System.Windows.Forms.Message uMsg)
		{
			System.Windows.Forms.Message temp_SystemWindowsFormsMessage = uMsg;
			base.WndProc(ref temp_SystemWindowsFormsMessage);
		}
		
	}

}
