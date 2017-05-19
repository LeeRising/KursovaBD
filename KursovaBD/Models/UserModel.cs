using System;
using System.Collections.Generic;

namespace KursovaBD.Models
{
    public static class UserModel
    {
        public static Int64 Id { get; set; }
        public static string Name { get; set; }
        public static string Surname { get; set; }
        public static string Login { get; set; }
        public static List<string> Clubs = new List<string>();
        public static List<string> Masters = new List<string>();
    }
}
