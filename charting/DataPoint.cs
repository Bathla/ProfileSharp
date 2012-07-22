using System;
using System.Drawing;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Individual data point on chart. This is immutable object.
	/// </summary>
	public class DataPoint
	{
        private string m_name;
        private float m_value;
        private Brush m_brush;
        private Color m_color;
        private Color m_outlineColor;

        public DataPoint(string name) : this(name, 0.0F) {
        }

        public DataPoint(float value) : this (null, value) {
        }

        public DataPoint(string name, float value) : this(name, value, Color.Black) {
        }
        public DataPoint(string name, float value, Color color) : this (name, value, (Brush) null) {
            m_color = color;
            m_outlineColor = Color.Black;
        }

		public DataPoint(string name, float value, Brush brush)
		{
            m_name = name;
            m_value = value;
            m_brush = brush;
            m_color = Color.Black;
		}
        public Color Color {
            get { return m_color; }
        }
        public Color OutlineColor {
            get { return m_outlineColor; }
        }

        public string Name {
            get { return m_name; }
        }
        public float Value {
            get { return m_value; }
        }
        public Brush Brush {
            get { return m_brush; }
        }
	}
}
