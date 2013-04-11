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

namespace ClientApplication.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        public ICollectionView ServerCollectionView { get; private set; }
        public ICollectionView GameListView { get; private set; }

        public ICommand CreateGame { get; private set; }
        public ICommand JoinGame { get; private set; }
        public ICommand LeaveGame { get; private set; }

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
            //serverCollection.Add(new ServerInfo("Fizzi's Test Server", "12.123.123.123", 1234));

            ServerCollectionView = CollectionViewSource.GetDefaultView(serverCollection);
            GameListView = CollectionViewSource.GetDefaultView(gameLoader.Games);

            ServerCollectionView.CurrentChanged += (sender, e) =>
            {
                var selected = (ServerInfo)ServerCollectionView.CurrentItem;
                if (selected == null) return;

                //Connect to server to enumerate the games it is managing
                gameLoader.SwitchServer(selected.IpAddress, selected.Port);
            };

            //Set up Create Game button click event
            CreateGame = Command.CreateAsync(() => true, () =>
            {
                var createdGame = gameLoader.CreateGame();

                CurrentGameLobby = createdGame;
            }, () => IsBusy = true, () => 
            {
                GameListView.MoveCurrentTo(CurrentGameLobby);
                IsBusy = false;
            });

            //Set up Join Game button click event
            JoinGame = Command.CreateAsync(() => true, () =>
            {
                var gameInfo = (GameInfo)GameListView.CurrentItem;
                if (gameInfo == null) return;

                gameLoader.JoinGame(gameInfo);
                CurrentGameLobby = gameInfo;
            }, () => IsBusy = true, () => IsBusy = false);

            //Set up Leave Game button click event
            LeaveGame = Command.CreateAsync(() => true, () =>
            {
                var gameInfo = CurrentGameLobby;
                if (gameInfo == null) return;

                gameLoader.LeaveGame(gameInfo);
                CurrentGameLobby = null;
            }, () => IsBusy = true, () => IsBusy = false);
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
