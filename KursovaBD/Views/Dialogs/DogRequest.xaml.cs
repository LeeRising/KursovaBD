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

            };
            Decline.Click += delegate
            {

            };
            this.KeyDown += (s, e) =>
            {
                if (Keyboard.IsKeyDown(Key.F5))
                    dbGeter();
            };
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
                        NameAge = (string)mdr[0]+","+(string)mdr[1],
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
            switcher(element_id=0);
        }
        void switcher(int _element_id)
        {
            this.DataContext = _DogRequestList[_element_id];
        }
    }
}
