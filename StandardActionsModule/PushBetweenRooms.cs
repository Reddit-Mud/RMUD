// So this is an interesting method I've wanted to try for a while: A tutorial that is, itself,
// source. I want to give an example of how to write a new command. It's a complex topic, but
// there are a ton of examples already in the project.

// In this case, we'll create a command to push things between rooms. This is a fairly obscure
// action that doesn't have many uses, but crops up now and then and manages to still be built
// into Inform7. 

// Comments are supposed to commentary. If they were supposed to be documentation, they would be
// called documents. So that's what we'll have here: A lot of commentary.

// First, boiler plate nonsense.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace StandardActionsModule
{
    // The engine will automatically discover every CommandFactory when it loads the module, and call
    // Create on them.
    internal class PushBetweenRooms : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            // The engine has passed us the default parser. We want to add the command to it.
            // The command should follow the form 'push [object] [direction]'. CommandFactory defines
            // a bunch of functions to generate matchers. Calling 'KeyWord(...)' is equivilent to 
            // 'new RMUD.Matchers.KeyWord(...)'. It's minor, but makes things far more readable.
            // The actual parser operates on a list of words, with each piece matching or failing.
            // Each matcher returns a set of possible matches.
            Parser.AddCommand(
                Sequence(      // Attemps to match a set of matchers in sequence.
                    KeyWord("PUSH"), // Is the word 'push'? Yes -> match. No -> Fail.
                    BestScore("SUBJECT", // Match the child, and choose the subset of possible matches that are best.
                        MustMatch("@not here", // If the child doesn't match, go ahead and report an error.
                            Object("SUBJECT", InScope))), // Match a mud object. "SUBJECT" is the argument we store it in, InScope is the source of the objects.
                    MustMatch("@unmatched cardinal", Cardinal("DIRECTION")))) // Finally, match a cardinal direction.
                // With the matcher itself built, the call to AddCommand is over. AddCommand return a 
                // command building object with a fluent interface.
                
                // Lets attach some help documentation to the command.
                .Manual("Push objects from room to room.")

                // Every command has a rulebook called its 'procedural rules'. When the command is matched,
                // these rules are invoked. The rules can access things matched above (such as the object
                // matched by that object matcher, which will be stored as "SUBJECT". There are several
                // helpful functions to make common procedural rules easy to add. We start with the raw form,
                // since we need to translate the direction we matched to the actual link object. When the 
                // command is matched, these rules will be called in the order declared.
                .ProceduralRule((match, actor) =>
                {
                    // The direction matched was stored in the match as "DIRECTION".
                    var direction = match["DIRECTION"] as Direction?;
                    var location = actor.Location as Room;
                    // Rooms have a collection of objects that are in them. Links happen to have two specific 
                    // properties set that we can use to find them: First, 'portal?' will be true, and 
                    // 'link direction' will hold the direction the link goes in. So we search for the link.
                    var link = location.EnumerateObjects().FirstOrDefault(thing => thing.GetPropertyOrDefault<bool>("portal?", false) && thing.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE) == direction.Value);
                    // Store the link in the match, and later procedural rules will be able to find it.
                    match.Upsert("LINK", link);
                    // Procedural rules return PerformResults. If they return stop, the command stops right there.
                    return PerformResult.Continue;
                }, "lookup link rule") // We can also name procedural rules. This is always a good idea, as the
                // name will help with debugging the rules later.

                // Pushing between rooms is also going, so we want to make sure the player can go that direction.
                // The command builder type provides the 'Check' function which adds a procedural rule to invoke
                // a check rulebook (and stop the command if it returns CheckResult.Disallow). So we can go ahead
                // and check 'can go?', giving the go rules a chance to stop this action. 
                // Check takes the name of the rulebook, and then any arguments to pass to it. The arguments are
                // drawn from the match: So "ACTOR" would be the actor invoking the command, and "LINK" is that
                // link we found in the proceeding rule. 
                // The 'can go?' rulebook also handles the case where "LINK" is null, so we don't have to worry
                // about it here.
                .Check("can go?", "ACTOR", "LINK")

                // We still want some rules to govern the pushing action too, and in this case the rulebook will
                // take one more argument than the 'can go?' rulebook. Lets go ahead and invoke them too.
                .Check("can push direction?", "ACTOR", "SUBJECT", "LINK")

                // There's a rulebook every command that counts as the player 'acting' (meaning they are affecting
                // the world in some way) should check before actually doing anything. Since every command should
                // check it, it has its own function to add it.
                .BeforeActing()

                // We're done checking if the command is allowed at this point, so we can finally get around to
                // actually performing the action. The arguments to Perform behave the same as those to Check.
                // It just invokes a different kind of rulebook.
                .Perform("push direction", "ACTOR", "SUBJECT", "LINK")

                // The opposite of BeforeActing is AfterActing. Can you guess when it should be invoked?
                .AfterActing()

                // The last procedural rule lets the engine know it needs to update both sides of the link. 
                // Most commands can just call 'MarkLocale()' here to add a rule to mark the actor's locale
                // for update. Since the actor moved, that won't work in this case. We need to make sure we
                // update where the player WAS as well as where he IS!
                .ProceduralRule((match, actor) =>
                {
                    Core.MarkLocaleForUpdate(actor);
                    Core.MarkLocaleForUpdate(match["LINK"] as MudObject);
                    return PerformResult.Continue;
                }, "Mark both sides of link for update rule"); // Hey the command is finally done.
		}

        // Any function with this signature will be called when the module is loaded. This is our chance to
        // do anything we want, but usually all we'll do is define some rules. We used some rulebooks above,
        // now we have to actually define them.
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            // Lets start by defining some messages we'll use later. This commentary isn't about the message
            // formatting system, but lets still do this 'right'.
            Core.StandardMessage("can't push direction", "That isn't going to work.");
            Core.StandardMessage("you push", "You push <the0> <s1>.");
            Core.StandardMessage("they push", "^<the0> pushed <the1> <s2>.");
            Core.StandardMessage("they arrive pushing", "^<the0> arrives <s2> pushing <the1>.");

            // We'll declare the 'can push direction?' rulebook first. Notice that we were passed the global rules,
            // and that is what we want to declare it on. DeclareCheckRuleBook takes the types the rulebook
            // expects as generic arguments, then the name of the rulebook. The documentation is helpful but
            // not required. This rulebook takes three mud objects. Their usage is documented in the rulebook
            // description.
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can push direction?", "[Actor, Subject, Link] : Can the actor push the subject through that link?", "actor", "subject", "link");

            // Now we want to define a global rule in the 'can push direction?' rulebook. Rulebooks do not have
            // to be declared before rules are defined, but it's good style. The Check function returns a 
            // rulebuilder, which provides a fluent interface for declaring rules.
            GlobalRules.Check<MudObject, MudObject, MudObject>("can push direction?")
                // Do expects a lambda, and lets us specify what the rule should actually do.
                .Do((actor, subject, link) =>
                {
                    MudObject.SendMessage(actor, "@can't push direction"); // We defined this message above.
                    return CheckResult.Disallow; // Disallow the action.
                }).Name("Can't push between rooms by default rule.");
            // So by default, nothing can be pushed between rooms. What a useful command!

            // That's the only check rule we need. Now lets define the perform rules. We need to duplicate
            // large portions of the functionality of the go command. I'll comment in a little less detail
            // now - I think you've got the idea of how this works. These rules will be invoked by the command
            // in the order they are declared. We could smush them all into one rule, but that would be
            // poor style maybe.
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("push direction", "[Actor, Subject, Link] : Handle the actor pushing the subject through the link.", "actor", "subject", "link");

            // First we want to report the action to the player and any other players that happen to be around.
            GlobalRules.Perform<MudObject, MudObject, MudObject>("push direction")
                .Do((actor, subject, link) =>
                {
                    var direction = link.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE);
                    MudObject.SendMessage(actor, "@you push", subject, direction.ToString().ToLower());

                    // SendExternalMessage sends the message to everyone in the same place as the actor, 
                    // except the actor themself.
                    MudObject.SendExternalMessage(actor, "@they push", actor, subject, direction.ToString().ToLower());
                    return PerformResult.Continue;
                })
                .Name("Report pushing between rooms rule.");

            // Now we want to actually move the player, and the object they are pushing.
            GlobalRules.Perform<MudObject, MudObject, MudObject>("push direction")
                .Do((actor, subject, link) =>
                {
                    var destination = MudObject.GetObject(link.GetProperty<String>("link destination")) as Room;
                    if (destination == null)
                    {
                        MudObject.SendMessage(actor, "@bad link");
                        return PerformResult.Stop;
                    }

                    MudObject.Move(actor, destination);
                    MudObject.Move(subject, destination);
                    return PerformResult.Continue;
                })
                .Name("Push through the link rule.");

            // For most actions that's enough. But in this case, we want to let players in the room the
            // actor went to know they have arrived.
            GlobalRules.Perform<MudObject, MudObject, MudObject>("push direction")
                .Do((actor, subject, link) =>
                {
                    var direction = link.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE);
                    var arriveMessage = Link.FromMessage(Link.Opposite(direction));
                    MudObject.SendExternalMessage(actor, "@they arrive pushing", actor, subject, arriveMessage);
                    return PerformResult.Continue;
                })
                .Name("Report arrival while pushing rule.");

            // And finally, lets make sure the player gets a description of the room they have arrived in.
            GlobalRules.Perform<MudObject, MudObject, MudObject>("push direction")
                .When((actor, subject, link) => actor is Player && (actor as Player).ConnectedClient != null)
                .Do((actor, subject, link) =>
                {
                    // We set the 'auto' flag to let the look command know it's been generated, and not
                    // typed by the player. This is handy for deciding wether to show a brief description
                    // or the full description of a room.
                    Core.EnqueuActorCommand(actor as Actor, "look", HelperExtensions.MakeDictionary("AUTO", true));
                    return PerformResult.Continue;
                })
                .Name("Players look after pushing between rooms rule.");
        }
    }

    // The last thing we need to do is define some extension methods to help us write rules for this command.
    // Without these, anyone writing rules meant to work with this command needs to know the types it takes.
    // These just provide us with a little bit of type erasure to make usage a little less error prone.
    public static class PushDirectionExtensions
    {
        public static RuleBuilder<MudObject, MudObject, MudObject, CheckResult> CheckCanPushDirection(this MudObject Object)
        {
            return Object.Check<MudObject, MudObject, MudObject>("can push direction?").When((actor, subject, link) => System.Object.ReferenceEquals(subject, Object));
        }

        public static RuleBuilder<MudObject, MudObject, MudObject, PerformResult> PerformPushDirection(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject, MudObject>("push direction").When((actor, subject, link) => System.Object.ReferenceEquals(subject, Object));
        }
    }
}

// That's it. I hope you enjoyed this wild ride through defining a command for RMUD, and that it helped shed
// some light on how this beast actually works.
