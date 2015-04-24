using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using SteaMUlti.Views;

namespace SteaMUlti.Converters
{
	class ViewModelLocatorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Main mainWindow = (Main)value;
			return mainWindow.mainViewModel;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
