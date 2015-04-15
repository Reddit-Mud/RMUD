using System;
using RMUD;

namespace RMUD
{
    public static class WhenHelper
    {
        public static RuleBuilder<MudObject, MudObject, PerformResult> DescribeThis(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject>("describe").When(Object.WhenThis());
        }

        public static Func<MudObject, MudObject, bool> WhenThis(this MudObject Object)
        {
            return (viewer, thing) => System.Object.ReferenceEquals(Object, thing);
        }

        public static Func<MudObject, MudObject, bool> WhenFirstTime(this MudObject Object)
        {
            var variableID = Guid.NewGuid();
            return (viewer, thing) =>
            {
                var r = !thing.GetBooleanProperty(variableID.ToString());
                thing.SetProperty(variableID.ToString(), true);
                return r;
            };
        }

        public static Func<MudObject, MudObject, PerformResult> DoMessage(this MudObject Object, String Str)
        {
            return (viewer, thing) =>
            {
                MudObject.SendMessage(viewer, Str);
                return PerformResult.Stop;
            };
        }
    }
}
