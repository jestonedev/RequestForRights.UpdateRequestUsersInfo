namespace UpdateUsersLogins
{
    public interface ILdapRepository
    {
        User FindUser(string login, string snp, string department, string unit);
    }
}
