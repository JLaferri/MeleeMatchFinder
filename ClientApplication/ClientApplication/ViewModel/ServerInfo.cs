using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApplication.ViewModel
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ServerInfo(string name, string ipAddress, int port)
        {
            Name = name;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
