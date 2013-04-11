using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ContractObjects
{
    [DataContract]
    public class PlayerInfo
    {
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string PlayerIp { get; private set; }

        [DataMember]
        public int HostingPort { get; private set; }

        public PlayerInfo(string name, string ip, int port)
        {
            Name = name;
            PlayerIp = ip;
            HostingPort = port;
        }
    }
}
