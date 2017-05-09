﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using MaterialDesignThemes.Wpf;
using System;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace KursovaBD.UI.Pages
{
    public partial class RegisterDog : UserControl
    {
        MySqlConnection DbConnection = new MySqlConnection("Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root");
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        public string[] Breads => new string[] { "Akita Inu", "English Bulldog", "English Cocker Spaniel", "Afghanician Bossia", "Border Collie", "Briar", "Brusselsky Griffon", "Welsh-Corgias", "Greyhound", "Dalmathin", "Labrador", "Keeshond", "Hungarian Shepherd", "Kurtzhaar", "Levretka", "Leonberger", "Pekingese", "Pomeransky Spitz", "Poodle", "The Samish dog", "Japanese chin", "Shelti", "Shi-tcu" };
        public List<string> Clubs = new List<string>();
        public List<string> Masters = new List<string>();
        OpenFileDialog ofd = new OpenFileDialog
        {
            Filter= "Image Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png",
            FileName="Chose dog picture"
        };
        int club_id, master_id;
        string avatar;
        public RegisterDog()
        {
            InitializeComponent();
            DataContext = this;
            messageQueue = MessagesSnackbar.MessageQueue;

            ofd.FileOk += delegate
            {
                try
                {
                    DogPhoto.Source = new BitmapImage(new Uri(ofd.FileName));
                    avatar = ofd.FileName;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() => messageQueue.Enqueue(ex.Message));
                }
            };
            DogPhoto.MouseDown += delegate
            {
                ofd.ShowDialog();
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MySqlDataReader mdr;
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand("select Club_name from clubs", DbConnection);
                mdr = msc.ExecuteReader();
                while (mdr.Read())
                {
                    Clubs.Add(mdr["Club_name"].ToString());
                }
            }
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand("select Surname,Name from masters", DbConnection);
                mdr = msc.ExecuteReader();
                while (mdr.Read())
                {
                    Masters.Add(mdr["Surname"].ToString() + " " + mdr["Name"].ToString());
                }
            }
            ClubComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = Clubs });
            MasterComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = Masters });
        }
    }
}
