using RMUD;
using StandardActionsModule;

namespace Akko.Areas.Prologue
{
    public class Watch : MudObject
    {
        public override void Initialize()
        {
            SimpleName("pocket watch");
            Long = "This simple gold watch belonged to your father. It still keeps great time.";

            this.CheckCanDrop().Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "That is all you've got left of your father. You don't want to lose it.");
                    return SharpRuleEngine.CheckResult.Disallow;
                });

            this.PerformDescribe().First.Do((viewer, thing) =>
                {
                    thing.SetProperty("has-been-viewed", true);
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}
