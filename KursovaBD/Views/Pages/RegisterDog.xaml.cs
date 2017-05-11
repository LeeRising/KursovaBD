using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using MaterialDesignThemes.Wpf;
using System;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using KursovaBD.Models;
using System.IO;
using System.Net;

namespace KursovaBD.UI.Pages
{
    public partial class RegisterDog : UserControl
    {
        MySqlConnection DbConnection = Utilits.DbConnector._MySqlConnection();
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        public string[] Breads => new string[] { "Akita Inu", "English Bulldog", "English Cocker Spaniel", "Afghanician Bossia", "Border Collie", "Briar", "Brusselsky Griffon", "Welsh-Corgias", "Greyhound", "Dalmathin", "Labrador", "Keeshond", "Hungarian Shepherd", "Kurtzhaar", "Levretka", "Leonberger", "Pekingese", "Pomeransky Spitz", "Poodle", "The Samish dog", "Japanese chin", "Shelti", "Shi-tcu" };
        OpenFileDialog _OpenFileDialog = new OpenFileDialog
        {
            Filter= "Image Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png",
            FileName="Chose dog picture"
        };
        string avatar = "No_image.png",avatar1;
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
                    avatar1 = avatar;
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
                BreadComboBox.SelectedValue != null &&  ClubComboBox.SelectedValue != null)
            {
                using (DbConnection)
                {
                    DbConnection.Open();
                    msc = new MySqlCommand("select count(id) from dogs",DbConnection);
                    var _id = Int64.Parse(msc.ExecuteScalar().ToString());
                    avatar = avatar != "No_image.png" ? (_id==0?"1":(_id+=1).ToString()) + "." + avatar.Split('.')[1] : "No_image.png";
                    msc = new MySqlCommand(String.Format("insert into dogs (Club_id,Name,Breed,Age,Document_info,Parents_name,Date_last_vaccenation,Master_id,Photo,Request) " +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','waiting')",
                        ClubComboBox.SelectedIndex+1,NameTb.Text,BreadComboBox.SelectedValue,AgeTb.Text,DocumentInfoTb.Text,ParentsnameTb.Text, LastVacDate.DisplayDate.Date.ToString("yyyy-MM-dd"),UserModel.Id,avatar), DbConnection);
                    msc.ExecuteNonQuery();
                    if (!String.IsNullOrEmpty(avatar1))
                    {
                        File.Copy(avatar1, Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\") + avatar);
                        FtpWebRequest request = FtpWebRequest.Create("ftp://kursova.sytes.net/" + avatar) as FtpWebRequest;
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential("ftp", "ftp");
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.KeepAlive = false;
                        FileStream stream = File.OpenRead(avatar);
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        stream.Close();
                        Stream reqStream = request.GetRequestStream();
                        reqStream.Write(buffer, 0, buffer.Length);
                        reqStream.Close();
                        request.Abort();
                        File.Delete(avatar);
                    }
                }
                Task.Factory.StartNew(() => messageQueue.Enqueue("New dog has been registred!"));
                MainWindow.Instance.SetDefaultContent();
                MainWindow.Instance.DogRegisterBtn.Style = FindResource("MaterialDesignRaisedButton") as Style;
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
                    UserModel.Clubs.Add(mdr["Club_name"].ToString());
                }
            }
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand("select Surname,Name from masters", DbConnection);
                mdr = msc.ExecuteReader();
                while (mdr.Read())
                {
                    UserModel.Masters.Add(mdr["Surname"].ToString() + " " + mdr["Name"].ToString());
                }
            }
            ClubComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = UserModel.Clubs });
            MasterComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = UserModel.Masters });
            string[] tmp = UserModel.Masters[int.Parse(UserModel.Id) - 1].ToString().Split(' ');
            UserModel.Surname = tmp[0];
            UserModel.Name = tmp[1];
        }
    }
}
