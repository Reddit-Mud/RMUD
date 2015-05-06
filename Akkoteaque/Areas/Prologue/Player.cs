using RMUD;
using StandardActionsModule;

namespace Akko.Areas.Prologue
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            Short = "you";

            this.PerformDescribe().FirstTimeOnly.DoSimpleDescription("Well, you're kind of introverted, if we define 'kind of' as totally uninterested in interacting with other people ever. You're not terribly bright, though everyone keeps telling you how smart you are. They are idiots so you don't believe them. You're very worried about a large number of things all the time and.. oh. Oh. You meant, what do you look like? That's kind of boring, isn't it? Hmm. You're a girl.");

            this.PerformDescribe().Do((viewer, player) =>
                {
                    SendMessage(viewer, ChooseAtRandom("You're kind of frumpy, aren't you?", "You suspect that you are perfectly, entirely, and very dissapointingly, average.", "You aren't like other girls. This is probably not a bad thing."));
                    return PerformResult.Stop;
                });

            Move(GetObject("Areas.Prologue.Watch"), this, RelativeLocations.Held);
        }
    }
}
