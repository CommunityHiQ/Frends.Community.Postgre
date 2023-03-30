using Newtonsoft.Json;
using Npgsql;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Postgre.Definitions
{
    internal static class JsonFileWriter
    {
        public static async Task<int> ToJsonFileAsync(NpgsqlDataReader reader, SaveQueryToFileProperties output, Encoding encoding, CancellationToken cancellationToken)
        {
            var rows = 0;
            var culture = string.IsNullOrWhiteSpace(output.JsonOptions.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(output.JsonOptions.CultureInfo);
            using (var fileWriter = new StreamWriter(output.Path, output.Append, encoding))
            using (var writer = new JsonTextWriter(fileWriter))
            {
                writer.Formatting = Formatting.Indented;
                writer.Culture = culture;

                await writer.WriteStartArrayAsync(cancellationToken);

                while (await reader.ReadAsync())
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
                    rows++;
                }

                // end array
                await writer.WriteEndArrayAsync(cancellationToken);
            }
            return rows;
        }
    }
}
