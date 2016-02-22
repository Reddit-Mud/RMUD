using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class RegisterRoomProperties
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("room type", typeof(RoomType), RoomType.NotARoom);
            PropertyManifest.RegisterProperty("light", typeof(LightingLevel), LightingLevel.Dark);
            PropertyManifest.RegisterProperty("ambient light", typeof(LightingLevel), LightingLevel.Dark);
        }
    }

    public partial class MudObject
	{
        public void Room(RoomType Type)
        {
            Container(RelativeLocations.Contents, RelativeLocations.Contents);
            SetProperty("room type", Type);
        }

        public void OpenLink(Direction Direction, String Destination, MudObject Portal = null)
        {
            if (RemoveAll(thing => thing.GetPropertyOrDefault<Direction>("link direction") == Direction && thing.GetPropertyOrDefault<bool>("portal?")) > 0)
                Core.LogWarning("Opened duplicate link in " + Path);

            if (Portal == null)
            {
                Portal = new MudObject();
                Portal.SetProperty("link anonymous?", true);
                Portal.SetProperty("short", "link " + Direction + " to " + Destination);
            }

            Portal.SetProperty("portal?", true);
            Portal.SetProperty("link direction", Direction);
            Portal.SetProperty("link destination", Destination);
            Portal.Location = this;
            Add(Portal, RelativeLocations.Contents);
        }
    }
}
