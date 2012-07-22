using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;

namespace PSUI.Helpers {
	/// <summary>
	/// PSImgMapEditor provides a drop-down pop-up of all images in the
	/// associated <see cref="PSImgSet"/>
	/// </summary>
	public class PSImgMapEditor	: UITypeEditor {
		#region Fields
		/// <summary>
		/// Service used to provide the image popup
		/// </summary>
		private IWindowsFormsEditorService wfes = null ;

		/// <summary>
		/// Image selected from the popup (-1 is cancel)
		/// </summary>
		private int selectedIndex = -1 ;

		/// <summary>
		/// Instance of <see cref="PSImgPanel"/> for the drop-down pop-up
		/// </summary>
		private PSImgPanel imagePanel = null ;
		#endregion Fields

		/// <summary>
		/// Create an <c>PSImgMapEditor</c>
		/// </summary>
		public PSImgMapEditor() {}

		/// <summary>
		/// Tell the designer we use the <see cref="UITypeEditorEditStyle.DropDown"/>
		/// style
		/// </summary>
		/// <param name="context">designer context</param>
		/// <returns>
		/// <see cref="UITypeEditorEditStyle.DropDown"/> if the context and instance
		/// are valid, otherwise whatever the base class says
		/// </returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null ) {
				return UITypeEditorEditStyle.DropDown ;
			}
			return base.GetEditStyle(context) ;
		}

		/// <summary>
		/// Extract the <see cref="PSImgSet"/> associated with the instance
		/// being edited
		/// </summary>
		/// <param name="component">The item being edited</param>
		/// <returns>
		/// The edited items associated <see cref="PSImgSet"/> or 
		/// <see langword="null"/> if it cant be found or is 
		/// undefined
		/// </returns>
		protected virtual PSImgSet GetPSImgSet(object component) {
			if (component is PSImgItemCollection) {
				return ((PSImgItemCollection) component).PSImgSet ;
			}

			return null ;
		}

		/// <summary>
		/// Yes we paint values
		/// </summary>
		/// <param name="context">designer context</param>
		/// <returns>
		/// <see langword="true"/>
		/// </returns>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}

		/// <summary>
		/// Paint a preview of the <see cref="Image"/> specified by
		/// the image index provided by the <see cref="PaintValueEventArgs"/>
		/// </summary>
		/// <param name="pe">The PaintValue event args</param>
		public override void PaintValue(PaintValueEventArgs pe) {
			int imageIndex = -1 ;	

			// value is the image index
			if(pe.Value != null) {
				try {
					imageIndex = (int)Convert.ToUInt16( pe.Value.ToString() ) ;
				}
				catch {}
			}

			// no instance, or the instance represents an undefined image
			if((pe.Context.Instance == null) || (imageIndex < 0))
				return ;

			// get the image set
			PSImgSet imageSet = GetPSImgSet(pe.Context.Instance) ;

			// make sure everything is valid
			if((imageSet == null) || (imageSet.Count == 0) || (imageIndex >= imageSet.Count))
				return ;

			// Draw the preview image
			pe.Graphics.DrawImage(imageSet.Images[imageIndex],pe.Bounds);
		}

		/// <summary>
		/// When editing an image index value, let the user choose an image from
		/// a popup that displays all the images in the associated <see cref="PSImgSet"/>
		/// </summary>
		/// <param name="context">designer context</param>
		/// <param name="provider">designer service provider</param>
		/// <param name="value">image index item</param>
		/// <returns>
		/// An image index (selected from the popup) or -1 if the user canceled the
		/// selection
		/// </returns>
		public override object EditValue(ITypeDescriptorContext context,IServiceProvider provider,object value) {
			wfes = (IWindowsFormsEditorService)
				provider.GetService(typeof(IWindowsFormsEditorService));

			if((wfes == null) || (context == null))
				return null ;

			// Get the image set
			PSImgSet imageSet = GetPSImgSet(context.Instance) ;

			// anything to show?
			if ((imageSet == null) || (imageSet.Count==0))
				return -1 ;

			// Create an image panel that is close to square
			Size dims = PSImgPanel.CalculateBestDimensions(imageSet.Count,PSImgPanel.PanelSizeHints.MinimizeBoth) ;
			imagePanel = new PSImgPanel((Bitmap) imageSet.Preview,imageSet.Count,dims.Height,dims.Width) ;
			// set the current image index value as the default selection
			imagePanel.DefaultImage = (int) value ;
			// no grid
			imagePanel.GridColor = Color.Empty ;
			// listen for an image to be selected
			imagePanel.ImageSelected += new EventHandler(imagePanel_ImageSelected);

			// show the popup as a drop-down
			wfes.DropDownControl(imagePanel) ;
			
			// return the selection (or the original value if none selected)
			return (selectedIndex != -1) ? selectedIndex : (int) value ;
		}

		/// <summary>
		/// <see cref="PSImgPanel.ImageSelected"/> listener
		/// </summary>
		/// <param name="sender">The <see cref="PSImgPanel"/></param>
		/// <param name="e"><see cref="PSImgEventArgs"/> specifying the selection (or -1)</param>
		private void imagePanel_ImageSelected(object sender, EventArgs e) {
			// get the selection
			selectedIndex = ((PSImgEventArgs) e).ImageIndex ;
			// remove the listener
			imagePanel.ImageSelected -= new EventHandler(imagePanel_ImageSelected);
			// close the drop-dwon, we are done
			wfes.CloseDropDown() ;

			imagePanel.Dispose() ;
			imagePanel = null ;
		}
	}
}