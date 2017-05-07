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

namespace KursovaBD.UI.Pages
{
    /// <summary>
    /// Interaction logic for RegisterDog.xaml
    /// </summary>
    public partial class RegisterDog : UserControl
    {
        public RegisterDog()
        {
            InitializeComponent();
            DataContext = this;
        }
        string[] Breads = {"Akita Inu","English Bulldog","English Cocker Spaniel","Afghanician Bossia","Border Collie","Briar","Brusselsky Griffon","Welsh-Corgias","Greyhound","Dalmathin","Labrador","Keeshond","Hungarian Shepherd","Kurtzhaar","Levretka","Leonberger","Pekingese","Pomeransky Spitz","Poodle","The Samish dog","Japanese chin","Shelti","Shi-tcu"};
}
}
