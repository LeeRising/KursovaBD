using KursovaBD.Controls;
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
    public partial class DogsBattleCreator : Window
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;
        public List<DogModel> _dogs = new List<DogModel>();
        List<string> _breed = new List<string>();
        public List<int> _expert_ids = new List<int>();
        public static DogsBattleCreator Instance { get; set; }

        public DogsBattleCreator()
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
            Instance = this;
            Breed.SelectionChanged += async delegate
            {
                MemderAddPanel.Children.Clear();
                _dogs.Clear();
                using (DbConnection)
                {
                    await DbConnection.OpenAsync();
                    msc = new MySqlCommand(String.Format("select id,Name from dogs where Breed='{0}'", Breed.SelectedValue), DbConnection);
                    mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                    while (await mdr.ReadAsync())
                        _dogs.Add(new DogModel { Id = Convert.ToInt32(mdr[0]), NameAge = mdr[1] as string });
                    mdr.Close();
                }
                AddDog();
            };
            ExpertAddBttn.Click += delegate
            {
                ExpertChooserDialog ecd = new ExpertChooserDialog();
                ecd.ShowDialog();
            };
            CreateBattleBtn.Click += async delegate
            {
                using (DbConnection)
                {
                    await DbConnection.OpenAsync();
                    string s = "",s1="";
                    foreach (var item in _dogs.Select(x => x.Id).ToArray())
                    {
                        s += item.ToString() + ",";
                    }
                    s = s.Substring(0, s.Length - 1);
                    foreach (var item in _expert_ids)
                    {
                        s1 += item.ToString() + ",";
                    }
                    s1 = s1.Substring(0, s1.Length - 1);
                    msc = new MySqlCommand(String.Format("insert into dogs_battle Breed,Members_id,Date_start,Experts_id values ('{0}','{1}','{2}','{3}')", 
                        Breed.SelectedValue, s,DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS"),s1), DbConnection);
                    await msc.ExecuteNonQueryAsync();
                }
            };
        }

        public async void GetInfo()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select distinct Breed from dogs", DbConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                    _breed.Add(mdr[0] as string);
                mdr.Close();
            }
            Breed.ItemsSource = _breed;
        }

        public void AddDog()
        {
            MemderAddPanel.Children.Add(new NewDogElement());
        }
    }
}
