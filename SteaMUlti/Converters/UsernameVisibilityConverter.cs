using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	class UsernameVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DBNull.Value)
				return Visibility.Visible;
			else
			{
				string username = (string)value;

				if (string.IsNullOrWhiteSpace(username))
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string ret = "";
			return ret;
		}
	}
}
