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
            if (aUser is null)
            {
                throw new ArgumentNullException(nameof(aUser));
            }

            if (otherUser is null)
            {
                throw new ArgumentNullException(nameof(otherUser));
            }

            return aUser.Name == otherUser.Name;
        }

        public int GetHashCode(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Name.GetHashCode();
        }
    }
}
