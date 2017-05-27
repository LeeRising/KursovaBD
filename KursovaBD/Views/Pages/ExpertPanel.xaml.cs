using System.Windows.Controls;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using KursovaBD.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using KursovaBD.Controls;

namespace KursovaBD.Views.Pages
{
    public partial class ExpertPanel : UserControl
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;
        string[] members_id;
        string date_start;
        List<DogModel> _dogModel = new List<DogModel>();
        Uri _no_image = new Uri("pack://application:,,,/KursovaBD;component/Assets/No_image.png");

        public static ExpertPanel Instance { get; set; }

        public ExpertPanel()
        {
            InitializeComponent();
            Instance = this;
        }

        public async void GetInfo()
        {
            _dogModel.Clear();
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select Ring_id from experts where Login='{0}'", UserModel.Login), DbConnection);
                int? ring_id = Convert.ToInt32(await msc.ExecuteScalarAsync());
                if (ring_id!=null)
                {
                    msc = new MySqlCommand(String.Format("select Members_id,Date_start from dogs_battle where id='{0}'", ring_id), DbConnection);
                    mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                    while(await mdr.ReadAsync())
                    {
                        members_id = (mdr["Members_id"] as string).Split(',');
                        date_start = mdr["Date_start"] as string;
                    }
                    mdr.Close();
                    foreach (var item in members_id)
                    {
                        msc = new MySqlCommand(String.Format("select Club_id,Name,Breed,Photo from dogs where id='{0}'", item), DbConnection);
                        mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                        await mdr.ReadAsync();
                        _dogModel.Add(new DogModel
                        {
                            NameAge = mdr["Name"] as string,
                            ClubName = UserModel.Clubs[Convert.ToInt32(mdr["Club_id"])],
                            Breed = mdr["Breed"] as string,
                            PhotoUrl = (string)mdr["Photo"] == "No_image.png" ? _no_image : new Uri("http://kursova.sytes.net/" + mdr["Photo"] as string)
                        });
                        mdr.Close();
                    }
                }
            }
            DogsPanel.Children.Clear();
            foreach (var item in _dogModel)
            {
                DogsPanel.Children.Add(new DogCard(item.NameAge,item.ClubName,item.PhotoUrl));
            }
        }
    }
}
