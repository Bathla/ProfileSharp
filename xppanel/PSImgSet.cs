
using System ;
using System.ComponentModel ;
using System.Drawing ;
using System.Windows.Forms ;

namespace PSUI {
	/// <summary>
	/// Designer <see cref="Component"/> that wraps an <see cref="PSImgCollection"/>
	/// </summary>
	/// <remarks>
	/// <para>This class is <see cref="SerializableAttribute"/> but does not implement 
	/// <see cref="System.Runtime.Serialization.ISerializable"/></para>
	/// </remarks>
	[Serializable]
	public class PSImgSet : Component {
		#region Fields
		/// <summary>
		/// The <see cref="PSImgCollection"/>
		/// </summary>
		private PSImgCollection images ;
		#endregion Fields

		#region Constructor(s)
		/// <summary>
		/// Create an empty <c>PSImgSet</c>
		/// </summary>
		public PSImgSet() {
			images = new PSImgCollection() ;
		}

		/// <summary>
		/// Create an empty <c>PSImgSet</c> with the specified
		/// canonical <see cref="System.Drawing.Size"/>
		/// </summary>
		public PSImgSet(Size size) {
			images = new PSImgCollection(size) ;
		}

		/// <summary>
		/// Create an empty <c>PSImgSet</c> with the specified
		/// canonical <see cref="System.Drawing.Size"/> and
		/// transparent color mask value
		/// </summary>
		public PSImgSet(Size size,Color transparentColor) {
			images = new PSImgCollection(size,transparentColor) ;
		}

		/// <summary>
		/// Create an empty <c>PSImgSet</c> with the specified
		/// transparent color mask value
		/// </summary>
		public PSImgSet(Color transparentColor) {
			images = new PSImgCollection(transparentColor) ;
		}

		/// <summary>
		/// Create an <c>PSImgSet</c> from the specified <i>image strip</i>
		/// which contains the specified number of images
		/// </summary>
		/// <param name="images">The image strip</param>
		/// <param name="count">Number of images in the strip</param>
		public PSImgSet(Bitmap images,int count) {
			this.images = new PSImgCollection(images,count) ;
		}

		/// <summary>
		/// Create an <c>PSImgSet</c> from the specified <i>image strip</i>
		/// which contains the specified number of images and transparent
		/// color mask value
		/// </summary>
		/// <param name="images">The image strip</param>
		/// <param name="count">Number of images in the strip</param>
		/// <param name="transparentColor">Transparent color mask value</param>
		public PSImgSet(Bitmap images,int count,Color transparentColor) {
			this.images = new PSImgCollection(images,count,transparentColor) ;
		}
		#endregion Constructor(s)

		#region Properties
		/// <summary>
		/// Get/Set the transparent mask <see cref="Color"/> value
		/// </summary>
		[Description("Transparent color for images in the PSImgSet")]
		public Color TransparentColor {
			get {
				return images.TransparentColor ;
			}

			set {
				images.TransparentColor = value ;
			}
		}

		/// <summary>
		/// Get/Set the <see cref="PSImgCollection"/>
		/// </summary>
		/// <remarks>
		/// Requires <see cref="DesignerSerializationVisibility.Content"/> for proper
		/// code generation (??)
		/// </remarks>
		/// 
#if DEBUG
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(PSUI.Helpers.PSImgCollectionEditor),typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public PSImgCollection Images {
			get {
				return images ;
			}

			set {
				if (images != value) {
					images = value ;
				}
			}
		}

		/// <summary>
		/// Get an <see cref="ImageList"/> representation of the <c>PSImgSet</c>
		/// </summary>
		/// <remarks>
		/// Use <see cref="DesignerSerializationVisibility.Hidden"/> because this is not
		/// a 'real' property
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImageList ImageList {
			get {
				return images.ImageList ;
			}
		}

		/// <summary>
		/// Return an <i>image strip</i> representation of the <c>PSImgSet</c>
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image Preview {
			get {
				return images.Images ;
			}
		}

		/// <summary>
		/// Return the number of images in the <c>PSImgSet</c>
		/// </summary>
		[Description("Number of images in the PSImgSet")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ReadOnly(true)]
		public int Count {
			get {
				return images.Count ;
			}
		}

		/// <summary>
		/// Return the canonical <see cref="System.Drawing.Size"/> of the
		/// <c>PSImgSet</c>
		/// </summary>
		[Description("Dimensions for images in the PSImgSet")]
		public Size Size {
			get {
				return images.Size ;
			}

			set {
				images.Size = value ;
			}
		}

		/// <summary>
		/// Return the width of images in the <c>PSImgSet</c>
		/// </summary>
		/// <remarks>
		/// Use <see cref="DesignerSerializationVisibility.Hidden"/> because this is not
		/// a 'real' property
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Width {
			get {
				return images.Size.Width ;
			}
		}

		/// <summary>
		/// Return the height of images in the <c>PSImgSet</c>
		/// </summary>
		/// <remarks>
		/// Use <see cref="DesignerSerializationVisibility.Hidden"/> because this is not
		/// a 'real' property
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Height {
			get {
				return images.Size.Height ;
			}
		}
		#endregion Properties
	}
}
