using System;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for XYDataPoint.
	/// </summary>
	public class XYDataPoint : DataPoint
	{
        private float m_y;
        private float m_x;

        public float X {
            get { return m_x; }
        }
        public float Y {
            get { return m_y; }
        }
		public XYDataPoint(float x, float y) : base("")
		{
            m_x = x;
            m_y = y;
		}
	}
}
