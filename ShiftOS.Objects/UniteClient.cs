using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Objects;

namespace ShiftOS.Unite
{
    public class UniteClient
    {
        /// <summary>
        /// Gets a string represents the user token for this Unite Client.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Gets the base URL used in all API calls. Retrieved from the user's servers.json file.
        /// </summary>
        public string BaseURL
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Get the display name of a user.
        /// </summary>
        /// <param name="id">The user ID to look at.</param>
        /// <returns></returns>
        public string GetDisplayNameId(string id)
        {
            return MakeCall("/API/GetDisplayName/" + id);
        }

        /// <summary>
        /// Get the Pong highscore stats for all users. 
        /// </summary>
        /// <returns></returns>
        public PongHighscoreModel GetPongHighscores()
        {
            return JsonConvert.DeserializeObject<PongHighscoreModel>(MakeCall("/API/GetPongHighscores"));
        }

        /// <summary>
        /// Create a new instance of the <see cref="UniteClient"/> object. 
        /// </summary>
        /// <param name="baseurl">Unused.</param>
        /// <param name="usertoken">The user API token to use for this client (see http://getshiftos.ml/Manage and click "API" to see your tokens)</param>
        public UniteClient(string baseurl, string usertoken)
        {
            //Handled by the servers.json file
            //BaseURL = baseurl;
            Token = usertoken;
        }

        /// <summary>
        /// Make a call to the Unite API using the current user token and base URL.
        /// </summary>
        /// <param name="url">The path, relative to the base URL, to call.</param>
        /// <returns>The server's response.</returns>
        internal string MakeCall(string url)
        {
            var webrequest = WebRequest.Create(BaseURL + url);
            webrequest.Headers.Add("Authentication: Token " + Token);
            using (var response = webrequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Get the Pong codepoint highscore for the current user.
        /// </summary>
        /// <returns>The amount of Codepoints returned by the server</returns>
        public ulong GetPongCP()
        {
            return Convert.ToUInt64(MakeCall("/API/GetPongCP"));
        }

        /// <summary>
        /// Get the pong highest level score for this user
        /// </summary>
        /// <returns>The highest level the user has reached.</returns>
        public int GetPongLevel()
        {
            return Convert.ToInt32(MakeCall("/API/GetPongLevel"));
        }

        /// <summary>
        /// Set the user's highest level record for Pong.
        /// </summary>
        /// <param name="value">The level to set the record to.</param>
        public void SetPongLevel(int value)
        {
            MakeCall("/API/SetPongLevel/" + value.ToString());
        }

        /// <summary>
        /// Set the pong Codepoints record for the user
        /// </summary>
        /// <param name="value">The amount of Codepoints to set the record to</param>
        public void SetPongCP(ulong value)
        {
            MakeCall("/API/SetPongCP/" + value.ToString());
        }

        /// <summary>
        /// Get the user's email address.
        /// </summary>
        /// <returns>The user's email address.</returns>
        public string GetEmail()
        {
            return MakeCall("/API/GetEmail");
        }

        /// <summary>
        /// Get the user's system name.
        /// </summary>
        /// <returns>The user's system name.</returns>
        public string GetSysName()
        {
            return MakeCall("/API/GetSysName");
        }

        /// <summary>
        /// Set the user's system name.
        /// </summary>
        /// <param name="value">The system name to set the record to.</param>
        public void SetSysName(string value)
        {
            MakeCall("/API/SetSysName/" + value);
        }

        /// <summary>
        /// Get the user's display name.
        /// </summary>
        /// <returns>The user's display name.</returns>
        public string GetDisplayName()
        {
            return MakeCall("/API/GetDisplayName");
        }

        /// <summary>
        /// Set the user's display name.
        /// </summary>
        /// <param name="value">The display name to set the user's account to.</param>
        public void SetDisplayName(string value)
        {
            MakeCall("/API/SetDisplayName/" + value.ToString());
        }

        /// <summary>
        /// Get the user's full name if they have set it in their profile.
        /// </summary>
        /// <returns>Empty string if the user hasn't set their fullname, else, a string representing their fullname.</returns>
        public string GetFullName()
        {
            return MakeCall("/API/GetFullName");
        }

        /// <summary>
        /// Set the user's fullname.
        /// </summary>
        /// <param name="value">The new fullname.</param>
        public void SetFullName(string value)
        {
            MakeCall("/API/SetFullName/" + value.ToString());
        }

        /// <summary>
        /// Get the user's codepoints.
        /// </summary>
        /// <returns>The amount of codepoints stored on the server for this user.</returns>
        public ulong GetCodepoints()
        {
            return Convert.ToUInt64(MakeCall("/API/GetCodepoints"));
        }

        /// <summary>
        /// Set the user's codepoints.
        /// </summary>
        /// <param name="value">The amount of codepoints to set the user's codepoints value to.</param>
        public void SetCodepoints(ulong value)
        {
            MakeCall("/API/SetCodepoints/" + value.ToString());
        }
    }

    /// <summary>
    /// API data model for Unite pong highscores.
    /// </summary>
    public class PongHighscoreModel
    {
        /// <summary>
        /// Amount of pages in this list.
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// An array representing the highscores found on the server.
        /// </summary>
        public PongHighscore[] Highscores { get; set; }
    }

    /// <summary>
    /// API data model for a single Pong highscore.
    /// </summary>
    public class PongHighscore
    {
        /// <summary>
        /// The user ID linked to this highscore.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The highscore's level record.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The highscore's codepoint cashout record.
        /// </summary>
        public long CodepointsCashout { get; set; }
    }
}
