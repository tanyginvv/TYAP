using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SLRGenerator.Types;

namespace SLRGenerator
{
    public static class SimpleRulesParser
    {
        private static void InsertRuleAtStart(IList<Rule> rules)
        {
            rules.Insert(0, new Rule
            {
                NonTerminal = Extensions.GetNextFreeLetter(rules.GroupBy(x => x.NonTerminal)
                    .Select(k => k.Key).ToHashSet()),
                Items = new List<RuleItem> {new(rules[0].NonTerminal)}
            });
        }

        public static List<Rule> Parse(Stream stream)
        {
            using var sr = new StreamReader(stream);
            string line;
            var rawRules = new List<(string LeftBody, string RightBody)>();
            while ((line = sr.ReadLine()) != null)
            {
                var split = line.Split("->", StringSplitOptions.TrimEntries);
                var localRules = split[1].Split("|", StringSplitOptions.TrimEntries);
                rawRules.AddRange(localRules.Select(rule => (split[0].Trim(), rule.Trim())));
            }

            var nonTerminals = rawRules.Select(x => x.LeftBody).ToHashSet();

            var rules = rawRules.Select(rawRule => new Rule
                {
                    NonTerminal = rawRule.LeftBody,
                    Items = rawRule.RightBody.Split(" ", StringSplitOptions.TrimEntries)
                        .Select(x => nonTerminals.Contains(x)
                            ? new RuleItem(x)
                            : new RuleItem(x, true))
                        .ToList()
                })
                .ToList();

            if (rules[0].Items[^1] != Constants.EndSymbol)
            {
                if (rules.Count(x => x.NonTerminal == rules[0].NonTerminal) > 1) InsertRuleAtStart(rules);
                rules[0].Items.Add(new RuleItem(Constants.EndSymbol, true));
            }

            var fixedRules = new EmptyRemover(rules).RemoveEmpty();

            for (var i = 0; i < fixedRules.Count; i++)
            for (var j = 0; j < fixedRules[i].Items.Count; j++)
                fixedRules[i].Items[j].Id = new RuleItemId(i, j);

            foreach (var item in fixedRules) Console.WriteLine(item);
            Console.WriteLine();

            return fixedRules;
        }
    }
}