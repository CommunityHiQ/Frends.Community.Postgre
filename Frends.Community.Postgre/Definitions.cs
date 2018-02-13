using System.ComponentModel;
using Frends.Tasks.Attributes;

namespace Frends.Community.Postgre
{
    /// <summary>
    /// Return type
    /// </summary>
    public enum PostgreQueryReturnType { XMLString, JSONString }

    /// <summary>
    /// Parameter
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  Field value
        /// </summary>
        public dynamic Value { get; set; }
    }
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
        [DefaultValue("SELECT * FROM table WHERE \"Column\" = ''Value''")]
        public string Query { get; set; }
        /// <summary>
        /// Query parameters
        /// </summary>
        [DefaultValue(null)]
        public Parameter[] Parameters { get; set; }
        /// <summary>
        /// Return type
        /// </summary>
        [DefaultValue(PostgreQueryReturnType.XMLString)]
        public PostgreQueryReturnType ReturnType { get; set; }
        /// <summary>
        /// Specify the culture info to be used when parsing result to JSON and to XML. If this is left empty InvariantCulture will be used. List of cultures: https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx Use the Language Culture Name.
        /// </summary>
        [DefaultValue(null)]
        public string CultureInfo { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionInformation
    {
        /// <summary>
        /// Connection string
        /// </summary>
        [PasswordPropertyText(true)]
        [DefaultValue("\"Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;\"")]
        public string ConnectionString { get; set; }
        /// <summary>
        /// Timeout seconds
        /// </summary>
        [DefaultValue(30)]
        public int TimeoutSeconds { get; set; }
    }
    /// <summary>
    /// Return object
    /// </summary>
    public class Output
    {
        /// <summary>
        /// Request result
        /// </summary>
        public dynamic Result { get; set; }
    }
}
