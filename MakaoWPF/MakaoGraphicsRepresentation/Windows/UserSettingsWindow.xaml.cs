using CardGraphicsLibraryHandler;
using CardsRepresentation;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow : Window
    {
        #region Fields and properties

        //saved colour of backs of cards
        private BackColor backColor;
        public BackColor BackColor { get { return backColor; } }

        //saved location of third player cards
        private ThirdPlayerLocation thirdPlayerLocation;
        public ThirdPlayerLocation ThirdPlayerLocation { get { return thirdPlayerLocation; } }

        //enabled timeout of joining the room for players
        private bool joiningTheRoomTimeoutEnabled;
        public bool JoiningTheRoomTimeoutEnabled { get { return joiningTheRoomTimeoutEnabled; } }

        //amount of time for timeout of joining to the room - in minutes
        private int joiningTheRoomTimeoutInMinutes;
        public int JoiningTheRoomTimeoutInMinutes { get { return joiningTheRoomTimeoutInMinutes; } }

        //enabled timeout of confirming readiness to start the game
        private bool readinessForPlayTimeoutEnabled;
        public bool ReadinessForPlayTimeoutEnabled { get { return readinessForPlayTimeoutEnabled; } }

        //amount of time for timeout of confirming readiness to start play the game - in minutes
        private int readinessForPlayTimeoutInMinutes;
        public int ReadinessForPlayTimeoutInMinutes { get { return readinessForPlayTimeoutInMinutes; } }

        #endregion

        #region Constructor

        //passing in data app saved data
        public UserSettingsWindow(BackColor backColor, bool joiningTheRoomTimeoutEnabled, int joiningTheRoomTimeoutInMinutes,
            bool readinessForPlayTimeoutEnabled, int readinessForPlayTimeoutInMinutes, ThirdPlayerLocation location)
        {
            InitializeComponent();

            //assigning local varaibles
            this.backColor = backColor;
            this.joiningTheRoomTimeoutEnabled = joiningTheRoomTimeoutEnabled;
            this.joiningTheRoomTimeoutInMinutes = joiningTheRoomTimeoutInMinutes;
            this.readinessForPlayTimeoutEnabled = readinessForPlayTimeoutEnabled;
            this.readinessForPlayTimeoutInMinutes = readinessForPlayTimeoutInMinutes;
            this.thirdPlayerLocation = location;
            CheckSideRadiobutton(location);
            LimitTimeForJoiningToTheRoomCheckBox.IsChecked = this.joiningTheRoomTimeoutEnabled;
            AcceptanceReadinessToPlayGameCheckBox.IsChecked = this.readinessForPlayTimeoutEnabled;
            JoiningTimeoutSlider.Value = (double)this.joiningTheRoomTimeoutInMinutes;
            ReadinessTimeoutSlider.Value = (double)this.readinessForPlayTimeoutInMinutes;

            //adding all available cardBacks to WrapPanel
            AddAllCardsToWrapPanel();

            //selecting apropriate color
            SelectOneBack(backColor);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Settings Window opened.");
        }

        #endregion

        #region Adding cards to WrapPanel

        //select onlyOneCard
        private void SelectOneBack(BackColor backColor)
        {
            DeselectAllCards();
            foreach (WrapPanelSelectableItem item in CardBacksWrapPanel.Children)
            {
                if ((BackColor)item.Tag == backColor)
                {
                    item.ItemSelected = Visibility.Visible;
                    break;
                }
            }
        }

        //clear selection of all cards
        private void DeselectAllCards()
        {
            foreach (WrapPanelSelectableItem item in CardBacksWrapPanel.Children)
            {
                item.ItemSelected = Visibility.Collapsed;
            }
        }

        //foreach loop iteration for adding all available cards to the wrappanel
        private void AddAllCardsToWrapPanel()
        {
            foreach (BackColor backColor in (BackColor[])Enum.GetValues(typeof(BackColor)))
            {
                AddSingleCardBackToWrapPanel(backColor);
            }
        }

        //adding only one card to the wrap panel
        private void AddSingleCardBackToWrapPanel(BackColor color)
        {
            WrapPanelSelectableItem button = new WrapPanelSelectableItem
            {
                Height = 110,
                Width = 110,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0),
                ItemSelected = Visibility.Collapsed,
                ItemHoovered = Visibility.Collapsed,
                ImageSource = CardBackImageSourceObtainer.GetBackImageSource(color),
                Tag = color,
            };

            //mouse enter and leave event
            button.MouseEnter += CardBackButton_MouseEnter;
            button.MouseLeave += CardBackButton_MouseLeave;
            button.Click += AvatarButton_Click;

            CardBacksWrapPanel.Children.Add(button);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Added one card back to the Wrap Panel: " + color.ToString());
        }

        #endregion

        #region CardBacks event handlers

        //highlight button when mouse hoovers above it
        private void CardBackButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;
        }

        //unhighlight button when mouse stops hoover abowe it
        private void CardBackButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        //method for highlightening specific button
        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            SelectOneBack((BackColor)(sender as WrapPanelSelectableItem).Tag);

            //assign new backColor to local variable that holds its value
            backColor = (BackColor)(sender as WrapPanelSelectableItem).Tag;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("New Card Back CHoice: " + ((BackColor)(sender as WrapPanelSelectableItem).Tag).ToString());
        }

        #endregion

        #region Changing value of timeout - and checkboxes

        //event handler for changing the value of slider - readiness to the game
        private void ReadinessTimeoutSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            readinessForPlayTimeoutInMinutes = (int)(sender as Slider).Value;
        }

        //event handler for changing the value of slider - joining the game
        private void JoiningTimeoutSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            joiningTheRoomTimeoutInMinutes = (int)(sender as Slider).Value;
        }

        //enabking joining to the room timeout
        private void LimitTimeForJoiningToTheRoomCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            joiningTheRoomTimeoutEnabled = true;
        }

        //disabling joining to the room timeout
        private void LimitTimeForJoiningToTheRoomCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            joiningTheRoomTimeoutEnabled = false;
        }

        //enabling waitning for readiness confirmation timeout
        private void AcceptanceReadinessToPlayGameCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            readinessForPlayTimeoutEnabled = true;
        }

        //disabling waitning for readiness confirmation timeout
        private void AcceptanceReadinessToPlayGameCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            readinessForPlayTimeoutEnabled = false;
        }

        #endregion

        #region Dealing with RadioButton

        //method for selectin gone of two radiobuttons
        private void CheckSideRadiobutton(ThirdPlayerLocation location)
        {
            switch (location)
            {
                case ThirdPlayerLocation.Left:
                    LeftPlayerDisplay.IsChecked = true;
                    RightPlayerDisplay.IsChecked = false;
                    break;
                case ThirdPlayerLocation.Right:
                    RightPlayerDisplay.IsChecked = true;
                    LeftPlayerDisplay.IsChecked = false;
                    break;
            }
        }

        //selecting left side
        private void LeftPlayerDisplay_Click(object sender, RoutedEventArgs e)
        {
            thirdPlayerLocation = ThirdPlayerLocation.Left;
        }

        //selecting right side
        private void RightPlayerDisplay_Click(object sender, RoutedEventArgs e)
        {
            thirdPlayerLocation = ThirdPlayerLocation.Right;
        }

        #endregion

        #region Cancel and OK Buttons handling

        //Cancel button clicking
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //confirm button clicking
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            OnSettingsChanged(joiningTheRoomTimeoutEnabled, joiningTheRoomTimeoutInMinutes, readinessForPlayTimeoutEnabled,
                readinessForPlayTimeoutInMinutes, backColor, thirdPlayerLocation);
            this.Close();
        }

        #endregion

        #region Event for closing the window

        //event for passing data of nev event to main window
        public delegate void SettingsDataEventHandler(object sender, SettingsDataEventArgs e);
        public event SettingsDataEventHandler SettingsChanged;
        protected virtual void OnSettingsChanged(bool joiningTimeoutEnabled, int joiningTimeout, bool readinessTimeoutEnabled,
            int readinessTimeout, BackColor cardsBackColor, ThirdPlayerLocation location)
        {
            SettingsChanged?.Invoke(null, new SettingsDataEventArgs
            {
                JoiningEnabled = joiningTimeoutEnabled,
                JoiningTimeout = joiningTimeout,
                ReadinessEnabled = readinessTimeoutEnabled,
                ReadinessTimeout = readinessTimeout,
                CardBackColor = cardsBackColor,
                LocationOfThirdPLayersCards = location,
            });
        }

        #endregion

        #region Settings window closing

        //event handler of closing of the window event
        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Settings window is beeing closed.");
        }

        #endregion
    }

    #region Class holding data to pass it to main window

    public class SettingsDataEventArgs : EventArgs
    {
        public BackColor CardBackColor { get; set; }
        public ThirdPlayerLocation LocationOfThirdPLayersCards { get; set; }
        public bool JoiningEnabled { get; set; }
        public int JoiningTimeout { get; set; }
        public bool ReadinessEnabled { get; set; }
        public int ReadinessTimeout { get; set; }
    }

    #endregion
}
