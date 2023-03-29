namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Return object
    /// </summary>
    public class QueryResult : Result
    {
        /// <summary>
        /// Content returned from query.
        /// </summary>
        public string Output { get; set; }
    }
}
