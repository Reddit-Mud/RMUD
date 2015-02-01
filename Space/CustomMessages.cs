using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space
{
    internal static class CustomMessages
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            RMUD.Core.OverrideMessage("not here", "I don't think there's anything like that here.");
            RMUD.Core.OverrideMessage("gone", "That isn't here anymore.");
            RMUD.Core.OverrideMessage("dont have that", "I don't have that.");
            RMUD.Core.OverrideMessage("already have that", "I already have that.");
            RMUD.Core.OverrideMessage("does nothing", "I don't think that did anything.");
            RMUD.Core.OverrideMessage("nothing happens", "Doesn't look like anything happened.");
            RMUD.Core.OverrideMessage("unappreciated", "I'd really rather not.");
            RMUD.Core.OverrideMessage("already open", "It's open already.");
            RMUD.Core.OverrideMessage("already closed", "It's closed already.");
            RMUD.Core.OverrideMessage("close it first", "I think I should probably close it first.");
            RMUD.Core.OverrideMessage("wrong key", "I can't make this key work.");
            RMUD.Core.OverrideMessage("error locked", "I think it's locked.");
            RMUD.Core.OverrideMessage("you went", "Okay, I'm going <s0>.");
            RMUD.Core.OverrideMessage("you close", "It's closed now.");
            RMUD.Core.OverrideMessage("they close", "^<the0> closes <the1>.");
            RMUD.Core.OverrideMessage("you drop", "I put <the0> down.");
            RMUD.Core.OverrideMessage("they drop", "^<the0> drops <a1>.");
            RMUD.Core.OverrideMessage("is open", "^<the0> is open.");
            RMUD.Core.OverrideMessage("is closed", "^<the0> is closed.");
            RMUD.Core.OverrideMessage("describe on", "There's <l1> on <the0>.");
            RMUD.Core.OverrideMessage("describe in", "Inside <the0> there is <l1>.");
            RMUD.Core.OverrideMessage("unmatched cardinal", "I don't know what way that is.");
            RMUD.Core.OverrideMessage("go to null link", "I don't see how I can go that way.");
            RMUD.Core.OverrideMessage("go to closed door", "Uh, the door is closed.");
            RMUD.Core.OverrideMessage("they went", "^<the0> went <s1>.");
            RMUD.Core.OverrideMessage("bad link", "Something is wrong with that direction. I don't know what.");
            RMUD.Core.OverrideMessage("they arrive", "^<the0> arrives <s1>.");
            RMUD.Core.OverrideMessage("nude", "You are naked.");
            RMUD.Core.OverrideMessage("wearing", "You are wearing..");
            RMUD.Core.OverrideMessage("empty handed", "I have nothing.");
            RMUD.Core.OverrideMessage("carrying", "I've got..");
            RMUD.Core.OverrideMessage("not lockable", "I don't think that can be locked.");
            RMUD.Core.OverrideMessage("you lock", "Okay, <the0> is locked.");
            RMUD.Core.OverrideMessage("they lock", "^<the0> locks <the1> with <the2>.");
            RMUD.Core.OverrideMessage("nowhere", "You aren't anywhere.");
            RMUD.Core.OverrideMessage("dark", "It's so dark I can't see anything.");
            RMUD.Core.OverrideMessage("also here", "I also see <l0>.");
            RMUD.Core.OverrideMessage("on which", "(on which is <l0>)");
            RMUD.Core.OverrideMessage("obvious exits", "I could go");
            RMUD.Core.OverrideMessage("through", "through <the0>");
            RMUD.Core.OverrideMessage("to", "to <the0>");
            RMUD.Core.OverrideMessage("cant look relloc", "I can't figure out how to look <s0> that.");
            RMUD.Core.OverrideMessage("is closed error", "^<the0> is closed.");
            RMUD.Core.OverrideMessage("relloc it is", "^<s0> <the1> is..");
            RMUD.Core.OverrideMessage("nothing relloc it", "I don't see anything <s0> <the1>.");
            RMUD.Core.OverrideMessage("not openable", "I don't know how I would open that.");
            RMUD.Core.OverrideMessage("you open", "Okay, the <the0> is open.");
            RMUD.Core.OverrideMessage("they open", "^<the0> opens <the1>.");
            RMUD.Core.OverrideMessage("cant put relloc", "I don't think I can put things <s0> that.");
            RMUD.Core.OverrideMessage("you put", "Okay, I put <the0> <s1> <the2>.");
            RMUD.Core.OverrideMessage("they put", "^<the0> puts <the1> <s2> <the3>.");
            RMUD.Core.OverrideMessage("say what", "Say what?");
            RMUD.Core.OverrideMessage("emote what", "You exist. Actually this is an error message, but that's what you just told me to say.");
            RMUD.Core.OverrideMessage("speak", "^<the0> : \" < s1 > \"");
            RMUD.Core.OverrideMessage("emote", "^<the0> <s1>");
            RMUD.Core.OverrideMessage("you take", "Okay, I've got <the0>.");
            RMUD.Core.OverrideMessage("they take", "^<the0> takes <the1>.");
            RMUD.Core.OverrideMessage("cant take people", "How would I take a person?");
            RMUD.Core.OverrideMessage("cant take portals", "I think that's a bit impossible.");
            RMUD.Core.OverrideMessage("cant take scenery", "I don't think I can.");
            RMUD.Core.OverrideMessage("you unlock", "Okay, I unlocked <the0>.");
            RMUD.Core.OverrideMessage("they unlock", "^<the0> unlocks <the1> with <a2>.");
            RMUD.Core.OverrideMessage("help topics", "Available help topics");
            RMUD.Core.OverrideMessage("no help topic", "There is no help available for that topic.");
        }
    }
}
