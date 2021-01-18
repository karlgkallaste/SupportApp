using SupportApp.Data;
using SupportApp.Models.Accounts;

namespace SupportApp.Data
{
    public interface IUserRepository
    {
        User GetByUsernameAndPassword(string username, string password);
        User GetByGoogleId(string googleId);
    }
}