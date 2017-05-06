using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data.SQLite;
using KursovaBD.Tools.AppConfiguration;
using SQLite;
using KursovaBD.UI.Pages;

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        MySqlConnection DbConnection = new MySqlConnection("Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root");
        SnackbarMessageQueue messageQueue;
        MySqlCommand msc;
        SQLiteAsyncConnection db = new SQLiteAsyncConnection("cfg\\AppConfiguration.sqlite", SQLiteOpenFlags.ReadWrite, true);
        public object RequesCount { get; private set; }

        private bool IsLogin = false;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            messageQueue = MessagesSnackbar.MessageQueue;

            PressedBtnStyle = FindResource("MaterialDesignRaisedAccentButton") as Style;
            DefaultBtnStyle = FindResource("MaterialDesignRaisedButton") as Style;

            #region TitleBar
            CloseBtn.Click += delegate
            {
                Close();
            };
            MinimizeBtn.Click += delegate
            {
                MinimizeBtn.IsChecked = false;
                WindowState = WindowState.Minimized;
            };
            #endregion

            try
            {
                CheckRequests();
                AppConfigMethod();

                DogsShowBtn.Style = PressedBtnStyle;
                //Dogs Show Content
            }
            catch (MySqlException ms)
            {
                Task.Factory.StartNew(() => messageQueue.Enqueue(ms.Message));
            }
        }
        void CheckRequests()
        {
            DbConnection.Open();
            msc = new MySqlCommand("select count(id) from dogs where Request='waiting'", DbConnection);
            var v = int.Parse(msc.ExecuteScalar().ToString());
            msc = new MySqlCommand("select count(id) from experts where Request='waiting'", DbConnection);
            v += int.Parse(msc.ExecuteScalar().ToString());
            if (v != 0)
            {
                RequesCount = v;
                ShowRequestsBtn.Content = v > 1 ? "Show requests" : "Show request";
                ShowRequestsBtn.IsEnabled = true;
            }
            else
            {
                ShowRequestsBtn.Content = "None request";
                ShowRequestsBtn.IsEnabled = false;
                RequesCount = null;
            }
            DbConnection.Close();
        }
        void AppConfigMethod()
        {
            if (!File.Exists("cfg\\AppConfiguration.sqlite"))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile("AppConfiguration.sqlite");
                File.Move("AppConfiguration.sqlite", "cfg\\AppConfiguration.sqlite");
                db.CreateTableAsync<AppConfig>();
                var v = new List<AppConfig>();
                v.Add(new AppConfig
                {
                    login = "0",
                    password = "0"
                });
                db.InsertAllAsync(v);
            }
            //UserSetting.lang = db.GetAsync<UserAppInfo>(0).Result.translate;
        }

        private void LoginDialog_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            DbConnection.Open();
            if (!String.IsNullOrEmpty(LoginTb.Text) & !String.IsNullOrEmpty(PassTb.Password))
            {
                msc = new MySqlCommand(String.Format("select * from organizer where login='{0}' and password='{1}'",
                    LoginTb.Text, Cryptography.getHashSha256(PassTb.Password)), DbConnection);
                if (msc.ExecuteScalar() != null)
                {
                    LoginAsAdminBtn.Content = "Hello " + LoginTb.Text;
                    CountingRequestBadge.Visibility = Visibility.Visible;
                    LoginTb.Text = "";
                    PassTb.Password = "";
                }
                else
                    Task.Factory.StartNew(() => messageQueue.Enqueue("Login or password is incorect!"));
            }
            if(IsLogin)
                Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not empty!"));
            DbConnection.Close();
            IsLogin = false;
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLogin = true;
        }

        void MenuToggler()
        {
            foreach (Button c in MainMenu.Children.OfType<Button>())
            {
                if (c != MainMenu.Children[elementIndex])
                    c.Style = DefaultBtnStyle;
            }
        }

        int elementIndex;
        Style DefaultBtnStyle, PressedBtnStyle;
        DogsShow ds = new DogsShow();
        private void DogsShowBtn_Click(object sender, RoutedEventArgs e)
        {
            //MainWindowContent.Children.Add(ds);
            DogsShowBtn.Style = PressedBtnStyle;
            elementIndex = MainMenu.Children.IndexOf(DogsShowBtn);
            MenuToggler();
        }

        private void SendRegistrDogBtn_Click(object sender, RoutedEventArgs e)
        {
            SendRegistrDogBtn.Style = PressedBtnStyle;
            elementIndex = MainMenu.Children.IndexOf(SendRegistrDogBtn);
            MenuToggler();
        }

        private void ExpertRegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpertRegisterBtn.Style = PressedBtnStyle;
            elementIndex = MainMenu.Children.IndexOf(ExpertRegisterBtn);
            MenuToggler();
        }

        private void HallofFameBtn_Click(object sender, RoutedEventArgs e)
        {
            HallofFameBtn.Style = PressedBtnStyle;
            elementIndex = MainMenu.Children.IndexOf(HallofFameBtn);
            MenuToggler();
            //MainWindowContent.Children.Remove(ds);
        }
    }
}
