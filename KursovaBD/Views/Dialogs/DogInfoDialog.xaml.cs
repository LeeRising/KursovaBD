using KursovaBD.Models;
using KursovaBD.Utilits;
using MaterialDesignThemes.Wpf;
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
    public partial class DogInfoDialog : Window
    {
        private string name { get; set; }
        private int id { get; set; }
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        private ImageSource _no_image;

        public DogInfoDialog(string _name,int _id)
        {
            InitializeComponent();
            #region TitleBar
            CloseBtn.Click += delegate
            {
                Close();
            };
            TitleBarPanel.MouseDown += delegate
            {
                DragMove();
            };
            #endregion
            Header.Text = "About " + _name;
            this.name = _name;
            this.id = _id;
            _no_image = DogAvatar.Source;
            SetInfo();
        }
        async void SetInfo()
        {
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand(String.Format("select Club_id,Name,Breed,Age,Document_info,Parents_name,Date_last_vaccenation,Photo,About from dogs where id='{0}'", id), DbConnection);
                MySqlDataReader mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    DogAvatar.Source = mdr["Photo"] as string == "No_image.png" ? _no_image : new BitmapImage(new Uri("http://kursova.sytes.net/" + mdr["Photo"] as string));
                    int _id = Convert.ToInt32(mdr[0]);
                    ClubName.Text = UserModel.Clubs[_id--];
                    DogNameAge.Text = (string)mdr[1] + "," + Convert.ToString(mdr[3]);
                    Breed.Text = (string)mdr[2];
                    Document.Text = (string)mdr[4];
                    if (mdr[8] as string != null)
                        HintAssist.SetHint(About, "About dog");
                    else
                        HintAssist.SetHint(About, "Write about our dog");
                    About.Text = mdr[8] as string ?? "";
                    LastVaccenationDate.Text = mdr[6].ToString().Split(' ')[0];
                    ParentsName.Text = (string)mdr[5];
                }
            }
        }
    }
}
