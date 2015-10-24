using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace Notepad.NET
{
	/// <summary>
	/// Summary description for FindDialog.
	/// </summary>
	public class FindDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.CheckBox chkCaseSensitive;

		private string _text = "";
		static private bool   _caseSensitive = false;

		private TextBox _txtControl;
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnReplace;
		private bool	_replace = false;

		public string FindText
		{
			get
			{
				return _text;
			}

			set
			{
				_text = value;
			}
		}

		public bool Replace
		{
			set
			{
				_replace = value;
				txtReplace.Enabled = value;
				btnReplace.Visible = value;
			}
	    }

		public bool CaseSensitive
		{
			get
			{
				return _caseSensitive;
			}

			set
			{
				_caseSensitive = value;
			}
		}


		public FindDialog(TextBox txtControl, bool replace)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			txtFind.Focus();
			_txtControl = txtControl;
			Replace = replace;
			txtFind.Text = _txtControl.Text.Substring(_txtControl.SelectionStart, _txtControl.SelectionLength);

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
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtFind = new System.Windows.Forms.TextBox();
			this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
			this.txtReplace = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnReplace = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find What:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(24, 96);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "Find Next";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(200, 96);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtFind
			// 
			this.txtFind.Location = new System.Drawing.Point(104, 8);
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(176, 20);
			this.txtFind.TabIndex = 0;
			this.txtFind.Text = "";
			// 
			// chkCaseSensitive
			// 
			this.chkCaseSensitive.Location = new System.Drawing.Point(104, 66);
			this.chkCaseSensitive.Name = "chkCaseSensitive";
			this.chkCaseSensitive.TabIndex = 2;
			this.chkCaseSensitive.Text = "case sensitive";
			// 
			// txtReplace
			// 
			this.txtReplace.Location = new System.Drawing.Point(104, 40);
			this.txtReplace.Name = "txtReplace";
			this.txtReplace.Size = new System.Drawing.Size(176, 20);
			this.txtReplace.TabIndex = 1;
			this.txtReplace.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Replace With:";
			// 
			// btnReplace
			// 
			this.btnReplace.Location = new System.Drawing.Point(112, 96);
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Size = new System.Drawing.Size(80, 23);
			this.btnReplace.TabIndex = 8;
			this.btnReplace.Text = "Replace Next";
			this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
			// 
			// FindDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(292, 126);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.txtReplace);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.chkCaseSensitive);
			this.Controls.Add(this.txtFind);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Name = "FindDialog";
			this.Text = "Find Dialog";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		static private int _currentIndex = 0;
		static public string CurrentSearchString = "";

		static public bool FindNext(TextBox txtControl)
		{
			return FindNext(CurrentSearchString, _caseSensitive, txtControl);
		}

		static public bool FindNext(string searchString, bool caseSensitive, TextBox txtControl)
		{
			CurrentSearchString = searchString;
			// look for the text after the cursor
			// int startPosition = _currentIndex;
			_currentIndex = txtControl.SelectionStart + 1;
			int index = -1;
			if (caseSensitive)
			{
				_currentIndex = txtControl.Text.IndexOf(searchString, _currentIndex);
			}
			else
			{
				CultureInfo culture = new CultureInfo("en-us");

				_currentIndex = culture.CompareInfo.IndexOf(txtControl.Text, searchString, _currentIndex, System.Globalization.CompareOptions.IgnoreCase);
			}
			if (_currentIndex >= 0)
			{
				txtControl.SelectionStart = txtControl.Text.IndexOf("\n", _currentIndex) + 2;
				txtControl.SelectionLength = 0;
				txtControl.ScrollToCaret(); // scroll past selection
				txtControl.SelectionStart = _currentIndex;
				txtControl.SelectionLength = searchString.Length;
				_currentIndex += 1; // advance past selection
				//				_txtControl.Invalidate();
			}
			else
			{
				MessageBox.Show("Reached the end of the document.");
				_currentIndex = 0;
				return false;
			}

			return true;
		}


		private void btnOK_Click(object sender, System.EventArgs e)
		{
			_text = txtFind.Text;
			_caseSensitive = chkCaseSensitive.Checked;
			_txtControl.Focus();
			FindNext(_text, _caseSensitive, _txtControl);
		}

		void ReplaceSelection(string text, bool scroll)
		{
			FlickerFreeTextBox._Paint = false;
			int saveStart = _txtControl.SelectionStart;
			// replace text at found position (remember to go back one)
			_txtControl.Text = _txtControl.Text.Remove(_currentIndex-1, _txtControl.SelectionLength);
			_txtControl.Text = _txtControl.Text.Insert(_currentIndex-1, text);
			_txtControl.SelectionStart =  _txtControl.Text.IndexOf("\n", saveStart) + 2;
			_txtControl.SelectionLength = 0;
			if (scroll)
			{
				_txtControl.ScrollToCaret(); // scroll past selection
			}
			_txtControl.SelectionStart = saveStart;
			_txtControl.SelectionLength = text.Length;
			FlickerFreeTextBox._Paint = true;
		}

		bool CurrentSelectedTextIs(string txt)
		{
			if (_caseSensitive)
			{
				return (_txtControl.SelectedText == txt);
			}

			return (_txtControl.SelectedText.ToUpper() == txt.ToUpper());
		}

		private void btnReplace_Click(object sender, System.EventArgs e)
		{
			_text = txtFind.Text;
			if (CurrentSelectedTextIs(_text))
			{
				_currentIndex = _txtControl.SelectionStart+1;
				ReplaceSelection(txtReplace.Text, false);
				return;
			}

			_caseSensitive = chkCaseSensitive.Checked;
			_txtControl.Focus();
			bool stillgoing = FindNext(_text, _caseSensitive, _txtControl);
			if (stillgoing)
			{
				ReplaceSelection(txtReplace.Text, true);
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
