using System.Windows;
using System.Windows.Controls;

namespace KursovaBD
{
    public class MenuButton : Button
    {
        public MenuButton()
        {

        }

        protected override void OnClick()
        {
            base.OnClick();
            this.Style = FindResource("MaterialDesignRaisedAccentButton") as Style;
        }

    }
}
