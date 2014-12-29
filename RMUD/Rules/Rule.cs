using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RulePriority
    {
        First = 0,
        Neutral = 1,
        Last = 2,
        Delete = 3
    }

    public class Rule 
    {
        public String DescriptiveName;
        public String ID;
        public RuleDelegateWrapper<bool> WhenClause;
        public RulePriority Priority = RulePriority.Neutral;
    }

    public class Rule<RT> : Rule
    {
        public RuleDelegateWrapper<RT> BodyClause;
    }
}
