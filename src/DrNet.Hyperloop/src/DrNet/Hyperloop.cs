using System;
using System.Collections;
using System.Collections.Generic;

namespace DrNet
{
    /// <summary>
    /// Help class to implement non-quadratic recursive iterations. See https://github.com/dotnet/csharplang/discussions/378
    /// </summary>
    /// 
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public sealed class Hyperloop<T>: IEnumerable<T>
    {
        #region private properties

        /// <summary>
        /// Function that return implementation of non-quadratic recursive iterations enumerable.
        /// </summary>
        private Func<IHyperloopRecursion<T>, IEnumerable<T>>? EnumerableImplementation { get; set; }

        /// <summary>
        /// Function that return implementation of non-quadratic recursive iterations enumerator.
        /// </summary>
        private Func<IHyperloopRecursion<T>, IEnumerator<T>>? EnumeratorImplementation { get; set; }

        /// <summary>
        /// Recursion depth if you know it or less 0 if you don't know it.
        /// </summary>
        private int Depth { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerable.</param>
        public Hyperloop(Func<IHyperloopRecursion<T>, IEnumerable<T>> implementation) : this(implementation, -1) { }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerable.</param>
        /// 
        /// <param name="depth">Recursion depth.</param>
        public Hyperloop(Func<IHyperloopRecursion<T>, IEnumerable<T>> implementation, int depth)
        {
            EnumerableImplementation = implementation;
            Depth = depth;
        }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerator.</param>
        public Hyperloop(Func<IHyperloopRecursion<T>, IEnumerator<T>> implementation) : this(implementation, -1) { }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerator.</param>
        /// 
        /// <param name="depth">Recursion depth.</param>
        public Hyperloop(Func<IHyperloopRecursion<T>, IEnumerator<T>> implementation, int depth)
        {
            EnumeratorImplementation = implementation;
            Depth = depth;
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns an non-quadratic enumerator that iterates through the recursive collection.
        /// </summary>
        /// 
        /// <returns>An non-quadratic enumerator that can be used to iterate through the recursive collection.</returns>
        public Enumerator GetEnumerator()
        {
            var enumerator = new Enumerator(Depth);

            if (!(EnumerableImplementation is null))
                enumerator.Add(EnumerableImplementation(enumerator));
            else if (!(EnumeratorImplementation is null))
                enumerator.Add(EnumeratorImplementation(enumerator));

            return enumerator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Enumerator

        /// <summary>
        /// Implement non-quadratic recursive enumerator that iterates through the recursive collection.
        /// </summary>
        public struct Enumerator : IHyperloopRecursion<T>, IEnumerator<T>
        {
            #region private properties

            private readonly Stack<IEnumerator<T>> _stack;

            #endregion

            #region Constructor

            /// <summary>
            /// Creates non-quadratic recursive enumerator that iterates through the recursive collection.
            /// </summary>
            /// 
            /// <param name="depth">Recursion depth if you know it or less 0 if you don't know it.</param>
            public Enumerator(int depth)
            {
                _stack = depth >= 0 ? new Stack<IEnumerator<T>>(depth) : new Stack<IEnumerator<T>>();
            }

            #endregion

            #region IHyperloopRecursion

            /// <summary>
            /// Add recursion in non-quadratic recursive iterations.
            /// </summary>
            /// 
            /// <param name="recursion"></param>
            public void Add(IEnumerable<T> recursion) => Add(recursion.GetEnumerator());

            /// <summary>
            /// Add recursion in non-quadratic recursive iterations.
            /// </summary>
            /// 
            /// <param name="recursion"></param>
            public void Add(IEnumerator<T> recursion) => _stack.Push(recursion);

            /// <summary>
            /// Add tail recursion in non-quadratic recursive iterations.
            /// </summary>
            /// 
            /// <param name="recursion"></param>
            public void AddTail(IEnumerable<T> recursion) => AddTail(recursion.GetEnumerator());

            /// <summary>
            /// Add recursion in non-quadratic recursive iterations.
            /// </summary>
            /// 
            /// <param name="recursion"></param>
            public void AddTail(IEnumerator<T> recursion)
            {
                _stack.Pop().Dispose();
                _stack.Push(recursion);
            }

            #endregion

            #region IEnumerator

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public T Current => _stack.Peek().Current;

            object IEnumerator.Current => Current!;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// 
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element;
            /// false if the enumerator has passed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                while (_stack.Count > 0)
                {
                    IEnumerator<T> workNode = _stack.Peek();
                    if (workNode.MoveNext())
                    {
                        if (_stack.Peek() == workNode)
                            return true;
                    }
                    else
                    {
                        if (_stack.Peek() == workNode)
                            _stack.Pop().Dispose();
                    }
                }
                return false;
            }

            void IEnumerator.Reset() => throw new NotSupportedException();

            #endregion

            #region IDispose

            /// <summary>
            /// Dispose enumerator.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                // GC.SuppressFinalize(this);
            }

            void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                while (_stack.Count > 0)
                    _stack.Pop().Dispose();

                _stack.Clear();
            }

            #endregion
        }

        #endregion
    }
}
