using System;
using System.IO;
using System.Data;
using System.Collections.ObjectModel;
using SteaMUlti.ViewModels;

namespace SteaMUlti.Models
{
	enum AccountTypes
	{
		Steam, Origin, Uplay
	};

	partial class MainModel
	{

		#region Properties

		private DataSet steaMUltiDataSet;

		public DataSet SteaMUltiDataSet
		{
			get { return steaMUltiDataSet; }
			set { steaMUltiDataSet = value; }
		}

		private DataTable accountsTable;

		public DataTable AccountsTable
		{
			get { return accountsTable; }
			set { accountsTable = value; }
		}

		private DataTable gamesTable;

		public DataTable GamesTable
		{
			get { return gamesTable; }
			set { gamesTable = value; }
		}

		#endregion

		#region Constructor
	
		public MainModel()
		{
			steaMUltiDataSet = new DataSet();
			steaMUltiDataSet.DataSetName = "steaMUlti";
			AddAccountsTable();
			AddGamesTable();
			AddOwnedGamesRelationship();
			LoadDataSet();
			
		}

		#endregion

		#region Add Tables

		private void AddAccountsTable()
		{
			DataColumn accountId = new DataColumn("AccountId", typeof(int));
			accountId.ReadOnly = true;
			accountId.AllowDBNull = false;
			accountId.Unique = true;
			accountId.AutoIncrement = true;
			accountId.AutoIncrementSeed = 0;
			accountId.AutoIncrementStep = 1;
			DataColumn mainAccount = new DataColumn("MainAccount", typeof(bool));
			mainAccount.DefaultValue = false;
			DataColumn accountType = new DataColumn("AccountType", typeof(int));
			accountType.DefaultValue = false;
			DataColumn order = new DataColumn("Order", typeof(int));
			order.Unique = true;
			DataColumn accountName = new DataColumn("AccountName", typeof(string));
			DataColumn imagePath = new DataColumn("ImagePath", typeof(string));
			DataColumn userName = new DataColumn("UserName", typeof(string));
			DataColumn password = new DataColumn("Password", typeof(byte[]));
			DataColumn firstSetup = new DataColumn("FirstSetup", typeof(bool));
			firstSetup.DefaultValue = false;
			DataColumn steamId = new DataColumn("SteamId", typeof(long));
			DataColumn steamStatus = new DataColumn("SteamStatus", typeof(string));
			DataColumn gameCount = new DataColumn("GameCount", typeof(int));
			DataColumn lastUpdateHash = new DataColumn("LastUpdateHash", typeof(string));

			accountsTable = new DataTable("Accounts");
			accountsTable.Columns.AddRange(new DataColumn[] 
				{
					accountId,
					accountType,
					mainAccount,
					order,
					accountName,
					imagePath,
					userName,
					password,
					firstSetup,
					steamId,
					steamStatus,
					gameCount,
					lastUpdateHash
				});
			accountsTable.PrimaryKey = new DataColumn[] { accountsTable.Columns[0] };

			steaMUltiDataSet.Tables.Add(accountsTable);
		}

		private void AddGamesTable()
		{
			DataColumn gameId = new DataColumn("GameId", typeof(int));
			gameId.ReadOnly = true;
			gameId.AllowDBNull = false;
			gameId.Unique = true;
			gameId.AutoIncrement = true;
			gameId.AutoIncrementSeed = 0;
			gameId.AutoIncrementStep = 1;
			DataColumn accountId = new DataColumn("AccountId", typeof(int));
			DataColumn appId = new DataColumn("AppId", typeof(int));
			DataColumn duplicate = new DataColumn("Duplicate", typeof(bool));
			duplicate.DefaultValue = false;
			DataColumn name = new DataColumn("Name", typeof(string));
			DataColumn playtime = new DataColumn("Playtime", typeof(int));
			DataColumn isInstalled = new DataColumn("IsInstalled", typeof(bool));
			isInstalled.DefaultValue = false;
			DataColumn icon = new DataColumn("Icon", typeof(string));
			DataColumn logo = new DataColumn("Logo", typeof(string));
			DataColumn logoLocalPath = new DataColumn("LogoLocalPath", typeof(string));
			DataColumn hasCommunityStats = new DataColumn("HasCommunityStats", typeof(bool));
			hasCommunityStats.DefaultValue = false;

			gamesTable = new DataTable("Games");
			gamesTable.Columns.AddRange(new DataColumn[]
				{
					gameId,
					accountId,
					appId,
					duplicate,
					name,
					playtime,
					isInstalled,
					icon,
					logo,
					logoLocalPath,
					hasCommunityStats
				});
			gamesTable.PrimaryKey = new DataColumn[] { gamesTable.Columns[0] };

			Constraint unique = new UniqueConstraint("AccountApp", new DataColumn[] { accountId, appId }, false);
			gamesTable.Constraints.Add(unique);

			steaMUltiDataSet.Tables.Add(gamesTable);
		}

		private void AddOwnedGamesRelationship()
		{
			DataRelation dr = new DataRelation("OwnedGames",
				steaMUltiDataSet.Tables["Accounts"].Columns["AccountId"],
				steaMUltiDataSet.Tables["Games"].Columns["AccountId"]
				);

			steaMUltiDataSet.Relations.Add(dr);
		}

		#endregion

		#region GAMES LOGIC

		public void AddGame(SteamAPI.GameInfo game, string steaMUltiPath, bool isInstalled)
		{
			string filePath = steaMUltiPath;
			filePath += "Images\\Games\\";
			filePath += game.appId.ToString() + "_header.jpg";

			DataRow newRow = GamesTable.NewRow();
			newRow["AccountId"] = game.accountId;
			newRow["AppId"] = game.appId;
			newRow["Name"] = game.name;
			newRow["Playtime"] = game.playtime;
			newRow["IsInstalled"] = isInstalled;
			newRow["Icon"] = game.icon;
			newRow["Logo"] = game.logo;
			newRow["LogoLocalPath"] = filePath;
			newRow["HasCommunityStats"] = game.hasCommunityStats;

			if (!File.Exists(filePath))
			{
				string url = "http://cdn.akamai.steamstatic.com/steam/apps/";
				url += game.appId.ToString() + "/";
				url += "header.jpg";
				SteamAPI.GetGameLogo(url, steaMUltiPath, game.appId.ToString());
			}

			try { GamesTable.Rows.Add(newRow); }
			catch (Exception) { }
			GamesTable.AcceptChanges();
		}

		public void UpdateGame(SteamAPI.GameInfo game, string steaMUltiPath, bool isInstalled)
		{
			DataRow[] gameRow = GamesTable.Select("AccountId = " + game.accountId + " and AppId = " + game.appId.ToString());

			if (gameRow.Length != 0)
			{
				gameRow[0]["Playtime"] = game.playtime;
				gameRow[0]["IsInstalled"] = isInstalled;
			}
			else
				AddGame(game, steaMUltiPath, isInstalled);
		}

		#endregion

		#region ACCOUNT LOGIC

		public void AddAccount(string steamId, string userName)
		{
			DataRow newDataRow = AccountsTable.NewRow();
			newDataRow["ImagePath"] = "/Resources/Images/noImage.png";
			newDataRow["SteamId"] = long.Parse(steamId);
			newDataRow["UserName"] = userName;
			newDataRow["Order"] = newDataRow["AccountId"];

			try { AccountsTable.Rows.Add(newDataRow); }
			catch (Exception e) { System.Windows.MessageBox.Show(e.ToString()); }
			AccountsTable.AcceptChanges();
		}

		public DataRow AddNewAccount(Account account)
		{
			DataRow newDataRow = AccountsTable.NewRow();
			newDataRow["ImagePath"] = account.ImagePath;
			newDataRow["UserName"] = account.UserName;
			newDataRow["Order"] = newDataRow["AccountId"];
			newDataRow["AccountName"] = account.AccountName;
			newDataRow["AccountType"] = account.AccountType;
			newDataRow["Password"] = account.Password;

			try { AccountsTable.Rows.Add(newDataRow); }
			catch (Exception e) { System.Windows.MessageBox.Show(e.ToString()); }
			AccountsTable.AcceptChanges();

			return newDataRow;
		}

		public void UpdateAccount(int index, string accountName, string imagePath, string steamStatus, int gameCount, string lastUpdateHash)
		{
			AccountsTable.Rows[index]["AccountName"] = accountName;
			AccountsTable.Rows[index]["ImagePath"] = imagePath;
			AccountsTable.Rows[index]["SteamStatus"] = steamStatus;
			AccountsTable.Rows[index]["GameCount"] = gameCount;
			AccountsTable.Rows[index]["LastUpdateHash"] = lastUpdateHash;
			AccountsTable.AcceptChanges();
		}

		public void UpdateAccount(int index, string steamId, string imagePath, string steamStatus)
		{
			AccountsTable.Rows[index]["SteamId"] = long.Parse(steamId);
			AccountsTable.Rows[index]["ImagePath"] = imagePath;
			AccountsTable.Rows[index]["SteamStatus"] = steamStatus;
			AccountsTable.AcceptChanges();
		}

		public void UpdateAccountPhoto(int index, string imagePath)
		{
			AccountsTable.Rows[index]["ImagePath"] = imagePath;
			AccountsTable.AcceptChanges();
		}

		public void UpdateSteamStatus(int index, string status)
		{
			AccountsTable.Rows[index]["SteamStatus"] = status;
			AccountsTable.AcceptChanges();
		}

		

		#endregion

		#region DataSet Functions

		public bool SaveDataSet(DataSet ds)
		{
			ds.WriteXml("accounts.xml");
			ds.WriteXmlSchema("accounts.xsd");

			return true;
		}

		private bool LoadDataSet()
		{
			string filePath = "accounts.xml";

			if (File.Exists(filePath))
			{
				try { steaMUltiDataSet.ReadXml(filePath); }
				catch (Exception) 
				{
					BackupDatabase();
					steaMUltiDataSet.RejectChanges();
					System.Windows.MessageBox.Show(
						"Error: Database file is corrupted and program could not load the data\n" +
						"Database file was backuped for manual correction as 'date_time_accounts.xml' " +
						"\nNew database was created"
						);
				}
				return true;
			}
			else
				return false;
		}

		public bool BackupDatabase()
		{
			string filePath = "accounts.xml";

			if (File.Exists(filePath))
			{
				DateTime now = DateTime.Now;
				string backupFile = "\\backups\\";
				if (!Directory.Exists(backupFile))
					Directory.CreateDirectory(backupFile);
				backupFile +=
					now.Year.ToString() +
					now.Month.ToString() +
					now.Day.ToString() + "_" +
					now.Hour.ToString() +
					now.Minute.ToString() +
					now.Second.ToString() + "_accounts.xml";
				
				File.Copy(filePath, backupFile);

				return true;
			}
			else
				return false;
		}

		#endregion

	}

	
}
