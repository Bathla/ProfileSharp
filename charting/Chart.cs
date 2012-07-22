//using System;
//using System.Drawing;
//using System.Collections;
//
//namespace SharpClient.UI.Charting
//{
//	/// <summary>
//	/// Summary description for Chart.
//	/// </summary>
//	public abstract class Chart : Canvas
//	{
//        private string m_title;
//        private int m_depth = 30;
//        private ArrayList m_seriesCollection;
//        private Insets m_insets;
//        private Label m_titleLabel;
//
//
//        public int Depth {
//            get { return m_depth; }
//            set { m_depth = value; }
//        }
//
//        public string Title {
//            get { return m_title; }
//            set { 
//                m_title = value; 
//                m_titleLabel = new Label(value);
//            }
//        }
//
//        public void add(Series series) {
//            m_seriesCollection.Add(series);
//        }
//
//        public ArrayList SeriesCollection {
//            get { return m_seriesCollection; }
//        }
//
//		public Chart(int width, int height) : base(width, height)
//		{
//            m_seriesCollection = new ArrayList();
//            m_insets = new Insets(10, 10, 10, 10);
//		}
//
//        public override void draw(Graphics g) {
//            if (g == null) {
//                throw new ArgumentNullException("g");
//            }
//            drawTitle(g);
//            if (SeriesCollection.Count == 0) {
//                return;
//            }
//
//            drawChart(g, getRecommendedBound(g));
//
//        }
//
//        protected abstract void drawChart(Graphics g, Rectangle bound);
//
//        protected Rectangle getRecommendedBound(Graphics g) {
//            int top = m_insets.Top;
//
//            if (m_title != null) {
//                top += (int) m_titleLabel.getHeight(g);
//            }
//
//            return new Rectangle(m_insets.Left, 
//                top, 
//                this.Width - m_insets.Left - m_insets.Right, 
//                this.Height - m_insets.Top - m_insets.Bottom);
//    
//        }
//        protected void drawTitle(Graphics g) {
//            if (m_titleLabel == null) {
//                return;
//            }
//            m_titleLabel.Position = new Point(m_insets.Left + (this.Width - m_insets.Left - m_insets.Right) / 2, m_insets.Top);
//            m_titleLabel.draw(g);
//        }
//	}
//}
