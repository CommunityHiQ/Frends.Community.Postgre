using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Properties for when user wants to write the result directly into a file
    /// </summary>
    [DisplayName("OutputProperties")]
    public class SaveQueryToFileProperties
    {
        /// <summary>
        /// Query output filepath
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("c:\\temp\\output.csv")]
        public string Path { get; set; }

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

        ///<summary>
        /// Enables the task to append to the end of the file. If disabled the file is overwritten.
        /// </summary>
        [DefaultValue(false)]
        public bool Append { get; set; } = false;

        /// <summary>
        /// Output file encoding
        /// </summary>
        [DefaultValue(Enums.EncodingOptions.UTF8)]
        public Enums.EncodingOptions Encoding { get; set; }

        /// <summary>
        /// Manualy give encoding.
        /// </summary>
        [UIHint(nameof(Encoding), "", Enums.EncodingOptions.Other)]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string EncodingString { get; set; }

        /// <summary>
        /// Enable bom on utf-8 encoding
        /// </summary>
        [UIHint(nameof(Encoding), "", Enums.EncodingOptions.UTF8)]
        [DefaultValue(false)]
        public bool EnableBom { get; set; }
    }
}
