using System;

namespace RMUD.Modules.Network
{
    public class Account
    {
        public String UserName;
        public String HashedPassword;
        public String Salt;
        public String AFKMessage = "AFK";

        [Newtonsoft.Json.JsonIgnore]
        public Actor LoggedInCharacter;
    }
}
