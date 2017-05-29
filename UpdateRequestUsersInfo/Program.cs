using System.Configuration;

namespace UpdateUsersLogins
{
    internal class Program
    {
        private static void Main()
        {
            ILogger logger = new ConsoleLogger();
            
            var ldapUsername = ConfigurationManager.AppSettings["ldap_username"];
            var ldapPassword = ConfigurationManager.AppSettings["ldap_password"];
            ILdapRepository ldapRepository = new LdapRepository(ldapUsername, ldapPassword, logger);

            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            IDatabaseRepository databaseRepository = new DatabaseRepository(connectionString, logger);

            var users = databaseRepository.GetUsers();
            foreach (var user in users)
            {
                logger.Log(string.Format("IdRequestUser: {0}", user.IdUser), LogLevel.Notice);
                var ldapUser = ldapRepository.FindUser(user.Login, user.Snp, user.Department, user.Unit);
                if (ldapUser == null || user == ldapUser) continue;
                if (user.Department != ldapUser.Department || user.Unit != ldapUser.Unit) continue;
                var result = databaseRepository.UpdateUser(user.IdUser, ldapUser);
                if (!result)
                {
                    logger.Log("User was not changed", LogLevel.Warning);
                }
            }
        }
    }
}
