public class disambig : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Test disambiguation";

        OpenLink(RMUD.Direction.WEST, "testing/disambig", new demo_door("red"));
        OpenLink(RMUD.Direction.EAST, "testing/disambig", new demo_door("blue"));

        RMUD.Thing.Move(new demo_key(), this);
	}
}

public class demo_key : RMUD.Thing
{
    public demo_key()
    {
        Short = "key";
        Nouns.Add("KEY");
    }
}

public class demo_door : RMUD.LockedDoor
{
    public demo_door(string Adjective)
    {
        this.Nouns.Add(Adjective);
        this.Open = false;
        this.Locked = true;
        this.IsMatchingKey = (k) => { return k.GetType() == typeof(demo_key); };

        this.Short = Adjective + " door";
    }

    public override void Initialize()
    {


    }
}
