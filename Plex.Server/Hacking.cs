using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class Hacking
    {
        private static List<HackSession> _hsessions = new List<HackSession>();

        //[SessionRequired]
        //[ServerMessageHandler("hack_solvepuzzle")]
        //public static void SolvePuzzle(string session_id, string content, string ip, int port)
        //{
        //    var puzzledata = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        //    var hsession = _hsessions.FirstOrDefault(x => x.SessionID == session_id && x.HackableID == puzzledata["hackable"]);
        //    if (hsession != null)
        //    {
        //        var puzzle = hsession.Puzzles.FirstOrDefault(x => x.Data.ID == puzzledata["puzzle"]);
        //        bool completed = puzzle.Data.Completed;
        //        if (completed)
        //        {
        //            Program.SendMessage(new Objects.PlexServerHeader
        //            {
        //                SessionID = session_id,
        //                IPForwardedBy = ip,
        //                Message = "hack_puzzlesolved",
        //                Content = ""
        //            }, port);
        //        }
        //        else
        //        {
        //            completed = puzzle.Evaluate(puzzledata["value"]);
        //            string res = (completed) ? "1" : "0";
        //            puzzle.Data.Completed = completed;
        //            if (completed == true)
        //            {
        //                Program.SendMessage(new Objects.PlexServerHeader
        //                {
        //                    Content = res,
        //                    IPForwardedBy = ip,
        //                    Message = "hack_puzzleresult",
        //                    SessionID = session_id
        //                }, port);
        //                Program.SaveWorld();
        //            }
        //            else
        //            {
        //                Program.SendMessage(new Objects.PlexServerHeader
        //                {
        //                    Content = res,
        //                    IPForwardedBy = ip,
        //                    SessionID = session_id,
        //                    Message = "hack_puzzleresult"
        //                }, port);
        //            }
        //        }
        //    }
        //}

        public static IEnumerable<PortAttribute> GetPorts(SystemType systemType)
        {
            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is PortAttribute) != null))
                {
                    var attr = method.GetCustomAttributes(false).FirstOrDefault(x => x is PortAttribute) as PortAttribute;
                    if(attr.AttachTo.HasFlag(systemType))
                        yield return attr;
                }
            }

        }

        public static HackSession GrabSession(string _sessionID)
        {
            return _hsessions.FirstOrDefault(x => x.SessionID == _sessionID);
        }

        //[SessionRequired]
        //[ServerMessageHandler("hack_puzzlehint")]
        //public static void PuzzleHint(string session_id, string content, string ip, int port)
        //{
        //    var puzzledata = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        //    var hsession = _hsessions.FirstOrDefault(x => x.SessionID == session_id && x.HackableID == puzzledata["hackable"]);
        //    if (hsession != null)
        //    {
        //        var puzzle = hsession.Puzzles.FirstOrDefault(x => x.Data.ID == puzzledata["puzzle"]);
        //        Program.SendMessage(new Objects.PlexServerHeader
        //        {
        //            Message = "hack_hintmsg",
        //            Content = puzzle.GetHint(),
        //            IPForwardedBy = ip,
        //            SessionID = session_id
        //        }, port);
        //    }
        //}


        //[SessionRequired]
        //[ServerMessageHandler("hack_abort")]
        //public static void AbortHack(string session_id, string content, string ip, int port)
        //{
        //    var hsession = _hsessions.FirstOrDefault(x => x.SessionID == session_id && x.HackableID == content);
        //    if (hsession != null)
        //    {
        //        _hsessions.Remove(hsession);
        //    }
        //}


        //[SessionRequired]
        //[ServerMessageHandler("hack_start")]
        //public static void StartHack(string session_id, string content, string ip, int port)
        //{
        //    if (_hsessions.FirstOrDefault(x => x.SessionID == session_id && x.HackableID == content) != null)
        //    {
        //        Program.SendMessage(new Objects.PlexServerHeader
        //        {
        //            Message = "hack_started",
        //            IPForwardedBy = ip,
        //            SessionID = session_id,
        //            Content = ""
        //        }, port);
        //        return;
        //    }
        //    var system = Program.GetSaveFromPrl(content);
        //    if (system != null)
        //    {
        //        var session = new HackSession
        //        {
        //            SessionID = session_id,
        //            HackableID = content,
        //            Puzzles = new List<IPuzzle>()
        //        };
        //        foreach (var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IPuzzle))))
        //        {
        //            var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is PuzzleAttribute) as PuzzleAttribute;
        //            if (attrib != null)
        //            {
        //                var puzzle = system.Puzzles.FirstOrDefault(x => x.ID == attrib.ID && x.Rank == attrib.Rank);
        //                if (puzzle != null)
        //                {
        //                    IPuzzle p = (IPuzzle)Activator.CreateInstance(type, null);
        //                    p.Data = puzzle;
        //                    session.Puzzles.Add(p);
        //                }
        //            }
        //        }
        //        Program.SendMessage(new Objects.PlexServerHeader
        //        {
        //            Message = "hack_started",
        //            IPForwardedBy = ip,
        //            SessionID = session_id,
        //            Content = ""
        //        }, port);
        //        _hsessions.Add(session);
        //    }
        //}

  
        //[SessionRequired]
        //[ServerMessageHandler("get_hackable")]
        //public static void GetHackable(string session_id, string content, string ip, int port)
        //{
        //    var system = Program.GetSaveFromPrl(content);
        //    if (system != null)
        //    {
        //        system.NetName = content.Split('.')[0];
        //        Program.SendMessage(new Objects.PlexServerHeader
        //        {
        //            Content = JsonConvert.SerializeObject(system),
        //            Message = "hackable_data",
        //            IPForwardedBy = ip,
        //            SessionID = session_id
        //        }, port);
        //    }
        //}

        //[Port("ssh", "Secure-ish shell", 22, SystemType.Computer | SystemType.Mobile)]
        //public static void SSHConnect(string auth, string session, string ip, int port)
        //{
        //    Program.SendMessage(new PlexServerHeader
        //    {
        //        Content = auth,
        //        IPForwardedBy = ip,
        //        Message = "trm_syschange",
        //        SessionID = session
        //    }, port);
        //}


        internal static void PortConnect(string auth_token, int port1, string sessionID, string iPAddress, int port2)
        {
            foreach(var type in ReflectMan.Types)
            {
                foreach(var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x=>x.GetCustomAttributes(false).FirstOrDefault(y=>y is PortAttribute) != null))
                {
                    var attr = method.GetCustomAttributes(false).FirstOrDefault(x => x is PortAttribute) as PortAttribute;
                    if(attr.Value == port1)
                    {
                        method.Invoke(null, new object[] { auth_token, sessionID, iPAddress, port2 });
                        return;
                    }
                }
            }
            throw new Exception("This port has no server-side connection method implemented.");
        }
    }
}
