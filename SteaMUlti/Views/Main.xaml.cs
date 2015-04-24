using SteaMUlti.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteaMUlti.Views
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		public AccountViewModel mainViewModel;

		public Main()
		{
			InitializeComponent();
			mainViewModel = new AccountViewModel();
			DataContext = mainViewModel;
			AccountListBoxContextMenu.DataContext = mainViewModel;
			EditOkCancelPanel.DataContext = mainViewModel;
			NewAccountOkCancelPanel.DataContext = mainViewModel;
		}

		private void DragWindow(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void AccountsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GamesListBox != null)
			{
				if (GamesListBox.Items.Count > 0)
				{
					GamesListBox.UnselectAll();
					GamesListBox.ScrollIntoView(GamesListBox.Items[0]);
				} 
			}
		}

		//private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		//{
		//	AccountsListBox.Items.SortDescriptions.Add(
		//		new System.ComponentModel.SortDescription("Order", System.ComponentModel.ListSortDirection.Ascending));

		//	GamesListBox.Items.SortDescriptions.Add(
		//		new System.ComponentModel.SortDescription("Playtime", System.ComponentModel.ListSortDirection.Descending));

		//	AllGamesListBox.Items.SortDescriptions.Add(
		//		new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
		//}

	}
}
