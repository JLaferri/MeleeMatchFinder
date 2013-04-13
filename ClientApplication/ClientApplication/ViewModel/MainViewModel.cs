using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ClientApplication.Common;
using ContractObjects;
using ClientApplication.Model;
using ClientApplication.Properties;
using System.Reactive.Linq;

namespace ClientApplication.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        public ICollectionView ServerCollectionView { get; private set; }
        public ICollectionView GameListView { get; private set; }

        public ICommand CreateGame { get; private set; }
        public ICommand JoinGame { get; private set; }
        public ICommand LeaveGame { get; private set; }

        public ChatViewModel ServerChatViewModel { get; private set; }
        public ChatViewModel LobbyChatViewModel { get; private set; }

        public LobbySettings CreateLobbySettings { get; private set; }

        private bool _isBusy = false;
        public bool IsBusy { get { return _isBusy; } set { this.RaiseAndSetIfChanged("IsBusy", ref _isBusy, value, PropertyChanged); } }

        private GameInfo _currentGameLobby = null;
        public GameInfo CurrentGameLobby { get { return _currentGameLobby; } set { this.RaiseAndSetIfChanged("CurrentGameLobby", ref _currentGameLobby, value, PropertyChanged); } }

        private readonly GameLoader gameLoader = new GameLoader();

        public MainViewModel()
        {
            //Update settings if new version
            if (Settings.Default.NewVersion)
            {
                Settings.Default.Upgrade();
                Settings.Default.NewVersion = false;
                Settings.Default.Save();
            }

            var serverCollection = new ObservableCollection<ServerInfo>();
            serverCollection.Add(new ServerInfo("Fizzi's Test Server", "192.168.1.101", 58198));

            ServerCollectionView = CollectionViewSource.GetDefaultView(serverCollection);
            GameListView = CollectionViewSource.GetDefaultView(gameLoader.Games);

            CreateLobbySettings = new LobbySettings();
            CreateLobbySettings.LobbyName = "Lobby Name";
            CreateLobbySettings.DolphinVersion = "3.5-1206";
            CreateLobbySettings.MaxPlayers = 2;
            CreateLobbySettings.SelectedCpuMode = CpuMode.Single;
            CreateLobbySettings.SelectedFpsMode = FpsMode.FPS60;

            ServerChatViewModel = new ChatViewModel(message => gameLoader.SendServerMessage(message));
            LobbyChatViewModel = new ChatViewModel(message => gameLoader.SendLobbyMessage(message, CurrentGameLobby));

            //Listen for server message observable to add chat messages
            gameLoader.ServerMessageStream.ObserveOn(System.Threading.SynchronizationContext.Current).Subscribe(msg => ServerChatViewModel.MessageCollection.Add(msg));

            //Listen for lobby message observable to add chat messages
            gameLoader.LobbyMessageStream.ObserveOn(System.Threading.SynchronizationContext.Current).Subscribe(msg => LobbyChatViewModel.MessageCollection.Add(msg));

            ServerCollectionView.CurrentChanged += (sender, e) =>
            {
                var selected = (ServerInfo)ServerCollectionView.CurrentItem;
                if (selected == null) return;

                //Connect to server to enumerate the games it is managing
                gameLoader.SwitchServer(selected.IpAddress, selected.Port);

                //Write about the event in chat box
                ServerChatViewModel.MessageCollection.Add(string.Format("<< CONNECTED TO SERVER \"{0}\" >>", selected.Name));
            };

            //Set up Create Game command
            CreateGame = Command.CreateAsync(() => true, () =>
            {
                var createdGame = gameLoader.CreateGame(CreateLobbySettings);

                CurrentGameLobby = createdGame;
            }, () => IsBusy = true, () => 
            {
                //Write about event in lobby chat box
                LobbyChatViewModel.MessageCollection.Add(string.Format("<< CONNECTED TO LOBBY \"{0}\" >>", CurrentGameLobby.Game.LobbyName));

                GameListView.MoveCurrentTo(CurrentGameLobby);
                IsBusy = false;
            });

            //Set up Join Game command
            JoinGame = Command.CreateAsync(() => true, () =>
            {
                var gameInfo = (GameInfo)GameListView.CurrentItem;
                if (gameInfo == null) return;

                gameLoader.JoinGame(gameInfo);
                CurrentGameLobby = gameInfo;
            }, () => IsBusy = true, () =>
            {
                //Write about event in lobby chat box
                LobbyChatViewModel.MessageCollection.Add(string.Format("<< CONNECTED TO LOBBY \"{0}\" >>", CurrentGameLobby.Game.LobbyName));

                IsBusy = false;
            });

            //Set up Leave Game command
            LeaveGame = Command.CreateAsync(() => true, () =>
            {
                var gameInfo = CurrentGameLobby;
                if (gameInfo == null) return;

                gameLoader.LeaveGame(gameInfo);
                CurrentGameLobby = null;
            }, () => IsBusy = true, () =>
            {
                //Write about event in lobby chat box
                LobbyChatViewModel.MessageCollection.Add("<< DISCONNECTED FROM LOBBY >>");

                IsBusy = false;
            });
        }

        public void Dispose()
        {
            if (gameLoader != null)
            {
                gameLoader.Dispose();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
