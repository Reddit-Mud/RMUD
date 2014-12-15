using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum PerformResult
    {
        Allow = 0,
        Default = 1,
        Continue = 1,
        Stop = 2,
        Disallow = 2,
    }

    public enum CheckResult
    {
        Allow = 0,
        Continue = 1,
        Disallow = 2
    }
}
