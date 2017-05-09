using MaterialDesignThemes.Wpf;
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
using MaterialDesignThemes.Wpf.Transitions;

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        MySqlConnection DbConnection = new MySqlConnection("Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root");
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        SQLiteAsyncConnection db = new SQLiteAsyncConnection("cfg\\AppConfiguration.sqlite", SQLiteOpenFlags.ReadWrite, true);
        
        Style DefaultBtnStyle;

        DogsShow _DogsShow = new DogsShow();
        HallOfFame _HallOfFame = new HallOfFame();
        RegisterDog _RegisterDog = new RegisterDog();
        RegisterAsExpert _RegisterAsExpert = new RegisterAsExpert();
        ShowRequests _ShowRequests = new ShowRequests();
        UserRegister _UserRegister = new UserRegister();

        //Dictionary<MenuButton, UserControl> _views = new Dictionary<MenuButton, UserControl>();
        Dictionary<MenuButton, int> _views = new Dictionary<MenuButton, int>();
        public object RequesCount { get; private set; }
        public bool IsLogin = false;
        string username, password;

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

#if DEBUG
            username = "admin";
            password = Cryptography.getHashSha256("admin");
#endif

            UserLoginBtn.Click += delegate
            {
                if ((string)UserLoginBtn.Content != "Login")
                {
                    LoginPanel.Visibility = Visibility.Collapsed;
                    LogoutPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    LoginPanel.Visibility = Visibility.Visible;
                    LogoutPanel.Visibility = Visibility.Collapsed;
                }
            };

            try
            {
                AppConfigMethod();
                setViews();
                SetDefaultContent();
                logining(username, password);
            }
            catch (MySqlException ms)
            {
                Task.Factory.StartNew(() => messageQueue.Enqueue(ms.Message));
            }
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
        void CheckRequests()
        {
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
        }
        
        public void SetDefaultContent()
        {
            DogsShowBtn.Style = FindResource("MaterialDesignRaisedAccentButton") as Style;
            ContentSlider.SelectedIndex = 0;
        }
        void setViews()
        {
            _views.Add(DogsShowBtn, 0);
            _views.Add(HallofFameBtn, 1);
            _views.Add(DogRegisterBtn, 2);
            _views.Add(ExpertRegisterBtn, 3);
            _views.Add(ShowRequestsBtn, 4);
        }
        void contentToggler(int uie)
        {
            ContentSlider.SelectedIndex = uie;
        }
        private void resetButtons(Button button)
        {
            foreach (var btn in MainMenu.Children.OfType<MenuButton>())
                if (btn != button)
                    btn.Style = DefaultBtnStyle;
        }

        void logining(string _username,string _password)
        {
            using (DbConnection)
            {
                msc = new MySqlCommand(String.Format("select rights from users where login='{0}' and password='{1}'", _username, _password), DbConnection);
                DbConnection.Open();
                if (msc.ExecuteScalar() != null)
                {
                    if ((string)msc.ExecuteScalar() == "organizer")
                    {
                        UserLoginBtn.Content = "Hello " + _username;
                        CountingRequestBadge.Visibility = Visibility.Visible;
                        CheckRequests();
                    }
                    else
                    {
                        UserLoginBtn.Content = "Hello " + _username;
                        DogRegisterBtn.Visibility = Visibility.Visible;
                    }
                }
                else
                    Task.Factory.StartNew(() => messageQueue.Enqueue("Login or password is incorect!"));
            }
            LoginTb.Text = "";
            PassTb.Password = "";
            Task.Factory.StartNew(() => messageQueue.Enqueue("Welcome " + _username));
            IsLogin = false;
        }

        private void LoginDialog_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!String.IsNullOrEmpty(LoginTb.Text) & !String.IsNullOrEmpty(PassTb.Password))
            {
                logining(LoginTb.Text, Cryptography.getHashSha256(PassTb.Password));
                return;
            }
            if (IsLogin&&(String.IsNullOrEmpty(LoginTb.Text) || String.IsNullOrEmpty(PassTb.Password)))
                Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not empty!"));
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLogin = true;
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            contentToggler(5);
            resetButtons(sender as Button);
        }
        private void menuButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (sender as MenuButton);
            contentToggler(_views[button]);
            resetButtons(button);
        }

        private void YesOutBtn_Click(object sender, RoutedEventArgs e)
        {
            username = "";
            password = "";
            CountingRequestBadge.Visibility = Visibility.Collapsed;
            DogRegisterBtn.Visibility = Visibility.Collapsed;
            UserLoginBtn.Content = "Login";
        }

    }
}