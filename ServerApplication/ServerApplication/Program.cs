using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ContractObjects;

namespace ServerApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new GameManagerService();

            ServiceHost svh = new ServiceHost(instance);
            svh.AddServiceEndpoint(typeof(IGameManagerService), new NetTcpBinding(SecurityMode.None), "net.tcp://localhost:2626/GameManager");
            svh.Open();

            Console.WriteLine("Server Started");
            Console.ReadKey();

            svh.Close();
        }
    }
}
