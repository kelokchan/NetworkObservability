using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    namespace Core
    {
        public class Graph : IEnumerable<Node>
        {
            private HashSet<Node> nodes;
            private HashSet<Arc> arcs;

            public Graph(Node firstNode)
            {
				nodes = new HashSet<Node> { firstNode };
			}

            public void Add(Node @from, Node @to, Arc through, bool blockLeft = false, bool blockRight = false, bool blocked = false)
            {
                through.AttachTo(@from, @to, blockLeft, blockRight, blocked);
            }

            /// <summary>
            /// This method returns a <see cref="IEnumerable{Node}"/> that
            /// iterate in Breadth First sequence.
            /// </summary>
            public IEnumerable<Node> BreadthFirstTraversal()
            {
                var explored = new HashSet<Node>();
                var frontier = new Queue<Node>();
                frontier.Enqueue(nodes.First());

                while (frontier.Count != 0)
                {
                    var current = frontier.Dequeue();
                    yield return current;

                    explored.Add(current);
                    var childrenLink = current.GetAdjacencies;
                    foreach (var childLink in childrenLink)
                    {
                        var child = childLink.AnotherEnd(current);
                        if (!explored.Contains(child))
                            frontier.Enqueue(child);
                    }
                }

            }

            /// <summary>
            /// This method returns a <see cref="IEnumerable{Node}"/> that
            /// iterate in Depth First sequence.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Node> DepthFirstTraversal()
            {
                var explored = new HashSet<Node>();
                var frontier = new Stack<Node>();
                frontier.Push(nodes.First());
                
                while (frontier.Count != 0)
                {
                    var current = frontier.Pop();
                    yield return current;

                    explored.Add(current);
                    var childrenLink = current.GetAdjacencies;
                    foreach (var childLink in childrenLink)
                    {
                        var child = childLink.AnotherEnd(current);
                        if (!explored.Contains(child))
                            frontier.Push(child);
                    }
                }
            }

            public Node BreadthFirstSearchByName(String name)
            {
                var explored = new HashSet<Node>(new NodeNameComparer());
                var frontier = new Queue<Node>();
                frontier.Enqueue(nodes.First());

                while (frontier.Count != 0)
                {
                    var current = frontier.Dequeue();
                    explored.Add(current);
                    if (current.Name == name)
                    {
                        return current;
                    }
                    else
                    {
                        var childrenLink = current.GetAdjacencies;
                        foreach (var childLink in childrenLink)
                        {
                            var child = childLink.AnotherEnd(current);
                            if (!explored.Contains(child))
                                frontier.Enqueue(child);
                        }
                    }
                }

                return null;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (nodes as IEnumerable<Node>).GetEnumerator();
            }

            IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
            {
                return new GraphEnum(nodes.ToList());
            }

            /// <summary>
            /// This class is a helper class of <see cref="Graph"/> to enumerate through
            /// all the nodes.
            /// </summary>
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
        }
    }
}
