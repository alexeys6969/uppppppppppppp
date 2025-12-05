using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Classes
{
    public static class CurrentUser
    {
        public static string ActiveConnectionString { get; set; }

        public static int Id { get; set; }
        public static string Login { get; set; }
        public static string Role { get; set; }

        public static string BuildConnectionString(string dbUser, string dbPassword)
        {
            return $@"Server=LAPTOP3019;Database=music_store;User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";
        }

        public static void Clear()
        {
            Id = 0;
            Login = null;
            Role = null;
        }
    }
}
