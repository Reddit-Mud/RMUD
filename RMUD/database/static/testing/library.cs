public class library : RMUD.Room
{
	public override void Initialize()
	{
		Short = "The ancient library of Kuz";
        RMUD.Thing.Move(new kuz_shelf(), this);
	}
}

public class kuz_shelf : RMUD.Thing, RMUD.ITakeRules
{
    public kuz_shelf()
    {
        Short = "a dusty shelf full of books";
        Long = "There are so many books, and they all look so interesting and inviting. You could just go right ahead and take one.";
        Nouns.Add("BOOK", "BOOKS", "SHELF", "DUSTY");
    }

    bool RMUD.ITakeRules.CanTake(RMUD.Actor Actor)
    {
        return true;
    }

    RMUD.RuleHandlerFollowUp RMUD.ITakeRules.HandleTake(RMUD.Actor Actor)
    {
        var newBook = new kuz_book();
        RMUD.Thing.Move(newBook, Actor);

        RMUD.Mud.SendEventMessage(Actor, RMUD.EventMessageScope.Single, "You take a book.\r\n");
	    RMUD.Mud.SendEventMessage(Actor, RMUD.EventMessageScope.External, Actor.Short + " takes a book.\r\n");

        return RMUD.RuleHandlerFollowUp.Stop;
    }
}

public class kuz_book : RMUD.Thing
{
    public kuz_book()
    {
        Short = "a generated book";
        Nouns.Add("BOOK");
    }
}
