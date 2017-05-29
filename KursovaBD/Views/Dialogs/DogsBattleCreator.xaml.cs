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
                    msc = new MySqlCommand(String.Format("select Name from dogs where Breed='{0}'", Breed.SelectedValue), DbConnection);
                    mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                    while (await mdr.ReadAsync())
                        _dogs.Add(new DogModel { NameAge = mdr[0] as string });
                    mdr.Close();
                }
                AddDog();
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
