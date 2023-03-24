#pragma warning disable 1591

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Enumerations used in Task parameters.
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// Return type
        /// </summary>
        public enum QueryReturnType { Csv, Json, Xml }

        /// <summary>
        /// CSV field delimeter options
        /// </summary>
        public enum CsvFieldDelimiter
        {
            Comma,
            Semicolon,
            Pipe
        }

        /// <summary>
        /// CSV line break options
        /// </summary>
        public enum CsvLineBreak
        {
            CRLF,
            LF,
            CR
        }

        /// <summary>
        /// Encoding options
        /// </summary>
        public enum EncodingOptions
        {
            UTF8,
            ANSI,
            ASCII,
            WINDOWS1252,
            /// <summary>
            /// Other enables users to add other encoding options as string.
            /// </summary>
            Other
        }

    }
}
