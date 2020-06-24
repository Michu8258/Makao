using CardGraphicsLibraryHandler;
using CardsRepresentation;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    class RealmUserHandler
    {
        #region Fields and constructor

        private readonly Realm realm;

        public RealmUserHandler()
        {
            realm = realm = Realm.GetInstance(System.Reflection.Assembly.GetExecutingAssembly().
            Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.Length - 31) + @"MakaoUsers.realm");
        }

        #endregion

        #region Checking if login is free to use

        public bool CheckIfLoginIsAvailable(string login)
        {
            List<PlayerDefinition> playerDefList = realm.All<PlayerDefinition>().ToList();
            if (playerDefList.Count == 0)
            {
                return true;
            }
            else
            {
                try
                {
                    PlayerDefinition onePlayerDef = playerDefList.Where(x => x.PlayerLogin == login).First();
                    return false;
                }
                catch (Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error($"Error while trying to find player with login {login} in RealmDB: {ex.Message}.");
                    return true;
                }
            }
        }

        #endregion

        #region Adding new player to DB

        public int AddNewPlayerToDB(string PlayerName, string PlayerPassword, string PlayerLogin)
        {
            int readOutputID;

            if (PlayerName.Length <= 4 || PlayerName == "User")
                throw new ArgumentException("Player name has to be longer than 4 characters and must not be equal to: User.");
            if (PlayerLogin.Length <= 4 || PlayerLogin == "User")
                throw new ArgumentException("Player login has to be longer than 4 characters and must not be equal to: User.");
            if (PlayerPassword.Length <= 7)
                throw new ArgumentException("Player password must be longer than 7 characters");

            if (!CheckIfLoginIsAvailable(PlayerLogin))
                throw new ArgumentOutOfRangeException($"Player with login: {PlayerLogin} is already defined. Choose another login");

            int newID = GetMaxDatabaseID() + 1;
            readOutputID = AddNewPlayerDefinition(newID, PlayerName, PlayerPassword, PlayerLogin);
            AddDefaultPlayerData(readOutputID);
            return readOutputID;
        }

        private int GetMaxDatabaseID()
        {
            int maxID = 0;
            var playerDefinitionList = realm.All<PlayerDefinition>().ToList();
            if (playerDefinitionList.Count > 0)
            {
                maxID = playerDefinitionList.Max(x => x.ID);
            }
            return maxID;
        }

        private int AddNewPlayerDefinition(int newID, string playerName, string playerPassword, string playerLogin)
        {
            PasswordEncryptor encryptor = new PasswordEncryptor();

            realm.Write(() =>
            {
                realm.Add(new PlayerDefinition()
                {
                    ID = newID,
                    PlayerName = playerName,
                    PlayerPassword = encryptor.EnctyptPassword(playerPassword),
                    PlayerLogin = playerLogin,
                });
            });

            return newID;
        }

        private void AddDefaultPlayerData(int inputID)
        {
            CardBackColorConverter colorConverter = new CardBackColorConverter();
            ThirdPlayerLocationConverter locationConverter = new ThirdPlayerLocationConverter();

            realm.Write(() =>
            {
                realm.Add(new PlayerData()
                {
                    ID = inputID,
                    AvatarName = "/Avatars/01.png",
                    AmountOfPlayers = 2,
                    AmountOfDecks = 1,
                    AmountOfJokers = 3,
                    AmountOfStartCards = 5,
                    PlayedGames = 0,
                    CardBack = colorConverter.ConvertToNumber(BackColor.Blue),
                    ReadinesTimeoutEnabled = false,
                    WaitingForReadinessTimeout = 5,
                    JoiningTimeoutEnabled = false,
                    WaitingForJoiningTimeout = 5,
                    PlayedAndWonGames = 0,
                    LocationOfThirdPlayer = locationConverter.ConvertToNumber(ThirdPlayerLocation.Left),
                });
            });
        }

        #endregion

        #region Loging validation

        public (SettingsData, int) Login(string login, string password)
        {
            bool loginExists = CheckIfLoginIsAvailable(login);
            if(!loginExists)
            {
                int playerID = GetPlayerID(login);
                if(playerID > 0)
                {
                    string hashedPassword = GetHashedPassword(playerID);
                    if (hashedPassword.Length > 0)
                    {
                        bool passwordIsCorrect = CheckPasswordCorrectness(password, hashedPassword);
                        if (passwordIsCorrect)
                        {
                            return (ConvertToSettingsData(GetPlayerData(playerID), GetPlayerName(playerID)), playerID);
                        }
                    }
                }
            }
            return (null, 0);
        }

        private int GetPlayerID(string login)
        {
            int playerID = 0;
            try
            {
                playerID = realm.All<PlayerDefinition>().ToList().Single(x => x.PlayerLogin == login).ID;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to obtain player ID: {ex.Message}.");
            }
            return playerID;
        }

        private string GetPlayerName(int playerID)
        {
            string playerName = "";
            try
            {
                playerName = realm.All<PlayerDefinition>().ToList().Single(x => x.ID == playerID).PlayerName;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to obtain player ID: {ex.Message}.");
            }
            return playerName;
        }

        private string GetHashedPassword(int playerID)
        {
            string playerHashedPassword = "";
            try
            {
                playerHashedPassword = realm.All<PlayerDefinition>().ToList().Single(x => x.ID == playerID).PlayerPassword;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to obtain player hashed password: {ex.Message}.");
            }
            return playerHashedPassword;
        }

        private bool CheckPasswordCorrectness(string password, string hashedPassword)
        {
            PasswordEncryptor decryptor = new PasswordEncryptor();
            string decryptedPassword = "";
            try
            {
                decryptedPassword = decryptor.DecryptPassword(hashedPassword, password);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying decrypt password: {ex.Message}.");
            }
            if (decryptedPassword == password) return true;
            else return false;
        }

        private PlayerData GetPlayerData(int playerID)
        {
            PlayerData output = null;

            try
            {
                output = realm.All<PlayerData>().ToList().Single(x => x.ID == playerID);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to obtain data: {ex.Message}.");
            }
            return output;
        }

        #endregion

        #region Updating player information

        public bool UpdatePlayerName(int playerID, string playerName)
        {
            try
            {
                PlayerDefinition def = realm.All<PlayerDefinition>().ToList().Single(x => x.ID == playerID);
                
                using (var trans = realm.BeginWrite())
                {
                    def.PlayerName = playerName;
                    trans.Commit();
                }
                return true;
            }
            catch(Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while updating player name: {ex.Message}.");
                return false;
            }
        }

        public bool UpdatePlayerData(SettingsData data, int playerID)
        {
            PlayerData newData = ConvertToPlayerData(data, playerID);
            try
            {
                PlayerData playerData = realm.All<PlayerData>().ToList().Single(x => x.ID == playerID);

                using (var trans = realm.BeginWrite())
                {
                    playerData.AvatarName = newData.AvatarName;
                    playerData.AmountOfPlayers = newData.AmountOfPlayers;
                    playerData.AmountOfDecks = newData.AmountOfDecks;
                    playerData.AmountOfJokers = newData.AmountOfJokers;
                    playerData.AmountOfStartCards = newData.AmountOfStartCards;
                    playerData.PlayedGames = newData.PlayedGames;
                    playerData.CardBack = newData.CardBack;
                    playerData.ReadinesTimeoutEnabled = newData.ReadinesTimeoutEnabled;
                    playerData.WaitingForReadinessTimeout = newData.WaitingForReadinessTimeout;
                    playerData.JoiningTimeoutEnabled = newData.JoiningTimeoutEnabled;
                    playerData.WaitingForJoiningTimeout = newData.WaitingForJoiningTimeout;
                    playerData.PlayedAndWonGames = newData.PlayedAndWonGames;
                    playerData.LocationOfThirdPlayer = newData.LocationOfThirdPlayer;
                    trans.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while updating player data: {ex.Message}.");
                return false;
            }
        }

        #endregion

        #region Data conversion

        private PlayerData ConvertToPlayerData(SettingsData data, int playerID)
        {
            CardBackColorConverter colorConverter = new CardBackColorConverter();
            ThirdPlayerLocationConverter locationConverter = new ThirdPlayerLocationConverter();

            PlayerData output = new PlayerData()
            {
                ID = playerID,
                AvatarName = data.TypeOfAvatar,
                AmountOfPlayers = data.AmountOfPlayers,
                AmountOfDecks = data.AmountOfDecks,
                AmountOfJokers = data.AmountOfJokers,
                AmountOfStartCards = data.AmountOfStartCards,
                PlayedGames = data.PlayedGames,
                CardBack = colorConverter.ConvertToNumber(data.CardsBackColor),
                ReadinesTimeoutEnabled = data.ReadinessTimeoutEnabled,
                WaitingForReadinessTimeout = data.WaitingForPlayersReadinessTimeout,
                JoiningTimeoutEnabled = data.JoiningTimeoutEnabled,
                WaitingForJoiningTimeout = data.WaitingForPlayersJoiningTimeout,
                PlayedAndWonGames = data.PlayedAndWonGames,
                LocationOfThirdPlayer = locationConverter.ConvertToNumber(data.LocationOfThirdPlayer),
            };

            return output;
        }

        private SettingsData ConvertToSettingsData(PlayerData data, string playerName)
        {
            CardBackColorConverter colorConverter = new CardBackColorConverter();
            ThirdPlayerLocationConverter locationConverter = new ThirdPlayerLocationConverter();

            SettingsData output = new SettingsData()
            {
                UserName = playerName,
                TypeOfAvatar = data.AvatarName,
                AmountOfPlayers = data.AmountOfPlayers,
                AmountOfDecks = data.AmountOfDecks,
                AmountOfJokers = data.AmountOfJokers,
                AmountOfStartCards = data.AmountOfStartCards,
                PlayedGames = data.PlayedGames,
                PlayedAndWonGames = data.PlayedAndWonGames,
                ReadinessTimeoutEnabled = data.ReadinesTimeoutEnabled,
                WaitingForPlayersReadinessTimeout = data.WaitingForReadinessTimeout,
                JoiningTimeoutEnabled = data.JoiningTimeoutEnabled,
                WaitingForPlayersJoiningTimeout = data.WaitingForJoiningTimeout,
                CardsBackColor = colorConverter.ConvertToEnum(data.CardBack),
                LocationOfThirdPlayer = locationConverter.ConvertToEnum(data.LocationOfThirdPlayer),
            };

            return output;
        }

        #endregion
    }
}
