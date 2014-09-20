public class lighthouse_balcony : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Lighthouse Balcony";

                Long = "The mechanism that gives this building its name has fallen in disuse long ago. There is rot, rust and it shows signs of internal breakage. The view from here is spectacular. The sea, the clearing in the forest, the town, and the harbor all are encompassed. It is breathtaking.";

                AddScenery("The sea from here is a blue infinity, the inviting goddess that lured so many men away from their homes. It looks calm, peaceful, gentle. Even knowing geography as we do, it still draws out the question from the deepest recesses of our being: 'What is out there?'", "sea");

                AddScenery("From such a remote standpoint, any human dwelling is small. The town, which already feels small when you're inside, now looks insignificant. Why does it still exist?", "town");

                AddScenery("A few boats indicate where the harbor is. It would be negligible otherwise, just a human effort reaching out into the sea.", "harbor");

                AddScenery("The clearing with the altar looks just a bit strange from here. It looks like it is a perfect circle, like the trees just happen to grow exactly where they need to for that shape.", "clearing");

                OpenLink(RMUD.Direction.EAST, "trevoke-demo/lighthouse_stairway");
        }
}
