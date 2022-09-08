using CourseWorkChatWithServer.Core;
using CourseWorkChatWithServer.MVVM.Model;
using CourseWorkChatWithServer.Net;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CourseWorkChatWithServer.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }

        public RelayCommand SendCommand { get; set; }
        private Server _server;

        private string _username;
        public string Username
        {
            get { return _username; }
            set 
            { 
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set 
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<MessageModel>();
            _server = new Server();

            Username = Application.Current.Resources["username"].ToString();

            _server.connectedEvent += UserConnected;
            _server.msgRecievedEvent += MessageRecieved;
            _server.UserDisconnectedEvent += UserDisconnected;
            _server.sysInfoRecievedEvent += SysInfoRecieved;

            _server.Connect2Server(Username);

           

            SendCommand = new RelayCommand(o =>
            {
                _server.SendMessage2Server(new MessageModel
                {
                    Message = _message.Trim(),
                    Time = DateTime.Now.ToString()
                });
                Message = "";
            });
        }

        private void SysInfoRecieved()
        {
            string message = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new MessageModel
            {
                Message = message,
                SysInfo = true
            }));
        }

        private void UserDisconnected()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageRecieved()
        {
            string user = _server.PacketReader.ReadMessage();
            string msg = _server.PacketReader.ReadMessage();
            string time = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new MessageModel
            {
                Username = user,
                Message = msg,
                Time = time
            }));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
