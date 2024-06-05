using System.Collections.Generic;
using System.Linq;
using SLRGenerator.Types;

namespace SLRGenerator
{
    public class EmptyRemover
    {
        private readonly List<Rule> _rules;

        public EmptyRemover(IEnumerable<Rule> rules)
        {
            _rules = rules.ToList();
        }


        public List<Rule> RemoveEmpty()
        {
            var foundEmpty = true;
            while (foundEmpty) foundEmpty = RemoveEmptySingle();

            return _rules;
        }

        private bool RemoveEmptySingle()
        {
            for (var i = 0; i < _rules.Count; i++)
                if (_rules[i].Items.Any(t => t.Value == Constants.EmptySymbol))
                {
                    var nonTerminal = _rules[i].NonTerminal;
                    _rules.RemoveAt(i);
                    RebuildRules(nonTerminal);
                    return true;
                }

            return false;
        }

        private void RebuildRules(string nonTerm)
        {
            foreach (var rule in _rules.ToList())
                if (rule.Items.Any(x => x.Value == nonTerm))
                {
                    var newItems = rule.Items
                        .Where(x => x.Value != nonTerm)
                        .Select(x => x.Clone()).ToList();
                    if (newItems.All(x => x.Value == Constants.EndSymbol))
                        continue;

                    var newRule = new Rule {NonTerminal = rule.NonTerminal, Items = newItems};
                    var index = _rules.FindLastIndex(x => x.NonTerminal == newRule.NonTerminal);
                    _rules.Insert(index + 1, newRule);
                }
        }
    }
}