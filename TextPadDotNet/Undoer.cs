using System;
using System.Collections;

namespace Notepad.NET
{

	/// <summary>
	/// Summary description for Undoer.
	/// </summary>
	public class Undoer
	{
		Stack _textStack = new Stack();
		public static int UndoerLimit = 10;

		public Undoer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void CheckPoint(string text)
		{
			if (_textStack.Count < UndoerLimit)
			{
				_textStack.Push(text);
			}
		}

		public string Undo()
		{
			if (_textStack.Count  == 0)
				return new string((char)2, 1); // return some untypable character

			return _textStack.Pop() as string;
		}
	}
}
