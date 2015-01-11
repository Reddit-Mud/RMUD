public class ball : RMUD.MudObject
{
    public ball() : base("ball", "This is a small dirty ball.") { }


    public static void AtStartup(RMUD.RuleEngine GlobalRules)
    {
        RMUD.Core.DefaultParser.AddCommand(RMUD.CommandFactory.KeyWord("BOUNCE"))
            .ProceduralRule((match, actor) =>
            {
                SendMessage(actor, "Database defined commands appear to work.");
                return RMUD.PerformResult.Continue;
            });
    }
}