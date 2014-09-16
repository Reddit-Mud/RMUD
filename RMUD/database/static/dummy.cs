public class dummy : RMUD.Room
{
	public override void Initialize()
	{
		Short = "dummy";
		Long = "This is a dummy room. It exists for testing purposes only.";
		OpenLink(RMUD.Direction.SOUTH, "foo");

		var thing = new RMUD.Thing();
		thing.Short = "a dummy thing";
		thing.Long = "This thing exists just to test commands.";
		thing.Adjectives.Add("dummy");
		thing.Nouns.Add("thing");

		Contents.Add(thing);
	}
}
