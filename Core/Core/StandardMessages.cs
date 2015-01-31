using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    internal static class StandardMessages
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("not here", "I don't see that here.");
            Core.StandardMessage("gone", "The doesn't seem to be here any more.");
            Core.StandardMessage("dont have that", "You don't have that.");
            Core.StandardMessage("already have that", "You already have that.");
            Core.StandardMessage("does nothing", "That doesn't seem to do anything.");
            Core.StandardMessage("nothing happens", "Nothing happens.");
            Core.StandardMessage("unappreciated", "I don't think <the0> would appreciate that.");
            Core.StandardMessage("already open", "It is already open.");
            Core.StandardMessage("already closed", "It is already closed.");
            Core.StandardMessage("close it first", "You'll have to close it first.");
            Core.StandardMessage("wrong key", "That is not the right key.");
            Core.StandardMessage("error locked", "It seems to be locked.");
        }
    }
}
