namespace Space
{
    public class Room : RMUD.Room
    {
        public int TimesViewed = 0;
        public string QuickDescription;

        public override void Initialize()
        {
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<RMUD.MudObject, Room>("describe")
                .When((viewer, item) => item.TimesViewed == 0)
                .Do((viewer, item) =>
                {
                    RMUD.MudObject.SendMessage(viewer, item.Long);
                    item.TimesViewed += 1;
                    return RMUD.PerformResult.Stop;
                })
                .Name("First visit description rule.");

            GlobalRules.Perform<RMUD.MudObject, Room>("describe")
                .When((viewer, item) => item.TimesViewed > 0)
                .Do((viewer, item) =>
                {
                    RMUD.MudObject.SendMessage(viewer, item.QuickDescription);
                    item.TimesViewed += 1;
                    return RMUD.PerformResult.Stop;
                })
                .Name("Visit again description rule.");
        }
    }   
}