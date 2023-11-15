using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser(false)]
    public class SpreadBenchmark
    {
        [Params(10, 10_000, 1_000_000, 100_000_000)]
        public int TestSize { get; set; }

        public List<long>? Array1 { get; set; }
        public List<long>? Array2 { get; set; }
        public List<long>? Array3 { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Array1 = Enumerable.Range(0, TestSize).Select(x => (long)x).ToList();
            Array2 = Enumerable.Range(0, TestSize).Select(x => (long)x).ToList();
            Array3 = Enumerable.Range(0, TestSize).Select(x => (long)x).ToList();
        }

        [Benchmark]
        public List<long> Concat()
        {
            var res = Array1!.Concat(Array2!).Concat(Array3!).ToList();

            return res;
        }

        [Benchmark]
        public List<long> Spread()
        {
            List<long> res = [.. Array1, ..Array2, ..Array3];

            return res;
        }

    }
}
