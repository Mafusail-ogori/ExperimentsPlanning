using System;

namespace ScientificTheoryAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define data sizes and thread counts to test
            int[] dataSizes = { 1000, 10000, 100000, 1000000 };
            int[] threadCounts = { 1, 2, 4, 8 };

            // Create performance analyzer
            var performanceAnalyzer = new PerformanceAnalyzer();

            // Run performance measurements
            performanceAnalyzer.MeasurePerformance(dataSizes, threadCounts);
        }
    }
}