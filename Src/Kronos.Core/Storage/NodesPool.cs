using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public class NodesPool
    {
        private readonly Stack<NodeMetatada> _nodes = new Stack<NodeMetatada>();

        public NodeMetatada Rent()
        {
            if (_nodes.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    _nodes.Push(new NodeMetatada());
                }
            }

            return _nodes.Pop();
        }

        public void Return(NodeMetatada node)
        {
            _nodes.Push(node);
        }
    }
}
