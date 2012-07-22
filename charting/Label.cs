using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpClient.UI.Charting
{
    public enum HorizontalAlign {
        Left, Center, Right
    }
    public enum VerticalAlign {
        Bottom, Middle, Top
    }

	/// <summary>
	/// Summary description for Label.
	/// </summary>
	public class Label
	{
        private Font m_font;
        private HorizontalAlign m_horizontalAlign;
        private VerticalAlign m_verticalAlign;
        private int m_angle = 0;
        private string m_text;
        private PointF m_position = new Point(0, 0);
        private StringFormat m_stringFormat;
        private Color m_color;

        public Color Color {
            get { return m_color; }
            set { m_color = value; }
        }
        public Font Font {
            get { return m_font; }
            set { m_font = value; }
        }
        public int RotateAngle {
            get { return m_angle; }
            set { m_angle = value; }
        }
        public PointF Position {
            get { return m_position; }
            set { m_position = value; }
        }
        public VerticalAlign VerticalAlign {
            get { return m_verticalAlign; }
            set { 
                m_verticalAlign = value; 
                switch (m_verticalAlign) {
                    case VerticalAlign.Bottom:
                        m_stringFormat.LineAlignment = StringAlignment.Far; break;
                    case VerticalAlign.Middle:
                        m_stringFormat.LineAlignment = StringAlignment.Center; break;
                    case VerticalAlign.Top:
                        m_stringFormat.LineAlignment = StringAlignment.Near; break;
                }
            }
        }
        public HorizontalAlign HorizontalAlign {
            get { return m_horizontalAlign; }
            set { 
                m_horizontalAlign = value; 
                switch (m_horizontalAlign) {
                    case HorizontalAlign.Left:
                        m_stringFormat.Alignment = StringAlignment.Near; break;
                    case HorizontalAlign.Center:
                        m_stringFormat.Alignment = StringAlignment.Center; break;
                    case HorizontalAlign.Right:
                        m_stringFormat.Alignment = StringAlignment.Far; break;
                }
            }
        }
		public Label() : this(null)
		{
		}
        public Label(string text) : this(text, HorizontalAlign.Center, VerticalAlign.Middle) {
        }
        public Label(string text, HorizontalAlign halign, VerticalAlign valign) {
            m_text = text;
            
            m_font = new Font("Arial", 8, FontStyle.Regular);
            m_stringFormat = new StringFormat();
            m_color = Color.Black;
            this.HorizontalAlign = halign;
            this.VerticalAlign = valign;
        }

        public void draw(Graphics g) {

            //g.DrawRectangle(new Pen(new SolidBrush(System.Drawing.Color.Red)), m_position.X, m_position.Y, 2, 2);

            Matrix oldTransform = g.Transform;
            g.TranslateTransform(m_position.X, m_position.Y);
            g.RotateTransform(m_angle, MatrixOrder.Prepend);
            using (Brush brush = new SolidBrush(m_color)) {
                g.DrawString(m_text, m_font, brush, 0, 0, m_stringFormat);
            }
            g.Transform = oldTransform;
        }
        public int getWidth(Graphics g) {
            SizeF size = g.MeasureString(m_text, m_font);
            float theta = (float) (m_angle * Math.PI / 180.0F);
            float width = (float) (size.Width * Math.Abs(Math.Cos(theta)) + size.Height * Math.Abs(Math.Cos(Math.PI / 2 - theta)));
            return (int) width;
        }
        public int getHeight(Graphics g) {
            SizeF size = g.MeasureString(m_text, m_font);
            float theta = (float) (m_angle * Math.PI / 180.0F);
            float height = (float) (size.Width * Math.Abs(Math.Sin(theta)) + size.Height * Math.Abs(Math.Sin(Math.PI / 2 - theta)));
            return (int) height;
        }
	}
}
