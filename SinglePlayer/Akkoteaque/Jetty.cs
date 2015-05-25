using RMUD;

namespace Akkoteaque
{

    public class Jetty : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Stone Jetty";
			Perform<MudObject, MudObject>("describe")
				.When((actor, item) => item == this )
				.Do((actor, item) => {
					SendMessage(actor, "A narrow stone jetty juts into the sea, the waves slapping against it's sides and occasionally flowing up and over the stone. Smalls pools form where the water has worn away at the rock, and the cracks between the stones are full of creeping green life.");
                    return SharpRuleEngine.PerformResult.Continue;
				});

        }
    }   
}