using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DBNull.Value)
				return Visibility.Visible;
			else
			{
				if (parameter == null)
				{
					if ((bool)value)
						return Visibility.Visible;
					else
						return Visibility.Collapsed; 
				}
				else
				{
					if ((bool)value)
						if ((bool)parameter)
							return Visibility.Collapsed;
						else
							return Visibility.Visible;
					else
						if ((bool)parameter)
							return Visibility.Visible;
						else
							return Visibility.Collapsed;
				}
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string ret = "";
			return ret;
		}
	}
}
