using System;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// An Insets is an object represent border of a container. Borrow from Java.
	/// </summary>
	public class Insets
	{
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;
        public Insets(int top, int left, int bottom, int right) {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
        }
	}
}
