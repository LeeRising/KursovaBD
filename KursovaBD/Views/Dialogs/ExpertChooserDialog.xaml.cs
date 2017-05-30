﻿using KursovaBD.Models;
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
    public partial class ExpertChooserDialog : Window
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;

        List<ExpertModel> experts = new List<ExpertModel>();

        public ExpertChooserDialog()
        {
            InitializeComponent();
            GetInfo();
            BackBtn.Click += delegate
            {
                this.Close();
            };
            AddBtn.Click += delegate
            {
                ShowAddExpertPanel.Children.Add(new TextBox
                {
                    Text = Experts.SelectedValue as string,
                    Margin = new Thickness(5,5,0,0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                });
                DogsBattleCreator.Instance._expert_ids.Add(experts[Experts.SelectedIndex].Id);
                experts.RemoveAt(Experts.SelectedIndex);
            };
        }
        public async void GetInfo()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select id,Name,Surname from experts where Request='accept'", DbConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    experts.Add(new ExpertModel
                    {
                        Id = Convert.ToInt32(mdr[0]),
                        Name = (string)mdr[1],
                        Surname = (string)mdr[2]
                    });
                }
                mdr.Close();
            }
            Experts.ItemsSource = experts.Select(x=>x.Name+" "+x.Surname).ToArray();
        }
    }
}
