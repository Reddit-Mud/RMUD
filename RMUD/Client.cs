using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Client
    {
        public virtual void ImplementSend(String message) { }
        public virtual void Disconnect() { }

        public Actor Player;
		public IClientCommandHandler CommandHandler;

        public void Send(String message)
        {
            var realMessage = new StringBuilder();
            int p = 0;
            while (true)
            {
                if (p >= message.Length) break;
                if (message[p] == '\\')
                {
                    ++p;
                    if (p >= message.Length) break;
                    if (message[p] == 'n') realMessage.Append("\n\r");
                    else if (message[p] == 't') realMessage.Append("\t");
                    else realMessage.Append(message[p]);
                }
                else if (message[p] == '^')
                {
                    ++p;
                    if (p >= message.Length) break;
                    realMessage.Append((new String(message[p], 1)).ToUpperInvariant());
                }
				else if (message[p] == '\n')
				{
					realMessage.Append("\n\r");
				}
				else
					realMessage.Append(message[p]);

                ++p;
            }

            ImplementSend(realMessage.ToString());
        }
    }
}
