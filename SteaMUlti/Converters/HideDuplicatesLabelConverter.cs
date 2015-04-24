using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(bool), typeof(string))]
	class HideDuplicatesLabelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return "SHOW DUPLICATES";
			else
				return "HIDE DUPLICATES";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string ret = "";
			return ret;
		}
	}
}
