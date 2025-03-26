using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace KMPStringSearchProject.Utilities
{
    public static class TextGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a random text with optional pattern embedding
        /// </summary>
        /// <param name="length">Total length of generated text</param>
        /// <param name="pattern">Optional pattern to embed multiple times</param>
        /// <param name="patternEmbedCount">Number of times to embed the pattern</param>
        /// <returns>Generated text</returns>
        public static string GenerateRandomText(
            int length, 
            string? pattern = null, 
            int patternEmbedCount = 0)
        {
            // Character set to generate random text
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
            var textBuilder = new StringBuilder(length);
            
            // If no pattern provided, generate completely random text
            if (string.IsNullOrEmpty(pattern) || patternEmbedCount <= 0)
            {
                for (int i = 0; i < length; i++)
                {
                    textBuilder.Append(characters[_random.Next(characters.Length)]);
                }
                return textBuilder.ToString();
            }

            // Ensure pattern is not longer than text
            if (pattern.Length > length)
            {
                throw new ArgumentException("Pattern length cannot exceed text length");
            }
            
            // Track used indices to avoid overlap
            var patternIndices = new HashSet<int>();
            
            for (int i = 0; i < patternEmbedCount; i++)
            {
                int embedIndex;
                do
                {
                    embedIndex = _random.Next(0, length - pattern.Length + 1);
                } while (patternIndices.Contains(embedIndex) || 
                         patternIndices.Any(p => Math.Abs(p - embedIndex) < pattern.Length));
                
                patternIndices.Add(embedIndex);
            }
            
            // Sort indices to help with placement
            var sortedIndices = patternIndices.OrderBy(x => x).ToList();
            
            int currentIndex = 0;
            for (int i = 0; i < length; i++)
            {
                // Check if this is a pattern embedding point
                if (currentIndex < sortedIndices.Count && i == sortedIndices[currentIndex])
                {
                    textBuilder.Append(pattern);
                    i += pattern.Length - 1;
                    currentIndex++;
                }
                else
                {
                    // Fill with random characters
                    textBuilder.Append(characters[_random.Next(characters.Length)]);
                }
            }
            
            // Pad or trim to exact length
            if (textBuilder.Length > length)
                textBuilder.Remove(length, textBuilder.Length - length);
            else if (textBuilder.Length < length)
            {
                while (textBuilder.Length < length)
                    textBuilder.Append(characters[_random.Next(characters.Length)]);
            }
            
            return textBuilder.ToString();
        }

        /// <summary>
        /// Generate multiple test scenarios with varying text sizes and pattern embedding
        /// </summary>
        /// <returns>Array of test scenario tuples</returns>
        public static (string text, string pattern)[] GenerateTestScenarios()
        {
            return new[]
            {
                // Small scenario
                (GenerateRandomText(1000, "ABABCABAB", 5), "ABABCABAB"),
                
                // Medium scenario
                (GenerateRandomText(10000, "ALGORITHMSEARCH", 10), "ALGORITHMSEARCH"),
                
                // Large scenario
                (GenerateRandomText(100000, "PERFORMANCETEST", 20), "PERFORMANCETEST"),
                
                // Extremely large scenario
                (GenerateRandomText(1000000, "LARGESCALETEST", 50), "LARGESCALETEST")
            };
        }
    }
}