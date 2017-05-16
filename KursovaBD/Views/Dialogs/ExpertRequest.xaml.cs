using KursovaBD.Models;
using KursovaBD.Utilits;
using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace KursovaBD.Views.Dialogs
{
    public partial class ExpertRequest : Window
    {
        MySqlConnection _DBConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;
        List<ExpertModel> _ExpertRequestList = new List<ExpertModel>();
        int element_id = 0;
        public ExpertRequest()
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
            TitleBarPanel.MouseDown += delegate
            {
                DragMove();
            };
            #endregion
            
            Previous.Click += delegate
            {
                if (element_id <= _ExpertRequestList.Count - 1)
                    element_id--;
                if (element_id < 0)
                    element_id = _ExpertRequestList.Count - 1;
                switcher(element_id);
            };
            Next.Click += delegate
            {
                if (element_id <= _ExpertRequestList.Count - 1)
                    element_id++;
                if (element_id > _ExpertRequestList.Count - 1)
                    element_id = 0;
                switcher(element_id);
            };
            Accept.Click += delegate
            {
                requestAnswer("accept", null);
            };
            Decline.Click += delegate
            {
                DeclineReasonPanel.Visibility = Visibility.Visible;
                NoneRequestPanel.Visibility = Visibility.Collapsed;
            };
            this.KeyDown += delegate
            {
                if (Keyboard.IsKeyDown(Key.F5))
                    dbGeter();
            };
            CloseThisBtn.Click += delegate
            {
                this.Close();
            };
            dbGeter();
        }
        async void requestAnswer(string _ans, string _declineReason)
        {
            using (_DBConnection)
            {
                await _DBConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("update experts set Request='{0}',Decline_reason='{1}' where Login='{2}'",
                    _ans, _declineReason, _ExpertRequestList[element_id].Login), _DBConnection);
                await msc.ExecuteNonQueryAsync();
            }
            dbGeter();
        }
        async void dbGeter()
        {
            _ExpertRequestList.Clear();
            using (_DBConnection)
            {
                await _DBConnection.OpenAsync();
                msc = new MySqlCommand("select Login,Club_id,Name,Surname from experts where Request='waiting'", _DBConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    _ExpertRequestList.Add(new ExpertModel
                    {
                        Login = (string)mdr[0],
                        ClubName = UserModel.Clubs[Convert.ToInt32(mdr[1]) - 1],
                        Name = (string)mdr[2],
                        Surname = (string)mdr[3]
                    });
                }
            }
            if (_ExpertRequestList.Count == 0)
            {
                ExpertRequestDialog.IsOpen = true;
                NoneRequestPanel.Visibility = Visibility.Visible;
                DeclineReasonPanel.Visibility = Visibility.Collapsed;
            }
            else
                switcher(element_id = 0);
        }
        void switcher(int _element_id)
        {
            if (_ExpertRequestList.Count > 0)
                this.DataContext = _ExpertRequestList[_element_id];
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(DeclineTb.Text))
            {
                requestAnswer("decline", DeclineTb.Text);
                DeclineTb.Text = "";
            }
            else
                ExpertRequestDialog.IsOpen = true;
        }
    }
}
