using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using MaterialDesignThemes.Wpf;

namespace KursovaBD.Views.Pages
{
    public partial class UserRegister : UserControl
    {
        MySqlConnection DbConnection = Utilits.DbConnector._MySqlConnection();
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        public UserRegister()
        {
            InitializeComponent();

            messageQueue = MessagesSnackbar.MessageQueue;
            
            LoginTb.TextChanged += delegate
            {
                if (!String.IsNullOrEmpty(LoginTb.Text))
                {
                    LoginCheckerMethod();
                    LoginTb.Width = 310;
                }
                else
                {
                    LoginTb.Width = 340;
                    LoginChecker.Visibility = Visibility.Collapsed;
                }
            };
            RepeatPassTb.PasswordChanged += (s, e) =>
            {
                if (!String.IsNullOrEmpty(RepeatPassTb.Password) && !String.IsNullOrEmpty(PassTb.Password))
                {
                    RepeatPassTb.Width = 310;
                    PassChecker.Visibility = Visibility.Visible;
                    if (PassTb.Password != RepeatPassTb.Password)
                    {
                        PassChecker.Kind = PackIconKind.CloseCircle;
                    }
                    else
                        PassChecker.Kind = PackIconKind.CheckCircle;
                }
                else
                {
                    RepeatPassTb.Width = 340;
                    PassChecker.Visibility = Visibility.Collapsed;
                }
            };
            PassTb.PasswordChanged += (s, e) =>
            {
                if (!String.IsNullOrEmpty(RepeatPassTb.Password) && !String.IsNullOrEmpty(PassTb.Password))
                {
                    LoginChecker.Visibility = Visibility.Visible;
                    if (PassTb.Password != RepeatPassTb.Password)
                    {
                        PassChecker.Kind = PackIconKind.CloseCircle;
                    }
                    else
                        PassChecker.Kind = PackIconKind.CheckCircle;
                }
            };

            RegisterBtn.Click += delegate
            {
                if (PassChecker.Kind == PackIconKind.CheckCircle && LoginChecker.Kind == PackIconKind.CheckCircle)
                {
                    if (!String.IsNullOrEmpty(NameTb.Text) &&
                    !String.IsNullOrEmpty(SurnameTb.Text) &&
                    !String.IsNullOrEmpty(FathernameTb.Text) &&
                    !String.IsNullOrEmpty(PassportTb.Text))
                    {
                        using (DbConnection)
                        {
                            DbConnection.Open();
                            msc = new MySqlCommand(String.Format("insert into users (login,password,rights) values('{0}','{1}','user')", LoginTb.Text, Cryptography.getHashSha256(PassTb.Password)), DbConnection);
                            msc.ExecuteScalar();
                            msc = new MySqlCommand(String.Format("insert into masters (Surname,Name,Fathername,Passport_info) values('{0}','{1}','{2}','{3}')", NameTb.Text, SurnameTb.Text, FathernameTb.Text, PassportTb.Text), DbConnection);
                            msc.ExecuteScalar();
                        }
                        Task.Factory.StartNew(() => messageQueue.Enqueue("New user was created!"));
                        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer
                        {
                            Enabled = true,
                            Interval = 1500
                        };
                        t.Start();
                        t.Tick += delegate
                        {
                            MainWindow.Instance.SetDefaultContent();
                        };
                    }
                    else
                        Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not emtpy!"));
                }
            };
            CancelBtn.Click += (sender, obj) =>
            {
                MainWindow.Instance.SetDefaultContent();
            };
        }
        async void LoginCheckerMethod()
        {
            if (!String.IsNullOrEmpty(LoginTb.Text))
            {
                using (DbConnection)
                {
                    await DbConnection.OpenAsync();
                    LoginChecker.Visibility = Visibility.Visible;
                    msc = new MySqlCommand(String.Format("select id from users where login='{0}'", LoginTb.Text), DbConnection);
                    if (await msc.ExecuteScalarAsync() != null)
                    {
                        await Task.Factory.StartNew(() => messageQueue.Enqueue("This login is already taken!"));
                        LoginChecker.Kind = PackIconKind.CloseCircle;
                    }
                    else
                        LoginChecker.Kind = PackIconKind.CheckCircle;
                }
            }
        }
    }
}
