using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D ;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices ;

namespace PSUI {
	#region PSPanelGroup enumerations
	/// <summary>
	/// Enumeration of <see cref="PSPanelGroup"/> properties
	/// </summary>
	public enum PSPanelGroupProperties {
		/// <summary>
		/// <see cref="PSPanelGroup.BorderMargin"/>
		/// </summary>
		BorderMarginProperty,

		/// <summary>
		/// <see cref="PSPanelGroup.PanelSpacing"/>
		/// </summary>
		PanelSpacingProperty,

		/// <summary>
		/// <see cref="PSPanelGroup.PanelGradient"/>
		/// </summary>
		PanelGradientProperty
	}
	#endregion PSPanelGroup enumerations

	/// <summary>
	/// PSPanelGroup provides a container for <see cref="PSPanel"/> controls
	/// </summary>
	/// <remarks>
	/// <para>
	/// <c>PSPanelGroup</c> has the following primary properties:
	///	<list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Purpose</description>
	///		</listheader>
	///		<item>
	///			<term><see cref="BorderMargin"/></term>
	///			<description>Controls the Left/Right/Top margins for contained <see cref="PSPanel"/> controls</description>
	///		</item>
	///		<item>
	///			<term><see cref="PanelSpacing"/></term>
	///			<description>Controls the Y spacing between <see cref="PSPanel"/> controls</description>
	///		</item>
	///		<item>
	///			<term><see cref="PanelGradient"/></term>
	///			<description>Defines the <see cref="GradientColor"/> for the background of the <c>PSPanelGroup</c></description>
	///		</item>
	///	</list>
	///	</para>
	///	<para>
	///	<c>PSPanelGroup</c> has the following primary events:
	///	<list type="table">
	///		<listheader>
	///			<term>Event</term>
	///			<description>Purpose</description>
	///		</listheader>
	///		<item>
	///			<term><see cref="PropertyChange"/></term>
	///			<description>Triggered when a primary property changes. See <see cref="PSPanelGroupProperties"/></description>
	///		</item>
	///	</list>
	///	</para>
	///	<para>
	///	<c>PSPanelGroup</c> has the following primary methods:
	///	<list type="table">
	///		<listheader>
	///			<term>Method</term>
	///			<description>Purpose</description>
	///		</listheader>
	///		<item>
	///			<term><see cref="MovePanel"/></term>
	///			<description>Change the order/position of a panel within the <c>PSPanelGroup</c></description>
	///		</item>
	///		<item>
	///			<term><see cref="EnsureVisible"/></term>
	///			<description>Attempt to make an <see cref="PSPanel"/> fully visible
	///			within the <c>PSPanelGroup</c> </description>
	///		</item>
	///	</list>
	///	</para>
	///	</remarks>
	public class PSPanelGroup : System.Windows.Forms.Panel, System.ComponentModel.ISupportInitialize {
		#region class PropertyChangeEventArgs
		/// <summary>
		/// <see cref="PSPanelGroup.PropertyChange"/> event arguments
		/// </summary>
		public class PropertyChangeEventArgs : System.EventArgs {
			/// <summary>
			/// The enumeration for the property that changed
			/// </summary>
			private readonly PSPanelGroupProperties property ;

			/// <summary>
			/// Create a <c>PropertyChangeEventArgs</c>
			/// </summary>
			/// <param name="property">The enumeration for the property that changed</param>
			public PropertyChangeEventArgs(PSPanelGroupProperties property) {
				this.property = property ;
			}

			/// <summary>
			/// Get the enumeration for the property that changed
			/// </summary>
			public PSPanelGroupProperties Property {
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
		public static readonly GradientColor DefaultPanelGradient = new GradientColor(Color.White) ;

		/// <summary>
		/// Default value for <see cref="BorderMargin"/>
		/// </summary>
		public static readonly Size DefaultBorderMargin = new Size(8,8) ;

		/// <summary>
		/// Default value for <see cref="PanelSpacing"/>
		/// </summary>
		public const int DefaultSpacing = 8 ;
		#endregion Constants

		#region Fields
		/// <summary>
		/// Controls left/top/right spacing of <see cref="PSPanel"/> controls within the <c>PSPanelGroup</c>
		/// </summary>
		private Size borderMargin = new Size(DefaultBorderMargin.Width,DefaultBorderMargin.Height) ;

		/// <summary>
		/// Controls the Y spacing between <see cref="PSPanel"/> controls within the <c>PSPanelGroup</c>
		/// </summary>
		private int panelSpacing = DefaultSpacing ;

		/// <summary>
		/// <see langword="true"/> when we are in InitializeComponent()
		/// </summary>
		/// <remarks>
		/// <see cref="ISupportInitialize"/>
		/// </remarks>
		private bool isInitializingComponent = false ;

		/// <summary>
		/// The <see cref="GradientColor"/> for the background of the <c>PSPanelGroup</c>
		/// </summary>
		private GradientColor panelGradient = new GradientColor(DefaultPanelGradient) ;

		/// <summary>
		/// Collection to hold the <see cref="PSPanel"/> controls in the <c>PSPanelGroup</c>
		/// </summary>
		private ArrayList panels = new ArrayList() ;

			#region Events (and related)
		/// <summary>
		/// Event handler for <see cref="PSPanel.PanelStateChange"/>
		/// </summary>
		private EventHandler psPanelEventHandler ;

		/// <summary>
		/// PropertyChange event listeners
		/// </summary>
		private EventHandler propertyChangeListeners = null ;
			#endregion Events (and related)
		#endregion Fields
		
		#region Constructor(s)
		/// <summary>
		/// Construct an <c>PSPanelGroup</c>
		/// </summary>
		public PSPanelGroup() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// force this on
			AutoScroll = true ;
			BackColor = Color.Transparent ;

			// Use these control styles for smoother drawing and transparency support
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ContainerControl, true);

			// single instance of PSPanel.PanelStateChange event handler
			psPanelEventHandler =  new EventHandler(PSPanelGroup_PanelStateChange) ;
		}
		#endregion Constructor(s)

		#region Dispose (and related)
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			base.Dispose( disposing );
		}
		#endregion Dispose (and related)

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {}
		#endregion

		#region Properties
		/// <summary>
		/// Sets the left/right/top margins for <see cref="PSPanel"/> controls within the <c>PSPanelGroup</c>
		/// </summary>
		/// <remarks>
		/// <para>Fires a PropertyChange event w/ <see cref="PSPanelGroupProperties.BorderMarginProperty"/> argument</para>
		/// </remarks>
		[Category("Appearance"),
		Description("X/Y margins for PSPanels in the PSPanelGroup")]
		public Size BorderMargin {
			get {
				return borderMargin ;
			}
		
			set {
				if (!borderMargin.Equals(value)) {
					borderMargin = value ;
					OnPropertyChange(PSPanelGroupProperties.BorderMarginProperty) ;
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
		/// Y spacing between <see cref="PSPanel"/> controls within the <c>PSPanelGroup</c>
		/// </summary>
		/// <remarks>
		/// Default value for this property is <see cref="DefaultSpacing"/>
		/// <para>Fires a PropertyChange event w/ <see cref="PSPanelGroupProperties.PanelSpacingProperty"/> argument</para>
		/// </remarks>
		[Category("Appearance"),
		Description("Y spacing between PSPanels in the PSPanelGroup"),
		DefaultValue(DefaultSpacing)]
		public int PanelSpacing {
			get {
				return panelSpacing ;
			}

			set {
				if (panelSpacing != value) {
					panelSpacing = value ;
					OnPropertyChange(PSPanelGroupProperties.PanelSpacingProperty) ;
				}
			}
		}

		/// <summary>
		/// <see cref="GradientColor"/> used to draw the background of the <c>PSPanelGroup</c>
		/// </summary>
		/// <remarks>
		/// <para>Fires a PropertyChange event w/ <see cref="PSPanelGroupProperties.PanelGradientProperty"/> argument</para>
		/// </remarks>
		[Category("Appearance"),
		Description("Gradient color for the background of the PSPanelGroup")]
		public GradientColor PanelGradient {
			get {
				return panelGradient ;
			}

			set {
				if (!panelGradient.Equals(value)) {
					panelGradient = value ;
					OnPropertyChange(PSPanelGroupProperties.PanelGradientProperty) ;
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
		#endregion Properties

		#region Methods
		/// <summary>
		/// Change the order/position of an <see cref="PSPanel"/> 
		/// </summary>
		/// <param name="currIndex">The current index/position</param>
		/// <param name="newIndex">The new index/position</param>
		public void MovePanel(int currIndex,int newIndex) {
			if ((currIndex >= 0) && (currIndex < panels.Count) && (currIndex != newIndex)) {
				Object item = panels[currIndex] ;
				panels.RemoveAt(currIndex) ;
				panels.Insert(newIndex,item) ;
				UpdatePanels() ;
			}
		}

		/// <summary>
		/// Change the order/position of the specified <see cref="PSPanel"/> 
		/// </summary>
		/// <param name="newIndex">The new index/position</param>
		/// <param name="psPanel">The <see cref="PSPanel"/> to move</param>
		public void MovePanel(int newIndex,PSPanel psPanel) {
			int indexOf = panels.IndexOf(psPanel) ;

			if ((indexOf != -1) && (indexOf != newIndex)) {
				panels.RemoveAt(indexOf) ;
				panels.Insert(newIndex,psPanel) ;
				UpdatePanels() ;
			}
		}

		/// <summary>
		/// Ensure that the specified panel is fully visible (if possible) within 
		/// the <c>PSPanelGroup</c>
		/// </summary>
		/// <param name="psPanel">The <see cref="PSPanel"/> to make visible</param>
		/// <remarks>
		/// If the panel is not visible it is made visible. If the bottom of the panel
		/// not visible, the group is scrolled to make it visible, otherwise if the
		/// top of the panel is not visible the scroll is adjusted to make it visible
		/// </remarks>
		/// <exception cref="ArgumentException">If the specified <see cref="PSPanel"/> is not
		/// a member of the <c>PSPanelGroup</c></exception>
		public void EnsureVisible(PSPanel psPanel) {
			if (!panels.Contains(psPanel)) {
				throw new ArgumentException("The specified PSPanel is not a member of this PSPanelGroup","PSPanel") ;
			}

			if (!psPanel.Visible)
				psPanel.Visible = true ;

			if (psPanel.Bottom > (Height + AutoScrollPosition.Y)) {
				AutoScrollPosition = new Point(AutoScrollPosition.X,psPanel.Bottom - Height) ;
			} else if (psPanel.Top < 0) {
				AutoScrollPosition = new Point(AutoScrollPosition.X,Math.Max(0,psPanel.Top-1)) ;
			}
		}
		#endregion Methods

		#region Events
		/// <summary>
		/// Add/Remove a PropertyChange listener
		/// </summary>
		public event EventHandler PropertyChange {
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
		/// React to property changes and invoke PropertyChange event to listeners
		/// </summary>
		/// <param name="property">The property that changed</param>
		protected virtual void OnPropertyChange(PSPanelGroupProperties property) {
			switch(property) {
				case PSPanelGroupProperties.BorderMarginProperty:
				case PSPanelGroupProperties.PanelSpacingProperty:
					// force the position of panels to be reevaluated
					UpdatePanels() ;
					break ;

				case PSPanelGroupProperties.PanelGradientProperty:
					break ;
			}

			if (propertyChangeListeners != null) {
				propertyChangeListeners(this,new PropertyChangeEventArgs(property)) ;
			}

			Invalidate() ;
		}

		/// <summary>
		/// Update an individual <see cref="PSPanel"/>
		/// </summary>
		/// <param name="panel">The panel to be updated</param>
		/// <param name="lastBottom">Bottom position of last <see cref="PSPanel"/> or 0 if this is the 1st panel
		/// in the <c>PSPanelGroup</c></param>
		private void UpdatePanel(PSPanel panel, int lastBottom) {
			if (panel.Left != BorderMargin.Width) 
				panel.Left = BorderMargin.Width ;

			if (lastBottom != 0) {
				if (panel.Top != lastBottom + PanelSpacing) 
					panel.Top = lastBottom + PanelSpacing ;
			} else {
				if (panel.Top != lastBottom + BorderMargin.Height)
					panel.Top = lastBottom + BorderMargin.Height ;
			}

			if (panel.Width != this.Width - (BorderMargin.Width << 1) - (VScroll ? SystemInformation.VerticalScrollBarWidth : 0))
				panel.Width = this.Width - (BorderMargin.Width << 1) - (VScroll ? SystemInformation.VerticalScrollBarWidth : 0) ;
		}

		/// <summary>
		/// Flag to avoid reentrancy (just a waste of cpu cycles)
		/// </summary>
		private bool updatingPanels = false ;

		/// <summary>
		/// Update the location and width of all the <see cref="PSPanel"/> controls
		/// in the <see cref="PSPanelGroup"/>
		/// </summary>
		private void UpdatePanels() {
			int lastBottom = 0 ;

			if (isInitializingComponent || updatingPanels)
				return ;

			updatingPanels = true ;

			for(int i=0; i < panels.Count ; i++) {
				PSPanel panel = (PSPanel) panels[i] ;

				if (!panel.Visible)
					continue ;

				UpdatePanel(panel,lastBottom) ;
				lastBottom = panel.Top + panel.Height ;
			}

			updatingPanels = false ;
		}

		/// <summary>
		/// Update the location of all the <see cref="PSPanel"/> controls
		/// in the <see cref="PSPanelGroup"/> after a particular index
		/// </summary>
		/// <remarks>
		/// Used when a panel is collapsed or expanded to change all the
		/// subsequent panels
		/// </remarks>
		private void UpdatePanelsAfter(PSPanel panel) {
			// @@BUGFIX: 1.1
			if (!panel.Visible) {
				UpdatePanels() ;
				return ;
			}

			if (isInitializingComponent || updatingPanels)
				return ;

			updatingPanels = true ;

			// the bottom of the specified panel plus the BorderMargin.Height (which we always need
			// to take into account)
			int lastBottom = panel.Top + panel.Height ;

			// map the panel to its index in our panels collection
			int panelIndex = panels.IndexOf(panel) + 1 ;

			// for each following panel, relocate it
			for(int i=panelIndex ; i < panels.Count ; i++) {
				PSPanel nextPanel = (PSPanel) panels[i] ;

				// @@BUGFIX: 1.1
				if (!nextPanel.Visible)
					continue ;

				UpdatePanel(nextPanel,lastBottom) ;
				lastBottom = nextPanel.Top + nextPanel.Height ;
			}

			updatingPanels = false ;
		}
		#endregion Implementation

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

		#region Overrides
		/// <summary>
		/// Provide a version of <see cref="ScrollableControl.AutoScroll"/> that hides the base class
		/// version
		/// </summary>
		[Browsable(false)]
		public override bool AutoScroll {
			get {
				return base.AutoScroll ;
			}

			set {
				if (value == true) {
					base.AutoScroll = value ;
				}
			}
		}

		/// <summary>
		/// Provide a version of <see cref="Control.BackColor"/>that hides the base class
		/// version
		/// </summary>
		[Browsable(false)]
		public override Color BackColor {
			get {
				return base.BackColor ;
			}

			set {
				if (value == Color.Transparent) {
					base.BackColor = value ;
				}
			}
		}

		/// <summary>
		/// Overridden to handle the addition of <see cref="PSPanel"/> items
		/// specially
		/// </summary>
		/// <param name="e">ControlAdded event args</param>
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded (e);

			if (e.Control is PSPanel) {
				e.Control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;

				// during InitializeComponent panels are provided in the inverse order
				if (isInitializingComponent) {
					// insert as the 1st panel
					panels.Insert(0,(PSPanel) e.Control) ;
				} else {
					// add to the end
					panels.Add((PSPanel) e.Control) ;

					// force all panels to be updated
					UpdatePanels() ;
				}

				// listen for PanelStateChange events so we can make adjustments
				((PSPanel) e.Control).PanelStateChange += psPanelEventHandler ;
				((PSPanel) e.Control).Expanded += new EventHandler(PSPanelGroup_Expanded);
				((PSPanel) e.Control).PropertyChange += new PanelPropertyChangeHandler(PSPanelGroup_PropertyChange);
			}

			e.Control.VisibleChanged += new EventHandler(Control_VisibleChanged);
		}

		/// <summary>
		/// Overridden to provide special handling for <see cref="PSPanel"/> controls
		/// </summary>
		/// <param name="e">ControlRemoved event args</param>
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved (e);

			if (e.Control is PSPanel) {
				// dont track the panel anymore
				panels.Remove((PSPanel) e.Control) ;
				// dont listen to events
				((PSPanel) e.Control).PanelStateChange -= psPanelEventHandler ;
				((PSPanel) e.Control).Expanded -= new EventHandler(PSPanelGroup_Expanded);
				((PSPanel) e.Control).PropertyChange -= new PanelPropertyChangeHandler(PSPanelGroup_PropertyChange);
				// reposition everything
				UpdatePanels() ;
			}

			e.Control.VisibleChanged -= new EventHandler(Control_VisibleChanged);
		}

		/// <summary>
		/// Paint the background of the <c>PSPanelGroup</c> using the gradient color for the panel
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			base.OnPaintBackground(pevent) ;

			// add support for transparent background by just not drawing anything
			try
			{
				if (!panelGradient.IsTransparent) 
				{
					using(LinearGradientBrush b = new LinearGradientBrush(this.ClientRectangle,panelGradient.Start,panelGradient.End,LinearGradientMode.Vertical)) 
					{
						pevent.Graphics.FillRectangle(b,pevent.ClipRectangle) ;
					}
				}
			}
			catch{}
		}

		/// <summary>
		/// Our size changed, relocate all panels
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged (e);
			UpdatePanels() ;
		}
		#endregion Overrides

		#region Event Handlers
		/// <summary>
		/// <see cref="PSPanel.PanelStateChange"/> event handler
		/// </summary>
		/// <param name="sender">The <see cref="PSPanel"/> that changed</param>
		/// <param name="e"><see cref="System.EventArgs.Empty"/></param>
		private void PSPanelGroup_PanelStateChange(object sender, EventArgs e) {
			PSPanel panel = sender as PSPanel ;

			// sometimes this needs to be forced
			if (panel.Width != this.Width - (BorderMargin.Width << 1) - (VScroll ? SystemInformation.VerticalScrollBarWidth : 0))
				panel.Width = this.Width - (BorderMargin.Width << 1) - (VScroll ? SystemInformation.VerticalScrollBarWidth : 0) ;

			UpdatePanelsAfter((PSPanel) sender) ;
		}

		/// <summary>
		/// Handle panel/control visibility changes
		/// </summary>
		/// <param name="sender">The control whose visibility changed</param>
		/// <param name="e"><see cref="System.EventArgs.Empty"/></param>
		private void Control_VisibleChanged(object sender, EventArgs e) {
			AutoScrollPosition = new Point(0,0) ;
			UpdatePanels() ;
		}

		/// <summary>
		///	Handle the <see cref="PSPanel.Expanded"/> event so that the entire panel can
		///	be made visible
		/// </summary>
		/// <param name="sender">The <see cref="PSPanel"/> that is expanded</param>
		/// <param name="e"><see cref="System.EventArgs.Empty"/></param>
		/// <remarks>
		/// Generally this event is triggered when the caption of the <see cref="PSPanel"/>
		/// is clicked, or a programattic action triggers expansion
		/// </remarks>
		private void PSPanelGroup_Expanded(object sender, EventArgs e) {
			EnsureVisible((PSPanel) sender) ;
		}

		/// <summary>
		/// Handle the <see cref="PSPanel.PropertyChange"/> event 
		/// </summary>
		/// <param name="psPanel">The <see cref="PSPanel"/> whose property changed</param>
		/// <param name="e">instance of <see cref="PSPanelPropertyChangeEventArgs"/> describing the property change</param>
		/// <remarks>
		/// Currently we handle the <see cref="PSPanelProperties.PanelHeightProperty"/> so that we can
		/// ensure that the entire panel is visible. The <see cref="PSPanelProperties.PanelHeightProperty"/>
		/// is only changed when a <see cref="PSPanel"/> resizes due to a change in the size of child 
		/// controls. Typically this indicates some interest/focus on the users part, but you may want to
		/// supress this event if these changes are 'randomesque' in your application
		/// </remarks>
		private void PSPanelGroup_PropertyChange(PSPanel psPanel, PSPanelPropertyChangeEventArgs e) {
			if (e.PSPanelProperty == PSPanelProperties.PanelHeightProperty) {
				EnsureVisible(psPanel) ;
			}
		}
		#endregion Event Handlers
	}
}
