using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public partial class NPC : Actor
	{
        public void Wear(MudObject Item)
        {
            MudObject.Move(Item, this, RelativeLocations.Worn);
        }

        public void Wear(String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            Wear(Clothing.Create(Short, Layer, BodyPart));
        }
	}
}
