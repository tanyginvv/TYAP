using System;
using System.Collections.Generic;

public static class RuleConverter
{
    public static List<string> ConvertRules(List<string> rules)
    {
        var convertedRules = new List<string>();

        foreach (var rule in rules)
        {
            var parts = rule.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
            var nonTerminal = parts[0].Trim();
            var alternatives = parts[1].Split('|');

            foreach (var alternative in alternatives)
            {
                convertedRules.Add($"{nonTerminal} -> {alternative.Trim()}");
            }
        }

        return convertedRules;
    }
}