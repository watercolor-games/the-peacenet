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
using System.Threading;

namespace Plex.Engine.Server
{
    public class WatercolorAPIManager : IEngineComponent
    {
        [Dependency]
        private ConfigManager _config = null;
        private string _apiKey = "";
        private string _sessionToken = "";
        private string _baseUrl = "https://getshiftos.net/api";
        private bool _offline = false;

        [Dependency]
        private WindowSystem _winsys = null;

        private WatercolorUser _user = null;

        public void Initiate()
        {
            _apiKey = _config.GetValue("wgApiKey", _apiKey);
            _baseUrl = _config.GetValue("wgApiBaseUrl", _baseUrl);
            Task.Run(() =>
            {
                while (true)
                {
                    queryForSession(queryForUser, () =>
    {
        _offline = true;
        _sessionToken = "";
    });
                    Thread.Sleep(120000);
                }
            });
        }

        private void queryForSession(Action completed, Action error)
        {
            if(!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                error?.Invoke();
                return;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(_sessionToken))
                {
                    var wr = WebRequest.Create($"{_baseUrl}/sessions/grant");
                    wr.Method = "GET";
                    wr.ContentType = "text/plain";
                    using (var req = wr.GetResponse())
                    {
                        using (var stream = req.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                _sessionToken = reader.ReadToEnd();
                                completed?.Invoke();
                            }
                        }
                    }

                }
                else
                {
                    var wr = WebRequest.Create($"{_baseUrl}/sessions/heyimstillhere");
                    wr.Method = "GET";
                    wr.ContentType = "text/plain";
                    wr.Headers.Add("Authorization: " + _sessionToken);
                    using (var req = wr.GetResponse())
                    {
                        using (var stream = req.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                completed?.Invoke();
                            }
                        }
                    }

                }
            }
            catch
            {
                error?.Invoke();
            }
        }

        private void queryForUser()
        {
            string uid = null;
            var wr = WebRequest.Create(_baseUrl + "/users/getid");
            wr.ContentType = "text/plain";
            wr.Method = "GET";
            wr.Headers.Add("Authentication: " + _apiKey);
            wr.Headers.Add("Authorization: " + _sessionToken);
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
                    ur.Headers.Add("Authorization: " + _sessionToken);

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
                wr.Headers.Add("Authorization: " + _sessionToken);
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
                wr.Headers.Add("Authorization: " + _sessionToken);
                using (var req = await wr.GetRequestStreamAsync())
                {
                    using (var writer = new StreamWriter(req))
                    {
                        writer.Write(JsonConvert.SerializeObject(new
                        {
                            email = username,
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
                            queryForUser();
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
