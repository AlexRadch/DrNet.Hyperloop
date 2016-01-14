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
            FromToNestedHackedС nestedHackedD2 = new FromToNestedHackedС(-2);
            nestedHackedD2._b = b;
            nestedHackedD2._e = e;
            return nestedHackedD2;
        }

        [IteratorStateMachine(typeof(FromToNestedStackedС))]
        static IEnumerable<int> FromToNestedStacked(int b, int e)
        {
            FromToNestedStackedС nestedStackedD2 = new FromToNestedStackedС(-2);
            nestedStackedD2._b = b;
            nestedStackedD2._e = e;
            return nestedStackedD2;
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
            private int _2;

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
                        _2 = _1.Current;
                        current = _2;
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
                FromToNestedHackedС nestedHackedD2;
                if (state == -2 && initialThreadId == Environment.CurrentManagedThreadId)
                {
                    this.state = 0;
                    nestedHackedD2 = this;
                }
                else
                    nestedHackedD2 = new FromToNestedHackedС(0);
                nestedHackedD2.b = _b;
                nestedHackedD2.e = _e;
                return nestedHackedD2;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)this).GetEnumerator();
            }
        }

        [CompilerGenerated]
        private sealed class FromToNestedStackedС : IEnumerable<int>, IEnumerable, IEnumerator<int>, IDisposable, IEnumerator
        {
            private int state;
            private int current;
            private int initialThreadId;
            private int b;
            public int _b;
            private int e;
            public int _e;
            private IEnumerator<int> _1;
            private int _2;

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
            public FromToNestedStackedС(int state)
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
                        _2 = _1.Current;
                        current = _2;
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
                FromToNestedStackedС nestedStackedD2;
                if (state == -2 && initialThreadId == Environment.CurrentManagedThreadId)
                {
                    this.state = 0;
                    nestedStackedD2 = this;
                }
                else
                    nestedStackedD2 = new FromToNestedStackedС(0);
                nestedStackedD2.b = _b;
                nestedStackedD2.e = _e;
                return nestedStackedD2;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)this).GetEnumerator();
            }
        }
    }

    interface IRecursiveEnumerator<T>: IEnumerator<T>
    {
        void SetStack(Stack<IRecursiveEnumerator<T>> stack);
    }

}
