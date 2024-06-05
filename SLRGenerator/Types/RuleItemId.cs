using System;

namespace SLRGenerator.Types
{
    public class RuleItemId : IEquatable<RuleItemId>
    {
        public readonly int ItemIndex;
        public readonly int RuleIndex;

        public RuleItemId(int ruleRuleIndex, int ruleItemIndex)
        {
            RuleIndex = ruleRuleIndex;
            ItemIndex = ruleItemIndex;
        }

        public bool Equals(RuleItemId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RuleIndex == other.RuleIndex && ItemIndex == other.ItemIndex;
        }

        public override string ToString()
        {
            return $"{RuleIndex + 1}{ItemIndex + 1}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RuleItemId) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RuleIndex, ItemIndex);
        }
    }
}