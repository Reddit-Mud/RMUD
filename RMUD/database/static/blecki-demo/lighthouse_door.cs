public class lighthouse_door : RMUD.Door
{
	public override void Initialize()
	{
		Short = "round steel door";
		Long = "There is a round steel door in the side of the lighthouse.";
		Nouns.Add("STEEL", "DOOR", "ROUND");

		Open = true;
	}
}
