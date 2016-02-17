using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public partial class MudObject
	{
        public void Room(RoomType Type)
        {
            Container(RelativeLocations.Contents, RelativeLocations.Contents);
            UpsertProperty<RoomType>("room type", Type);
            UpsertProperty<LightingLevel>("light", LightingLevel.Dark);
            UpsertProperty<LightingLevel>("ambient light", LightingLevel.Dark);
        }

        public void OpenLink(Direction Direction, String Destination, MudObject Portal = null)
        {
            if (RemoveAll(thing => thing.GetPropertyOrDefault<Direction>("link direction", RMUD.Direction.NOWHERE) == Direction && thing.GetPropertyOrDefault<bool>("portal?", false)) > 0)
                Core.LogWarning("Opened duplicate link in " + Path);

            if (Portal == null)
            {
                Portal = new MudObject();
                Portal.SetProperty("link anonymous?", true);
                Portal.SetProperty("Short", "link " + Direction + " to " + Destination);
            }

            Portal.SetProperty("portal?", true);
            Portal.SetProperty("link direction", Direction);
            Portal.SetProperty("link destination", Destination);
            Portal.Location = this;
            Add(Portal, RelativeLocations.Contents);
        }

        #region Scenery 

        public MudObject AddScenery(String Description, params String[] Nouns)
		{
			var scenery = new MudObject();
            scenery.SetProperty("scenery?", true);
            scenery.SetProperty("Long", Description);
			foreach (var noun in Nouns)
				scenery.GetProperty<NounList>("Nouns").Add(noun.ToUpper());
            AddScenery(scenery);
            return scenery;
		}

        public void AddScenery(MudObject Scenery)
        {
            Scenery.SetProperty("scenery?", true);
            Add(Scenery, RelativeLocations.Contents);
            Scenery.Location = this;
        }

        #endregion
        
    }
}
