using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRuleEngine
{
    public interface RuleSource
    {
        RuleSet Rules { get; }
        RuleSource LinkedRuleSource { get; }
    }
}
