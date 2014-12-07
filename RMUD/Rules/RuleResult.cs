using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RuleResult
    {
        Allow = 0,
        Default = 1,
        Continue = 1,
        Stop = 2,
        Disallow = 2,
    }
}
