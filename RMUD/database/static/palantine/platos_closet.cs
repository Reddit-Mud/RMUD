public class platos_closet : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Interior;
        Short = "Palantine Villa - Plato's Closet";

        AddScenery(new lamp());

        Move(Clothing.Create("pair of jeans", ClothingLayer.Outer, ClothingBodyPart.Legs), this);
        Move(Clothing.Create("polo shirt", ClothingLayer.Outer, ClothingBodyPart.Torso), this);
        Move(Clothing.Create("pair of briefs", ClothingLayer.Under, ClothingBodyPart.Legs), this);

        OpenLink(RMUD.Direction.WEST, "palantine\\solar");
	}
}

public class lamp : RMUD.Scenery
{
    public lamp()
    {
        Nouns.Add("gas", "lamp");
        Long = "This little gas lamp somehow manages to fill the endless closet with light.";

        Value<RMUD.MudObject, RMUD.LightingLevel>("emits-light").Do(a => RMUD.LightingLevel.Bright);
    }

}