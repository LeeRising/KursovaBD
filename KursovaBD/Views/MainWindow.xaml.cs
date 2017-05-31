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
using KursovaBD.Views.Pages;

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

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public static bool IsShowDogsRequest { get; set; }
        public static bool IsShowExpertsRequest { get; set; }

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
                appConfigMethod();
                setViews();
                SetDefaultContent();
                logining(username, password);
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
        async void checkRequests()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select count(id) from dogs where Request='waiting'", DbConnection);
                var _requestCounter = int.Parse((await msc.ExecuteScalarAsync()).ToString());
                IsShowDogsRequest = int.Parse((await msc.ExecuteScalarAsync()).ToString()) == 0 ? false : true;
                msc = new MySqlCommand("select count(id) from experts where Request='waiting'", DbConnection);
                IsShowExpertsRequest = int.Parse((await msc.ExecuteScalarAsync()).ToString()) == 0 ? false : true;
                _requestCounter += int.Parse((await msc.ExecuteScalarAsync()).ToString());
                CountingRequestBadge.Badge = _requestCounter > 0 ? _requestCounter : (object)null;
            }
        }
        async void expertChecker()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select Request from experts where Login='{0}'", UserModel.Login), DbConnection);
                var query = await msc.ExecuteScalarAsync();
                if (query == null)
                {
                    RegisterAsExpertPanel.Visibility = Visibility.Visible;
                    WaitingAcceptsPanel.Visibility = Visibility.Collapsed;
                    ExpertPanelBtn.Content = "Register as expert";
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Command = btnCommand;
                    ClubNameComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = UserModel.Clubs });
                    MyDogBtn.Visibility = Visibility.Visible;
                }
                if ((string)query == "waiting")
                {
                    RegisterAsExpertPanel.Visibility = Visibility.Collapsed;
                    WaitingAcceptsPanel.Visibility = Visibility.Visible;
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Command = btnCommand;
                    MyDogBtn.Visibility = Visibility.Collapsed;
                }
                if ((string)query == "accept")
                {
                    try{_views.Add(ExpertPanelBtn, 3);}catch (Exception){}
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.Command = null;
                    ExpertPanelBtn.IsEnabled = true;
                    ExpertPanelBtn.Click += menuButton_Clicked;
                    ExpertPanelBtn.Style = FindResource("MaterialDesignRaisedLightButton") as Style;
                    MyDogBtn.Visibility = Visibility.Collapsed;
                }
                if ((string)query == "decline")
                {
                    ExpertPanelBtn.Content = "Expert panel";
                    ExpertPanelBtn.IsEnabled = false;
                    ExpertPanelBtn.Command = null;
                    MyDogBtn.Visibility = Visibility.Visible;
                }
            }
        }
        async void dogChecker()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select Request from dogs where Master_id='{0}'", UserModel.Id), DbConnection);                
                switch ((string)await msc.ExecuteScalarAsync())
                {
                    case null:
                        MyDogBtn.Visibility = Visibility.Visible;
                        ExpertPanelBtn.Visibility = Visibility.Visible;
                        MyDogBtn.IsEnabled = true;
                        MyDogBtn.Content = "Register my dog";
                        MyDogBtn.IsEnabled = true;
                        _views.Remove(MyDogBtn);
                        _views.Add(MyDogBtn, 2);
                        RegisterDog.Instance.TimerChecker();
                        break;
                    case "accept":
                        MyDogBtn.Visibility = Visibility.Visible;
                        MyDogBtn.Content = "My dog";
                        MyDogBtn.IsEnabled = true;
                        _views.Remove(MyDogBtn);
                        _views.Add(MyDogBtn, 6);
                        break;
                    default:
                        MyDogBtn.IsEnabled = false;
                        MyDogBtn.Content = "My dog";
                        ExpertPanelBtn.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
        async void clubsAndmastersChecker()
        {
            UserModel.Clubs = new List<string>();
            UserModel.Masters = new List<string>();
            MySqlDataReader mdr;
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select Club_name from clubs", DbConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    UserModel.Clubs.Add(mdr["Club_name"].ToString());
                }
            }
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select Surname,Name from masters", DbConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    UserModel.Masters.Add(mdr["Surname"].ToString() + " " + mdr["Name"].ToString());
                }
            }
            string[] tmp = UserModel.Masters[(int)UserModel.Id - 1].ToString().Split(' ');
            UserModel.Surname = tmp[0];
            UserModel.Name = tmp[1];
        }

        async void logining(string _username, string _password)
        {
            string queary;
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select id from users where login='{0}' and password='{1}'", _username, _password), DbConnection);
                UserModel.Id = await msc.ExecuteScalarAsync()!=null?(Int64)await msc.ExecuteScalarAsync():0;
                msc = new MySqlCommand(String.Format("select rights from users where login='{0}' and password='{1}'", _username, _password), DbConnection);
                queary = await msc.ExecuteScalarAsync() as string;
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
                }
                timer.Start();
                timer.Interval = 1;
                timer.Tick += (_, __) =>
                {
                    timer.Interval = 7000;
                    if (UserModel.Id == 1)
                    {
                        checkRequests();
                    }
                    else
                    {
                        UserModel.Login = _username;
                        dogChecker();
                        expertChecker();
                    }
                    clubsAndmastersChecker();
                };
            }
            else
                await Task.Factory.StartNew(() => messageQueue.Enqueue("Login or password is incorect!"));
            LoginTb.Text = "";
            PassTb.Password = "";
            await Task.Factory.StartNew(() => messageQueue.Enqueue("Welcome " + _username));
            IsLogin = false;
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
            _views.Add(MyDogBtn, 2);
            //_views.Add(ExpertPanelBtn, 3);
            _views.Add(AdminPabelBtn, 4);
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
            AdminPabelBtn.Style = DefaultBtnStyle;
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
            MyDogBtn.Visibility = Visibility.Collapsed;
            ExpertPanelBtn.Visibility = Visibility.Collapsed;
            UserLoginBtn.Content = "Login";
            timer.Stop();
            MyDogBtn.IsEnabled = true;
            SetDefaultContent();
            MyDogBtn.Style = DefaultBtnStyle;
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
            if (button == MyDogBtn)
                MyDogPage.Instance.GetDogInfo();
            if (button == HallofFameBtn)
                HallOfFame.Instance.GetInfo();
            if (button == AdminPabelBtn)
                AdminPanel.Instance.GetInfo();
            if (button == ExpertPanelBtn)
                ExpertPanel.Instance.GetInfo();
        }

        private async void SendRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClubNameComboBox.SelectedValue != null)
            {
                using (DbConnection)
                {
                    await DbConnection.OpenAsync();
                    msc = new MySqlCommand(String.Format("insert into experts (Login_id,Surname,Name,Club_id,Request) value('{0}','{1}','{2}','{3}','waiting')",
                        UserModel.Id, UserModel.Surname, UserModel.Name, ClubNameComboBox.SelectedIndex + 1), DbConnection);
                    await msc.ExecuteNonQueryAsync();
                    await Task.Factory.StartNew(() => messageQueue.Enqueue("Succesfull request send"));
                }
                expertChecker();
            }
            else
                await Task.Factory.StartNew(() => messageQueue.Enqueue("Please choose your club"));
        }
    }
}