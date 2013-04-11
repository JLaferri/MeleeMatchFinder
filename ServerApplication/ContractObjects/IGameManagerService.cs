﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContractObjects
{
    [ServiceContract(CallbackContract = typeof(IGameManagerCallback))]
    public interface IGameManagerService
    {
        [OperationContract]
        SynchronizedGame[] Connect(string name, int hostingPort);

        [OperationContract]
        SynchronizedGame CreateGame(string dolphinVersion);

        [OperationContract]
        string JoinGame(SynchronizedGame game);

        [OperationContract]
        string LeaveGame(SynchronizedGame game);

        //[OperationContract(IsOneWay = true)]
        //void SendMessage(string message);
    }
}
