using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;

namespace SharpClient.UI.Charting {
    public enum PieLabelFormat {
        None, Name, Value, Percent, NameValue, NamePercent
    }
    /// <summary>
    /// Summary description for Piechart.
    /// </summary>
    public class Piechart : AbstractChart {
        private bool m_enableOutline = true;
        private int m_space = 5;
        private int[] m_startAngles;
        private int[] m_sweepAngles;
        private Label[] m_labels;
        private PieLabelFormat m_labelFormat = PieLabelFormat.NamePercent;

        public PieLabelFormat LabelFormat {
            get { return m_labelFormat; }
            set { m_labelFormat = value; }
        }

        public bool EnableOutline {
            get { return m_enableOutline; }
            set { m_enableOutline = value; }
        }

        public Piechart(int width, int height) : base (width, height) {
        }
		public Piechart(Size size):base(size) 
		{
		}

        protected override void calculateInsets(Insets insets, Graphics g) 
		{
            base.calculateInsets(insets, g);
            if (m_labels == null || m_labels.Length == 0) {
                return;
            }
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (Label label in m_labels) {
                int labelWidth = label.getWidth(g);
                int labelHeight = label.getHeight(g);
                maxWidth = maxWidth < labelWidth ? labelWidth : maxWidth;
                maxHeight = maxHeight < labelHeight ? labelHeight : maxHeight;
            }
            insets.Left += (maxWidth + m_space);
            insets.Right += (maxWidth + m_space);
            insets.Top += (maxHeight + m_space);
            insets.Bottom += (maxHeight + m_space);

        }

        protected override void drawChart(Graphics g, Rectangle bound) {			
            foreach(Series series in DataSource.SeriesCollection)
			{
                if (series == null) {
                    return;
                }
                int size = series.DataPoints.Count;
                if (size == 0 || series.Total <= 0) {
                    return; // No data to draw.
                }

                // Make circle.				
				if(bound.Width <0) 
				{					
					bound.Width =3*Math.Abs(bound.Width);   
				}

				if(bound.Height  <0) 
				{					
					bound.Height  =3*Math.Abs(bound.Height); 
				}

				
				if (bound.Width < bound.Height) 
					{
						bound.Height = bound.Width;
						bound.Y = (this.Height - bound.Height) / 2;
					} 
					else 
					{
						bound.Width = bound.Height;
						bound.X = (this.Width - bound.Width) / 2;
					}				

				if (this.Enable3D) 
				{
					for (int index = 0; index < size; index++) 
					{
						DataPoint data = (DataPoint) series.DataPoints[index];
						bool isExplode = false;
						if (data is PieDataPoint) 
						{
							isExplode = ((PieDataPoint) data).IsExplode;
						}
						int dx = 10;
						int dy = 10;
						if (isExplode) 
						{
							Point p = convertPolarToRect(m_startAngles[index] + m_sweepAngles[index] / 2, bound);
							dx += (p.X - (bound.X + bound.Width / 2)) / 4;
							dy += (p.Y - (bound.Y + bound.Height / 2)) / 4;
						}
						Color darker = ControlPaint.Dark(data.Color);
						g.TranslateTransform(dx, dy);
						using (Brush brush = new SolidBrush(darker)) 
						{
							g.FillPie(brush, bound, -1 * m_startAngles[index], -1 * m_sweepAngles[index]);							
						}
						g.TranslateTransform(-1 * dx, -1 * dy);

					}
				}
                for (int index = 0; index < size; index++) {
                    DataPoint data = (DataPoint) series.DataPoints[index];
                    bool isExplode = false;
                    if (data is PieDataPoint) {
                        isExplode = ((PieDataPoint) data).IsExplode;
                    }
                    int dx = 0;
                    int dy = 0;
                    if (isExplode) {
                        Point p = convertPolarToRect(m_startAngles[index] + m_sweepAngles[index] / 2, bound);
                        dx = (p.X - (bound.X + bound.Width / 2)) / 4;
                        dy = (p.Y - (bound.Y + bound.Height / 2)) / 4;
                    }
                    g.TranslateTransform(dx, dy);
                    
                    Color darker = ControlPaint.Light(data.Color);
                    using (LinearGradientBrush brush = new LinearGradientBrush(bound, data.Color, darker, LinearGradientMode.Vertical)) {
                        g.FillPie(brush, bound, -1 * m_startAngles[index], -1 * m_sweepAngles[index]);
                    }

                    if (m_enableOutline) {
                        using (Pen pen = new Pen(new SolidBrush(data.OutlineColor))) {
                            g.DrawPie(pen, bound, -1 * m_startAngles[index], -1 * m_sweepAngles[index]);
                        }

                    }
					//if(!bOverSpanning)
					{
						if (m_labels != null) 
						{
							int angle = m_startAngles[index] + m_sweepAngles[index]  / 2;

							Rectangle rect = new Rectangle( bound.X,  bound.Y,  bound.Width + Depth,  bound.Height + Depth);

							Point pt = convertPolarToRect(angle, rect);
							if (angle < 180) 
							{
								m_labels[index].VerticalAlign = VerticalAlign.Bottom;
								if (angle < 90) 
								{
									m_labels[index].HorizontalAlign = HorizontalAlign.Left;
								} 
								else 
								{
									m_labels[index].HorizontalAlign = HorizontalAlign.Right;
								}
							} 
							else 
							{
								m_labels[index].VerticalAlign = VerticalAlign.Top;
								if (angle < 270) 
								{
									m_labels[index].HorizontalAlign = HorizontalAlign.Right;
								} 
								else 
								{
									m_labels[index].HorizontalAlign = HorizontalAlign.Left;
								}
							}
							//m_labels[index].HorizontalAlign = HorizontalAlign.Center;
							m_labels[index].Position = pt;
							m_labels[index].draw(g);						

						}
						g.TranslateTransform(-1 * dx, -1 * dy);
					}

                }
           }
        }

//        private Rectangle adjustBoundBasedOnLabel(Rectangle bound, Graphics g) {
//            if (m_labels == null || m_labels.Length == 0) {
//                return bound;
//            }
//            int maxWidth = 0;
//            int maxHeight = 0;
//            foreach (Label label in m_labels) {
//                int labelWidth = label.getWidth(g);
//                int labelHeight = label.getHeight(g);
//                maxWidth = maxWidth < labelWidth ? labelWidth : maxWidth;
//                maxHeight = maxHeight < labelHeight ? labelHeight : maxHeight;
//            }
//            bound.X += (maxWidth + m_space);
//            bound.Width -= (2 * (maxWidth + m_space));
//            bound.Y += (maxHeight + m_space);
//            bound.Height -= (2 * (maxHeight + m_space));
//
//            // Make a circle.
//            if (bound.Width < bound.Height) {
//                bound.Height = bound.Width;
//                bound.Y = (this.Height - bound.Height) / 2;
//            } else {
//                bound.Width = bound.Height;
//            }
//            return bound;
//        }

        protected override void init() {
            if (DataSource.SeriesCollection.Count > 1) {
                throw new Exception("There should be only 1 series for piechart.");
            }
            foreach(Series series in DataSource.SeriesCollection) {
                if (series == null) {
                    return;
                }

                int size = series.DataPoints.Count;
                if (size == 0 || series.Total <= 0) {
                    return; // No data to draw.
                }
                m_startAngles = new int[size];
                m_sweepAngles = new int[size];

                if (m_labelFormat != PieLabelFormat.None) {
                    m_labels = new Label[size];
                }

                // Pre compute list of start degree for each pie slice.
                // Steps:
                //    Calculate start degree of each category.
                float total = series.Total;
                m_startAngles[0] = 0;
                m_sweepAngles[size - 1] = 360;

                for (int index = 0; index < size; index++) {
                    DataPoint data = (DataPoint) series.DataPoints[index];

                    string labelText = "";
                    switch (m_labelFormat) {
                        case PieLabelFormat.Name:
                            labelText = data.Name;
                            break;
                        case PieLabelFormat.NamePercent:
                            labelText = String.Format("{0} ({1:P})", data.Name, data.Value / total);
                            break;
                        case PieLabelFormat.NameValue:
                            labelText = String.Format("{0} {1:N}", data.Name, data.Value);
                            break;
                        case PieLabelFormat.Percent:
                            labelText = String.Format("({1:P})", data.Value / total);
                            break;
                        case PieLabelFormat.Value:
                            labelText = String.Format("{1:n}", data.Value);
                            break;
                    }
                    if (m_labelFormat != PieLabelFormat.None) {
                        m_labels[index] = new Label(labelText);
                    }
                    if (index != size - 1) {
                        m_sweepAngles[index] = (int) Math.Round((360 * data.Value) / total);
                        m_startAngles[index + 1] = m_sweepAngles[index] + m_startAngles[index];
                        m_sweepAngles[size - 1] -= m_sweepAngles[index];
                    }
                }
            }
            // End pre compute data.
        }
    }
}
