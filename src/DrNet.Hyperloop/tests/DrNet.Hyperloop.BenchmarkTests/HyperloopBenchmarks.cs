using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace DrNet.Hyperloop.BenchmarkTests
{
    [MemoryDiagnoser]
    public class HyperloopBenchmarks
    {
        [Params(0, 1, 10, 100, 1000)]
        public int RangeCount { get; set; }

        [Benchmark(Baseline = true)]
        public void BenchmarkRange()
        {
            foreach (var item in Enumerable.Range(0, RangeCount)) ;
        }

        [Benchmark]
        public void BenchmarkRecursiveRange()
        {
            foreach (var item in RecursiveRange(0, RangeCount)) ;
        }

        [Benchmark]
        public void BenchmarkRecursiveRangeHl()
        {
            foreach (var item in RecursiveRangeHl(0, RangeCount)) ;
        }

        [Benchmark]
        public void BenchmarkRecursiveRangeHl_TailOptimization()
        {
            foreach (var item in RecursiveRangeHl_TailOptimization(0, RangeCount)) ;
        }

        [Benchmark]
        public void BenchmarkRecursiveRangeHl_OrderOptimization()
        {
            foreach (var item in RecursiveRangeHl_OrderOptimization(0, RangeCount)) ;
        }

        [Benchmark]
        public void RecursiveRangeHl_TailOrderOptimization()
        {
            foreach (var item in RecursiveRangeHl_TailOrderOptimization(0, RangeCount)) ;
        }

        protected IEnumerable<int> RecursiveRange(int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                foreach (var value in RecursiveRange(start, 1))
                    yield return value;

                foreach (var value in RecursiveRange(start + 1, count - 1))
                    yield return value;
            }
            else
            {
                foreach (var value in RecursiveRange(start, count - 1))
                    yield return value;

                foreach (var value in RecursiveRange(start + count - 1, 1))
                    yield return value;
            }
        }

        protected Hyperloop<int> RecursiveRangeHl(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRangeHl_Impl(recursion, start, count));

        protected IEnumerable<int> RecursiveRangeHl_Impl(IHyperloopRecursion<int> recursion, int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                //foreach (var value in RecursiveRangeQ(start, 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_Impl(recursion, start, 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_Impl(recursion, start + 1, count - 1));
                yield return default; // will be skiped
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_Impl(recursion, start, count - 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_Impl(recursion, start + count - 1, 1));
                yield return default; // will be skiped
            }
        }

        protected Hyperloop<int> RecursiveRangeHl_TailOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRangeHl_TailOptimization_Impl(recursion, start, count));

        protected IEnumerable<int> RecursiveRangeHl_TailOptimization_Impl(IHyperloopRecursion<int> recursion, int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                //foreach (var value in RecursiveRangeQ(start, 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_TailOptimization_Impl(recursion, start, 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;
                recursion.AddTail(RecursiveRangeHl_TailOptimization_Impl(recursion, start + 1, count - 1));
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRangeHl_TailOptimization_Impl(recursion, start, count - 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;
                recursion.AddTail(RecursiveRangeHl_TailOptimization_Impl(recursion, start + count - 1, 1));
            }
        }

        protected Hyperloop<int> RecursiveRangeHl_OrderOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRangeHl_OrderOptimization_Impl(recursion, start, count));

        protected IEnumerable<int> RecursiveRangeHl_OrderOptimization_Impl(IHyperloopRecursion<int> recursion, int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                //foreach (var value in RecursiveRangeQ(start, 1))
                //    yield return value;

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;

                recursion.Add(RecursiveRangeHl_OrderOptimization_Impl(recursion, start + 1, count - 1));
                recursion.Add(RecursiveRangeHl_OrderOptimization_Impl(recursion, start, 1));
                yield return default; // will be skiped
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;

                recursion.Add(RecursiveRangeHl_OrderOptimization_Impl(recursion, start + count - 1, 1));
                recursion.Add(RecursiveRangeHl_OrderOptimization_Impl(recursion, start, count - 1));
                yield return default; // will be skiped
            }
        }

        protected Hyperloop<int> RecursiveRangeHl_TailOrderOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRangeHl_TailOrderOptimization_Impl(recursion, start, count));

        protected IEnumerable<int> RecursiveRangeHl_TailOrderOptimization_Impl(IHyperloopRecursion<int> recursion, int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                //foreach (var value in RecursiveRangeQ(start, 1))
                //    yield return value;

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;

                recursion.AddTail(RecursiveRangeHl_TailOrderOptimization_Impl(recursion, start + 1, count - 1));
                recursion.Add(RecursiveRangeHl_TailOrderOptimization_Impl(recursion, start, 1));
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;

                recursion.AddTail(RecursiveRangeHl_TailOrderOptimization_Impl(recursion, start + count - 1, 1));
                recursion.Add(RecursiveRangeHl_TailOrderOptimization_Impl(recursion, start, count - 1));
            }
        }
    }
}
