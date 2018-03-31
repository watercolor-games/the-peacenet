using System;
using System.IO;
using Plex.Engine.Interfaces;
using Peacenet.Server;
using Plex.Objects;
using System.Threading.Tasks;
using System.Linq;

namespace Plex.Engine
{
    public class RemoteStreams : IEngineComponent
    {
        [Dependency]
        Plexgate plexgate;
        public Stream Open(int id)
        {
            var s = plexgate.New<RemoteStream>();
            s.id = id;
            Logger.Log($"Remote Stream {id} Instantiated");
            return s;
        }

        public void Initiate()
        {
        }

        class RemoteStream : Stream
        {
            [Dependency]
            AsyncServerManager man;

            public int id;

            BinaryReader sendMsg(StreamOp op, Action<BinaryWriter> mkbody = null)
            {
                byte[] body;
                using (var ms = new MemoryStream())
                using (var write = new BinaryWriter(ms))
                {
                    write.Write(id);
                    write.Write((byte)op);
                    mkbody?.Invoke(write);
                    body = ms.ToArray();
                }
                ServerResponseType resp = ServerResponseType.REQ_SUCCESS;
                BinaryReader ret = null;
                Task.WaitAll(man.SendMessage(ServerMessageType.STREAM_OP, body, (aresp, aret) => { resp = aresp; ret = aret == null ? null : new BinaryReader(new MemoryStream(aret.ReadBytes((int)aret.BaseStream.Length))); }));
                switch (resp) // TODO: Improve error handling
                {
                    case ServerResponseType.REQ_SUCCESS:
                        break;
                    case ServerResponseType.REQ_ERROR:
                        throw new Exception(ret.ReadString());
                    default:
                        throw new Exception();
                }
                return ret;
            }

            bool getbool(StreamOp op)
            {
                using (var read = sendMsg(op))
                    return read.ReadBoolean();
            }

            long getlong(StreamOp op)
            {
                using (var read = sendMsg(op))
                    return read.ReadInt64();
            }

            void sendCmd(StreamOp op, Action<BinaryWriter> mkbody = null)
            {
                sendMsg(op, mkbody)?.Dispose();
            }
            public override bool CanRead => getbool(StreamOp.get_CanRead);
            public override bool CanSeek => getbool(StreamOp.get_CanSeek);
            public override bool CanWrite => getbool(StreamOp.get_CanWrite);
            public override long Length => getlong(StreamOp.get_Length);

            public override long Position
            {
                get
                {
                    return getlong(StreamOp.get_Position);
                }

                set
                {
                    sendCmd(StreamOp.set_Position, (write) => write.Write(value));
                }
            }

            public override void Flush()
            {
                sendCmd(StreamOp.Flush);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                using (var read = sendMsg(StreamOp.Read, (write) => write.Write(count)))
                {
                    var ret = read.ReadInt32();
                    read.Read(buffer, offset, read.ReadInt32());
                    return ret;
                }
            }

            public override int ReadByte()
            {
                using (var read = sendMsg(StreamOp.ReadByte, null))
                {
                    return read.ReadInt32();
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                using (var read = sendMsg(StreamOp.Seek, (write) => { write.Write(offset); write.Write((int)origin); }))
                    return read.ReadInt64();
            }

            public override void SetLength(long value)
            {
                sendCmd(StreamOp.SetLength, (write) => write.Write(value));
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (offset + count > buffer.Length)
                    throw new ArgumentException("The sum of offset and count is greater than the buffer length.");
                sendCmd(StreamOp.Write, (write) => { write.Write(count); write.Write(buffer.Skip(offset).Take(count).ToArray()); });
            }

            public override void Close()
            {
                sendCmd(StreamOp.Close);
            }
        }
    }
}
