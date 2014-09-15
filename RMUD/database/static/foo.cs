public class foo : RMUD.Room
{
	public foo()
	{
		Short = "foo";
		OpenLink(RMUD.Direction.NORTH, "dummy");
	}
}
