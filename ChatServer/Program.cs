using ChatServer.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

//new update
namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _tcpListener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 6666);
            _tcpListener.Start();

            while (true)
            {
                var client = new Client(_tcpListener.AcceptTcpClient());
                _users.Add(client);

                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteString(usr.Username);
                    broadcastPacket.WriteString(usr.UID.ToString());

                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string time, string message, string username)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteString(username);
                msgPacket.WriteString(message);
                msgPacket.WriteString(time);

                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastMessage(string sysinfo)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(6);
                msgPacket.WriteString(sysinfo);

                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteString(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            BroadcastMessage($"{disconnectedUser.Username} disconnected!");
        }
    }
}
