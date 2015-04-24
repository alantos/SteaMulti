using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(int), typeof(string))]
	class MinutesToHoursConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int hours = (int)value / 60;
			int minutes = (int)value % 60;

			return hours.ToString() + " hours " + minutes.ToString() + " minutes played";// (" + value.ToString() + ")";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string ret = "";
			return ret;
		}
	}
}
