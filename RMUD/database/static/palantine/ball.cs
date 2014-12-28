public class ball : RMUD.MudObject, RMUD.Startup
{
    public ball() : base("ball", "This is a small dirty ball.") { }


    public void Startup()
    {
        RMUD.Mud.DefaultParser.AddCommand(RMUD.CommandFactory.KeyWord("BOUNCE"))
            .ProceduralRule((match, actor) =>
            {
                RMUD.Mud.SendMessage(actor, "Database defined commands appear to work.");
                return RMUD.PerformResult.Continue;
            });
    }
}