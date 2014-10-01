public class antechamber : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Interior;
        Short = "Palantine Villa - Antechamber";
        Long = "Two imposing statues stand guard in this small room, on either side of the door to the room beyond. On the left, Jupiter, king of the gods. On the right, Minerva, the goddess of wisdom and beauty.";

        AddScenery(new Jupiter());
        AddScenery("Minerva is turned to regard her father Jupiter, and poses with one hand on her hips and the other on the shaft of a massive hammer.", "minerva");

        var table = new Table();
        RMUD.Thing.Move(table, this);

        RMUD.Thing.Move(new RMUD.Thing("old vase", "An old, cracked vase."), table);


        OpenLink(RMUD.Direction.NORTH, "palantine\\disambig");
	}
}

public class Jupiter : RMUD.Scenery, RMUD.EmitsLight
{
    public Jupiter()
    {
        Nouns.Add("jupiter");
        Long = "Jupiter holds in his left hand a gleaming thunderbolt. It glows bright enough to light the entire chamber. In his right, he holds a chisel.";
    }

    public bool EmitsLight
    {
        get { return true; }
    }
}

public class Table : RMUD.GenericContainer
{
    public Table() : base(RMUD.RelativeLocations.On, RMUD.RelativeLocations.On)
    {
        Short = "ancient table";
        Long = "As the years have worn long the wood of this table has dried and shrunk, and split, and what was once a finely crafted table is now pitted and gouged. The top is still mostly smooth, from use but not from care.";
        Nouns.Add("ancient", "table");
    }

    public override string Indefinite
    {
        get
        {
            return "an ancient table";
        }
    }
}
