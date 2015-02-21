public class fedora : MudObject
{
    public override void Initialize()
    {
        SimpleName("fedora", "hat");
        Long = "This hat is so not cool.";

        SetProperty("clothing layer", ClothingLayer.Outer);
        SetProperty("clothing part", ClothingBodyPart.Head);
        SetProperty("wearable?", true);
    }
}