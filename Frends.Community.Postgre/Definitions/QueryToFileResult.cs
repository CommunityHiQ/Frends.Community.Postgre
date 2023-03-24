namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Return object
    /// </summary>
    public class QueryToFileResult : Result
    {
        /// <summary>
        /// File path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Rows affected.
        /// </summary>
        public int Rows { get; set; }
    }
}
