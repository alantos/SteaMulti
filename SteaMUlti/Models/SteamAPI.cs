namespace SteaMUlti.Models
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Security.Cryptography;
	using System.Text;
	using System.Xml.Linq;

	public static class SteamAPI
	{
		public static long[] GetLocalSteamIds(string steamPath)
		{
			//string path = settings.SteamPath.Substring(0, settings.SteamPath.Length - 9);
			steamPath += "userdata\\";
			if (Directory.Exists(steamPath))
			{
				string[] numbersText = Directory.GetDirectories(steamPath);

				for (int i = 0; i < numbersText.Length; i++)
				{
					int last = numbersText[i].LastIndexOf('\\');
					numbersText[i] = numbersText[i].Substring(last + 1);
				}

				long[] numbers = new long[numbersText.Length];

				for (int i = 0; i < numbersText.Length; i++)
				{
					numbers[i] = long.Parse(numbersText[i]) + 76561197960265728;
				}

				return numbers;
			}

			return null;
		}

		public static string[] GetLocalSteamInfo(string steamPath)
		{
			int end = steamPath.LastIndexOf('\\');
			steamPath = steamPath.Substring(0, end);
			steamPath += "\\config\\config.vdf";

			if (File.Exists(steamPath))
			{
				StreamReader file = new StreamReader(steamPath);
				string data = file.ReadToEnd();

				string result = "";

				int beg = data.IndexOf("\"Accounts\"");
				bool doubleBracket = false;
				int close1 = data.IndexOf('{', beg);
				while (!doubleBracket)
				{
					int close2 = data.IndexOf('}', close1 + 1);
					string sub = data.Substring(close1 + 1, close2 - close1 - 1);

					if (string.IsNullOrWhiteSpace(sub))
					{
						doubleBracket = true;
						break;
					}

					string[] results = sub.Split('\"');
					result += results[1] + "\"";
					result += results[5] + "\"";

					close1 = close2;
				}

				return result.Split('\"');
			}

			return null;
		}

		public static AccountInfo GetSteamInfoByName(string accountName)
		{
			WebClient getInfoClient = new WebClient();
			getInfoClient.Proxy = null;

			string url = "http://steamcommunity.com/id/";
			url += accountName;
			url += "/?xml=1";

			string str = getInfoClient.DownloadString(new Uri(url));

			XDocument xdoc = XDocument.Load(new StringReader(str));
			var data = xdoc.Root.Elements()
						.ToDictionary(
							x => x.Name.ToString(),
							x => x.Value.ToString()
						);

			AccountInfo result = new AccountInfo();
			result.accountName = accountName;

			if (data.ContainsKey("error"))
			{
				return null;
			}
			else
			{
				if (data.ContainsKey("steamID64"))
					result.steamId = data["steamID64"];

				if (data.ContainsKey("stateMessage"))
					result.steamStatus = data["stateMessage"];
				
				if (data.ContainsKey("avatarFull"))
					result.avatarUrl = data["avatarFull"];

				return result;
			}
		}

		public static AccountInfo GetSteamInfoBySteamId(string steamId)
		{
			WebClient getSteamInfoBySteamId = new WebClient();
			getSteamInfoBySteamId.Proxy = null;
			
			string url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/";
			url += "?key=AD615BC1DCCBF492A618BC9474E76091";
			url += "&format=xml";
			url += "&steamids=";
			url += steamId;

			string str = null;
			int retry = 3;

			do
			{
				try
				{
					str = getSteamInfoBySteamId.DownloadString(new Uri(url));
					retry = 0;
				}
				catch (Exception ex)
				{
					retry--;
					if (retry < 1)
					{
						System.Windows.MessageBox.Show(ex.InnerException.ToString(), ex.Message);
						return null;
					}
				} 
			} while (retry > 0);
			
			XDocument xdoc = XDocument.Load(new StringReader(str));

			var data = xdoc.Descendants("player").Elements().ToDictionary(
				x => x.Name.ToString(),
				x => x.Value.ToString()
				);

			AccountInfo result = new AccountInfo();

			if (data.Count != 0)
			{
				result.accountName = data["personaname"];
				result.avatarUrl = data["avatarfull"];
				result.steamId = steamId;
				DateTime lastlogoff = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				lastlogoff = lastlogoff.AddSeconds(double.Parse(data["lastlogoff"]));
				result.steamStatus = "Last log off at " + lastlogoff.ToShortTimeString() + " on " + lastlogoff.ToShortDateString();
			}

			return result;
		}

		public static string GetSteamAvatar(string url, string oldPath)
		{
			WebClient getAvatarClient = new WebClient();
			getAvatarClient.Proxy = null;

			string filePath = ChooseImagePath(oldPath);
			//string filePath = steaMultiPath;
			//filePath += "Images\\Avatars\\";
			//if (!Directory.Exists(filePath))
			//	Directory.CreateDirectory(filePath);
			//filePath += accountInfo.accountName + "_avatar";
			//filePath = ChooseImagePath(filePath);

			try
			{
				getAvatarClient.DownloadFile(new Uri(url), filePath);
				return filePath;
			}
			catch (Exception e)
			{
				return e.InnerException.ToString();
			}
		}

		private static string ChooseImagePath(string path)
		{
			if (path.Contains("1.jpg"))
				return path.Substring(0, path.Length - 5) + "2.jpg";
			else
				if (path.Contains("2.jpg"))
					return path.Substring(0, path.Length - 5) + "1.jpg";
				else
				{
					path += "Images\\Avatars\\";
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);
					return path;
				}
		}

		public static AccountGamesInfo GetOwnedGamesInfo(string steamId, int accountID)
		{
			WebClient getOwnedGamesInfoClient = new WebClient();
			getOwnedGamesInfoClient.Proxy = null;

			string url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/";
			url += "?key=AD615BC1DCCBF492A618BC9474E76091";
			url += "&format=xml";
			url += "&include_appinfo=1";
			//url += "&include_played_free_games=1";
			url += "&steamid=";
			//url += steamId.ToString();
			url += steamId;

			string str;

			try { str = getOwnedGamesInfoClient.DownloadString(new Uri(url)); }
			catch (Exception) { return null; }

			if (string.IsNullOrWhiteSpace(str)) return null;
			
			XDocument xdoc = XDocument.Load(new StringReader(str));

			AccountGamesInfo result = new AccountGamesInfo();

			MD5 md5hash = MD5.Create();
			result.hash = GetMd5Hash(md5hash, xdoc.ToString());

			XElement xelem = xdoc.Descendants("game_count").First();
			if (xelem == null) return null;

			result.gameCount = int.Parse(xelem.Value);

			var data = from g in xdoc.Descendants("message")
					   select
					   new GameInfo
					   {
						   accountId = accountID,
						   appId = int.Parse(g.Element("appid").Value),
						   name = g.Element("name").Value,
						   playtime = int.Parse(g.Element("playtime_forever").Value),
						   icon = g.Element("img_icon_url").Value,
						   logo = g.Element("img_logo_url").Value
						   //hasCommunityStats = bool.TryParse(g.Element("has_community_visible_stats").Value, out temp)
					   };

			result.Games = data;

			return result;
		}

		public static string GetGameLogo(string url, string steaMultiPath, string appId)
		{
			WebClient getLogo = new WebClient();
			getLogo.Proxy = null;

			string filePath = steaMultiPath;
			filePath += "Images\\Games\\";
			if (!Directory.Exists(filePath))
				Directory.CreateDirectory(filePath);
			filePath += appId + "_header.jpg";

			try
			{
				getLogo.DownloadFile(new Uri(url), filePath);
				return filePath;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static bool IfGameInstalled(string steamPath, int appId)
		{
			int end = steamPath.LastIndexOf('\\');
			steamPath = steamPath.Substring(0, end);
			steamPath += "\\steamapps\\";
			steamPath += "appmanifest_" + appId.ToString() + ".acf";

			if (File.Exists(steamPath))
				return true;
			
			return false;
		}

		public static string GetMd5Hash(MD5 md5Hash, string input)
		{

			// Convert the input string to a byte array and compute the hash. 
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes 
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data  
			// and format each one as a hexadecimal string. 
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string. 
			return sBuilder.ToString();
		}

		public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
		{
			// Hash the input. 
			string hashOfInput = GetMd5Hash(md5Hash, input);

			// Create a StringComparer an compare the hashes.
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(hashOfInput, hash))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public class GameInfo
		{
			public int accountId;
			public int appId;
			public string name;
			public int playtime;
			public string icon;
			public string logo;
			public bool hasCommunityStats;
		}

		public class AccountGamesInfo
		{
			public int gameCount;
			public string hash;
			public IEnumerable<GameInfo> Games;
		}

		public class AccountInfo
		{
			public string steamId;
			public string accountName;
			public string avatarUrl;
			public string steamStatus;
		}

	}	// public static class SteamAPI
}


/*

 * 
 * 		// http://steamcommunity.com/id/a_lan/?xml=1
 * 		// http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=AD615BC1DCCBF492A618BC9474E76091&steamids=76561198038308469&format=xml
 * 		
		// http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=AD615BC1DCCBF492A618BC9474E76091&steamid=76561198007850536&format=xml
 * http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=AD615BC1DCCBF492A618BC9474E76091&steamid=76561198007850536&format=xml&include_appinfo=1
		// http://media.steampowered.com/steamcommunity/public/images/apps/204100/96af86331719b56cefc55298b4fcb99c99f1cfee.jpg
 * 76561198038308469
 * http://cdn4.steampowered.com/v/gfx/apps/570/header_292x136.jpg
 * 
 * folder number in steam directory + 76561197960265728 = steamId
 * 
namespace SteaMUlti.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	class SteamAsync
	{
		private string steamName;

		public string SteamName
		{
			get { return steamName; }
			set { steamName = value; }
		}

		private long steamID;

		public long SteamID
		{
			get { return steamID; }
			set { steamID = value; }
		}

		private int totalGames;

		public int TotalGames
		{
			get { return totalGames; }
			set { totalGames = value; }
		}

		private string GetSteamInfo(string accountName)
		{
			string request = SteamAPI.Request(SteamRequestFormat.XML)
				.Interface(x => x.IPlayerService)
				.Method(x => x.GetOwnedGames)
				.Parameters(x =>
				{
					x.IncludeAppInfo(true);
					x.IncludePlayedFreeGames(true);
					x.SteamId(steamID);
				})
				.GetRequest();

			string request = SteamAPI.Request(SteamRequestFormat.XML)
				.Interface(x => x.ISteamUser)
				.Method(x => x.ResolveVanityURL)
				.Parameters(x => x.VanityUrl("a_lan"))
				.GetRequest();

			WebClient webClient = new WebClient();

			webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;

			string urlString = "http://steamcommunity.com/id/";
			urlString += accountName;
			urlString += "/?xml=1";

			System.Uri url = new System.Uri(urlString);

			webClient.DownloadStringAsync(url);

			var xdoc = XDocument.Load(new StringReader(request));
			var data = xdoc.Root.Elements()
						.ToDictionary(
							x => x.Name.ToString(),
							x => x.Value.ToString()
						);

			long steamid;

			if (int.Parse(data["success"]) == 1)
			{
				steamid = long.Parse(data["steamid"]);
			}
			else
			{
				MessageBox.Show(data["message"]);
			}

			MessageBox.Show("Dupa");
			using (XmlReader reader = XmlReader.Create(new StringReader(request), settings))
			{
				reader.ReadToDescendant("success");
				int success = reader.ReadElementContentAsInt();
				reader.ReadStartElement();
				if (success == 1)
				{
					reader.ReadToDescendant("steamid");
					long steamid = reader.ReadElementContentAsLong();
				}

			}

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(request);
			writer.Flush();
			stream.Position = 0;

			<?xml version="1.0" encoding="UTF-8"?>
			<!DOCTYPE response>
			<response>
				<steamid>76561198007850536</steamid>
				<success>1</success>
			</response>

			<?xml version="1.0" encoding="UTF-8"?>
			<!DOCTYPE response>
			<response>
				<game_count>63</game_count>
				<games>
					<message>
						<appid>240</appid>
						<playtime_forever>381</playtime_forever>
					</message>
					<message>
						<appid>300</appid>
						<playtime_forever>120</playtime_forever>
					</message>

			var data = from d in xdoc.Descendants("response")
				   select new { Children = d.Descendants("") };


		}

	}


<profile>
  <steamID64>76561198007850536</steamID64>
  <steamID><![CDATA[A_lan]]></steamID>
  <onlineState>offline</onlineState>
  <stateMessage><![CDATA[Last Online: 13 days ago]]></stateMessage>
  <privacyState>public</privacyState>
  <visibilityState>3</visibilityState>
  <avatarIcon><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/05/058535dc03c703e8e61bd83339844ccf888908db.jpg]]></avatarIcon>
  <avatarMedium><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/05/058535dc03c703e8e61bd83339844ccf888908db_medium.jpg]]></avatarMedium>
  <avatarFull><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/05/058535dc03c703e8e61bd83339844ccf888908db_full.jpg]]></avatarFull>
  <vacBanned>0</vacBanned>
  <tradeBanState>None</tradeBanState>
  <isLimitedAccount>0</isLimitedAccount>
  <customURL><![CDATA[a_lan]]></customURL>
  <memberSince>March 14, 2009</memberSince>
  <steamRating></steamRating>
  <hoursPlayed2Wk>0.0</hoursPlayed2Wk>
  <headline><![CDATA[]]></headline>
  <location><![CDATA[Lancashire, United Kingdom (Great Britain)]]></location>
  <realname><![CDATA[]]></realname>
  <summary><![CDATA[No information given.]]></summary>
  <groups>
	<group isPrimary="1">
	  <groupID64>103582791429959616</groupID64>
	  <groupName><![CDATA[Operation Flashpoint 2: Dragon Rising]]></groupName>
	  <groupURL><![CDATA[opdr]]></groupURL>
	  <headline><![CDATA[Operation Flashpoint: Dragon Rising Official Steam Community]]></headline>
	  <summary><![CDATA[Official Steam Community Group for Operation Flashpoint: Dragon Rising. We follow the latest news in OF:DR and post it here. Check back regularly, make suggestions and get involved!  - We're looking to expand. Invite your friends.]]></summary>
	  <avatarIcon><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/78/786248d610a7aa5376b08739fa4a09d8a110b261.jpg]]></avatarIcon>
	  <avatarMedium><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/78/786248d610a7aa5376b08739fa4a09d8a110b261_medium.jpg]]></avatarMedium>
	  <avatarFull><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/78/786248d610a7aa5376b08739fa4a09d8a110b261_full.jpg]]></avatarFull>
	  <memberCount>388</memberCount>
	  <membersInChat>0</membersInChat>
	  <membersInGame>16</membersInGame>
	  <membersOnline>80</membersOnline>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791430400653</groupID64>
	  <groupName><![CDATA[Operation Flashpoint Dragon Rising]]></groupName>
	  <groupURL><![CDATA[OFP-DR]]></groupURL>
	  <headline><![CDATA[Operation Flashpoint Dragon Rising!]]></headline>
	  <summary><![CDATA[&quot;Taking gamers as close to war as they'll ever want to get, Operation Flashpoint: Dragon Rising is the much anticipated return of the genre-defining military conflict simulator. Set to deliver the total combat experience, Flashpoint: Dragon Rising will challenge players to survive the chaos and rapidly evolving situations of modern warfare in a new contemporary theatre.
<br>
<br>Players will experience the intensity, diversity and claustrophobia of a modern conflict from the unique perspectives of an infantry marine, a helicopter pilot, a Special Forces officer or a tank commander, each engaged against the full force of the Chinese PLA on a scale never previously experienced in a military action title. Gameplay simulates an immense conflict between advanced forces and provides unparalleled scope with different military disciplines, vehicles and equipment for players to utilize.&quot;]]></summary>
	  <avatarIcon><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/54/546c83169caee631674eebd7b0c981556a66430a.jpg]]></avatarIcon>
	  <avatarMedium><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/54/546c83169caee631674eebd7b0c981556a66430a_medium.jpg]]></avatarMedium>
	  <avatarFull><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/54/546c83169caee631674eebd7b0c981556a66430a_full.jpg]]></avatarFull>
	  <memberCount>375</memberCount>
	  <membersInChat>0</membersInChat>
	  <membersInGame>18</membersInGame>
	  <membersOnline>86</membersOnline>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791430659688</groupID64>
	  <groupName><![CDATA[OF2: Dragon Rising]]></groupName>
	  <groupURL><![CDATA[OF2DR]]></groupURL>
	  <headline><![CDATA[Operation Flashpoint 2: Dragon Rising]]></headline>
	  <summary><![CDATA[For All The Operation Flashpoint 2: Dragon Rising Fans out there... Feel free to use the group chat to organize matches on steam. Help make us the largest OF2: Dragon Rising group out there by inviting your friends!!!]]></summary>
	  <avatarIcon><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/e6/e6bba827085ad9e7c859c2d7ed82d9f82cc146f9.jpg]]></avatarIcon>
	  <avatarMedium><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/e6/e6bba827085ad9e7c859c2d7ed82d9f82cc146f9_medium.jpg]]></avatarMedium>
	  <avatarFull><![CDATA[http://media.steampowered.com/steamcommunity/public/images/avatars/e6/e6bba827085ad9e7c859c2d7ed82d9f82cc146f9_full.jpg]]></avatarFull>
	  <memberCount>37</memberCount>
	  <membersInChat>0</membersInChat>
	  <membersInGame>1</membersInGame>
	  <membersOnline>4</membersOnline>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791430787496</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791431062851</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791431671764</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791431949543</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791432008361</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791432158281</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791432448424</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791432961837</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433128082</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433279996</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433392601</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433657552</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433723341</groupID64>
	</group>
	<group isPrimary="0">
	  <groupID64>103582791433738079</groupID64>
	</group>
  </groups>
</profile>
 * 
 * <response>
  <error><![CDATA[The specified profile could not be found.]]></error>
</response>
}
*/