using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpClient.UI.Charting {
    /// <summary>
    /// Summary description for AbstractChart.
    /// </summary>
    public abstract class AbstractChart : IDrawable {
        private Size m_size;
        private IChartDataSource m_datasource;
        private Color m_backgroundColor;
        private Insets m_insets;
        private int m_depth = 0;
        private bool m_enableLegend;
        private Label m_titleLabel;

        public AbstractChart(int width, int height) : this (new Size(width, height)) {
        }
        public AbstractChart(Size size) {
            m_size = size;
            m_backgroundColor = Color.White;
            m_insets = new Insets(10, 10, 10, 10);
            m_enableLegend = true;
            Enable3D = true;
        }

        public bool Enable3D {
            get { return m_depth != 0; }
            set { m_depth = value ? 30 : 0; }
        }

        public bool EnableLegend {
            get { return m_enableLegend; }
            set { m_enableLegend = value; }
        }
        public int Depth {
            get { return m_depth; }
        }
        public Size Size {
            get { return m_size; }
            set { m_size = value; }
        }
        public int Width {
            get { return m_size.Width; }
			set { m_size.Width = value;}
        }
        public int Height {
            get { return m_size.Height; }
			set { m_size.Height = value;}
		}
        public IChartDataSource DataSource {
            get { return m_datasource; }
            set { m_datasource = value; }
        }
        public Color BackgroundColor {
            get { return m_backgroundColor; }
            set { m_backgroundColor = value; }
        }

        public void draw(Graphics g) {
            if (g == null) {
                throw new ArgumentNullException("g");
            }
            if (m_datasource == null) {
                return;
            }
			
            init();
            drawBackground(g);
            drawTitle(g);

            if (m_datasource.SeriesCollection.Count == 0) {
                return;
            }
            Insets insets = new Insets(m_insets.Top, m_insets.Left, m_insets.Bottom, m_insets.Right);
            calculateInsets(insets, g);
            Rectangle bound = new Rectangle(insets.Left, 
                                            insets.Top, 
                                            m_size.Width - insets.Left - insets.Right, 
                                            m_size.Height - insets.Top - insets.Bottom);
			
            drawChart(g, bound);
			
//            if (m_enableLegend) {
//                drawLegend(g);
//            }
        }


        protected abstract void drawChart(Graphics g, Rectangle bound);

        /// <summary>
        /// This method should perform any data calculation before chart is drawn.
        /// </summary>
        protected virtual void init() {
        }

        private void drawBackground(Graphics g) {
            using (Brush brush = new SolidBrush(m_backgroundColor)) {
                g.FillRectangle(brush, 0, 0, m_size.Width, m_size.Height);
            }
        }

        private void drawTitle(Graphics g) {
            if (m_datasource.Name == null || m_datasource.Name.Trim() == "") {
                return;
            }
            m_titleLabel = new Label(m_datasource.Name);
            m_titleLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            m_titleLabel.Position = new Point(m_insets.Left + (m_size.Width - m_insets.Left - m_insets.Right) / 2, m_insets.Top);
            m_titleLabel.draw(g);
        }

//        private void drawLegend(Graphics g) {
//            // TODO: Need to draw an outline rectangle for legend.
//            int hGap = 5;
//            int vGap = 5;
//            int space = 3;
//            int size = 10;
//            Point position = new Point(m_insets.Left, m_size.Height - m_insets.Bottom);
//            int seriesCount = m_datasource.SeriesCollection.Count;
//
//            foreach (Series series in m_datasource.SeriesCollection) {
//                if (seriesCount == 1) {
//                    foreach (DataPoint data in series.DataPoints) {
//                        g.FillRectangle(new SolidBrush(data.Color), position.X, position.Y - size, size, size);
//                        g.DrawRectangle(new Pen(Color.Black), position.X, position.Y - size, size, size);
//
//                        position.X += (space + size);
//                        Label label = new Label(data.Name);
//                        label.VerticalAlign = VerticalAlign.Bottom;
//                        label.HorizontalAlign = HorizontalAlign.Left;
//                        label.Position = position;
//                        label.draw(g);
//                        position.X += (label.getWidth(g) + hGap);
//                    }
//                } else {
//                    g.FillRectangle(new SolidBrush(series.Color), position.X, position.Y - size, size, size);
//                    g.DrawRectangle(new Pen(Color.Black), position.X, position.Y - size, size, size);
//
//                    position.X += (space + size);
//                    Label label = new Label(series.Name);
//                    label.VerticalAlign = VerticalAlign.Bottom;
//                    label.HorizontalAlign = HorizontalAlign.Left;
//                    label.Position = position;
//                    label.draw(g);
//                    position.X += (label.getWidth(g) + hGap);
//                }
//
//            }
//            int legendWidth = position.X - m_insets.Left + 10;
//           g.DrawRectangle(new Pen(Color.Black), m_insets.Left - 5, m_size.Height - m_insets.Bottom - 20, legendWidth, 25);
//
//        }

		public void drawLegend(Graphics g) 
		{
			// TODO: Need to draw an outline rectangle for legend.
			int hGap = 5;
			int vGap = 5;
			int space = 3;
			int size = 10;
			Point position = new Point(12 ,50); 
			int seriesCount = m_datasource.SeriesCollection.Count;

			foreach (Series series in m_datasource.SeriesCollection)
			{
				if (seriesCount == 1) 
				{
					foreach (DataPoint data in series.DataPoints) 
					{						
						g.FillRectangle(new SolidBrush(data.Color), position.X, position.Y - size, size, size);
						g.DrawRectangle(new Pen(Color.Black), position.X, position.Y - size, size, size);

						position.X = (space + size);						
						Label label = new Label("    "+data.Name);
						label.VerticalAlign = VerticalAlign.Bottom;
						label.HorizontalAlign = HorizontalAlign.Left; 
						label.Position = position;
						label.draw(g);						
						position.Y += (label.getHeight(g) + vGap);
					}
				} 
				else 
				{
					g.FillRectangle(new SolidBrush(series.Color), position.X, position.Y - size, size, size);
					g.DrawRectangle(new Pen(Color.Black), position.X, position.Y - size, size, size);

					position.X += (space + size);
					Label label = new Label(series.Name);
					label.VerticalAlign = VerticalAlign.Bottom;
					label.HorizontalAlign = HorizontalAlign.Left;
					label.Position = position;
					label.draw(g);
					position.X += (label.getWidth(g) + hGap);
				}

			}			

		}
        protected virtual void calculateInsets(Insets insets, Graphics g) {
            // TODO: Need to get an accurate legend height.
            int legendHeight = 25;
            if (m_titleLabel != null) {
                insets.Top += (int) m_titleLabel.getHeight(g);
            }
            if (m_enableLegend) {
                insets.Bottom += legendHeight;
            }

        }
        protected Rectangle getRecommendedBound(Graphics g) {
            int top = m_insets.Top;

            if (m_titleLabel != null) {
                top += (int) m_titleLabel.getHeight(g);
                
            }
            int legendHeight = 0;
            if (m_enableLegend) {
                legendHeight = top;
            }

            return new Rectangle(m_insets.Left, 
                top, 
                m_size.Width - m_insets.Left - m_insets.Right, 
                m_size.Height - top - legendHeight - m_insets.Top - m_insets.Bottom);
        }

        public void fillArc3D(Graphics g, Color color, Rectangle rect, int startAngle, int sweepAngle, int depth) {
            Color darker = ControlPaint.Dark(color);
            g.TranslateTransform(10, 10);
            g.FillPie(new SolidBrush(darker), rect, startAngle, sweepAngle);
            g.TranslateTransform(-10, -10);

            g.FillPie(new SolidBrush(color), rect, startAngle, sweepAngle);
            
        }
        public void draw3DRect(Graphics g, Color color, RectangleF rect, int depth) {
            int dx = depth;
            int dy = depth;
            if (rect.Width < 0) {
                rect.Width = -1 * rect.Width;
                rect.X -= rect.Width;
            }
            if (rect.Height < 0) {
                rect.Height = -1 * rect.Height;
                rect.Y -= rect.Height;
            }
            // Draw top face.
            PointF[] topFace = new PointF[4];
            topFace[0] = new PointF(rect.X, rect.Y);
            topFace[1] = new PointF(rect.X + dx, rect.Y - dy);
            topFace[2] = new PointF(rect.X + rect.Width + dx, rect.Y - dy);
            topFace[3] = new PointF(rect.X + rect.Width, rect.Y);

            PointF[] sideFace = new PointF[4];
            sideFace[0] = new PointF(rect.X + rect.Width, rect.Y);
            sideFace[1] = new PointF(rect.X + rect.Width + dx, rect.Y - dy);
            sideFace[2] = new PointF(rect.X + rect.Width + dx, rect.Y - dy + rect.Height);
            sideFace[3] = new PointF(rect.X + rect.Width, rect.Y + rect.Height);

            // Fill 3D Rectangle.
            Color brighterColor = ControlPaint.LightLight(color);
            Color darkerColor = ControlPaint.Dark(color);
            Pen grayPen = new Pen(Color.Gray);
            Brush brush = new SolidBrush(color);
            Brush brighterBrush = new SolidBrush(ControlPaint.LightLight(color));
            Brush darkerBrush = new SolidBrush(ControlPaint.Dark(color));
            g.FillRectangle(brush, rect);
            g.FillPolygon(brighterBrush, topFace);
            g.FillPolygon(darkerBrush, sideFace);

            // Draw outline
            g.DrawRectangle(grayPen, rect.X, rect.Y, rect.Width, rect.Height);
            g.DrawPolygon(grayPen, topFace);
            g.DrawPolygon(grayPen, sideFace);

            grayPen.Dispose();
            darkerBrush.Dispose();
            brighterBrush.Dispose();

        }
        public Point convertPolarToRect(int angle, Rectangle bound) {
            float theta = (float) (angle * Math.PI / 180.0);
            float radiusX = bound.Width / 2.0F;
            float radiusY = bound.Height / 2.0F;
            return new Point(bound.X + (int) (radiusX + Math.Round(radiusX * (float) Math.Cos(theta))),
                bound.Y + (int) radiusY - (int) (Math.Round(radiusY * (float) Math.Sin(theta))));
        }
    }
}
