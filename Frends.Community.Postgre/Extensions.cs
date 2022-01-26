using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Frends.Community.Postgre
{
    static class Extensions
    {
        public static string ToJson(this NpgsqlDataReader reader, string cultureInfo)
        {
            var culture = string.IsNullOrWhiteSpace(cultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(cultureInfo);

            // Create JSON result.
            using (var writer = new JTokenWriter())
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                writer.Culture = culture;

                // Start array.
                writer.WriteStartArray();

                while (reader.Read())
                {
                    // Start row object.
                    writer.WriteStartObject();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        // Add row element name.
                        writer.WritePropertyName(reader.GetName(i));

                        // Add row element value.
                        writer.WriteValue(reader.GetValue(i) ?? string.Empty);
                    }

                    // End row object.
                    writer.WriteEndObject();
                }
                // End array.
                writer.WriteEndArray();

                return writer.Token.ToString();
            }
        }
        public static string ToXml(this NpgsqlDataReader reader, string cultureInfo)
        {
            var culture = string.IsNullOrWhiteSpace(cultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(cultureInfo);

            using (var ms = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(ms, new UTF8Encoding(false)) { Formatting = Formatting.Indented })
                {
                    // Start XML document.
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Root");

                    while (reader.Read())
                    {
                        // Start row element.
                        writer.WriteStartElement("Row");
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            // Get column value.
                            var propertyValue = Convert.ToString(reader.GetValue(i), culture);

                            // Write column element.
                            writer.WriteElementString(reader.GetName(i), reader.GetValue(i).ToString() ?? string.Empty);
                        }
                        // Close row element.
                        writer.WriteEndElement();
                    }
                    // End Root element.
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
