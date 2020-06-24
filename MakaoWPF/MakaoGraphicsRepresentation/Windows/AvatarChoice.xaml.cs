using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for AvatarChoice.xaml
    /// </summary>
    public partial class AvatarChoice : Window
    {
        private string currentAvatar;
        private string defuaultAvatarDirectory;
        private string userAvatarDirectory;
        private readonly List<string> AvatarsList;

        //for copying new png as new avatar
        private string fileName;
        private string sourceDirectory;

        public AvatarChoice(string picture)
        {
            InitializeComponent();

            //create list of images
            AvatarsList = new List<string>();
            GetDirectories(MainWindow.LogLocation);
            CreateActualImagesList();

            //Adding buttons
            AddAllButtonsToWrapPanel();

            //choose the right image as startup avatar
            currentAvatar = picture;
            HighlightAvatarAtTheStart(picture);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Avatar choice window succeddfully opened.");
        }

        #region Reading avatars names from directories

        //finding directories for avatars
        private void GetDirectories(string logLocationString)
        {
            defuaultAvatarDirectory = logLocationString.Substring(0, logLocationString.Length - 5) + @"\Avatars\";
            userAvatarDirectory = logLocationString.Substring(0, logLocationString.Length - 5) + @"\AvatarsUser\";
        }

        //creation of images list
        private void CreateActualImagesList()
        {
            AvatarsList.Clear();

            try
            {
                ReadFilesFromtDirectory(defuaultAvatarDirectory, false);
                ReadFilesFromtDirectory(userAvatarDirectory, true);
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Couldn't create list of avatars: " + ex.Message);
            }
        }

        //method for reading file names from some directory
        private void ReadFilesFromtDirectory(string directory, bool userDirectory)
        {
            DirectoryInfo info = new DirectoryInfo(directory);
            FileInfo[] files = info.GetFiles("*.png");
            for (int i = 0; i < files.Length; i++)
            {
                if (!userDirectory) AvatarsList.Add(defuaultAvatarDirectory + files[i].Name);
                else AvatarsList.Add(userAvatarDirectory + files[i].Name);
            }
        }

        #endregion

        #region Adding buttons to WrapPanel

        //method for adding all buttons
        private void AddAllButtonsToWrapPanel()
        {
            AvatarsWrapPanel.Children.Clear();

            for (int i = 0; i < AvatarsList.Count; i++)
            {
                AddSingleButton(AvatarsList[i], i);
            }

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Amount of pictures in WrapPane changed to: " + AvatarsWrapPanel.Children.Count);
        }

        //Add single button to WrapPanel
        private void AddSingleButton(string imageSource, int tag)
        {
            WrapPanelSelectableItem button = new WrapPanelSelectableItem
            {
                Height = 128,
                Width = 128,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0),
                ItemSelected = Visibility.Collapsed,
                ItemHoovered = Visibility.Collapsed,
                Picture = imageSource,
                Tag = tag,
            };

            //mouse enter and leave event
            button.MouseEnter += AvatarButton_MouseEnter;
            button.MouseLeave += AvatarButton_MouseLeave;
            button.Click += AvatarButton_Click;

            AvatarsWrapPanel.Children.Add(button);
        }

        //highlight button when mouse hoovers abowe it
        private void AvatarButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Visible;
        }

        //unhighlight button when mouse stops hoover abowe it
        private void AvatarButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as WrapPanelSelectableItem).ItemHoovered = Visibility.Collapsed;
        }

        #endregion

        #region Dealing with highlight of avatars   

        //method for highlightening specific button
        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAvatarHighlight();
            (sender as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
            currentAvatar = (sender as WrapPanelSelectableItem).Picture;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("New avatar choice: " + currentAvatar);
        }

        //highlight avater at the window opening
        private void HighlightAvatarAtTheStart(string picture)
        {
            ResetAvatarHighlight();
            foreach (var item in AvatarsWrapPanel.Children)
            {
                if ((item as WrapPanelSelectableItem).Picture == picture)
                {
                    (item as WrapPanelSelectableItem).ItemSelected = Visibility.Visible;
                }
            }
        }

        //method thar resets all the highlight of buttons with avatars
        private void ResetAvatarHighlight()
        {
            foreach (var item in AvatarsWrapPanel.Children)
            {
                (item as WrapPanelSelectableItem).ItemSelected = Visibility.Collapsed;
            }
        }

        #endregion

        #region Confirming and canceling buttons handling

        //clising window method
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseTheWindow();
        }

        //method for confirmation of choice of new avatar
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Change the avatar at closing avatar window to: " + currentAvatar.ToString());
            OnAvatarAssignmentChanged(currentAvatar);
            CloseTheWindow();
        }

        //closing method
        private void CloseTheWindow()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Avatar choice window is closing.");
            this.Close();
        }

        //event for passing data of nev event to main window
        public delegate void AvatarChangedEventHandler(object sender, AvatarAssigningEventArgs e);

        public  event AvatarChangedEventHandler AvatarAssignmentChanged;

        protected virtual void OnAvatarAssignmentChanged(string avatarString)
        {
            AvatarAssignmentChanged?.Invoke(null, new AvatarAssigningEventArgs { TypeOfAvatar = avatarString });
        }

        #endregion

        #region Adding custom avatars

        //event handler that allow to browse to new avatar file
        private void NewAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            //configure file explorer window
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Wybierz nowy avatar",
                DefaultExt = "png",
                Filter = "Pliki png (*.png)|*.png|Wszystkie pliki (*.*)|*.*",
                FilterIndex = 1,
            };

            //establishing directory of default-opened file explorer
            if (!string.IsNullOrWhiteSpace(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                openFileDialog.FileName = Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }

            //displaying the file explorer dialog
            bool? result = openFileDialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                if (openFileDialog.FileName.Substring(openFileDialog.FileName.Length - 3) == "png")
                {
                    fileName = openFileDialog.SafeFileName;
                    sourceDirectory = openFileDialog.FileName.Substring(0, openFileDialog.FileName.Length - fileName.Length);
                    //openFileDialog = null;
                    CopyTheAvatar(fileName, sourceDirectory, userAvatarDirectory);
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Attempt of copying not an png file.");
                    MessageBox.Show("Wybrany plik nie ma rozszerzenia *.png", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //method for copying one file
        private void CopyTheAvatar(string fileName, string sourceDirectory, string destinationDirectory)
        {
            string sourceFile = System.IO.Path.Combine(sourceDirectory, fileName);
            string destinationFile = System.IO.Path.Combine(destinationDirectory, fileName);
            
            try
            {
                //proper copying
                System.IO.File.Copy(sourceFile, destinationFile, true);

                //updating the WrapPanelContent
                CreateActualImagesList();
                AddAllButtonsToWrapPanel();
            }
            catch (Exception ex)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error("Copying new avatar failed: " + ex.Message);

                MessageBox.Show("Nie udało się skopiować pliku!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Information tooltip

        //showing the tooltip
        private void InfoLabel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TooltipGrid.Visibility = Visibility.Visible;
        }

        //hiding the tooltip
        private void InfoLabel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TooltipGrid.Visibility = Visibility.Collapsed;
        }

        #endregion
    }

    #region class for passing avatar to main window

    public class AvatarAssigningEventArgs : EventArgs
    {
        public string TypeOfAvatar { get; set; }
    }

    #endregion
}
