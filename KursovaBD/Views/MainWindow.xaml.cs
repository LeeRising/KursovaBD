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
using System.Windows.Data;
using KursovaBD.Models;
using System.Windows.Input;

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        MySqlConnection DbConnection = Utilits.DbConnector._MySqlConnection();
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        SQLiteAsyncConnection db = new SQLiteAsyncConnection("cfg\\AppConfiguration.sqlite", SQLiteOpenFlags.ReadWrite, true);

        Style DefaultBtnStyle;

        Dictionary<Button, int> _views = new Dictionary<Button, int>();
        public bool IsLogin = false;
        string username, password;

        ICommand btnCommand;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer{Interval = 1};

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            messageQueue = MessagesSnackbar.MessageQueue;

            DefaultBtnStyle = FindResource("MaterialDesignRaisedButton") as Style;
            btnCommand = ExpertPanelBtn.Command;

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
            username = "1";
            password = Cryptography.getHashSha256("1");
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
                appConfigMethod();
                setViews();
                logining(username, password);
                SetDefaultContent();
            }
            catch (MySqlException ms)
            {
                Task.Factory.StartNew(() => messageQueue.Enqueue(ms.Message));
            }
        }

        void appConfigMethod()
        {
            try
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
            catch (Exception)
            {
                Directory.CreateDirectory("cfg");
                appConfigMethod();
            }
        }
        void checkRequests()
        {
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand("select count(id) from dogs where Request='waiting'", DbConnection);
                var _requestCounter = int.Parse(msc.ExecuteScalar().ToString());
                msc = new MySqlCommand("select count(id) from experts where Request='waiting'", DbConnection);
                _requestCounter += int.Parse(msc.ExecuteScalar().ToString());
                if (_requestCounter > 0)
                    CountingRequestBadge.Badge = _requestCounter;
            }
        }
        void expertChecker()
        {
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand(String.Format("select Request from experts where Login_id='{0}'", UserModel.Id), DbConnection);
                if (msc.ExecuteScalar() == null)
                {
                    RegisterAsExpertPanel.Visibility = Visibility.Visible;
                    WaitingAcceptsPanel.Visibility = Visibility.Collapsed;
                    ExpertPanelBtn.Content = "Register as expert";
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Command = btnCommand;
                    ClubNameComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = UserModel.Clubs });
                    DogRegisterBtn.Visibility = Visibility.Visible;
                }
                if ((string)msc.ExecuteScalar() == "waiting")
                {
                    RegisterAsExpertPanel.Visibility = Visibility.Collapsed;
                    WaitingAcceptsPanel.Visibility = Visibility.Visible;
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Command = btnCommand;
                    DogRegisterBtn.Visibility = Visibility.Collapsed;
                }
                if ((string)msc.ExecuteScalar() == "accept")
                {
                    _views.Add(ExpertPanelBtn, 3);
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.Command = null;
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Click += menuButton_Clicked;
                    ExpertPanelBtn.Style = FindResource("MaterialDesignRaisedLightButton") as Style;
                    DogRegisterBtn.Visibility = Visibility.Collapsed;
                }
                if ((string)msc.ExecuteScalar() == "decline")
                {
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.IsEnabled = false;
                    ExpertPanelBtn.Command = null;
                    DogRegisterBtn.Visibility = Visibility.Visible;
                }
            }
        }
        void dogChecker()
        {
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand(String.Format("select Request from dogs where Master_id='{0}'", UserModel.Id), DbConnection);
                switch ((string)msc.ExecuteScalar())
                {
                    case null:
                        DogRegisterBtn.Visibility = Visibility.Visible;
                        ExpertPanelBtn.Visibility = Visibility.Visible;
                        DogRegisterBtn.IsEnabled = true;
                        break;
                    default:
                        DogRegisterBtn.IsEnabled = false;
                        ExpertPanelBtn.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        void logining(string _username, string _password)
        {
            string queary;
            using (DbConnection)
            {
                DbConnection.Open();
                msc = new MySqlCommand(String.Format("select id from users where login='{0}' and password='{1}'", _username, _password), DbConnection);
                UserModel.Id = msc.ExecuteScalar().ToString();
                msc = new MySqlCommand(String.Format("select rights from users where login='{0}' and password='{1}'", _username, _password), DbConnection);
                queary = msc.ExecuteScalar() as string;
            }
            if (queary!= null)
            {
                if (queary == "organizer")
                {
                    UserLoginBtn.Content = "Hello " + _username;
                    CountingRequestBadge.Visibility = Visibility.Visible;
                }
                else
                {
                    UserLoginBtn.Content = "Hello " + _username;
                    expertChecker();
                    dogChecker();
                }
            }
            else
                Task.Factory.StartNew(() => messageQueue.Enqueue("Login or password is incorect!"));
            LoginTb.Text = "";
            PassTb.Password = "";
            Task.Factory.StartNew(() => messageQueue.Enqueue("Welcome " + _username));
            IsLogin = false;
        }
        public void SetDefaultContent()
        {
            DogsShowBtn.Style = FindResource("MaterialDesignRaisedAccentButton") as Style;
            ContentSlider.SelectedIndex = 0;
            timer.Start();
            timer.Tick += (_, __) =>
            {
                if (UserModel.Id == "1")
                    checkRequests();
                else
                {
                    expertChecker();
                    dogChecker();
                }
                timer.Stop();
            };
        }
        void setViews()
        {
            _views.Add(DogsShowBtn, 0);
            _views.Add(HallofFameBtn, 1);
            _views.Add(DogRegisterBtn, 2);
            _views.Add(ExpertPanelBtn, 3);
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
            ShowRequestsBtn.Style = DefaultBtnStyle;
        }

        private void LoginDialog_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!String.IsNullOrEmpty(LoginTb.Text) & !String.IsNullOrEmpty(PassTb.Password))
            {
                logining(LoginTb.Text, Cryptography.getHashSha256(PassTb.Password));
                return;
            }
            if (IsLogin && (String.IsNullOrEmpty(LoginTb.Text) || String.IsNullOrEmpty(PassTb.Password)))
                Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not empty!"));
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            IsLogin = true;
        }
        private void YesOutBtn_Click(object sender, RoutedEventArgs e)
        {
            username = "";
            password = "";
            CountingRequestBadge.Visibility = Visibility.Collapsed;
            DogRegisterBtn.Visibility = Visibility.Collapsed;
            ExpertPanelBtn.Visibility = Visibility.Collapsed;
            UserLoginBtn.Content = "Login";
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            contentToggler(5);
            resetButtons(sender as Button);
        }
        private void menuButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            contentToggler(_views[button]);
            resetButtons(button);
        }

        private void SendRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClubNameComboBox.SelectedValue != null)
            {
                using (DbConnection)
                {
                    DbConnection.Open();
                    msc = new MySqlCommand(String.Format("insert into experts (Login_id,Surname,Name,Club_id,Request) value('{0}','{1}','{2}','{3}','waiting')",
                        UserModel.Id, UserModel.Surname, UserModel.Name, ClubNameComboBox.SelectedIndex + 1), DbConnection);
                    msc.ExecuteNonQuery();
                    Task.Factory.StartNew(() => messageQueue.Enqueue("Succesfull request send"));
                }
                expertChecker();
            }
            else
                Task.Factory.StartNew(() => messageQueue.Enqueue("Please choose your club"));
        }
    }
}