using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser(false)]
    public class ContainsBenchmark
    {
        [Params(0, 10_000, 1_000_000)]
        public int TestSize { get; set; }


        public List<long>? Models { get; set; }


        [GlobalSetup]
        public void Setup()
        {
            Models = Enumerable.Range(0, TestSize).Select(x => (long)x).ToList();
        }



        [Benchmark]
        public bool AnyContains_Speed()
        {
            if (Models!.Any())
            {
                return Models!.Contains(1337);
            }

            return false;
        }

        [Benchmark]
        public bool Contains_Speed()
        {
            return Models!.Contains(1337);
        }

    }
}
