using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using System.IO;
using Newtonsoft.Json;
using NetSockets;
using static ShiftOS.Server.Program;

namespace ShiftOS.Server
{
    public static class SaveManager
    {
        [MudRequest("usr_getcp")]
        public static void GetCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (!args.ContainsKey("username"))
                throw new MudException("No 'username' argument supplied.");

            foreach(var savefile in Directory.GetFiles("saves"))
            {
                var save = ReadSave(savefile);
                if(save.Username == args["username"] as string)
                {
                    Program.ClientDispatcher.DispatchTo("usr_codepoints", guid, save.Codepoints);
                    return;
                }
            }

            throw new MudException("User " + args["username"] as string + " not found on this multi-user domain.");
            
        }

        [MudRequest("mud_login")]
        public static void UserLogin(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null)
            {
                foreach (var savefile in Directory.GetFiles("saves"))
                {
                    try
                    {
                        var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(savefile));


                        if (save.Username == args["username"].ToString() && save.Password == args["password"].ToString())
                        {

                            Program.server.DispatchTo(new Guid(guid), new NetObject("mud_savefile", new ServerMessage
                            {
                                Name = "mud_savefile",
                                GUID = "server",
                                Contents = JsonConvert.SerializeObject(save)
                            }));
                            return;
                        }
                    }
                    catch { }
                }
                Program.server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                {
                    Name = "mud_login_denied",
                    GUID = "server"
                }));
            }
            else
            {
                Program.server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                {
                    Name = "mud_login_denied",
                    GUID = "server"
                }));
            }

        }

        [MudRequest("mud_checkuserexists")]
        public static void CheckUserExists(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null)
            {
                foreach (var savefile in Directory.GetFiles("saves"))
                {
                    try
                    {
                        var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(savefile));


                        if (save.Username == args["username"].ToString() && save.Password == args["password"].ToString())
                        {
                            server.DispatchTo(new Guid(guid), new NetObject("mud_savefile", new ServerMessage
                            {
                                Name = "mud_found",
                                GUID = "server",
                            }));
                            return;
                        }
                    }
                    catch { }
                }
                server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                {
                    Name = "mud_notfound",
                    GUID = "server"
                }));
            }
            else
            {
                server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                {
                    Name = "mud_notfound",
                    GUID = "server"
                }));
            }

        }

        [MudRequest("mud_save")]
        public static void SaveGame(string guid, object contents)
        {
            var sav = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(contents));

            WriteEncFile("saves/" + sav.Username + ".save", JsonConvert.SerializeObject(sav, Formatting.Indented));


            try
            {
                Program.server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                {
                    Name = "mud_saved",
                    GUID = "server"
                }));
            }
            catch { }
        }

        [MudRequest("usr_givecp")]
        public static void GiveCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["amount"] != null)
            {
                string userName = args["username"] as string;
                int cpAmount = (int)args["amount"];

                if (Directory.Exists("saves"))
                {
                    foreach (var saveFile in Directory.GetFiles("saves"))
                    {
                        var saveFileContents = JsonConvert.DeserializeObject<Save>(ReadEncFile(saveFile));
                        if (saveFileContents.Username == userName)
                        {
                            saveFileContents.Codepoints += cpAmount;
                            WriteEncFile(saveFile, JsonConvert.SerializeObject(saveFileContents, Formatting.Indented));
                            Program.ClientDispatcher.Broadcast("update_your_cp", new
                            {
                                username = userName,
                                amount = cpAmount
                            });

                            return;
                        }
                    }
                }
            }

        }

        [MudRequest("usr_takecp")]
        public static void TakeCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null && args["amount"] != null && args["yourusername"] != null)
            {
                string userName = args["username"] as string;
                string passw = args["password"] as string;
                int cpAmount = (int)args["amount"];

                if (Directory.Exists("saves"))
                {
                    foreach (var saveFile in Directory.GetFiles("saves"))
                    {
                        var saveFileContents = JsonConvert.DeserializeObject<Save>(ReadEncFile(saveFile));
                        if (saveFileContents.Username == userName && saveFileContents.Password == passw)
                        {
                            saveFileContents.Codepoints += cpAmount;
                            WriteEncFile(saveFile, JsonConvert.SerializeObject(saveFileContents, Formatting.Indented));
                            Program.ClientDispatcher.Broadcast("update_your_cp", new {
                                username = userName,
                                amount = -cpAmount
                            });
                            Program.ClientDispatcher.DispatchTo("update_your_cp", guid, new
                            {
                                username = args["yourusername"].ToString(),
                                amount = cpAmount
                            });
                            return;
                        }
                    }
                }
            }

        }

        private static Save ReadSave(string fPath)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Save>(ReadEncFile(fPath));
        }


        private static string ReadEncFile(string fPath)
        {
            return Encryption.Decrypt(File.ReadAllText(fPath));
        }

        private static void WriteEncFile(string fPath, string contents)
        {
            File.WriteAllText(fPath, Encryption.Encrypt(contents));
        }

    }
}
