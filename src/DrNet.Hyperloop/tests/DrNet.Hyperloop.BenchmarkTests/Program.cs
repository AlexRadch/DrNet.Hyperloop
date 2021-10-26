using System;
using BenchmarkDotNet.Running;

namespace DrNet.Hyperloop.BenchmarkTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hyperloopSummary = BenchmarkRunner.Run<HyperloopBenchmarks>();
        }
    }
}
