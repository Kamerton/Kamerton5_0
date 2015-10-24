using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Notepad.NET
{
	/// <summary>
	/// Summary description for VirtualFile.
	/// </summary>
	public class VirtualFile
	{
		static public int MAX_BUFFER_LENGTH = 1000000;

		private int _offset = 0;
		StreamReader _reader;
		FileStream _stream;
		Form _form;
		string _filename = "";

		bool _modified = false;
		bool _initialLoad = true;

		Undoer _undoer = new Undoer();

		public Undoer Undoer
		{
			get
			{
				return _undoer;
			}
		}

		public bool Modified
		{
			get
			{
				return _modified;
			}

			set
			{
				_modified = value;
			}
		}

		public bool InitialLoad
		{
			get
			{
				return _initialLoad;
			}

			set
			{
				_initialLoad = value;
			}
		}


		public VirtualFile(string filename, Form1 form)
		{
			//
			// TODO: Add constructor logic here
			//
			_stream = new FileStream(filename,  FileMode.Open, FileAccess.Read);
			_reader = new StreamReader(_stream);
			_form = form;
			_filename = filename;
			ReadCurrentText();
		}

		public string FileName
		{
			get
			{
				return _filename;
			}

			set
			{	
				_filename = value;
			}
		}

		public VirtualFile(Form1 form)
		{
			_form = form;
			_filename = "";
		}



		byte[] buffer = new byte[MAX_BUFFER_LENGTH];
		public string GetCurrentText(int characters)
		{
			_stream.Read(buffer, _offset, characters);
			return System.Text.Encoding.ASCII.GetString(buffer, (int)0, characters);
		}

		private string _text;
		private bool   _cutOff = false;
		public  bool CutOff
		{
			get
			{
				return  _cutOff;
			}
		}

		public string ReadCurrentText()
		{
			_stream.Seek(0, SeekOrigin.Begin);
			_length = _stream.Read(buffer, _offset, MAX_BUFFER_LENGTH);
			if (_length >= MAX_BUFFER_LENGTH)
			{
				// if we hit the maximum wall, set the cutoff flag
				_cutOff = true;
			}
			else
			{
				_cutOff = false;
			}

			_stream.Close();
			_text =  System.Text.Encoding.ASCII.GetString(buffer, (int)0, _length);
			return _text;
		}

		public void WriteFile(string filename)
		{
			FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
			fs.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(_text), 0, _text.Length);
			fs.Close();
			_modified = false; // once saved, unmodified
		}

		public void SetCurrentText(string newText)
		{
			_text = newText;
		}

		int _length = 0;
		public string GetCurrentText()
		{
			return _text;
		}

	}
}
