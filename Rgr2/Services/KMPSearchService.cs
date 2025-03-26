using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using KMPStringSearchProject.Utilities;

namespace KMPStringSearchProject.Services
{
    public static class KMPSearchService
    {
        // Sequential KMP Search (unchanged)
        public static ConcurrentBag<int> SequentialSearch(string text, string pattern)
        {
            var results = new ConcurrentBag<int>();
            int[] prefix = PrefixFunctionHelper.ComputePrefixFunction(pattern);

            int j = 0; // Pattern index
            int i = 0; // Text index

            while (i < text.Length)
            {
                if (pattern[j] == text[i])
                {
                    i++;
                    j++;
                }

                if (j == pattern.Length)
                {
                    results.Add(i - j);
                    j = prefix[j - 1];
                }
                else if (i < text.Length && pattern[j] != text[i])
                {
                    if (j != 0)
                    {
                        j = prefix[j - 1];
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return results;
        }

        // Improved Parallel KMP Search
        public static ConcurrentBag<int> ParallelSearch(string text, string pattern, int threadCount)
        {
            var results = new ConcurrentBag<int>();
            
            // Ensure thread count is valid
            threadCount = Math.Max(1, Math.Min(threadCount, Environment.ProcessorCount));
            
            int textLength = text.Length;
            int segmentLength = Math.Max(1, textLength / threadCount);

            Parallel.For(0, threadCount, threadIndex =>
            {
                // Calculate safe start and end indices
                int start = Math.Max(0, threadIndex * segmentLength - pattern.Length + 1);
                int end = Math.Min(textLength, (threadIndex + 1) * segmentLength + pattern.Length - 1);

                // Ensure we don't go out of bounds
                if (start < 0) start = 0;
                if (end > textLength) end = textLength;

                // Ensure valid segment length
                if (end - start > 0)
                {
                    string segment = text.Substring(start, end - start);
                    var segmentResults = SequentialSearch(segment, pattern);

                    foreach (var result in segmentResults)
                    {
                        results.Add(result + start);
                    }
                }
            });

            return results;
        }
    }
}