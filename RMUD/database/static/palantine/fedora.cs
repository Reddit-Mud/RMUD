public class fedora : RMUD.MudObject
{
    public override void Initialize()
    {
        Short = "fedora";
        Nouns.Add("fedora", "hat");
        Long = "This hat is so not cool.";

        AddActionRule<RMUD.MudObject, RMUD.MudObject>("can-be-worn").Do((a, b) => RMUD.RuleResult.Allow);
    }
}