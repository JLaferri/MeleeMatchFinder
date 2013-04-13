using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ContractObjects
{
    [DataContract]
    public class SynchronizedGame
    {
        [DataMember]
        public Guid GameId { get; private set; }

        [DataMember]
        public string LobbyName { get; private set; }

        [DataMember]
        public int PlayerLimit { get; private set; }

        [DataMember]
        public DolphinOptions Options { get; private set; }

        [DataMember]
        public ObservableCollection<PlayerInfo> Players { get; set; }

        public SynchronizedGame(string lobbyName, int playerLimit, DolphinOptions options)
        {
            GameId = Guid.NewGuid();

            Players = new ObservableCollection<PlayerInfo>();

            LobbyName = lobbyName;
            PlayerLimit = playerLimit;
            Options = options;
        }
    }
}