using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(string), typeof(string))]
	class UsernameTextBlockConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DBNull.Value)
				return "Please set a username";
			else
			{
				string username = (string)value;

				if (string.IsNullOrWhiteSpace(username))
					return "Please set a username";
				else
					return "Username";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string ret = "";
			return ret;
		}
	}
}
