public class settings : RMUD.Settings
{
	public override void Initialize()
	{
		Banner = "~~== REDDIT MUD ==~~\r\n";
		MessageOfTheDay = "Prototype by Blecki\r\nFor now, just type 'login [name]'. You'll enter the world and be given wizard powers. Neat!\r\n";
        NewPlayerStartRoom = "testing/disambig";
	}
}
