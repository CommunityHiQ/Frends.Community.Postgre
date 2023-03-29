using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Xml output specific properties
    /// </summary>
    public class XmlOutputProperties
    {
        /// <summary>
        /// Xml root element name
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("ROWSET")]
        public string RootElementName { get; set; }

        /// <summary>
        /// Xml row element name
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("ROW")]
        public string RowElementName { get; set; }
    }
}
