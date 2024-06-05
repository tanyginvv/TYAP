using System;
using System.Collections.Generic;
using System.Linq;

namespace SLRGenerator.Types
{
    public class RuleItems : List<RuleItem>, IEquatable<RuleItems>
    {
        public bool Equals(RuleItems other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.SequenceEqual(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RuleItems) obj);
        }

        public override int GetHashCode()
        {
            return string.Join("", this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Join("", this);
        }
    }
}