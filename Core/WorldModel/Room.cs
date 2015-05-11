using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Room : Container
	{
        public RoomType RoomType = RoomType.Exterior;

        public Room() : base(RelativeLocations.Contents | RelativeLocations.Links | RelativeLocations.Scenery, RelativeLocations.Contents)
        { }

        public void OpenLink(Direction Direction, String Destination, MudObject Portal = null)
        {
            if (RemoveAll(thing => thing.GetPropertyOrDefault<Direction>("link direction", RMUD.Direction.NOWHERE) == Direction && thing.GetPropertyOrDefault<bool>("portal?", false)) > 0)
                Core.LogWarning("Opened duplicate link in " + Path);

            if (Portal == null)
            {
                Portal = new MudObject();
                Portal.SetProperty("link anonymous?", true);
            }

            Portal.SetProperty("portal?", true);
            Portal.SetProperty("link direction", Direction);
            Portal.SetProperty("link destination", Destination);
            Portal.Location = this;
            Add(Portal, RelativeLocations.Links);
        }

        #region Scenery 

        public Scenery AddScenery(String Description, params String[] Nouns)
		{
			var scenery = new Scenery();
			scenery.Long = Description;
			foreach (var noun in Nouns)
				scenery.Nouns.Add(noun.ToUpper());
            AddScenery(scenery);
            return scenery;
		}

        public void AddScenery(MudObject Scenery)
        {
            Add(Scenery, RelativeLocations.Scenery);
            Scenery.Location = this;
        }

        #endregion

        public LightingLevel Light { get; private set; }
        public LightingLevel AmbientLighting = LightingLevel.Dark;

        public void UpdateLighting()
        {
            Light = LightingLevel.Dark;

            if (RoomType == RMUD.RoomType.Exterior)
            {
                Light = AmbientExteriorLightingLevel;
            }

            foreach (var item in MudObject.EnumerateVisibleTree(this))
            {
                var lightingLevel = GlobalRules.ConsiderValueRule<LightingLevel>("light level", item);
                if (lightingLevel > Light) Light = lightingLevel;
            }

            if (AmbientLighting > Light) Light = AmbientLighting;
        }
    }
}
