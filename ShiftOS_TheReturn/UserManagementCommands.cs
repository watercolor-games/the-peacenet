using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;

namespace ShiftOS.Engine
{
    [Namespace("admin")]
    [KernelMode]
    [RequiresUpgrade("mud_fundamentals")]
    public static class AdminUserManagementCommands
    {
        [Command("add", description = "Add a user to the system.", usage ="name:")]
        [RequiresArgument("name")]
        public static bool AddUser(Dictionary<string, object> args)
        {
            string name = args["name"].ToString();
            if(SaveSystem.CurrentSave.Users.FirstOrDefault(x=>x.Username==name) != null)
            {
                Console.WriteLine("Error: User already exists.");
                return true;
            }

            var user = new ClientSave
            {
                Username = name,
                Password = "",
                Permissions = UserPermissions.User
            };
            SaveSystem.CurrentSave.Users.Add(user);
            Console.WriteLine($"Creating new user \"{name}\" with no password and User permissions.");
            SaveSystem.SaveGame();
            return true;
        }

        [Command("remove", description = "Remove a user from the system.", usage = "name:")]
        [RequiresArgument("name")]
        public static bool RemoveUser(Dictionary<string, object> args)
        {
            string name = args["name"].ToString();
            if (SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == name) == null)
            {
                Console.WriteLine("Error: User doesn't exist.");
                return true;
            }

            var user = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == name);
            SaveSystem.CurrentSave.Users.Remove(user);
            Console.WriteLine($"Removing user \"{name}\" from system...");
            SaveSystem.SaveGame();
            return true;
        }



    }

    [Namespace("user")]
    [RequiresUpgrade("mud_fundamentals")]
    public static class UserManagementCommands
    {


        [Command("setpass", description ="Allows you to set your password to a new value.", usage ="old:,new:")]
        [RequiresArgument("old")]
        [RequiresArgument("new")]
        public static bool SetPassword(Dictionary<string, object> args)
        {
            string old = args["old"].ToString();
            string newpass = args["new"].ToString();

            if(old == SaveSystem.CurrentUser.Password)
            {
                SaveSystem.CurrentUser.Password = newpass;
                SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == SaveSystem.CurrentUser.Username).Password = newpass;
                Console.WriteLine("Password set successfully.");
                SaveSystem.SaveGame();
            }
            else
            {
                Console.WriteLine("Passwords do not match.");
            }
            return true;
        }
    }
}
