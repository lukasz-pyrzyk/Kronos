using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public class KeyComperer : IEqualityComparer<Key>
    {
        public bool Equals(Key x, Key y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(Key obj)
        {
            return obj.GetHashCode();
        }
    }
}
