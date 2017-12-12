using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Config;
using Plex.Engine.GUI;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Engine.Server
{
    public class WatercolorAPIManager : IEngineComponent
    {
        [Dependency]
        private ConfigManager _config = null;
        private string _apiKey = "";
        private string _baseUrl = "https://getshiftos.net/api";

        [Dependency]
        private WindowSystem _winsys = null;

        private WatercolorUser _user = null;

        public void Initiate()
        {
            _apiKey = _config.GetValue("wgApiKey", _apiKey);
            _baseUrl = _config.GetValue("wgApiBaseUrl", _baseUrl);
            queryForUser();
        }

        private void queryForUser()
        {
            string uid = null;
            var wr = WebRequest.Create(_baseUrl + "/users/getid");
            wr.ContentType = "text/plain";
            wr.Method = "GET";
            wr.Headers.Add("Authentication: " + _apiKey);
            try
            {
                using (var res = wr.GetResponse())
                {
                    using (var str = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(str))
                        {
                            uid = reader.ReadToEnd();
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(uid))
                {
                    var ur = WebRequest.Create(_baseUrl + "/users/" + uid);
                    ur.Method = "GET";
                    ur.ContentType = "application/json";
                    using (var res = ur.GetResponse())
                    {
                        using (var str = res.GetResponseStream())
                        {
                            using (var reader = new StreamReader(str))
                            {
                                _user = JsonConvert.DeserializeObject<WatercolorUser>(reader.ReadToEnd());

                            }
                        }
                    }
                }
            }
            catch { }
        }

        public string Token
        {
            get
            {
                return _apiKey;
            }
        }

        public WatercolorUser User
        {
            get
            {
                return _user;
            }
        }

        public bool LoggedIn
        {
            get
            {
                return _user != null;
            }
        }

        public void Logout()
        {
            _user = null;
            _apiKey = "";
            _config.SetValue("wgApiKey", _apiKey);
            _config.SaveToDisk();
        }

        public async Task Register(string username, string password, string email, Action onComplete, Action<string> onError)
        {
            try
            {
                var wr = WebRequest.Create(_baseUrl + "/users/register");
                wr.Method = "POST";
                wr.ContentType = "application/json";
                using (var req = await wr.GetRequestStreamAsync())
                {
                    using (var writer = new StreamWriter(req))
                    {
                        writer.Write(JsonConvert.SerializeObject(new
                        {
                            username = username,
                            password = password,
                            email = email
                        }));
                    }
                }
                using (var res = await wr.GetResponseAsync())
                {
                    onComplete?.Invoke();
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }

        }

        public async Task Login(string username, string password, Action onComplete, Action<string> onError)
        {
            try
            {
                var wr = WebRequest.Create(_baseUrl + "/users/login");
                wr.Method = "POST";
                wr.ContentType = "application/json";
                using (var req = await wr.GetRequestStreamAsync())
                {
                    using (var writer = new StreamWriter(req))
                    {
                        writer.Write(JsonConvert.SerializeObject(new
                        {
                            username = username,
                            password = password
                        }));
                    }
                }
                using (var res = await wr.GetResponseAsync())
                {
                    using (var str = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(str))
                        {
                            _apiKey = reader.ReadToEnd();
                            _config.SetValue("wgApiKey", _apiKey);
                            _config.SaveToDisk();
                        }
                    }
                }
                onComplete?.Invoke();
            }
            catch(Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }
}
