﻿using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using KursovaBD.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace KursovaBD.Views.Pages
{
    public partial class DogPage : UserControl
    {
        ImageSource _no_image;
        MySqlConnection _DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        string _lastUpdatesrt = "";
        public DogPage()
        {
            InitializeComponent();
            Instance = this;
            _no_image = DogAvatar.Source;
            messageQueue = MessagesSnackbar.MessageQueue;
            About.LostFocus += delegate { _aboutUpdate(); };
            About.KeyDown += (s, e) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
                    _aboutUpdate();
            };
        }

        async void _aboutUpdate()
        {
            if (_lastUpdatesrt != About.Text)
            {
                using (_DbConnection)
                {
                    await _DbConnection.OpenAsync();
                    msc = new MySqlCommand(String.Format("update dogs set About='{0}' where Master_id='{1}'", About.Text, UserModel.Id), _DbConnection);
                    await msc.ExecuteNonQueryAsync();
                    await Task.Factory.StartNew(() => messageQueue.Enqueue("About dog updated"));
                }
                _lastUpdatesrt = About.Text;
            }
        }

        public static DogPage Instance { get; private set; }
        public async void GetDogInfo()
        {
            using (_DbConnection)
            {
                await _DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select Club_id,Name,Breed,Age,Document_info,Parents_name,Date_last_vaccenation,Photo,About from dogs where Master_id='{0}'", UserModel.Id), _DbConnection);
                MySqlDataReader mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    DogAvatar.Source = mdr["Photo"] as string == "No_image.png" ? _no_image : new BitmapImage(new Uri("http://kursova.sytes.net/" + mdr["Photo"] as string));
                    int _id = Convert.ToInt32(mdr[0]);
                    ClubName.Text = UserModel.Clubs[_id--];
                    DogNameAge.Text = (string)mdr[1] + "," + Convert.ToString(mdr[3]);
                    Breed.Text = (string)mdr[2];
                    Document.Text = (string)mdr[4];
                    if (mdr[8] as string != null)
                        HintAssist.SetHint(About, "About dog");
                    else
                        HintAssist.SetHint(About, "Write about our dog");
                    _lastUpdatesrt = About.Text = mdr[8] as string ?? "";
                    LastVaccenationDate.Text = mdr[6].ToString().Split(' ')[0];
                    ParentsName.Text = (string)mdr[5];
                }
            }
        }
    }
}
