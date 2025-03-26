using System;

namespace ScientificTheoryAnalyzer
{
    /// <summary>
    /// Represents a scientific theory article with basic properties
    /// </summary>
    public class ScienceTheoryArticle : IComparable<ScienceTheoryArticle?>
    {
        // Use nullable reference types and provide default values
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        public double Value { get; set; }

        /// <summary>
        /// Compares articles first by title, then by publication date
        /// </summary>
        public int CompareTo(ScienceTheoryArticle? other)
        {
            // Handle null comparison
            if (other is null) return 1;

            // First, compare by title
            int titleComparison = string.Compare(this.Title, other.Title, StringComparison.Ordinal);
            if (titleComparison != 0) return titleComparison;

            // If titles are the same, compare by publication date
            return this.PublicationDate.CompareTo(other.PublicationDate);
        }

        public override string ToString()
        {
            return $"Title: {Title}, Author: {Author}, Publication Date: {PublicationDate}, Value: {Value}";
        }

        // Optional: Constructor to ensure non-null values
        public ScienceTheoryArticle()
        {
            // Default constructor with empty strings
        }

        // Optional: Parameterized constructor
        public ScienceTheoryArticle(string title, string description, string author, 
            DateTime? publicationDate = null, double value = 0)
        {
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
            Author = author ?? string.Empty;
            PublicationDate = publicationDate ?? DateTime.Now;
            Value = value;
        }
    }
}