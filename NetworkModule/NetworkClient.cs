using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
{
    public class NetworkClient : Client
    {
		public override String ConnectionDescription { get { throw new NotImplementedException(); } }
        public virtual String IPString { get { throw new NotImplementedException(); } }

        

        public bool IsLoggedOn { get; set; }
        public DateTime TimeOfLastCommand = DateTime.Now;

        public bool IsAfk
        {
            get
            {
                return (DateTime.Now - TimeOfLastCommand) > TimeSpan.FromMilliseconds(Core.SettingsObject.AFKTime);
            }
        }

        public NetworkClient()
        {
            IsLoggedOn = false;
        }

    }
}
