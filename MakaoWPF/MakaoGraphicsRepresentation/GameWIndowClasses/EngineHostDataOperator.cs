using CardsRepresentation;
using MakaoGameHostService.Messages;
using MakaoGameHostService.ServiceContracts;
using MakaoGraphicsRepresentation.Windows;
using MakaoInterfaces;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MakaoGraphicsRepresentation.GameWIndowClasses
{
    public class EngineHostDataOperator
    {
        //channel factory for retrieving data from hosts
        private ChannelFactory<IMakaoGameHostService> factory;

        #region Constructing the instance

        public EngineHostDataOperator()
        {
            AssignChannelFactory();
        }

        private void AssignChannelFactory()
        {
            try
            {
                factory = new ChannelFactory<IMakaoGameHostService>(new BasicHttpBinding(),
                new EndpointAddress(MainWindow.MakaoGameHostEndpoint));
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while trying to connect to host service: {ex.Message}.");
            }
        }

        #endregion

        #region Joker changing handling

        public void ChangeJokerIntoAnotherCard(ChangeJokerIntoAnotherCardRequest request,
            MainUser control, SynchronizationContext synchCont)
        {
            Task.Run(() => ChangeJokerIntoCard(request, control, synchCont));
        }

        public void ChangeJokerBackFromAnotherCard(ChangeJokerBackRequest request,
            MainUser control, SynchronizationContext synchCont)
        {
            Task.Run(() => ChangeCardBackToJoker(request, control, synchCont));
        }

        private void ChangeJokerIntoCard(ChangeJokerIntoAnotherCardRequest request,
            MainUser control, SynchronizationContext synchCont)
        {
            try
            {
                IMakaoGameHostService proxy = factory.CreateChannel();
                ChangeJokerCardResponse response = proxy.ChangeJokerIntoANotherCard(request);
                if (response.PlayerID == MainWindow.PlayerID)
                {
                    synchCont.Post(_ => AssignMainControCards(control, response.CurrentPlayerCards), null);
                }
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while changing joker to another card: {ex.Message}.");
            }
        }

        private void ChangeCardBackToJoker(ChangeJokerBackRequest request,
            MainUser control, SynchronizationContext synchCont)
        {
            try
            {
                IMakaoGameHostService proxy = factory.CreateChannel();
                ChangeJokerCardResponse response = proxy.ChangeCardIntoJokerBack(request);
                if (response.PlayerID == MainWindow.PlayerID)
                {
                    synchCont.Post(_ => AssignMainControCards(control, response.CurrentPlayerCards), null);
                }
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while changing joker back: {ex.Message}.");
            }
        }

        private void AssignMainControCards(MainUser control, List<PlayingCard> cardsList)
        {
            control.RemoveAllCardsFromControl();
            control.AddCardsToCOntrol(cardsList);
            control.ResetAllHighlights();

            OnResponseArrived();
        }

        #endregion

        #region Making a move by this player

        public void MakeAMove(MakeAMoveRequest request, SynchronizationContext synchCont)
        {
            Task.Run(() => SendInfoAboutMovementToHost(request, synchCont));
        }

        private void SendInfoAboutMovementToHost(MakeAMoveRequest request, SynchronizationContext synchCont)
        {
            try
            {
                IMakaoGameHostService proxy = factory.CreateChannel();
                bool response = proxy.PerformPlayerMove(request);
                if (!response)
                {
                    synchCont.Post(_ => ShowInfoAboutMovePerformingError(), null);
                }
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error($"Error while performing a move - sending data to host: {ex.Message}.");
            }
        }

        private void ShowInfoAboutMovePerformingError()
        {
            MessageBox.Show("Niepowodzenie przy wykonywaniu ruchu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #endregion

        #region autoResponse

        public delegate void ReceivedResponseFromHostEventHandler(object sender, EventArgs e);
        public event ReceivedResponseFromHostEventHandler ResponseArrived;
        protected virtual void OnResponseArrived()
        {
            ResponseArrived?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
