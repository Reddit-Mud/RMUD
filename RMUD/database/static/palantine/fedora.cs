public class fedora : Clothing
{
    public override void Initialize()
    {
        SimpleName("fedora", "hat");
        Long = "This hat is so not cool.";

        Layer = ClothingLayer.Outer;
        BodyPart = ClothingBodyPart.Head;
    }
}