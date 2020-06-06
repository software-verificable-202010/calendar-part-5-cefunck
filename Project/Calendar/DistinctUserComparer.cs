using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    class DistinctUserComparer : IEqualityComparer<User>
    {
        public bool Equals(User aUser, User otherUser)
        {
            return aUser.Name == otherUser.Name;
        }

        public int GetHashCode(User user)
        {
            return user.Name.GetHashCode();
        }
    }
}
