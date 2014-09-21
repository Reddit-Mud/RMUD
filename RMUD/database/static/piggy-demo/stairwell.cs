public class stairwell : RMUD.Room
{
	public override void  Initialize()
{
 	Short = "Spiral Staircase";
		Long = "As you climb the mesh metal stairs, you notice another large window facing South to the sea, interrupting a string of elegantly framed photographs that depict the lighthouse's history. One features a small ribbon cutting ceremony, and the others are shots of visitors or the workers. Perhaps this once attracted tourists in the days before electronic navigation. A platform looms above you.";

		OpenLink(RMUD.Direction.DOWN, "piggy-demo/lobby");
		OpenLink(RMUD.Direction.UP, "piggy-demo/balcony");
	}
}
