using System;
using System.IO;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SteaMUlti.ViewModels;

namespace SteaMUlti.Models
{
	public class Account : BaseViewModel
	{
		public Account()
		{
			AccountType = 0;
			ImagePath = "/Resources/Images/noImage.png";
		}

		//public int AccountId { get; set; }
		//public int Order { get; set; }

		private int accountType;

		public int AccountType
		{
			get { return accountType; }
			set
			{
				accountType = value;
				OnPropertyChanged("AccountType");
			}
		}

		private string accountName;

		public string AccountName
		{
			get { return accountName; }
			set
			{
				accountName = value;
				OnPropertyChanged("AccountName");
			}
		}

		private string imagePath;

		public string ImagePath
		{
			get { return imagePath; }
			set
			{
				imagePath = value;
				OnPropertyChanged("ImagePath");
			}
		}

		private string userName;

		public string UserName
		{
			get { return userName; }
			set
			{
				userName = value;
				OnPropertyChanged("UserName");
			}
		}

		private byte[] password;

		public byte[] Password
		{
			get { return password; }
			set { password = value; }
		}

	}


}
