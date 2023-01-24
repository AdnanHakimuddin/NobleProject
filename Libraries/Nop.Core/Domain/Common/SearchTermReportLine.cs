namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Search term record (for statistics)
    /// </summary>
    public class SearchTermReportLine
    {
        /// <summary>
        /// Gets or sets the keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets search count
        /// </summary>
        public int Count { get; set; }

        public int Year { get; set; }

        public int Make { get; set; }

        public int Model { get; set; }

        public int Engine { get; set; }
    }
}
