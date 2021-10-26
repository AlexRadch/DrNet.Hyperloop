using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

#pragma warning disable IDE0008 // Use explicit type
namespace DrNet.Tests
{
    public class HyperloopTest
    {
        [Fact]
        public void TestConstructor()
        {
            {
                var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10));
                //Assert.NotNull(hl.EnumerableImplementation);
                //Assert.Null(hl.EnumeratorImplementation);
                //Assert.True(hl.Depth < 0);
            }

            {
                var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10).GetEnumerator());
                //Assert.Null(hl.EnumerableImplementation);
                //Assert.NotNull(hl.EnumeratorImplementation);
                //Assert.True(hl.Depth < 0);
            }

            {
                var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10), 10);
                //Assert.NotNull(hl.EnumerableImplementation);
                //Assert.Null(hl.EnumeratorImplementation);
                //Assert.Equal(10, hl.Depth);
            }

            {
                var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10).GetEnumerator(), 10);
                //Assert.Null(hl.EnumerableImplementation);
                //Assert.NotNull(hl.EnumeratorImplementation);
                //Assert.Equal(10, hl.Depth);
            }
        }

        [Fact]
        public void TestGetEnumerator()
        {
            var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10));

            var enumerator = hl.GetEnumerator();
            Assert.Equal(default, enumerator.Current);

            enumerator.Dispose();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public void TestRecursiveRangeQ()
        {
            Assert.Equal(Enumerable.Range(0, 10), RecursiveRangeQ(0, 10));
        }

        [Fact]
        public void TestHyperloop()
        {
            var hl = new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, 0, 10));

            Assert.Equal(Enumerable.Range(0, 10), hl);
        }

        [Fact]
        public void TestRecursiveRangeHl()
        {
            Assert.Equal(Enumerable.Range(0, 10), RecursiveRangeHl(0, 10));
        }

        [Fact]
        public void TestRecursiveRangeHl_TailOptimization()
        {
            Assert.Equal(Enumerable.Range(0, 10), RecursiveRangeHl_TailOptimization(0, 10));
        }

        [Fact]
        public void TestRecursiveRangeHl_OrderOptimization()
        {
            Assert.Equal(Enumerable.Range(0, 10), RecursiveRangeHl_OrderOptimization(0, 10));
        }

        [Fact]
        public void TestRecursiveRangeHl_TailOrderOptimization()
        {
            Assert.Equal(Enumerable.Range(0, 10), RecursiveRangeHl_TailOrderOptimization(0, 10));
        }

        protected IEnumerable<int> RecursiveRangeQ(int start, int count)
        {
            if (count <= 1)
            {
                if (count == 1)
                    yield return start;
                yield break;
            }

            if (count % 2 == 1)
            {
                foreach (var value in RecursiveRangeQ(start, 1))
                    yield return value;

                foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                    yield return value;
            }
            else
            {
                foreach (var value in RecursiveRangeQ(start, count - 1))
                    yield return value;

                foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                    yield return value;
            }
        }

        protected Hyperloop<int> RecursiveRangeHl(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRange_Impl(recursion, start, count));

        protected IEnumerable<int> RecursiveRange_Impl(HyperloopRecursions<int> recursion, int start, int count)
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
                recursion.Add(RecursiveRange_Impl(recursion, start, 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRange_Impl(recursion, start + 1, count - 1));
                yield return default; // will be skiped
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRange_Impl(recursion, start, count - 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;
                recursion.Add(RecursiveRange_Impl(recursion, start + count - 1, 1));
                yield return default; // will be skiped
            }
        }

        protected Hyperloop<int> RecursiveRangeHl_TailOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRange_Impl_TailOptimization(recursion, start, count));

        protected IEnumerable<int> RecursiveRange_Impl_TailOptimization(HyperloopRecursions<int> recursion, int start, int count)
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
                recursion.Add(RecursiveRange_Impl_TailOptimization(recursion, start, 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + 1, count - 1))
                //    yield return value;
                recursion.AddTail(RecursiveRange_Impl_TailOptimization(recursion, start + 1, count - 1));
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                recursion.Add(RecursiveRange_Impl_TailOptimization(recursion, start, count - 1));
                yield return default; // will be skiped

                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;
                recursion.AddTail(RecursiveRange_Impl_TailOptimization(recursion, start + count - 1, 1));
            }

            if (count % 3 != 0)
                yield break; // usual way

            yield return default; // will be skiped
            yield return 1; // will be ignored
            yield return 2; // will be ignored
            yield return 3; // will be ignored
        }

        protected Hyperloop<int> RecursiveRangeHl_OrderOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRange_Impl_OrderOptimization(recursion, start, count));

        protected IEnumerable<int> RecursiveRange_Impl_OrderOptimization(HyperloopRecursions<int> recursion, int start, int count)
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

                recursion.Add(RecursiveRange_Impl_OrderOptimization(recursion, start + 1, count - 1));
                recursion.Add(RecursiveRange_Impl_OrderOptimization(recursion, start, 1));
                yield return default; // will be skiped
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;

                recursion.Add(RecursiveRange_Impl_OrderOptimization(recursion, start + count - 1, 1));
                recursion.Add(RecursiveRange_Impl_OrderOptimization(recursion, start, count - 1));
                yield return default; // will be skiped
            }
        }

        protected Hyperloop<int> RecursiveRangeHl_TailOrderOptimization(int start, int count)
            => new Hyperloop<int>(recursion => RecursiveRange_Impl_TailOrderOptimization(recursion, start, count));

        protected IEnumerable<int> RecursiveRange_Impl_TailOrderOptimization(HyperloopRecursions<int> recursion, int start, int count)
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

                recursion.AddTail(RecursiveRange_Impl_TailOrderOptimization(recursion, start + 1, count - 1));
                recursion.Add(RecursiveRange_Impl_TailOrderOptimization(recursion, start, 1));
            }
            else
            {
                //foreach (var value in RecursiveRangeQ(start, count - 1))
                //    yield return value;
                //foreach (var value in RecursiveRangeQ(start + count - 1, 1))
                //    yield return value;

                recursion.AddTail(RecursiveRange_Impl_TailOrderOptimization(recursion, start + count - 1, 1));
                recursion.Add(RecursiveRange_Impl_TailOrderOptimization(recursion, start, count - 1));
            }

            if (count % 3 != 0)
                yield break; // usual way

            yield return default; // will be skiped
            yield return 1; // will be ignored
            yield return 2; // will be ignored
            yield return 3; // will be ignored
        }
    }
}
