using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class SceneryRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.Check<MudObject, MudObject>("can take?")
                .When((actor, thing) => thing is Scenery)
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "That's a terrible idea.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take scenery rule.");
        }
    }

	public class Scenery : MudObject
	{
        public Scenery() { }

        public Scenery(String Short, String Long) : base(Short, Long) { }
	}
}
