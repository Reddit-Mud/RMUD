public class settings : RMUD.Settings
{
	public override void Initialize()
	{
		Banner = "RMUD\r\n";
		MessageOfTheDay = "Player accounts are not yet implemented. For now, just use 'login [name]'.\r\n";
        	NewPlayerStartRoom = "tutorial/school/lobby";
        	UpfrontCompiliation = "true";
	}
}
