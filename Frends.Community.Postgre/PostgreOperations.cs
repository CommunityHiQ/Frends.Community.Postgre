using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Frends.Community.Postgre
{
    /// <summary>
    /// Postgre operations
    /// </summary>
    public class PostgreOperations
    {
        /// <summary>
        /// Return type
        /// </summary>
        public enum PostgreQueryReturnType { XMLString, XMLDocument, XDocument, JSONString, Dynamic }

        /// <summary>
        /// Parameter
        /// </summary>
        public class Parameter
        {
            /// <summary>
            /// Field name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            ///  Field value
            /// </summary>
            public dynamic Value { get; set; }
        }
        /// <summary>
        /// Query Postgre
        /// </summary>
        public class Input
        {
            /// <summary>
            /// Query
            /// </summary>
            [DefaultValue("Note: Normal query requires double quotes around Column and 2 single quotes around Value. Example: SELECT * FROM table WHERE \"Column\" = ''Value'' " +
                          "Query with params Example: SELECT * FROM table WHERE \"Column\" = '||:Value||' ")]
            public string Query { get; set; }
            /// <summary>
            /// Query parameters
            /// </summary>
            public Parameter[] Parameters { get; set; }
            /// <summary>
            /// Return type
            /// </summary>
            public PostgreQueryReturnType ReturnType { get; set; }

        }
        /// <summary>
        /// 
        /// </summary>
        public class ConnectionInformation
        {
            /// <summary>
            /// Connection string
            /// </summary>
            [PasswordPropertyText(true)]
            [DefaultValue("\"Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;\"")]
            public string ConnectionString { get; set; }
            /// <summary>
            /// Timeout seconds
            /// </summary>
            [DefaultValue(30)]
            public int TimeoutSeconds { get; set; }
        }
        /// <summary>
        /// Return object
        /// </summary>
        public class Output
        {
            /// <summary>
            /// Request result
            /// </summary>
            public dynamic Result { get; set; }
        }

        private static void ParseToDynamic(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        ParseToDynamic(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();
                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }
                    foreach (var element in node.Elements())
                    {
                        ParseToDynamic(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }

        /// <summary>
        /// Query data using PostgreSQL. Example SELECT * FROM table WHERE "id" = '||:condition||'
        /// Note: Normal query requires double quotes around Column and 2 single quotes around Value. Example: SELECT * FROM table WHERE "Column" = ''Value''
        ///       Query with params Example: SELECT * FROM table WHERE "Column" = '||:Value||' 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<dynamic> QueryData(Input parameters, ConnectionInformation connectionInfo, CancellationToken cancellationToken)
        {
            using (var conn = new NpgsqlConnection(connectionInfo.ConnectionString))
            {
                await conn.OpenAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = connectionInfo.TimeoutSeconds;
                    cmd.CommandText = "SELECT query_to_xml('" + parameters.Query + "', true, false, '')";

                    // Lisätään parametrit, jos niitä oli
                    if (parameters.Parameters != null)
                    {
                        foreach (var parameter in parameters.Parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Name, "'" + parameter.Value + "'");
                        }
                    }

                    // Suoritetaan käsky.
                    var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    // Luetaan data. npgsql paketti ei sisällä command.ExecuteXmlReaderAsync();
                    var xmlData = new StringBuilder();
                    while (reader.Read())
                    {
                        xmlData.Append(reader.GetString(0));
                    }

                    // Viedään se XML-dokumentiksi
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlData.ToString());

                    // Palautetaan haluttu tyyppi
                    switch (parameters.ReturnType)
                    {
                        case PostgreQueryReturnType.XMLString:
                            return new Output() { Result = xmlDocument.OuterXml };
                        case PostgreQueryReturnType.XMLDocument:
                            return new Output() { Result = xmlDocument };
                        case PostgreQueryReturnType.JSONString:
                            return new Output() { Result = JsonConvert.SerializeXmlNode(xmlDocument) };
                        case PostgreQueryReturnType.XDocument:
                            var nodeReader = new XmlNodeReader(xmlDocument);
                            nodeReader.MoveToContent();
                            return new Output() { Result = XDocument.Load(nodeReader) };
                        case PostgreQueryReturnType.Dynamic:
                            dynamic root = new ExpandoObject();
                            var outputDoc = XDocument.Parse(xmlDocument.OuterXml, LoadOptions.PreserveWhitespace);
                            ParseToDynamic(root, outputDoc.Elements().First());
                            return new Output() { Result = root };
                        default:
                            return new Output() { Result = null };
                    }
                }
            }
        }
    }
}
