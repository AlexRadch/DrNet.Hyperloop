using System.Collections.Generic;

namespace DrNet
{
    /// <summary>
    /// Exposes the recursion in non-quadratic recursive iterations.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public interface IHyperloopRecursion<in T>
    {
        /// <summary>
        /// Add recursion in non-quadratic recursive iterations.
        /// </summary>
        /// 
        /// <param name="recursion"></param>
        void Add(IEnumerable<T> recursion);

        /// <summary>
        /// Add recursion in non-quadratic recursive iterations.
        /// </summary>
        /// 
        /// <param name="recursion"></param>
        void Add(IEnumerator<T> recursion);

        /// <summary>
        /// Add tail recursion in non-quadratic recursive iterations.
        /// </summary>
        /// 
        /// <param name="recursion"></param>
        void AddTail(IEnumerable<T> recursion);

        /// <summary>
        /// Add tail recursion in non-quadratic recursive iterations.
        /// </summary>
        /// 
        /// <param name="recursion"></param>
        void AddTail(IEnumerator<T> recursion);

    }
}
