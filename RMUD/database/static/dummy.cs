using System.Text;

public class dummy : RMUD.Room
{
	private int TimesViewed = 0;

	public override void Initialize()
	{
		Short = "dummy";
		Long = new RMUD.DescriptiveText((v, o) =>
		{
			TimesViewed += 1;

			var builder = new StringBuilder();
			builder.Append("You've looked at this room ");
			builder.Append(TimesViewed);
			builder.Append(" times.\r\n");

			builder.Append("This room is a hub between multiple demonstration areas. Travel in any direction to reach a demo area.");

			return builder.ToString();
		});
			
		OpenLink(RMUD.Direction.SOUTH, "blecki-demo/area");

	}
}
