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
using KursovaBD.Tools.BDStructure;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using KursovaBD.Tools;

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        Connector MySqlConnector = new Connector();
        SnackbarMessageQueue messageQueue;
        MySqlCommand msc;
        public int Count => 6;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            messageQueue = MessagesSnackbar.MessageQueue;
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
                //CheckRequests();
            }
            catch (MySqlException ms)
            {
                Task.Factory.StartNew(() => messageQueue.Enqueue(ms.Message));
            }
            SendRegistrDogBtn.Click += delegate
            {

            };
            CheckRequests();
        }
        void CheckRequests()
        {
            //MySqlConnection mc = MySqlConnector.MySqlConnectionMethod();
            //mc.Open();
            //msc = new MySqlCommand("select count(id) from requests",mc);
            //var v = int.Parse(msc.ExecuteScalar().ToString());
            //if (v != 0)
            //    CountingRequestBadge.Badge = (object)v;
            //else
            //    CountingRequestBadge.Badge = null;
            //mc.Close();
        }

        private void LoginDialog_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!String.IsNullOrEmpty(LoginTb.Text) & !String.IsNullOrEmpty(PassTb.Password))
            {
                MySqlConnection mc = MySqlConnector.MySqlConnectionMethod();
                mc.Open();
                msc = new MySqlCommand(String.Format("select * from organizer where login='{0}' and password='{1}'",
                    LoginTb.Text, Cryptography.getHashSha256(PassTb.Password)), mc);
                if (msc.ExecuteScalar() != null)
                {
                        Task.Factory.StartNew(() => messageQueue.Enqueue("Hello " + LoginTb.Text));
                        LoginBtn.Content = "Hello " + LoginTb.Text;
                        CountingRequestBadge.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
