using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(byte[]), typeof(Visibility))]
	class PasswordVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DBNull.Value)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			byte[] ret = null;
			return ret;
		}
	}
}
