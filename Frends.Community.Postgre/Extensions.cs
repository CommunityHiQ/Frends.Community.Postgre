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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace Frends.Community.Postgre
{
    static class Extensions
    {
        /// <summary>
        /// Write query results to csv string or file
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToXmlAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            // utf-8 as default encoding
            var encoding = string.IsNullOrWhiteSpace(output.OutputFile?.Encoding) ? Encoding.UTF8 : Encoding.GetEncoding(output.OutputFile.Encoding);

            using (var writer = output.OutputToFile ? new StreamWriter(output.OutputFile.Path, false, encoding) : new StringWriter() as TextWriter)
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Async = true, Indent = true }))
                {
                    await xmlWriter.WriteStartDocumentAsync();
                    await xmlWriter.WriteStartElementAsync("", output.XmlOutput.RootElementName, "");

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        // single row element container
                        await xmlWriter.WriteStartElementAsync("", output.XmlOutput.RowElementName, "");

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

                return output.OutputToFile ? output.OutputFile.Path : writer.ToString();
            }
        }

        /// <summary>
        /// Write query results to json string or file
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                var culture = string.IsNullOrWhiteSpace(output.JsonOutput.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(output.JsonOutput.CultureInfo);

                // utf-8 as default encoding
                var encoding = string.IsNullOrWhiteSpace(output.OutputFile?.Encoding) ? Encoding.UTF8 : Encoding.GetEncoding(output.OutputFile.Encoding);

                // create json result
                using (var fileWriter = output.OutputToFile ? new StreamWriter(output.OutputFile.Path, false, encoding) : null)
                using (var writer = output.OutputToFile ? new JsonTextWriter(fileWriter) : new JTokenWriter() as JsonWriter)
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.Culture = culture;

                    // start array
                    await writer.WriteStartArrayAsync(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

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

                        await writer.WriteEndObjectAsync(cancellationToken); // end row object

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    // end array
                    await writer.WriteEndArrayAsync(cancellationToken);

                    return output.OutputToFile ? output.OutputFile.Path : ((JTokenWriter)writer).Token.ToString();
                }
            }
        }

        /// <summary>
        /// Write query results to csv string or file
        /// </summary>
        /// <param name="command"></param>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ToCsvAsync(this NpgsqlCommand command, OutputProperties output, CancellationToken cancellationToken)
        {
            // utf-8 as default encoding
            var encoding = string.IsNullOrWhiteSpace(output.OutputFile?.Encoding) ? Encoding.UTF8 : Encoding.GetEncoding(output.OutputFile.Encoding);

            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            using (var w = output.OutputToFile ? new StreamWriter(output.OutputFile.Path, false, encoding) : new StringWriter() as TextWriter)
            {
                bool headerWritten = false;

                while (await reader.ReadAsync(cancellationToken))
                {
                    // write csv header if necessary
                    if (!headerWritten && output.CsvOutput.IncludeHeaders)
                    {
                        var fieldNames = new object[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fieldNames[i] = reader.GetName(i);
                        }
                        await w.WriteLineAsync(string.Join(output.CsvOutput.CsvSeparator, fieldNames));
                        headerWritten = true;
                    }

                    var fieldValues = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        fieldValues[i] = reader.GetValue(i);
                    }
                    await w.WriteLineAsync(string.Join(output.CsvOutput.CsvSeparator, fieldValues));

                    // write only complete rows, but stop if process was terminated
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return output.OutputToFile ? output.OutputFile.Path : w.ToString();
            }
        }
        #region QueryToFileTask

        internal static async Task<int> ToCsvFileAsync(this NpgsqlCommand command, SaveQueryToCsvOptions output, CancellationToken cancellationToken)
        {
            int result;
            command.CommandType = CommandType.Text;

            using (var reader = await command.ExecuteReaderAsync(cancellationToken) as NpgsqlDataReader)
            using (var writer = new StreamWriter(output.OutputFilePath))
            using (var csvFile = CreateCsvWriter(output.GetFieldDelimiterAsString(), writer))
            {
                writer.NewLine = output.GetLineBreakAsString();

                result = DataReaderToCsv(reader, csvFile, output, cancellationToken);

                csvFile.Flush();
            }
            return result;
        }

        internal static CsvWriter CreateCsvWriter(string delimiter, TextWriter writer)
        {
            var csvOptions = new Configuration
            {
                Delimiter = delimiter,
            };
            return new CsvWriter(writer, csvOptions);
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

        internal static string FormatDbValue(object value, string dbTypeName, Type dbType, SaveQueryToCsvOptions options)
        {
            if (value == null || value == DBNull.Value)
            {
                if (dbType == typeof(string)) return "\"\"";
                if (dbType == typeof(DateTime) && options.AddQuotesToDates) return "\"\"";
                return "";
            }

            if (dbType == typeof(string))
            {
                var str = (string)value;
                str = str.Replace("\"", "\\\"");
                str = str.Replace("\r\n", " ");
                str = str.Replace("\r", " ");
                str = str.Replace("\n", " ");
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

        internal static int DataReaderToCsv(NpgsqlDataReader reader, CsvWriter csvWriter, SaveQueryToCsvOptions options, CancellationToken cancellationToken)
        {
            // Write header and remember column indexes to include
            var columnIndexesToInclude = new List<int>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var includeColumn =
                    options.ColumnsToInclude == null ||
                    options.ColumnsToInclude.Length == 0 ||
                    ((IList)options.ColumnsToInclude).Contains(columnName);

                if (includeColumn)
                {
                    if (options.IncludeHeadersInOutput)
                    {
                        var formattedHeader = FormatDbHeader(columnName, options.SanitizeColumnHeaders);
                        csvWriter.WriteField(formattedHeader);
                    }
                    columnIndexesToInclude.Add(i);
                }
            }

            if (options.IncludeHeadersInOutput) csvWriter.NextRecord();

            var count = 0;
            while (reader.Read())
            {
                foreach (var columnIndex in columnIndexesToInclude)
                {
                    var dbType = reader.GetFieldType(columnIndex);
                    var dbTypeName = reader.GetDataTypeName(columnIndex);
                    var value = reader.GetValue(columnIndex);
                    var formattedValue = FormatDbValue(value, dbTypeName, dbType, options);
 
                    csvWriter.WriteField(formattedValue, false);
                }
                csvWriter.NextRecord();
                count++;
            }

            return count;
        }

        #endregion
    }
}

