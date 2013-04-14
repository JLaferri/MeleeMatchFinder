using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApplication.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = (ViewModel.MainViewModel)this.DataContext;

            viewModel.LobbyChatViewModel.ChatView.CollectionChanged += (sender, e) =>
            {
                LobbyChatScrollViewer.ScrollToBottom();
            };

            viewModel.ServerChatViewModel.ChatView.CollectionChanged += (sender, e) =>
            {
                ServerChatScrollViewer.ScrollToBottom();
            };
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (ViewModel.MainViewModel)this.DataContext;

            var configureWindow = new LobbyConfigurationWindow();
            configureWindow.DataContext = viewModel;

            var checkVal = configureWindow.ShowDialog();
            if (checkVal == true)
            {
                viewModel.CreateGame.Execute(null);
            }
        }

        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                var chatViewModel = (ViewModel.ChatViewModel)textBox.DataContext;

                if (chatViewModel.SendMessage.CanExecute(null)) chatViewModel.SendMessage.Execute(null);
            }
        }
    }
}
