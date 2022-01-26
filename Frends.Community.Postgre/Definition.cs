#pragma warning disable 1591

using System.ComponentModel;

namespace Frends.Community.Postgre
{
    /// <summary>
    /// Return type.
    /// </summary>
    public enum PostgreQueryReturnType { XMLString, JSONString }

    /// <summary>
    /// Parameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Field name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  Field value.
        /// </summary>
        public dynamic Value { get; set; }
    }
    /// <summary>
    /// Query Postgre.
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
        /// Query parameters.
        /// </summary>
        [DefaultValue(null)]
        public Parameter[] Parameters { get; set; }
        /// <summary>
        /// Return type.
        /// </summary>
        [DefaultValue(PostgreQueryReturnType.XMLString)]
        public PostgreQueryReturnType ReturnType { get; set; }
        /// <summary>
        /// Specify the culture info to be used when parsing result to JSON and to XML.
        /// If this is left empty InvariantCulture will be used.
        /// Use the Language Culture Name, like en-US.
        /// </summary>
        [DefaultValue(null)]
        public string CultureInfo { get; set; }
    }
    public class ConnectionInformation
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        [PasswordPropertyText(true)]
        [DefaultValue("\"Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;\"")]
        public string ConnectionString { get; set; }
        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        [DefaultValue(30)]
        public int TimeoutSeconds { get; set; }
    }
    /// <summary>
    /// Return object.
    /// </summary>
    public class Output
    {
        /// <summary>
        /// Request result.
        /// </summary>
        public string Result { get; set; }
    }
}
