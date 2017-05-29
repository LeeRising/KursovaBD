using KursovaBD.Utilits;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows;

namespace KursovaBD.Controls
{
    public partial class DogCard : UserControl
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        int _id;

        public DogCard(int id,string name,string club,Uri photo,int mark)
        {
            InitializeComponent();
            _id = id;
            Name.Text = name;
            Club.Text = club;
            Photo.Source = new BitmapImage(photo);
            DogMark.Value = mark;
            DogMark.LostFocus += DogMark_MouseLeftButtonDown;
        }

        private async void DogMark_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("update dogs set Mark='{0}' where id='{1}'", DogMark.Value, _id), DbConnection);
                await msc.ExecuteNonQueryAsync();
            }
        }
    }
}
