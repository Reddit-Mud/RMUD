using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Client
    {
        public virtual void Send(String message) { }
        public virtual void Disconnect() { }
		public virtual String ConnectionDescription { get { throw new NotImplementedException(); } }
        public virtual String IPString { get { throw new NotImplementedException(); } }

        public bool IsLoggedOn { get { return Player != null; } }
        public Account Account;
        public Actor Player;
        public int Rank = 0;
		public ClientCommandHandler CommandHandler;
        public DateTime TimeOfLastCommand = DateTime.Now;

        public bool IsAfk
        {
            get
            {
                return (DateTime.Now - TimeOfLastCommand) > TimeSpan.FromMilliseconds(Mud.SettingsObject.AFKTime);
            }
        }
    }
}
