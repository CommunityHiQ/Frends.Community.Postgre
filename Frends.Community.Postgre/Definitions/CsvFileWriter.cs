using System.Text;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using System.IO;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;

namespace Frends.Community.Postgre.Definitions
{
    internal static class CsvFileWriter
    {
        public static int ToCsvFile(NpgsqlDataReader reader, SaveQueryToFileProperties output, Encoding encoding, CancellationToken cancellationToken)
        {
            var rows = 0;
            using (var writer = new StreamWriter(output.Path, output.Append, encoding))
            {
                var csvOptions = new Configuration
                {
                    Delimiter = output.CsvOptions.GetFieldDelimiterAsString(),
                };
                using (var csvFile = new CsvWriter(writer, csvOptions))
                {
                    writer.NewLine = output.CsvOptions.GetLineBreakAsString();
                    rows = DataReaderToCsvWriter(reader, csvFile, output.CsvOptions, cancellationToken);
                    csvFile.Flush();
                }
            }
            return rows;
        }

        internal static int DataReaderToCsvWriter(NpgsqlDataReader reader, CsvWriter csvWriter, CsvOutputProperties options, CancellationToken cancellationToken)
        {
            // Write header and remember column indexes to include
            var columnIndexesToInclude = new List<int>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var columnName = reader.GetName(i);
                var includeColumn =
                    options.ColumnsToInclude == null ||
                    options.ColumnsToInclude.Length == 0 ||
                    ((IList)options.ColumnsToInclude).Contains(columnName);

                if (includeColumn)
                {
                    if (options.IncludeHeaders)
                    {
                        var formattedHeader = Extensions.FormatDbHeader(columnName, options.SanitizeColumnHeaders);
                        csvWriter.WriteField(formattedHeader);
                    }
                    columnIndexesToInclude.Add(i);
                }
            }

            if (options.IncludeHeaders) csvWriter.NextRecord();

            var rows = 0;

            while (reader.Read())
            {
                foreach (var columnIndex in columnIndexesToInclude)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var dbType = reader.GetFieldType(columnIndex);
                    var dbTypeName = reader.GetDataTypeName(columnIndex);
                    var value = reader.GetValue(columnIndex);
                    var formattedValue = Extensions.FormatDbValue(value, dbTypeName, dbType, options);

                    csvWriter.WriteField(formattedValue, false);
                }
                csvWriter.NextRecord();
                rows++;
            }

            return rows;
        }
    }
}
