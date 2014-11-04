public class fedora : RMUD.MudObject, RMUD.WearableRules
{
    public override void Initialize()
    {
        Short = "fedora";
        Nouns.Add("fedora", "hat");
        Long = "This hat is so not cool.";

    }

    RMUD.CheckRule RMUD.WearableRules.CheckWear(RMUD.Actor Actor)
    {
        return RMUD.CheckRule.Allow();
    }

    RMUD.CheckRule RMUD.WearableRules.CheckRemove(RMUD.Actor Actor)
    {
        throw new System.NotImplementedException();
    }

    RMUD.RuleHandlerFollowUp RMUD.WearableRules.HandleWear(RMUD.Actor Actor)
    {
        return RMUD.RuleHandlerFollowUp.Continue;
    }

    RMUD.RuleHandlerFollowUp RMUD.WearableRules.HandleRemove(RMUD.Actor Actor)
    {
        throw new System.NotImplementedException();
    }
}