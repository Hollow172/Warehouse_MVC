using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Warehouse.Data;
using static System.Reflection.Metadata.BlobBuilder;

namespace Warehouse.Models
{
    public class ReadingUsers
    {
        public static List<VMLogin> ListOfUsers()
        {
            var users = new List<VMLogin>();

            using (StreamReader r = new StreamReader("users.json"))
            {
                string json = r.ReadToEnd();
                users = JsonSerializer.Deserialize<List<VMLogin>>(json);
            }
            return users;
        }

        public static bool AddUser(VMLogin user, WarehouseContext context)
        {
            var users = context.VMLogin.ToList();
            if(user.User ==  null || user.User.Length < 3 || user.Pwd == null || user.Pwd.Length < 3)
            {
                return false;
            }
            foreach(var userInUsers in users)
            {
                if (user.User == userInUsers.User)
                    return false;
            }
            users.Add(user);
            string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions() { WriteIndented = true });
            using (StreamWriter outputFile = new StreamWriter("users.json"))
            {
                outputFile.WriteLine(jsonString);
            }
            return true;
        }

        public static void DeleteUser(VMLogin login)
        {
            var users = ListOfUsers();
            for(int i = 0; i < users.Count; i++)
            {
                if(login.User == users[i].User)
                {
                    users.RemoveAt(i);
                }
            }

            string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions() { WriteIndented = true });
            using (StreamWriter outputFile = new StreamWriter("users.json"))
            {
                outputFile.WriteLine(jsonString);
            }
            return;

        }

    }

    public class VMLogin
    {
        public int id { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
    }
}
