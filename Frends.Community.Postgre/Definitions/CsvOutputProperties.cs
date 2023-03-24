using System;
using System.ComponentModel;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Csv output specific properties
    /// </summary>
    public class CsvOutputProperties
    {
        /// <summary>
        /// Include headers in csv output file?
        /// </summary>
        public bool IncludeHeaders { get; set; }

        /// <summary>
        /// Columns to include in the CSV output. Leave empty to include all columns in output.
        /// </summary>
        public string[] ColumnsToInclude { get; set; }

        /// <summary>
        /// What to use as field separators
        /// </summary>
        [DefaultValue(Enums.CsvFieldDelimiter.Semicolon)]
        public Enums.CsvFieldDelimiter FieldDelimiter { get; set; } = Enums.CsvFieldDelimiter.Semicolon;

        /// <summary>
        /// What to use as line breaks
        /// </summary>
        [DefaultValue(Enums.CsvLineBreak.CRLF)]
        public Enums.CsvLineBreak LineBreak { get; set; } = Enums.CsvLineBreak.CRLF;

        /// <summary>
        /// Whether to sanitize headers in output:
        /// - Strip any chars that are not 0-9, a-z or _
        /// - Make sure that column does not start with a number or underscore
        /// - Force lower case
        /// </summary>
        [DefaultValue(true)]
        public bool SanitizeColumnHeaders { get; set; } = true;

        /// <summary>
        /// Whether to add quotes around DATE and DATETIME fields
        /// </summary>
        [DefaultValue(true)]
        public bool AddQuotesToDates { get; set; } = true;

        /// <summary>
        /// Date format to use for formatting DATE columns, use .NET formatting tokens.
        /// Note that formatting is done using invariant culture.
        /// </summary>
        [DefaultValue("\"yyyy-MM-dd\"")]
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// Date format to use for formatting DATETIME columns, use .NET formatting tokens.
        /// Note that formatting is done using invariant culture.
        /// </summary>
        [DefaultValue("\"yyyy-MM-dd HH:mm:ss\"")]
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        internal string GetFieldDelimiterAsString()
        {
            switch (FieldDelimiter)
            {
                case Enums.CsvFieldDelimiter.Comma:
                    return ",";
                case Enums.CsvFieldDelimiter.Pipe:
                    return "|";
                case Enums.CsvFieldDelimiter.Semicolon:
                    return ";";
                default:
                    throw new Exception($"Unknown field delimiter: {FieldDelimiter}");
            }
        }
        internal string GetLineBreakAsString()
        {
            switch (LineBreak)
            {
                case Enums.CsvLineBreak.CRLF:
                    return "\r\n";
                case Enums.CsvLineBreak.CR:
                    return "\r";
                case Enums.CsvLineBreak.LF:
                    return "\n";
                default:
                    throw new Exception($"Unknown field delimiter: {FieldDelimiter}");
            }
        }
    }
}
