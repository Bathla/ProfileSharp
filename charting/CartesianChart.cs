using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpClient.UI.Charting
{

    public enum AxisType {
        Bottom, Left, Top, Right
    }

    public class Axis {
        private bool m_visible = true;
        private bool m_enableMajorTick = true;
        private bool m_enableMinorTick = true;
        private int m_majorTickCount = 5;
        private int m_minorTickCount = 10;
        private int m_majorTickLength = 5;
        private int m_minorTickLength = 2;
        private Color m_color = Color.Black;
        private Color m_tickColor = Color.Black;

        public bool Visible {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public bool EnableMajorTick {
            get { return m_enableMajorTick; }
            set { m_enableMajorTick = value; }
        }

        public bool EnableMinorTick {
            get { return m_enableMinorTick; }
            set { m_enableMinorTick = value; }
        }

        public int MajorTickCount {
			get 
			{ 
				try
				{					
					return m_majorTickCount;
					
				}
				catch
				{
					return 0;
				}
			}
            set { m_majorTickCount = value; }
        }
        public int MinorTickCount {
            get { return m_minorTickCount; }
            set { m_minorTickCount = value; }
        }
        public int MajorTickLength {
            get { return m_majorTickLength; }
            set { m_majorTickLength = value; }
        }
        public int MinorTickLength {
            get { return m_minorTickLength; }
            set { m_minorTickLength = value; }
        }

        public Color Color {
            get { return m_color; }
        }

        public Color TickColor {
            get { return m_color; }
        }

    }

	/// <summary>
	/// Summary description for CartesianChart.
	/// </summary>
    public abstract class CartesianChart : AbstractChart {
        private bool m_enableHorizontalGrid;
        private bool m_enableVerticalGrid;
        private bool m_isShowLabel;
        private bool m_xautoscale;
        private bool m_yautoscale;
        private float m_xmin = 0;
        private float m_xmax = 100;
        private float m_ymin = 0;
        private float m_ymax = 100;
        private Point[] m_vertices;
        private Axis[] m_axis;
        private Matrix m_matrix;
        private string[] m_xLabelList;
        private string[] m_yLabelList;
        private string[] m_customXLabelList;
        private string[] m_customYLabelList;
        private int m_maxXLabelHeight = 0;
        private int m_maxYLabelWidth = 0;

        public bool IsShowLabel {
            get { return m_isShowLabel; }
            set { m_isShowLabel = value; }
        }

        public bool EnableHorizontalGrid {
            get { return m_enableHorizontalGrid; }
            set { m_enableHorizontalGrid = value; }
        }
        public bool EnableVerticalGrid {
            get { return m_enableVerticalGrid; }
            set { m_enableVerticalGrid = value; }
        }

        public float XMin {
            get { return m_xmin; }
            set { m_xmin = value; }
        }

        public float XMax {
            get { return m_xmax; }
            set { m_xmax = value; }
        }
        public float YMin {
            get { return m_ymin; }
            set { m_ymin = value; }
        }
        public float YMax {
            get { return m_ymax; }
            set { m_ymax = value; }
        }
        
        public bool EnableAutoscale {
            set {
                m_xautoscale = value;
                m_yautoscale = value;
            }
        }
        public bool EnableXAutoscale {
            get { return m_xautoscale; }
            set { m_xautoscale = value; }
        }
        public bool EnableYAutoscale {
            get { return m_yautoscale; }
            set { m_yautoscale = value; }
        }

        public string[] XDataList {
            set { m_customXLabelList = value; }
        }
        public string[] YDataList {
            set { m_customYLabelList = value; }
        }

        public Axis LeftAxis {
            get { return m_axis[(int) AxisType.Left]; }
        }
        public Axis BottomAxis {
            get { return m_axis[(int) AxisType.Bottom]; }
        }
        public Axis TopAxis {
            get { return m_axis[(int) AxisType.Top]; }
        }
        public Axis RightAxis {
            get { return m_axis[(int) AxisType.Right]; }
        }
		public CartesianChart(int width, int height) : this( new Size(width, height)) 
		{
		}
		public CartesianChart(Size size) : base(size) 
		{
            m_axis = new Axis[4];
            m_axis[0] = new Axis();
            m_axis[1] = new Axis();
            m_axis[2] = new Axis();
            m_axis[3] = new Axis();
            m_enableHorizontalGrid = true;
            m_enableVerticalGrid = true;
            EnableAutoscale = true;

            TopAxis.Visible = false;
            RightAxis.Visible = false;
            EnableVerticalGrid = false;
            EnableHorizontalGrid = false;

        }
		
        public Axis this[AxisType type] {
            get {
                return m_axis[(int) type];
            }
        }

        /// <summary>
        /// List and order of vertices for the frame in cartesian class.
        ///     2 --------- 3
        ///    /|          /| 
        ///   / |         / |
        ///  /  |        /  |
        /// 1 --------- 4   |
        /// |   |       |   |
        /// |   7 --------- 6
        /// | /         |  / 
        /// |/          | / 
        /// 0 --------- 5
        /// </summary>
        private void calculateVertices(Graphics g, Rectangle bound) {
            m_vertices = new Point[8];

            m_vertices[0] = new Point(bound.X, bound.Y + bound.Height);
            m_vertices[1] = new Point(bound.X, bound.Y + Depth);
            m_vertices[2] = new Point(bound.X + Depth, bound.Y);
            m_vertices[3] = new Point(bound.X + bound.Width, bound.Y);
            m_vertices[4] = new Point(bound.X + bound.Width - Depth, bound.Y + Depth);
            m_vertices[5] = new Point(bound.X + bound.Width - Depth, bound.Y + bound.Height);
            m_vertices[6] = new Point(bound.X + bound.Width, bound.Y + bound.Height - Depth);
            m_vertices[7] = new Point(bound.X + Depth, bound.Y + bound.Height - Depth);
        }


        protected override void drawChart(Graphics g, Rectangle bound) {
            drawCartesianPlane(g, bound);
            drawCartesianChart(g, bound);
        }

        protected abstract void drawCartesianChart(Graphics g, Rectangle bound);

       
		private void drawCartesianPlane(Graphics g, Rectangle bound)
		{
			calculateVertices(g, bound);

			Point[] leftSide = new Point[4];
			leftSide[0] = m_vertices[0];
			leftSide[1] = m_vertices[1];
			leftSide[2] = m_vertices[2];
			leftSide[3] = m_vertices[7];

			Point[] backSide = new Point[4];
			backSide[0] = m_vertices[2];
			backSide[1] = m_vertices[3];
			backSide[2] = m_vertices[6];
			backSide[3] = m_vertices[7];

			Point[] bottomSide = new Point[4];
			bottomSide[0] = m_vertices[0];
			bottomSide[1] = m_vertices[7];
			bottomSide[2] = m_vertices[6];
			bottomSide[3] = m_vertices[5];

			g.FillPolygon(new SolidBrush(Color.LightGray), leftSide);
			g.FillPolygon(new SolidBrush(Color.DarkGray), bottomSide);

			#region Draw x axis at y = 0;
			PointF[] pts = new PointF[2];
			pts[0] = new PointF(0, 0);
			pts[1] = new PointF(m_xmin, m_ymin);
			transformPoints(pts);

			Matrix matrix = new Matrix();
			matrix.Translate(0, pts[0].Y - pts[1].Y);
			matrix.TransformPoints(bottomSide);
			g.FillPolygon(new SolidBrush(Color.DarkGray), bottomSide);
			#endregion

			#region Draw y axis at x = 0
			matrix.Reset();
			matrix.Translate(pts[0].X - pts[1].X, 0);
			matrix.TransformPoints(leftSide);
			g.FillPolygon(new SolidBrush(Color.LightGray), leftSide);
			#endregion
			Axis bottomAxis = this[AxisType.Bottom];
			Axis topAxis = this[AxisType.Top];
			Axis leftAxis = this[AxisType.Left];
			Axis rightAxis = this[AxisType.Right];

			#region Draw axises
			// Draw axises.
			// TODO: Is there a way to simplify these lines to loop.
			if (leftAxis.Visible) 
			{
				g.DrawLine(new Pen(leftAxis.Color), m_vertices[0], m_vertices[1]);
			}
			if (rightAxis.Visible) 
			{
				g.DrawLine(new Pen(rightAxis.Color), m_vertices[3], m_vertices[6]);
				g.DrawLine(new Pen(rightAxis.Color), m_vertices[5], m_vertices[6]);
			}
			if (bottomAxis.Visible) 
			{
				g.DrawLine(new Pen(bottomAxis.Color), m_vertices[0], m_vertices[5]);
			}
			if (topAxis.Visible) 
			{
				g.DrawLine(new Pen(topAxis.Color), m_vertices[1], m_vertices[2]);
				g.DrawLine(new Pen(topAxis.Color), m_vertices[2], m_vertices[3]);
			}

			#endregion
			#region Draw vertical label.
			if (DataSource.VerticalTitle != null) 
			{
				Label title = new Label(DataSource.VerticalTitle);
				title.Position = new PointF(m_vertices[0].X - m_maxYLabelWidth - 5, (m_vertices[0].Y + m_vertices[1].Y) / 2);
				title.RotateAngle = -90;
				title.HorizontalAlign = HorizontalAlign.Center;
				title.VerticalAlign = VerticalAlign.Bottom;
				title.draw(g);
			}
			#endregion
			#region Draw horizontal label.
			if (DataSource.HorizontalTitle != null) 
			{
				Label title = new Label(DataSource.HorizontalTitle);
				title.Position = new PointF((m_vertices[0].X + m_vertices[5].X) / 2 , m_vertices[0].Y + m_maxXLabelHeight + 5);
				title.VerticalAlign = VerticalAlign.Top;
				title.HorizontalAlign = HorizontalAlign.Center;
				title.draw(g);
			}
			#endregion
			#region Draw vertical grid, major & minor tick.
			// Draw grid & major & minor tick
			float dx = (bound.Width - this.Depth) / (bottomAxis.MajorTickCount - 1);
			float dx1 = dx / bottomAxis.MinorTickCount;

			int i = 0;
			float majorOffset = 0.0F;
			for (i = 0, majorOffset = 0.0F; i < bottomAxis.MajorTickCount; i++, majorOffset += dx)
			{
				if (i == (bottomAxis.MajorTickCount - 1)) 
				{
					majorOffset = bound.Width - this.Depth;
                
				}
				if (m_enableVerticalGrid) 
				{
					Pen pen = new Pen(Color.Black);
					g.DrawLine(pen, m_vertices[0].X + majorOffset, m_vertices[0].Y, m_vertices[7].X + majorOffset, m_vertices[7].Y);
					g.DrawLine(pen, m_vertices[7].X + majorOffset, m_vertices[7].Y, m_vertices[2].X + majorOffset, m_vertices[2].Y);
				}
				//
				//                g.setColor(tick_color);
				int i1;
				float minorOffset;
				Pen tickPen = new Pen(bottomAxis.TickColor);
				for (i1 = 0, minorOffset = majorOffset; i1 < bottomAxis.MinorTickCount && i != (bottomAxis.MajorTickCount - 1); i1++, minorOffset += dx1) 
				{
					//                    if (draw_tick_mark[BOTTOM_AXIS][MINOR_TICK])
					//                        g.drawLine(vertices[0].x + (int) minor_offset, vertices[0].y - tick_length[BOTTOM_AXIS][MINOR_TICK], vertices[0].x + (int) minor_offset, vertices[0].y);		
					if (BottomAxis.Visible && BottomAxis.EnableMinorTick) 
					{
						g.DrawLine(tickPen, 
							m_vertices[0].X + minorOffset, 
							m_vertices[0].Y - bottomAxis.MinorTickLength, 
							m_vertices[0].X + minorOffset, 
							m_vertices[0].Y); 
					}


					//                    if (draw_tick_mark[TOP_AXIS][MINOR_TICK])
					//                        g.drawLine(vertices[2].x + (int) minor_offset, vertices[2].y + tick_length[TOP_AXIS][MINOR_TICK], vertices[2].x + (int) minor_offset, vertices[2].y);		
					if (TopAxis.Visible && TopAxis.EnableMinorTick) 
					{
						g.DrawLine(tickPen, 
							m_vertices[2].X + minorOffset, 
							m_vertices[2].Y + topAxis.MinorTickLength, 
							m_vertices[2].X + minorOffset, 
							m_vertices[2].Y);
					}
				}   
				//	    
				//                if (draw_tick_mark[BOTTOM_AXIS][MAJOR_TICK])
				//                    g.drawLine(vertices[0].x + (int) major_offset, vertices[0].y - tick_length[BOTTOM_AXIS][MAJOR_TICK], vertices[0].x + (int) major_offset, vertices[0].y);
				if (BottomAxis.Visible && BottomAxis.EnableMajorTick) 
				{
					g.DrawLine(tickPen, 
						m_vertices[0].X + majorOffset, 
						m_vertices[0].Y - bottomAxis.MajorTickLength, 
						m_vertices[0].X + majorOffset, 
						m_vertices[0].Y);
				}

				//                if (draw_tick_mark[TOP_AXIS][MAJOR_TICK])
				//                    g.drawLine(vertices[2].x + (int) major_offset, vertices[2].y + tick_length[TOP_AXIS][MAJOR_TICK], vertices[2].x + (int) major_offset, vertices[2].y);
				if (TopAxis.Visible && TopAxis.EnableMajorTick) 
				{
					g.DrawLine(tickPen, 
						m_vertices[2].X + majorOffset, 
						m_vertices[2].Y + bottomAxis.MajorTickLength, 
						m_vertices[2].X + majorOffset, 
						m_vertices[2].Y);
				}


				
				if (i < m_xLabelList.Length ) 
				{
					string strLabel=m_xLabelList[i];					
					if(strLabel!=null)
					{
						if(strLabel.IndexOf("::")==-1 && strLabel.IndexOf("\\")==-1)
						{
							Label label = new Label(strLabel);
							label.Position = new Point(m_vertices[0].X + (int) majorOffset, m_vertices[0].Y);
							label.VerticalAlign = VerticalAlign.Top;
							label.HorizontalAlign = HorizontalAlign.Center;
							label.draw(g);

						}
					}
					else
					{
						Label label = new Label(strLabel);
						label.Position = new Point(m_vertices[0].X + (int) majorOffset, m_vertices[0].Y);
						label.VerticalAlign = VerticalAlign.Top;
						label.HorizontalAlign = HorizontalAlign.Center;
						label.draw(g);
					}
					
				}	

				//                if (i < x_label_list.length) {
				//                    if (label_major_tick[BOTTOM_AXIS]) {
				//                        adLabel label = new adLabel(x_label_list[i]);
				//                        label.setPosition(vertices[0].x + (int) major_offset, vertices[0].y + space);
				//                        label.setNumberFormat(x_label_format);
				//                        label.setAlign(adLabel.T_CENTER);
				//                        label.setGraphics(g);
				//                        label.draw();
				//                    }
				//	    
				//                    if (label_major_tick[TOP_AXIS]) {
				//                        adLabel label = new adLabel(x_label_list[i]);
				//                        label.setPosition(vertices[2].x + (int) major_offset, vertices[2].y - space);
				//                        label.setAlign(adLabel.B_CENTER);
				//                        label.setNumberFormat(x_label_format);
				//                        label.setGraphics(g);
				//                        label.draw();
				//                    }
				//                }
			}
			#endregion

			#region Draw horizontal grid, major & minor tick.
			float dy = (float) (bound.Height - this.Depth) / ((float) leftAxis.MajorTickCount - 1.0F);
			float dy1 = dy / leftAxis.MinorTickCount;
			for (i = leftAxis.MajorTickCount - 1, majorOffset = 0.0F; i >= 0; i--, majorOffset += dy)
			{
				if (i == 0)
					majorOffset = bound.Height - this.Depth;
				if (m_enableHorizontalGrid) 
				{
					Pen pen = new Pen(Color.Black);
					g.DrawLine(pen, 
						m_vertices[1].X, 
						m_vertices[1].Y + majorOffset, 
						m_vertices[2].X, 
						m_vertices[2].Y + majorOffset);
					g.DrawLine(pen, 
						m_vertices[2].X, 
						m_vertices[2].Y + majorOffset, 
						m_vertices[3].X, 
						m_vertices[3].Y + majorOffset);
				}
				int i1;
				float minorOffset;
				Pen tickPen = new Pen(leftAxis.TickColor);
				for (i1 = 0, minorOffset = majorOffset; i1 < leftAxis.MinorTickCount && i != 0; i1++, minorOffset += dy1) 
				{
					if (LeftAxis.Visible && LeftAxis.EnableMinorTick) 
					{
						g.DrawLine(tickPen, 
							m_vertices[1].X, 
							m_vertices[1].Y + minorOffset, 
							m_vertices[1].X + leftAxis.MinorTickLength, 
							m_vertices[1].Y + minorOffset);
					}
					if (RightAxis.Visible && RightAxis.EnableMinorTick) 
					{
						g.DrawLine(tickPen, 
							m_vertices[3].X, 
							m_vertices[3].Y + minorOffset, 
							m_vertices[3].X - leftAxis.MinorTickLength, 
							m_vertices[3].Y + minorOffset);
					}
				}		    
				if (LeftAxis.Visible && LeftAxis.EnableMajorTick) 
				{
					g.DrawLine(tickPen, 
						m_vertices[1].X, 
						m_vertices[1].Y + majorOffset, 
						m_vertices[1].X + leftAxis.MajorTickLength, 
						m_vertices[1].Y + majorOffset);
				}
				if (RightAxis.Visible && RightAxis.EnableMajorTick) 
				{
					g.DrawLine(tickPen, 
						m_vertices[3].X, 
						m_vertices[3].Y + majorOffset, 
						m_vertices[3].X - leftAxis.MajorTickLength, 
						m_vertices[3].Y + majorOffset);
				}
				if (i < m_yLabelList.Length) 
				{
					Label label = new Label(m_yLabelList[i]);
					label.Position = new Point(m_vertices[1].X , m_vertices[1].Y + (int) majorOffset);
					label.HorizontalAlign =HorizontalAlign.Right;
					label.VerticalAlign = VerticalAlign.Middle;
					label.draw(g);
				}

				
			}
			#endregion

		}

        private void calculateViewPortMatrix() 
		{
            int axisHeight = m_vertices[0].Y - m_vertices[1].Y;
            int axisWidth = m_vertices[5].X - m_vertices[0].X;

            m_matrix = new Matrix();
            m_matrix.Translate(-1 * m_xmin * axisWidth / (m_xmax - m_xmin) + m_vertices[0].X, m_ymin * axisHeight / (m_ymax - m_ymin) + m_vertices[0].Y, MatrixOrder.Prepend);
            m_matrix.Scale(axisWidth / (m_xmax - m_xmin) , axisHeight / (m_ymin - m_ymax), MatrixOrder.Prepend);
        }

        public void transformPoints(PointF[] points) {
//            if (m_matrix == null) {
            calculateViewPortMatrix();
//          }
            m_matrix.TransformPoints(points);
        }

        public void generateAxisLabel() {
            m_xLabelList = new String[this[AxisType.Bottom].MajorTickCount + 1];
            m_yLabelList = new String[this[AxisType.Left].MajorTickCount + 1];

            float val;
            float majorTickStep;

            if (m_customXLabelList != null) {
                if (m_customXLabelList.Length > 0) {
                    this[AxisType.Bottom].MajorTickCount = m_customXLabelList.Length;
                    m_xLabelList = m_customXLabelList;
                }
            } else {
                if (!m_xautoscale) {
                    majorTickStep = (m_xmax - m_xmin) / (this[AxisType.Bottom].MajorTickCount - 1);
                } else {
                    if (m_xmin >= 0)
                        m_xmin = 0;
	    
                    majorTickStep = calculateStepAndTickCount(AxisType.Bottom, m_xmax - m_xmin);

                    if (m_xmin < 0) {
                        int tmp = (int) Math.Abs(m_xmin / majorTickStep) + 1;
                        m_xmin = -1 * Math.Abs(tmp * majorTickStep);
                        this[AxisType.Bottom].MajorTickCount++;
                    }
                    float oldXMax = m_xmax;
                    m_xmax = m_xmin + (this[AxisType.Bottom].MajorTickCount - 1) * majorTickStep;
		
                    if (oldXMax >= m_xmax) {
                        m_xmax += majorTickStep;
                        this[AxisType.Bottom].MajorTickCount++;
                    }

                }
                val = m_xmin;
                m_xLabelList = new String[this[AxisType.Bottom].MajorTickCount + 1];

                for (int index = 0; index < m_xLabelList.Length; index++, val+= majorTickStep) 
                    m_xLabelList[index] = val.ToString();
            }

            if (m_customYLabelList != null) {
                if (m_customYLabelList.Length > 0) {
                    this[AxisType.Left].MajorTickCount = m_customYLabelList.Length;
                    m_yLabelList = m_customYLabelList;
                }
            } else {
                if (!m_yautoscale) {
                    majorTickStep = (m_ymax - m_ymin) / (this[AxisType.Left].MajorTickCount - 1);
                } else {
                    if (m_ymin >= 0)
                        m_ymin = 0;
		
                    majorTickStep = calculateStepAndTickCount(AxisType.Left, m_ymax - m_ymin);
                    if (m_ymin < 0) {
                        int tmp = (int) Math.Floor(Math.Abs(m_ymin / majorTickStep)) + 1;
		    
                        m_ymin = -1 * Math.Abs(tmp * majorTickStep);
                        this[AxisType.Left].MajorTickCount++;
                    }
		
                    float oldYMax = m_ymax;
                    m_ymax = m_ymin + (this[AxisType.Left].MajorTickCount - 1) * majorTickStep;
                    if (oldYMax >= m_ymax) {
                        m_ymax += majorTickStep;
                        this[AxisType.Left].MajorTickCount++;
                    }
                }
                val = m_ymin;
                m_yLabelList = new String[this[AxisType.Left].MajorTickCount + 1];
                for (int index = 0; index < m_yLabelList.Length; index++, val+= majorTickStep) 
                    m_yLabelList[index] = val.ToString();
            }

        }

        private void setMaxXLabelHeight(Graphics g) {

            for (int index = 0; index <= this[AxisType.Bottom].MajorTickCount; index++) {
                if (index < m_xLabelList.Length) {
                    Label label = new Label(m_xLabelList[index]);
                    if (label.getHeight(g) > m_maxXLabelHeight)
                        m_maxXLabelHeight = label.getHeight(g);
                }
            }
        }

        private void setMaxYLabelWidth(Graphics g) {
            for (int index = 0; index <= this[AxisType.Left].MajorTickCount; index++) {
                if (index < m_yLabelList.Length) {
                    Label label = new Label(m_yLabelList[index]);
                    if (label.getWidth(g) > m_maxYLabelWidth)
                        m_maxYLabelWidth = label.getWidth(g);
                }
            }
        }

        private float roundUp(float val) {
            int exponent = (int) Math.Floor(Math.Log10(Math.Abs(val)));

            // normalize
            if (exponent < 0) {
                for (int i = exponent; i < 0; i++)
                    val *= 10.0F;
            } else {
                for (int i = 0; i < exponent; i++) 
                    val *= .1F;
            }
            // now round to one of four nice numbers for tics
            if      (val > 5.0F) val = 10.0F;
            else if (val > 2.0F) val =  5.0F;
            else if (val > 1.0F) val =  2.0F;
            else if (val > 0.0F) val =  1.0F;
            else if (val > -1.0F) val =  -1.0F;
            else if (val > -2.0F) val =  -2.0F;
            else if (val > -5.0F) val =  -5.0F;
            else                  val = -10.0F;
	

            // de-normalize
            if (exponent < 0) {
                for (int i = exponent; i < 0; i++) 
                    val *= 0.1F;
            } else {
                for (int i=0; i < exponent; i++) 
                    val *= 10.0F; 
            }
          
            return val;
        }

        private float calculateStepAndTickCount(AxisType axis, float range) {
            int exponent = (int) Math.Floor(Math.Log10(Math.Abs(range)));
	
            float[] tickIncrement = {1, 2, 5, 10};
            int[] numOfTick = {0, 0, 0, 0};
            // Recalculate all possibles tick increment.
            for (int i = 0; i < tickIncrement.Length; i++)
                tickIncrement[i] = (float) (tickIncrement[i] * Math.Pow(10, exponent));

            // Calculate number of tick marks required.
            for (int i = 0; i < tickIncrement.Length; i++) {
                numOfTick[i] = (int) (Math.Floor(range / tickIncrement[i])) + 1;
            }

            // Find the optimal number of tick marks.
            int min = this[axis].MajorTickCount;
            int minIndex = 0;
            for (int i = 0; i < tickIncrement.Length; i++) {

                if (min > Math.Abs(numOfTick[i] - this[axis].MajorTickCount)) {
                    minIndex = i;
                    min = Math.Abs(numOfTick[i] - this[axis].MajorTickCount);
                }
            }

            this[axis].MajorTickCount = numOfTick[minIndex];
            return tickIncrement[minIndex];
	    
        }

        protected override void calculateInsets(SharpClient.UI.Charting.Insets insets, System.Drawing.Graphics g) {
            base.calculateInsets(insets, g);
            generateAxisLabel();
            setMaxXLabelHeight(g);
            setMaxYLabelWidth(g);
            insets.Left += (m_maxYLabelWidth + m_maxXLabelHeight);
            //insets.Top += m_maxXLabelHeight;
            insets.Bottom += 3 * m_maxXLabelHeight;
            insets.Right += 2 * m_maxYLabelWidth;
        }
    }
}
