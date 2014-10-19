public class skull : RMUD.MudObject
{
    public int ExamineCount = 0;

    public override void Initialize()
    {
        RMUD.Mud.PersistInstance(this);
        if (PersistenceObject.Data.ContainsKey("EC"))
            ExamineCount = System.Convert.ToInt32(PersistenceObject.Data["EC"]);

        Short = "human skull";
        Nouns.Add("human", "skull");

        Long = new RMUD.DescriptiveText((actor, owner) =>
        {
            ExamineCount += 1;
            PersistenceObject.Data.Upsert("EC", ExamineCount.ToString());

            return string.Format("How many times? {0} times.", ExamineCount);
        });
    }

}