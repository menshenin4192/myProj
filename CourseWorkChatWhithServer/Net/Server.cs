using CourseWorkChatWithServer.MVVM.Model;
using CourseWorkChatWithServer.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CourseWorkChatWithServer.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgRecievedEvent;
        public event Action sysInfoRecievedEvent;
        public event Action UserDisconnectedEvent;

        public Server()
        {
            _client = new TcpClient();

        }

        public void Connect2Server(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 6666);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgRecievedEvent?.Invoke();
                            break;
                        case 6:
                            sysInfoRecievedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectedEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Recieved an unexisting opcode in [Server.cs]");
                            break;
                    }
                }
            });
        }

        public void SendMessage2Server(MessageModel message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteString(message.Time);
            messagePacket.WriteString(message.Message);

            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
