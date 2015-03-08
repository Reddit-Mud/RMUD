using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRuleEngine
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

        public bool CheckWhenClause(Object[] Arguments)
        {
            if (WhenClause == null) return true;
            return WhenClause.Invoke(Arguments);
        }

        public virtual Type[] GetArgumentTypes()
        {
            throw new NotImplementedException();
        }
    }

    public class Rule<RT> : Rule
    {
        public RuleDelegateWrapper<RT> BodyClause;
        
        public bool AreArgumentsCompatible(Object[] Arguments)
        {
            if (BodyClause == null) return false;
            return BodyClause.AreArgumentsCompatible(Arguments);
        }

        public override Type[] GetArgumentTypes()
        {
            if (BodyClause == null) return new Type[]{};
            var genericTypes = BodyClause.GetType().GetGenericArguments();
            return genericTypes.Take(genericTypes.Length - 1).ToArray();
        }
    }
}
