public class start : MudObject
{
	public override void Initialize()
	{
        Room(RoomType.Interior);

        SetProperty("short", "Chamber of the swirling elements");
        SetProperty("ambient light", LightingLevel.Bright);
        OpenLink(Direction.NORTH, "area/beginning/secondroom");

        MudObject.Move(new target(), this);
    }
}

public class target : Monster
{
    public target()
    { 
        Actor();
        SimpleName("mob");
        SetProperty("current-hp", 10);

        base.Initialize();
    }
}

