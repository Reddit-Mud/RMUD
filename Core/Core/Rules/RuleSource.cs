using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public interface RuleSource
    {
        RuleSet Rules { get; }
        RuleSource LinkedRuleSource { get; }
    }
}
