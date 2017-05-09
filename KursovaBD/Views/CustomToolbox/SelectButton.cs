using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KursovaBD
{
    public class SelectButton: Button
    {
        Style DefaultBtnStyle, PressedBtnStyle;

        public SelectButton()
        {
            PressedBtnStyle = FindResource("MaterialDesignRaisedAccentButton") as Style;
            DefaultBtnStyle = FindResource("MaterialDesignRaisedButton") as Style;
        }

        protected override void OnClick()
        {
            base.OnClick();
            this.Style = PressedBtnStyle;
        }

    }
}
