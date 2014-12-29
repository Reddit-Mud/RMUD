public class fedora : RMUD.Clothing
{
    public override void Initialize()
    {
        SimpleName("fedora", "hat");
        Long = "This hat is so not cool.";

        Layer = RMUD.ClothingLayer.Outer;
        BodyPart = RMUD.ClothingBodyPart.Head;
    }
}