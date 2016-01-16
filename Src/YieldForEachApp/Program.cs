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
                Console.WriteLine(fmt, "Standart", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHacked(b, e).Sum());
                Console.WriteLine(fmt, "Hacked", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHyperloopNoTail(b, e).Sum());
                Console.WriteLine(fmt, "HLnoTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHyperloopWithTail(b, e).Sum());
                Console.WriteLine(fmt, "HLwTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHyperloopNoTail2(b, e).Sum());
                Console.WriteLine(fmt, "HLnoTail2", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedHyperloopWithTail2(b, e).Sum());
                Console.WriteLine(fmt, "HLwTail2", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedNewHyperloopNoTail(b, e).Sum());
                Console.WriteLine(fmt, "NHLnoTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => FromToNestedNewHyperloopWithTail(b, e).Sum());
                Console.WriteLine(fmt, "NHLwTail", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => RangeHyperloop(b, e).Sum());
                Console.WriteLine(fmt, "HL Range", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var time = Benchmark(times, () => Enumerable.Range(b, e - b + 1).Sum());
                Console.WriteLine(fmt, "Range", b, e, times, time.Item1, time.Item2);
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

        static IEnumerable<int> FromToNestedHyperloopNoTail(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopNoTailLoop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopNoTailLoop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.GetHyperloop().AddLoop(FromToNestedHyperloopNoTailLoop(b + 1, e, hl).GetEnumerator());
            yield return 100;
        }

        static IEnumerable<int> FromToNestedHyperloopWithTail(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopWithTailLoop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopWithTailLoop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.GetHyperloop().AddTail(FromToNestedHyperloopWithTailLoop(b + 1, e, hl).GetEnumerator());
        }

        static IEnumerable<int> FromToNestedHyperloopNoTail2(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopNoTail2Loop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopNoTail2Loop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl = hl.GetHyperloop();
            hl.AddLoop((FromToNestedHyperloopNoTail22(b + 1, e) as ILoopProvider<int>).GetLoop(hl));
            yield return 100;
        }

        static IEnumerable<int> FromToNestedHyperloopNoTail22(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopNoTail22Loop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopNoTail22Loop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl = hl.GetHyperloop();
            hl.AddLoop((FromToNestedHyperloopNoTail2(b + 1, e) as ILoopProvider<int>).GetLoop(hl));
            yield return 100;
        }

        static IEnumerable<int> FromToNestedHyperloopWithTail2(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopWithTail2Loop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopWithTail2Loop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl = hl.GetHyperloop();
            hl.AddTail((FromToNestedHyperloopWithTail22(b + 1, e) as ILoopProvider<int>).GetLoop(hl));
        }

        static IEnumerable<int> FromToNestedHyperloopWithTail22(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(FromToNestedHyperloopWithTail22Loop(b, e, hl).GetEnumerator());
            return hl;
        }

        private static IEnumerable<int> FromToNestedHyperloopWithTail22Loop(int b, int e, IOldHyperloop<int> hl)
        {
            if (b > e)
                yield break;
            yield return b;
            hl = hl.GetHyperloop();
            hl.AddLoop((FromToNestedHyperloopWithTail2(b + 1, e) as ILoopProvider<int>).GetLoop(hl));
        }

        static IEnumerable<int> RangeHyperloop(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.AddLoop(Enumerable.Range(b, e - b + 1).GetEnumerator());
            return hl;
        }

        [IteratorStateMachine(typeof(FromToNestedHackedС))]
        static IEnumerable<int> FromToNestedHacked(int b, int e)
        {
            var src = new FromToNestedHackedС(-2);
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

        [IteratorStateMachine(typeof(FromToNestedNewHyperloopNoTailEnumerator))]
        static IEnumerable<int> FromToNestedNewHyperloopNoTail(int b, int e)
        {
            var src = new FromToNestedNewHyperloopNoTailEnumerable();
            src._b = b;
            src._e = e;
            return src;
        }

        [CompilerGenerated]
        private sealed class FromToNestedNewHyperloopNoTailEnumerable : NewHyperloop<int>, ILoopProvider<int>, IEnumerable<int>, IEnumerable
        {
            public int _b;
            public int _e;

            [DebuggerHidden]
            public IEnumerator<int> GetLoop(IHyperloop<int> hyperloop) // can get control from other accessibility level
            {
                var src = new FromToNestedNewHyperloopNoTailEnumerator();
                src.hyperloop = hyperloop;
                src.b = _b;
                src.e = _e;
                return src;
            }

            [DebuggerHidden]
            public IEnumerator<int> GetEnumerator()
            {
                if (loops == null)
                {
                    AddLoop(GetLoop(this));
                    return this;
                }
                var src = new FromToNestedNewHyperloopNoTailEnumerable();
                src._b = _b;
                src._e = _e;
                return src.GetEnumerator();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }

        [CompilerGenerated]
        private sealed class FromToNestedNewHyperloopNoTailEnumerator : IEnumerator<int>, IEnumerator, IDisposable
        {
            private int state;
            private int current;
            public IHyperloop<int> hyperloop;
            public int b;
            public int e;

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
            void IDisposable.Dispose()
            {
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
                            var src = new FromToNestedNewHyperloopNoTailEnumerator();
                            src.hyperloop = hyperloop;
                            src.b = b + 1;
                            src.e = e;
                            hyperloop.AddLoop(src);
                            state = 2;
                            return false;
                        case 2:
                            state = -3;
                            break;
                        default:
                            return false;
                    }
                    return false;
                }
                catch
                {
                    ((IDisposable)this).Dispose();
                    throw;
                }
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }
        }

        [IteratorStateMachine(typeof(FromToNestedNewHyperloopWithTailEnumerator))]
        static IEnumerable<int> FromToNestedNewHyperloopWithTail(int b, int e)
        {
            var src = new FromToNestedNewHyperloopNoTailEnumerable();
            src._b = b;
            src._e = e;
            return src;
        }

        [CompilerGenerated]
        private sealed class FromToNestedHyperloopWithTailEnumerable : NewHyperloop<int>, ILoopProvider<int>, IEnumerable<int>, IEnumerable
        {
            public int _b;
            public int _e;

            [DebuggerHidden]
            public IEnumerator<int> GetLoop(IHyperloop<int> hyperloop) // can get control from other accessibility level
            {
                var src = new FromToNestedNewHyperloopWithTailEnumerator();
                src.hyperloop = hyperloop;
                src.b = _b;
                src.e = _e;
                return src;
            }

            [DebuggerHidden]
            public IEnumerator<int> GetEnumerator()
            {
                if (loops == null)
                {
                    AddLoop(GetLoop(this));
                    return this;
                }
                var src = new FromToNestedHyperloopWithTailEnumerable();
                src._b = _b;
                src._e = _e;
                return src.GetEnumerator();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }

        [CompilerGenerated]
        private sealed class FromToNestedNewHyperloopWithTailEnumerator : IEnumerator<int>, IEnumerator, IDisposable
        {
            private int state;
            private int current;
            public IHyperloop<int> hyperloop;
            public int b;
            public int e;

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
            void IDisposable.Dispose()
            {
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
                            var src = new FromToNestedNewHyperloopWithTailEnumerator();
                            src.hyperloop = hyperloop;
                            src.b = b + 1;
                            src.e = e;
                            hyperloop.AddTail(src);
                            state = -3;
                            break;
                        default:
                            return false;
                    }
                    return false;
                }
                catch
                {
                    ((IDisposable)this).Dispose();
                    throw;
                }
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }
        }
    }

}
