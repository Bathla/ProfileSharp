using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for XYChart.
	/// </summary>
	public class XYChart : CartesianChart
	{
		private bool mDrawCoordinates = true;
		public XYChart(int width, int height) : base(width, height)
		{
		}
		public XYChart(Size size) : base(size)
		{
		}
		public bool DrawCoordinates 
		{
			get 
			{
				return mDrawCoordinates;
			}
			set 
			{
				mDrawCoordinates = value;
			}
		}
        protected override void init() {
            PointF min = new PointF(float.MaxValue, float.MaxValue);
            PointF max = new PointF(float.MinValue, float.MinValue);

            int seriesCount = DataSource.SeriesCollection.Count;
	        // Calculate the min and the max of all data points
            foreach (Series series in DataSource.SeriesCollection) {
                foreach (XYDataPoint data in series.DataPoints) {
                    if (data.X < min.X)
                        min.X = data.X;
                    if (data.Y < min.Y)
                        min.Y = data.Y;
                    if (max.X < data.X)
                        max.X = data.X;
                    if (max.Y < data.Y)
                        max.Y = data.Y;
                }
            }

            XMin = min.X;
            XMax = max.X;
            YMin = min.Y;
            YMax = max.Y;
        }

        protected override void drawCartesianChart(Graphics g, Rectangle bound) {
            // Draw line.
            // TODO: Need to check whether the points are connected or not.
            foreach (Series series in DataSource.SeriesCollection) {
                if (series.DataPoints.Count == 0) {
                    continue;
                }

                PointF[] points = new PointF[series.DataPoints.Count];
                string[] labels = new String[series.DataPoints.Count];
                int index = 0;
                foreach (XYDataPoint data in series.DataPoints) {
                    points[index] = new PointF(data.X, data.Y);
                    labels[index++] = String.Format("({0},{1})", data.X, data.Y);
                }
                transformPoints(points);
                g.DrawLines(new Pen(series.Color,2), points);

                // Draw symbol. Default is circle.
                index = 0;
                foreach (PointF point in points) {
                    g.FillEllipse(new SolidBrush(series.Color), point.X - 2, point.Y - 2, 4, 4);
//                    if (c.getPointLabelFormat() != POINT_LABEL_NONE) {
//                        String label = null;
//                        String xy_label = null;
//
//                        try {
//                            DecimalFormat df = new DecimalFormat(xy_label_format);
//                            xy_label = "(" + df.format(c.x) + ", " + df.format(c.y) + ")";
//                        } catch (Exception exc) {}
//                        if (c.getPointLabelFormat() == POINT_LABEL_XY)
//                            label = xy_label;
//                        else if (c.getPointLabelFormat() == POINT_LABEL_NAME)
//                            label = c.name;
//                        else if (c.getPointLabelFormat() == POINT_LABEL_FULL)
//                            label = c.name + xy_label;
//
					if(mDrawCoordinates) 
					{
						Label label = new Label(labels[index++]);
						label.Position = new PointF(point.X, point.Y - 8);
						label.HorizontalAlign = HorizontalAlign.Center;
						label.VerticalAlign = VerticalAlign.Bottom;
						label.draw(g);
					}

//                        adlabel.setGraphics(g);
//                        if (c.getPointLabelPosition() == POINT_LABEL_POSITION_TOP) {
//                            adlabel.setPosition(pt.x, pt.y - symbol.getSize() - 3);
//                            adlabel.setAlign(B_CENTER);
//                        }
//                        else if (c.getPointLabelPosition() == POINT_LABEL_POSITION_BOTTOM) {
//                            adlabel.setPosition(pt.x, pt.y + symbol.getSize() + 3);
//                            adlabel.setAlign(T_CENTER);
//                        }
//                        else if (c.getPointLabelPosition() == POINT_LABEL_POSITION_LEFT) {
//                            adlabel.setPosition(pt.x - symbol.getSize() - 3, pt.y);
//                            adlabel.setAlign(M_RIGHT);
//                        }
//                        else if (c.getPointLabelPosition() == POINT_LABEL_POSITION_RIGHT) {
//                            adlabel.setPosition(pt.x + symbol.getSize() + 3, pt.y);
//                            adlabel.setAlign(M_LEFT);
//                        }
//                        adlabel.draw();

                }
            }
        }
	}
}
