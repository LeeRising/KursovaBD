using KursovaBD.Models;
using KursovaBD.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KursovaBD.Controls
{
    public partial class RingCards : UserControl
    {
        public RingCards(string header, Uri breed, List<string> experts, string timeStart, string timeEnd, List<DogModel> members)
        {
            InitializeComponent();
            Header.Header = header;
            Breed.Source = new BitmapImage(breed);
            Time.Text = timeStart + " - " + timeEnd;
            Members.ItemsSource = members.Select(x => x.NameAge).ToArray();

            string judgeName = "";
            foreach (var item in experts)
            {
                judgeName += item + ",";
            }
            JudgeNameTb.Text = "Judge: " + judgeName.Substring(0, judgeName.Length - 1);
            Members.SelectionChanged += delegate
            {
                MainWindow.Instance.Hide();
                new DogInfoDialog(members[Members.SelectedIndex].NameAge, members[Members.SelectedIndex].Id).ShowDialog();
                MainWindow.Instance.Show();
            };
        }
    }
}
