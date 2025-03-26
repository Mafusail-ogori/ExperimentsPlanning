using System;
using System.Collections.Generic;

namespace ScientificTheoryAnalyzer
{
    /// <summary>
    /// Utility class for generating sample scientific theory article data
    /// </summary>
    public static class DataGenerator
    {
        /// <summary>
        /// Generates a list of sample scientific theory articles
        /// </summary>
        /// <param name="count">Number of articles to generate</param>
        /// <returns>List of generated ScienceTheoryArticle objects</returns>
        public static List<ScienceTheoryArticle> GenerateSampleData(int count)
        {
            var random = new Random();
            var articles = new List<ScienceTheoryArticle>();

            for (int i = 0; i < count; i++)
            {
                articles.Add(new ScienceTheoryArticle
                {
                    Title = $"Theory {i + 1}",
                    Description = $"Description for Theory {i + 1}",
                    Author = $"Author {random.Next(1, 11)}",
                    PublicationDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                    Value = random.NextDouble() * 100
                });
            }

            return articles;
        }
    }
}