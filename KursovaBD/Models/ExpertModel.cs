﻿namespace KursovaBD.Models
{
    public class ExpertModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string ClubName { get; set; }

        public ExpertModel()
        {
            
        }
    }
}
