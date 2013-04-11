using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ClientApplication.ViewModel;

namespace ClientApplication.Model
{
    public class GameInformationReceiver : ContractObjects.IGameManagerCallback
    {
        private ObservableCollection<GameInfo> collection;

        public GameInformationReceiver(ObservableCollection<GameInfo> collection)
        {
            this.collection = collection;
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
    }
}
