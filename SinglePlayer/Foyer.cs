using RMUD;

namespace CloakOfDarkness
{

    public class Foyer : RMUD.MudObject
    {
        public override void Initialize()
        {
            /*
             * Foyer of the Opera House is a room.  "You are standing in a spacious hall,
splendidly decorated in red and gold, with glittering chandeliers overhead.
The entrance from the street is to the north, and there are doorways south and west."

Instead of going north in the Foyer, say "You've only just arrived, and besides,
the weather outside seems to be getting worse."

             */
            Room(RoomType.Interior);

            SetProperty("short", "Foyer of the Opera House");
            SetProperty("long", "You are standing in a spacious hall, splendidly decorated in red and gold, with glittering chandeliers overhead.");

            OpenLink(Direction.NORTH, "Outside");
            OpenLink(Direction.SOUTH, "Bar");
            OpenLink(Direction.WEST, "Cloakroom");

            Check<MudObject, MudObject>("can go?")
               .First
               .When((actor, link) => link != null && link.Location is Foyer && link.GetProperty<Direction>("link direction") == Direction.NORTH)
               .Do((actor, link) =>
               {
                   MudObject.SendMessage(actor, "You've only just arrived, and besides, the weather outside seems to be getting worse.");
                   return SharpRuleEngine.CheckResult.Disallow;
               });
        }
    }

   
}