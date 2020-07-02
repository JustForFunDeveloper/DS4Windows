using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DS4WinWPF.DraculaView.Converter
{
    public class ScrollViewMarginConverter : MarkupExtension, IValueConverter
    {
        private static ScrollViewMarginConverter _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isWidth = System.Convert.ToBoolean(parameter);
            var convertedValue = System.Convert.ToDouble(value);
            if (isWidth)
            {
                return new Thickness(-50, 0, -50, -convertedValue);
            }
            else
            {
                return new Thickness(-50, -convertedValue, -50, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ScrollViewMarginConverter());
        }
    }
}
