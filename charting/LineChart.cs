using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for LineChart.
	/// </summary>
	public class LineChart : CartesianChart
	{
		public LineChart(int width, int height) : base(width, height)
		{
		}
		public LineChart(Size size):base(size) 
		{
		}
        protected override void init() {
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            int dataCount = 0;
            foreach (Series series in DataSource.SeriesCollection) {
                minValue = minValue < series.MininumValue ? minValue : series.MininumValue;
                maxValue = maxValue < series.MaximumValue ? series.MaximumValue : maxValue;                
                dataCount = series.DataPoints.Count;
            }
            this.YMin = minValue;
            this.YMax = maxValue;
            this.XMin = 0;
            this.XMax = dataCount + 1;
            XDataList = new string[dataCount + 2];
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
                    foreach (DataPoint data in series.DataPoints) {
                        points[index] = new PointF(index, data.Value);
                        //labels[index++] = String.Format("({0},{1})", data.X, data.Y);
                        index++;
                    }
                    transformPoints(points);
                    g.DrawLines(new Pen(series.Color), points);

                    // Draw symbol. Default is circle.
                    index = 0;
                    foreach (PointF point in points) {
                        g.FillEllipse(new SolidBrush(series.Color), point.X - 5, point.Y - 5, 10, 10);
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
//                        Label label = new Label(labels[index++]);
//                        label.Position = new PointF(point.X, point.Y - 8);
//                        label.HorizontalAlign = HorizontalAlign.Center;
//                        label.VerticalAlign = VerticalAlign.Bottom;
//                        label.draw(g);

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
