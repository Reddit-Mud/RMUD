public class lighthouse_door : RMUD.LockedDoor
{
	public override void Initialize()
	{
		Short = "round steel door";
		Long = "There is a round steel door in the side of the lighthouse.";
		Nouns.Add("STEEL", "DOOR", "ROUND");
        IsMatchingKey = k => k.Is("blecki-demo/lighthouse_key");

		Open = false;
		Locked = true;
	}
}
