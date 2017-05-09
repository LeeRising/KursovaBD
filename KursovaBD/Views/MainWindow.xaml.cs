﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using KursovaBD.Tools.AppConfiguration;
using SQLite;
using KursovaBD.UI.Pages;

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        MySqlConnection DbConnection = new MySqlConnection("Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root");
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        SQLiteAsyncConnection db = new SQLiteAsyncConnection("cfg\\AppConfiguration.sqlite", SQLiteOpenFlags.ReadWrite, true);
        public object RequesCount { get; private set; }
        private bool IsLogin = false;

        Style DefaultBtnStyle;

        DogsShow _DogsShow = new DogsShow();
        HallOfFame _HallOfFame = new HallOfFame();
        RegisterDog _RegisterDog = new RegisterDog();
        RegisterAsExpert _RegisterAsExpert = new RegisterAsExpert();
        ShowRequests _ShowRequests = new ShowRequests();
        UserRegister _UserRegister = new UserRegister();

        Dictionary<SelectButton, UserControl> _views = new Dictionary<SelectButton, UserControl>();

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            DataContext = this;
            messageQueue = MessagesSnackbar.MessageQueue;
            
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
            TitleBarPanel.MouseDown += delegate
            {
                DragMove();
            };
            #endregion

            try
            {
                CheckRequests();
                AppConfigMethod();
                setViews();
                SetDefaultContent();
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

        void setViews()
        {
            _views.Add(DogsShowBtn, _DogsShow);
            _views.Add(HallofFameBtn, _HallOfFame);
            _views.Add(SendRegistrDogBtn, _RegisterDog);
            _views.Add(ExpertRegisterBtn, _RegisterAsExpert);
            _views.Add(ShowRequestsBtn, _ShowRequests);
        }

        public void SetDefaultContent()
        {
            DogsShowBtn.Style = FindResource("MaterialDesignRaisedAccentButton") as Style;
            MainWindowContent.Children.Add(_DogsShow);
        }

        private void LoginDialog_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            string username;
            DbConnection.Open();
            if (!String.IsNullOrEmpty(LoginTb.Text) & !String.IsNullOrEmpty(PassTb.Password))
            {
                msc = new MySqlCommand(String.Format("select rights from users where login='{0}' and password='{1}'",
                    LoginTb.Text, Cryptography.getHashSha256(PassTb.Password)), DbConnection);
                if (msc.ExecuteScalar() != null)
                {
                    if ((string)msc.ExecuteScalar() == "organizer")
                    {
                        UserLoginBtn.Content = "Hello " + LoginTb.Text;
                        username = LoginTb.Text;
                        CountingRequestBadge.Visibility = Visibility.Visible;
                        LoginTb.Text = "";
                        PassTb.Password = "";
                        Task.Factory.StartNew(() => messageQueue.Enqueue("Welcome " + username));
                        return;
                    }
                    else
                    {
                        UserLoginBtn.Content = "Hello " + LoginTb.Text;
                        SendRegistrDogBtn.Visibility = Visibility.Visible;
                        username = LoginTb.Text;
                        LoginTb.Text = "";
                        PassTb.Password = "";
                        Task.Factory.StartNew(() => messageQueue.Enqueue("Welcome " + username));
                        return;
                    }
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

        void ContentToggler(UserControl uie)
        {
            MainWindowContent.Children.Clear();
            MainWindowContent.Children.Add(uie);
        }
        private void resetButtons(Button button)
        {
            foreach (var btn in MainMenu.Children.OfType<SelectButton>())
                if (btn != button)
                    btn.Style = DefaultBtnStyle;
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentToggler(_UserRegister);
            resetButtons(sender as Button);
        }

        private void menuButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (sender as SelectButton);
            ContentToggler(_views[button]);
            resetButtons(button);
        }
    }
}
