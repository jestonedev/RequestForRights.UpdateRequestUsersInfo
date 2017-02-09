using System.Collections.Generic;

namespace UpdateUsersLogins
{
    public interface IDatabaseRepository
    {
        bool UpdateUser(int idUser, User userInfo);
        IEnumerable<User> GetUsers();
    }
}
