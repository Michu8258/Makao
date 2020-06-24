using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MakaoGraphicsRepresentation
{
    public class WrapPanelSelectableItem : Button
    {
        #region Selected Item property

        public Visibility ItemSelected
        {
            get { return (Visibility)GetValue(ItemSelectedProperty); }
            set { SetValue(ItemSelectedProperty, value); }
        }

        public static readonly DependencyProperty ItemSelectedProperty =
            DependencyProperty.Register("ItemSelected", typeof(Visibility), typeof(WrapPanelSelectableItem), null);

        #endregion

        #region Hoover at Item Property

        public Visibility ItemHoovered
        {
            get { return (Visibility)GetValue(ItemHooveredProperty); }
            set { SetValue(ItemHooveredProperty, value); }
        }

        public static readonly DependencyProperty ItemHooveredProperty =
            DependencyProperty.Register("ItemHoovered", typeof(Visibility), typeof(WrapPanelSelectableItem), null);

        #endregion

        #region Picture Property

        public string Picture
        {
            get { return (string)GetValue(PictureProperty); }
            set { SetValue(PictureProperty, value); }
        }

        public static readonly DependencyProperty PictureProperty =
            DependencyProperty.Register("Picture", typeof(string), typeof(WrapPanelSelectableItem), null);

        #endregion

        #region Image Property

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(WrapPanelSelectableItem), null);

        #endregion
    }
}
