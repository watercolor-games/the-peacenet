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
using System.Threading;

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
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
            Thread.Sleep(2000);
            AppDomain.CurrentDomain.UnhandledException += (o, a) =>
            {
                System.Diagnostics.Process.Start("ShiftOS.Server.exe");
                Environment.Exit(0);
            };
            UserConfig.Get();
            System.Timers.Timer tmr = new System.Timers.Timer(5000);
            tmr.Elapsed += (o, a) =>
            {
                if (server.IsOnline)
                {

                    try
                    {
                        server.DispatchAll(new NetObject("heartbeat", new ServerMessage
                        {
                            Name = "heartbeat",
                            GUID = "server"
                        }));
                    }
                    catch { }
                }
            };
			if (!Directory.Exists("saves"))
			{
				Directory.CreateDirectory("saves");
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
                tmr.Start();
            };

			server.OnStopped += (o, a) =>
			{
				Console.WriteLine("WARNING! Server stopped.");
                tmr.Stop();
			};

			server.OnError += (o, a) =>
			{
				Console.WriteLine("ERROR: " + a.Exception.Message);
			};

			server.OnClientAccepted += (o, a) =>
			{
				Console.WriteLine("Client connected.");
                try
                {
                    server.DispatchTo(a.Guid, new NetObject("welcome", new ServerMessage { Name = "Welcome", Contents = a.Guid.ToString(), GUID = "Server" }));
                }
                catch
                {
                    Console.WriteLine("Oh, you don't have time to finish the handshake? Fine. Get off.");
                }
            };

            server.OnClientDisconnected += (o, a) =>
            {
                Console.WriteLine("Client disconnected.");
            };

            server.OnClientRejected += (o, a) =>
            {
                Console.WriteLine("FUCK. Something HORRIBLE JUST HAPPENED.");
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
            
            /*
            var task = ChatBackend.StartDiscordBots();
            task.Wait();
            */


            while (server.IsOnline)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();
                try
                {
                    if (cmd.ToLower().StartsWith("decrypt "))
                    {
                        string username = cmd.Remove(0, 8);
                        if (File.Exists("saves/" + username + ".save"))
                        {
                            Console.WriteLine(ReadEncFile("saves/" + username + ".save"));
                        }
                        else
                        {
                            Console.WriteLine("Save not found.");
                        }
                    }
                    else if (cmd == "purge_all_bad_saves")
                    {
                        foreach(var f in Directory.GetFiles("saves"))
                        {
                            try
                            {
                                Console.WriteLine("Testing " + f + "...");
                                ReadEncFile(f);
                                Console.WriteLine("OK");
                            }
                            catch
                            {
                                Console.WriteLine("Not OK. Deleting.");
                                File.Delete(f);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
                                            var mAttrib = attrib as MudRequestAttribute;
                                            if (mAttrib.RequestName == msg.Name)
                                            {
                                                new Thread(() =>
                                                {
                                                    try
                                                    {
                                                        object contents = null;
                                                        

                                                        if (mAttrib.ExpectedType == typeof(int))
                                                        {
                                                            int result = 0;
                                                            if (int.TryParse(msg.Contents, out result) == true)
                                                            {
                                                                contents = result;
                                                            }
                                                            else
                                                            {
                                                                throw new MudException($"Protocol error: {msg.Name} expects a 32-bit signed integer for the message contents.");
                                                            }
                                                        }
                                                        else if (mAttrib.ExpectedType == typeof(long))
                                                        {
                                                            long result = 0;
                                                            if (long.TryParse(msg.Contents, out result) == true)
                                                            {
                                                                contents = result;
                                                            }
                                                            else
                                                            {
                                                                throw new MudException($"Protocol error: {msg.Name} expects a 64-bit signed integer for the message contents.");
                                                            }
                                                        }
                                                        else if (mAttrib.ExpectedType == typeof(bool))
                                                        {
                                                            if (msg.Contents.ToLower() == "true")
                                                            {
                                                                contents = true;
                                                            }
                                                            else if (msg.Contents.ToLower() == "false")
                                                            {
                                                                contents = false;
                                                            }
                                                            else
                                                            {
                                                                contents = null;
                                                                throw new MudException("Protocol error: " + msg.Name + " expects a content type of 'boolean'. Please send either 'true' or 'false'.");
                                                            }
                                                        }
                                                        else if (mAttrib.ExpectedType == null)
                                                        {
                                                        }
                                                        else if(mAttrib.ExpectedType == typeof(string))
                                                        {
                                                            contents = msg.Contents;
                                                        }
                                                        else
                                                        {
                                                            //object type
                                                            object result = null;
                                                            try
                                                            {
                                                                result = JsonConvert.DeserializeObject(msg.Contents, mAttrib.ExpectedType);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Console.WriteLine(ex);
                                                                result = null;
                                                            }
                                                            if (result == null)
                                                                throw new MudException($"Protocol error: {msg.Name} expects an object of type {mAttrib.ExpectedType.FullName}. Please send a JSON string representing an object of this type.");
                                                            contents = result;
                                                        }

                                                        method?.Invoke(null, new[] { msg.GUID, contents });
                                                    }
                                                    catch (Exception mEx)
                                                    {
                                                        Console.WriteLine(mEx);
                                                        ClientDispatcher.DispatchTo("Error", msg.GUID, mEx);
                                                    }
                                                    return;
                                                }).Start();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"[{DateTime.Now}] Exception while handling request {msg.Name}: {ex}");
                            //return;
                        }
                    }
                }

                ClientDispatcher.DispatchTo("Error", msg.GUID, new MudRequestHandlerNotFoundException());
			}
			catch(Exception ex)
			{
				Console.WriteLine("An error occurred with that one.");
				Console.WriteLine(ex);

                ClientDispatcher.DispatchTo("Error", msg.GUID, ex);
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
            try
            {
                if (Server.Clients.Contains(new Guid(cGuid)))
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
            catch
            {
                //FUCKING SHOOT ME.
            }
        }
    }
}

// Uncommenting by Michael