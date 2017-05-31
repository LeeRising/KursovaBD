using KursovaBD.Utilits;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows;
using KursovaBD.Models;

namespace KursovaBD.Controls
{
    public partial class DogCard : UserControl
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        int _id;
        RateViewModel rtm = new RateViewModel();

        public DogCard(int id,string name,string club,Uri photo,int mark)
        {
            InitializeComponent();
            this.DataContext = rtm;
            _id = id;
            Name.Text = name;
            Club.Text = club;
            Photo.Source = new BitmapImage(photo);
            rtm.Rating = mark;
            rtm.PropertyChanged += Rtm_PropertyChanged;
        }

        private async void Rtm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Rating")
            {
                using (DbConnection)
                {
                    await DbConnection.OpenAsync();
                    msc = new MySqlCommand(String.Format("update dogs set Mark='{0}' where id='{1}'", rtm.Rating, _id), DbConnection);
                    await msc.ExecuteNonQueryAsync();
                }
            }
        }
    }
    class RateViewModel : ObservableObject
    {
        private int _rating;

        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                SetProperty(ref _rating, value);
            }
        }
    }
}
