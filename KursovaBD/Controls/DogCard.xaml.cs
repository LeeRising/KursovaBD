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

namespace KursovaBD.Controls
{
    public partial class DogCard : UserControl
    {
        public DogCard(string name,string club,Uri photo)
        {
            InitializeComponent();
            Name.Text = name;
            Club.Text = club;
            Photo.Source = new BitmapImage(photo);
        }
    }
}
