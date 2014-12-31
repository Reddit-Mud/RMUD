public class library : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Palantine Villa - The Ancient Library of Kuz";
        Move(new kuz_shelf(), this);

        OpenLink(RMUD.Direction.EAST, "palantine/disambig", GetObject("palantine/disambig_blue_door"));
	}
}

public class kuz_shelf : RMUD.MudObject
{
    public kuz_shelf()
    {
        Short = "dusty shelf full of books";
        Long = "There are so many books, and they all look so interesting and inviting. You could just go right ahead and take one.";
        Nouns.Add("BOOK", "BOOKS", "SHELF", "DUSTY");

        Perform<RMUD.MudObject, RMUD.MudObject>("describe in locale").Do((actor, item) =>
            {
                SendMessage(actor, "A massive book shelf looms in the center of the room.");
                return RMUD.PerformResult.Continue;
            });

        Check<RMUD.MudObject, RMUD.MudObject>("can take?").Do((a, b) => RMUD.CheckResult.Allow);

        Perform<RMUD.MudObject, RMUD.MudObject>("taken").Do((actor, target) =>
            {
                var newBook = new kuz_book();
                Move(newBook, actor);

                SendMessage(actor, "You take <a0>.", newBook);
                SendExternalMessage(actor, "<a0> takes <a1>.", actor, newBook);
                return RMUD.PerformResult.Stop;
            });
    }
}

public class kuz_book : RMUD.MudObject
{
    public static System.Collections.Generic.List<System.String> TitlesA = new System.Collections.Generic.List<System.String>(new System.String[]{
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

    public static System.Collections.Generic.List<System.String> TitlesB = new System.Collections.Generic.List<System.String>(new System.String[]{
        "History",
        "Forgotten Kings",
        "Antquity",
        "Bear Livers",
        "Half-Burnt Candles",
        "High Places",
        "Sir Richard the Sexy",
        "the Utterings of Slow Children"
                                        });

    public static System.Collections.Generic.List<System.String> Volumes = new System.Collections.Generic.List<System.String>(new System.String[]{
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

    public static System.Collections.Generic.List<System.String> Covers = new System.Collections.Generic.List<System.String>(new System.String[]{
        "plain",
        "leather bound",
        "ragged",
        "embossed"
    });

    public static System.String Latin = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat...\r\n (It's written in Latin.)";

    public kuz_book()
    {
        var title = TitlesA[Random.Next(0, TitlesA.Count)] + " " + TitlesB[Random.Next(0, TitlesB.Count)];
        var volume = Volumes[Random.Next(0, Volumes.Count)];
        var cover = Covers[Random.Next(0, Covers.Count)];
        Short = cover + " copy of " + title;
        if (!System.String.IsNullOrEmpty(volume)) Short += ", " + volume;
        Nouns.Add("BOOK");
        Long = Latin;

        if (cover == "embossed")
            Article = "an";
    }
}
