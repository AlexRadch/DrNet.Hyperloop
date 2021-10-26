using System.Collections.Generic;

namespace DrNet
{
    /// <summary>
    /// Contains recursions for non-quadratic recursive iterations.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public struct HyperloopRecursions<T>
    {
        #region private properties

        internal readonly Stack<IEnumerator<T>> _stack;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates non-quadratic recursive enumerator that iterates through the recursive collection.
        /// </summary>
        /// 
        /// <param name="depth">Recursion depth if you know it or less 0 if you don't know it.</param>
        internal HyperloopRecursions(int depth)
        {
            _stack = depth >= 0 ? new Stack<IEnumerator<T>>(depth) : new Stack<IEnumerator<T>>();
        }

        #endregion

        #region Add recursion methods

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
    }
}
