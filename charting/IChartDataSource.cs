using System;
using System.Collections;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for IChartDataSource.
	/// </summary>
	public interface IChartDataSource
	{
	    string Name {
            get;
        }
        string VerticalTitle {
            get; 
        }
        string HorizontalTitle {
            get;
        }
        ICollection SeriesCollection {
            get;
        }

    }
}
