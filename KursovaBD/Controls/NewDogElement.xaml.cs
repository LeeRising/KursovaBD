using KursovaBD.Views.Dialogs;
using System.Linq;
using System.Windows.Controls;

namespace KursovaBD.Controls
{
    public partial class NewDogElement : UserControl
    {
        public NewDogElement()
        {
            InitializeComponent();
            DogName.ItemsSource = DogsBattleCreator.Instance._dogs.Select(x=>x.NameAge);
            AddNewBtn.Click += delegate
            {
                DogsBattleCreator.Instance._dogs.RemoveAt(DogName.SelectedIndex);
                DogsBattleCreator.Instance.AddDog();
            };
        }
    }
}
