using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public enum TypesOfUsers
    {
        AllUsers = 1,
        SpecificUser = 2
    }

    public class UserType
    {
        public static string AllUsers = "All Users";
        public static string JustMine = "Mine";

        public static IList<UserType> GetUserTypes()
        {
            var userTypes = new List<UserType>();

            userTypes.Add(new UserType { UserTypeId = (int)TypesOfUsers.AllUsers, Description = AllUsers });
            userTypes.Add(new UserType { UserTypeId = (int)TypesOfUsers.SpecificUser, Description = JustMine });

            return userTypes;
        }

        public int UserTypeId { get; set; }
        public string Description { get; set; }
    }
}
