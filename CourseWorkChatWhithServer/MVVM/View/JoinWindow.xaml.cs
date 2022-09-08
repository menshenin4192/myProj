using CourseWorkChatWhithServer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CourseWorkChatWithServer.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для JoinWindow.xaml
    /// </summary>
    public partial class JoinWindow : Window
    {
        public JoinWindow()
        {
            InitializeComponent();
            ConnectBtn.IsEnabled = false;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //MainViewModel mainViewModel = new MainViewModel(usernameTB.Text);
            Application.Current.Resources.Add("username", usernameTB.Text);
            MainWindow mainWindow = new();
            mainWindow.Show();
           
        }

        private void usernameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (usernameTB.Text.Length == 0) ConnectBtn.IsEnabled = false;
            else ConnectBtn.IsEnabled = true;
        }
    }
}
