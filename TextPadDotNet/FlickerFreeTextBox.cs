using System;
using System;
using System.Windows.Forms;

namespace Notepad.NET
{
		/// <summary>
		/// Summary description for FlickerFreeRichEditTextBox - Subclasses the RichTextBox to allow control over flicker
		/// </summary> 
		public class FlickerFreeTextBox : TextBox
		{
			const short WM_PAINT = 0x00f;

			private void InitializeComponent()
			{
				// 
				// FlickerFreeTextBox
				// 
				this.DoubleClick += new System.EventHandler(this.FlickerFreeTextBox_DoubleClick);

			}
		
			public FlickerFreeTextBox()
			{
			}

			public static bool _Paint = true;
			protected override void WndProc(ref System.Windows.Forms.Message m)
			{
				// Code courtesy of Mark Mihevc
				// sometimes we want to eat the paint message so we don't have to see all the
				// flicker from when we select the text to change the color.
				if (m.Msg == WM_PAINT)
				{
					if (_Paint)
						base.WndProc(ref m);   // if we decided to paint this control, just call the RichTextBox WndProc
					else
						m.Result = IntPtr.Zero;   //  not painting, must set this to IntPtr.Zero if not painting otherwise serious problems.
				}
				else
					base.WndProc (ref m);   // message other than WM_PAINT, jsut do what you normally do.

			}

			private void FlickerFreeTextBox_DoubleClick(object sender, System.EventArgs e)
			{
				_Paint = false;
			}
		}
	}
