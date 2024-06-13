using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SilverlightContrib.Controls
{
    /// <summary>
    /// Represents a Group Box control which allows content to be grouped under a common header.
    /// </summary>
    public class GroupBox : ContentControl
    {
        // GroupBox control courtesy Tim Greenfield.
        // http://programmerpayback.com/2008/11/26/silverlight-groupbox-control/

        private RectangleGeometry FullRect;
        private RectangleGeometry HeaderRect;
        private ContentControl HeaderContainer;

        private bool _loaded, _applyTemplateDone;

        /// <summary>
        /// Creates a new instance of the GroupBox control.
        /// </summary>
        public GroupBox()
        {
            DefaultStyleKey = typeof(GroupBox);
            this.SizeChanged += GroupBox_SizeChanged;
            Loaded += (s, a) => {
                _loaded = true;
                PostponeUpdateInitialBackground();
            };
        }

        private void PostponeUpdateInitialBackground() {
            if (_loaded && _applyTemplateDone)
                Dispatcher.BeginInvoke(UpdateInitialBackground);
        }

        private static bool IsTransparentBrush(Brush brush) {
            if (brush != null) {
                if (brush is SolidColorBrush solid) {
                    if (solid.Color != Colors.Transparent)
                        return false;
                } else 
                    // background of a different Brush type, assume not transparent
                    return false;
            }

            return true;
        }

        private void UpdateInitialBackground() {
            if (!IsTransparentBrush(Background))
                return;

            var parent = VisualTreeHelper.GetParent(this);
            Brush parentBrush = null;
            while (parent != null && parentBrush == null) {
                if (parent is Control control) {
                    if (!IsTransparentBrush(control.Background))
                        parentBrush = control.Background;
                } else if (parent is Panel panel) {
                    if (!IsTransparentBrush(panel.Background))
                        parentBrush = panel.Background;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parentBrush != null)
                Background = parentBrush;
        }

        /// <summary>
        /// Builds the visual tree for the GroupBox control when the template is applied. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FullRect = (RectangleGeometry)GetTemplateChild("FullRect");
            HeaderRect = (RectangleGeometry)GetTemplateChild("HeaderRect");
            HeaderContainer = (ContentControl)GetTemplateChild("HeaderContainer");
            HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;

            _applyTemplateDone = true;
            PostponeUpdateInitialBackground();
        }

        /// <summary>
        /// Header Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = 
            DependencyProperty.Register("Header", typeof(object), typeof(GroupBox), null);

        /// <summary>
        /// The Header for the group box.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBox), null);

        /// <summary>
        /// The Header Data Template.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        private void GroupBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FullRect.Rect = new Rect(new Point(), e.NewSize);
        }

        private void HeaderContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HeaderRect.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
        } 
    }
}
