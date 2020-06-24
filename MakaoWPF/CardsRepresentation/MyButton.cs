using MakaoInterfaces;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace CardsRepresentation
{
    public class MyButton : Button
    {
        #region Card Property
       
        public PlayingCard Card
        {
            get { return (PlayingCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(PlayingCard), typeof(MyButton), null);

        #endregion
        
        #region Card image Property

        public ImageSource CardImage
        {
            get { return (ImageSource)GetValue(CardImageSource); }
            set { SetValue(CardImageSource, value); }
        }

        public static readonly DependencyProperty CardImageSource =
            DependencyProperty.Register("CardImage", typeof(ImageSource), typeof(MyButton), null);

        #endregion

        #region Back card image Property

        public ImageSource BackCardImage
        {
            get { return (ImageSource)GetValue(BackCardImageSource); }
            set { SetValue(BackCardImageSource, value); }
        }

        public static readonly DependencyProperty BackCardImageSource =
            DependencyProperty.Register("BackCardImage", typeof(ImageSource), typeof(MyButton), null);

        #endregion

        #region StringRepresentation Property

        public string StringRepresentation
        {
            get { return (string)GetValue(StringRepresentationProperty); }
            set { SetValue(StringRepresentationProperty, value); }
        }

        public static readonly DependencyProperty StringRepresentationProperty =
            DependencyProperty.Register("StringRepresentation", typeof(string), typeof(MyButton), null);

        #endregion

        #region NotPermitted Property

        public Visibility NotPermitted
        {
            get { return (Visibility)GetValue(NotPermittedProperty); }
            set { SetValue(NotPermittedProperty, value);}
        }

        public static readonly DependencyProperty NotPermittedProperty =
            DependencyProperty.Register("NotPermitted", typeof(Visibility), typeof(MyButton), null);

        #endregion

        #region AlreadyChoosen Property

        public Visibility AlreadySelected
        {
            get { return (Visibility)GetValue(AlreadySelectedProperty); }
            set { SetValue(AlreadySelectedProperty, value); }
        }

        public static readonly DependencyProperty AlreadySelectedProperty =
            DependencyProperty.Register("AlreadySelected", typeof(Visibility), typeof(MyButton), null);

        #endregion

        #region FromJoker Property

        public Visibility FromJoker
        {
            get { return (Visibility)GetValue(FromJokerProperty); }
            set { SetValue(FromJokerProperty, value); }
        }

        public static readonly DependencyProperty FromJokerProperty =
            DependencyProperty.Register("FromJoker", typeof(Visibility), typeof(MyButton), null);

        #endregion
    }
}
