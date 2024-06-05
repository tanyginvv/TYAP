using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SLRGenerator.Types;

namespace SLRGenerator
{
    public class Analyzer
    {
        private readonly string[] _input;
        private readonly List<Rule> _rules;
        private readonly List<TableRule> _tableRules;

        public Analyzer(Stream stream, List<TableRule> table, List<Rule> rules)
        {
            _input = InputParser(stream);
            _tableRules = table;
            _rules = rules;
        }

        private static string[] InputParser(Stream stream)
        {
            using var sr = new StreamReader(stream);
            string line;
            var split = Array.Empty<string>();
            while ((line = sr.ReadLine()) != null)
            {
                split = line.Split();
                break;
            }

            return split;
        }

        public void Analyze()
        {
            var left = new Stack<string>();
            var right = new Stack<string>();
            var inputStack = new Stack<string>();
            foreach (var input in _input.Reverse())
                inputStack.Push(input);

            right.Push(_tableRules.First().Key);
            while (true)
                try
                {
                    var character = "";
                    if (inputStack.Count > 0) character = inputStack.Pop();
                    var values = _tableRules.First(x => x.Key == right.Peek()).Values;
                    var items = character == ""
                        ? values.Where(x => x.Key == Constants.EndSymbol).ToList()
                        : values.Where(x => x.Key == character).ToList();

                    if (items.Count == 0)
                        throw new Exception("Items are empty");

                    var elements = items.First().Value;
                    if (elements.First().Value.Length > 1 && elements.First().Value.StartsWith("R"))
                    {
                        if (char.IsDigit(elements.First().Value[1]))
                        {
                            if (character != "")
                                inputStack.Push(character);

                            var ruleNumber =
                                int.Parse(elements.First().Value.Substring(1, elements.First().Value.Length - 1)) - 1;
                            var rule = _rules[ruleNumber];

                            if (rule.Items[0].Value != Constants.EmptySymbol)
                                for (var i = 0; i < rule.Items.Count && rule.Items[i].Value != Constants.EndSymbol; i++)
                                {
                                    left.Pop();
                                    right.Pop();
                                }

                            if (right.Count == 1 && left.Count == 0 && inputStack.Count == 0)
                            {
                                Console.WriteLine("Analyzer correct!");
                                return;
                            }

                            inputStack.Push(rule.NonTerminal);
                        }
                    }
                    else
                    {
                        right.Push(elements.ToString());
                        left.Push(character);
                    }

                    Console.WriteLine($"Left [{string.Join(", ", left.ToArray())}]" +
                                      $" Input [{string.Join(" ", inputStack.ToArray())}]" +
                                      $" Right [{string.Join(", ", right.ToArray())}]");
                }
                catch (Exception e)
                {
                    throw new ArgumentException("[Syntax Analyzer Error] " + e + "\r\n*** Analyzer State ***" +
                                                $"\r\nLeft [{string.Join(", ", left.ToArray())}]" +
                                                $"\r\nInput [{string.Join(" ", inputStack.ToArray())}]" +
                                                $"\r\nRight [{string.Join(", ", right.ToArray())}]");
                }
        }
    }
}