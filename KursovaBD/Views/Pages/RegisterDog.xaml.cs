using System.Collections.Generic;
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
        OpenFileDialog _OpenFileDialog = new OpenFileDialog
        {
            Filter= "Image Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png",
            FileName="Chose dog picture"
        };
        int club_id, master_id;
        string avatar="as";
        public RegisterDog()
        {
            InitializeComponent();
            DataContext = this;
            messageQueue = MessagesSnackbar.MessageQueue;

            _OpenFileDialog.FileOk += delegate
            {
                try
                {
                    DogPhoto.Source = new BitmapImage(new Uri(_OpenFileDialog.FileName));
                    avatar = _OpenFileDialog.FileName;
                }
                catch (Exception)
                {
                    _OpenFileDialog.ShowDialog();
                }
            };
            DogPhoto.MouseDown += delegate
            {
                _OpenFileDialog.ShowDialog();
            };
            CancelBtn.Click += delegate
            {
                MainWindow.Instance.SetDefaultContent();
                MainWindow.Instance.DogRegisterBtn.Style = FindResource("MaterialDesignRaisedButton") as Style;
            };
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(NameTb.Text) && !String.IsNullOrEmpty(AgeTb.Text) && !String.IsNullOrEmpty(DocumentInfoTb.Text) && !String.IsNullOrEmpty(ParentsnameTb.Text) &&
                BreadComboBox.SelectedValue != null && MasterComboBox.SelectedValue != null && ClubComboBox.SelectedValue != null)
            {
                using (DbConnection)
                {
                    DbConnection.Open();
                    msc = new MySqlCommand(String.Format("insert into dogs (Club_id,Name,Breed,Age,Document_info,Parents_name,Date_last_vaccenation,Master_id,Photo,Request) " +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','waiting')",
                        ClubComboBox.SelectedIndex+1,NameTb.Text,BreadComboBox.SelectedValue,AgeTb.Text,DocumentInfoTb.Text,ParentsnameTb.Text, LastVacDate.DisplayDate.Date.ToString("yyyy-MM-dd"),MasterComboBox.SelectedIndex+1,avatar), DbConnection);
                    msc.ExecuteScalar();
                }
            }
            else
                Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not empty!"));
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
