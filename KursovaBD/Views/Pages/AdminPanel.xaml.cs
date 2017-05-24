using System.Windows.Controls;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using System.Data;
using KursovaBD.Views.Dialogs;

namespace KursovaBD.Views.Pages
{
    public partial class AdminPanel : UserControl
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataAdapter mda;
        DataSet ds;
        public static AdminPanel Instance { get; set; }
        public AdminPanel()
        {
            InitializeComponent();
            Instance = this;
            DogsRequestBtn.Click += delegate
            {
                DogRequest dr = new DogRequest();
                MainWindow.Instance.Hide();
                dr.ShowDialog();
                MainWindow.Instance.Show();
            };
            ExpertsRequestBtn.Click += delegate
            {
                ExpertRequest er = new ExpertRequest();
                MainWindow.Instance.Hide();
                er.ShowDialog();
                MainWindow.Instance.Show();
            };
        }
        public async void GetInfo()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                ds = new DataSet();
                msc = new MySqlCommand("select * from dogs_battle",DbConnection);
                mda = new MySqlDataAdapter(msc);
                mda.Fill(ds, "DogsBattleBinder");
                DogsBattleDataGrid.DataContext = ds;

                ds = new DataSet();
                msc = new MySqlCommand("select * from dogs", DbConnection);
                mda = new MySqlDataAdapter(msc);
                mda.Fill(ds, "DogsBinder");
                DogsDataGrid.DataContext = ds;

                ds = new DataSet();
                msc = new MySqlCommand("select * from experts", DbConnection);
                mda = new MySqlDataAdapter(msc);
                mda.Fill(ds, "ExpertsBinder");
                ExpertsDataGrid.DataContext = ds;

                ds = new DataSet();
                msc = new MySqlCommand("select * from masters", DbConnection);
                mda = new MySqlDataAdapter(msc);
                mda.Fill(ds, "MastersBinder");
                MastersDataGrid.DataContext = ds;

                ds = new DataSet();
                msc = new MySqlCommand("select * from clubs", DbConnection);
                mda = new MySqlDataAdapter(msc);
                mda.Fill(ds, "ClubsBinder");
                ClubsDataGrid.DataContext = ds;
            }
            ButtonStateChange();
        }
        public void ButtonStateChange()
        {
            DogsRequestBtn.IsEnabled = MainWindow.IsShowDogsRequest ? true : false;
            ExpertsRequestBtn.IsEnabled = MainWindow.IsShowExpertsRequest ? true : false;
        }
    }
}
