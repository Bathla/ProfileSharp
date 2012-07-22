using System;
using System.Collections;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for DefaultDataSource.
	/// </summary>
	public class DefaultDataSource : IChartDataSource
	{
        private string m_name;
        private string m_verticalTitle;
        private string m_horizontalTitle;

        private ArrayList m_seriesCollection;

		public DefaultDataSource() : this(null)
		{
		}
        public DefaultDataSource(string name) {
            m_name = name; 
            m_seriesCollection = new ArrayList();
        }

        public void add(Series series) {
            m_seriesCollection.Add(series);
        }
        #region Implementation of IChartDataSource
    

        public string Name {
            get { return m_name; }
        }

        public string VerticalTitle {
            get { return m_verticalTitle; }
            set { m_verticalTitle = value; }
        }
        public string HorizontalTitle {
            get { return m_horizontalTitle; }
            set { m_horizontalTitle = value; }
        }
        public System.Collections.ICollection SeriesCollection {
            get { return m_seriesCollection; }
        }
        #endregion
	}
}
