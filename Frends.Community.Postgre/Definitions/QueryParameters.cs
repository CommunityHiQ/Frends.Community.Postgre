using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Frends.Community.Postgre.Tests")]
namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Query Postgre
    /// </summary>
    [DisplayName("Query")]
    public class QueryParameters
    {
        /// <summary>
        /// Query.
        /// Note: Normal query requires double quotes around Column and 2 single quotes around Value.
        /// Query with params Example: SELECT * FROM table WHERE \"Column\" = '||:Value||' 
        /// </summary>
        [DisplayFormat(DataFormatString = "Sql")]
        [DefaultValue("SELECT * FROM table WHERE \"Column\" = ''Value''")]
        public string Query { get; set; }
        /// <summary>
        /// Query parameters
        /// </summary>
        [DefaultValue(null)]
        public Parameter[] Parameters { get; set; }
    }
}
