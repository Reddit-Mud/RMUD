public class settings : RMUD.Settings
{
	public override void Initialize()
	{
		Banner = "~~== REDDIT MUD ==~~\r\n";
		MessageOfTheDay = "register username - Create a new account.\r\nlogin username - Log into an existing account.\r\n";
        NewPlayerStartRoom = "palantine/antechamber";
	}
}
