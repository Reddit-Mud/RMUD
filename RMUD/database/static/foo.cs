public class foo : RMUD.Room
{
	public override void Initialize()
	{
		Short = "foo";
		OpenLink(RMUD.Direction.NORTH, "dummy");
	}
}
