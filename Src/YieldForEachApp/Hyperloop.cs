using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{

    public interface IHyperloop<in T>
    {
        void Add(IEnumerable<T> source);
        void Add(IEnumerator<T> loop);
    }

    public sealed class Hyperloop<T>: IHyperloop<T>, IEnumerable<T>, /*IEnumerable,*/ IEnumerator<T> /*, IEnumerator, IDisposable */
    {

        private readonly LinkedList<IEnumerator<T>> _loops = new LinkedList<IEnumerator<T>>();
        private LinkedListNode<IEnumerator<T>> _workNode;

        #region IHyperloop

        public void Add(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Add(source.GetEnumerator());
        }

        public void Add(IEnumerator<T> loop)
        {
            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            if (_workNode == null)
                _loops.AddFirst(loop);
            else
                _loops.AddAfter(_workNode, loop);
        }

        #endregion

        #region IEnumerable

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        #endregion

        #region IEnumerator

        public T Current => _loops.Last.Value.Current;

        object IEnumerator.Current => _loops.Last.Value.Current;

        public bool MoveNext()
        {
            while (_loops.Count > 0)
            {
                _workNode = _loops.Last;
                if (_workNode.Value.MoveNext())
                {
                    if (_workNode.Next == null)
                        return true;
                }
                else
                    Dispose(_workNode);
            }
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDispose

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        private void Dispose(LinkedListNode<IEnumerator<T>> node)
        {
            _loops.Remove(node);
            node.Value.Dispose();
        }

        /*protected virtual*/ void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var node in _loops)
                node.Dispose();
            _loops.Clear();
        }

        #endregion
    }
}
