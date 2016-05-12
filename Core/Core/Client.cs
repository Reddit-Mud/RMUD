using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class Client
    {
        public virtual void Send(String message) { }
        public virtual void Disconnect() { }
        public virtual String ConnectionDescription { get { return "?"; } }

        public MudObject Player;
    }
}
