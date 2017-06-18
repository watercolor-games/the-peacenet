using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Administrative user management terminal commands.
    /// </summary>
    [KernelMode]
    [RequiresUpgrade("mud_fundamentals")]
    public static class AdminUserManagementCommands
    {
        /// <summary>
        /// Add a user to the system.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>
        [Command("adduser", description = "{DESC_ADDUSER}")]
        [RequiresArgument("name")]
        public static bool AddUser(Dictionary<string, object> args)
        {
            string name = args["name"].ToString();
            if (SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == name) != null)
            {
                Console.WriteLine("{ERR_USERFOUND}");
                return true;
            }

            var user = new ClientSave
            {
                Username = name,
                Password = "",
                Permissions = UserPermissions.User
            };
            SaveSystem.CurrentSave.Users.Add(user);
            Console.WriteLine(Localization.Parse("{RES_CREATINGUSER}", new Dictionary<string, string>
            {
                { "%name", name }
            }));
            SaveSystem.SaveGame();
            return true;
        }

        /// <summary>
        /// Remove a user from the system.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>

        [Command("removeuser", description = "{DESC_REMOVEUSER}")]
        [RequiresArgument("name")]
        public static bool RemoveUser(Dictionary<string, object> args)
        {
            string name = args["name"].ToString();
            if (SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == name) == null)
            {
                Console.WriteLine("{ERR_NOUSER}");
                return true;
            }

            var user = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == name);
            if (user.Username != SaveSystem.CurrentUser.Username)
            {
                Console.WriteLine("{ERR_REMOVEYOU}");
                return true;
            }
            SaveSystem.CurrentSave.Users.Remove(user);
            Console.WriteLine(Localization.Parse("{RES_REMOVINGUSER}", new Dictionary<string, string>
            {
                ["%name"] = name
            }));
            SaveSystem.SaveGame();
            return true;
        }



        /// <summary>
        /// Set access control level for a user.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>

        [Command("setuserpermissions", description = "{DESC_SETUSERPERMISSIONS}")]
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
                Console.WriteLine("{ERR_BADACL}");
                return true;
            }

            if(SaveSystem.CurrentSave.Users.FirstOrDefault(x=>x.Username==username) == null)
            {
                Console.WriteLine("{ERR_NOUSER}");
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
                    Console.WriteLine("{ERR_BADACL}");
                    return true;
            }

            //Permissions are backwards... oops...
            if(uperm < SaveSystem.CurrentUser.Permissions)
            {
                Console.WriteLine("{ERR_ACLHIGHERVALUE}");
                return true;
            }

            var oldperm = SaveSystem.Users.FirstOrDefault(x => x.Username == username).Permissions;
            if (SaveSystem.CurrentUser.Permissions > oldperm)
            {
                Console.WriteLine("{ERR_HIGHERPERMS}");
                return true;
            }

            SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == username).Permissions = uperm;
            Console.WriteLine("{RES_ACLUPDATED}");
            return true;
        }

        /// <summary>
        /// List all users in the system.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>

        [Command("users", description = "{DESC_USERS}")]
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

    /// <summary>
    /// Non-administrative user management terminal commands.
    /// </summary>
    [RequiresUpgrade("mud_fundamentals")]
    public static class UserManagementCommands
    {
        /// <summary>
        /// Log in as another user.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>
        [Command("su", description = "{DESC_SU}")]
        [RequiresArgument("user")]
        [RequiresArgument("pass")]
        public static bool Login(Dictionary<string, object> args)
        {
            string user = args["user"].ToString();
            string pass = args["pass"].ToString();

            var usr = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == user);
            if(usr==null)
            {
                Console.WriteLine("{ERR_NOUSER}");
                return true;
            }

            if (usr.Password != pass)
            {
                Console.WriteLine("{RES_DENIED}");
                return true;
            }

            SaveSystem.CurrentUser = usr;
            Console.WriteLine("{RES_GRANTED}");
            return true;
        }

        /// <summary>
        /// Set the password for the current user.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command result.</returns>
        [Command("passwd", description ="{DESC_PASSWD}", usage ="old:,new:")]
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
                Console.WriteLine("{RES_PASSWD_SET}");
                SaveSystem.SaveGame();
            }
            else
            {
                Console.WriteLine("{ERR_PASSWD_MISMATCH}");
            }
            return true;
        }
    }
}
