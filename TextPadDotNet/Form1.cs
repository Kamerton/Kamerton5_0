using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Configuration;

namespace Notepad.NET
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private FlickerFreeTextBox txtPad;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuOpen;
		private System.Windows.Forms.MenuItem mnuPrint;
		private System.Windows.Forms.MenuItem mnuPrintPreview;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mnuFind;
		private System.Windows.Forms.MenuItem mnuReplace;
		private System.Windows.Forms.TabControl tbDocuments;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuWordWrap;
		private System.Windows.Forms.MenuItem mnuSetFont;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ArrayList  _documents  = new ArrayList();
		private VirtualFile _currentFile = null;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
		private System.Windows.Forms.PrintDialog printDialog1;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private System.Windows.Forms.MenuItem mnuNew;
		private System.Windows.Forms.MenuItem mnuClose;
		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.MenuItem mnuSave;
		private System.Windows.Forms.MenuItem mnuCloseAll;
		private System.Windows.Forms.MenuItem mnuSaveAll;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mnuCopy;
		private System.Windows.Forms.MenuItem mnuCut;
		private System.Windows.Forms.MenuItem mnuDelete;
		private System.Windows.Forms.MenuItem mnuPaste;
		private System.Windows.Forms.MenuItem mnuInsertTime;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem mnuSelectAll;
		private System.Windows.Forms.MenuItem mnuUndo;
		private System.Windows.Forms.MenuItem mnuFindNext;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private System.Windows.Forms.MenuItem mnuPageSetup;
		private System.Windows.Forms.MenuItem mnuSelectFind;
		private int			_documentCount = 0;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ConfigHandler.LoadConfigPath();			
			_currentFile = new VirtualFile(this);				
			_documents.Add(_currentFile);
			_documentCount++;
			LoadConfigurationFile();

			_textBoxPrinter = new TextBoxPrinter();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			mRecent.Text = "Recent Files";
			menuItem1.MenuItems.Add(10, mRecent); 

			mRecent.InitializeFromConfig(4);
			mRecent.MRUClicked+=new EventHandler(MRUClick);
			mRecent.MRUChanged+=new EventHandler(MRUChanged);
		}

		void LoadConfigurationFile()
		{
			txtPad.WordWrap = ConfigHandler.GetConfigSettingBool("wordwrap");
			mnuWordWrap.Checked = txtPad.WordWrap;
			VirtualFile.MAX_BUFFER_LENGTH = ConfigHandler.GetConfigSettingInt("maximumbuffer");
			txtPad.Font = new Font(ConfigHandler.GetConfigSetting("font"), ConfigHandler.GetConfigSettingFloat("fontsize"));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.txtPad = new Notepad.NET.FlickerFreeTextBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuNew = new System.Windows.Forms.MenuItem();
			this.mnuOpen = new System.Windows.Forms.MenuItem();
			this.mnuClose = new System.Windows.Forms.MenuItem();
			this.mnuCloseAll = new System.Windows.Forms.MenuItem();
			this.mnuSave = new System.Windows.Forms.MenuItem();
			this.mnuSaveAll = new System.Windows.Forms.MenuItem();
			this.mnuPageSetup = new System.Windows.Forms.MenuItem();
			this.mnuPrint = new System.Windows.Forms.MenuItem();
			this.mnuPrintPreview = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mnuUndo = new System.Windows.Forms.MenuItem();
			this.mnuCopy = new System.Windows.Forms.MenuItem();
			this.mnuCut = new System.Windows.Forms.MenuItem();
			this.mnuPaste = new System.Windows.Forms.MenuItem();
			this.mnuDelete = new System.Windows.Forms.MenuItem();
			this.mnuInsertTime = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.mnuSelectAll = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mnuFind = new System.Windows.Forms.MenuItem();
			this.mnuFindNext = new System.Windows.Forms.MenuItem();
			this.mnuReplace = new System.Windows.Forms.MenuItem();
			this.mnuSelectFind = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuWordWrap = new System.Windows.Forms.MenuItem();
			this.mnuSetFont = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnuAbout = new System.Windows.Forms.MenuItem();
			this.tbDocuments = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
			this.printDocument1 = new System.Drawing.Printing.PrintDocument();
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
			this.tbDocuments.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtPad
			// 
			this.txtPad.HideSelection = false;
			this.txtPad.Location = new System.Drawing.Point(-104, -80);
			this.txtPad.MaxLength = 1000000;
			this.txtPad.Multiline = true;
			this.txtPad.Name = "txtPad";
			this.txtPad.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtPad.Size = new System.Drawing.Size(664, 328);
			this.txtPad.TabIndex = 0;
			this.txtPad.Text = "";
			this.txtPad.WordWrap = false;
			this.txtPad.DoubleClick += new System.EventHandler(this.txtPad_DoubleClick);
			this.txtPad.TextChanged += new System.EventHandler(this.txtPad_TextChanged);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem6,
																					  this.menuItem2,
																					  this.menuItem3,
																					  this.menuItem4});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuNew,
																					  this.mnuOpen,
																					  this.mnuClose,
																					  this.mnuCloseAll,
																					  this.mnuSave,
																					  this.mnuSaveAll,
																					  this.mnuPageSetup,
																					  this.mnuPrint,
																					  this.mnuPrintPreview,
																					  this.menuItem8,
																					  this.menuItem5,
																					  this.mnuExit});
			this.menuItem1.Text = "File";
			// 
			// mnuNew
			// 
			this.mnuNew.Index = 0;
			this.mnuNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.mnuNew.Text = "New";
			this.mnuNew.Click += new System.EventHandler(this.menuNew_Click);
			// 
			// mnuOpen
			// 
			this.mnuOpen.Index = 1;
			this.mnuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mnuOpen.Text = "Open...";
			this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
			// 
			// mnuClose
			// 
			this.mnuClose.Index = 2;
			this.mnuClose.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.mnuClose.Text = "Close";
			this.mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
			// 
			// mnuCloseAll
			// 
			this.mnuCloseAll.Index = 3;
			this.mnuCloseAll.Text = "Close All";
			this.mnuCloseAll.Click += new System.EventHandler(this.mnuCloseAll_Click);
			// 
			// mnuSave
			// 
			this.mnuSave.Index = 4;
			this.mnuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuSave.Text = "Save...";
			// 
			// mnuSaveAll
			// 
			this.mnuSaveAll.Index = 5;
			this.mnuSaveAll.Text = "Save All";
			this.mnuSaveAll.Click += new System.EventHandler(this.mnuSaveAll_Click);
			// 
			// mnuPageSetup
			// 
			this.mnuPageSetup.Index = 6;
			this.mnuPageSetup.Text = "Page Setup...";
			this.mnuPageSetup.Click += new System.EventHandler(this.mnuPageSetup_Click);
			// 
			// mnuPrint
			// 
			this.mnuPrint.Index = 7;
			this.mnuPrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.mnuPrint.Text = "Print...";
			this.mnuPrint.Click += new System.EventHandler(this.mnuPrint_Click);
			// 
			// mnuPrintPreview
			// 
			this.mnuPrintPreview.Index = 8;
			this.mnuPrintPreview.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.mnuPrintPreview.Text = "Print Preview...";
			this.mnuPrintPreview.Click += new System.EventHandler(this.mnuPrintPreview_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 9;
			this.menuItem8.Text = "-";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 10;
			this.menuItem5.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 11;
			this.mnuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.mnuExit.Text = "Exit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuUndo,
																					  this.mnuCopy,
																					  this.mnuCut,
																					  this.mnuPaste,
																					  this.mnuDelete,
																					  this.mnuInsertTime,
																					  this.menuItem12,
																					  this.mnuSelectAll});
			this.menuItem6.Text = "Edit";
			// 
			// mnuUndo
			// 
			this.mnuUndo.Index = 0;
			this.mnuUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.mnuUndo.Text = "Undo";
			this.mnuUndo.Click += new System.EventHandler(this.mnuUndo_Click);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Index = 1;
			this.mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.mnuCopy.Text = "Copy";
			this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
			// 
			// mnuCut
			// 
			this.mnuCut.Index = 2;
			this.mnuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.mnuCut.Text = "Cut";
			this.mnuCut.Click += new System.EventHandler(this.mnuCut_Click);
			// 
			// mnuPaste
			// 
			this.mnuPaste.Index = 3;
			this.mnuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.mnuPaste.Text = "Paste";
			this.mnuPaste.Click += new System.EventHandler(this.mnuPaste_Click);
			// 
			// mnuDelete
			// 
			this.mnuDelete.Index = 4;
			this.mnuDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.mnuDelete.Text = "Delete";
			this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
			// 
			// mnuInsertTime
			// 
			this.mnuInsertTime.Index = 5;
			this.mnuInsertTime.Text = "Insert Date/Time";
			this.mnuInsertTime.Click += new System.EventHandler(this.mnuInsertTime_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 6;
			this.menuItem12.Text = "-";
			// 
			// mnuSelectAll
			// 
			this.mnuSelectAll.Index = 7;
			this.mnuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.mnuSelectAll.Text = "Select All";
			this.mnuSelectAll.Click += new System.EventHandler(this.mnuSelectAll_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 2;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFind,
																					  this.mnuFindNext,
																					  this.mnuReplace,
																					  this.mnuSelectFind});
			this.menuItem2.Text = "Search";
			// 
			// mnuFind
			// 
			this.mnuFind.Index = 0;
			this.mnuFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.mnuFind.Text = "Find...";
			this.mnuFind.Click += new System.EventHandler(this.mnuFind_Click);
			// 
			// mnuFindNext
			// 
			this.mnuFindNext.Index = 1;
			this.mnuFindNext.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.mnuFindNext.Text = "Find Next...";
			this.mnuFindNext.Click += new System.EventHandler(this.mnuFindNext_Click);
			// 
			// mnuReplace
			// 
			this.mnuReplace.Index = 2;
			this.mnuReplace.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.mnuReplace.Text = "Replace...";
			this.mnuReplace.Click += new System.EventHandler(this.mnuReplace_Click);
			// 
			// mnuSelectFind
			// 
			this.mnuSelectFind.Index = 3;
			this.mnuSelectFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF3;
			this.mnuSelectFind.Text = "Select and Find";
			this.mnuSelectFind.Visible = false;
			this.mnuSelectFind.Click += new System.EventHandler(this.mnuSelectFind_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 3;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuWordWrap,
																					  this.mnuSetFont});
			this.menuItem3.Text = "Settings";
			// 
			// mnuWordWrap
			// 
			this.mnuWordWrap.Index = 0;
			this.mnuWordWrap.Text = "Word Wrap";
			this.mnuWordWrap.Click += new System.EventHandler(this.mnuWordWrap_Click);
			// 
			// mnuSetFont
			// 
			this.mnuSetFont.Index = 1;
			this.mnuSetFont.Text = "Set Font";
			this.mnuSetFont.Click += new System.EventHandler(this.mnuSetFont_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuAbout});
			this.menuItem4.Text = "Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Index = 0;
			this.mnuAbout.Text = "About...";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// tbDocuments
			// 
			this.tbDocuments.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tbDocuments.Controls.Add(this.tabPage1);
			this.tbDocuments.Location = new System.Drawing.Point(0, 8);
			this.tbDocuments.Name = "tbDocuments";
			this.tbDocuments.SelectedIndex = 0;
			this.tbDocuments.Size = new System.Drawing.Size(576, 280);
			this.tbDocuments.TabIndex = 0;
			this.tbDocuments.TabIndexChanged += new System.EventHandler(this.tbDocuments_TabIndexChanged);
			this.tbDocuments.SelectedIndexChanged += new System.EventHandler(this.tbDocuments_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.txtPad);
			this.tabPage1.Location = new System.Drawing.Point(4, 4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(568, 254);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Document1";
			// 
			// printPreviewDialog1
			// 
			this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDialog1.Document = this.printDocument1;
			this.printPreviewDialog1.Enabled = true;
			this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
			this.printPreviewDialog1.Location = new System.Drawing.Point(384, 17);
			this.printPreviewDialog1.MinimumSize = new System.Drawing.Size(375, 250);
			this.printPreviewDialog1.Name = "printPreviewDialog1";
			this.printPreviewDialog1.TransparencyKey = System.Drawing.Color.Empty;
			this.printPreviewDialog1.Visible = false;
			// 
			// printDocument1
			// 
			this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
			// 
			// printDialog1
			// 
			this.printDialog1.Document = this.printDocument1;
			// 
			// pageSetupDialog1
			// 
			this.pageSetupDialog1.Document = this.printDocument1;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 313);
			this.Controls.Add(this.tbDocuments);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Notepad.NET";
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.tbDocuments.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			tbDocuments.SetBounds(0, 0, ClientRectangle.Width, ClientRectangle.Height);
			txtPad.SetBounds(tbDocuments.Left, tbDocuments.Top, tbDocuments.Width - 10, tbDocuments.Height - 30);
		}

		void MoveTextSurface(int index)
		{
				tbDocuments.TabPages[tbDocuments.TabIndex].Controls.Remove(txtPad);
				tbDocuments.TabIndex = index;
				tbDocuments.TabPages[tbDocuments.TabIndex].Controls.Add(txtPad);
				tbDocuments.SelectedIndex = index;
			    _currentFile = _documents[index] as VirtualFile;
		}

		void SetSurface(VirtualFile file)
		{
			txtPad.Text = file.GetCurrentText();	
		}

		int GetExistingFile(string filename)
		{
			// loop through all files and see if it exists
			for (int i = 0; i < _documents.Count; i++)
			{
				VirtualFile nextFile = _documents[i] as VirtualFile;
				if (filename == nextFile.FileName)
				{
					return i;
				}
			}

			return -1;
		}

		private void OpenFile(string filename)
		{
			// if it all ready exists, just go to it
			int index = GetExistingFile(filename);
			if (index != -1)
			{
				_userClicked = false;
				MoveTextSurface(index);
				SetSurface(_currentFile);
				return;
			}

			_currentFile.SetCurrentText(txtPad.Text);
			_documentCount++;
			_currentFile = new VirtualFile(filename, this);				
			if (_currentFile.CutOff)
			{
				MessageBox.Show(String.Format("File {1} was cut-off, because it was greater than {0} characters (the maximum buffer size)", VirtualFile.MAX_BUFFER_LENGTH, filename));
			}
			_documents.Add(_currentFile);
			// create a new tab
			tbDocuments.TabPages.Add(new TabPage(StripPath(filename)));
			// move text pad
			_userClicked = false;
			MoveTextSurface(_documentCount-1);

			SetSurface(_currentFile);
		}

		private void mnuOpen_Click(object sender, System.EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				mRecent.FileOpened(openFileDialog1.FileName);
				OpenFile(openFileDialog1.FileName);
			}
		}

		private void mnuPrint_Click(object sender, System.EventArgs e)
		{
			if (printDialog1.ShowDialog() == DialogResult.OK)
			{
				printDocument1.Print();
			}
		}

		/// <summary>
		/// Preview document for printing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuPrintPreview_Click(object sender, System.EventArgs e)
		{
			printPreviewDialog1.ShowDialog();
		}

		private void mnuFind_Click(object sender, System.EventArgs e)
		{
			FindDialog findDialog = new FindDialog(txtPad, false);
			findDialog.Show();
		}

		/// <summary>
		/// Exit App
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			CloseAllDocuments();
			mRecent.WriteToConfig();
			Application.Exit();
		}


		private void mnuReplace_Click(object sender, System.EventArgs e)
		{
			FindDialog findDialog = new FindDialog(txtPad, true);
			findDialog.Show();
		}

		private void tbDocuments_TabIndexChanged(object sender, System.EventArgs e)
		{
		}

		private void SaveTextSurface(int index)
		{
			if (index >= _documents.Count)
			{
				Console.WriteLine("Error: Index out of range in SaveTextSurface");
				return; // precondition
			}

			(_documents[index] as VirtualFile).SetCurrentText(txtPad.Text);
		}

		bool _userClicked = true;
		bool _ignoreModification = false;
		private void tbDocuments_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_userClicked)
			{
				SaveTextSurface(tbDocuments.TabIndex);
			}
			else
			{
				_userClicked = true;
			}

				_ignoreModification = true;
				MoveTextSurface(tbDocuments.SelectedIndex);
				SetSurface(_documents[tbDocuments.SelectedIndex] as VirtualFile);
				tbDocuments.TabIndex = tbDocuments.SelectedIndex;
		}

		string StripPath(string path)
		{
			return path.Substring(path.LastIndexOf("\\") + 1);
		}

		/// <summary>
		/// Saves the current file
		/// </summary>
		private void SaveCurrentFile()
		{
			_currentFile = _documents[tbDocuments.TabIndex] as VirtualFile;
			if (_currentFile.FileName.Length == 0)
			{
				saveFileDialog1.Title = "Save " + tbDocuments.TabPages[tbDocuments.TabIndex].Text + " As";
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					SaveTextSurface(tbDocuments.TabIndex);
					_currentFile.WriteFile(saveFileDialog1.FileName);
					tbDocuments.TabPages[tbDocuments.TabIndex].Text = StripPath(saveFileDialog1.FileName);
				}
			}
			else
			{
				SaveTextSurface(tbDocuments.TabIndex);
				_currentFile.WriteFile((_documents[tbDocuments.TabIndex] as VirtualFile).FileName);
			}
		}

		/// <summary>
		/// Saves the current file
		/// </summary>
		private void SaveFile(int index)
		{
			_currentFile = _documents[index] as VirtualFile;
			if (_currentFile.FileName.Length == 0)
			{
				saveFileDialog1.Title = "Save " + tbDocuments.TabPages[index].Text + " As";
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					_currentFile.WriteFile(saveFileDialog1.FileName);
					tbDocuments.TabPages[index].Text = saveFileDialog1.FileName;
				}
			}
			else
			{
				_currentFile.WriteFile((_documents[TabIndex] as VirtualFile).FileName);
			}
		}


		private void mnuSave_Click(object sender, System.EventArgs e)
		{
			SaveCurrentFile();
		}

		TextBoxPrinter _textBoxPrinter;
		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			e.HasMorePages = _textBoxPrinter.Print(e.Graphics, txtPad, printDocument1, _screenResolutionX);
		}

		private float _screenResolutionX = 0;
		private float _screenResolutionY = 0;
		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// get the resolution here
			_screenResolutionX = e.Graphics.DpiX;
			_screenResolutionY = e.Graphics.DpiY;

			// set the last position of the text box
			_textBoxPrinter.LastPosition = (int)_screenResolutionX;
		}

		private int _newFileCount = 1;

		private void menuNew_Click(object sender, System.EventArgs e)
		{
			_currentFile.SetCurrentText(txtPad.Text);
			_documentCount++;
			_newFileCount++;
			_currentFile = new VirtualFile(this);			
			_documents.Add(_currentFile);
			// create a new tab
			tbDocuments.TabPages.Add(new TabPage("Document" + _newFileCount));
			// move text pad
			_userClicked = false;
			MoveTextSurface(_documentCount-1);

			SetSurface(_currentFile);
		
		}

		private void CloseFile()
		{
			// don't close if its the last tab
			if (_documents.Count == 1)
				return; // precondition
	
			// Move the surface back one
			int currentSurface = tbDocuments.TabIndex;
			int nextSurface = ((tbDocuments.TabIndex - 1) + tbDocuments.TabPages.Count) % tbDocuments.TabPages.Count;
			_userClicked = false;
			MoveTextSurface(nextSurface);
			SetSurface(_documents[nextSurface] as VirtualFile);
			tbDocuments.TabIndex = nextSurface;

			// remove the current tab and the document
			_documents.RemoveAt(currentSurface);
			tbDocuments.TabPages.RemoveAt(currentSurface);
			_documentCount--;
		}

		/// <summary>
		/// Saves current file if user chooses to 
		/// </summary>
		/// <returns>true if saved</returns>
		bool TestIfFileIsModifiedAndSave()
		{
			if  (_currentFile.Modified)
			{
				if (MessageBox.Show(String.Format("Would You Like To Save the Current File {0} Before Closing?", _currentFile.FileName), "Query", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					SaveCurrentFile();
					return true;
				}
				else
				{
					return false;
				}
			}

			return true;

		}

		/// <summary>
		/// Tricky. On the close you need to remove the tab
		/// and move the text pad somewhere else (previous window)
		/// if there is no previous window, don't allow a close
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuClose_Click(object sender, System.EventArgs e)
		{
			TestIfFileIsModifiedAndSave();

			// close the file
			CloseFile();
		}

		private void txtPad_TextChanged(object sender, System.EventArgs e)
		{
			// if any text changed, set modify bit of current file
			if (_ignoreModification == false)
			{
				_currentFile.Modified = true;
			}
			else
			{
				_ignoreModification = false;
			}
		}

		private void mnuWordWrap_Click(object sender, System.EventArgs e)
		{
			if (txtPad.WordWrap)
			{
				txtPad.WordWrap = false;
				ConfigHandler.replaceConfigSettings ("wordwrap", "false");
				mnuWordWrap.Checked = false;
			}
			else
			{
				txtPad.WordWrap = true;
				ConfigHandler.replaceConfigSettings ("wordwrap", "true");
				mnuWordWrap.Checked = true;
			}
		}

		private void mnuSetFont_Click(object sender, System.EventArgs e)
		{
			fontDialog1.Font = txtPad.Font;
			if (fontDialog1.ShowDialog() == DialogResult.OK)
			{
				txtPad.Font = fontDialog1.Font;
				ConfigHandler.replaceConfigSettings("font", txtPad.Font.FontFamily.Name);
				ConfigHandler.replaceConfigSettings("fontsize", txtPad.Font.Size.ToString());
			}
		}

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			AboutBox dlg = new AboutBox();
			dlg.ShowDialog();
		}

		private void txtPad_DoubleClick(object sender, System.EventArgs e)
		{
			FlickerFreeTextBox._Paint = false;
			while ((txtPad.Text.Substring(txtPad.SelectionStart,1) == " ") &&
		(txtPad.SelectionStart < txtPad.Text.Length))
			{
				txtPad.SelectionStart++;
			}

			int count = 0;
			while ((txtPad.Text.Substring(txtPad.SelectionStart + txtPad.SelectionLength -(count + 1), 1) == " ") &&
				(txtPad.SelectionLength > 0))
			{
				count++;
			}

			txtPad.SelectionLength-= count;


			FlickerFreeTextBox._Paint = true;


		}


		private void CloseAllDocuments()
		{
			while (_documents.Count > 1)
			{
				_currentFile = _documents[tbDocuments.TabIndex] as VirtualFile;
				bool saved = TestIfFileIsModifiedAndSave();
				CloseFile();
			}
		}

		private void SaveAllDocuments()
		{
			int oldIndex = tbDocuments.TabIndex;
			SaveCurrentFile();
			for (int i = 0; i < tbDocuments.TabPages.Count; i++)
			{
				SaveFile(i);
			}

			tbDocuments.TabIndex = oldIndex;
			tbDocuments.SelectedIndex = oldIndex;
		}



		private void mnuCloseAll_Click(object sender, System.EventArgs e)
		{
			// closes all the unmodified documents
			CloseAllDocuments();
		}

		private void mnuSaveAll_Click(object sender, System.EventArgs e)
		{
			SaveAllDocuments();
		}

		private void mnuFindNext_Click(object sender, System.EventArgs e)
		{
			FindDialog.FindNext(txtPad);
		}

		private void mnuUndo_Click(object sender, System.EventArgs e)
		{
			txtPad.Undo();
		}

		private void mnuCopy_Click(object sender, System.EventArgs e)
		{
			txtPad.Copy();
		}

		private void mnuCut_Click(object sender, System.EventArgs e)
		{
			txtPad.Cut();
		}

		private void mnuPaste_Click(object sender, System.EventArgs e)
		{
			txtPad.Paste();
		}

		private void mnuDelete_Click(object sender, System.EventArgs e)
		{
			txtPad.Cut();
		}

		private void mnuInsertTime_Click(object sender, System.EventArgs e)
		{
			int pos = txtPad.SelectionStart;
			txtPad.Text = txtPad.Text.Insert(pos, DateTime.Now.ToString());
		}

		private void mnuSelectAll_Click(object sender, System.EventArgs e)
		{
			int len = txtPad.Text.Length;
			int pos = 0;

			txtPad.SelectionStart = pos;
			txtPad.SelectionLength = len;
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
				mRecent.WriteToConfig();
				CloseAllDocuments();
		}

		MRUMenuItem mRecent=new MRUMenuItem(); 

		private void MRUClick(object sender, System.EventArgs e) 
		{
			MenuItem mSender = sender as MenuItem;
			MRUMenuItem mParent = mSender.Parent as MRUMenuItem;
			// Use mSender.Text to handle the clicked event
			// When the file is succesfully opened, use FileOpened
			OpenFile(mSender.Text);
			mParent.FileOpened(mSender.Text);
		}	
	
		private void MRUChanged(object sender, System.EventArgs e) 
		{
			MRUMenuItem m=sender as MRUMenuItem;
			// Persist m.MRUFiles[0] through m.MRUFiles[3]
		}

		private void mnuPageSetup_Click(object sender, System.EventArgs e)
		{
			pageSetupDialog1.ShowDialog();
		}

		private void mnuSelectFind_Click(object sender, System.EventArgs e)
		{
			FindDialog.CurrentSearchString = txtPad.SelectedText;
			FindDialog.FindNext(txtPad);		
		}


	}
}
