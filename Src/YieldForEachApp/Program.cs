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


            var fmt = string.Format("{{0, 10}} from {{1,{0}}} to {{2,{1}}}, {{3,{2}}} times: {{4,9}} ticks. Equal {{5,6}}", bLen, endLen, timesLen);
            for (var e = 1; e <= end;  e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedStandart(b, e).Sum());
                Console.WriteLine(fmt, "Old", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHacked(b, e).Sum());
                Console.WriteLine(fmt, "Hacked", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedOldHyperloopNoTail(b, e).Sum());
                Console.WriteLine(fmt, "OHLnoTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedOldHyperloopWithTail(b, e).Sum());
                Console.WriteLine(fmt, "OHLwTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => Enumerable.Range(b, e - b + 1).Sum());
                Console.WriteLine(fmt, "Range", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => RangeOldHyperloop(b, e).Sum());
                Console.WriteLine(fmt, "OHL Range", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => RangeHyperloop(b, e).Sum());
                Console.WriteLine(fmt, "HL Range", b, e, times, time.Item1, time.Item2);
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

        static Tuple<long, T> Benchmark<T>(int times, Func<T> func)
        {
            GC.Collect();
            var res = func(); // run once outside of loop to avoid initialization costs
            Stopwatch sw = Stopwatch.StartNew();
            while (--times >= 0)
                func();
            sw.Stop();
            return Tuple.Create(sw.ElapsedTicks, res);
        }

        static IEnumerable<int> FromToNestedStandart(int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            foreach(var v in FromToNestedStandart(b + 1, e))
                yield return v;
        }

        static IEnumerable<int> FromToNestedOldHyperloopNoTail(int b, int e)
        {
            var hl = new OldHyperloop<int>();
            hl.AddLoop(FromToNestedOldHyperloopNoTailLoop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedOldHyperloopNoTailLoop(int b, int e, IHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.AddLoop(FromToNestedOldHyperloopNoTailLoop(b + 1, e, hl).GetEnumerator());
        }

        static IEnumerable<int> FromToNestedOldHyperloopWithTail(int b, int e)
        {
            var hl = new OldHyperloop<int>();
            hl.AddLoop(FromToNestedOldHyperloopWithTailLoop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedOldHyperloopWithTailLoop(int b, int e, IHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.AddTail(FromToNestedOldHyperloopWithTailLoop(b + 1, e, hl).GetEnumerator());
        }

        static IEnumerable<int> RangeOldHyperloop(int b, int e)
        {
            var hl = new OldHyperloop<int>();
            hl.AddLoop(Enumerable.Range(b, e - b + 1).GetEnumerator());
            return hl;
        }

        static IEnumerable<int> RangeHyperloop(int b, int e)
        {
            return new Hyperloop<int>(Enumerable.Range(b, e - b + 1));
        }

        [IteratorStateMachine(typeof(FromToNestedHackedС))]
        static IEnumerable<int> FromToNestedHacked(int b, int e)
        {
            FromToNestedHackedС src = new FromToNestedHackedС(-2);
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
                            state = -3;
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
                state = -1;
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
    }

}
