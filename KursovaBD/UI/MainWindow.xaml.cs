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

namespace KursovaBD
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        }
        private void LoginAsAdminDialog_Closing(object sender, DialogClosingEventArgs eventArgs)
        {
            //try
            //{
            //    Connector.msc().Open();
            //    MessageBox.Show("1");
            //}
            //catch (MySqlException ms)
            //{
            //    MessageBox.Show(ms.Message);
            //}
        }
    }
}
