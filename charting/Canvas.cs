//using System;
//using System.Drawing;
//using System.Windows.Forms;
//
//namespace SharpClient.UI.Charting
//{
//	/// <summary>
//	/// Summary description for Canvas.
//	/// </summary>
//	public abstract class Canvas
//	{
//        private int m_width;
//        private int m_height;
//
//        public int Width {
//            get { return m_width; }
//        }
//        public int Height {
//            get { return m_height; }
//        }
//
//		public Canvas(int width, int height)
//		{
//            m_width = width;
//            m_height = height;
//		}
//
//        public abstract void draw(Graphics g);
//
//        public void fillArc3D(Graphics g, Color color, Rectangle rect, int startAngle, int sweepAngle, int depth) {
//            Color darker = ControlPaint.Dark(color);
//            for (int i = 0; i < depth; i++) 
//                g.FillPie(new SolidBrush(darker), rect.X +i, rect.Y + i, rect.Width, rect.Height, startAngle, sweepAngle);
//
//            g.FillPie(new SolidBrush(color), rect, startAngle, sweepAngle);
//            
//        }
//        public void draw3DRect(Graphics g, Color color, Rectangle rect, int depth) {
//            int dx = depth;
//            int dy = depth;
//            if (rect.Width < 0) {
//                rect.Width = -1 * rect.Width;
//                rect.X -= rect.Width;
//            }
//            if (rect.Height < 0) {
//                rect.Height = -1 * rect.Height;
//                rect.Y -= rect.Height;
//            }
//            // Draw top face.
//            Point[] topFace = new Point[4];
//            topFace[0] = new Point(rect.X, rect.Y);
//            topFace[1] = new Point(rect.X + dx, rect.Y - dy);
//            topFace[2] = new Point(rect.X + rect.Width + dx, rect.Y - dy);
//            topFace[3] = new Point(rect.X + rect.Width, rect.Y);
//
//            Point[] sideFace = new Point[4];
//            sideFace[0] = new Point(rect.X + rect.Width, rect.Y);
//            sideFace[1] = new Point(rect.X + rect.Width + dx, rect.Y - dy);
//            sideFace[2] = new Point(rect.X + rect.Width + dx, rect.Y - dy + rect.Height);
//            sideFace[3] = new Point(rect.X + rect.Width, rect.Y + rect.Height);
//
//            // Fill 3D Rectangle.
//            Color brighterColor = ControlPaint.LightLight(color);
//            Color darkerColor = ControlPaint.Dark(color);
//            g.FillRectangle(new SolidBrush(color), rect);
//            g.FillPolygon(new SolidBrush(brighterColor), topFace);
//            g.FillPolygon(new SolidBrush(darkerColor), sideFace);
//
//            // Draw outline
//            g.DrawRectangle(new Pen(Color.Gray), rect);
//            g.DrawPolygon(new Pen(Color.Gray), topFace);
//            g.DrawPolygon(new Pen(Color.Gray), sideFace);
//
//
//        }
//        public Point convertPolarToRect(int angle, Rectangle bound) {
//            float theta = (float) (angle * Math.PI / 180.0);
//            float radiusX = bound.Width / 2.0F;
//            float radiusY = bound.Height / 2.0F;
//            return new Point(bound.X + (int) (radiusX + Math.Round(radiusX * (float) Math.Cos(theta))),
//                bound.Y + (int) radiusY - (int) (Math.Round(radiusY * (float) Math.Sin(theta))));
//        }
//	}
//}
