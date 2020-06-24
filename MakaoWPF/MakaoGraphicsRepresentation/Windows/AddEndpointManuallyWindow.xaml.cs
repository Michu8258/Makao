using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for AddEndpointManuallyWindow.xaml
    /// </summary>
    public partial class AddEndpointManuallyWindow : Window
    {
        #region Filds and constructor

        private List<TextBox> IPOctetsList;
        private List<int> IPOctets;
        private bool addressError;
        private string outputIP;

        public AddEndpointManuallyWindow()
        {
            InitializeComponent();
            PopulateListOfOctets();
            ClearOctets();

            addressError = false;
            outputIP = "";

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Window for manually adding endpoint to list of endpoints opened");
        }

        #endregion

        #region Dealing with octets of IP address

        private void PopulateListOfOctets()
        {
            IPOctetsList = new List<TextBox>()
            {
                FirstOctet,
                SecondOctet,
                ThirdOctet,
                FourthOctet,
            };
        }

        private void AssignOctets()
        {
            IPOctets = new List<int>();
            addressError = false;

            foreach (var item in IPOctetsList)
            {
                try
                {
                    bool success = Int32.TryParse(item.Text, out int octet);
                    if (success && octet < 256)
                    {
                        IPOctets.Add(octet);
                    }
                    else
                    {
                        addressError = true;
                        IPOctets.Add(-10);
                        ShowMessageBoxAbouthOctetError(IPOctets.Count);
                    }
                }
                catch (Exception ex)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Error($"Error while trying to parse string to int - IP addres: {ex.Message}.");
                }
            }

            if (addressError) ClearOctets();
        }

        private void ShowMessageBoxAbouthOctetError(int octetNumber)
        {
            MessageBox.Show($"Niepoprawny adres IP: {octetNumber} oktet.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void ClearOctets()
        {
            foreach (var item in IPOctetsList)
            {
                item.Text = "";
            }
        }

        #endregion

        #region Reaction for clicking buttos (OK and cancel)

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            AssignOctets();
            if (!addressError)
            {
                outputIP = $"{IPOctets[0]}.{IPOctets[1]}.{IPOctets[2]}.{IPOctets[3]}";
                OnManualHostInputed(outputIP);
                Close();
            }
            else
            {
                ClearOctets();
            }
        }

        #endregion

        #region Event that fires when password was not null

        public delegate void AddHostManuallyEventHandler(object sender, IPAddresEventArgs e);
        public event AddHostManuallyEventHandler ManualHostInputed;
        protected virtual void OnManualHostInputed(string address)
        {
            ManualHostInputed?.Invoke(this, new IPAddresEventArgs { IPAddress = address });
        }

        #endregion
    }

    #region Class for closing event args

    public class IPAddresEventArgs : EventArgs
    {
        public string IPAddress;
    }

    #endregion
}
