using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpClient.UI.Charting {
    public enum Orientation {
        Horizontal, Vertical
    };

    public class AscendingDataComparer : IComparer {
    
        #region Implementation of IComparer
        public int Compare(object x, object y) {
            float xValue = ((DataPoint) x).Value;
            float yValue = ((DataPoint) y).Value;
            if (xValue < yValue)
                return -1;
            else if (xValue > yValue)
                return 1;
            else
                return 0;
        }
    
        #endregion
    }
    public class DescendingDataComparer : IComparer {
    
        #region Implementation of IComparer
        public int Compare(object x, object y) {
            float xValue = ((DataPoint) x).Value;
            float yValue = ((DataPoint) y).Value;
            if (xValue < yValue)
                return 1;
            else if (xValue > yValue)
                return -1;
            else
                return 0;
        }
    
        #endregion
    }

    /// <summary>
    /// Summary description for Barchart.
    /// </summary>
    public class Barchart : CartesianChart {

        private Orientation m_orientation = Orientation.Vertical;
        private bool m_enableStackChart = false;
        private ArrayList m_categoryNames;
        private Hashtable m_hashTable;

        public bool EnableStackChart {
            get { return m_enableStackChart; }
            set { m_enableStackChart = value; }
        }

        public Orientation Orientation {
            get { return m_orientation; }
            set { m_orientation = value; }
        }
        public Barchart(int width, int height) : base(width, height) {
        }
		public Barchart(Size size):base(size) 
		{
		}


        protected override void init() 
		{
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            string[] dataLabels = null;

            m_hashTable = new Hashtable();
            m_categoryNames = new ArrayList();
            int numOfSeries = DataSource.SeriesCollection.Count;
            foreach (Series series in DataSource.SeriesCollection) {
                minValue = minValue < series.MininumValue ? minValue : series.MininumValue;
                maxValue = maxValue < series.MaximumValue ? series.MaximumValue : maxValue;
                foreach (DataPoint data in series.DataPoints) {
                    Color color = data.Color;
                    if (numOfSeries > 1) {
                        color = series.Color;
                    }
                    ArrayList list = (ArrayList) m_hashTable[data.Name];
                    if (list == null) {
                        list = new ArrayList();
                        list.Add(new DataPoint(data.Name, data.Value, color));
                        m_categoryNames.Add(data.Name);
                        m_hashTable[data.Name] = list;
                    } else {
                        list.Add(new DataPoint(data.Name, data.Value, color));
                    }
                }
            }

            dataLabels = new string[m_categoryNames.Count + 2];
            for (int i = 0; i < m_categoryNames.Count; i++) {
                dataLabels[i + 1] = (string) m_categoryNames[i];
            }

            if (m_orientation == Orientation.Vertical) {
                XDataList = dataLabels;
                this.XMin = 0;
                this.XMax = dataLabels.Length - 1;
                this.YMin = minValue;
                this.YMax = maxValue;
            } else {
                YDataList = dataLabels;
                this.YMin = 0;
                this.YMax = dataLabels.Length - 1;
                this.XMin = minValue;
                this.XMax = maxValue;
            }

        }
        protected override void drawCartesianChart(Graphics g, Rectangle bound) {
            int numOfSeries = DataSource.SeriesCollection.Count;

            float oldValue = 0.0F;
            int columnPerGroup = numOfSeries;
            int columnIndex = 0;
            int groupIndex = 0;

            if (m_enableStackChart) {
                columnPerGroup = 1;
            }
            for (int index = 0; index < m_categoryNames.Count; index++) {
                ArrayList listValues = (ArrayList) m_hashTable[(string) m_categoryNames[index]];
                oldValue = 0.0F;
                ArrayList list = new ArrayList();
                foreach (DataPoint data in listValues) {
                    list.Add(data);
                }
                if (m_enableStackChart) {
                    list.Sort(new AscendingDataComparer());
                }

                for (int index1 = 0; index1 < list.Count; index1++) {
                    DataPoint data = (DataPoint) list[index1];
                    if (m_enableStackChart || numOfSeries == 1) {
                        columnIndex = 0;
                    } else {
                        columnIndex = index1;
                    }
                    groupIndex =index;

                    Label label = null;
                    
                    PointF[] pts = new PointF[2];
                    if (m_orientation == Orientation.Vertical) {
                        pts[0] = new PointF(groupIndex + .75F + columnIndex * .5F / columnPerGroup, data.Value);
                        pts[1] = new PointF(groupIndex + .75F + (columnIndex + 1) * .5F / columnPerGroup, oldValue);

                        transformPoints(pts);
                        int labelGap;
                        label = new Label(data.Value.ToString("#.##"));

                        if (data.Value <= 0) {
                            labelGap = 5;
                            label.VerticalAlign = VerticalAlign.Top;
                        } else {
                            labelGap = -5;
                            label.VerticalAlign = VerticalAlign.Bottom;
                        }
                        label.Position = new PointF((pts[0].X + pts[1].X) / 2, pts[0].Y + labelGap);
                        label.HorizontalAlign = HorizontalAlign.Center;

                    } else {
                        pts[0] = new PointF(oldValue, groupIndex + .75F + (columnIndex + 1) *.5F / columnPerGroup);
                        pts[1] = new PointF(data.Value, groupIndex + .75F + (columnIndex * .5F / columnPerGroup));
                        transformPoints(pts);

                        int labelGap;
                        label = new Label(data.Value.ToString());
                        if (data.Value <= 0) {
                            labelGap = -15;
                            label.HorizontalAlign = HorizontalAlign.Right;
                        } else {
                            labelGap = 15;
                            label.HorizontalAlign = HorizontalAlign.Left;
                        }
                        label.Position = new PointF(pts[1].X + labelGap, (pts[0].Y + pts[1].Y) / 2);
                        label.VerticalAlign = VerticalAlign.Middle;

                    }
                    draw3DRect(g,data.Color, new RectangleF(pts[0].X, pts[0].Y, pts[1].X - pts[0].X, pts[1].Y - pts[0].Y), Depth);
                    //
                    //                    if (show_label)
                    //                        label_value.draw();
                    if (IsShowLabel)
                        label.draw(g);
                    if (m_enableStackChart && data.Value > 0) {
                        oldValue = data.Value;
                    } else {
                        oldValue = 0.0F;
                    }
                }

            }


        }
    }
}
