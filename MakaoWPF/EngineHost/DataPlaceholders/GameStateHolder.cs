using MakaoEngine;
using System;
using System.Diagnostics;

namespace EngineHost.DataPlaceholders
{
    static class GameStateHolder
    {
        #region Stored Data

        //instance of the engine
        private static Engine makaoEngineInstance;
        public static Engine EngineInstance { get { return makaoEngineInstance; } }

        //variable that holds state if engine was constructed or disposed
        //false - disposed, true - constructed
        private static bool engineConstructed;
        public static bool EngineConstructed { get { return engineConstructed; } }

        //timer for measuringgame duration and two properties for geting measured time
        private static Stopwatch gameTimer;
        public static TimeSpan GameTimerTimeSpan
        {
            get
            {
                if (gameTimer != null) return gameTimer.Elapsed;
                else return new TimeSpan(0);
            }
        }
        public static long GameTimerTimeSpanMiliseconds
        {
            get
            {
                if (gameTimer != null) return gameTimer.ElapsedMilliseconds;
                else return 0;
            }
        }

        #endregion

        #region Game timer handling

        //method for starting the game timer
        public static void StartGameTimer()
        {
            gameTimer = new Stopwatch();
            gameTimer.Stop();
            gameTimer.Reset();
            gameTimer.Start();
        }

        //method for stoping game timer
        public static TimeSpan StopGameTimer()
        {
            gameTimer.Stop();
            return gameTimer.Elapsed;
        }

        #endregion

        #region Start and dispose engine

        //method for constructiing new instance of engine
        public static bool CreateNewGame(int amountOfPlayers, int amountOfDecks, int amountOfJokers, int amountOfCards = 5)
        {
            makaoEngineInstance = null;
            try
            {
                makaoEngineInstance = new Engine(amountOfPlayers, amountOfDecks, amountOfJokers, false, amountOfCards);
                makaoEngineInstance.CreateGame();
                engineConstructed = true;
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("Start new game engine in GameStateHolder static class successfull");
                return true;
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info($"Start new game engine in GameStateHolder static class failed: {ex.Message}");
                engineConstructed = false;
                return false;
            }
        }

        //disposing an instance of engine.
        public static void Dispose()
        {
            if (makaoEngineInstance != null)
            {
                makaoEngineInstance.Dispose();
                makaoEngineInstance = null;

                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("GameStateHolder class disposed.");
            }
        }

        #endregion
    }
}
