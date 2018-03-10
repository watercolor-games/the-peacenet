using System;
using System.Collections.Generic;
using System.IO;
using Plex.Objects;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Peacenet.Backend
{
    public class RemoteStreams : IBackendComponent
    {
        List<Tuple<Stream, string>> info;

        public void Initiate()
        {
            info = new List<Tuple<Stream, string>>();
        }

        public void SafetyCheck()
        {
            // TODO: Dispose & null streams owned by clients who have disconnected
        }

        public void Unload()
        {
            foreach (var s in info)
                s.Item1.Dispose();
            info = null;
        }

        /// <summary>
        /// Adds fobj as a remote stream.
        /// </summary>
        /// <returns>The remote stream ID to give to the client.</returns>
        /// <param name="fobj">The local stream.</param>
        /// <param name="auth">The session ID of the client who can interact with the stream.</param>
        public int Create(Stream fobj, string auth)
        {
            var s = Tuple.Create(fobj, auth);
            var ret = info.IndexOf(null);
            if (ret < 0)
            {
                ret = info.Count;
                info.Add(s);
            }
            else
                info[ret] = s;
            return ret;
        }

        public class RemoteStreamHandler : IMessageHandler
        {
            [Dependency]
            RemoteStreams holder;

            public ServerMessageType HandledMessageType => ServerMessageType.STREAM_OP;

            public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
            {
                // The easy part: authorising.
                var id = datareader.ReadInt32();
                var fail = false;
                fail = id < 0 || id >= holder.info.Count; // Stream ID out of range
                Tuple<Stream, string> s = null;
                if (!fail)
                {
                    s = holder.info[id];
                    fail = s == null; // Stream ID nulled (closed already)
                }
                if (!fail)
                    fail = s.Item2 != session; // Stream doesn't belong to accessing user
                if (fail)
                {
                    Logger.Log("failed");
                    // We give the same error in all cases so the user can't figure anything out
                    // by making deliberately dodgy requests
                    datawriter.Write($"Could not get a handle on stream {id} - nonexistent or not authorised.");
                    return ServerResponseType.REQ_ERROR;
                }
                var opi = datareader.ReadByte();
                if (!Enum.IsDefined(typeof(StreamOp), opi)) // Make sure the byte is in range of the enum
                {
                    Logger.Log("invalid stream operation");
                    datawriter.Write($"Invalid stream operation {opi}.");
                    return ServerResponseType.REQ_ERROR;
                }
                var op = (StreamOp)opi; // We know it's valid now, so get the enum entry
                Logger.Log($"Doing Stream Op: {op}");
                MethodInfo fun;
                object ret;
                try
                {
                    switch (op) // special cases
                    {
                        case StreamOp.Close:
                            s.Item1.Dispose();
                            holder.info[id] = null;
                            return ServerResponseType.REQ_SUCCESS;
                        case StreamOp.Read:
                            var r = new byte[datareader.ReadInt32()];
                            datawriter.Write(s.Item1.Read(r, 0, r.Length));
                            datawriter.Write(r.Length);
                            datawriter.Write(r);
                            Logger.Log("Finished read");
                            return ServerResponseType.REQ_SUCCESS;
                        case StreamOp.Write:
                            var w = datareader.ReadBytes(datareader.ReadInt32());
                            s.Item1.Write(w, 0, w.Length);
                            Logger.Log("Finished write");
                            return ServerResponseType.REQ_SUCCESS;
                    }
                    // Look up the method from its enum entry.
                    fun = typeof(Stream).GetMethod(op.ToString());
                    Logger.Log($"Calling {fun}");

                    // Call it and get the result.
                    // The Select finds a BinaryReader method that reads each argument type.
                    ret = fun.Invoke(s.Item1, fun.GetParameters().Select(p => typeof(BinaryReader).GetMethods().First(m => m.Name.Length > 4 && m.Name.StartsWith("Read", StringComparison.InvariantCulture) && m.ReturnType == (p.ParameterType.IsEnum ? Enum.GetUnderlyingType(p.ParameterType) : p.ParameterType)).Invoke(datareader, null)).ToArray());
                }
                catch (Exception ex)
                {
                    // Not sending back the exception, who knows what's in it?
                    Logger.Log(ex.ToString());
                    datawriter.Write($"Internal error.");
                    return ServerResponseType.REQ_ERROR;
                }
                if (fun.ReturnType != typeof(void))
                    ((dynamic)datawriter).Write(ret);
                return ServerResponseType.REQ_SUCCESS;
            }
        }

        /// <summary>
        /// Server side part of the remote streams test
        /// </summary>
        public class RemoteStreamsTestHandler : IMessageHandler
        {
            [Dependency]
            RemoteStreams rstreams;

            public ServerMessageType HandledMessageType => ServerMessageType.STREAM_TEST;

            public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
            {
                datawriter.Write(rstreams.Create(new MemoryStream(Encoding.UTF8.GetBytes("SOMETHING SOMETHING IS A TESTAMENT TO WHY THE WAYS OF THE BLIND WILL NEVER GET")), session));
                return ServerResponseType.REQ_SUCCESS;
            }
        }
    }
}
