using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using SteaMUlti.ViewModels;
using System.Data;

namespace SteaMUlti.Converters
{
	[ValueConversion(typeof(int), typeof(string))]
	class AccountIdToNameConverter : IMultiValueConverter
	{
		public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
			int id = (int)value[0];
			AccountViewModel viewModel = (AccountViewModel)value[1];
			DataView accounts = viewModel.Accounts;
			accounts.Sort = "AccountId";
			int rowId = accounts.Find(id);
			return accounts[rowId]["AccountName"].ToString();
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
