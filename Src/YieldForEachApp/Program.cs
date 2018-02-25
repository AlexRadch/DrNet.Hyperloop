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
                "{{0,10:N}} from {{1,{0}:N0}} to {{2,{1}:N0}}, {{3,{2}:N0}} times: {{4,9:N0}} ticks. Equal {{5,7:N0}}",
                bLen, endLen, timesLen);

            for (var e = 1; e <= end;  e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => FromToNestedStandart(b, e1).Sum());
                Console.WriteLine(fmt, "Standart", b, e, times, time.Item1, time.Item2);
            }

           for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => FromToNestedNewHyperloop(null, b, e1).Sum());
                Console.WriteLine(fmt, "HL", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => new Hyperloop<int>{{Enumerable.Range(b, e1 - b + 1)}}.Sum());
                Console.WriteLine(fmt, "HL Range", b, e, times, time.Item1, time.Item2);
            }

            for (var e = 1; e <= end; e *= 2)
            {
                var e1 = e;
                var time = Benchmark(times, () => Enumerable.Range(b, e1 - b + 1).Sum());
                Console.WriteLine(fmt, "Range", b, e, times, time.Item1, time.Item2);
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

        static IEnumerable<int> FromToNestedStandart(int b, int e)
        {
            if (b > e)
                yield break;
            yield return b;
            foreach(var v in FromToNestedStandart(b + 1, e))
                yield return v;
        }

        static IEnumerable<int> FromToNestedNewHyperloop(IHyperloop<int> hl, int b, int e)
        {
            hl = hl ?? new Hyperloop<int>();
            hl.Add(FromToNestedNewHyperloopImp(hl, b, e));
            return hl;
        }

        static IEnumerable<int> FromToNestedNewHyperloopImp(IHyperloop<int> hl, int b, int e)
        {
            if (hl == null) throw new ArgumentNullException(nameof(hl));
            if (b > e)
                yield break;
            yield return b;
            hl.Add(FromToNestedNewHyperloopImp(hl, b+1, e));
        }
    }

}
