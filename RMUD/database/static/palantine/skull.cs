public class skull : RMUD.MudObject
{
    [RMUD.Persist]
    public int ExamineCount { get; set; }

    public override void Initialize()
    {
        RMUD.Mud.PersistInstance(this);

        Short = "human skull";
        Nouns.Add("human", "skull");

        Perform<RMUD.MudObject, RMUD.MudObject>("describe")
            .Do((viewer, thing) =>
            {
                ExamineCount += 1;
                RMUD.Mud.SendMessage(viewer, string.Format("How many times? {0} times.", ExamineCount));
                return RMUD.PerformResult.Continue;
            });
    }

}