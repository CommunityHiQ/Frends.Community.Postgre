using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Output parameters.
    /// </summary>
    public class OutputProperties
    {
        /// <summary>
        /// Return type of the query.
        /// </summary>
        [DefaultValue(Enums.QueryReturnType.Xml)]
        public Enums.QueryReturnType ReturnType { get; set; }

        /// <summary>
        /// Xml specific output properties.
        /// </summary>
        [UIHint(nameof(ReturnType), "", Enums.QueryReturnType.Xml)]
        public XmlOutputProperties XmlOptions { get; set; }

        /// <summary>
        /// Json specific output properties.
        /// </summary>
        [UIHint(nameof(ReturnType), "", Enums.QueryReturnType.Json)]
        public JsonOutputProperties JsonOptions { get; set; }

        /// <summary>
        /// Csv specific output properties.
        /// </summary>
        [UIHint(nameof(ReturnType), "", Enums.QueryReturnType.Csv)]
        public CsvOutputProperties CsvOptions { get; set; }
    }
}
