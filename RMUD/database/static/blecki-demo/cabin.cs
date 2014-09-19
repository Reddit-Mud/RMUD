public class cabin : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Boat Cabin";

		OpenLink(RMUD.Direction.EAST, "blecki-demo/deck");

		RMUD.Thing.Move(RMUD.Mud.GetObject("blecki-demo/lighthouse_key") as RMUD.Thing, this);
	}
}
