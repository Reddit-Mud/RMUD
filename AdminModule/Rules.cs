﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace AdminModule
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
                    if (match.ContainsKey("OBJECT"))
                    {
                        if (match.ContainsKey("BOOK-NAME"))
                            DisplaySingleBook(actor, (match["OBJECT"] as MudObject).Rules, match["BOOK-NAME"].ToString());
                        else
                            DisplayBookList(actor, (match["OBJECT"] as MudObject).Rules);
                    }
                    else if (match.ContainsKey("BOOK-NAME"))
                        DisplaySingleBook(actor, Core.GlobalRules.Rules, match["BOOK-NAME"].ToString());
                    else
                        DisplayBookList(actor, Core.GlobalRules.Rules);
                    return PerformResult.Continue;
                });
        }

        private static void DisplaySingleBook(MudObject Actor, RuleSet From, String BookName)
        {
            if (From == null || From.FindRuleBook(BookName) == null)
                MudObject.SendMessage(Actor, "[no rules]");
            else
            {
                var book = From.FindRuleBook(BookName);
                DisplayBookHeader(Actor, book);
                foreach (var rule in book.Rules)
                    MudObject.SendMessage(Actor, rule.DescriptiveName == null ? "[Unnamed rule]" : rule.DescriptiveName);
            }
        }

        private static void DisplayBookHeader(MudObject Actor, RuleBook Book)
        {
            MudObject.SendMessage(Actor, Book.Name + " -> " + Book.ResultType.Name + " : " + Book.Description);
        }

        private static void DisplayBookList(MudObject Actor, RuleSet Rules)
        {
            if (Rules == null || Rules.RuleBooks.Count == 0)
                MudObject.SendMessage(Actor, "[no rules]");
            else
                foreach (var book in Rules.RuleBooks)
                    DisplayBookHeader(Actor, book);
        }

        
    }
}