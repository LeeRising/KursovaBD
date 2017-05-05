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
        private static string connectionString;

        public Connector()
        {
            connectionString = "Database=dogs_show;Data Source=leerain-interactive.sytes.net;User Id=admin;Password=root";
        }
        public MySqlConnection MySqlConnectionMethod()
        {
            return new MySqlConnection(connectionString);
        } 
    }
}
