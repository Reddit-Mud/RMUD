
public class disambig : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Palantine Villa - Hall of Ambiguity";
        Long = "This room might be round. It is roundish, at the very least. It is very hard to tell, what with how all of the walls are covered, floor to ceiling, in mirrors. Your only point of reference in the place is the doors on opposite walls. There are thousands of them, reflected here and there and everywhere.";

        AddScenery("Which do you mean?", "MIRROR", "MIRRORS");

        OpenLink(RMUD.Direction.WEST, "palantine\\library", GetObject("palantine\\disambig_blue_door@inside"));
        OpenLink(RMUD.Direction.EAST, "palantine\\dark_room", GetObject("palantine\\disambig_red_door@inside"));
        OpenLink(RMUD.Direction.SOUTH, "palantine\\antechamber");

        Move(GetObject("palantine\\disambig_key"), this);
        Move(GetObject("palantine\\library_key"), this);
        Move(new torch(), this);
        Move(GetObject("palantine/skull"), this);
	}
}

public class torch : RMUD.MudObject
{
    public torch()
    {
        Short = "torch";
        Nouns.Add("torch");

        Value<RMUD.MudObject, RMUD.LightingLevel>("light level").Do(a => RMUD.LightingLevel.Bright);
    }
}
