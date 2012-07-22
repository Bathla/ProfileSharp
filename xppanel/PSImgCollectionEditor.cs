
using System ;
using System.Drawing ;
using System.ComponentModel ;
using System.ComponentModel.Design ;
using System.Drawing.Design ;

namespace PSUI.Helpers {
	/// <summary>
	/// Simple <see cref="UITypeEditor"/> which forwards to the standard <see cref="CollectionEditor"/>
	/// for <see cref="Image"/> types
	/// </summary>
	public class PSImgCollectionEditor : System.ComponentModel.Design.CollectionEditor {
		/// <summary>
		/// Create an <c>PSImgCollectionEditor</c>
		/// </summary>
		public PSImgCollectionEditor() : base(typeof(PSImgCollection)) {}

		/// <summary>
		/// Forward to base class implementation
		/// </summary>
		/// <param name="context">designer context</param>
		/// <param name="provider">designer service provider</param>
		/// <param name="value">value to be edited</param>
		/// <returns>
		/// The edited value
		/// </returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return base.EditValue(context, provider, value);
		}

		/// <summary>
		/// Forward to the normal <see cref="Image"/> editor
		/// </summary>
		/// <param name="ItemType">ignored</param>
		/// <returns>
		/// A new <see cref="Image"/>
		/// </returns>
		protected override object CreateInstance(Type ItemType) {
			UITypeEditor editor = ((UITypeEditor) TypeDescriptor.GetEditor(typeof(Image), typeof(UITypeEditor)));
			return ((Image) editor.EditValue(base.Context, null));
		}
	}
}
