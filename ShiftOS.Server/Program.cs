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
using NetSockets;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Reflection;

namespace ShiftOS.Server
{
	
    public class MudException : Exception
    {
        public MudException(string message) : base(message)
        {

        }
    }

	/// <summary>
	/// Program.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The admin username.
		/// </summary>
		public static string AdminUsername = "admin";

		/// <summary>
		/// The admin password.
		/// </summary>
		public static string AdminPassword = "admin";


		/// <summary>
		/// The server.
		/// </summary>
		public static NetObjectServer server;

		/// <summary>
		/// String event handler.
		/// </summary>
		public delegate void StringEventHandler(string str);

		/// <summary>
		/// Occurs when server started.
		/// </summary>
		public static event StringEventHandler ServerStarted;

		/// <summary>
		/// Saves the chats.
		/// </summary>
		/// <returns>The chats.</returns>
		public static void SaveChats()
		{
			List<Channel> saved = new List<Channel>();
			foreach(var chat in chats)
			{
				saved.Add(new Channel
					{
						ID = chat.ID,
						Name = chat.Name,
						MaxUsers = chat.MaxUsers,
						Topic = chat.Topic,
						Users = new List<Save>()
					});
			}
			File.WriteAllText("chats.json", JsonConvert.SerializeObject(saved));
		}

		/// <summary>
		/// Loads the chats.
		/// </summary>
		/// <returns>The chats.</returns>
		public static void LoadChats()
		{
			chats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json"));
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
            
			if (!Directory.Exists("saves"))
			{
				Directory.CreateDirectory("saves");
			}

			if(!File.Exists("chats.json"))
			{
				SaveChats();
			}
			else
			{
				LoadChats();
			}

			if(!Directory.Exists("scripts"))
			{
				Console.WriteLine("Creating scripts directory...");
				Directory.CreateDirectory("scripts");
				Console.WriteLine("NOTE: This MUD is not just gonna generate scripts for you. You're going to need to write them. YOU are DevX.");
			}

			Console.WriteLine("Starting server...");
			server = new NetObjectServer();

			server.OnStarted += (o, a) =>
			{
				Console.WriteLine($"Server started on address {server.Address}, port {server.Port}.");
				ServerStarted?.Invoke(server.Address.ToString());
			};

			server.OnStopped += (o, a) =>
			{
				Console.WriteLine("WARNING! Server stopped.");
			};

			server.OnError += (o, a) =>
			{
				Console.WriteLine("ERROR: " + a.Exception.Message);
			};

			server.OnClientAccepted += (o, a) =>
			{
				Console.WriteLine("Client connected.");
				server.DispatchTo(a.Guid, new NetObject("welcome", new ServerMessage { Name = "Welcome", Contents = a.Guid.ToString(), GUID = "Server" }));
			};
				
			server.OnReceived += (o, a) =>
			{
				var obj = a.Data.Object;

				var msg = obj as ServerMessage;

				if(msg != null)
				{
					Interpret(msg);
				}
			};
				
			IPAddress defaultAddress = null;

			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					defaultAddress = ip;
				}
			}

			try
			{
				server.Start(defaultAddress, 13370);
			}
			catch
			{
				Console.WriteLine("So we tried to bind the server to your IP address automatically, but your operating system or the .NET environment you are in (possibly Mono on Linux) is preventing us from doing so. We'll try to bind to the loopback IP address (127.0.0.1) and if that doesn't work, the multi-user domain software may not be compatible with this OS or .NET environment.");
				server.Stop();
				server.Start(IPAddress.Loopback, 13370);

			}
            ClientDispatcher = new Server.MudClientDispatcher(server);
            server.OnStopped += (o, a) =>
			{
                Console.WriteLine("Server stopping.");

			};
		}

		/// <summary>
		/// Users the in chat.
		/// </summary>
		/// <returns>The in chat.</returns>
		/// <param name="chan">Chan.</param>
		/// <param name="user">User.</param>
		public static bool UserInChat(Channel chan, Save user)
		{
			foreach(var usr in chan.Users)
			{
				if(usr.Username == user.Username)
				{
					return true;
				}
			}
			return false;
		}


        public static string ReadEncFile(string fPath)
        {
            return Encryption.Decrypt(File.ReadAllText(fPath));
        }

        public static void WriteEncFile(string fPath, string contents)
        {
            File.WriteAllText(fPath, Encryption.Encrypt(contents));
        }

        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Interpret the specified msg.
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void Interpret(ServerMessage msg)
		{
			Dictionary<string, object> args = null;

			try
			{
				Console.WriteLine($@"[{DateTime.Now}] Message received from {msg.GUID}: {msg.Name}");

                foreach (var asmFile in Directory.GetFiles(Environment.CurrentDirectory))
                {
                    if (asmFile.EndsWith(".exe") || asmFile.EndsWith(".dll"))
                    {
                        try
                        {
                            var asm = Assembly.LoadFile(asmFile);
                            foreach (var type in asm.GetTypes())
                            {
                                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                {
                                    foreach (var attrib in method.GetCustomAttributes(false))
                                    {
                                        if (attrib is MudRequestAttribute)
                                        {
                                            if ((attrib as MudRequestAttribute).RequestName == msg.Name)
                                            {
                                                try
                                                {
                                                    object contents = msg.Contents;
                                                    try
                                                    {
                                                        contents = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                                                    }
                                                    catch
                                                    {

                                                    }

                                                    method?.Invoke(null, new[] { msg.GUID, contents });
                                                }
                                                catch (MudException mEx)
                                                {
                                                    ClientDispatcher.DispatchTo("Error", msg.GUID, mEx);
                                                }
                                                catch
                                                {
                                                    Console.WriteLine($@"[{DateTime.Now}] {method.Name}: Missing guid and content parameters, request handler NOT RAN.");
                                                }
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now}] Exception while handling request {msg.Name}: {ex}");
                            return;
                        }
                    }
                }

                ClientDispatcher.DispatchTo("Error", msg.GUID, new MudRequestHandlerNotFoundException());


				switch (msg.Name)
				{
                    
				case "get_memos_for_user":
					if(args["username"] != null)
					{
						string usrnme = args["username"].ToString();

						List<MUDMemo> mmos = new List<MUDMemo>();

						if (File.Exists("memos.json"))
						{
							foreach(var mmo in JsonConvert.DeserializeObject<MUDMemo[]>(File.ReadAllText("memos.json")))
							{
								if(mmo.UserTo == usrnme)
								{
									mmos.Add(mmo);
								}
							}
						}

						server.DispatchTo(new Guid(msg.GUID), new NetObject("mud_memos", new ServerMessage
							{
								Name = "mud_usermemos",
								GUID = "server",
								Contents = JsonConvert.SerializeObject(mmos)
							}));
					}
					break;
				case "mud_post_memo":
					MUDMemo memo = JsonConvert.DeserializeObject<MUDMemo>(msg.Contents);
					List<MUDMemo> memos = new List<MUDMemo>();

					if (File.Exists("memos.json"))
						memos = JsonConvert.DeserializeObject<List<MUDMemo>>(File.ReadAllText("memos.json"));

					memos.Add(memo);
					File.WriteAllText("memos.txt", JsonConvert.SerializeObject(memos));


					break;
				    case "download_start":
                        if (!msg.Contents.StartsWith("shiftnet/"))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = (File.Exists("badrequest.md") == true) ? File.ReadAllText("badrequest.md") : @"# Bad request.

You have sent a bad request to the multi-user domain. Please try again."
                            }));
                            return;
                        }

                        if (File.Exists(msg.Contents))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("download", new ServerMessage
                            {
                                Name = "download_meta",
                                GUID = "server",
                                Contents = JsonConvert.SerializeObject(File.ReadAllBytes(msg.Contents))
                            }));
                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = (File.Exists("notfound.md") == true) ? File.ReadAllText("notfound.md") : @"# Not found.

The page you requested at was not found on this multi-user domain."
                            }));

                        }
                        break;
                    case "shiftnet_get":
                        string surl = args["url"] as string;
                        while (surl.EndsWith("/"))
                        {
                            surl = surl.Remove(surl.Length - 1, 1);
                        }
                        if (!surl.StartsWith("shiftnet/"))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = (File.Exists("badrequest.md") == true) ? File.ReadAllText("badrequest.md") : @"# Bad request.

You have sent a bad request to the multi-user domain. Please try again."
                            }));

                            return;
                        }

                        if (File.Exists(surl))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = File.ReadAllText(surl)
                            }));
                        }
                else if (File.Exists(surl + "/home.rnp"))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = File.ReadAllText(surl + "/home.rnp")
                            }));

                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("shiftnet_got", new ServerMessage
                            {
                                Name = "shiftnet_file",
                                GUID = "server",
                                Contents = (File.Exists("notfound.md") == true) ? File.ReadAllText("notfound.md") : @"# Not found.

The page you requested at was not found on this multi-user domain."
                            }));

                        }
                        break;
				case "chat_join":
					if (args.ContainsKey("id"))
					{
						var cuser = new Save();
						if (args.ContainsKey("user"))
						{
							cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


						}
						int index = -1;
						string chat_id = args["id"] as string;
						foreach(var chat in chats)
						{
							if(chat.ID == chat_id)
							{
								if(chat.Users.Count < chat.MaxUsers || chat.MaxUsers == 0)
								{
									//user can join chat.
									if(cuser != null)
									{
										index = chats.IndexOf(chat);
									}
								}   
							}
						}
						if(index > -1)
						{
							chats[index].Users.Add(cuser);
							server.DispatchAll(new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chat_id}: {cuser.Username} {{CHAT_HAS_JOINED}}"
								}));
						}
						else
						{
							server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chat_id}: {{CHAT_NOT_FOUND_OR_TOO_MANY_MEMBERS}}"
								}));
						}
					}
					break;
				case "chat":
					if (args.ContainsKey("id"))
					{
						var cuser = new Save();
						if (args.ContainsKey("user"))
						{
							cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


						}
						string message = "";
						if (args.ContainsKey("msg"))
							message = args["msg"] as string;

						int index = -1;
						string chat_id = args["id"] as string;
						foreach (var chat in chats)
						{
							if (chat.ID == chat_id)
							{
								if (cuser != null && !string.IsNullOrWhiteSpace(message) && UserInChat(chat, cuser))
								{
									index = chats.IndexOf(chat);
								}
							}
						}
						if (index > -1)
						{
							server.DispatchAll(new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chat_id}/{cuser.Username}: {message}"
								}));
						}
						else
						{
							server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chats[index].ID}: {{CHAT_NOT_FOUND_OR_NOT_IN_CHAT}}"
								}));
						}
					}

					break;
				case "chat_leave":
					if (args.ContainsKey("id"))
					{
						var cuser = new Save();
						if (args.ContainsKey("user"))
						{
							cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


						}
						int index = -1;
						string chat_id = args["id"] as string;
						foreach (var chat in chats)
						{
							if (chat.ID == chat_id)
							{
								if (cuser != null && UserInChat(chat, cuser))
								{
									index = chats.IndexOf(chat);
								}
							}
						}
						if (index > -1)
						{
							server.DispatchAll(new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chats[index].ID}: {cuser.Username} {{HAS_LEFT_CHAT}}"
								}));
						}
						else
						{
							server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
								{
									Name = "cbroadcast",
									GUID = "server",
									Contents = $"{chat_id}: {{CHAT_NOT_FOUND_OR_NOT_IN_CHAT}}"
								}));
						}
					}
					break;
				case "chat_create":
					string id = "";
					string topic = "";
					string name = "";
					int max_users = 0;

					if (args.ContainsKey("id"))
						id = args["id"] as string;
					if (args.ContainsKey("topic"))
						name = args["topic"] as string;
					if (args.ContainsKey("name"))
						topic = args["name"] as string;
					if (args.ContainsKey("max_users"))
						max_users = Convert.ToInt32(args["max_users"].ToString());

					bool id_taken = false;

					foreach(var chat in chats)
					{
						if (chat.ID == id)
							id_taken = true;
					}

					if (id_taken == false)
					{
						chats.Add(new Channel
							{
								ID = id,
								Name = name,
								Topic = topic,
								MaxUsers = max_users,
								Users = new List<Save>()
							});
						SaveChats();
						server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
							{
								Name = "cbroadcast",
								GUID = "server",
								Contents = $"{id}: {{SUCCESSFULLY_CREATED_CHAT}}"
							}));
					}
					else
					{
						server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
							{
								Name = "cbroadcast",
								GUID = "server",
								Contents = $"{id}: {{ID_TAKEN}}"
							}));
					}

					break;
				case "broadcast":
					string text = msg.Contents;
					if (!string.IsNullOrWhiteSpace(text))
					{
						server.DispatchTo(new Guid(msg.GUID), new NetObject("runme", new ServerMessage
							{
								Name = "broadcast",
								GUID = "Server",
								Contents = text

							}));
					}
					break;
				case "getguid_reply":
					msg.GUID = "server";
					//The message's GUID was manipulated by the client to send to another client.
					//So we can just bounce back the message to the other client.
					server.DispatchTo(new Guid(msg.GUID), new NetObject("bounce", msg));
					break;
				case "getguid_send":
					string usrname = msg.Contents;
					string guid = msg.GUID;
					server.DispatchAll(new NetObject("are_you_this_guy", new ServerMessage
						{
							Name = "getguid_fromserver",
							GUID = guid,
							Contents = usrname,
						}));
					break;
				case "script":
					string user = "";
					string script = "";
					string sArgs = "";

					if (!args.ContainsKey("user"))
						throw new Exception("No 'user' arg specified in message to server");

					if (!args.ContainsKey("script"))
						throw new Exception("No 'script' arg specified in message to server");

					if (!args.ContainsKey("args"))
						throw new Exception("No 'args' arg specified in message to server");

					user = args["user"] as string;
					script = args["script"] as string;
					sArgs = args["args"] as string;

					if(File.Exists($"scripts/{user}/{script}.lua"))
					{
						var script_arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(sArgs);
						server.DispatchTo(new Guid(msg.GUID), new NetObject("runme", new ServerMessage {
							Name="run",
							GUID="Server",
							Contents = $@"{{
    script:""{File.ReadAllText($"scripts/{user}/{script}.lua").Replace("\"", "\\\"")}"",
							args:""{sArgs}""
							}}"
							}));
					}
					else
					{
						throw new Exception($"{user}.{script}: Script not found.");
					}
					break;
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("An error occurred with that one.");
				Console.WriteLine(ex);

				server.DispatchTo(new Guid(msg.GUID), new NetObject("error", new ServerMessage { Name = "Error", GUID = "Server", Contents = JsonConvert.SerializeObject(ex) }));
			}
		}

		/// <summary>
		/// Generates the random password.
		/// </summary>
		/// <returns>The random password.</returns>
		public static string GenerateRandomPassword()
		{
			return Guid.NewGuid().ToString();
		}

		/// <summary>
		/// The MUD hack passwords.
		/// </summary>
		public static Dictionary<string, string> MUDHackPasswords = new Dictionary<string, string>();

        public static MudClientDispatcher ClientDispatcher { get; private set; }

		/// <summary>
		/// Stop this instance.
		/// </summary>
		public static void Stop()
		{
			try
			{
				if (server.IsOnline)
				{
					try
					{
						server.Stop();
					}
					catch
					{

					}
				}
				server = null;
			}
			catch { }
		}

		/// <summary>
		/// The chats.
		/// </summary>
		public static List<Channel> chats = new List<Channel>();
	}

    public static class Encryption
    {
        public static string GetMacAddress()
        {
            if (!File.Exists("hash.dat"))
                File.WriteAllText("hash.dat", Guid.NewGuid().ToString());

            return File.ReadAllText("hash.dat");

        }


        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="plainText">Raw string to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(GetMacAddress(), null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }


        public static string Decrypt(string cipherText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(GetMacAddress(), null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    }

    public class MudRequestHandlerNotFoundException : MudException
    {
        public MudRequestHandlerNotFoundException() : base("The request handler for this request couldn't be found.")
        {

        }
    }

    public class MudClientDispatcher
    {
        public NetObjectServer Server { get; private set; }

        public MudClientDispatcher(NetObjectServer srv)
        {
            Server = srv;
            DispatcherGUID = Guid.NewGuid().ToString();
            Console.WriteLine($"[{DateTime.Now}] <ClientDispatcher> Dispatcher started.");
        }

        public string DispatcherGUID { get; private set; }

        public void Broadcast(string msgHeader, object contents)
        {
            Server.DispatchAll(new NetObject
            {
                Name = "broadcast",
                Object = new ServerMessage
                {
                    Name = msgHeader,
                    GUID = DispatcherGUID,
                    Contents = JsonConvert.SerializeObject(contents)
                }
            });
        }

        public void DispatchTo(string msgName, string cGuid, object mContents)
        {
            if(Server.Clients.Contains(new Guid(cGuid)))
            {
                Server.DispatchTo(new Guid(cGuid), new NetObject("dispatch", new ServerMessage
                {
                    Name = msgName,
                    GUID = DispatcherGUID,
                    Contents = JsonConvert.SerializeObject(mContents)
                }));
                Console.WriteLine($"[{DateTime.Now}] <ClientDispatcher> Dispatching to {cGuid}: {msgName}.");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] <ClientDispatcher> Client \"{cGuid}\" not found on server. Possibly a connection drop.");
            }
        }
    }
}

// Uncommenting by Michael