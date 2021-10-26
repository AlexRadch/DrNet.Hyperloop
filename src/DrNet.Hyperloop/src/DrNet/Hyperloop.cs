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
        private Func<HyperloopRecursions<T>, IEnumerable<T>>? EnumerableImplementation { get; set; }

        /// <summary>
        /// Function that return implementation of non-quadratic recursive iterations enumerator.
        /// </summary>
        private Func<HyperloopRecursions<T>, IEnumerator<T>>? EnumeratorImplementation { get; set; }

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
        public Hyperloop(Func<HyperloopRecursions<T>, IEnumerable<T>> implementation) : this(implementation, -1) { }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerable.</param>
        /// 
        /// <param name="depth">Recursion depth.</param>
        public Hyperloop(Func<HyperloopRecursions<T>, IEnumerable<T>> implementation, int depth)
        {
            EnumerableImplementation = implementation;
            Depth = depth;
        }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerator.</param>
        public Hyperloop(Func<HyperloopRecursions<T>, IEnumerator<T>> implementation) : this(implementation, -1) { }

        /// <summary>
        /// Creates non-quadratic recursive enumerable.
        /// </summary>
        /// 
        /// <param name="implementation">Function that return implementation of non-quadratic recursive iterations enumerator.</param>
        /// 
        /// <param name="depth">Recursion depth.</param>
        public Hyperloop(Func<HyperloopRecursions<T>, IEnumerator<T>> implementation, int depth)
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
                enumerator._recursions.Add(EnumerableImplementation(enumerator._recursions));
            else if (!(EnumeratorImplementation is null))
                enumerator._recursions.Add(EnumeratorImplementation(enumerator._recursions));

            return enumerator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Enumerator

        /// <summary>
        /// Implement non-quadratic recursive enumerator that iterates through the recursive collection.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            #region private properties

            internal readonly HyperloopRecursions<T> _recursions;

            #endregion

            #region Constructor

            /// <summary>
            /// Creates non-quadratic recursive enumerator that iterates through the recursive collection.
            /// </summary>
            /// 
            /// <param name="depth">Recursion depth if you know it or less 0 if you don't know it.</param>
            internal Enumerator(int depth)
            {
                _recursions = new HyperloopRecursions<T>(depth);
            }

            #endregion

            #region IEnumerator

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public T Current => _recursions._stack.Peek().Current;

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
                while (_recursions._stack.Count > 0)
                {
                    IEnumerator<T> workNode = _recursions._stack.Peek();
                    if (workNode.MoveNext())
                    {
                        if (_recursions._stack.Peek() == workNode)
                            return true;
                    }
                    else
                    {
                        if (_recursions._stack.Peek() == workNode)
                            _recursions._stack.Pop().Dispose();
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
                while (_recursions._stack.Count > 0)
                    _recursions._stack.Pop().Dispose();

                _recursions._stack.Clear();
            }

            #endregion
        }

        #endregion
    }
}
