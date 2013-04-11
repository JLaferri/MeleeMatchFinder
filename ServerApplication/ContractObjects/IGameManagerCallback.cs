using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ContractObjects
{
    public interface IGameManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void PropagateChange(SynchronizedGame game);

        //[OperationContract(IsOneWay = true)]
        //void PropagateMessage(string message);
    }
}
