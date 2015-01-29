using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.SinglePlayer
{
    class DummyClient : RMUD.Client
    {
        public Action<String> Output;

        public DummyClient(Action<String> Output)
        {
            this.Output = Output;
        }

        public override void Send(string message)
        {
            Output(message);
        }

        public override void Disconnect()
        {

        }
    }
}
