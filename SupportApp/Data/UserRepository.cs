using System.Collections.Generic;
using System.Linq;
using SupportApp.Data;
using SupportApp.Models.Accounts;


namespace ConfArch.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static List<User> users = new List<User>
        {
            new User { Id = 3522, Name = "roland", Password = "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=",
                FavoriteColor = "blue", Role = "Admin", GoogleId = "101517359495305583936" }
        };

        public static User GetByUsernameAndPassword(string username, string password)
        {
            var user = users.SingleOrDefault(u => u.Name == username &&
                                                  u.Password == password.Sha256());
            return user;
        }

        User IUserRepository.GetByUsernameAndPassword(string username, string password)
        {
            return GetByUsernameAndPassword(username, password);
        }

        public User GetByGoogleId(string googleId)
        {
            var user = users.SingleOrDefault(u => u.GoogleId == googleId);
            return user;
        }
    }
}