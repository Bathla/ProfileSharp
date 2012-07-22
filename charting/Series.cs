using System;
using System.Collections;
using System.Drawing;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// A collection of DataPoint
	/// </summary>
	public class Series
	{
        private string m_name;
        private Brush m_brush;
        private Color m_color;
        private Color m_outlineColor;
        private ArrayList m_points;
        private float m_minValue = float.MaxValue;
        private float m_maxValue = float.MinValue;
        private float m_totalValue = 0.0F;

        public string Name {
            get { return m_name; }
        }
        public Brush Brush {
            get { return m_brush; }
        }
        public ArrayList DataPoints {
            get { return m_points; }
        }
        public Color Color {
            get { return m_color; }
        }

        public Color OutlineColor {
            get { return m_outlineColor; }
        }

        public float MininumValue {
            get { return m_minValue; }
        }
        public float MaximumValue {
            get { return m_maxValue; }
        }
        public float Total {
            get { return m_totalValue; }
        }

        public Series() : this (null) {
        }

        public Series(string name) : this (name, Color.Black) {
        }

        public Series(string name, Color color) : this (name, (Brush) null) {
            m_color = color;
            m_outlineColor = Color.Black;
        }

		public Series(string name, Brush brush)
		{
            m_name = name;
            //m_brush = brush;
            m_points = new ArrayList();
		}

        public void add(DataPoint data) {
            m_points.Add(data);
            m_minValue = m_minValue < data.Value ? m_minValue : data.Value;
            m_maxValue = m_maxValue < data.Value ? data.Value : m_maxValue;
            m_totalValue += data.Value;
        }
	}
}
