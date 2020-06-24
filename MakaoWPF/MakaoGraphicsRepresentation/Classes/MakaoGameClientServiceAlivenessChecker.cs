using MakaoGameClientService.ServiceImplementations;
using System.Threading;
using System.Threading.Tasks;

namespace MakaoGraphicsRepresentation
{
    class MakaoGameClientServiceAlivenessChecker
    {
        #region Fields and Poperties

        //Synchronization context
        private readonly SynchronizationContext SynchCont;

        //flag that indicates if player can join the game (client service started)
        private bool canJoinTheGame;
        public bool CanJoinTheGame { get { return canJoinTheGame; } }

        #endregion

        #region Constructor

        public MakaoGameClientServiceAlivenessChecker()
        {
            canJoinTheGame = false;
            SynchCont = SynchronizationContext.Current;
            CheckClientService();
        }

        #endregion

        #region Checking if service is running

        //method wich starts the algorithm in another task
        private void CheckClientService()
        {
            Task.Run(() => CheckIfClientIsAlive());
        }

        private void CheckIfClientIsAlive()
        {
            bool response = EngineClientHandler.CheckIfServiceIsAlive();
            SynchCont.Post(_ => CheckIfClientIsAliveResponseHandler(response), null);
        }

        private void CheckIfClientIsAliveResponseHandler(bool response)
        {
            if (!response)
            {
                canJoinTheGame = false;
            }
            else
            {
                canJoinTheGame = true;
            }

            OnResponseReceived(canJoinTheGame);
        }

        #endregion

        #region Got the response event

        public delegate void GotTheResponseEventHandler(object sender, ServiceRunningEventArgs e);

        public event GotTheResponseEventHandler ResponseReceived;

        protected virtual void OnResponseReceived(bool response)
        {
            ResponseReceived?.Invoke(this, new ServiceRunningEventArgs { IsRunning = response });
        }

        #endregion
    }
}
