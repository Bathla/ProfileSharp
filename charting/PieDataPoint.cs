using System;
using System.Drawing;

namespace SharpClient.UI.Charting {
    /// <summary>
    /// Summary description for PieDataPoint.
    /// </summary>
    public class PieDataPoint : DataPoint {
        private bool m_isExplode;

        public bool IsExplode {
            get { return m_isExplode; }
            set { m_isExplode = value; }
        }

        public PieDataPoint(string name) : this(name, 0.0F) {
        }

        public PieDataPoint(float value) : this (null, value) {
        }

        public PieDataPoint(string name, float value) : this(name, value, Color.Black) {
        }
        public PieDataPoint(string name, float value, Color color) : this (name, value, color, false) {
        }
        public PieDataPoint(string name, float value, Color color, bool isExplode) : base(name, value, color) {
            m_isExplode = isExplode;
        }
    }
}
