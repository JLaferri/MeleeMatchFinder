using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContractObjects;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ServerApplication
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class GameManagerService : IGameManagerService
    {
        private readonly List<SynchronizedGame> gameList = new List<SynchronizedGame>();
        private readonly Dictionary<IGameManagerCallback, PlayerInfo> playerList = new Dictionary<IGameManagerCallback, PlayerInfo>();

        public SynchronizedGame[] Connect(string name, int hostingPort)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();
            var clientEndpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            var player = new PlayerInfo(name, clientEndpoint.Address, hostingPort);
            playerList[callback] = player;

            //When this object gets closed, remove player from all lobbies 
            OperationContext.Current.Channel.Closed += (sender, e) =>
            {
                var lobbiesWithPlayer = gameList.Where(game => game.Players.Contains(player)).ToArray();
                foreach (var lobby in lobbiesWithPlayer) leaveGame(callback, lobby);

                playerList.Remove(callback);
            };

            Console.WriteLine("New player connected");

            return gameList.ToArray();
        }

        public SynchronizedGame CreateGame(string dolphinVersion)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();

            var newGame = new SynchronizedGame(dolphinVersion);
            newGame.Players.Add(playerList[callback]);

            gameList.Add(newGame);

            //Propagate change to all users
            propagateChanges(newGame);

            return newGame;
        }

        public string JoinGame(SynchronizedGame game)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();

            game = getGameByGuid(game.GameId);

            if (game == null) return "Game no longer exists.";

            game.Players.Add(playerList[callback]);

            //Propagate change to all users
            propagateChanges(game);

            return null;
        }

        public string LeaveGame(SynchronizedGame game)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();

            return leaveGame(callback, game);
        }

        private string leaveGame(IGameManagerCallback callback, SynchronizedGame game)
        {
            game = getGameByGuid(game.GameId);

            if (game == null) return "Game no longer exists.";

            game.Players.Remove(playerList[callback]);

            //If no one left in game, remove it
            if (game.Players.Count == 0)
            {
                gameList.Remove(game);
            }

            propagateChanges(game);

            return null;
        }

        private SynchronizedGame getGameByGuid(Guid guid)
        {
            return gameList.Where(gs => gs.GameId == guid).FirstOrDefault();
        }

        private void propagateChanges(SynchronizedGame game)
        {
            var disconnectedPlayers = new List<IGameManagerCallback>();

            //Propagate change to all users
            foreach (var callbackChannel in playerList.Keys)
            {
                try
                {
                    callbackChannel.PropagateChange(game);
                }
                catch (Exception ex)
                {
                    if (ex is CommunicationException || ex is TimeoutException || ex is ObjectDisposedException)
                    {
                        disconnectedPlayers.Add(callbackChannel);
                    }
                    else throw;
                }
            }

            //Remove all players that could not be contacted
            foreach (var keyToRemove in disconnectedPlayers) playerList.Remove(keyToRemove);
        }
    }
}
