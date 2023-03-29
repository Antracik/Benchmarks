using BenchmarkDotNet.Running;

namespace Benchmarks
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<StringNumberParseBenchmark>();
        }
    }
}