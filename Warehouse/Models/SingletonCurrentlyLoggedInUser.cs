using Warehouse.Models;

namespace Warehouse.Models
{
    public sealed class SingletonCurrentlyLoggedInUser
    {
        private SingletonCurrentlyLoggedInUser() { }
        private static SingletonCurrentlyLoggedInUser _instance = null;
        private static VMLogin login = null;
        private static int userID = 0;
        public static SingletonCurrentlyLoggedInUser GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingletonCurrentlyLoggedInUser();
            }
            return _instance;
        }
        public void setLogin(VMLogin newLogin)
        {
            login = newLogin;
        }
        public void setID(int newID)
        {
            userID = newID;
        }
        public VMLogin getLogin()
        {
            return login;
        }

        public int getUserID()
        {
            return userID;
        }


    }
}

