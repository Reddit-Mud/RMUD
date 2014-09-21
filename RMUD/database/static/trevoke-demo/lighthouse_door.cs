public class lighthouse_door : RMUD.BasicDoor
{
        public override void Initialize()
        {
                Short = "round wooden door";
                Long = "This is an old, rotting wooden door in the side of the lighthouse.";
                Nouns.Add("WOODEN", "DOOR", "ROUND");

                Open = true;
        }
}
