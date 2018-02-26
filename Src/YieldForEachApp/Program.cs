using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace YieldForEachApp
{
    public class Program
    {
        static void Main(/*string[] args*/)
        {
            var b = 1;
            var end = 1024;
            var times = 100;

            var bLen = $"{b:N0}".Length;
            var endLen = $"{end:N0}".Length;
            var timesLen = $"{times:N0}".Length;

            var fmt = string.Format(CultureInfo.InvariantCulture, 
                "{{0,-8:N}} from {{1,{0}:N0}} to {{2,{1}:N0}}, {{3,{2}:N0}} times: {{4,9:N0}} ticks. Equal {{5,7:N0}}",
                bLen, endLen, timesLen);

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => Enumerable.Range(b, e1 - b + 1).Sum());
                Console.WriteLine(fmt, "Range", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => new Hyperloop<int>{{Enumerable.Range(b, e1 - b + 1)}}.Sum());
                Console.WriteLine(fmt, "Range HL", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end;  e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive1(b, e1).Sum());
                Console.WriteLine(fmt, "RR1", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive1_Hl(b, e1).Sum());
                Console.WriteLine(fmt, "RR1 HL", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end;  e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive2(b, e1).Sum());
                Console.WriteLine(fmt, "RR2", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive2_Hl(b, e1).Sum());
                Console.WriteLine(fmt, "RR2 HL", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end;  e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive31(b, e1).Sum());
                Console.WriteLine(fmt, "RR3", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => RangeRecursive31_Hl(b, e1).Sum());
                Console.WriteLine(fmt, "RR3 HL", b, e, times, time.Item1, time.Item2);
            }

            Console.ReadLine();
        }

        //static long Benchmark(int times, Action act)
        //{
        //    GC.Collect();
        //    act(); // run once outside of loop to avoid initialization costs
        //    Stopwatch sw = Stopwatch.StartNew();
        //    while (--times >= 0)
        //        act();
        //    sw.Stop();
        //    return sw.ElapsedTicks;
        //}

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

        static IEnumerable<int> RangeRecursive1(int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            foreach(var v in RangeRecursive1(b + 1, e))
                yield return v;
        }

        static IEnumerable<int> RangeRecursive1_Hl(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.Add(RangeRecursive1_HlImp(hl, b, e));
            return hl;
        }

        static IEnumerable<int> RangeRecursive1_HlImp(IHyperloop<int> hl, int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.Add(RangeRecursive1_HlImp(hl, b + 1, e));
        }

        static IEnumerable<int> RangeRecursive2(int b, int e)
        {
            if (b > e)
                yield break;
            foreach(var v in RangeRecursive2(b, e - 1))
                yield return v;
            yield return e;
        }

        static IEnumerable<int> RangeRecursive2_Hl(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.Add(RangeRecursive2_HlImp(hl, b, e));
            return hl;
        }

        static IEnumerable<int> RangeRecursive2_HlImp(IHyperloop<int> hl, int b, int e)
        {
            if (b > e)
                yield break;
            hl.Add(RangeRecursive2_HlImp(hl, b, e - 1));
            yield return e;
        }

        static IEnumerable<int> RangeRecursive31(int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            foreach(var v in RangeRecursive32(b + 1, e))
                yield return v;
        }

        static IEnumerable<int> RangeRecursive32(int b, int e)
        {
            if (b > e)
                yield break;
            foreach(var v in RangeRecursive31(b, e - 1))
                yield return v;
            yield return e;
        }

        static IEnumerable<int> RangeRecursive31_Hl(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.Add(RangeRecursive31_HlImp(hl, b, e));
            return hl;
        }

        static IEnumerable<int> RangeRecursive31_HlImp(IHyperloop<int> hl, int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            hl.Add(RangeRecursive32_HlImp(hl, b + 1, e));
        }

        static IEnumerable<int> RangeRecursive32_Hl(int b, int e)
        {
            var hl = new Hyperloop<int>();
            hl.Add(RangeRecursive32_HlImp(hl, b, e));
            return hl;
        }

        static IEnumerable<int> RangeRecursive32_HlImp(IHyperloop<int> hl, int b, int e)
        {
            if (b > e)
                yield break;
            hl.Add(RangeRecursive31_HlImp(hl, b, e - 1));
            yield return e;
        }

    }

}
