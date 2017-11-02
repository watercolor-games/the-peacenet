using System;
using System.IO;
using System.Collections.Generic;
using Plex.Objects;

namespace Plex.Server
{
    public static class RemoteStreams
    {
        private class RemoteStreamInfo
        {
            public Stream stream;
            public string session;
        }
        
        private static List<RemoteStreamInfo> streams = new List<RemoteStreamInfo>();
        
        public static int Open(Stream stream, string session)
        {
            RemoteStreamInfo info = new RemoteStreamInfo();
            info.stream = stream;
            info.session = session;
            int ret = streams.IndexOf(null);
            if (ret == -1)
            {
                ret = streams.Count;
                streams.Add(info);
            }
            else
            {
                streams[ret] = info;
            }
            return ret;
        }
        
        [ServerMessageHandler(ServerMessageType.STREAM_OP)]
        [SessionRequired]
        public static byte StreamOperation(string session, BinaryReader read, BinaryWriter write)
        {
            int streamid = read.ReadInt32();
            RemoteStreamInfo streaminfo = streams[streamid];
            if (streaminfo.session != session)
            {
                return (byte)ServerResponseType.REQ_ERROR;
            }
            Stream stream = streaminfo.stream;
            switch ((StreamOp)read.ReadByte())
            {
                case StreamOp.get_CanRead:
                {
                    write.Write(stream.CanRead);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.get_CanSeek:
                {
                    write.Write(stream.CanSeek);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.get_CanWrite:
                {
                    write.Write(stream.CanWrite);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.get_Length:
                {
                    write.Write(stream.Length);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.get_Position:
                {
                    write.Write(stream.Position);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.set_Position:
                {
                    stream.Position = read.ReadInt64();
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.Flush:
                {
                    stream.Flush();
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.Read:
                {
                    byte[] buf = new byte[read.ReadInt32()];
                    write.Write(stream.Read(buf, 0, buf.Length));
                    write.Write(buf);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.Seek:
                {
                    write.Write(stream.Seek(read.ReadInt64(), (SeekOrigin)read.ReadInt32()));
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.SetLength:
                {
                    stream.SetLength(read.ReadInt64());
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.Write:
                {
                    byte[] buf = read.ReadBytes(read.ReadInt32());
                    stream.Write(buf, 0, buf.Length);
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                case StreamOp.Close:
                {
                    stream.Close();
                    streams[streamid] = null;
                    return (byte)ServerResponseType.REQ_SUCCESS;
                }
                default:
                {
                    return (byte)ServerResponseType.REQ_ERROR;
                }
            }
        }
    }
}
