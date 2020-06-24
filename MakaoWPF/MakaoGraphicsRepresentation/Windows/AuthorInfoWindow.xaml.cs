using System.Windows;

namespace MakaoGraphicsRepresentation.Windows
{
    /// <summary>
    /// Interaction logic for AuthorInfoWindow.xaml
    /// </summary>
    public partial class AuthorInfoWindow : Window
    {
        public AuthorInfoWindow()
        {
            InitializeComponent();
            AuthorInfoLabel.Text = Properties.AuthorInfoResource.AuthorInfoText;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
