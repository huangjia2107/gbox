using System.Collections.ObjectModel;

namespace G_Box.Server
{
    public class ConnViewModel
    {
        ObservableCollection<User> _allUsers;

        public ObservableCollection<User> AllUsers
        {
            get
            {
                if (_allUsers == null)
                    _allUsers = new ObservableCollection<User>();

                return _allUsers;
            }
        }

        public bool IsExist(User user)
        {
            foreach (User u in AllUsers)
            {
                if (u.IPAndPort == user.IPAndPort)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
