using System;

namespace KursovaBD.Models
{
    public class DogModel
    {
        public string NameAge { get; set; }
        public string ClubName { get; set; }
        public string Breed { get; set; }
        public string DocumentInfo { get; set; }
        public string DateLastVaccenation { get; set; }
        public string MasterName { get; set; }
        public string About { get; set; }
        public Uri PhotoUrl { get; set; }

        public DogModel()
        {

        }
    }
}
