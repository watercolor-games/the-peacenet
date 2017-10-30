using System;
using System.IO;
using System.Text;
using Plex.Objects;

namespace Plex.Engine
{
    public class RemoteStream: Stream
    {
        protected int streamId;
        
        protected BinaryReader prop(StreamOp op)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)op);
                return new BinaryReader(ServerManager.GetResponseStream(write.Send()));
            }
        }
        
        protected bool boolprop(StreamOp op)
        {
            using (var read = prop(op))
                return read.ReadBoolean();
        }
        
        protected long longprop(StreamOp op)
        {
            using (var read = prop(op))
                return read.ReadInt64();
        }
        
        protected void action(StreamOp op)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)op);
                write.Send();
            }
        }
        
        public override bool CanRead  { get { return boolprop(StreamOp.get_CanRead);  } }
        public override bool CanSeek  { get { return boolprop(StreamOp.get_CanSeek);  } }
        public override bool CanWrite { get { return boolprop(StreamOp.get_CanWrite); } }
        public override long Length   { get { return longprop(StreamOp.get_Length);   } }
        public override long Position
        {
            get 
            {
                return longprop(StreamOp.get_Position);
            }
            set
            {
                using (var write = new ServerStream(ServerMessageType.STREAM_OP))
                {
                    write.Write(streamId);
                    write.Write((byte)StreamOp.set_Position);
                    write.Write(value);
                    write.Send();
                }
            }
        }
        
        public override void Flush()
        {
            action(StreamOp.Flush);
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)StreamOp.Read);
                write.Write(count);
                using (var read = new BinaryReader(ServerManager.GetResponseStream(write.Send())))
                {
                    int ret = read.ReadInt32();
                    byte[] buf = read.ReadBytes(ret);
                    Array.Copy(buf, 0, buffer, offset, buf.Length);
                    return ret;
                }
            }
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)StreamOp.Seek);
                write.Write(offset);
                write.Write((int)origin);
                using (var read = new BinaryReader(ServerManager.GetResponseStream(write.Send())))
                    return read.ReadInt64();
            }
        }
        
        public override void SetLength(long value)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)StreamOp.SetLength);
                write.Write(value);
                write.Send();
            }
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            using (var write = new ServerStream(ServerMessageType.STREAM_OP))
            {
                write.Write(streamId);
                write.Write((byte)StreamOp.Write);
                write.Write(Math.Min(count, buffer.Length - offset));
                write.Write(buffer, offset, count);
                write.Send();
            }
        }
        
        public void Dispose()
        {
            action(StreamOp.Dispose);
        }
        
        public RemoteStream(int streamid)
        {
            this.streamId = streamid;
        }
    }
}
