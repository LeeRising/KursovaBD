using MySql.Data.MySqlClient;

namespace KursovaBD.Utilits
{
    public static class DbConnector
    {
        public static MySqlConnection _MySqlConnection()
        {
            return new MySqlConnection("Database=dogs_show;Data Source=195.69.247.115;User Id=admin;Password=root;Convert Zero Datetime=True");            
        }
    }
}
