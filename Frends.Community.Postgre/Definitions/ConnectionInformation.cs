using System.ComponentModel;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Connection parameters
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
}
