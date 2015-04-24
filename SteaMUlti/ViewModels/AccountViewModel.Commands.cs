namespace SteaMUlti.ViewModels
{
	using SteaMUlti.Models;
	using System.Xml;
	using System.Xml.Serialization;
	using System.IO;
	using System.Data;
	using System.Windows.Input;
	using System.Windows.Controls;
	using SteaMUlti.Views;
	using SteaMUlti.Commands;
	using System.Windows;
	using System.Diagnostics;
	
	using System.Xml.Linq;
	using System.Linq;
	using System.Net;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Text;
	using Microsoft.Win32;
	using System.Threading;
	using System.Windows.Media.Animation;
	using System.Windows.Media;
	using System.ComponentModel;
	using System.Windows.Data;
	using System.Security.Permissions;
	using System.Windows.Threading;

	public partial class AccountViewModel : BaseViewModel
	{

		#region Add Account Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand AddCommand
		{
			get;
			private set;
		}

		public void AddNewAccount()
		{
			if (Accounts != null)
			{
				NewAccount = new Account();
				ChangePanel(7);
			}
		}

		public bool AddNewCanExecute
		{
			get
			{
				//if (updatingAccounts)
				//	return false;

                return true;
			}
		}

		#endregion

		#region Add Accounts Auto Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand AddAccountsAutoCommand
		{
			get;
			private set;
		}

		public void AddAccountsAutoExecute()
		{
			firstSetupThread.Start();
		}

		public bool AddAccountsAutoCanExecute
		{
			get
			{
				if (string.IsNullOrWhiteSpace(settings.SteamPath))
					return false;
				if (_accountModel.AccountsTable.Rows.Count > 0)
					return false;
				return true;
			}
		}

		#endregion

		#region Edit Account Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand EditCommand
		{
			get;
			private set;
		}

		public void EditAccountExecute()
		{
			mainWindow.steamAccountPassword.Clear();
			ChangePanel(1);
		}

		public bool EditAccountCanExecute
		{
			get
			{
				if (updatingAccounts)
					return false;

				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedItem == null)
						return false;
					else
						return true; 
				}

				return false;
			}
		}

		#endregion

		#region Delete Account Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand DeleteAccountCommand
		{
			get;
			private set;
		}

		public void DeleteAccountExecute()
		{
			ChangePanel((int)Panels.DeleteAccountPanel);
			//DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
			//selectedRow.Delete();
			//DataRow row = ((DataRowView)mainWindow.AccountsListBox.SelectedItem).Row;
			//_accountModel.AccountsTable.Rows.Remove(row);
			//_accountModel.AccountsTable.AcceptChanges();
			//_accountModel.SteaMUltiDataSet.AcceptChanges();
			//_accountModel.SaveDataSet(_accountModel.SteaMUltiDataSet);
		}

		public bool DeleteAccountCanExecute
		{
			get
			{
				if (updatingAccounts)
					return false;

				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedItem == null)
						return false;
					else
						return true; 
				}

				return false;
			}
		}

		#endregion

		#region Move Up Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand MoveUpCommand
        {
            get;
            private set;
        }

        public void MoveUpExecute()
        {
			MoveAccount(-1);
        }

        public bool MoveUpCanExecute
        {
            get
            {
				if (updatingAccounts)
					return false;

				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedIndex == -1)
						return false;

					if (mainWindow.AccountsListBox.SelectedIndex == 0)
						return false; 
				}

				return true;
            }
        }

        #endregion

		#region Move Down Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand MoveDownCommand
		{
			get;
			private set;
		}

		public void MoveDownExecute()
		{
			MoveAccount(1);
		}

		public bool MoveDownCanExecute
		{
			get
			{
				if (updatingAccounts)
					return false;

				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedIndex == -1)
						return false;

					if (mainWindow.AccountsListBox.SelectedIndex == mainWindow.AccountsListBox.Items.Count - 1)
						return false; 
				}

				return true;
			}
		}

		#endregion

		#region Details OK Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand DetailsOKCommand
		{
			get;
			private set;
		}

		public void DetailsOKExecute(string label)
		{
			if (label.Equals("EDIT"))
			{
				if (!string.IsNullOrEmpty(mainWindow.steamAccountPassword.Password))
				{
					byte[] encryptedPassword = Crypto.Encrypt(mainWindow.steamAccountPassword.Password);

					DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
					selectedRow.Row["Password"] = encryptedPassword;
				}

				_accountModel.AccountsTable.AcceptChanges();
				ChangePanel((int)Panels.AccountsListPanel);
			}

			if (label.Equals("DELETE"))
			{
				DataRow row = ((DataRowView)mainWindow.AccountsListBox.SelectedItem).Row;
				_accountModel.AccountsTable.Rows.Remove(row);
				_accountModel.AccountsTable.AcceptChanges();
				_accountModel.AccountsTable.AcceptChanges();
				ChangePanel((int)Panels.AccountsListPanel);
			}

			if (label.Equals("NEW"))
			{
				if (!string.IsNullOrEmpty(mainWindow.newAccountPassword.Password))
				{
					byte[] encryptedPassword = Crypto.Encrypt(mainWindow.newAccountPassword.Password);

					NewAccount.Password = encryptedPassword;
				}
				
				_accountModel.AddNewAccount(NewAccount);
				ChangePanel((int)Panels.AccountsListPanel); 
			}
				

		}

		public bool DetailsOKCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Sync With Steam Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SyncWithSteamCommand
		{
			get;
			private set;
		}

		public void SyncWithSteamExecute(string label)
		{
			if (label.Equals("EDIT"))
			{
				if (!string.IsNullOrEmpty(mainWindow.steamAccountPassword.Password))
				{
					byte[] encryptedPassword = Crypto.Encrypt(mainWindow.steamAccountPassword.Password);

					DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
					selectedRow.Row["Password"] = encryptedPassword;
				}

				_accountModel.AccountsTable.AcceptChanges();
				ChangePanel((int)Panels.AccountsListPanel);
			}

			if (label.Equals("DELETE"))
			{
				DataRow row = ((DataRowView)mainWindow.AccountsListBox.SelectedItem).Row;
				_accountModel.AccountsTable.Rows.Remove(row);
				_accountModel.AccountsTable.AcceptChanges();
				_accountModel.AccountsTable.AcceptChanges();
				ChangePanel((int)Panels.AccountsListPanel);
			}

			if (label.Equals("NEW"))
			{
				if (!string.IsNullOrEmpty(mainWindow.newAccountPassword.Password))
				{
					byte[] encryptedPassword = Crypto.Encrypt(mainWindow.newAccountPassword.Password);

					NewAccount.Password = encryptedPassword;
				}

				SyncAccountWithSteam(_accountModel.AddNewAccount(NewAccount));
				
				ChangePanel((int)Panels.AccountsListPanel);
			}


		}

		public bool SyncWithSteamCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Details Cancel Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand DetailsCancelCommand
		{
			get;
			private set;
		}

		public void DetailsCancelExecute(string label)
		{
			if (label.Equals("EDIT"))
			{
				_accountModel.AccountsTable.RejectChanges();
			}
			
			ChangePanel((int)Panels.AccountsListPanel);
		}

		public bool DetailsCancelCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Lunch Steam Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand LunchSteamCommand
		{
			get;
			private set;
		}

		public void LunchSteamExecute()
		{
			KillSteamProcess();

			DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
			string username = selectedRow.Row["Username"].ToString();
			byte[] encryptedPassword = (byte[])selectedRow.Row["Password"];
			Process.Start(settings.SteamPath, " -login " + username + " " + Crypto.Decrypt(encryptedPassword));
		}

		public bool LunchSteamCanExecute
		{
			get
			{
				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedItem == null)
						return false;
					else
					{
						DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;

						if (string.IsNullOrWhiteSpace(selectedRow.Row["Username"].ToString()))
							return false;
						if (selectedRow.Row["Password"] == DBNull.Value)
							return false;

						return true;
					} 
				}

				return false;
			}
		}

		#endregion

		#region Lunch Steam App Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand LunchSteamAppCommand
		{
			get;
			private set;
		}

		public void LunchSteamAppExecute()
		{
			KillSteamProcess();

			DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
			string username = selectedRow.Row["Username"].ToString();
			byte[] encryptedPassword = (byte[])selectedRow.Row["Password"];
			string appId = ((DataRowView)mainWindow.GamesListBox.SelectedItem)["AppId"].ToString();
			Process steamProcess = Process.Start(settings.SteamPath,
				" -login " + username + " " + Crypto.Decrypt(encryptedPassword) +
				" -applaunch " + appId);

			//string windowName = "Origin";
			//string windowClass = "Qt5QWindowIcon";

			//IntPtr HWND = new IntPtr(0);
			//HWND = FindWindow(windowClass, windowName);

			//UInt32 WM_SETTEXT = 0x000C;
			//string text = "dupa";
			//IntPtr wParam = new IntPtr(text.Length);
			//StringBuilder sb = new StringBuilder(text);

			//SendMessage(HWND, WM_SETTEXT, wParam, sb);
		}

		public bool LunchSteamAppCanExecute
		{
			get
			{
				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedItem == null)
						return false;
					else
					{
						DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;

						if (string.IsNullOrWhiteSpace(selectedRow.Row["Username"].ToString()))
							return false;
						if (selectedRow.Row["Password"] == DBNull.Value)
							return false;

						return true;
					}
				}

				return false;
			}
		}

		private void KillSteamProcess()
		{
			Process[] procs = Process.GetProcessesByName("steam");

			if (procs.Length > 0)
			{
				procs[0].Kill();
			}
		}

		#endregion

		#region Browse Steam Path Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand BrowseSteamPathCommand
		{
			get;
			private set;
		}

		public void BrowseSteamPathExecute()
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.DefaultExt = ".exe";
			dialog.Filter = "Executable (.exe)|*.exe";
			dialog.Title = "Locate steam.exe on your system";
			dialog.Multiselect = false;

			Nullable<bool> result = dialog.ShowDialog();

			if (result == true)
			{
				settings.SteamPath = dialog.FileName;
				OnPropertyChanged("settings");
			}

			//GetLocalSteamIds();
			//GetSteamInfoBySteamId(0);

			//****** Old Sync Command Body *******
			//_accountModel.AccountsTable.AcceptChanges();
			//GetSteamInfo(mainWindow.AccountsListBox.SelectedIndex);
			//mainWindow.GamesListBox.ItemsSource = _accountModel.GamesTable.DefaultView;
			//DataRow accountRow = _accountModel.AccountsTable.Rows[mainWindow.AccountsListBox.SelectedIndex];
			//DataRow[] ownedGamesRows = accountRow.GetChildRows("Accounts_OwnedGames");

			//DataView ownedGamesView = new DataView(OwnedGames, "AccountId = " + accountId, "OwnedGameId", DataViewRowState.CurrentRows);
			//DataView selectedAccountGamesView = new DataView();
			//selectedAccountGamesView.AllowNew = true;

			//string filter = "GameId = ";

			//foreach (DataRow row in ownedGamesRows)
			//{
			//	filter += row["GameId"].ToString();
			//	filter += " OR GameId = ";
			//}

			//filter = filter.Substring(0, filter.Length - 13);


		}

		public bool BrowseSteamPathCanExecute
		{
			get
			{
				//return !((bool)_accountModel.AccountsTable.Rows[mainWindow.AccountsListBox.SelectedIndex]["FirstSetup"]);
				return true;
			}
		}

		#endregion

		#region Settings Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SettingsCommand
		{
			get;
			private set;
		}

		public void SettingsExecute()
		{
			if (panelSwitch.Settings)
			{
				ChangePanel(lastView);
			}
			else
			{
				lastView = settings.CurentPanel;
				ChangePanel(4);
			}
		}

		public bool SettingsCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Set Master Password Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SetMasterPasswordCommand
		{
			get;
			private set;
		}

		public void SetMasterPasswordExecute()
		{
			if (!string.IsNullOrWhiteSpace(mainWindow.masterPassword.Password))
			{
				byte[] hash = Crypto.Hash(Encoding.UTF8.GetString(Crypto.Hash(mainWindow.masterPassword.Password)));
				settings.PasswordHash = hash;
				mainWindow.masterPassword.Clear();
			}
		}

		public bool SetMasterPasswordCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region ResetDatabase Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ResetDatabaseCommand
		{
			get;
			private set;
		}

		public void ResetDatabaseExecute()
		{
			if (_accountModel.BackupDatabase())
				MessageBox.Show("Your old database was backuped, just in case, in 'date_time__accountModel.AccountsTable.xml' file");

			_accountModel.AccountsTable.DataSet.EnforceConstraints = false;
			_accountModel.AccountsTable.Clear();
			_accountModel.GamesTable.Clear();
			_accountModel.AccountsTable.DataSet.EnforceConstraints = true;
		}

		public bool ResetDatabaseCanExecute
		{
			get
			{
				if (updatingAccounts)
					return false;
				if (_accountModel.AccountsTable.Rows.Count < 1)
					return false;
				return true;
			}
		}

		#endregion

		#region ShowAccounts Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ShowAccountsCommand
		{
			get;
			private set;
		}

		public void ShowAccountsExecute()
		{
			ChangePanel(0);
		}

		public bool ShowAccountsCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region ShowGames Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ShowGamesCommand
		{
			get;
			private set;
		}

		public void ShowGamesExecute()
		{
			//string buttonText = mainWindow.ShowGamesButton.Content.ToString();
			//if (buttonText == "SHOW GAMES")
			//{
				ChangePanel(3);
			//	mainWindow.ShowGamesButton.Content = "HIDE GAMES";
			//	mainWindow.LunchSteamButton.Content = "PLAY";
			//}
			//else
			//{
			//	ChangePanel(0);
			//	mainWindow.ShowGamesButton.Content = "SHOW GAMES";
			//	mainWindow.LunchSteamButton.Content = "LUNCH STEAM";
			//}
		}

		public bool ShowGamesCanExecute
		{
			get
			{
				if (mainWindow.AccountsListBox != null)
				{
					if (mainWindow.AccountsListBox.SelectedItem == null)
						return false;
					else
						return true; 
				}

				return false;
			}
		}

		#endregion

		#region ShowAllGames Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ShowAllGamesCommand
		{
			get;
			private set;
		}

		public void ShowAllGamesExecute()
		{
			//string buttonText = mainWindow.ShowAllGamesButton.Content.ToString();
			//if (buttonText == "SHOW ALL GAMES")
			//{
				ChangePanel(6);
			//	mainWindow.ShowAllGamesButton.Content = "BACK";
			//}
			//else
			//{
			//	ChangePanel(0);
			//	mainWindow.ShowAllGamesButton.Content = "SHOW ALL GAMES";
				
			//}
		}

		public bool ShowAllGamesCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region ChangeSorting Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ChangeSortingCommand
		{
			get;
			private set;
		}

		public void ChangeSortingExecute(object parameters)
		{
			Object[] param = (Object[])parameters;
			object sender = param[0];
			ListBox listBox = (ListBox)param[1];
			listBox.Items.SortDescriptions.Clear();

			switch (((ComboBox)sender).SelectedIndex)
			{
				case 0:
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
					break;
				case 1:
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Descending));
					break;
				case 2:
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("Playtime", System.ComponentModel.ListSortDirection.Ascending));
					break;
				case 3:
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("Playtime", System.ComponentModel.ListSortDirection.Descending));
					break;
				case 4:
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("IsInstalled", System.ComponentModel.ListSortDirection.Descending));
					listBox.Items.SortDescriptions.Add(
						new System.ComponentModel.SortDescription("Playtime", System.ComponentModel.ListSortDirection.Descending));
					break;
			}
		}

		public bool ChangeSortingCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region ChooseNewAccountTypeCommand @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand ChooseNewAccountTypeCommand
		{
			get;
			private set;
		}

		public void ChooseNewAccountTypeExecute(object sender)
		{
			NewAccount.AccountType = ((ComboBox)sender).SelectedIndex;
		}

		public bool ChooseNewAccountTypeCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region SearchGames EventToCommand @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SearchGamesCommand
		{
			get;
			private set;
		}

		public void SearchGamesExecute(object parameters)
		{
			Object[] param = (Object[])parameters;
			object sender = param[0];
			ListBox listBox = (ListBox)param[1];

			if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
			{
				listBox.ItemsSource = _accountModel.GamesTable.DefaultView;
			}
			else
			{
				DataView view = new DataView(_accountModel.GamesTable);
				view.RowFilter = "Name like '%" + ((TextBox)sender).Text + "%'";
				listBox.ItemsSource = view;
			}
		}

		public bool SearchGamesCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region SetAsMainAccount Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SetAsMainAccountCommand
		{
			get;
			private set;
		}

		public void SetAsMainAccountExecute(object parameter)
		{
			foreach (DataRowView item in mainWindow.AccountsListBox.Items)
			{
				item["MainAccount"] = false;
			}

			DataRowView selectedItem = (DataRowView)(mainWindow.AccountsListBox.SelectedItem);
			selectedItem["MainAccount"] = true;
		}

		public bool SetAsMainAccountCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region HideDuplicates Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand HideDuplicatesCommand
		{
			get;
			private set;
		}

		public void HideDuplicatesExecute()
		{
			if (settings.HideDuplicates)
				settings.HideDuplicates = false;
			else
				settings.HideDuplicates = true;
		}

		public bool HideDuplicatesCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region SearchForDuplicates Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand SearchForDuplicatesCommand
		{
			get;
			private set;
		}

		public void SearchForDuplicatesExecute()
		{
			DataRow[] mainAccount = _accountModel.AccountsTable.Select("MainAccount = True");
			FindDuplicates((int)mainAccount[0]["AccountId"]);
		}

		public bool SearchForDuplicatesCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Close App Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand CloseAppCommand
		{
			get;
			private set;
		}

		public void CloseAppExecute()
		{
			_accountModel.AccountsTable.AcceptChanges();
			_accountModel.GamesTable.AcceptChanges();
			_accountModel.SaveDataSet(_accountModel.SteaMUltiDataSet);

			settings.PositionX = Application.Current.MainWindow.Left;
			settings.PositionY = Application.Current.MainWindow.Top;
			settings.SaveSettings(settings);

			updateAccountsThread.Abort();

			Application.Current.Shutdown();
		}

		public bool CloseAppCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Minimize App Command @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		public ICommand MinimizeAppCommand
		{
			get;
			private set;
		}

		public void MinimizeAppExecute()
		{
			mainWindow.WindowState = WindowState.Minimized;
		}

		public bool MinimizeAppCanExecute
		{
			get
			{
				return true;
			}
		}

		#endregion

	}
}
