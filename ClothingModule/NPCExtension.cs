using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ClothingModule
{
	public static class NPCExtension
	{
        public static void Wear(this NPC NPC, MudObject Item)
        {
            MudObject.Move(Item, NPC, RelativeLocations.Worn);
        }

        public static void Wear(this NPC NPC, String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            Wear(NPC, Clothing.Create(Short, Layer, BodyPart));
        }
	}
}
