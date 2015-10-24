using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

namespace Notepad.NET
{
	/// <summary>
	/// Summary description for TextBoxPrinter.
	/// </summary>
	public class TextBoxPrinter
	{
		public TextBoxPrinter()
		{
			_printMargin = ConfigHandler.GetConfigSettingInt("printmargin");
		}

		private int _printMargin = 1;

		private int _lastPosition = 0;
		private int _lastIndex = 0;

		public int LastPosition
		{
			set
			{
				_lastPosition = value;
			}
		}

		/// <summary>
		/// Go through each line of the text box and print it
		/// </summary>
		/// <param name="txtSurface"></param>
		public bool Print(Graphics g, TextBox txtSurface, PrintDocument printer, float screenResolution)
		{
			Font textFont = txtSurface.Font;

			// go line by line and draw each string
			int startIndex = _lastIndex;
			int index = txtSurface.Text.IndexOf("\n", startIndex);

			int nextPosition = (int)_lastPosition;
			// just use the default string format
			StringFormat sf = new StringFormat();

			// sf.FormatFlags = StringFormatFlags.NoClip | (~StringFormatFlags.NoWrap );
			// get the page height
			int lastPagePosition = (int)(((printer.DefaultPageSettings.PaperSize.Height/ 100.0f) - 1.0f) * (float)screenResolution);
	//		int resolution = printer.DefaultPageSettings.PrinterResolution.X;

			// use the screen resolution for measuring the page
			int resolution = (int)screenResolution;

			// calculate the maximum width in inches from the default paper size and the margin
			int maxwidth = (int)((printer.DefaultPageSettings.PaperSize.Width/100.0f - _printMargin*2)*resolution);

			// get the margin in inches
			int printMarginInPixels = resolution * _printMargin;
			Rectangle rtLayout = new Rectangle(0,0,0,0);
			int lineheight = 0;

			while (index != -1)
			  {
			  string nextLine = txtSurface.Text.Substring(startIndex, index - startIndex);
			  lineheight = (int)(g.MeasureString(nextLine, textFont, maxwidth, sf).Height); 
			  rtLayout = new Rectangle(printMarginInPixels, nextPosition, maxwidth, lineheight);
			  g.DrawString(nextLine, textFont, Brushes.Black, rtLayout, sf);

			  nextPosition += (int)(lineheight + 3);
			  startIndex = index + 1;
			  index = txtSurface.Text.IndexOf("\n", startIndex);
			  if (nextPosition > lastPagePosition)
				{
				  _lastPosition = (int)screenResolution;
				  _lastIndex = index;
				  return true; // reached end of page
				}
			}

			// draw the last line
			string lastLine = txtSurface.Text.Substring(startIndex);
			lineheight = (int)(g.MeasureString(lastLine, textFont, maxwidth, sf).Height); 
			rtLayout = new Rectangle(printMarginInPixels, nextPosition, maxwidth, lineheight);
			g.DrawString(lastLine, textFont, Brushes.Black, rtLayout, sf);
			  
			_lastPosition = (int)screenResolution;
			_lastIndex = 0;
			return false;
		}
	}
}
