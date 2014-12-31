using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RoomType
    {
        Exterior,
        Interior
    }

    public class RoomLightingRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("emits-light", "[item] -> LightingLevel, How much light does the item emit?");
            GlobalRules.Value<MudObject, LightingLevel>("emits-light").Do(item => LightingLevel.Dark);
        }
    }

	public class Room : Container
	{
        public RoomType RoomType = RoomType.Exterior;

        public Room() : base(RelativeLocations.Contents | RelativeLocations.Links | RelativeLocations.Scenery, RelativeLocations.Contents)
        { }

		public void OpenLink(Direction Direction, String Destination, MudObject Portal = null)
		{
            if (Portal != null && !(Portal is Portal)) 
                Core.LogWarning("Object passed to OpenLink in " + Path + " is not a portal.");
            if (RemoveAll(thing => thing is Link && (thing as Link).Direction == Direction) > 0)
                Core.LogWarning("Opened duplicate link in " + Path);

            var link = new Link { Direction = Direction, Destination = Destination, Portal = Portal as Portal };
            if (Portal is Portal) (Portal as Portal).AddSide(this);
            link.Location = this;
            Add(link, RelativeLocations.Links);
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

        public void AddScenery(Scenery Scenery)
        {
            Add(Scenery, RelativeLocations.Scenery);
            Scenery.Location = this;
        }

        #endregion

        public LightingLevel AmbientLighting { get; private set; }

        private void UpdateLighting()
        {           
            AmbientLighting = LightingLevel.Dark;

            if (RoomType == RMUD.RoomType.Exterior)
            {
                AmbientLighting = MudObject.AmbientExteriorLightingLevel;
            }
            else
            {
                foreach (var item in MudObject.EnumerateVisibleTree(this))
                {
                    var lightingLevel = GlobalRules.ConsiderValueRule<LightingLevel>("emits-light", item);
                    if (lightingLevel > AmbientLighting) AmbientLighting = lightingLevel;
                }
            }
        }

        public override void HandleMarkedUpdate()
        {
            UpdateLighting();
        }
    }
}
