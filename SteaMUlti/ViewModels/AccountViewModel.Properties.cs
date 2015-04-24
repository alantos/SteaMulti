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
	using System.Runtime.InteropServices;

	public partial class AccountViewModel : BaseViewModel
	{

		//[DllImport("user32.dll", SetLastError = true)]
		//static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		//[DllImport("user32.dll", CharSet = CharSet.Auto)]
		//static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);

		private MainModel _accountModel;

		private Main mainWindow;

		private DataView accounts;

		public DataView Accounts
		{
			get { return accounts; }

			set
			{
				accounts = value;
				OnPropertyChanged("Accounts");
			}
		}

		private DataView games;

		private Account newAccount;

		public Account NewAccount
		{
			get { return newAccount; }
			set
			{
				newAccount = value;
				OnPropertyChanged("NewAccount");
			}
		}
		
		public DataView Games
		{
			get { return games; }

			set
			{
				games = value;
				OnPropertyChanged("Games");
			}
		}
		
		private Settings _settings;

		public Settings settings
		{
			get { return _settings; }

			set
			{
				_settings = value;
			}
		}

		private PanelSwitch _panelSwitch;

		public PanelSwitch panelSwitch
		{
			get { return _panelSwitch; }
			set
			{ 
				_panelSwitch = value;
				OnPropertyChanged("panelSwitch");
			}
		}
		
		public int cp;
		private int lastView;
		private Thread firstSetupThread;
		private Thread updateAccountsThread;
		private bool updatingAccounts;

	}
}
