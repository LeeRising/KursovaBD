using KursovaBD.Views.Dialogs;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace KursovaBD.Controls
{
    public partial class NewDogElement : UserControl
    {
        public NewDogElement()
        {
            InitializeComponent();
            DogName.ItemsSource = DogsBattleCreator.Instance.DogsList.Select(x=>x.NameAge);
            AddNewBtn.IsEnabled = false;
            AddNewBtn.Click += delegate
            {
                DogsBattleCreator.Instance.DogsList.RemoveAt(DogName.SelectedIndex);
                DogsBattleCreator.Instance.AddDog();
                AddNewBtn.Visibility = Visibility.Hidden;
                DogName.IsEnabled = false;
            };
            DogName.SelectionChanged += delegate
            {
                if (DogName.SelectedValue != null)
                    AddNewBtn.IsEnabled = true;
            };
        }
    }
}
