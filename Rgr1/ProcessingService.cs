using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScientificTheoryAnalyzer
{
    /// <summary>
    /// Service for processing scientific theory articles
    /// </summary>
    public class ProcessingService
    {
        /// <summary>
        /// Processes articles sequentially
        /// </summary>
        /// <param name="articles">List of articles to process</param>
        /// <returns>Processed list of articles</returns>
        public List<ScienceTheoryArticle> ProcessSequential(List<ScienceTheoryArticle> articles)
        {
            // Sort the articles
            articles.Sort();

            // Simulate processing for each article
            foreach (var article in articles)
            {
                ProcessArticle(article);
            }

            return articles;
        }

        /// <summary>
        /// Processes articles in parallel
        /// </summary>
        /// <param name="articles">List of articles to process</param>
        /// <param name="threadCount">Number of threads to use</param>
        /// <returns>Processed list of articles</returns>
        public List<ScienceTheoryArticle> ProcessParallel(List<ScienceTheoryArticle> articles, int threadCount)
        {
            // Sort the articles
            articles.Sort();

            // Parallel processing
            Parallel.ForEach(articles, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, ProcessArticle);

            return articles;
        }

        /// <summary>
        /// Simulates processing for a single article
        /// </summary>
        /// <param name="article">Article to process</param>
        private void ProcessArticle(ScienceTheoryArticle article)
        {
            // Simulate some computational work
            double result = 0;
            for (int i = 0; i < 10000; i++)
            {
                result += Math.Sqrt(article.Value);
            }
            article.Value = result;
        }
    }
}