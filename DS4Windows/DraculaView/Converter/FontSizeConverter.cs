using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DS4WinWPF.DraculaView.Converter
{
    public class FontSizeConverter : MarkupExtension, IValueConverter
    {
        private static FontSizeConverter _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var width = System.Convert.ToDouble(value);
            var resizeValue = (width * (System.Convert.ToDouble(parameter)) - width) / 2;

            return -resizeValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new FontSizeConverter());
        }
    }
}
