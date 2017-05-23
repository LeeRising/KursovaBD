using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using KursovaBD.Models;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace KursovaBD.Views.Pages
{
    public partial class HallOfFame : UserControl
    {
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        List<DogModel> _DogModel = new List<DogModel>();
        Uri _no_image = new Uri("pack://application:,,,/KursovaBD;component/Assets/No_image.png");
        public static HallOfFame Instance { get; set; }
        public HallOfFame()
        {
            InitializeComponent();
            Instance = this;
        }
        async public void GetInfo()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("SELECT Name,Photo,Medals_count FROM dogs ORDER BY Medals_count DESC LIMIT 3", DbConnection);
                MySqlDataReader mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                _DogModel.Clear();
                while (await mdr.ReadAsync())
                {
                    _DogModel.Add(new DogModel((string)mdr["Name"],
                        (string)mdr["Photo"] == "No_image.png" ? _no_image : new Uri("http://kursova.sytes.net/" + mdr["Photo"] as string),
                        (int)mdr["Medals_count"]));
                }
                //TopClubsDataGrid. = ;
            }
            FirstPlaceName.Text = _DogModel[0].NameAge + "," + Convert.ToString(_DogModel[0].MedalsCount);
            FirstPlaceImage.Source = new BitmapImage(_DogModel[0].PhotoUrl);
            SecondPlaceName.Text = _DogModel[1].NameAge + "," + Convert.ToString(_DogModel[1].MedalsCount);
            SecondPlaceImage.Source = new BitmapImage(_DogModel[1].PhotoUrl);
            ThirdPlaceName.Text = _DogModel[2].NameAge + "," + Convert.ToString(_DogModel[2].MedalsCount);
            ThirdPlaceImage.Source = new BitmapImage(_DogModel[2].PhotoUrl);
        }
    }
}
