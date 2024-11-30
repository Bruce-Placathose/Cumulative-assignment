using MySql.Data.MySqlClient;

namespace Cumulative_assignment.Models
{
    public class SchoolDbContext
    {
        private static string User { get { return "bruce"; } }
        private static string Password { get { return "bruce"; } }
        private static string Database { get { return "schooldb"; } }
        private static string Server { get { return "localhost"; } }
        private static string Port { get { return "3307"; } }

        protected static string ConnectionString
        {
            get
            {
               
                return "server = " + Server
                    + "; user = " + User
                    + "; database = " + Database
                    + "; port = " + Port
                    + "; password = " + Password
                    + "; convert zero datetime = True";
            }
        }
       
        public MySqlConnection AccessDatabase()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}

