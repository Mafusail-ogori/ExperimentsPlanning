using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ScientificTheoryAnalyzer
{
    /// <summary>
    /// Provides performance analysis for sequential and parallel processing
    /// </summary>
    public class PerformanceAnalyzer
    {
        private readonly ProcessingService _processingService;

        public PerformanceAnalyzer()
        {
            _processingService = new ProcessingService();
        }

        /// <summary>
        /// Measures and compares performance of sequential and parallel processing
        /// </summary>
        /// <param name="dataSizes">Array of different data sizes to test</param>
        /// <param name="threadCounts">Array of thread counts to test</param>
        public void MeasurePerformance(int[] dataSizes, int[] threadCounts)
        {
            Console.WriteLine("Performance Comparison: Sequential vs Parallel Processing");
            Console.WriteLine("{0,-15}{1,-20}{2,-20}{3,-20}", "Data Size", "Sequential Time (ms)", "Parallel Time (ms)", "Speedup");

            foreach (int dataSize in dataSizes)
            {
                // Generate original data
                var originalData = DataGenerator.GenerateSampleData(dataSize);

                // Measure sequential processing
                var sequentialData = new List<ScienceTheoryArticle>(originalData);
                var sequentialStopwatch = Stopwatch.StartNew();
                _processingService.ProcessSequential(sequentialData);
                sequentialStopwatch.Stop();

                // Measure parallel processing for different thread counts
                foreach (int threadCount in threadCounts)
                {
                    var parallelData = new List<ScienceTheoryArticle>(originalData);
                    var parallelStopwatch = Stopwatch.StartNew();
                    _processingService.ProcessParallel(parallelData, threadCount);
                    parallelStopwatch.Stop();

                    // Calculate speedup
                    double speedup = (double)sequentialStopwatch.ElapsedMilliseconds / parallelStopwatch.ElapsedMilliseconds;

                    Console.WriteLine("{0,-15}{1,-20}{2,-20}{3,-20:F2}", 
                        dataSize, 
                        sequentialStopwatch.ElapsedMilliseconds, 
                        parallelStopwatch.ElapsedMilliseconds, 
                        speedup);
                }

                Console.WriteLine(); // Empty line for readability
            }
        }
    }
}