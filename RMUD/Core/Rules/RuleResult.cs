using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum PerformResult
    {
        Continue = 1,
        Stop = 2,
    }

    public enum CheckResult
    {
        Allow = 0,
        Continue = 1,
        Disallow = 2
    }
}
