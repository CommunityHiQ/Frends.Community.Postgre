using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Text;
using Npgsql;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Frends.Community.Postgre.Definitions
{
    internal static class Extensions
    {
        /// <summary>
        /// Write query results to csv string.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToXmlAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            using (var writer =  new StringWriter() as TextWriter)
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Async = true, Indent = true }))
                {
                    await xmlWriter.WriteStartDocumentAsync();
                    await xmlWriter.WriteStartElementAsync("", output.XmlOptions.RootElementName, "");

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        // single row element container
                        await xmlWriter.WriteStartElementAsync("", output.XmlOptions.RowElementName, "");

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            await xmlWriter.WriteElementStringAsync("", reader.GetName(i), "", reader.GetValue(i).ToString());
                        }

                        // close single row element container
                        await xmlWriter.WriteEndElementAsync();

                        // write only complete elements, but stop if process was terminated
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    await xmlWriter.WriteEndElementAsync();
                    await xmlWriter.WriteEndDocumentAsync();
                }

                return writer.ToString();
            }
        }

        /// <summary>
        /// Write query results to json string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                var culture = string.IsNullOrWhiteSpace(output.JsonOptions.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(output.JsonOptions.CultureInfo);

                // create json result
                using (var writer = new JTokenWriter() as JsonWriter)
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.Culture = culture;
                    
                    // start array
                    await writer.WriteStartArrayAsync(cancellationToken);

                    while (reader.Read())
                    {
                        // start row object
                        await writer.WriteStartObjectAsync(cancellationToken);

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            // add row element name
                            await writer.WritePropertyNameAsync(reader.GetName(i), cancellationToken);
                            await writer.WriteValueAsync(reader.GetValue(i) ?? string.Empty, cancellationToken);

                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await writer.WriteEndObjectAsync(cancellationToken);
                    }

                    // end array
                    await writer.WriteEndArrayAsync(cancellationToken);

                    return ((JTokenWriter)writer).Token.ToString();
                }
            }
        }

        /// <summary>
        /// Write query results to csv string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToCsvAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            using (var writer =  new StringWriter() as TextWriter)
            {
                await DataReaderToCsvString(reader, writer, output.CsvOptions, cancellationToken);
                return writer.ToString();
            }
        }

        public static async Task<Tuple<int, string>> WriteToFileAsync(this NpgsqlCommand command, SaveQueryToFileProperties output, CancellationToken cancellationToken)
        {
            var rows = 0;
            command.CommandType = CommandType.Text;
            var encoding = GetEncoding(output.Encoding, output.EncodingString, output.EnableBom);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken) as NpgsqlDataReader)
            {
                switch (output.ReturnType)
                {
                    case Enums.QueryReturnType.Csv:
                        rows = CsvFileWriter.ToCsvFile(reader, output, encoding, cancellationToken);
                        break;
                    case Enums.QueryReturnType.Xml:
                        rows = await XmlFileWriter.ToXmlFile(reader, output, encoding, cancellationToken);
                        break;
                    case Enums.QueryReturnType.Json:
                        rows = await JsonFileWriter.ToJsonFileAsync(reader, output, encoding, cancellationToken);
                        break;
                }
                return new Tuple<int, string>(rows, output.Path);
            }
        }

        internal static Encoding GetEncoding(Enums.EncodingOptions encoding, string encodingString, bool enableBom)
        {
            switch (encoding)
            {
                case Enums.EncodingOptions.UTF8:
                    return enableBom ? new UTF8Encoding(true) : new UTF8Encoding(false);
                case Enums.EncodingOptions.ASCII:
                    return new ASCIIEncoding();
                case Enums.EncodingOptions.ANSI:
                    return Encoding.Default;
                case Enums.EncodingOptions.WINDOWS1252:
                    return CodePagesEncodingProvider.Instance.GetEncoding("windows-1252");
                case Enums.EncodingOptions.Other:
                    return CodePagesEncodingProvider.Instance.GetEncoding(encodingString);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown Encoding type: '{encoding}'.");
            }
        }

        internal static async Task DataReaderToCsvString(NpgsqlDataReader reader, TextWriter writer, CsvOutputProperties options, CancellationToken cancellationToken)
        {
            // Write header and remember column indexes to include
            var columnIndexesToInclude = new List<int>();
            var fieldNames = new object[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var includeColumn =
                    options.ColumnsToInclude == null ||
                    options.ColumnsToInclude.Length == 0 ||
                    ((IList)options.ColumnsToInclude).Contains(columnName);

                if (includeColumn)
                {
                    if (options.IncludeHeaders)
                    {
                        var formattedHeader = FormatDbHeader(columnName, options.SanitizeColumnHeaders);
                        fieldNames[i] = formattedHeader;
                    }
                    columnIndexesToInclude.Add(i);
                }
            }

            await writer.WriteLineAsync(string.Join(options.GetFieldDelimiterAsString(), fieldNames));

            var fieldValues = new List<object>();
            while (await reader.ReadAsync(cancellationToken))
            {
                foreach (var columnIndex in columnIndexesToInclude)
                {
                    var dbType = reader.GetFieldType(columnIndex);
                    var dbTypeName = reader.GetDataTypeName(columnIndex);
                    var value = reader.GetValue(columnIndex);
                    var formattedValue = FormatDbValue(value, dbTypeName, dbType, options);
                    fieldValues.Add(formattedValue);
                }
            }
            await writer.WriteLineAsync(string.Join(options.GetFieldDelimiterAsString(), fieldValues));
        }

        internal static string FormatDbHeader(string header, bool forceSpecialFormatting)
        {
            if (!forceSpecialFormatting) return header;

            // First part of regex removes all non-alphanumeric ('_' also allowed) chars from the whole string
            // Second part removed any leading numbers or underscores
            Regex rgx = new Regex("[^a-zA-Z0-9_-]|^[0-9_]+");
            header = rgx.Replace(header, "");
            return header.ToLower();
        }

        internal static string FormatDbValue(object value, string dbTypeName, Type dbType, CsvOutputProperties options)
        {
            if (value == null || value == DBNull.Value)
            {
                if (options.RemoveQuotesFromColumns) return string.Empty;
                if (dbType == typeof(string) || (dbType == typeof(DateTime) && options.AddQuotesToDates)) return "\"\"";
                return "";
            }

            if (dbType == typeof(string))
            {
                var str = (string)value;
                str = str.Replace("\"", "\\\"");
                str = str.Replace("\r\n", " ");
                str = str.Replace("\r", " ");
                str = str.Replace("\n", " ");
                if (options.RemoveQuotesFromColumns) return str;
                return $"\"{str}\"";
            }

            if (dbType == typeof(DateTime))
            {
                var dateTime = (DateTime)value;
                string output;
                switch (dbTypeName?.ToLower())
                {
                    case "date":
                        output = dateTime.ToString(options.DateFormat, CultureInfo.InvariantCulture);
                        break;
                    default:
                        output = dateTime.ToString(options.DateTimeFormat, CultureInfo.InvariantCulture);
                        break;
                }

                if (options.AddQuotesToDates) return $"\"{output}\"";
                return output;
            }

            if (dbType == typeof(float))
            {
                var floatValue = (float)value;
                return floatValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            if (dbType == typeof(double))
            {
                var doubleValue = (double)value;
                return doubleValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            if (dbType == typeof(decimal))
            {
                var decimalValue = (decimal)value;
                return decimalValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }
    }
}