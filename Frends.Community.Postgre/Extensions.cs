using Frends.Community.Postgre.Definitions;
using System;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Text;
using Npgsql;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Frends.Community.Postgre
{
    internal static class Extensions
    {
        /// <summary>
        /// Write query results to csv string
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
                var culture = string.IsNullOrWhiteSpace(output.JsonOutput.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(output.JsonOutput.CultureInfo);

                // create json result
                using (var writer = new JTokenWriter() as JsonWriter)
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

                        await writer.WriteEndObjectAsync(cancellationToken);

                        cancellationToken.ThrowIfCancellationRequested();
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
                        await writer.WriteLineAsync(string.Join(output.CsvOutput.GetFieldDelimiterAsString(), fieldNames));
                        headerWritten = true;
                    }

                    var fieldValues = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        fieldValues[i] = reader.GetValue(i);
                    }
                    await writer.WriteLineAsync(string.Join(output.CsvOutput.GetFieldDelimiterAsString(), fieldValues));

                    // write only complete rows, but stop if process was terminated
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return writer.ToString();
            }
        }

        public static async Task<Tuple<int, string>> WriteToFileAsync(this NpgsqlCommand command, SaveQueryToFileProperties output, CancellationToken cancellationToken)
        {
            command.CommandType = CommandType.Text;
            var encoding = GetEncoding(output.Encoding, output.EncodingString, output.EnableBom);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken) as NpgsqlDataReader)
            {
                switch (output.ReturnType)
                {
                    case Enums.QueryReturnType.Csv:
                        CsvFileWriter.ToCsvFile(reader, output, encoding, cancellationToken);
                        break;
                    case Enums.QueryReturnType.Xml:
                        await XmlFileWriter.ToXmlFile(reader, output, encoding, cancellationToken);
                        break;
                    case Enums.QueryReturnType.Json:
                        await JsonFileWriter.ToJsonFileAsync(reader, output, encoding, cancellationToken);
                        break;
                }
                return new Tuple<int, string>(reader.RecordsAffected, output.Path);
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
    }
}