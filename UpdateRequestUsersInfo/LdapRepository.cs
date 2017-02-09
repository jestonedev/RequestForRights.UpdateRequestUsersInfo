using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;

namespace UpdateUsersLogins
{
    public sealed class LdapRepository : ILdapRepository
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly ILogger _logger;

        public LdapRepository(string userName, string password, ILogger logger)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            _userName = userName;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            _password = password;
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }
        private static IEnumerable<string> GetDomains()
        {
            return from Domain domain in Forest.GetCurrentForest().Domains
                   select domain.Name;
        }

        public User FindUser(string login, string snp, string department, string unit)
        {
            _logger.Log(string.Format("Searching user {0} : {1} : {2} : {3}",
                login, snp, department, unit), LogLevel.Notice);
            foreach (var domainName in GetDomains())
            {
                var context = new DirectoryContext(
                    DirectoryContextType.Domain, domainName, _userName, _password);
                using (var domain = Domain.GetDomain(context))
                using (var domainEntry = domain.GetDirectoryEntry())
                {
                    var domainLoginPrefix = domain.Name.Split('.')[0];
                    using (var searcher = new DirectorySearcher())
                    {
                        searcher.SearchRoot = domainEntry;
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.PropertiesToLoad.Add("displayName");
                        searcher.PropertiesToLoad.Add("samAccountName");
                        searcher.PropertiesToLoad.Add("title");
                        searcher.PropertiesToLoad.Add("company");
                        searcher.PropertiesToLoad.Add("department");
                        searcher.PropertiesToLoad.Add("physicaldeliveryofficename");
                        searcher.PropertiesToLoad.Add("telephonenumber");
                        if (!string.IsNullOrEmpty(login))
                        {
                            var loginParts = login.Split('\\');
                            searcher.Filter = string.Format(CultureInfo.InvariantCulture,
                                "(&(objectClass=user)(objectClass=person)(samAccountName={0}))",
                                loginParts[loginParts.Length - 1]);
                        }
                        else if (!string.IsNullOrEmpty(department) && !string.IsNullOrEmpty(snp))
                        {
                            searcher.Filter = string.Format(CultureInfo.InvariantCulture,
                                "(&(objectClass=user)(objectClass=person)(displayName={0})(company={1}){2}(!(useraccountcontrol:1.2.840.113556.1.4.803:=2)))",
                                snp, department,
                                string.IsNullOrEmpty(unit) ? "" : string.Format("(department={0})", unit));
                        }
                        else
                        {
                            return null;
                        }
                        var results = searcher.FindAll();
                        if (results.Count == 0)
                        {
                            continue;
                        }
                        if (results.Count > 1)
                        {
                            return null;
                        }
                        var result = results[0];
                        var user = new User
                        {
                            Snp = GetValue(result.Properties, "displayName"),
                            Login =
                                domainLoginPrefix + "\\" +
                                GetValue(result.Properties, "samAccountName").ToLower(),
                            Post = GetValue(result.Properties, "title"),
                            Department = GetValue(result.Properties, "company"),
                            Unit = GetValue(result.Properties, "department"),
                            Office = GetValue(result.Properties, "physicaldeliveryofficename"),
                            Phone = GetValue(result.Properties, "telephonenumber")
                        };
                        return user;
                    }
                }
            }
            return null;
        }

        private static string GetValue(ResultPropertyCollection properties, string parameter)
        {
            return properties.Contains(parameter)
                ? properties[parameter][0].ToString()
                : null;
        }
    }
}
