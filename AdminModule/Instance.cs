﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AdminModule
{
    internal class Instance : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!INSTANCE"),
                    MustMatch("It helps if you give me a path.",
                        Path("PATH"))))
                .Manual("Given a path, create a new instance of an object.")
                .ProceduralRule((match, actor) =>
                {
                    var path = match["PATH"].ToString();
                    var newObject = MudObject.GetObject(path + "@" + Guid.NewGuid().ToString());
                    if (newObject == null) MudObject.SendMessage(actor, "Failed to instance " + path + ".");
                    else
                    {
                        MudObject.Move(newObject, actor);
                        MudObject.SendMessage(actor, "Instanced " + path + ".");
                    }
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}