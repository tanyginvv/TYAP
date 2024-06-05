using System;
using System.Collections.Generic;
using System.Linq;
using SLRGenerator.Types;

namespace SLRGenerator.Table
{
    public class TableBuilder
    {
        private readonly List<Rule> _rules;
        private readonly List<string> _valueKeys;

        public TableBuilder(List<Rule> rules)
        {
            _rules = rules;
            var valueKeys = new HashSet<string>();
            foreach (var rule in rules)
            {
                valueKeys.Add(rule.NonTerminal);
                foreach (var ruleItem in rule.Items.Where(ruleItem =>
                    ruleItem != Constants.EmptySymbol && ruleItem != Constants.EndSymbol))
                    valueKeys.Add(ruleItem.Value);
            }

            valueKeys.Add(Constants.EndSymbol);
            _valueKeys = valueKeys.ToList();
        }

        public List<TableRule> CreateTable()
        {
            var tableRules = new List<TableRule>();
            var keyQueue = new Queue<RuleItems>();
            var queueBlackList = new HashSet<RuleItems>();
            {
                var itemId = new RuleItemId(0, -1);
                var tableRule = new TableRule(_rules[itemId.RuleIndex].NonTerminal, _valueKeys);

                AddNext(tableRule, itemId);
                AddToQueue(tableRule);
                tableRules.Add(tableRule);
            }

            while (keyQueue.Count > 0)
            {
                var items = keyQueue.Dequeue();
                queueBlackList.Add(items);
                var key = string.Join("", items.Select(x => x.ToString()));
                if (tableRules.Any(x => x.Key == key))
                    continue;

                var tableRule = new TableRule(key, _valueKeys);
                foreach (var item in items)
                    if (_rules[item.Id.RuleIndex].Items.Count <= item.Id.ItemIndex + 1)
                    {
                        var nextItems = FindNextRecursive(_rules[item.Id.RuleIndex].NonTerminal);
                        foreach (var nextItem in nextItems)
                            AddFold(tableRule, new RuleItemId(nextItem.Id.RuleIndex, nextItem.Id.ItemIndex - 1),
                                new RuleItem("R" + (item.Id.RuleIndex + 1)));
                    }
                    else if (_rules[item.Id.RuleIndex].Items[item.Id.ItemIndex + 1].Value == Constants.EndSymbol)
                    {
                        tableRule.Values[Constants.EndSymbol].Add(new RuleItem("R" + (item.Id.RuleIndex + 1)));
                    }
                    else
                    {
                        AddNext(tableRule, item.Id);
                    }

                tableRules.Add(tableRule);
                AddToQueue(tableRule);
            }

            // Вывод направляющих множеств для каждого правила
            foreach (var tableRule in tableRules)
            {
                Console.WriteLine($"Guiding Set for {tableRule.Key}:");
                var guidingSet = GetGuidingSet(tableRule);
                foreach (var item in guidingSet)
                {
                    Console.WriteLine($"   {item}");
                }
            }

            return tableRules;

            void AddNext(TableRule tableRule, RuleItemId itemId)
            {
                var next = _rules[itemId.RuleIndex].Items[itemId.ItemIndex + 1];
                tableRule.QuickAdd(next);
                foreach (var rule in _rules.Where(x => x.NonTerminal == next.Value))
                    if (next.Value != rule.Items[0].Value && !rule.Items[0].IsTerminal)
                    {
                        AddNext(tableRule, new RuleItemId(rule.Items[0].Id.RuleIndex, rule.Items[0].Id.ItemIndex - 1));
                    }
                    else if (rule.Items[0].Value == Constants.EmptySymbol)
                    {
                        var nextItems = FindNextRecursive(rule.NonTerminal);
                        foreach (var nItem in nextItems)
                            if (nItem.IsTerminal)
                                tableRule.Values[nItem.Value].Add(new RuleItem("R" + (rule.Items[0].Id.RuleIndex + 1)));
                            else
                                AddFold(tableRule, new RuleItemId(nItem.Id.RuleIndex, nItem.Id.ItemIndex - 1),
                                    new RuleItem("R" + (rule.Items[0].Id.RuleIndex + 1)));
                    }
                    else
                    {
                        tableRule.QuickAdd(rule.Items[0]);
                    }
            }

            void AddFold(TableRule tableRule, RuleItemId itemId, RuleItem ruleItem)
            {
                var next = _rules[itemId.RuleIndex].Items[itemId.ItemIndex + 1];
                tableRule.QuickFold(next, ruleItem);
                foreach (var rule in _rules.Where(x => x.NonTerminal == next.Value))
                    if (next.Value != rule.Items[0].Value && !rule.Items[0].IsTerminal)
                    {
                        AddFold(tableRule, new RuleItemId(rule.Items[0].Id.RuleIndex, rule.Items[0].Id.ItemIndex - 1),
                            ruleItem);
                    }
                    else if (rule.Items[0].Value == Constants.EmptySymbol)
                    {
                        var nextItems = FindNextRecursive(rule.NonTerminal);
                        foreach (var nItem in nextItems)
                            if (nItem.IsTerminal)
                                tableRule.Values[nItem.Value].Add(ruleItem);
                            else
                                AddFold(tableRule, new RuleItemId(nItem.Id.RuleIndex, nItem.Id.ItemIndex - 1),
                                    ruleItem);
                    }
                    else
                    {
                        tableRule.QuickFold(rule.Items[0], ruleItem);
                    }
            }

            void AddToQueue(TableRule tableRule)
            {
                foreach (var item in tableRule.Values
                    .Where(x => x.Value.Count > 0))
                {
                    var value = item.Value;
                    if (!queueBlackList.Contains(value) &&
                        !(value[0].Value.StartsWith("R") && char.IsDigit(value[0].Value[1])))
                        keyQueue.Enqueue(value);
                }
            }
        }

        private IEnumerable<RuleItem> FindNextRecursive(string nonTerm)
        {
            return FindUp(nonTerm, new HashSet<int>());
        }

        private IEnumerable<RuleItem> FindUp(string nonTerm, ISet<int> history)
        {
            var returns = new HashSet<RuleItem>();
            for (var i = 0; i < _rules.Count; i++)
            {
                var rule = _rules[i];
                for (var j = 0; j < rule.Items.Count; j++)
                    if (rule.Items[j].Value == nonTerm)
                    {
                        if (++j < rule.Items.Count)
                        {
                            returns.Add(rule.Items[j]);
                        }
                        else
                        {
                            if (history.Contains(i)) return returns;
                            history.Add(i);
                            var nextReturns = FindUp(rule.NonTerminal, history);
                            foreach (var item in nextReturns)
                                returns.Add(item);
                        }
                    }
            }

            return returns;
        }

        private IEnumerable<string> GetGuidingSet(TableRule tableRule)
        {
            var guidingSet = new HashSet<string>();

            foreach (var value in tableRule.Values)
            {
                foreach (var item in value.Value)
                {
                    if (!item.Value.StartsWith("R"))
                    {
                        guidingSet.Add(item.Value);
                    }
                }

            }

            return guidingSet;
        }
    }
}
