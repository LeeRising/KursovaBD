using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace KursovaBD.Tools.BDStructure
{
    public class Connector
    {
        private string server;
        private string database;
        private string password;
        private string uid;
        private static string connectionString;

        public Connector()
        {
            server = "leerain-interactive.sytes.net";
            database = "dogs_show";
            uid = "admin";
            password = "root";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        }
        public static MySqlConnection msc()
        {
            return new MySqlConnection(connectionString);
        } 
    }
}
