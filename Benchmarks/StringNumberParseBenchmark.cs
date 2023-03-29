using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class StringNumberParseBenchmark
    {
        [Params(10, 100, 500, 1000)]
        public int TestSize { get; set; }

        public List<string>? Models { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Models = new List<string>();

            var testNums = Enumerable
                .Range(0, TestSize / 2)
                .Select(x => x.ToString());

            var random = new Random(1337420);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            for (int i = 0; i < TestSize / 2; i++)
            {
                Models.Add(new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray()));
            }

            Models.AddRange(testNums);
        }

        [Benchmark]
        public long[] TryParseBench()
        {
            var result = Models!
            .Where(y => long.TryParse(y, out var _))
            .Select(y => Convert.ToInt64(y))
            .ToArray();

            return result;
        }
        
        [Benchmark]
        public long[] AllIsDigitBench()
        {
            var result = Models!
            .Where(x => x.All(char.IsDigit))
            .Select(y => Convert.ToInt64(y))
            .ToArray();

            return result;
        }
        
        [Benchmark]
        public long[] SelectTryParseBench()
        {
            var result = Models!
                .Select(s =>
                {
                    long.TryParse(s, out long result);
                    return result;
                }).ToArray();

            return result;
        }
    }
}
