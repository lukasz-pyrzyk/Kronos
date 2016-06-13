using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public class NodeComparer : IEqualityComparer<NodeMetatada>
    {
        public bool Equals(NodeMetatada x, NodeMetatada y)
        {
            if (x == null || y == null) return false;

            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(NodeMetatada obj)
        {
            return obj.GetHashCode();
        }
    }
}
