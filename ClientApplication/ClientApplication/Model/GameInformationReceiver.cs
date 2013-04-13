using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ClientApplication.ViewModel;
using System.Reactive.Subjects;

namespace ClientApplication.Model
{
    public class GameInformationReceiver : ContractObjects.IGameManagerCallback
    {
        private ObservableCollection<GameInfo> collection;
        private Subject<string> serverMessageNotifier;
        private Subject<string> lobbyMessageNotifier;

        public GameInformationReceiver(ObservableCollection<GameInfo> collection, Subject<string> serverMessageNotifier, Subject<string> lobbyMessageNotifier)
        {
            this.collection = collection;
            this.serverMessageNotifier = serverMessageNotifier;
            this.lobbyMessageNotifier = lobbyMessageNotifier;
        }

        public void PropagateChange(ContractObjects.SynchronizedGame game)
        {
            var localGame = collection.Where(g => g.Game.GameId == game.GameId).FirstOrDefault();

            if (localGame == null) 
            {
                localGame = new GameInfo(game); 
                collection.Add(localGame);
            }
            else localGame.Game = game;

            if (localGame.Game.Players.Count == 0) collection.Remove(localGame);
        }

        public void PropagateServerMessage(string message)
        {
            serverMessageNotifier.OnNext(message);
        }

        public void PropagateLobbyMessage(string message)
        {
            lobbyMessageNotifier.OnNext(message);
        }
    }
}
