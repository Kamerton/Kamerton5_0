using System;
using System.Windows.Forms;
using System.Configuration;

namespace Notepad.NET {
	/// <summary>
	/// Summary description for MRUMenuItem.
	/// </summary>
	public class MRUMenuItem : MenuItem	{
		string[] mru;
		int _count;
		public string[] MRUFiles {
			get {return mru;}
		}

		public void InitializeFromConfig(int count)
		{
			mru = (string[])Array.CreateInstance(typeof(string), count);
			for (int i = 0; i < count; i++)
			{
				string nextString = ConfigurationSettings.AppSettings["mrufile"+(i+1).ToString()];
				if (nextString != null)
				{
					mru[i] = nextString;
					_count=mru.Length;
					if (""!=mru[i]) 
						{
							MenuItem mmru = new MenuItem(mru[i], new EventHandler(OnMRUClick));
							this.MenuItems.Add(mmru);
						}
				}
			}
		}

		public void WriteToConfig()
		{
			for (int i = 0; i < mru.Length; i++)
			{
				string nextString = mru[i];
				if (nextString != null)
				{
					ConfigHandler.replaceConfigSettings("mrufile"+(i+1).ToString(), mru[i]);
				}
			}
		}


		public void Initialize(string[] files) 
		{
			mru=files;
			_count=mru.Length;
			for (int i=0;i<_count;i++) {
				if (""!=mru[i]) {
					MenuItem mmru = new MenuItem(mru[i], new EventHandler(OnMRUClick));
					this.MenuItems.Add(mmru);
				}
			}
		}
		public void FileOpened(string file) {
			int found=_count-1;
			for (int j=0;j<_count;j++) {
				if (file==mru[j]) {
					found=j;
					break;
				}
			}
			while (found>0)	mru[found]=mru[--found];
			mru[0]=file;
			this.MenuItems.Clear();
			for (int i=0;i<_count;i++) {
				if (""!=mru[i]) {
					MenuItem mmru = new MenuItem(mru[i], new EventHandler(OnMRUClick));
					this.MenuItems.Add(mmru);
				}
			}
			if (MRUChanged != null) {
				MRUChanged(this, new EventArgs()); 
			}
		}
		/// <summary>
		/// Fired when a mru menuitem is clicked
		/// </summary>
		public event EventHandler MRUClicked; 
		protected virtual void OnMRUClick(object o, EventArgs e) {
			if (MRUClicked != null) {
				MRUClicked(o, e); 
			}
		}
		/// <summary>
		/// Fired when the mru is changed
		/// </summary>
		public event EventHandler MRUChanged; 
	}
}
