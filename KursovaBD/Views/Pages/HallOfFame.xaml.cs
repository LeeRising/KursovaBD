using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KursovaBD.Views.Pages
{
    public partial class HallOfFame : UserControl
    {
        public HallOfFame()
        {
            InitializeComponent();
            //SELECT Name,Age FROM dogs ORDER BY Medals_count DESC LIMIT 3
        }
    }
}
