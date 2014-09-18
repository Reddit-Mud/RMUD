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
			builder.Append(" times.");

			return builder.ToString();
		});
			
		OpenLink(RMUD.Direction.SOUTH, "foo");

		var thing = new RMUD.Thing();
		thing.Short = "dummy thing";
		thing.Long = "This thing exists just to test commands.";
		thing.Nouns.Add("dummy", "thing");

		Add(thing);
	}
}
