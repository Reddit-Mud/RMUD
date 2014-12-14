using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class SceneryRules : DeclaresRules
    {
        public void InitializeGlobalRules()
        {
            GlobalRules.AddCheckRule<MudObject, MudObject>("can-take").When((actor, thing) => thing is Scenery).Do((actor, thing) =>
                {
                    Mud.SendMessage(actor, "That's a terrible idea.");
                    return CheckResult.Disallow;
                });
        }
    }

	public class Scenery : MudObject
	{
        public Scenery() { }

        public Scenery(String Short, String Long) : base(Short, Long) { }
	}
}
