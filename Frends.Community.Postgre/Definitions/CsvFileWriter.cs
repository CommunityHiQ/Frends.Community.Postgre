using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;

namespace Frends.Community.Postgre.Definitions
{
    internal static class CsvFileWriter
    {
        public static void ToCsvFile(NpgsqlDataReader reader, SaveQueryToFileProperties output, Encoding encoding, CancellationToken cancellationToken)
        {
            using (var writer = new StreamWriter(output.Path, output.Append, encoding))
            {
                var csvOptions = new Configuration
                {
                    Delimiter = output.CsvOptions.GetFieldDelimiterAsString(),
                };
                using (var csvFile = new CsvWriter(writer, csvOptions))
                {
                    writer.NewLine = output.CsvOptions.GetLineBreakAsString();
                    DataReaderToCsv(reader, csvFile, output.CsvOptions, cancellationToken);
                    csvFile.Flush();
                }
            }
        }

        internal static void DataReaderToCsv(NpgsqlDataReader reader, CsvWriter csvWriter, CsvOutputProperties options, CancellationToken cancellationToken)
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
                    if (options.IncludeHeaders)
                    {
                        var formattedHeader = FormatDbHeader(columnName, options.SanitizeColumnHeaders);
                        csvWriter.WriteField(formattedHeader);
                    }
                    columnIndexesToInclude.Add(i);
                }
            }

            if (options.IncludeHeaders) csvWriter.NextRecord();

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
            }
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
    }
}
