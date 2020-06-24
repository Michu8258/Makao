using MakaoGraphicsRepresentation.RulesWindow.PagesEnums;
using NLog;
using System.Windows.Controls;

namespace MakaoGraphicsRepresentation.RulesWindow.MovementPages
{
    /// <summary>
    /// Interaction logic for MoreThanOneCardPage.xaml
    /// </summary>
    public partial class MoreThanOneCardPage : Page
    {
        #region fields reusable in derivered pages

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructor

        public MoreThanOneCardPage()
        {
            InitializeComponent();
            AssignContent(MakingMoveEnum.MoreThanOneCard);
        }

        #endregion

        #region Assigning page content

        private void AssignContent(MakingMoveEnum moveType)
        {
            _ = new ContentAssigner(ref Heading, ref DescriptionText01,
                ref DescriptionText02, ref MovementImage, moveType);
            logger.Info($"{moveType.ToString()}Page opened");
        }

        #endregion
    }
}
