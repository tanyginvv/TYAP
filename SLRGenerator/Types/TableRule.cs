using System.Collections.Generic;
using System.Linq;

namespace SLRGenerator.Types
{
    public class TableRule
    {
        public TableRule(string key, IEnumerable<string> keys)
        {
            Key = key;
            Values = keys.ToDictionary(x => x, _ => new RuleItems());
        }

        public string Key { get; }
        public Dictionary<string, RuleItems> Values { get; }

        public void QuickAdd(RuleItem ruleItem)
        {
            if (Values.ContainsKey(ruleItem.Value))
                if (!Values[ruleItem.Value].Contains(ruleItem))
                    Values[ruleItem.Value].Add(ruleItem);
        }

        public void QuickFold(RuleItem ruleItem, RuleItem foldItem)
        {
            if (Values.ContainsKey(ruleItem.Value))
                if (!Values[ruleItem.Value].Contains(foldItem))
                    Values[ruleItem.Value].Add(foldItem);
        }

        public override string ToString()
        {
            return $"{Key} | {string.Join(" ", Values.Select(x => x.Value))}";
        }
    }
}