using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ContractObjects;
using System.ServiceModel;
using ClientApplication.Properties;
using ClientApplication.ViewModel;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace ClientApplication.Model
{
    public class GameLoader : IDisposable
    {
        public string ServerIp { get; private set;}
        public int ServerPort { get; private set; }
        public ObservableCollection<GameInfo> Games { get; private set; }

        private readonly Subject<string> serverMessageNotifier = new Subject<string>();
        private readonly Subject<string> lobbyMessageNotifier = new Subject<string>();

        public IObservable<string> ServerMessageStream { get { return serverMessageNotifier.AsObservable(); } }
        public IObservable<string> LobbyMessageStream { get { return lobbyMessageNotifier.AsObservable(); } }

        private IGameManagerService channel;

        public GameLoader()
        {
            Games = new ObservableCollection<GameInfo>();
        }

        public void DisconnectFromServer()
        {
            Games.Clear();

            closeCurrentChannel();
        }

        public void ConnectToServer(string ip, int port)
        {
            if (string.IsNullOrWhiteSpace(ip) || port < 0) return;

            ServerIp = ip;
            ServerPort = port;
            
            //Connect to new server and store channel
            var binding = new NetTcpBinding(SecurityMode.None);
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = TimeSpan.MaxValue;

            var address = new EndpointAddress((new UriBuilder("net.tcp", ip, port, "GameManager")).Uri);

            var callback = new GameInformationReceiver(Games, serverMessageNotifier, lobbyMessageNotifier);
            channel = DuplexChannelFactory<IGameManagerService>.CreateChannel(callback, binding, address);

            //Attempt connection and game list retrieval
            var gameList = channel.Connect(Settings.Default.PlayerName, Settings.Default.HostingPort);

            //Load games into observable collection
            foreach (var game in gameList) Games.Add(new GameInfo(game));
        }

        public GameInfo CreateGame(LobbySettings settings)
        {
            var dolphinOptions = new DolphinOptions(settings.SelectedFpsMode, settings.SelectedCpuMode, settings.DolphinVersion);

            var createdGame = channel.CreateGame(settings.LobbyName, settings.MaxPlayers, dolphinOptions);
            GameInfo gameInfo;

            //Wait for server to propagate the server back and find that object
            while ((gameInfo = Games.FirstOrDefault(gi => gi.Game.GameId == createdGame.GameId)) == null);

            return gameInfo;
        }

        public void JoinGame(GameInfo game)
        {
            channel.JoinGame(game.Game);
        }

        public void LeaveGame(GameInfo game)
        {
            channel.LeaveGame(game.Game);
        }

        public void SendServerMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            channel.SendServerMessage(message);
        }

        public void SendLobbyMessage(string message, GameInfo game)
        {
            if (string.IsNullOrWhiteSpace(message) || game == null) return;

            channel.SendLobbyMessage(message, game.Game);
        }

        private void closeCurrentChannel()
        {
            //Close current channel if it is already active
            if (channel != null)
            {
                var castedChannel = (ICommunicationObject)channel;
                try
                {
                    castedChannel.Close();
                }
                catch (CommunicationException)
                {
                    castedChannel.Abort();
                }
            }
        }

        public void Dispose()
        {
            closeCurrentChannel();
        }
    }
}
