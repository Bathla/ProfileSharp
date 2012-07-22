using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;

namespace SharpClient.UI.Charting
{
	/// <summary>
	/// Summary description for AbstractChartPage.
	/// </summary>
	public abstract class AbstractChartPage : Page
	{
        public abstract AbstractChart generateChart();

        protected override void OnInit(EventArgs e) {
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        private void Page_Load(object sender, System.EventArgs e) {
            AbstractChart chart = generateChart();
            Bitmap bitmap = new Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);

            chart.draw(g);

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            
            Response.Clear();
            Response.ContentType = "image/png";
            Response.BinaryWrite(stream.ToArray());
            g.Dispose();
        }
	}
}
