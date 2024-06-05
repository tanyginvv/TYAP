using System.Collections.Generic;

namespace SLRGenerator.Types
{
    public class Rule
    {
        public string NonTerminal { get; init; }
        public List<RuleItem> Items { get; init; }

        public override string ToString()
        {
            return $"{NonTerminal} -> {string.Join(" ", Items)}";
        }
    }
}