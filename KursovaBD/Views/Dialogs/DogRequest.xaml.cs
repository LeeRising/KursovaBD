using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using KursovaBD.Models;
using System.Windows.Input;

namespace KursovaBD.Views.Dialogs
{
    public partial class DogRequest : Window
    {
        MySqlConnection _DBConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;
        List<DogModel> _DogRequestList = new List<DogModel>();
        int element_id = 0;
        private string urlLink => "http://kursova.sytes.net/";
        public DogRequest()
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
                if(element_id <= _DogRequestList.Count-1)
                    element_id--;
                if (element_id < 0)
                    element_id = _DogRequestList.Count - 1;
                switcher(element_id);
            };
            Next.Click += delegate
            {
                if (element_id <= _DogRequestList.Count - 1)
                    element_id++;
                if (element_id > _DogRequestList.Count - 1)
                    element_id = 0;
                switcher(element_id);
            };
            Accept.Click += delegate
            {
                requestAnswer("accept",null);
            };
            Decline.Click += delegate
            {
                DeclineReasonPanel.Visibility = Visibility.Visible;
                NoneRequestPanel.Visibility = Visibility.Collapsed;
            };
            this.KeyDown +=delegate
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
        async void requestAnswer(string _ans,string _declineReason)
        {
            using (_DBConnection)
            {
                await _DBConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("update dogs set Request='{0}',Decline_reason='{1}' where Master_id='{2}'",
                    _ans, _declineReason, UserModel.Masters.IndexOf(_DogRequestList[element_id].MasterName)+1),_DBConnection);
                await msc.ExecuteNonQueryAsync();
            }
            dbGeter();
        }
        async void dbGeter()
        {
            _DogRequestList.Clear();
            using (_DBConnection)
            {
                await _DBConnection.OpenAsync();
                msc = new MySqlCommand("select Name,Age,Club_id,Breed,Document_info,Date_last_vaccenation,Master_id,About,Photo from dogs where Request='waiting'",_DBConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while(await mdr.ReadAsync())
                {
                    _DogRequestList.Add(new DogModel
                    {
                        NameAge = (string)mdr[0]+","+mdr[1].ToString(),
                        ClubName = UserModel.Clubs[Convert.ToInt32(mdr[2]) - 1],
                        Breed = (string)mdr[3],
                        DocumentInfo = (string)mdr[4],
                        DateLastVaccenation = mdr[5].ToString().Split(' ')[0],
                        MasterName = UserModel.Masters[Convert.ToInt32(mdr[6]) - 1],
                        About = mdr[7] as string ?? "",
                        PhotoUrl = (string)mdr[8]=="No_image.png" ? new Uri("pack://application:,,,/Assets/No_image.png") : new Uri(urlLink + (string)mdr[8])
                    }); 
                }
            }
            if (_DogRequestList.Count == 0)
            {
                DogRequestDialog.IsOpen = true;
                NoneRequestPanel.Visibility = Visibility.Visible;
                DeclineReasonPanel.Visibility = Visibility.Collapsed;
            }
            else
                switcher(element_id=0);
        }
        void switcher(int _element_id)
        {
            if(_DogRequestList.Count>0)
                this.DataContext = _DogRequestList[_element_id];
        }
        
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(DeclineTb.Text))
            {
                requestAnswer("decline", DeclineTb.Text);
                DeclineTb.Text = "";
            }
            else
                DogRequestDialog.IsOpen = true;
        }
    }
}
