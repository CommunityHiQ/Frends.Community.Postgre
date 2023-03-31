using Npgsql;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Frends.Community.Postgre.Definitions
{
    internal static class XmlFileWriter
    {
        public static async Task<int> ToXmlFile(NpgsqlDataReader reader, SaveQueryToFileProperties output, Encoding encoding, CancellationToken cancellationToken)
        {
            var rows = 0;
            using (var writer = new StreamWriter(output.Path, output.Append, encoding))
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
                    rows++;

                    // write only complete elements, but stop if process was terminated
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndDocumentAsync();
            }

            return rows;
        }
    }
}
