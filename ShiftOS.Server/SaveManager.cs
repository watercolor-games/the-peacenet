/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
using System.Net;

namespace ShiftOS.Server
{
    public static class SaveManager
    {
        [MudRequest("usr_getcp", typeof(Dictionary<string, object>))]
        public static void GetCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (!args.ContainsKey("username"))
                throw new MudException("No 'username' argument supplied.");
            args["username"] = args["username"].ToString().ToLower();
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

        [MudRequest("mud_login", typeof(Dictionary<string, object>))]
        public static void UserLogin(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null)
            {
                args["username"] = args["username"].ToString().ToLower();
                foreach (var savefile in Directory.GetFiles("saves"))
                {
                    try
                    {
                        var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(savefile));


                        if (save.Username == args["username"].ToString() && save.Password == args["password"].ToString())
                        {
                            if(save.ID == new Guid())
                            {
                                save.ID = Guid.NewGuid();
                                WriteEncFile(savefile, JsonConvert.SerializeObject(save));
                            }


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
                try
                {
                    Program.server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                    {
                        Name = "mud_login_denied",
                        GUID = "server"
                    }));
                }
                catch { }
            }
            else
            {
                try
                {
                    Program.server.DispatchTo(new Guid(guid), new NetObject("auth_failed", new ServerMessage
                    {
                        Name = "mud_login_denied",
                        GUID = "server"
                    }));
                }
                catch { }
            }

        }

        [MudRequest("mud_checkuserexists", typeof(Dictionary<string, object>))]
        public static void CheckUserExists(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null)
            {
                args["username"] = args["username"].ToString().ToLower();
                foreach (var savefile in Directory.GetFiles("saves"))
                {
                    try
                    {
                        var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(savefile));


                        if (save.Username == args["username"].ToString())
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

        [MudRequest("mud_save", typeof(Save))]
        public static void SaveGame(string guid, object contents)
        {
            var sav = contents as Save;
            if (string.IsNullOrWhiteSpace(sav.Username))
                return;
            sav.Username = sav.Username.ToLower();
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

        [MudRequest("mud_token_login", typeof(string))]
        public static void TokenLogin(string guid, string token)
        {
            foreach (var savefile in Directory.GetFiles("saves"))
            {
                try
                {
                    var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(savefile));

                    if (save.UniteAuthToken == token)
                    {
                        if (save.ID == new Guid())
                        {
                            save.ID = Guid.NewGuid();
                            WriteEncFile(savefile, JsonConvert.SerializeObject(save));
                        }

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

        [MudRequest("delete_save", typeof(ClientSave))]
        public static void DeleteSave(string guid, object contents)
        {
            var cSave = contents as ClientSave;
            cSave.Username = cSave.Username.ToLower();
            foreach(var saveFile in Directory.GetFiles("saves"))
            {
                try
                {
                    var save = JsonConvert.DeserializeObject<Save>(ReadEncFile(saveFile));
                    if(save.Username == cSave.Username && save.Password == cSave.Password)
                    {
                        File.Delete(saveFile);
                        return;
                    }
                }
                catch { }
            }

        }

        [MudRequest("usr_givecp", typeof(Dictionary<string, object>))]
        public static void GiveCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["amount"] != null)
            {
                args["username"] = args["username"].ToString().ToLower();
                string userName = args["username"] as string;
                ulong cpAmount = (ulong)args["amount"];

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

        [MudRequest("usr_takecp", typeof(Dictionary<string, object>))]
        public static void TakeCodepoints(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null && args["amount"] != null && args["yourusername"] != null)
            {
                args["username"] = args["username"].ToString().ToLower();
                string userName = args["username"] as string;
                string passw = args["password"] as string;
                ulong cpAmount = (ulong)args["amount"];

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
                                amount = -(long)cpAmount
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
