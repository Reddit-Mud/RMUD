public class library : Room
{
	public override void Initialize()
	{
		Short = "Palantine Villa - The Ancient Library of Kuz";
        MudObject.Move(new kuz_shelf(), this);

        OpenLink(RMUD.Direction.EAST, "palantine/disambig", RMUD.Mud.GetObject("palantine/disambig_blue_door"));
	}
}

public class kuz_shelf : MudObject, TakeRules, LocaleDescriptionRules
{
    public DescriptiveText LocaleDescription { get; set; }

    public kuz_shelf()
    {
        Short = "dusty shelf full of books";
        Long = "There are so many books, and they all look so interesting and inviting. You could just go right ahead and take one.";
        LocaleDescription = "A massive book shelf looms in the center of the room.";
        Nouns.Add("BOOK", "BOOKS", "SHELF", "DUSTY");
    }

    CheckRule TakeRules.Check(Actor Actor)
    {
        return CheckRule.Allow();
    }

    RuleHandlerFollowUp TakeRules.Handle(Actor Actor)
    {
        var newBook = new kuz_book();
        MudObject.Move(newBook, Actor);

        Mud.SendMessage(Actor, String.Format("You take {0}.\r\n", newBook.Indefinite));
	    Mud.SendExternalMessage(Actor, String.Format("{0} takes {1}.\r\n", Actor.Short, newBook.Indefinite));

        //Tell the take command not to emit messages or move this object to the player's inventory.
        return RuleHandlerFollowUp.Stop;
    }

}

public class kuz_book : MudObject
{
    public static List<String> TitlesA = new List<String>(new String[]{
        "Chronicles of",
        "A Compendium of",
        "Untranslated Analysis of",
        "Adventures in",
        "Nonsense, Nonesuch, and",
        "Impersonal Theories Regarding",
        "How to Cook",
        "A Brief History of",
        "The Complete History of"
                                         });

    public static List<String> TitlesB = new List<String>(new String[]{
        "History",
        "Forgotten Kings",
        "Antquity",
        "Bear Livers",
        "Half-Burnt Candles",
        "High Places",
        "Sir Richard the Sexy",
        "the Utterings of Slow Children"
                                        });

    public static List<String> Volumes = new List<String>(new String[]{
        "",
        "volume I",
        "volume II",
        "volume III",
        "volume IV",
        "volume LXXIX",
        "second edition",
        "third edition",
        "abridged version"
    });

    public static List<String> Covers = new List<String>(new String[]{
        "plain",
        "leather bound",
        "ragged",
        "embossed"
    });

    public static Random Random = new Random();

    public static String Latin = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat...\r\n (It's written in Latin.)";

    public kuz_book()
    {
        var title = TitlesA[Random.Next(0, TitlesA.Count)] + " " + TitlesB[Random.Next(0, TitlesB.Count)];
        var volume = Volumes[Random.Next(0, Volumes.Count)];
        var cover = Covers[Random.Next(0, Covers.Count)];
        Short = cover + " copy of " + title;
        if (!String.IsNullOrEmpty(volume)) Short += ", " + volume;
        Nouns.Add("BOOK");
        Long = Latin;

        if (cover == "embossed")
            Article = "an";
    }
}
