using System.Windows.Controls;
using MySql.Data.MySqlClient;
using KursovaBD.Utilits;
using System.Collections.Generic;
using System;
using KursovaBD.Models;
using KursovaBD.Controls;

namespace KursovaBD.Views.Pages
{
    public partial class DogsShow : UserControl
    {
        public static DogsShow Instance { get; set; }
        MySqlConnection DbConnection = DbConnector._MySqlConnection();
        MySqlCommand msc;
        MySqlDataReader mdr;
        Dictionary<string, Uri> breeds = new Dictionary<string, Uri>();
        List<DogBattleModel> _dogsBattle = new List<DogBattleModel>();
        public DogsShow()
        {
            InitializeComponent();
            Instance = this;
            setBreeds();
        }
        void setBreeds()
        {
            breeds.Clear();
            breeds.Add("Afghanician Bossia", new Uri("/KursovaBD;component/Assets/DogsBreed/AfghanicianBossia.jpg",UriKind.Relative));
            breeds.Add("Akita Inu", new Uri("/KursovaBD;component/Assets/DogsBreed/AkitaInu.jpg", UriKind.Relative));
            breeds.Add("Border Collie", new Uri("/KursovaBD;component/Assets/DogsBreed/BorderCollie.jpg", UriKind.Relative));
            breeds.Add("Briar", new Uri("/KursovaBD;component/Assets/DogsBreed/Briar.jpg", UriKind.Relative));
            breeds.Add("Brusselsky Griffon", new Uri("/KursovaBD;component/Assets/DogsBreed/BrusselskyGriffon.jpg", UriKind.Relative));
            breeds.Add("Dalmathin", new Uri("/KursovaBD;component/Assets/DogsBreed/Dalmathin.jpg", UriKind.Relative));
            breeds.Add("English Bulldog", new Uri("/KursovaBD;component/Assets/DogsBreed/EnglishBulldog.jpg", UriKind.Relative));
            breeds.Add("English Cocker Spaniel", new Uri("/KursovaBD;component/Assets/DogsBreed/EnglishCockerSpaniel.jpg", UriKind.Relative));
            breeds.Add("Greyhound", new Uri("/KursovaBD;component/Assets/DogsBreed/Greyhound.jpg", UriKind.Relative));
            breeds.Add("Welsh-Corgias", new Uri("/KursovaBD;component/Assets/DogsBreed/Welsh-Corgias.JPG", UriKind.Relative));
        }
        public async void GetInfo()
        {
            _dogsBattle.Clear();
            Rings.Children.Clear();
            using (DbConnection)
            {
                await DbConnection.OpenAsync();
                msc = new MySqlCommand("select id,Breed,Members_id,Date_start,Date_end,Experts_id from dogs_battle",DbConnection);
                mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                while (await mdr.ReadAsync())
                {
                    _dogsBattle.Add(new DogBattleModel
                    {
                        Id = Convert.ToInt32(mdr[0]),
                        Breed = mdr[1] as string,
                        Members_id = mdr[2] as string,
                        Date_start = mdr.GetDateTime(3),
                        Date_end = mdr.GetDateTime(4),
                        Experts_id = mdr[5] as string
                    });
                }
                mdr.Close();
                foreach (var item in _dogsBattle)
                {
                    string[] _m_ids = item.Members_id.Split(',');
                    List<DogModel> members = new List<DogModel>();
                    string[] _e_ids = item.Experts_id.Split(',');
                    List<string> experts = new List<string>();
                    foreach (var v in _m_ids)
                    {
                        msc = new MySqlCommand(String.Format("select Name from dogs where id='{0}'",v),DbConnection);
                        members.Add(new DogModel
                        {
                            Id = Convert.ToInt32(v),
                            NameAge = await msc.ExecuteScalarAsync() as string
                        });
                    }
                    foreach (var v in _e_ids)
                    {
                        msc = new MySqlCommand(String.Format("select Name,Surname from experts where id='{0}'", v), DbConnection);
                        mdr = await msc.ExecuteReaderAsync() as MySqlDataReader;
                        while(await mdr.ReadAsync())
                            experts.Add(String.Format("{0} {1}", mdr[0], mdr[1]));
                        mdr.Close();
                    }
                    Rings.Children.Add(new RingCards(
                        String.Format("Ring {0}: {1}",item.Id.ToString(),item.Breed),
                        breeds[item.Breed],
                        experts,
                        item.Date_start.ToLongDateString(),
                        item.Date_end.ToLongDateString(),
                        members
                        ));
                }
            }
        }
    }
}
