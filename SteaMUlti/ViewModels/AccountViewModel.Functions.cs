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

		private void FirstSetup()
		{
			DispatchIfNecessary(() => { ChangePanel(5); });
			
			string[] accounts = SteamAPI.GetLocalSteamInfo(settings.SteamPath);

			DispatchIfNecessary(() => { DisplayLoadingText("Running first setup ..."); });

			if (accounts != null)
			{
				for (int i = 1; i < accounts.Length - 1; i += 2)
				{
					_accountModel.AddAccount(accounts[i], accounts[i - 1]);
					DispatchIfNecessary(() => { DisplayLoadingText("Adding account:    " + accounts[i - 1]); });
				}

				for (int i = 0; i < _accountModel.AccountsTable.Rows.Count; i++)
				{
					DispatchIfNecessary(() => { DisplayLoadingText("Downloading account info:    " + _accountModel.AccountsTable.Rows[i]["UserName"]); });

					SteamAPI.AccountInfo steamInfo = SteamAPI.GetSteamInfoBySteamId(_accountModel.AccountsTable.Rows[i]["SteamId"].ToString());
					//string avatarPath = SteamAPI.GetSteamAvatar(steamInfo.avatarUrl, settings.SteaMUltiPath);

					SteamAPI.AccountGamesInfo gamesInfo = SteamAPI.GetOwnedGamesInfo(
															_accountModel.AccountsTable.Rows[i]["SteamId"].ToString(),
															int.Parse(_accountModel.AccountsTable.Rows[i]["AccountId"].ToString()));

					DispatchIfNecessary(() => { _accountModel.UpdateAccount(i, steamInfo.accountName, steamInfo.avatarUrl, steamInfo.steamStatus, gamesInfo.gameCount, gamesInfo.hash); });

					foreach (SteamAPI.GameInfo g in gamesInfo.Games)
					{
						DispatchIfNecessary(() => { _accountModel.AddGame(g, settings.SteaMUltiPath, SteamAPI.IfGameInstalled(settings.SteamPath, g.appId)); });
					}
				}

				DispatchIfNecessary(() => { ChangePanel(0); });
			}
			settings.FirstSetup = true;
		}	// END OF FirstSetup()

		private void UpdateAccounts()
		{
			updatingAccounts = true;
			
			//DispatchIfNecessary(() =>
			//{
			//	DisplayLoadingBottomBarText("Updating account: ");
			//	mainWindow.LoadingBottomBar.Visibility = Visibility.Visible;
			//});

			for (int i = 0; i < _accountModel.AccountsTable.Rows.Count; i++)
			{
				if ((int)_accountModel.AccountsTable.Rows[i]["AccountType"] == 0)
				{
					string filePath = settings.SteaMUltiPath;
					filePath += "Images\\Avatars\\updating.jpg";

					DispatchIfNecessary(() =>
					{
						_accountModel.UpdateSteamStatus(i, "Updating account info...");
						DisplayLoadingBottomBarText("Updating account: " + _accountModel.AccountsTable.Rows[i]["AccountName"].ToString());
					});

					SteamAPI.AccountInfo steamInfo = SteamAPI.GetSteamInfoBySteamId(_accountModel.AccountsTable.Rows[i]["SteamId"].ToString());
					//string avatarPath = SteamAPI.GetSteamAvatar(steamInfo.avatarUrl, _accountModel.AccountsTable.Rows[i]["ImagePath"].ToString());

					SteamAPI.AccountGamesInfo gamesInfo = SteamAPI.GetOwnedGamesInfo(
															_accountModel.AccountsTable.Rows[i]["SteamId"].ToString(),
															int.Parse(_accountModel.AccountsTable.Rows[i]["AccountId"].ToString()));

					if (gamesInfo != null)
					{
						foreach (SteamAPI.GameInfo g in gamesInfo.Games)
						{
							DispatchIfNecessary(() => { _accountModel.UpdateGame(g, settings.SteaMUltiPath, SteamAPI.IfGameInstalled(settings.SteamPath, g.appId)); });
						}

						DispatchIfNecessary(() => { _accountModel.UpdateAccount(i, steamInfo.accountName, steamInfo.avatarUrl, steamInfo.steamStatus, gamesInfo.gameCount, gamesInfo.hash); });
					} 
				}
			}

			//DispatchIfNecessary(() => { mainWindow.LoadingBottomBar.Visibility = Visibility.Collapsed; });

			updatingAccounts = false;
		}

		private void SyncAccountWithSteam(DataRow account)
		{
			int i = _accountModel.AccountsTable.Rows.IndexOf(account);

			SteamAPI.AccountInfo steamInfo = SteamAPI.GetSteamInfoByName(account["AccountName"].ToString());
			_accountModel.UpdateAccount(i, steamInfo.steamId, steamInfo.avatarUrl, steamInfo.steamStatus);
			string avatarPath = SteamAPI.GetSteamAvatar(steamInfo.avatarUrl, settings.SteaMUltiPath);

			SteamAPI.AccountGamesInfo gamesInfo = SteamAPI.GetOwnedGamesInfo(
													_accountModel.AccountsTable.Rows[i]["SteamId"].ToString(),
													int.Parse(_accountModel.AccountsTable.Rows[i]["AccountId"].ToString()));

			_accountModel.UpdateAccount(i, steamInfo.accountName, steamInfo.avatarUrl, steamInfo.steamStatus, gamesInfo.gameCount, gamesInfo.hash);

			foreach (SteamAPI.GameInfo g in gamesInfo.Games)
			{
				_accountModel.AddGame(g, settings.SteaMUltiPath, SteamAPI.IfGameInstalled(settings.SteamPath, g.appId));
			}
		}

		private void DisplayLoadingText(string text)
		{
			mainWindow.LoadingText.Text = text;
		}

		private void DisplayLoadingBottomBarText(string text)
		{
			//mainWindow.LoadingBottomBar.Text = text;
		}

		private void DispatchIfNecessary(Action action)
		{
			try
			{
				if (!Application.Current.Dispatcher.CheckAccess())
					Application.Current.Dispatcher.Invoke(action);
				else
					action.Invoke();
			}
			catch (Exception ex)
			{
				
			}
		}

		private void MoveAccount(int where)
		{
			DataRowView selectedRow = (DataRowView)mainWindow.AccountsListBox.SelectedItem;
			DataRowView targetRow = (DataRowView)mainWindow.AccountsListBox.Items[mainWindow.AccountsListBox.SelectedIndex + where];
			
			int selectedOrder = (int)selectedRow.Row["Order"];
			int targetOrder = (int)targetRow.Row["Order"];
			targetRow.Row["Order"] = -1;
			selectedRow.Row["Order"] = targetOrder;
			targetRow["Order"] = selectedOrder;

			_accountModel.AccountsTable.AcceptChanges();
			mainWindow.AccountsListBox.ScrollIntoView(selectedRow);
		}

		private void FindDuplicates(int mainAccountId)
		{
			DataRow[] mainAccountGames = _accountModel.GamesTable.Select("AccountId = " + mainAccountId.ToString());
			DataRow[] restGames = _accountModel.GamesTable.Select("AccountId <> " + mainAccountId.ToString());

			for (int j = 0; j < restGames.Length; j++)
				restGames[j]["Duplicate"] = false;
			
			for (int i = 0; i < mainAccountGames.Length; i++)
			{
				mainAccountGames[i]["Duplicate"] = false;
				for (int j = 0; j < restGames.Length; j++)
					if ((int)mainAccountGames[i]["AppId"] == (int)restGames[j]["AppId"])
						restGames[j]["Duplicate"] = true;
			}
		}

	}
}
