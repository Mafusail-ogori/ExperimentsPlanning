using System;
using KMPStringSearchProject.Services;
using KMPStringSearchProject.Utilities;

namespace KMPStringSearchProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get test scenarios
            var testScenarios = TextGenerator.GenerateTestScenarios();

            // Run tests for each scenario
            foreach (var (text, pattern) in testScenarios)
            {
                Console.WriteLine($"\n--- Test Scenario ---");
                Console.WriteLine($"Text Length: {text.Length}");
                Console.WriteLine($"Pattern: {pattern}");

                // Perform sequential search
                var sequentialWatch = System.Diagnostics.Stopwatch.StartNew();
                var sequentialResults = KMPSearchService.SequentialSearch(text, pattern);
                sequentialWatch.Stop();

                Console.WriteLine("\nSequential Search Results:");
                Console.WriteLine($"Matches Found: {sequentialResults.Count}");
                Console.WriteLine($"Sequential Search Time: {sequentialWatch.ElapsedMilliseconds} ms");

                // Print first few match indices (if any)
                int displayCount = Math.Min(sequentialResults.Count, 10);
                for (int i = 0; i < displayCount; i++)
                {
                    Console.WriteLine($"Pattern found at index: {sequentialResults.ToArray()[i]}");
                }

                // Performance analysis with various thread counts
                PerformanceAnalyzer.MeasurePerformance(text, pattern);
            }
        }
    }
}