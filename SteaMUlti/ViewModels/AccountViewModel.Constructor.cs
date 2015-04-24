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
	using System.Security.Cryptography;
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

		public AccountViewModel()
		{
			_accountModel = new MainModel();
			accounts = new DataView();
			games = new DataView();
			NewAccount = new Account();
			
			mainWindow = (Main)Application.Current.Windows[0];
			
			settings = new Settings();
			settings = settings.LoadSettings();
			panelSwitch = new PanelSwitch();
			cp = settings.CurentPanel;

			Application.Current.MainWindow.Left = settings.PositionX;
			Application.Current.MainWindow.Top = settings.PositionY;
			lastView = 0;
			if (!settings.FirstSetup)
				ChangePanel(4);
			//else
			//	settings.ViewOption = 0;
			//settings.EditMode = false;
			updatingAccounts = false;

			//accounts = _accountModel.AccountsTable;
			//games = _accountModel.GamesTable;
			_accountModel.AccountsTable.AcceptChanges();	// Accept changes so from now on database can not be rejected by RejectChanges()
			_accountModel.GamesTable.AcceptChanges();
			Accounts = _accountModel.AccountsTable.DefaultView;
			Games = _accountModel.GamesTable.DefaultView;
			
			//Accountss = _accountModel.ConvertTo(_accountModel.AccountsTable);
			//mainWindow.AccountsListBox.ItemsSource = Accounts;

			GetSteaMUltiPath();

			//mainWindow.AccountsListBox.ItemsSource = _accountModel.AccountsTable.DefaultView;
			//mainWindow.AccountsSmallListBox.ItemsSource = _accountModel.AccountsTable.DefaultView;
            
			// Add sort to the DefaultView of AccountListBox
			mainWindow.AccountsListBox.Items.SortDescriptions.Add(
				new System.ComponentModel.SortDescription("Order", System.ComponentModel.ListSortDirection.Ascending));

			// Add sort to the DefaultView of GamesListBox
			mainWindow.GamesListBox.Items.SortDescriptions.Add(
				new System.ComponentModel.SortDescription("Playtime", System.ComponentModel.ListSortDirection.Descending));

			mainWindow.AllGamesListBox.Items.SortDescriptions.Add(
				new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

			AddCommand = new AddAccountCommand(this);
			EditCommand = new EditAccountCommand(this);
			DeleteAccountCommand = new DeleteAccountCommand(this);
            MoveUpCommand = new MoveUpCommand(this);
			MoveDownCommand = new MoveDownCommand(this);
			CloseAppCommand = new CloseAppCommand(this);
			MinimizeAppCommand = new MinimizeAppCommand(this);
			DetailsOKCommand = new DetailsOKCommand(this);
			SyncWithSteamCommand = new SyncWithSteamCommand(this);
			DetailsCancelCommand = new DetailsCancelCommand(this);
			LunchSteamCommand = new LunchSteamCommand(this);
			LunchSteamAppCommand = new LunchSteamAppCommand(this);
			BrowseSteamPathCommand = new BrowseSteamPathCommand(this);
			SettingsCommand = new SettingsCommand(this);
			ShowAccountsCommand = new ShowAccountsCommand(this);
			ShowGamesCommand = new ShowGamesCommand(this);
			ShowAllGamesCommand = new ShowAllGamesCommand(this);
			SearchGamesCommand = new SearchGamesCommand(this);
			ResetDatabaseCommand = new ResetDatabaseCommand(this);
			AddAccountsAutoCommand = new AddAccountsAutoCommand(this);
			SetMasterPasswordCommand = new SetMasterPasswordCommand(this);
			ChangeSortingCommand = new ChangeSortingCommand(this);
			ChooseNewAccountTypeCommand = new ChooseNewAccountTypeCommand(this);
			SetAsMainAccountCommand = new SetAsMainAccountCommand(this);
			HideDuplicatesCommand = new HideDuplicatesCommand(this);
			SearchForDuplicatesCommand = new SearchForDuplicatesCommand(this);
			
			firstSetupThread = new Thread(new ThreadStart(FirstSetup));
			updateAccountsThread = new Thread(new ThreadStart(UpdateAccounts));
			updateAccountsThread.Start();
		}

		private void GetSteaMUltiPath()
		{
            if (settings.SteaMUltiPath != AppDomain.CurrentDomain.BaseDirectory)
            {
                ChangePathsInDB(settings.SteaMUltiPath, AppDomain.CurrentDomain.BaseDirectory);
                settings.SteaMUltiPath = AppDomain.CurrentDomain.BaseDirectory;
            }
		}

        private void ChangePathsInDB(string oldPath, string newPath)
        {
			for (int i = 0; i < _accountModel.AccountsTable.Rows.Count; i++)
            {
				_accountModel.AccountsTable.Rows[i]["ImagePath"] = ((string)_accountModel.AccountsTable.Rows[i]["ImagePath"]).Replace(oldPath, newPath);
            }

			_accountModel.AccountsTable.AcceptChanges();

			for (int i = 0; i < _accountModel.GamesTable.Rows.Count; i++)
            {
				_accountModel.GamesTable.Rows[i]["LogoLocalPath"] = ((string)_accountModel.GamesTable.Rows[i]["LogoLocalPath"]).Replace(oldPath, newPath);
            }

			_accountModel.GamesTable.AcceptChanges();
        }

	}
}
