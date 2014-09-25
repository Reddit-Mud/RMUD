public class library : Room
{
	public override void Initialize()
	{
		Short = "The ancient library of Kuz";
        Thing.Move(new kuz_shelf(), this);
	}
}

public class kuz_shelf : Thing, ITakeRules, ILocaleDescriptionRules
{
    public DescriptiveText LocaleDescription { get; set; }

    public kuz_shelf()
    {
        Short = "dusty shelf full of books";
        Long = "There are so many books, and they all look so interesting and inviting. You could just go right ahead and take one.";
        LocaleDescription = "A massive book shelf looms in the center of the room.";
        Nouns.Add("BOOK", "BOOKS", "SHELF", "DUSTY");
    }

    bool ITakeRules.CanTake(Actor Actor)
    {
        return true;
    }

    RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor)
    {
        var newBook = new kuz_book();
        Thing.Move(newBook, Actor);

        Mud.SendEventMessage(Actor, EventMessageScope.Single, "You take a book.\r\n");
	    Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " takes a book.\r\n");

        //Tell the take command not to emit messages or move this object to the player's inventory.
        return RuleHandlerFollowUp.Stop;
    }

}

public class kuz_book : Thing
{
    public kuz_book()
    {
        Short = "generated book";
        Nouns.Add("BOOK");
    }
}
