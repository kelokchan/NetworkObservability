using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability.core
{
    class Graph : IEnumerable<Node>
    {
        private List<Node> nodes;
        private List<Arc> arcs;

        public Graph(Node firstNode)
        {
            nodes = new List<Node>();
            nodes.Add(firstNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Node>)nodes).GetEnumerator();
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            return new GraphEnum(nodes);
        }

        private class GraphEnum : IEnumerator<Node>
        {
            private Int32 position;
            private IList<Node> nodes;

            public GraphEnum(IList<Node> nodes)
            {
                this.nodes = nodes;
                Reset();
            }

            public bool MoveNext()
            {
                return ++position < nodes.Count;
            }

            public void Reset()
            {
                position = -1;
            }

            public Node Current
            {
                get
                {
                    try
                    {
                        return nodes[position];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    nodes = null;
                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~GraphEnum() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                GC.SuppressFinalize(this);
            }
            #endregion
        }

        public static bool BFS(Node root, Node target)
        {
            var explored = new HashSet<Node>();
            var frontier = new Queue<Node>();
            frontier.Enqueue(root);

            while (frontier.Count != 0)
            {
                var node = frontier.Dequeue();
                if (node.Equals(target))
                {
                    return true;
                }
                else
                {
                    explored.Add(node);
                    foreach (var child in node.GetAdjacencies)
                    {
                        frontier.Enqueue(child);
                    }
                }
            }
            return false;
        }
    }

}
