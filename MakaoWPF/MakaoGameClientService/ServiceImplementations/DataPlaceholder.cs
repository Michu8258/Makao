using MakaoGameClientService.Messages;
using System;

namespace MakaoGameClientService.ServiceImplementations
{
    public static class DataPlaceholder
    {
        #region Event fired up when new player joins the room

        public delegate void RefreshListOfPlayersEventHandler(object sender, EventArgs e);
        public static event RefreshListOfPlayersEventHandler RefreshCurrentPlayersList;
        public static void OnRefreshCurrentPlayersList()
        {
            RefreshCurrentPlayersList?.Invoke(null, new EventArgs {});
        }

        #endregion

        #region Event fired up when the room was deleted

        public delegate void DoomDeletedEventHandler(object sender, RoomDeletionReasonsEventArgs e);
        public static event DoomDeletedEventHandler TheRoomWasDeleted;
        public static void OnTheRoomWasDeleted(DeletionReason reason)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Game client got info about room deletion from host. Type: " + reason.ToString());

            TheRoomWasDeleted?.Invoke(null, new RoomDeletionReasonsEventArgs { DeletionReason  = reason});
        }

        #endregion

        #region Event fired up when players readiness to game changed

        public delegate void PlayersReadinessEventHandler(object sender, PlayersReadinessToPlayEventArgs e);
        public static event PlayersReadinessEventHandler PlayersReadinesChanged;
        public static void OnPlayersReadinesChanged(ActualizedPlayersReadinessDataRequest args)
        {
            PlayersReadinesChanged?.Invoke(null, new PlayersReadinessToPlayEventArgs { ReadinessData = args});
        }

        #endregion

        #region Event fired up when host sends data for new game

        public delegate void StartNewGameEventHandler(object sender, OpenNewGameWindowEventArgs e);
        public static event StartNewGameEventHandler NewGameStarted;
        public static void OnNewGameStarted(PersonalizedForSpecificPlayerStartGameDataRequest args)
        {
            NewGameStarted?.Invoke(null, new OpenNewGameWindowEventArgs { ReceivedData = args });
        }

        #endregion

        #region Event fired up when host sends updated data for the game

        public delegate void UpdateTheGameEventHandler(object sender, UpdateGameWindowEventArgs e);
        public static event UpdateTheGameEventHandler UpdteTheGame;
        public static void OnUpdteTheGame(PersonalizedPlayerDataRequest args)
        {
            UpdteTheGame?.Invoke(null, new UpdateGameWindowEventArgs { ReceivedData = args });
        }

        #endregion

        #region Event fired when game ended

        public delegate void GameEndedEventHandler(object sender, GameEndedEventArgs e);
        public static event GameEndedEventHandler GameEnded;
        public static void OnGameEnded(GameFinishedDataRequest args)
        {
            GameEnded?.Invoke(null, new GameEndedEventArgs { GameEndedData = args });
        }

        #endregion
    }

    #region EventArgs for events

    //checking if service is running
    public class ServiceRunningEventArgs : EventArgs
    {
        public bool IsRunning;
    }

    //receiving data about players readiness to play from host
    public class PlayersReadinessToPlayEventArgs : EventArgs
    {
        public ActualizedPlayersReadinessDataRequest ReadinessData;
    }

    //data for sendin reason why room was deleted
    public class RoomDeletionReasonsEventArgs : EventArgs
    {
        public DeletionReason DeletionReason;
    }

    //data received from host while creating new game - opening game window
    public class OpenNewGameWindowEventArgs : EventArgs
    {
        public PersonalizedForSpecificPlayerStartGameDataRequest ReceivedData;
    }

    //data received from host - update of current game state and status
    public class UpdateGameWindowEventArgs : EventArgs
    {
        public PersonalizedPlayerDataRequest ReceivedData;
    }

    //data for game ended event - information from host
    public class GameEndedEventArgs : EventArgs
    {
        public GameFinishedDataRequest GameEndedData;
    }

    #endregion

    #region Enumeration of Room deletion reasons

    public enum DeletionReason
    {
        ClosedByHost,
        JoiningTimeout,
        ReadinessTimeout,
        LostConnection,
        PlayerLeftGame,
    }

    #endregion
}
