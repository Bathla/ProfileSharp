using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D ;
using System.Windows.Forms;
using System.Drawing.Imaging ;

namespace PSUI {
	/// <summary>
	/// PSItmLayoutPanel control
	/// </summary>
	public class PSItmLayoutPanel : Panel, ISupportInitialize {
		#region interface IItemLayout 
		/// <summary>
		/// Interface used by <see cref="PSItmLayoutPanel"/> to provide item layout
		/// functionality
		/// </summary>
		public interface IItemLayout {
			/// <summary>
			/// Layout the child controls for the specified <see cref="Control.Parent"/>
			/// </summary>
			/// <param name="parent">The parent control whose children need to be layed out</param>
			/// <returns>
			/// The "best height" for <c>Parent</c>
			/// </returns>
			/// <remarks>
			/// This method should be implemented as:
			/// <code>
			///		return LayoutItems(parent,parent.Controls) ;
			/// </code>
			/// </remarks>
			int LayoutItems(Control parent) ;

			/// <summary>
			/// Layout a set of child controls for the specified <see cref="Control.Parent"/>
			/// </summary>
			/// <param name="parent">The parent control</param>
			/// <param name="controls">The set of controls to be layed out</param>
			/// <returns>
			/// The "best height" for <c>Parent</c>
			/// </returns>
			/// <remarks>
			/// This method provides an oppurtunity for selective layout, or to specify a
			/// different control order than the default
			/// </remarks>
			int LayoutItems(Control parent,IEnumerable controls) ;
		}
		#endregion interface IItemLayout 

		#region class DefaultItemLayout
		/// <summary>
		/// Default implementation of <see cref="IItemLayout"/> used by <see cref="PSItmLayoutPanel"/>
		/// when a custom <see cref="IItemLayout"/> is not specified 
		/// </summary>
		/// <remarks>
		/// This is a simple algorithm for (left) aligning and spacing items within an <see cref="PSItmLayoutPanel"/>
		/// <seealso cref="PSItmLayoutPanel.ItemLayout"/>
		/// </remarks>
		class DefaultItemLayout : IItemLayout {
			#region IItemLayout Members
			/// <summary>
			/// Layout all visible controls, left-aligned and using the spacing
			/// information provided by the parent
			/// </summary>
			/// <param name="parent">An instance of PSItmLayoutPanel</param>
			/// <param name="controls">The controls to be layed out</param>
			/// <returns>
			/// The offset of the bottom of the last placed control + the
			/// <see cref="PSUI.PSItmLayoutPanel.BorderMargin"/>
			/// </returns>
			public int LayoutItems(Control parent,IEnumerable controls) {
				PSItmLayoutPanel itemLayoutPanel = parent as PSItmLayoutPanel ;

				// Initialize these variables in this manner so that if
				// we have no controls the best size is BorderMargin.Height * 2
				int yOffset = itemLayoutPanel.BorderMargin.Height ;
				int ySpacing = 0 ;

				foreach(Control control in controls) {
					if (!control.Visible)
						continue ;

					// set the top to the bottom of the last control (if any)
					// + the spacing. If this is the 1st control ySpacing is zero
					// and yOffset = BorderMargin.Height
					control.Top = yOffset + ySpacing ;

					control.Left = itemLayoutPanel.BorderMargin.Width ;

					// point to the bottom of the current control
					yOffset += control.Height + ySpacing ;

					// next time around (if any) add this much spacing)
					ySpacing = itemLayoutPanel.ItemSpacing ;
				}

				// The bottom of the last control + the BorderMargin spacing
				return yOffset + itemLayoutPanel.BorderMargin.Height ;
			}

			/// <summary>
			/// Layout the child controls
			/// </summary>
			/// <param name="parent">An instance of <see cref="PSItmLayoutPanel"/></param>
			/// <returns>The best height for the <see cref="PSItmLayoutPanel"/></returns>
			public int LayoutItems(Control parent) {
				return LayoutItems(parent,parent.Controls) ;
			}
			#endregion
		}
		#endregion class DefaultItemLayout

		#region enum PSItmLayoutPanelProperty
		/// <summary>
		/// Enumeration used for <see cref="PSItmLayoutPanel.PropertyChange"/> events
		/// </summary>
		public enum PSItmLayoutPanelProperty {
			/// <summary>
			/// <see cref="PSItmLayoutPanel.PanelGradient"/> property
			/// </summary>
			PanelGradientProperty,
			/// <summary>
			/// <see cref="PSItmLayoutPanel.BorderMargin"/> property
			/// </summary>
			BorderMarginProperty,
			/// <summary>
			/// <see cref="PSItmLayoutPanel.ItemSpacing"/> property
			/// </summary>
			ItemSpacingProperty,
			/// <summary>
			/// <see cref="PSItmLayoutPanel.AutoSize"/> property
			/// </summary>
			AutoSizeProperty,
			/// <summary>
			/// <see cref="PSItmLayoutPanel.ItemLayout"/> property
			/// </summary>
			ItemLayoutProperty,
			/// <summary>
			/// <see cref="PSItmLayoutPanel.BackgroundStyle"/> property
			/// </summary>
			BackgroundStyleProperty,
			/// <summary>
			/// <see cref="Control.BackColor"/> property
			/// </summary>
			BackColorProperty
		}
		#endregion enum PSItmLayoutPanelProperty

		#region class PropertyChangeEventArgs (and related)
	/// <summary>
	/// Delegate signature for <see cref="PSItmLayoutPanel.PropertyChange"/> events
	/// </summary>
	public delegate void PropertyChangeHandler(PSItmLayoutPanel itemLayoutPanel,PropertyChangeEventArgs e) ;

	/// <summary>
	/// EventArgs for <see cref="PSPanel.PropertyChange"/> events
	/// </summary>
	public class PropertyChangeEventArgs : System.EventArgs {
		/// <summary>
		/// The property that changed
		/// </summary>
		private readonly PSItmLayoutPanelProperty property ;

		/// <summary>
		/// Create a new <c>PSPanelPropertyChangeArgs</c> with the specified
		/// property enumeration value
		/// </summary>
		/// <param name="property"></param>
		public PropertyChangeEventArgs(PSItmLayoutPanelProperty property) {
			this.property = property ;
		}

		/// <summary>
		/// Get the <see cref="PSPanelProperties"/> property that changed
		/// </summary>
		public PSItmLayoutPanelProperty PSPanelProperty {
			get {
				return property ;
			}
		}
	}
		#endregion class PropertyChangeEventArgs

		#region Constants
		/// <summary>
		/// Default value for <see cref="PanelGradient"/>
		/// </summary>
		public static readonly GradientColor DefaultPanelGradient = new GradientColor(Color.CornflowerBlue) ;

		/// <summary>
		/// Default value for <see cref="BorderMargin"/>
		/// </summary>
		public static readonly Size DefaultBorderMargin = new Size(8,8) ;

		/// <summary>
		/// Default value for <see cref="ItemSpacing"/>
		/// </summary>
		public const int DefaultSpacing = 8 ;
		#endregion Constants

		#region Static Fields
		/// <summary>
		/// Default <see cref="IItemLayout"/> handler used when a custom <see cref="IItemLayout"/>
		/// is not specified.
		/// <seealso cref="PSItmLayoutPanel.ItemLayout"/>
		/// </summary>
		private static DefaultItemLayout defaultItemLayout = new DefaultItemLayout() ;
		#endregion Static Fields

		#region Fields
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// The <see cref="GradientColor"/> for the background of the <c>PSItmLayoutPanel</c>
		/// </summary>
		private GradientColor panelGradient = new GradientColor(DefaultPanelGradient) ;

		/// <summary>
		/// Controls left/top/right spacing of items/controls controls within the <c>PSItmLayoutPanel</c>
		/// </summary>
		private Size borderMargin = new Size(DefaultBorderMargin.Width,DefaultBorderMargin.Height) ;

		/// <summary>
		/// Controls the Y spacing between <see cref="TextElement"/> items in the <c>PSItmLayoutPanel</c>
		/// </summary>
		private int itemSpacing = DefaultSpacing ;

		/// <summary>
		/// Gradient brush used to draw the background
		/// </summary>
		private LinearGradientBrush backgroundBrush = null ;

		/// <summary>
		/// <see langword="true"/> if the <c>PSItmLayoutPanel</c> auto-sizes itself based upon content
		/// </summary>
		private bool autoSize = false ;

		/// <summary>
		/// Item layout handler. <seealso cref="ItemLayout"/>
		/// </summary>
		private IItemLayout itemLayoutHandler = null ;

		/// <summary>
		/// Default background style is not to have one (i.e, Transparent)
		/// </summary>
		private BackgroundStyle backgroundStyle = BackgroundStyle.Transparent ;

		/// <summary>
		/// <see langword="true"/> when we are in InitializeComponent()
		/// </summary>
		/// <remarks>
		/// <see cref="ISupportInitialize"/>
		/// </remarks>
		private bool isInitializingComponent = false ;

		/// <summary>
		/// Stores items in runtime preferred order, which may different
		/// from order of controls in <see cref="Control.Controls"/>
		/// </summary>
		private ArrayList items = new ArrayList() ;

			#region Events
		[NonSerialized]
		private PropertyChangeHandler propertyChangeListeners = null ;
			#endregion Events
		#endregion Fields

		#region Constructor(s)
		/// <summary>
		/// Create an PSItmLayoutPanel
		/// </summary>
		/// <remarks>
		/// The <see cref="Control.BackColor"/> property is set to <see cref="Color.Transparent"/>, and
		/// the <see cref="ScrollableControl.AutoScroll"/> property is set to <see langword="true"/>
		/// </remarks>
		public PSItmLayoutPanel() {
			// Default values for these properties although neither is suppressed
			BackColor = Color.Transparent ;
			// AutoScroll = true ;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Use these control styles for smoother drawing and transparency support
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.Selectable,true) ;
		}		
		#endregion Constructor(s)

		#region Dispose
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}
		#endregion Dispose

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region Properties
		/// <summary>
		/// <see langword="true"/> if the <c>PSItmLayoutPanel</c> has one or more child controls
		/// </summary>
		[Browsable(false)]
		public bool HasItems {
			get {
				return items.Count > 0 ;
			}
		}

		/// <summary>
		/// Get the <see cref="ArrayList"/> of control items
		/// </summary>
		/// <remarks>
		/// The controls in the <c>Items</c> property are in appearance order
		/// <para>
		/// You should NOT directly modify the contents of this member
		/// </para>
		/// </remarks>
		public ArrayList Items {
			get {
				return items ;
			}
		}


		/// <summary>
		/// Get/Set the type of background drawing
		/// </summary>
		/// <remarks>
		/// <see cref="BackgroundStyle"/> for more information.
		/// 
		/// Note: This is probably NOT one of my better ideas...
		/// </remarks>
		[Category("Appearance"), Description("Background Style for PSItmLayoutPanel")]
		[DefaultValue("BackgroundStyle.Transparent")]
		public BackgroundStyle BackgroundStyle {
			get {
				return backgroundStyle ;
			}

			set {
				if (backgroundStyle != value) {
					backgroundStyle = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.BackgroundStyleProperty) ;
				}
			}
		}

		/// <summary>
		/// <see cref="GradientColor"/> used to draw the background of the <c>PSItmLayoutPanel</c>
		/// </summary>
		/// <remarks>
		/// <para>Fires a PropertyChange event w/ <see cref="PSItmLayoutPanelProperty.PanelGradientProperty"/> argument</para>
		/// </remarks>
		[Category("Appearance"),
		Description("Gradient color for the background of the PSItmLayoutPanel")]
		public GradientColor PanelGradient {
			get {
				return panelGradient ;
			}

			set {
				if (!panelGradient.Equals(value)) {
					if (value == null) {
						panelGradient = new GradientColor(SystemColors.Control) ;
					}

					panelGradient = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.PanelGradientProperty) ;
				}
			}
		}

		/// <summary>
		/// Determine if this property should be serialized
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the proeprty does not equal the default value
		/// </returns>
		protected bool ShouldSerializePanelGradient() {
			return panelGradient != DefaultPanelGradient ;
		}

		/// <summary>
		/// Reset the property to its default value
		/// </summary>
		/// <remarks>
		/// Called by the IDE designer
		/// </remarks>
		protected void ResetPanelGradient() {
			PanelGradient = DefaultPanelGradient ;
		}

		/// <summary>
		/// Sets the left/right/top margins for items/controls within the <c>PSItmLayoutPanel</c>
		/// </summary>
		/// <remarks>
		/// <para>Fires a PropertyChange event w/ <see cref="PSItmLayoutPanelProperty.BorderMarginProperty"/> argument</para>
		/// </remarks>
		[Category("Layout"),
		Description("X/Y margins for items in the PSItmLayoutPanel")]
		public Size BorderMargin {
			get {
				return borderMargin ;
			}
		
			set {
				if (!borderMargin.Equals(value)) {
					borderMargin = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.BorderMarginProperty) ;
				}
			}
		}

		/// <summary>
		/// Determine if this property should be serialized
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the proeprty does not equal the default value
		/// </returns>
		protected bool ShouldSerializeBorderMargin() {
			return borderMargin != DefaultBorderMargin ;
		}

		/// <summary>
		/// Reset the property to its default value
		/// </summary>
		/// <remarks>
		/// Called by the IDE designer
		/// </remarks>
		protected void ResetBorderMargin() {
			BorderMargin = DefaultBorderMargin ;
		}

		/// <summary>
		/// Y spacing between items/controls controls within the <c>PSItmLayoutPanel</c>
		/// </summary>
		/// <remarks>
		/// Default value for this property is <see cref="DefaultSpacing"/>
		/// <para>Fires a PropertyChange event w/ <see cref="PSItmLayoutPanelProperty.ItemSpacingProperty"/> argument</para>
		/// </remarks>
		[Category("Layout"),
		Description("Y spacing between Items in the PSItmLayoutPanel"),
		DefaultValue(DefaultSpacing)]
		public int ItemSpacing {
			get {
				return itemSpacing ;
			}

			set {
				if (itemSpacing != value) {
					itemSpacing = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.ItemSpacingProperty) ;
				}
			}
		}

		/// <summary>
		/// Get/Set the ItemLayout implementation (or <see langword="null"/> for the default)
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IItemLayout ItemLayout {
			get {
				return itemLayoutHandler ;
			}

			set {
				if (itemLayoutHandler != value) {
					itemLayoutHandler = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.ItemLayoutProperty) ;
				}
			}
		}

		/// <summary>
		/// <see langword="true"/> if the <c>PSItmLayoutPanel</c> should resize its parent
		/// <see cref="PSPanel"/> based on its size
		/// </summary>
		[Category("Layout"), Description("True if the panel resizes based on content")]
		[DefaultValue(false)]
		public bool AutoSize {
			get {
				return autoSize ;
			}

			set {
				if (autoSize != value) {
					autoSize = value ;
					OnPropertyChange(PSItmLayoutPanelProperty.AutoSizeProperty) ;
				}
			}
		}
		#endregion Properties

		#region Methods
		/// <summary>
		/// Reorder a control item relative to its peers
		/// </summary>
		/// <param name="currIndex">The current index of the control item</param>
		/// <param name="newIndex">The new index of the control item</param>
		public void MoveItem(int currIndex,int newIndex) {
			if ((currIndex < 0) || (currIndex >= items.Count) || (newIndex < 0)) {
				throw new ArgumentException("PSItmLayoutPanel.MoveItem: Invalid item indices") ;
			}

			if (currIndex == newIndex) 
				return ;

			Object item = items[currIndex] ;
			items.RemoveAt(currIndex) ;

			if (newIndex < items.Count) {
				items.Insert(newIndex,item) ;
			} else {
				items.Add(item) ;
			}

			PerformLayout() ;
		}

		/// <summary>
		/// Reorder a control item relative to its peers
		/// </summary>
		/// <param name="newIndex">The new index of the control item</param>
		/// <param name="control">The control item to move</param>
		public void MoveItem(int newIndex,Control control) {
			if (newIndex < 0) {
				throw new ArgumentException("PSItmLayoutPanel.MoveItem: Invalid item index") ;			
			}

			int indexOf = items.IndexOf(control) ;

			if (indexOf == -1) {
				throw new ArgumentException("PSItmLayoutPanel.MoveItem: Unknown control") ;			
			}

			if (indexOf != newIndex) {
				items.RemoveAt(indexOf) ;
				if (newIndex < items.Count) {
					items.Insert(newIndex,control) ;
				} else {
					items.Add(control) ;
				}

				PerformLayout() ;
			}
		}

		/// <summary>
		/// Show/Hide all controls in the <c>PSItmLayoutPanel</c>
		/// </summary>
		/// <param name="showItems"></param>
		public void ShowAll(bool showItems) {
			foreach(Control control in Controls) {
				control.Visible = showItems ;
			}
		}

		/// <summary>
		/// Show all the controls in the <c>PSItmLayoutPanel</c>
		/// </summary>
		public void ShowAll() {
			ShowAll(true) ;
		}

		/// <summary>
		/// Hide all the controls in the <c>PSItmLayoutPanel</c>
		/// </summary>
		public void HideAll() {
			ShowAll(false) ;
		}
		#endregion Methods

		#region Events
		/// <summary>
		/// Register/Unregister for property change notifications
		/// </summary>
		public event PropertyChangeHandler PropertyChange {
			add {
				propertyChangeListeners += value ;
			}

			remove {
				propertyChangeListeners -= value ;
			}
		}
		#endregion Events

		#region Implementation
		/// <summary>
		/// Property change handler
		/// </summary>
		/// <param name="property"></param>
		protected virtual void OnPropertyChange(PSItmLayoutPanelProperty property) {
			switch(property) {
				case PSItmLayoutPanelProperty.BackColorProperty:
				case PSItmLayoutPanelProperty.PanelGradientProperty:
				case PSItmLayoutPanelProperty.BackgroundStyleProperty:
					BackgroundBrush = null ;
					break ;

				case PSItmLayoutPanelProperty.BorderMarginProperty:
				case PSItmLayoutPanelProperty.ItemSpacingProperty:
				case PSItmLayoutPanelProperty.AutoSizeProperty:
				case PSItmLayoutPanelProperty.ItemLayoutProperty:
					break ;
			}

			if (propertyChangeListeners != null) {
				propertyChangeListeners(this,new PropertyChangeEventArgs(property)) ;
			}
		}
		#endregion Implementation

		#region Cached Drawing Properties
		/// <summary>
		/// Get/Set the background brush
		/// </summary>
		protected LinearGradientBrush BackgroundBrush {
			get {
				if (backgroundBrush == null) {
					if (BackgroundStyle == BackgroundStyle.Gradient) {
						backgroundBrush = new LinearGradientBrush(
							new Rectangle(0,0,Width,Height),
							PanelGradient.Start,
							PanelGradient.End,
							LinearGradientMode.Horizontal
							) ;
					} else {
						backgroundBrush = new LinearGradientBrush(
							new Rectangle(0,0,Width,Height),
							BackColor,
							BackColor,
							LinearGradientMode.Horizontal
							) ;
					}
				}

				return backgroundBrush ;
			}

			set {
				if (backgroundBrush != value) {
					if (backgroundBrush != null) {
						backgroundBrush.Dispose() ;
					}

					backgroundBrush = value ;
				}
			}
		}
		#endregion Cached Drawing Properties

		#region Layout Code
		/// <summary>
		/// Set the height of the <c>PSItmLayoutPanel</c> and if applicable, the height 
		/// of parent <see cref="PSPanel"/>'s panel area to match our height (with 
		/// a little margin)
		/// </summary>
		/// <param name="bestHeight">The best height as determined by the layout engine</param>
		public virtual void SetBestHeight(int bestHeight) {
			this.Height = bestHeight ;
		
			if (!DesignMode) {
				PSPanel panel = Parent as PSPanel ;
				if (panel != null) {
					panel.PanelHeight = bestHeight + 12 ;
				}
			}
		}

		/// <summary>
		/// Updates the layout of control items via an implementation of <see cref="IItemLayout"/>
		/// </summary>
		protected virtual void UpdateItems() {
			int bestHeight ;

			// custom?
			if (ItemLayout != null) {
				bestHeight = ItemLayout.LayoutItems(this,items) ;
			} else {
				// nope, use the default
				bestHeight = defaultItemLayout.LayoutItems(this,items) ;
			}

			// IFF we are auto-sizing then adjust our height, and possibly our parents height
			if (AutoSize) {
				SetBestHeight(bestHeight) ;
			}
		}
		#endregion Layout Code

		#region Overrides
		/// <summary>
		/// Handle this change since we may be drawing with the <see cref="Control.BackColor"/>
		/// depending on the value of <see cref="BackgroundStyle"/>
		/// </summary>
		/// <param name="e"></param>
		protected override void OnBackColorChanged(EventArgs e) {
			base.OnBackColorChanged (e);
			OnPropertyChange(PSItmLayoutPanelProperty.BackColorProperty) ;			
		}

		/// <summary>
		/// Paint the background of the <c>PSItmLayoutPanel</c> using the appropriate color(s) for the panel
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			if (BackgroundStyle != BackgroundStyle.Transparent) {
				pevent.Graphics.FillRectangle(BackgroundBrush,pevent.ClipRectangle) ;
			} else {
				base.OnPaintBackground(pevent) ;
			}
		}

		/// <summary>
		/// Update our parent and our own item layout on visibility changes
		/// </summary>
		/// <param name="e"></param>
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged (e) ;

			if (Parent is PSPanel) {
				Parent.Invalidate() ;
			}

			UpdateItems() ;
		}

		/// <summary>
		/// Reevaluate our layout when asked
		/// </summary>
		/// <param name="levent">ignored</param>
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent) ;

			UpdateItems() ;
		}

		/// <summary>
		/// Overridden to handle the addition of items
		/// </summary>
		/// <param name="e">ControlAdded event args</param>
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded (e);

			// during InitializeComponent panels are provided in the inverse order
			if (isInitializingComponent) {
				// insert as the 1st item
				items.Insert(0,e.Control) ;
			} else {
				// add to the end
				items.Add(e.Control) ;
			}

			// force item layout to be updated
			PerformLayout() ;

			// Monitor all controls for visibility changes
			e.Control.VisibleChanged += new EventHandler(Control_VisibleChanged);
		}

		/// <summary>
		/// Overridden to provide special handling for items
		/// </summary>
		/// <param name="e">ControlRemoved event args</param>
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved (e);

			items.Remove(e.Control) ;

			e.Control.VisibleChanged -= new EventHandler(Control_VisibleChanged);

			// force item layout to be updated
			PerformLayout() ;
		}

		/// <summary>
		/// Our size changed, relocate all panels
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged (e);
			// force item layout to be updated
			PerformLayout() ;
		}
		#endregion Overrides 

		#region ISupportInitialize Members
		/// <summary>
		/// Set flag noting that we are in InitializeComponent()
		/// </summary>
		public void BeginInit() {
			isInitializingComponent = true ;
		}

		/// <summary>
		/// Clear flag noting that we are in InitializeComponent()
		/// </summary>
		public void EndInit() {
			isInitializingComponent = false ;
		}
		#endregion ISupportInitialize Members

		#region Event Handlers
		/// <summary>
		/// When a control hides or shows we reset the scroll position and
		/// update the panels
		/// </summary>
		/// <param name="sender">The control being hidden/shown</param>
		/// <param name="e">The event arguments</param>
		private void Control_VisibleChanged(object sender, EventArgs e) {
			AutoScrollPosition = new Point(0,0) ;
			// force item layout to be updated
			PerformLayout() ;
		}
		#endregion Event Handlers
	}
}
