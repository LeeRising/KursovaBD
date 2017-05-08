using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MaterialDesignThemes.Wpf;

namespace KursovaBD.UI.Pages
{
    public partial class UserRegister : UserControl
    {
        MySqlConnection DbConnection = new MySqlConnection("Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root");
        MySqlCommand msc;
        SnackbarMessageQueue messageQueue;
        void LoginCheckerMethod()
        {
            if (!String.IsNullOrEmpty(LoginTb.Text))
            {
                DbConnection.Open();
                LoginChecker.Visibility = Visibility.Visible;
                msc = new MySqlCommand(String.Format("select id from users where login='{0}'", LoginTb.Text), DbConnection);
                if (msc.ExecuteScalar() != null)
                {
                    Task.Factory.StartNew(() => messageQueue.Enqueue("This login is already taken!"));
                    LoginChecker.Kind = PackIconKind.CloseCircle;
                }
                else
                    LoginChecker.Kind = PackIconKind.CheckCircle;
                DbConnection.Close();
            }
        }
        public UserRegister()
        {
            InitializeComponent();

            messageQueue = MessagesSnackbar.MessageQueue;

            LoginTb.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Tab)
                {
                    LoginCheckerMethod();
                }
            };
            LoginTb.TextChanged += delegate
            {
                if (!String.IsNullOrEmpty(LoginTb.Text))
                    LoginTb.Width = 310;
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
                LoginCheckerMethod();
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
                    if(!String.IsNullOrEmpty(NameTb.Text) &&
                    !String.IsNullOrEmpty(SurnameTb.Text) &&
                    !String.IsNullOrEmpty(FathernameTb.Text) &&
                    !String.IsNullOrEmpty(PassportTb.Text))
                    {
                        DbConnection.Open();
                        msc = new MySqlCommand(String.Format("insert into users (login,password,rights) values('{0}','{1}','user')", LoginTb.Text,PassTb.Password), DbConnection);
                        msc.ExecuteReader();
                        DbConnection.Close();
                        DbConnection.Open();
                        msc = new MySqlCommand(String.Format("insert into masters (Surname,Name,Fathername,Passport_info) values('{0}','{1}','{2}','{3}')", NameTb.Text, SurnameTb.Text, FathernameTb.Text, PassportTb.Text), DbConnection);
                        msc.ExecuteReader();
                        DbConnection.Close();
                        Task.Factory.StartNew(() => messageQueue.Enqueue("New user was created!"));
                    }
                    else
                        Task.Factory.StartNew(() => messageQueue.Enqueue("All fields must be not emtpy!"));
                }
            };
        }
    }
}
