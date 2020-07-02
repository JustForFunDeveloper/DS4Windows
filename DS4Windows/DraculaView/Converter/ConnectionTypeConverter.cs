using System;
using System.Globalization;
using System.Windows.Data;
using DS4Windows;
using MahApps.Metro.IconPacks;

namespace DS4WinWPF.DraculaView.Converter
{
    public class ConnectionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connectionType = (value as ConnectionType?);

            if (connectionType != null)
            {
                if (connectionType.Equals(ConnectionType.BT))
                {
                    return PackIconMaterialKind.Bluetooth;
                }
                else if (connectionType.Equals(ConnectionType.USB))
                {
                    return PackIconMaterialKind.Usb;
                }
                else
                {
                    return PackIconMaterialKind.Help;
                }
            }
            else
            {
                return PackIconMaterialKind.Help;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
