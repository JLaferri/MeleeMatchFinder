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
        public ObservableCollection<PlayerInfo> Players { get; set; }
        [DataMember]
        public string DolphinVersion { get; set; }

        public SynchronizedGame(string dolphinVersion)
        {
            GameId = Guid.NewGuid();

            Players = new ObservableCollection<PlayerInfo>();

            DolphinVersion = dolphinVersion;
        }
    }
}