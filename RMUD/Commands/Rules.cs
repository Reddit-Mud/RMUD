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
                new Sequence(
                    new RankGate(500),
                    new KeyWord("RULES", false),
                    new Optional(new ObjectMatcher("OBJECT", new InScopeObjectSource())),
                    new Optional(new Rest("BOOK-NAME"))),
                new RulesProcessor(),
                "View defined rules.");
        }
    }

    internal class RulesProcessor : CommandProcessor
    {
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

        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var bookNameBuilder = new StringBuilder();
            if (Match.Arguments.ContainsKey("BOOK-NAME"))
                Mud.AssembleText(Match.Arguments["BOOK-NAME"] as LinkedListNode<String>, bookNameBuilder);


            if (Match.Arguments.ContainsKey("OBJECT"))
            {
                if (Match.Arguments.ContainsKey("BOOK-NAME"))
                    DisplaySingleBook(Actor, (Match.Arguments["OBJECT"] as MudObject).Rules, bookNameBuilder.ToString());
                else
                    DisplayBookList(Actor, (Match.Arguments["OBJECT"] as MudObject).Rules);
            }
            else if (Match.Arguments.ContainsKey("BOOK-NAME"))
                DisplaySingleBook(Actor, GlobalRules.Rules, bookNameBuilder.ToString());
            else
                DisplayBookList(Actor, GlobalRules.Rules);
        }
    }
}