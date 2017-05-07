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
                MainWindowContent.Children.Add(_DogsShow);
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
            if (IsLogin)
                Task.Factory.StartNew(() => messageQueue.Enqueue("All fiel_DogsShow must be not empty!"));
            DbConnection.Close();
            IsLogin = false;
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLogin = true;
        }
        private void TitleBarPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void MenuToggler(int elementIndex)
        {
            foreach (Button c in MainMenu.Children.OfType<Button>())
            {
                if (c != MainMenu.Children[elementIndex])
                    c.Style = DefaultBtnStyle;
            }
        }
        void ContentToggler(UIElement uie)
        {
            MainWindowContent.Children.RemoveAt(0);
            MainWindowContent.Children.Add(uie);
        }
        
        Style DefaultBtnStyle, PressedBtnStyle;
        DogsShow _DogsShow = new DogsShow();
        HallOfFame _HallOfFame = new HallOfFame();
        RegisterDog _RegisterDog = new RegisterDog();
        RegisterAsExpert _RegisterAsExpert = new RegisterAsExpert();
        ShowRequests _ShowRequests = new ShowRequests();
        private void DogsShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DogsShowBtn.Style == DefaultBtnStyle)
            {
                DogsShowBtn.Style = PressedBtnStyle;
                MenuToggler(MainMenu.Children.IndexOf(DogsShowBtn));
                ContentToggler(_DogsShow);
            }
        }

        private void SendRegistrDogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SendRegistrDogBtn.Style == DefaultBtnStyle)
            {
                SendRegistrDogBtn.Style = PressedBtnStyle;
                MenuToggler(MainMenu.Children.IndexOf(SendRegistrDogBtn));
                ContentToggler(_RegisterDog);
            }
        }

        private void ExpertRegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ExpertRegisterBtn.Style == DefaultBtnStyle)
            {
                ExpertRegisterBtn.Style = PressedBtnStyle;
                MenuToggler(MainMenu.Children.IndexOf(ExpertRegisterBtn));
                ContentToggler(_RegisterAsExpert);
            }
        }

        private void HallofFameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (HallofFameBtn.Style == DefaultBtnStyle)
            {
                HallofFameBtn.Style = PressedBtnStyle;
                MenuToggler(MainMenu.Children.IndexOf(HallofFameBtn));
                ContentToggler(_HallOfFame);
            }
        }

        private void ShowRequestsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowRequestsBtn.Style == DefaultBtnStyle)
            {
                ShowRequestsBtn.Style = PressedBtnStyle;
                MenuToggler(MainMenu.Children.IndexOf(ShowRequestsBtn));
                ContentToggler(_ShowRequests);
            }
        }
    }
}
