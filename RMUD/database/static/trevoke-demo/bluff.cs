public class bluff : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Rocky Bluff";
                Long = "Rocks have been pushed to the side, revealing the dirt below and creating the path that led you here. The path ends a few feet short of the divide between earth and air, almost as though discouraging the idea of taking a running start before jumping. The sea can be heard before it can be seen. Continuing their tireless battle against the rock, the waves gently crash against the cliff face. The end of the lighthouse's shadow can be seen here.";

                // AddScenery("description", "word1", "word2");
                OpenLink(RMUD.Direction.SOUTH, "trevoke-demo/shoreline");
        }
}
