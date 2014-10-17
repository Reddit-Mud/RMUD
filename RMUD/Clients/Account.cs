using System;

namespace RMUD
{
    public class Account
    {
        public String UserName;
        public String HashedPassword;
        public String Salt;
        public String Character;
        public String AFKMessage = "AFK";

        public Actor LoggedInCharacter;
    }
}
