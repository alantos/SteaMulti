using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace SteaMUlti.Models
{
	[Serializable]
	public class Settings : INotifyPropertyChanged
	{

		#region Properties

		public event PropertyChangedEventHandler PropertyChanged;

		private string steamPath;

		public string SteamPath
		{
			get { return steamPath; }
			set
			{
				steamPath = value;
				OnPropertyChanged("SteamPath");
			}
		}

		private string steaMUltiPath;

		public string SteaMUltiPath
		{
			get { return steaMUltiPath; }
			set
			{
				steaMUltiPath = value;
				OnPropertyChanged("SteaMUlitPath");
			}
		}

		private bool editMode;

		public bool EditMode
		{
			get { return editMode; }
			set
			{
				editMode = value;
				OnPropertyChanged("EditMode");
			}
		}

		private int curentPanel;

		public int CurentPanel
		{
			get { return curentPanel; }
			set
			{
				curentPanel = value;
				OnPropertyChanged("CurentPanel");
			}
		}

		private bool firstSetup;

		public bool FirstSetup
		{
			get { return firstSetup; }
			set
			{
				firstSetup = value;
				OnPropertyChanged("FirstSetup");
			}
		}


		private double positionX;

		public double PositionX
		{
			get { return positionX; }
			set
			{
				positionX = value;
				OnPropertyChanged("PositionX");
			}
		}

		private double positionY;

		public double PositionY
		{
			get { return positionY; }
			set
			{
				positionY = value;
				OnPropertyChanged("PositionY");
			}
		}

		private byte[] passwordHash;

		public byte[] PasswordHash
		{
			get { return passwordHash; }
			set
			{
				passwordHash = value;
				OnPropertyChanged("PasswordHash");
			}
		}

		private bool hideDuplicates;

		public bool HideDuplicates
		{
			get { return hideDuplicates; }
			set
			{
				hideDuplicates = value;
				OnPropertyChanged("HideDuplicates");
			}
		}
		


		#endregion

		#region Constructor

		public Settings()
		{
		
		}

		#endregion

		#region Functions

		public bool SaveSettings(Settings settings)
		{
			XmlSerializer writer = new XmlSerializer(typeof(Settings));

			StreamWriter file = new StreamWriter("settings.xml");

			writer.Serialize(file, settings);

			file.Close();

			return true;
		}

		public Settings LoadSettings()
		{
			if (File.Exists("settings.xml"))
			{
				XmlSerializer reader = new XmlSerializer(typeof(Settings));

				StreamReader file = new StreamReader("settings.xml");

				Settings settings = (Settings)reader.Deserialize(file);

				file.Close();

				return settings;
			}
			else
				return new Settings();
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

	}

	
}
