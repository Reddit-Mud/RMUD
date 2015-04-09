using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AliasModule
{
	internal class Alias : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("ALIAS"),
                    SingleWord("NAME"),
                    Rest("RAW-COMMAND")))
                .Manual("Create an alias for another command, or a series of them.")
                .ProceduralRule((match, actor) =>
                {
                    if (!actor.HasProperty<Dictionary<String, String>>("aliases"))
                        actor.SetProperty("aliases", new Dictionary<String, String>());
                    var aliases = actor.GetProperty<Dictionary<String, String>>("aliases");
                    aliases.Add(match["NAME"].ToString().ToUpper(), match["RAW-COMMAND"].ToString());
                    MudObject.SendMessage(actor, "Alias added.");
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Generic((match, context) =>
                {   
                    var r = new List<PossibleMatch>();
                    if (!context.ExecutingActor.HasProperty<Dictionary<String, String>>("aliases"))
                        return r;
                    var aliases = context.ExecutingActor.GetProperty<Dictionary<String, String>>("aliases");
                    if (aliases.ContainsKey(match.Next.Value.ToUpper()))
                        r.Add(match.AdvanceWith("ALIAS", aliases[match.Next.Value.ToUpper()]));
                    return r;
                }, "<ALIAS NAME>"))
                .Manual("Execute an alias.")
                .ProceduralRule((match, actor) =>
                {
                    var commands = match["ALIAS"].ToString().Split(';');
                    foreach (var command in commands)
                        Core.EnqueuActorCommand(actor, command);
                    return PerformResult.Continue;
                });
        }
	}
}
