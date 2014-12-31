using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum Echo
    {
        None,   // No echo at all
        All,    // All user input will be echoed back
        Mask,   // A mask character will be echoed back
    }

    public class Client
    {
        public virtual void Send(String message) { }
        public virtual void Disconnect() { }
		public virtual String ConnectionDescription { get { throw new NotImplementedException(); } }
        public virtual String IPString { get { throw new NotImplementedException(); } }

        public bool IsLoggedOn { get { return Player != null; } }
        public Account Account;
        public Player Player;
        public int Rank = 0;
		public ClientCommandHandler CommandHandler;
        public DateTime TimeOfLastCommand = DateTime.Now;

        public bool IsAfk
        {
            get
            {
                return (DateTime.Now - TimeOfLastCommand) > TimeSpan.FromMilliseconds(MudObject.SettingsObject.AFKTime);
            }
        }

        protected Echo _myEcho = Echo.All;
        public virtual Echo Echo { get { return _myEcho; } set { _myEcho = value; } }

    }
}
