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

	enum Panels
	{
		AccountsListPanel,
		EditAccountPanel,
		DeleteAccountPanel,
		GamesListPanel,
		SettingsPanel,
		LoadingPanel,
		AllGamesListPanel,
		NewAccountPanel
	};

	public partial class AccountViewModel : BaseViewModel
	{

		private void ChangePanel(int option)
		{
			settings.CurentPanel = option;
			panelSwitch.ChangePanel(option);
			
		} // ChangeView()

		private void AnimateOut(StackPanel panel)
		{
			DoubleAnimation anim = new DoubleAnimation();
			anim.From = panel.ActualHeight;
			anim.To = 0;
			anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
			anim.Completed += (sender, eargs) => { panel.Visibility = Visibility.Collapsed; AnimateIn(mainWindow.EditModeControls); };
			
			panel.BeginAnimation(StackPanel.HeightProperty, anim);
		}

		private void AnimateIn(StackPanel panel)
		{
			panel.Visibility = Visibility.Visible;

			DoubleAnimation anim = new DoubleAnimation();
			anim.From = 0;
			anim.To = panel.ActualHeight;
			anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));

			panel.BeginAnimation(StackPanel.MaxHeightProperty, anim);

		}

		private void AnimateWindowHeight(int height)
		{
			DoubleAnimation anim = new DoubleAnimation();
			anim.From = mainWindow.ActualHeight;
			anim.To = height;
			anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
			CircleEase func = new CircleEase();
			func.EasingMode = EasingMode.EaseOut;
			anim.EasingFunction = func;
			mainWindow.BeginAnimation(Window.HeightProperty, anim);
		}

	}

	public class PanelSwitch : BaseViewModel
	{
		private bool accounts;

		public bool Accounts
		{
			get { return accounts; }
			set
			{
				accounts = value;
				OnPropertyChanged("Accounts");
			}
		}

		private bool games;

		public bool Games
		{
			get { return games; }
			set
			{
				games = value;
				OnPropertyChanged("Games");
			}
		}

		private bool delete;

		public bool Delete
		{
			get { return delete; }
			set
			{
				delete = value;
				OnPropertyChanged("Delete");
			}
		}
		

		private bool allGames;

		public bool AllGames
		{
			get { return allGames; }
			set
			{
				allGames = value;
				OnPropertyChanged("AllGames");
			}
		}

		private bool settings;

		public bool Settings
		{
			get { return settings; }
			set
			{
				settings = value;
				OnPropertyChanged("Settings");
			}
		}

		private bool edit;

		public bool Edit
		{
			get { return edit; }
			set
			{
				edit = value;
				OnPropertyChanged("Edit");
			}
		}

		private bool newAccount;

		public bool NewAccount
		{
			get { return newAccount; }
			set
			{
				newAccount = value;
				OnPropertyChanged("NewAccount");
			}
		}

		public PanelSwitch()
		{
			Accounts = true;
			Games = false;
			Delete = false;
			AllGames = false;
			Settings = false;
			Edit = false;
			NewAccount = false;
		}

		public void ChangePanel(int option)
		{
			Accounts = false;
			Games = false;
			Delete = false;
			AllGames = false;
			Settings = false;
			Edit = false;
			NewAccount = false;

			switch (option)
			{
				case 0:
					Accounts = true;
					break;
				case 1:
					Edit = true;
					break;
				case 2:
					Delete = true;
					break;
				case 3:
					Games = true;
					break;
				case 4:
					Settings = true;
					break;
				case 6:
					AllGames = true;
					break;
				case 7:
					NewAccount = true;
					break;
			}
		}
	}
}
