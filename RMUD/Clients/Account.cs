using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Account
    {
        public String UserName;
        public String HashedPassword;
        public String AFKMessage = "AFK";

        public Actor LoggedInCharacter;
    }
}
