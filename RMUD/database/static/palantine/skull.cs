public class skull : RMUD.MudObject
{
    [RMUD.Persist]
    public int ExamineCount { get; set; }

    public override void Initialize()
    {
        RMUD.Mud.PersistInstance(this);

        Short = "human skull";
        Nouns.Add("human", "skull");

        Long = new RMUD.DescriptiveText((actor, owner) =>
        {
            ExamineCount += 1;
            return string.Format("How many times? {0} times.", ExamineCount);
        });
    }

}