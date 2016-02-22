using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClothingModule
{
    public enum ClothingLayer
    {
        Under = 0,
        Outer = 1,
        Assecories = 2,
        Over = 3
    }

    public enum ClothingBodyPart
    {
        Feet,
        Legs,
        Torso,
        Hands,
        Neck,
        Head,
        Wrist,
        Fingers,
        Ears,
        Face,
        Cloak,
    }

    public static class Factory// : RMUD.MudObject
    {
        public static RMUD.MudObject Create(String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            var r = new RMUD.MudObject(Short, "This is a generic " + Short + ". Layer: " + Layer + " BodyPart: " + BodyPart);
            r.SetProperty("clothing layer", Layer);
            r.SetProperty("clothing part", BodyPart);
            r.SetProperty("wearable?", true);
            return r;
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            RMUD.PropertyManifest.RegisterProperty("clothing layer", typeof(ClothingLayer), ClothingLayer.Outer);
            RMUD.PropertyManifest.RegisterProperty("clothing part", typeof(ClothingBodyPart), ClothingBodyPart.Cloak);
            RMUD.PropertyManifest.RegisterProperty("wearable?", typeof(bool), false);
        }

        public static void Clothing(this RMUD.MudObject MudObject, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            MudObject.SetProperty("clothing layer", Layer);
            MudObject.SetProperty("clothing part", BodyPart);
            MudObject.SetProperty("wearable?", true);
        }
    }
}
