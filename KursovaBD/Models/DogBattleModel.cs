using System;

namespace KursovaBD.Models
{
    public class DogBattleModel
    {
        public DogBattleModel()
        {

        }
        public int Id { get; set; }
        public string Breed { get; set; }
        public string Members_id { get; set; }
        public DateTime Date_start { get; set; }
        public DateTime Date_end { get; set; }
        public string Experts_id { get; set; }
    }
}
