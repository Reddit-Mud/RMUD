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
        Last = 2
    }

    public class Rule 
    {
        public String DescriptiveName;
        public String ID;
        public RuleDelegateWrapper<bool> WhenClause;
        public RulePriority Priority;
    }

    public class Rule<RT> : Rule
    {
        public RuleDelegateWrapper<RT> BodyClause;
    }
}
