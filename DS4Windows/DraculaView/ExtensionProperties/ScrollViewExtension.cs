using System.Windows;

namespace DS4WinWPF.DraculaView.ExtensionProperties
{
    public class ScrollViewExtension
    {
        public static readonly DependencyProperty DynamicButtonMarginProperty =
            DependencyProperty.RegisterAttached("DynamicButtonMargin", typeof(Thickness), typeof(ScrollViewExtension), new PropertyMetadata(default(Thickness)));

        public static void SetDynamicButtonMargin(UIElement element, Thickness value)
        {
            element.SetValue(DynamicButtonMarginProperty, value);
        }

        public static Thickness GetDynamicButtonMargin(UIElement element)
        {
            return (Thickness)element.GetValue(DynamicButtonMarginProperty);
        }
    }
}
