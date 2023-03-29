using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class FilteringIdsBenchmark
    {
        [Params(1000, 20_000, 50_000)]
        public int TestSize { get; set; }

        [Params(2, 4, 6)]
        public int TestCount { get; set; }

        public List<TestingModel>? Models { get; set; }

        public const int standardId = 1;

        [GlobalSetup]
        public void Setup()
        {
            Models = new List<TestingModel>();

            var ids = Enumerable.Range(0, TestSize).ToList();

            for (int i = 1; i <= TestCount; i++)
            {
                foreach (var id in ids)
                {
                    var model = new TestingModel
                    {
                        CId = i,
                        EId = id
                    };

                    Models.Add(model);
                }
            }
        }

        [Benchmark]
        public List<long> Where_Speed()
        {
            var notStandard = Models!.Where(x => x.CId != standardId);
            var standard = Models!.Where(x => x.CId == standardId);

            var result = notStandard.Select(x => x.EId)
                .Intersect(standard.Select(x => x.EId));

            return result.ToList();
        }

        [Benchmark]
        public List<long> Dictionary_Speed()
        {
            var ids = Enumerable.Range(standardId, TestCount).ToList();

            var dict = Models!
                .GroupBy(x => x.CId)
                .ToDictionary(x => x.Key, x => x.Select(x => x.EId).ToList());

            var standardList = dict.GetValueOrDefault(standardId);

            var notStandard = new List<long>();

            ids.Remove(standardId);

            foreach (var id in ids)
            {
                if (dict.TryGetValue(id, out var EIds))
                {
                    notStandard.AddRange(EIds);
                }
            }

            var result = notStandard.Intersect(standardList!).ToList();

            return result.ToList();
        }

        [Benchmark]
        public HashSet<long> Dictionary_EGroup_Speed()
        {
            var result = Models!
                .GroupBy(x => x.EId)
                .Where(x => !x.Select(g => g.CId).Contains(standardId))
                .Select(x => x.Key);

            var hashSet = new HashSet<long>(result);

            return hashSet;
        }


        [Benchmark]
        public List<long> Dictionary_Optimized_Intersect_Speed()
        {
            var dict = Models!
                .GroupBy(x => x.CId)
                .ToDictionary(x => x.Key, x => x.Select(x => x.EId).ToList());

            var standard = dict.GetValueOrDefault(standardId);

            dict.Remove(standardId);

            var nonStandardList = dict.Values.SelectMany(x => x).ToList();

            var result = nonStandardList.Intersect(standard!).ToList();

            return result.ToList();
        }

        [Benchmark]
        public List<long> Dictionary_Optimized_Speed()
        {
            var dict = Models!
                .GroupBy(x => x.CId)
                .ToDictionary(x => x.Key, x => x.Select(x => x.EId).ToList());

            dict.Remove(standardId);

            var nonStandard = dict.Values.SelectMany(x => x).ToList();

            return nonStandard.ToList();
        }
    }
}
