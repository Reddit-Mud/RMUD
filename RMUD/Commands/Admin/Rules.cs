using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Rules : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!RULES"),
                    Optional(Object("OBJECT", InScope)),
                    Optional(Rest("BOOK-NAME"))))
                .Manual("Lists rules and rulebooks. Both arguments are optional. If no object is supplied, it will list global rules. If no book name is supplied, it will list books rather than listing rules.")
                .ProceduralRule((match, actor) =>
                {
                    if (match.Arguments.ContainsKey("OBJECT"))
                    {
                        if (match.Arguments.ContainsKey("BOOK-NAME"))
                            DisplaySingleBook(actor, (match.Arguments["OBJECT"] as MudObject).Rules, match.Arguments["BOOK-NAME"].ToString());
                        else
                            DisplayBookList(actor, (match.Arguments["OBJECT"] as MudObject).Rules);
                    }
                    else if (match.Arguments.ContainsKey("BOOK-NAME"))
                        DisplaySingleBook(actor, GlobalRules.Rules, match.Arguments["BOOK-NAME"].ToString());
                    else
                        DisplayBookList(actor, GlobalRules.Rules);
                    return PerformResult.Continue;
                });
        }

        private static void DisplaySingleBook(Actor Actor, RuleSet From, String BookName)
        {
            if (From == null || From.FindRuleBook(BookName) == null)
                Mud.SendMessage(Actor, "[no rules]");
            else
            {
                var book = From.FindRuleBook(BookName);
                DisplayBookHeader(Actor, book);
                foreach (var rule in book.Rules)
                    Mud.SendMessage(Actor, rule.DescriptiveName == null ? "[Unnamed rule]" : rule.DescriptiveName);
            }
        }

        private static void DisplayBookHeader(Actor Actor, RuleBook Book)
        {
            Mud.SendMessage(Actor, Book.Name + " [" + String.Join(", ", Book.ArgumentTypes.Select(t => t.Name)) + " -> " + Book.ResultType.Name + "] : " + Book.Description);
        }

        private static void DisplayBookList(Actor Actor, RuleSet Rules)
        {
            if (Rules == null || Rules.RuleBooks.Count == 0)
                Mud.SendMessage(Actor, "[no rules]");
            else
                foreach (var book in Rules.RuleBooks)
                    DisplayBookHeader(Actor, book);
        }

        
    }
}