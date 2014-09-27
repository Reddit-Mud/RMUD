using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum CheckRuleResponse
    {
        Allow,
        Disallow
    }

    public class CheckRule
    {
        public CheckRuleResponse Response;
        public String ReasonDisallowed;

        public bool Allowed { get { return Response == CheckRuleResponse.Allow; } }

        public static CheckRule Allow() { return new CheckRule { Response = CheckRuleResponse.Allow }; }
        public static CheckRule Disallow(String Reason)
        {
            return new CheckRule { Response = CheckRuleResponse.Disallow, ReasonDisallowed = Reason };
        }        
    }
}
