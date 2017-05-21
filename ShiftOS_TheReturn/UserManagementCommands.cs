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
            if(user.Username != SaveSystem.CurrentUser.Username)
            {
                Console.WriteLine("Error: Cannot remove yourself.");
                return true;
            }
            SaveSystem.CurrentSave.Users.Remove(user);
            Console.WriteLine($"Removing user \"{name}\" from system...");
            SaveSystem.SaveGame();
            return true;
        }




        [Command("set_acl")]
        [RequiresArgument("user")]
        [RequiresArgument("val")]
        public static bool SetUserPermission(Dictionary<string, object> args)
        {
            int permission = 0;
            string username = args["user"].ToString();
            try 
            {
                permission = Convert.ToInt32(args["val"].ToString());
            }
            catch
            {
                Console.WriteLine("Error: Permission value must be an integer.");
                return true;
            }

            if(SaveSystem.CurrentSave.Users.FirstOrDefault(x=>x.Username==username) == null)
            {
                Console.WriteLine("Error: User not found.");
                return true;
            }

            UserPermissions uperm = UserPermissions.Guest;

            switch (permission)
            {
                case 0:
                    uperm = UserPermissions.Guest;
                    break;
                case 1:
                    uperm = UserPermissions.User;
                    break;
                case 2:
                    uperm = UserPermissions.Admin;
                    break;
                case 3:
                    uperm = UserPermissions.Root;
                    break;
                default:
                    Console.WriteLine("Permission value must be between 0 and 4.");
                    return true;
            }

            //Permissions are backwards... oops...
            if(uperm < SaveSystem.CurrentUser.Permissions)
            {
                Console.WriteLine("Error: Cannot set user permissions to values greater than your own!");
                return true;
            }

            var oldperm = SaveSystem.Users.FirstOrDefault(x => x.Username == username).Permissions;
            if (SaveSystem.CurrentUser.Permissions > oldperm)
            {
                Console.WriteLine("Error: Can't set the permission of this user. They have more rights than you.");
                return true;
            }

            SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == username).Permissions = uperm;
            Console.WriteLine("User permissions updated.");
            return true;
        }


        [Command("users", description = "Get a list of all users on the system.")]
        public static bool GetUsers()
        {
            foreach (var u in SaveSystem.CurrentSave.Users)
            {
                if (u.Username == SaveSystem.CurrentUser.Username)
                {
                    ConsoleEx.ForegroundColor = ConsoleColor.Magenta;
                    ConsoleEx.Bold = true;
                }
                else
                {
                    ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                    ConsoleEx.Bold = false;
                }
                Console.WriteLine(u.Username);
            }
            return true;
        }
    }

    [Namespace("user")]
    [RequiresUpgrade("mud_fundamentals")]
    public static class UserManagementCommands
    {
        [Command("login", description = "Log in as another user.")]
        [RequiresArgument("user")]
        [RequiresArgument("pass")]
        public static bool Login(Dictionary<string, object> args)
        {
            string user = args["user"].ToString();
            string pass = args["pass"].ToString();

            var usr = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == user);
            if(usr==null)
            {
                Console.WriteLine("Error: No such user.");
                return true;
            }

            if (usr.Password != pass)
            {
                Console.WriteLine("Access denied.");
                return true;
            }

            SaveSystem.CurrentUser = usr;
            Console.WriteLine("Access granted.");
            return true;
        }

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
