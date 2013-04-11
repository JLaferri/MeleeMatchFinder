using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ContractObjects;
using System.ServiceModel;
using ClientApplication.Properties;
using ClientApplication.ViewModel;

namespace ClientApplication.Model
{
    public class GameLoader : IDisposable
    {
        public string ServerIp { get; private set;}
        public int ServerPort { get; private set; }
        public ObservableCollection<GameInfo> Games { get; private set; }

        private IGameManagerService channel;

        public GameLoader()
        {
            Games = new ObservableCollection<GameInfo>();
        }

        public void SwitchServer(string ip, int port)
        {
            //Do nothing if information not new
            if (ip == ServerIp && port == ServerPort) return;

            Games.Clear();

            ServerIp = ip;
            ServerPort = port;

            closeCurrentChannel();
            
            //Connect to new server and store channel
            var binding = new NetTcpBinding(SecurityMode.None);
            var address = new EndpointAddress((new UriBuilder("net.tcp", ip, port, "GameManager")).Uri);

            var callback = new GameInformationReceiver(Games);
            channel = DuplexChannelFactory<IGameManagerService>.CreateChannel(callback, binding, address);

            //Attempt connection and game list retrieval
            var gameList = channel.Connect(Settings.Default.PlayerName, Settings.Default.HostingPort);

            //Load games into observable collection
            foreach (var game in gameList) Games.Add(new GameInfo(game));
        }

        public GameInfo CreateGame()
        {
            var createdGame = channel.CreateGame("abcd");
            GameInfo gameInfo;

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
