﻿using Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GameLauncher_Console
{
	/// <summary>
	/// Contains the definition of the game data and its manipulation logic
	/// </summary>
	public static class CGameData
	{
		/// <summary>
		/// Enumerator containing currently supported game platforms
		/// </summary>
		public enum GamePlatform
		{
			[Description("Unknown")]
			Unknown = -1,
			[Description("Favourites")]
			Favourites = 0,
			[Description("Custom games")]
			Custom = 1,
			[Description("All games")]
			All = 2,
			[Description("Steam")]
			Steam = 3,
			[Description("GOG Galaxy")]
			GOG = 4,
			[Description("Ubisoft Connect")]
			Uplay = 5,
			[Description("Origin")]  // [soon to be "EA Desktop"]
			Origin = 6,
			[Description("Epic")]
			Epic = 7,
			[Description("Betheda.net")]
			Bethesda = 8,
			[Description("Battle.net")]
			Battlenet = 9,
			[Description("Rockstar")]		// TODO
			Rockstar = 10,
			[Description("Hidden games")]	// TODO
			Hidden = 11,
			[Description("Search results")]
			Search = 12,
			[Description("Amazon")]
			Amazon = 13,
			[Description("Big Fish")]
			BigFish = 14,
			[Description("Arc")]			// TODO
			Arc = 15,
			[Description("itch")]
			Itch = 16,
			[Description("Paradox")]        // TODO
			Paradox = 17,
			[Description("Plarium Play")]   // TODO
			Plarium = 18,
			[Description("Twitch")]			// TODO
			Twitch = 19,
			[Description("Wargaming.net")]  // TODO
			Wargaming = 20,
			[Description("Indiegala Client")]
			IGClient = 21,
			[Description("New games")]
			New = 22,
			[Description("Not installed")]  // TODO
			NotInstalled = 23,
		}

		public enum Match
		{
			[Description("No matches found")]
			NoMatches = 0,
			[Description("Any word fuzzy match")]
			BeginAnyWord = 5,
			[Description("Last word fuzzy match")]
			BeginLastWord = 15,
			[Description("Subtitle fuzzy match")]
			BeginSubtitle = 25,
			[Description("Alias fuzzy match")]
			BeginAlias = 50,
			[Description("Title fuzzy match")]
			BeginTitle = 60,
			[Description("Exact alias match")]
			ExactAlias = 90,
			[Description("Exact title match")]
			ExactTitle = 100
		}

		public static List<string> articles = new List<string>()
		{
			"The ",								// English definite
			"A ", "An "							// English indefinite
			/*
			"El ", "La ", "Los ", "Las ",		// Spanish definite
			"Un ", "Una ", "Unos ", "Unas ",	// Spanish indefinite
			"Le ", "Les ", "L\'",				//, "La" [Spanish] // French definite
			"Une ", "De ", "Des ",				//, "Un" [Spanish] // French indefinite [many French sort with indefinite article]
			"Der", "Das",						//, "Die" [English word] // German definite
			"Ein", "Eine"						// German indefinite
			*/
        };

		/// <summary>
		/// Contains information about a game
		/// </summary>
		public class CGame
		{
			private readonly string m_strID;
			private readonly string m_strTitle;
			private readonly string m_strLaunch;
			private readonly string m_strIcon;
			private readonly string m_strUninstall;
			private bool m_bIsInstalled;
			private bool m_bIsFavourite;
			private bool m_bIsNew;
			private bool m_bIsHidden;
			private string m_strAlias;
			private readonly GamePlatform m_platform;
			private double m_fOccurCount;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="strID">Unique ID for the game</param>
			/// <param name="strTitle">Title of the game</param>
			/// <param name="strLaunch">Game's launch command</param>
			/// <param name="strIconPath">Path to game's icon</param>
			/// <param name="strUninstall">Path to game's uninstaller</param>
			/// <param name="bIsInstalled">Flag indicating if the game is installed</param>
			/// <param name="bIsFavourite">Flag indicating if the game is in the favourite tab</param>
			/// <param name="bIsNew">Flag indicating the game was just found in the latest scan</param>
			/// <param name="bIsHidden">Flag indicating the game is hidden</param>
			/// <param name="strAlias">Alias for command-line or insert mode, user-definable, defaults to exe name</param>
			/// <param name="platformEnum">Game's platform enumerator</param>
			/// <param name="fOccurCount">Game's frequency counter</param>
			protected CGame(string strID, string strTitle, string strLaunch, string strIconPath, string strUninstall, bool bIsInstalled, bool bIsFavourite, bool bIsNew, bool bIsHidden, string strAlias, GamePlatform platformEnum, double fOccurCount)
			{
				m_strID = strID;
				m_strTitle = strTitle;
				m_strLaunch = strLaunch;
				m_strIcon = strIconPath;
				m_strUninstall = strUninstall;
				m_bIsInstalled = bIsInstalled;
				m_bIsFavourite = bIsFavourite;
				m_bIsNew = bIsNew;
				m_bIsHidden = bIsHidden;
				m_strAlias = strAlias;
				m_platform = platformEnum;
				m_fOccurCount = fOccurCount;
			}

			/// <summary>
			/// Equals override for HashSet comparison.
			/// </summary>
			/// <param name="other">Object to compare against</param>
			/// <returns>True is other is not null and the titles are matching</returns>
			public override bool Equals(object other)
			{
				// We're only interested in comparing the titles
				return (other is CGame game && this.m_strTitle == game.m_strTitle);
			}

			/// <summary>
			/// Return the hash code of this object's title variable
			/// </summary>
			/// <returns>Hash code</returns>
			public override int GetHashCode()
			{
				return this.m_strTitle.GetHashCode();
			}

			/// <summary>
			/// ID getter
			/// </summary>
			public string ID
			{
				get
				{
					return m_strID;
				}
			}

			/// <summary>
			/// Title getter
			/// </summary>
			public string Title
			{
				get
				{
					return m_strTitle;
				}
			}

			/// <summary>
			/// Launch command getter
			/// </summary>
			public string Launch
			{
				get
				{
					return m_strLaunch;
				}
			}

			/// <summary>
			/// Icon command getter
			/// </summary>
			public string Icon
			{
				get
				{
					return m_strIcon;
				}
			}

			/// <summary>
			/// Uninstaller command getter
			/// </summary>
			public string Uninstaller
			{
				get
				{
					return m_strUninstall;
				}
			}

			/// <summary>
			/// Installed flag getter/setter
			/// </summary>
			public bool IsInstalled
			{
				get
				{
					return m_bIsInstalled;
				}
				set
				{
					m_bIsInstalled = value;
				}
			}

			/// <summary>
			/// Favourite flag getter/setter
			/// </summary>
			public bool IsFavourite
			{
				get
				{
					return m_bIsFavourite;
				}
				set
				{
					m_bIsFavourite = value;
				}
			}

			/// <summary>
			/// New flag getter/setter
			/// </summary>
			public bool IsNew
			{
				get
				{
					return m_bIsNew;
				}
				set
				{
					m_bIsNew = value;
				}
			}

			/// <summary>
			/// Hidden flag getter/setter
			/// </summary>
			public bool IsHidden
			{
				get
				{
					return m_bIsHidden;
				}
				set
				{
					m_bIsHidden = value;
				}
			}

			/// <summary>
			/// Alias getter/setter
			/// </summary>
			public string Alias
			{
				get
				{
					return m_strAlias;
				}
				set
				{
					m_strAlias = value;
				}
			}

			/// <summary>
			/// Platform enumerator getter
			/// </summary>
			public GamePlatform Platform
			{
				get
				{
					return m_platform;
				}
			}

			/// <summary>
			/// Platform string getter
			/// </summary>
			public string PlatformString
			{
				get
				{
					return GetPlatformString(m_platform);
				}
			}

			/// <summary>
			/// OccurCount getter
			/// </summary>
			public double Frequency
			{
				get
				{
					return m_fOccurCount;
				}
			}

			/// <summary>
			/// Increment the frequency counter by 1
			/// </summary>
			public void IncrementFrequency()
			{
				m_fOccurCount += 5;
			}

			/// <summary>
			/// Decrease the frequency counter by 10%
			/// </summary>
			public void DecimateFrequency()
			{
				if (m_fOccurCount > 0f)
					m_fOccurCount *= 0.9f;
			}
		}

		/// <summary>
		/// Wrapper class for the Game class
		/// The goal is to make the Game class visible to the rest of the client, but make it impossible to create new instances outside of the AddGame() function
		/// </summary>
		private class CGameInstance : CGame
		{
			/// <summary>
			/// Constructor.
			/// Call constructor of base class
			/// </summary>
			/// <param name="strID">Unique ID of the game</param>
			/// <param name="strTitle">Title of the game</param>
			/// <param name="strLaunch">Game's launch command</param>
			/// <param name="strIconPath">Path to game's icon</param>
			/// <param name="strUninstall">Path to game's uninstaller</param>
			/// <param name="bIsInstalled">Flag indicating if the game is installed</param>
			/// <param name="bIsFavourite">Flag indicating if the game is in the favourite tab</param>
			/// <param name="bIsNew">Flag indicating the game was just found in the latest scan</param>
			/// <param name="bIsHidden">Flag indicating the game is hidden</param>
			/// <param name="strAlias">Alias for command-line or insert mode, user-definable, defaults to exe name</param>
			/// <param name="platformEnum">Game's platform enumerator</param>
			/// <param name="fOccurCount">Game's frequency counter</param>
			public CGameInstance(string strID, string strTitle, string strLaunch, string strIconPath, string strUninstall, bool bIsInstalled, bool bIsFavourite, bool bIsNew, bool bIsHidden, string strAlias, GamePlatform platformEnum, double fOccurCount)
				: base(strID, strTitle, strLaunch, strIconPath, strUninstall, bIsInstalled, bIsFavourite, bIsNew, bIsHidden, strAlias, platformEnum, fOccurCount)
			{

			}
		}

		/// <summary>
		/// Child class of HashSet which stores CGame objects.
		/// Designed to be used to temporarily store the game objects
		/// </summary>
		public class CTempGameSet : HashSet<CGame>
		{
			/// <summary>
			/// Default constructor
			/// </summary>
			public CTempGameSet()
			{

			}

			/// <summary>
			/// Instert CGame object into the HashSet
			/// </summary>
			/// <param name="strID">Game unique ID</param>
			/// <param name="strTitle">Game title</param>
			/// <param name="strLaunch">Game launch command</param>
			/// <param name="strIconPath">Path to game's icon</param>
			/// <param name="strUninstall">Path to game's uninstaller</param>
			/// <param name="bIsInstalled">Flag indicating if the game is installed</param>
			/// <param name="bIsFavourite">Flag indicating if the game is in the favourite tab</param>
			/// <param name="bIsNew">Flag indicating the game was just found in the latest scan</param>
			/// <param name="bIsHidden">Flag indicating the game is hidden</param>
			/// <param name="strAlias">Alias for command-line or insert mode, user-definable, defaults to exe name</param>
			/// <param name="strPlatform">Game platform string</param>
			/// <param name="fOccurCount">Game's frequency counter</param>
			public void InsertGame(string strID, string strTitle, string strLaunch, string strIconPath, string strUninstall, bool bIsInstalled, bool bIsFavourite, bool bIsNew, bool bIsHidden, string strAlias, string strPlatform, double fOccurCount)
			{
				GamePlatform platformEnum;
				// If platform is incorrect or unsupported, default to unknown.
				//if (!Enum.TryParse(strPlatform, true, out GamePlatform platformEnum))
				platformEnum = (GamePlatform)GetPlatformEnum(strPlatform);
				if (platformEnum < 0)
					platformEnum = GamePlatform.Unknown;

				this.Add(CreateGameInstance(strID, strTitle, strLaunch, strIconPath, strUninstall, bIsInstalled, bIsFavourite, bIsNew, bIsHidden, strAlias, platformEnum, fOccurCount));
			}
		}

		/// <summary>
		/// Create a new CGame instance
		/// </summary>
		/// <param name="strID">Unique ID of the game</param>
		/// <param name="strTitle">Title of the game</param>
		/// <param name="strLaunch">Game's launch command</param>
		/// <param name="strIconPath">Path to game's icon</param>
		/// <param name="strUninstall">Path to game's uninstaller</param>
		/// <param name="bIsInstalled">Flag indicating if the game is installed</param>
		/// <param name="bIsFavourite">Flag indicating if the game is in the favourite tab</param>
		/// <param name="bIsNew">Flag indicating the game was just found in the latest scan</param>
		/// <param name="bIsHidden">Flag indicating the game is hidden</param>
		/// <param name="strAlias">Alias for command-line or insert mode, user-definable, defaults to exe name</param>
		/// <param name="platformEnum">Game's platform enumerator</param>
		/// <param name="fOccurCount">Game's frequency counter</param>
		/// <returns>Instance of CGame</returns>
		private static CGame CreateGameInstance(string strID, string strTitle, string strLaunch, string strIconPath, string strUninstall, bool bIsInstalled, bool bIsFavourite, bool bIsNew, bool bIsHidden, string strAlias, GamePlatform platformEnum, double fOccurCount)
		{
			return new CGameInstance(strID, strTitle, strLaunch, strIconPath, strUninstall, bIsInstalled, bIsFavourite, bIsNew, bIsHidden, strAlias, platformEnum, fOccurCount);
		}

		private static readonly Dictionary<GamePlatform, HashSet<CGame>> m_gameDictionary = new Dictionary<GamePlatform, HashSet<CGame>>();
		private static HashSet<CGame> m_searchResults = new HashSet<CGame>();
		private static HashSet<CGame> m_favourites = new HashSet<CGame>();
		private static HashSet<CGame> m_newGames = new HashSet<CGame>();
		private static HashSet<CGame> m_allGames = new HashSet<CGame>();
		private static HashSet<CGame> m_hidden = new HashSet<CGame>();
		private static HashSet<CGame> m_notInstalled = new HashSet<CGame>();

		/// <summary>
		/// Return the list of Game objects with specified platform
		/// </summary>
		/// <param name="platformEnum">Platform enumerator</param>
		/// <returns>List of Game objects</returns>
		public static HashSet<CGame> GetPlatformGameList(GamePlatform platformEnum)
		{
			if (platformEnum == GamePlatform.Search)
				return m_searchResults;

			else if (platformEnum == GamePlatform.Favourites)
				return m_favourites;

			else if (platformEnum == GamePlatform.New)
				return m_newGames;

			else if (platformEnum == GamePlatform.All)
				return m_allGames;

			else if (platformEnum == GamePlatform.Hidden)
				return m_hidden;

			else if (platformEnum == GamePlatform.NotInstalled)
				return m_notInstalled;

			else
				return m_gameDictionary[platformEnum];
		}

		/// <summary>
		/// Remove all games from memory
		/// </summary>
		/// <param name="bRemoveCustom">If true, will also remove manually added games</param>
		public static void ClearGames(bool bRemoveCustom)
		{
			if (bRemoveCustom)
				m_gameDictionary.Clear();

			else
			{
				foreach (KeyValuePair<GamePlatform, HashSet<CGame>> gameSet in m_gameDictionary)
				{
					if (gameSet.Key != GamePlatform.Custom)
						gameSet.Value.Clear();
				}
			}

			m_allGames.Clear();
		}

		/// <summary>
		/// Return platform enum description
		/// </summary>
		/// <param name="value">Enum to match to string</param>
		public static string GetPlatformString(int value)
		{
			if (value > Enum.GetNames(typeof(GamePlatform)).Length)
				return "";

			return GetPlatformString((GamePlatform)value);
		}

		/// <summary>
		/// Return platform enum description
		/// </summary>
		/// <param name="value">Enum to match to string</param>
		public static string GetPlatformString(GamePlatform value)
		{
			return GetDescription<GamePlatform>(value);
			/*
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				FieldInfo field = type.GetField(name);
				if (field != null && Attribute.GetCustomAttribute(field,
					typeof(DescriptionAttribute)) is DescriptionAttribute attr)
				{
					return attr.Description;
				}
				return name;
			}
			return "";
			*/
		}

		/// <summary>
		/// Resolve platform enum from a string
		/// </summary>
		/// <param name="strPlatformName">Platform as a string input</param>
		/// <returns>GamePlatform enumerator, cast to int type. -1 on failed resolution</returns>
		public static int GetPlatformEnum(string strPlatformName)
		{
			Type type = typeof(GamePlatform);
			try
			{
				foreach (GamePlatform value in Enum.GetValues(type))
				{
					FieldInfo field = type.GetField(value.ToString());
					if (field != null && Attribute.GetCustomAttribute(field,
						typeof(DescriptionAttribute)) is DescriptionAttribute attr &&
						attr.Description.Equals(strPlatformName))
					{
						return (int)value;
					}
				}
			}
			catch (Exception e)
			{
				CLogger.LogError(e);
			}
			if (Enum.TryParse(strPlatformName, true, out GamePlatform platformEnum))
				return (int)platformEnum;
			return -1;
		}

		/// <summary>
		/// Resolve platform enum from a string
		/// </summary>
		/// <param name="strPlatformName">Platform as a string input</param>
		/// <param name="bStripStr">Whether to strip out the colon and number portion</param>
		/// <returns>GamePlatform enumerator, cast to int type. -1 on failed resolution</returns>
		public static int GetPlatformEnum(string strPlatformName, bool bStripStr)
		{
			if (bStripStr) strPlatformName = strPlatformName.Contains(":") ? strPlatformName.Substring(0, strPlatformName.IndexOf(':')) : strPlatformName;
			return GetPlatformEnum(strPlatformName);
		}

		/// <summary>
		/// Returns all platforms and the number of games per platform
		/// </summary>
		/// <returns>Dictionary of strings and counts</returns>
		public static Dictionary<string, int> GetPlatforms()
		{
			Dictionary<string, int> platformDict = new Dictionary<string, int>
			{
				{ GetPlatformString(GamePlatform.Search), m_searchResults.Count },
				{ GetPlatformString(GamePlatform.Favourites), m_favourites.Count },
				{ GetPlatformString(GamePlatform.New), m_newGames.Count },
				{ GetPlatformString(GamePlatform.All), m_allGames.Count },
				{ GetPlatformString(GamePlatform.Hidden), m_hidden.Count },
				{ GetPlatformString(GamePlatform.NotInstalled), m_notInstalled.Count },
			};

			foreach (KeyValuePair<GamePlatform, HashSet<CGame>> platform in m_gameDictionary)
			{
				platformDict.Add(GetPlatformString(platform.Key), platform.Value.Count);
			}

			return platformDict;
		}

		/// <summary>
		/// Return titles of games for specified platform
		/// </summary>
		/// <param name="platformEnum">Platform enumerator</param>
		/// <returns>List of strings</returns>
		public static List<string> GetPlatformTitles(GamePlatform platformEnum)
		{
			List<string> platformTitles = new List<string>();

			if (platformEnum == GamePlatform.Search)
			{
				foreach (CGame game in m_searchResults)
				{
					string strTitle = game.Title;
					if (game.IsFavourite)
						strTitle += " [F]";
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					if (!(game.IsHidden))
						platformTitles.Add(strTitle);
				}
			}
			else if (platformEnum == GamePlatform.Favourites)
			{
				foreach (CGame game in m_favourites)
				{
					string strTitle = game.Title;
					if (game.IsHidden)  // If a game is faved *and* hidden, hide from platform lists but still show it in favourites
						strTitle += " [H]";
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					platformTitles.Add(strTitle);
				}
			}
			else if (platformEnum == GamePlatform.New)
			{
				foreach (CGame game in m_newGames)
				{
					string strTitle = game.Title;
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					platformTitles.Add(strTitle);
				}
			}
			else if (platformEnum == GamePlatform.All)
			{
				foreach (CGame game in m_allGames)
				{
					string strTitle = game.Title;
					if (game.IsFavourite)
						strTitle += " [F]";
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					if (!(game.IsHidden))
						platformTitles.Add(strTitle);
				}
			}
			else if (platformEnum == GamePlatform.Hidden)
			{
				foreach (CGame game in m_hidden)
				{
					string strTitle = game.Title;
					if (game.IsFavourite)
						strTitle += " [F]";
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					platformTitles.Add(strTitle);
				}
			}
			else if (platformEnum == GamePlatform.NotInstalled)
			{
				foreach (CGame game in m_notInstalled)
				{
					string strTitle = game.Title;
					if (game.IsFavourite)
						strTitle += " [F]";
					if (!(game.IsHidden))
						platformTitles.Add(strTitle);
				}
			}
			else if (m_gameDictionary.ContainsKey(platformEnum))
			{
				foreach (CGame game in m_gameDictionary[platformEnum])
				{
					string strTitle = game.Title;
					if (game.IsFavourite)
						strTitle += " [F]";
					if (!(game.IsInstalled))
						strTitle = "*" + strTitle;
					if (!(game.IsHidden))
						platformTitles.Add(strTitle);
				}
			}

			return platformTitles;
		}

		/// <summary>
		/// Add game to the dictionary
		/// </summary>
		/// <param name="strID">Unique ID of the game</param>
		/// <param name="strTitle">Title of the game</param>
		/// <param name="strLaunch">Game's launch command</param>
		/// <param name="strIconPath">Path to game's icon</param>
		/// <param name="strUninstall">Path to game's uninstaller</param>
		/// <param name="bIsInstalled">Flag indicating if the game is installed</param>
		/// <param name="bIsFavourite">Flag indicating if the game is in the favourite tab</param>
		/// <param name="bIsNew">Flag indicating the game was just found in the latest scan</param>
		/// <param name="bIsHidden">Flag indicating the game is hidden</param>
		/// <param name="strAlias">Alias for command-line or insert mode, user-definable, defaults to exe name</param>
		/// <param name="strPlatform">Game's platform as a string value</param>
		/// <param name="fOccurCount">Game's frequency counter</param>
		public static void AddGame(string strID, string strTitle, string strLaunch, string strIconPath, string strUninstall, bool bIsInstalled, bool bIsFavourite, bool bIsNew, bool bIsHidden, string strAlias, string strPlatform, double fOccurCount)
		{
			GamePlatform platformEnum;
			// If platform is incorrect or unsupported, default to unknown.
			//if (!Enum.TryParse(strPlatform, true, out GamePlatform platformEnum))
			platformEnum = (GamePlatform)GetPlatformEnum(strPlatform);
			if (platformEnum < 0)
				platformEnum = GamePlatform.Unknown;

			// If this is the first entry in the key, we need to initialise the list
			if (!m_gameDictionary.ContainsKey(platformEnum))
				m_gameDictionary[platformEnum] = new HashSet<CGame>();

			CGame game = CreateGameInstance(strID, strTitle, strLaunch, strIconPath, strUninstall, bIsInstalled, bIsFavourite, bIsNew, bIsHidden, strAlias, platformEnum, fOccurCount);
			m_gameDictionary[platformEnum].Add(game);

			if (game.IsFavourite)
				m_favourites.Add(game);

			if (game.IsNew)
				m_newGames.Add(game);

			m_allGames.Add(game);

			if (game.IsHidden)
				m_hidden.Add(game);

			if (!(game.IsInstalled))
				m_notInstalled.Add(game);
		}

		/// <summary>
		/// Function overload
		/// Add game object from all game set to the dictionary
		/// </summary>
		/// <param name="game">instance of CGame to add</param>
		private static void AddGame(CGame game)
		{
			if (game != null)
			{
				if (!m_gameDictionary.ContainsKey(game.Platform))
					m_gameDictionary[game.Platform] = new HashSet<CGame>();

				m_gameDictionary[game.Platform].Add(game);

				if (game.IsFavourite)
					m_favourites.Add(game);

				if (game.IsNew)
					m_newGames.Add(game);

				if (game.IsHidden)
					m_hidden.Add(game);

				if (!(game.IsInstalled))
					m_notInstalled.Add(game);
			}
		}

		/// <summary>
		/// Return game object for the specified platform
		/// </summary>
		/// <param name="platformEnum">Game's platform index</param>
		/// <param name="nGameIndex">Index of the game list</param>
		/// <returns>Instance of CGame</returns>
		public static CGame GetPlatformGame(GamePlatform platformEnum, int nGameIndex)
		{
			if (nGameIndex > -1)
			{
				try
				{
					if (platformEnum == GamePlatform.Search)
						return m_searchResults.ElementAt(nGameIndex);
					else if (platformEnum == GamePlatform.Favourites)
						return m_favourites.ElementAt(nGameIndex);
					else if (platformEnum == GamePlatform.New)
						return m_newGames.ElementAt(nGameIndex);
					else if (platformEnum == GamePlatform.All)
						return m_allGames.ElementAt(nGameIndex);
					else if (platformEnum == GamePlatform.Hidden)
						return m_hidden.ElementAt(nGameIndex);
					else if (platformEnum == GamePlatform.NotInstalled)
						return m_notInstalled.ElementAt(nGameIndex);
					else
						return m_gameDictionary[platformEnum].ElementAt(nGameIndex);
				}
				catch (Exception e)
				{
					CLogger.LogError(e);
				}
			}
			return null;
		}

		/// <summary>
		/// Toggle the specified game's favourite flag
		/// </summary>
		/// <param name="platformEnum">Game's platform enumerator</param>
		/// <param name="nGameIndex">Index of the game list</param>
		public static void ToggleFavourite(GamePlatform platformEnum, int nGameIndex, bool alphaSort, bool faveSort, bool instSort, bool ignoreArticle)
		{
			if (nGameIndex > -1)
			{
				CGame gameCopy;
				if (platformEnum == GamePlatform.Search)
					gameCopy = m_searchResults.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.Favourites)
					gameCopy = m_favourites.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.New)
					gameCopy = m_newGames.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.All)
					gameCopy = m_allGames.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.Hidden)
					gameCopy = m_hidden.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.NotInstalled)
					gameCopy = m_notInstalled.ElementAt(nGameIndex);
				else
					gameCopy = m_gameDictionary[platformEnum].ElementAt(nGameIndex);

				if (gameCopy.IsFavourite)
				{
					gameCopy.IsFavourite = false;
					m_favourites.Remove(gameCopy);
				}
				else
				{
					gameCopy.IsFavourite = true;
					m_favourites.Add(gameCopy);
					SortGameSet(ref m_favourites, alphaSort, faveSort, instSort, ignoreArticle);
				}
			}
		}

		/// <summary>
		/// Toggle the specified game's hidden flag
		/// </summary>
		/// <param name="platformEnum">Game's platform enumerator</param>
		/// <param name="nGameIndex">Index of the game list</param>
		public static void ToggleHidden(GamePlatform platformEnum, int nGameIndex, bool alphaSort, bool faveSort, bool instSort, bool ignoreArticle)
		{
			if (nGameIndex > -1)
			{
				CGame gameCopy;
				if (platformEnum == GamePlatform.Search)
					gameCopy = m_searchResults.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.Favourites)
					gameCopy = m_favourites.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.New)
					gameCopy = m_newGames.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.All)
					gameCopy = m_allGames.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.Hidden)
					gameCopy = m_hidden.ElementAt(nGameIndex);
				else if (platformEnum == GamePlatform.NotInstalled)
					gameCopy = m_notInstalled.ElementAt(nGameIndex);
				else
					gameCopy = m_gameDictionary[platformEnum].ElementAt(nGameIndex);

				if (gameCopy.IsHidden)
				{
					gameCopy.IsHidden = false;
					m_hidden.Remove(gameCopy);
				}
				else
				{
					gameCopy.IsHidden = true;
					m_hidden.Add(gameCopy);
					SortGameSet(ref m_hidden, alphaSort, faveSort, instSort, ignoreArticle);
				}
			}
		}

		/// <summary>
		/// Remove selected game from memory
		/// </summary>
		/// <param name="game">Game object to remove</param>
		public static void RemoveGame(CGame game)
		{
			if (game != null)
			{
				if (game.IsFavourite)
					m_favourites.Remove(game);
				if (game.IsNew)
					m_newGames.Remove(game);
				m_allGames.Remove(game);
				if (game.IsHidden)
					m_hidden.Remove(game);
				if (!(game.IsInstalled))
					m_notInstalled.Remove(game);
				m_gameDictionary[game.Platform].Remove(game);
			}
		}

		public static void ClearNewGames()
        {
			m_newGames.Clear();
			foreach (CGame game in m_allGames)
			{
				game.IsNew = false;
			}
		}

		/// <summary>
		/// Merge the temporary game set with the main game set.
		/// Add new games to the main set and remove missing games from the main set.
		/// </summary>
		/// <param name="tempGameSet">Instance of CTempGameSet containing new games</param>
		public static void MergeGameSets(CTempGameSet tempGameSet)
		{
			m_gameDictionary.Clear();
			m_favourites.Clear();
			m_newGames.Clear();
			m_hidden.Clear();
			m_notInstalled.Clear();

			// Find games that are missing from tempGameSet and remove them from m_allGames
			// Find games that are missing from m_allGames and add them to m_allGames

			HashSet<CGame> newGames = new HashSet<CGame>(tempGameSet);
			newGames.ExceptWith(m_allGames);

			HashSet<CGame> gamesToRemove = new HashSet<CGame>(m_allGames);
			gamesToRemove.ExceptWith(tempGameSet);

			foreach (CGame game in gamesToRemove)
			{
				m_allGames.Remove(game);
			}

			foreach (CGame game in newGames)
			{
				m_newGames.Add(game);
				m_allGames.Add(game);
			}

			foreach (CGame game in m_allGames)
			{
				AddGame(game);
			}
		}

		/// <summary>
		/// Decrease the frequecny counter for all games by 10%
		/// Increment the selected game's frequency counter 
		/// </summary>
		/// <param name="selectedGame">CGame object that will be incremented</param>
		public static void NormaliseFrequencies(CGame selectedGame)
		{
			foreach (CGame game in m_allGames)
			{
				game.DecimateFrequency();

				if (game == selectedGame)
					game.IncrementFrequency();
			}
		}

		/// <summary>
		/// Sort all game containers by the game frequency counters
		/// </summary>
		public static void SortGames(bool alphaSort, bool faveSort, bool instSort, bool ignoreArticle)
		{
			for (int i = 0; i < m_gameDictionary.Count; i++)
			{
				var pair = m_gameDictionary.ElementAt(i);
				HashSet<CGame> temp = pair.Value;
				SortGameSet(ref temp, alphaSort, faveSort, instSort, ignoreArticle);
				m_gameDictionary[pair.Key] = temp;
			}
			SortGameSet(ref m_favourites, alphaSort, false, instSort, ignoreArticle);
			SortGameSet(ref m_newGames, alphaSort, faveSort, instSort, ignoreArticle);
			SortGameSet(ref m_allGames, alphaSort, faveSort, instSort, ignoreArticle);
			SortGameSet(ref m_hidden, alphaSort, faveSort, instSort, ignoreArticle);
			SortGameSet(ref m_notInstalled, alphaSort, faveSort, false, ignoreArticle);
		}

		/// <summary>
		/// Sort a game set by the game frequency counters
		/// </summary>
		/// <param name="gameSet">Set of games</param>
		private static void SortGameSet(ref HashSet<CGame> gameSet, bool alphaSort, bool faveSort, bool instSort, bool ignoreArticle)
		{
			if (ignoreArticle)
				articles = new List<string>() { };

			IOrderedEnumerable<CGame> tempSet;
			if (faveSort)
			{
				if (alphaSort)
				{
					if (instSort)
						tempSet = gameSet.OrderByDescending(games => games.IsInstalled).ThenBy(games => games.IsFavourite).ThenBy(games =>
							string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
					else
						tempSet = gameSet.OrderByDescending(games => games.IsFavourite).ThenBy(games =>
							string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
				}
				else if (instSort)
					tempSet = gameSet.OrderByDescending(games => games.IsInstalled).ThenBy(games => games.IsFavourite).ThenByDescending(games =>
						games.Frequency).ThenBy(games => string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
				else
					tempSet = gameSet.OrderByDescending(games => games.IsFavourite).ThenByDescending(games => games.Frequency).ThenBy(games =>
						string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
			}
			else
			{
				if (alphaSort)
				{
					if (instSort)
						tempSet = gameSet.OrderByDescending(games => games.IsInstalled).ThenBy(games =>
							string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
					else
						tempSet = gameSet.OrderByDescending(games =>
							string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
				}
				else if (instSort)
					tempSet = gameSet.OrderByDescending(games => games.IsInstalled).ThenByDescending(games => games.Frequency).ThenBy(games =>
						string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
				else
					tempSet = gameSet.OrderByDescending(games => games.Frequency).ThenBy(games =>
						string.Join(" ", games.Title.Split(' ').SkipWhile(s => articles.Any(x => Equals(s, CDock.IGNORE_ALL)))));
			}
			gameSet = tempSet.ToHashSet();
		}

		/// <summary>
		/// Get enum description
		/// </summary>
		/// <returns>description string</returns>
		/// <param name="enum">Enum</param>
		public static string GetDescription<T>(this T source)
		{
			try
			{
				FieldInfo field = source.GetType().GetField(source.ToString());

				DescriptionAttribute[] attr = (DescriptionAttribute[])field.GetCustomAttributes(
					typeof(DescriptionAttribute), false);

				if (attr != null && attr.Length > 0) return attr[0].Description;
			}
			catch (Exception e)
			{
				CLogger.LogError(e);
			}
			Type type = source.GetType();
			string output = type.GetEnumName(source);
			if (!string.IsNullOrEmpty(output))
				return output;
			return source.ToString();
		}

		/// <summary>
		/// Return set of games from a fuzzy match
		/// </summary>
		/// <returns>dictionary of titles with confidence levels</returns>
		/// <param name="match">String to match</param>
		public static Dictionary<string, int> FindMatchingTitles(string match)
		{
			return FindMatchingTitles(match, 0);
		}

		/// <summary>
		/// Return set of game titles from a fuzzy match
		/// </summary>
		/// <returns>dictionary of titles with confidence level</returns>
		/// <param name="match">String to match</param>
		public static Dictionary<string, int> FindMatchingTitles(string match, int max)
		{
			Dictionary<string, int> outDict = new Dictionary<string, int>();
			int i = 0;
			m_searchResults.Clear();
			match = match.ToLower();
			foreach (CGame game in m_allGames)
			{
				string fullTitle = game.Title.StartsWith("*") ? game.Title.Substring(1) : game.Title;
				fullTitle = fullTitle.EndsWith(" [F]") || fullTitle.EndsWith(" [H]") ? fullTitle.ToLower().Substring(0, fullTitle.Length - 4) : fullTitle.ToLower();
				string shortTitle = fullTitle;
				/*
				foreach (string prep in new List<string> { "for", "of", "to" })
				{
					if (shortTitle.StartsWith(prep + " "))
						shortTitle = shortTitle.Substring(prep.Length + 1);
				}
				*/
				foreach (string art in articles)
				{
					if (shortTitle.StartsWith(art + " "))
						shortTitle = shortTitle.Substring(art.Length + 1);
				}

				if (fullTitle.Equals(match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.ExactTitle);     // full confidence
					if (max > 0 && i >= max) break;
				}
				else if (game.Alias.Equals(match) ||
					shortTitle.Equals(match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.ExactAlias);     // very high confidence
					if (max > 0 && i >= max) break;
				}
				else if (shortTitle.StartsWith(match) ||
					fullTitle.StartsWith(match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.BeginTitle);     // medium confidence
					if (max > 0 && i >= max) break;
				}
				else if (game.Alias.StartsWith(match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.BeginAlias);     // medium confidence
					if (max > 0 && i >= max) break;
				}
				else if ((fullTitle.Contains("- ") &&
					fullTitle.Substring(fullTitle.IndexOf("- ") + 2).StartsWith(match)) ||
					(fullTitle.Contains(": ") &&
					fullTitle.Substring(fullTitle.IndexOf(": ") + 2).StartsWith(match)))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.BeginSubtitle);  // low confidence
					if (max > 0 && i >= max) break;
				}
				else if (fullTitle.Contains(" ") &&
					fullTitle.Substring(fullTitle.LastIndexOf(' ')).StartsWith(match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.BeginLastWord);  // low confidence
					if (max > 0 && i >= max) break;
				}
				else if (fullTitle.Contains(" ") &&
					fullTitle.Contains(" " + match))
				{
					i++;
					m_searchResults.Add(game);
					outDict.Add(game.Title, (int)Match.BeginAnyWord);   // low confidence
					if (max > 0 && i >= max) break;
				}
			}
			return outDict;
		}

		/// <summary>
		/// Return set of CGames from a fuzzy match
		/// </summary>
		/// <param name="match">String to match</param>
		/// <returns>Hashset of CGames</returns>
		public static HashSet<CGame> MatchGame(string match)
		{
			HashSet<CGame> outSet = new HashSet<CGame>();
			match = match.ToLower();
			foreach (CGame game in m_allGames)
			{
				string fullTitle = game.Title.StartsWith("*") ? game.Title.Substring(1) : game.Title;
				fullTitle = fullTitle.EndsWith(" [F]") || fullTitle.EndsWith(" [H]") ? fullTitle.ToLower().Substring(0, fullTitle.Length - 4) : fullTitle.ToLower();
				string shortTitle = fullTitle;
				/*
				foreach (string prep in new List<string> { "for", "of", "to" })
				{
					if (shortTitle.StartsWith(prep + " "))
						shortTitle = shortTitle.Substring(prep.Length + 1);
				}
				*/
				foreach (string art in articles)
				{
					if (shortTitle.StartsWith(art + " "))
						shortTitle = shortTitle.Substring(art.Length + 1);
				}
				if (game.Alias.StartsWith(match) ||
					shortTitle.StartsWith(match) ||
					fullTitle.StartsWith(match) ||
					(fullTitle.Contains(" ") &&
						fullTitle.Substring(fullTitle.LastIndexOf(' ')).StartsWith(match)))  // match last word
				{
					outSet.Add(game);
				}
			}
			return outSet;
		}

		/// <summary>
		/// Contains information about matches from a game search
		/// </summary>
		public struct CMatch
		{
			public string m_strTitle;
			public int m_nIndex;
			public int m_nPercent;

			public CMatch(string strTitle, int nIndex, int nPercent)
			{
				m_strTitle = strTitle;
				m_nIndex = nIndex;
				m_nPercent = nPercent;
			}
		}
	}
}