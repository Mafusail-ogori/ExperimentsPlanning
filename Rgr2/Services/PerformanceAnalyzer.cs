using System;
using System.Diagnostics;
using KMPStringSearchProject.Services;

namespace KMPStringSearchProject.Services
{
    public static class PerformanceAnalyzer
    {
        public static void MeasurePerformance(string text, string pattern)
        {
            int[] threadCounts = { 1, 2, 4, 8, 16 };
            Console.WriteLine("Performance Measurements:\n");

            foreach (int threadCount in threadCounts)
            {
                // Sequential Performance
                var sequentialWatch = Stopwatch.StartNew();
                var sequentialResults = KMPSearchService.SequentialSearch(text, pattern);
                sequentialWatch.Stop();

                // Parallel Performance
                var parallelWatch = Stopwatch.StartNew();
                var parallelResults = KMPSearchService.ParallelSearch(text, pattern, threadCount);
                parallelWatch.Stop();

                Console.WriteLine($"Threads: {threadCount}");
                Console.WriteLine($"Sequential Time: {sequentialWatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Parallel Time: {parallelWatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Speedup: {(double)sequentialWatch.ElapsedMilliseconds / parallelWatch.ElapsedMilliseconds:F2}x");
                Console.WriteLine($"Sequential Matches: {sequentialResults.Count}");
                Console.WriteLine($"Parallel Matches: {parallelResults.Count}\n");
            }
        }
    }
}