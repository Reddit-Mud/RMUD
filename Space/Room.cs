﻿namespace Space
{
    public enum AirLevel
    {
        Vacuum,
        Atmosphere
    }

    public class Room : RMUD.Room
    {
        public int TimesViewed = 0;
        public string BriefDescription;
        public AirLevel AirLevel = AirLevel.Atmosphere;

        public override void Initialize()
        {
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<RMUD.MudObject, Room>("describe")
                .Do((viewer, item) =>
                {
                    var auto = RMUD.Core.ExecutingCommand.ValueOrDefault("AUTO", false);

                    if (item.TimesViewed > 0 && auto)
                        RMUD.MudObject.SendMessage(viewer, item.BriefDescription);
                    else
                        RMUD.MudObject.SendMessage(viewer, item.Long);

                    item.TimesViewed += 1;
                    return SharpRuleEngine.PerformResult.Stop;
                }).Name("Choose brief or long description rule.");

        }
    }   
}