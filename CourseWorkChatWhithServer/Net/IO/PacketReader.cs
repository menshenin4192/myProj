using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CourseWorkChatWithServer.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length * 2];
            _ns.Read(msgBuffer, 0, length * 2);

            //var msg = Encoding.ASCII.GetString(msgBuffer);
            var msg = Encoding.Unicode.GetString(msgBuffer);
            return msg;
        }
    }
}
