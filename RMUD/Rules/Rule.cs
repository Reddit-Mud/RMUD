using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Rule { }

    public class Rule<RT> : Rule
    {
        public String DescriptiveName;
        public String ID; 
        public RuleDelegateWrapper<bool> WhenClause;
        public RuleDelegateWrapper<RT> BodyClause;
    }
}
