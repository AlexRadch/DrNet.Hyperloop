using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace YieldForEachApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = 1;
            var end = 1024;
            var times = 100;

            var bLen = b.ToString().Length.ToString(CultureInfo.InvariantCulture);
            var endLen = end.ToString().Length.ToString(CultureInfo.InvariantCulture);
            var timesLen = times.ToString().Length.ToString(CultureInfo.InvariantCulture);


            var fmt = string.Format("Standart from {{0,{0}}} to {{1,{1}}}, {{2,{2}}} times: {{3,10}} ticks", bLen, endLen, timesLen);
            for (var e = 1; e <= end;  e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedStandart(b, e).Count());
                Console.WriteLine(fmt, b, e, times, time);
            }

            fmt = string.Format("Hacked   from {{0,{0}}} to {{1,{1}}}, {{2,{2}}} times: {{3,10}} ticks", bLen, endLen, timesLen);
            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHacked(b, e).Count());
                Console.WriteLine(fmt, b, e, times, time);
            }

            Console.ReadLine();
        }

        static long Benchmark(int times, Action act)
        {
            GC.Collect();
            act(); // run once outside of loop to avoid initialization costs
            Stopwatch sw = Stopwatch.StartNew();
            while (--times >= 0)
                act();
            sw.Stop();
            return sw.ElapsedTicks;
        }

        static IEnumerable<int> FromToNestedStandart(int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            foreach(var v in FromToNestedStandart(b + 1, e))
                yield return v;
        }

        [IteratorStateMachine(typeof(FromToNestedHackedС))]
        static IEnumerable<int> FromToNestedHacked(int b, int e)
        {
            FromToNestedHackedС src = new FromToNestedHackedС(-2);
            src._b = b;
            src._e = e;
            return src;
        }

        [IteratorStateMachine(typeof(FromToNestedRecursiveС))]
        static IEnumerable<int> FromToNestedRecursive(int b, int e)
        {
            FromToNestedRecursiveС src = new FromToNestedRecursiveС(-2);
            src._b = b;
            src._e = e;
            return src;
        }

        [CompilerGenerated]
        private sealed class FromToNestedHackedС: IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
        {
            private int state;
            private int current;
            private int initialThreadId;
            private int b;
            public int _b;
            private int e;
            public int _e;
            private IEnumerator<int> _1;

            int IEnumerator<int>.Current
            {
                [DebuggerHidden]
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return current;
                }
            }

            [DebuggerHidden]
            public FromToNestedHackedС(int state)
            {
                this.state = state;
                initialThreadId = Environment.CurrentManagedThreadId;
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
                switch (state)
                {
                    case -3:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.Finally1();
                        }
                        break;
                }
            }

            bool IEnumerator.MoveNext()
            {
                // ISSUE: fault handler
                try
                {
                    switch (state)
                    {
                        case 0:
                            state = -1;
                            if (b > e)
                                return false;
                            current = b;
                            state = 1;
                            return true;
                        case 1:
                            state = -1;
                            _1 = FromToNestedHacked(b + 1, e).GetEnumerator();
                            state = -3;
                            break;
                        case 2:
                            this.state = -3;
                            break;
                        default:
                            return false;
                    }
                    if (_1.MoveNext())
                    {
                        current = _1.Current;
                        state = 2;
                        return true;
                    }
                    Finally1();
                    _1 = null;
                    return false;
                }
                catch
                {
                    ((IDisposable)this).Dispose();
                    throw;
                }
            }

            private void Finally1()
            {
                this.state = -1;
                if (_1 == null)
                    return;
                _1.Dispose();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                FromToNestedHackedС src;
                if (state == -2 && initialThreadId == Environment.CurrentManagedThreadId)
                {
                    state = 0;
                    src = this;
                }
                else
                    src = new FromToNestedHackedС(0);
                src.b = _b;
                src.e = _e;
                return src;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)this).GetEnumerator();
            }
        }

        [CompilerGenerated]
        private sealed class FromToNestedRecursiveС : IRecursiveEnumerable<int>, IEnumerable<int>, IEnumerable, IEnumerator<int>, IDisposable, IEnumerator
        {
            private int state;
            private int current;
            private int initialThreadId;
            private int b;
            public int _b;
            private int e;
            public int _e;
            private IEnumerator<int> _1;
            private Stack<IEnumerator<int>> stack;

            int IEnumerator<int>.Current
            {
                [DebuggerHidden]
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return current;
                }
            }

            [DebuggerHidden]
            public FromToNestedRecursiveС(int state, Stack<IEnumerator<int>> stack)
            {
                this.state = state;
                this.stack = stack;
                initialThreadId = Environment.CurrentManagedThreadId;
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
                switch (state)
                {
                    case -3:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.Finally1();
                        }
                        break;
                }
            }

            bool IEnumerator.MoveNext()
            {
                // ISSUE: fault handler
                try
                {
                    switch (state)
                    {
                        case 0:
                            state = -1;
                            if (stack == null)
                            {
                                stack = new Stack<IEnumerator<int>>();
                                stack.Push(this);
                            }
                                if (b > e)
                                return false;
                            current = b;
                            state = 1;
                            return true;
                        case 1:
                            state = -1;

                            //_1 = FromToNestedHacked(b + 1, e).GetEnumerator();
                            var src = FromToNestedHacked(b + 1, e);
                            var recursive = src as IRecursiveEnumerable<int>;
                            if (recursive != null)
                                _1 = recursive.GetEnumerator(stack);
                            else
                                _1 = src.GetEnumerator();

                            state = -3;

                            //break;
                            stack.Push(_1);
                            state = 2;
                            return false;

                        case 2:
                            this.state = -3;
                            break;
                        default:
                            return false;
                    }
                    //if (_1.MoveNext())
                    //{
                    //    current = _1.Current;
                    //    state = 2;
                    //    return true;
                    //}
                    Finally1();
                    _1 = null;
                    return false;
                }
                catch
                {
                    ((IDisposable)this).Dispose();
                    throw;
                }
            }

            private void Finally1()
            {
                this.state = -1;
                if (_1 == null)
                    return;
                _1.Dispose();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<int> IRecursiveEnumerable<int>.GetEnumerator(Stack<IEnumerator<int>> stack)
            {
                FromToNestedRecursiveС src;
                if (state == -2 && initialThreadId == Environment.CurrentManagedThreadId)
                {
                    state = 0;
                    this.stack = stack;
                    src = this;
                }
                else
                    src = new FromToNestedRecursiveС(0, stack);
                src.b = _b;
                src.e = _e;
                return src;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)this).GetEnumerator();
            }
        }
    }

    interface IRecursiveEnumerable</*out*/ T>: IEnumerable<T>
    {
        IEnumerator<T> GetEnumerator(Stack<IEnumerator<T>> stack);
    }

    public sealed class RecursiveEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
    {
        public RecursiveEnumerator(IEnumerable<T> src)
        {

        }
        T IEnumerator<T>.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
