using Frends.Community.Postgre.Definitions;
using Npgsql;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace Frends.Community.Postgre.Tests
{
    [TestFixture]
    public class DataBaseTests
    {
        /// <summary>
        /// These test requires local postgres database, create it e.g. with
        ///
        ///  docker run -p 5432:5432 -e POSTGRES_PASSWORD=mysecretpassword -d postgres
        ///
        /// </summary>
        
        [TestFixture]
        public class PostgreOperationsTests
        {
            private readonly string _connString = "Host=localhost;Database=postgres;Port=5432;User Id=postgres;Password=mysecretpassword;";

            private static ConnectionInformation _connection;

            private readonly string _baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));

            [OneTimeSetUp]
            public void TestSetup()
            {
                _connection = new ConnectionInformation
                {
                    ConnectionString = _connString,
                    TimeoutSeconds = 10
                };

                using (var conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(@"CREATE TABLE IF NOT EXISTS ""lista"" (Id int, Selite varchar)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new NpgsqlCommand(@"INSERT INTO ""lista"" (Id, Selite) VALUES (1, 'Ensimmäinen'), (2, 'foobar'), (3, ''), (4, null)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                
                Directory.CreateDirectory(_baseDir);
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                using (var conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(@"DROP TABLE ""lista""", conn))
                    {
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                Directory.Delete(_baseDir, true);
            }

            /// <summary>
            /// Check that returned id values are 1,2,3.
            /// </summary>
            [Test]
            public void QuerydataThreeRows()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista;",
                    Parameters = null
                };

                var output = new OutputProperties
                {
                    ReturnType = Enums.QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "ROW",
                        RowElementName = "ROWSET",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQuery(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Output);


                Assert.IsTrue(result.Output.Contains("<id>1</id>"));
                Assert.IsTrue(result.Output.Contains("<id>2</id>"));
                Assert.IsTrue(result.Output.Contains("<id>3</id>"));
            }

            /// <summary>
            /// Check that returns no rows with wrong id
            /// </summary>
            [Test]
            public void QuerydataNoRows()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * from lista WHERE id=:ehto",
                    Parameters = new[] { new Parameter { Name = "ehto", Value = 0 } },
                };

                var output = new OutputProperties
                {
                    ReturnType = Enums.QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "Root",
                        RowElementName = "Row",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQuery(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Output);

                var table = new XmlDocument();
                table.LoadXml(result.Output.ToString());
                var node = table.SelectSingleNode("/Root/Row[1]/id");
                Assert.IsNull(node);
            }

            /// <summary>
            /// Check that returns one row as xml
            /// </summary>
            [Test]
            public void QuerydataOneRowXmlDocument()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista WHERE selite LIKE :ehto OR selite LIKE :toinenehto",
                    Parameters = new[]
                    {
                        new Parameter {Name = "ehto", Value = "foobar"},
                        new Parameter {Name = "toinenehto", Value = "%Ensimm%"}
                    },
                };

                var output = new OutputProperties
                {
                    ReturnType = Enums.QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "Root",
                        RowElementName = "Row",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQuery(input, output, _connection, options, new CancellationToken()).Result;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result.Output);
                TestContext.Out.WriteLine("RESULT: " + result.Output);
                var node = doc.SelectSingleNode("/Root/Row[1]/id");
                Assert.IsNotNull(node);
                var value = node.SelectSingleNode("//id");
                Assert.IsNotNull(value);
                Assert.AreEqual("1", value.InnerText);
            }

            /// <summary>
            /// Check that returns one row as xml
            /// </summary>
            [Test]
            public void QuerydataOneRowJSON()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista WHERE selite LIKE :ehto OR selite LIKE :toinenehto ",
                    Parameters = new[]
                    {
                        new Parameter {Name = "ehto", Value = "foobar"},
                        new Parameter {Name = "toinenehto", Value = "%Ensimm%"}
                    }
                };

                var output = new OutputProperties
                {
                    ReturnType = Enums.QueryReturnType.Json,
                    JsonOutput = new JsonOutputProperties()
                    {
                        CultureInfo = "",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQuery(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Output);

                Assert.IsTrue(result.Output.Contains("\"id\": 1"));
            }

            [Test]
            public void QuerydataOneRowCSV()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista WHERE selite LIKE :ehto OR selite LIKE :toinenehto ",
                    Parameters = new[]
    {
                        new Parameter {Name = "ehto", Value = "foobar"},
                        new Parameter {Name = "toinenehto", Value = "%Ensimm%"}
                    }
                };

                var output = new OutputProperties
                {
                    ReturnType = Enums.QueryReturnType.Csv,
                    CsvOutput = new CsvOutputProperties()
                    {
                        IncludeHeaders = false,
                        FieldDelimiter = Enums.CsvFieldDelimiter.Semicolon
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQuery(input, output, _connection, options, new CancellationToken()).Result;

                
                Assert.IsTrue(result.Output.Contains("1"));
            }

            [Test]
            public void QueryToFileSingleCsv()
            {
                var path = Path.Combine(_baseDir, "temps.csv");
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista",
                    Parameters = null
                };

                var output = new SaveQueryToFileProperties
                {
                    ReturnType = Enums.QueryReturnType.Csv,
                    Path = path,
                    Encoding = Enums.EncodingOptions.UTF8,
                    EnableBom = false,
                    CsvOptions = new CsvOutputProperties
                    {
                        FieldDelimiter = Enums.CsvFieldDelimiter.Semicolon,
                        LineBreak = Enums.CsvLineBreak.CRLF,
                        IncludeHeaders = true,
                        SanitizeColumnHeaders = true,
                        AddQuotesToDates = true,
                        DateFormat = "yyyy-MM-dd",
                        DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                    },
                    Append = false
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQueryToFile(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine($"RESULT: {result.Path}, {result.Rows}");

                Assert.IsTrue(File.Exists(path));
                string fileData = File.ReadAllText(path);
                TestContext.Out.WriteLine("Content: " + fileData);

                Assert.IsTrue(fileData.Contains("1"));
                Assert.IsTrue(fileData.Contains("2"));
                Assert.IsTrue(fileData.Contains("3"));

                File.Delete(path);
            }

            [Test]
            public void QueryToFileSingleJson()
            {
                var path = Path.Combine(_baseDir, "temps.json");
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista",
                    Parameters = null
                };

                var output = new SaveQueryToFileProperties
                {
                    ReturnType = Enums.QueryReturnType.Json,
                    Path = path,
                    Encoding = Enums.EncodingOptions.UTF8,
                    EnableBom = false,
                    JsonOptions = new JsonOutputProperties
                    {
                        CultureInfo = "fi-FI"
                    },
                    Append = false
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQueryToFile(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine($"RESULT: {result.Path}, {result.Rows}");

                Assert.IsTrue(File.Exists(path));
                var fileData = JToken.Parse(File.ReadAllText(path));
                TestContext.Out.WriteLine("Content: " + fileData);
                Assert.AreEqual(1, fileData[0]["id"].Value<int>());
                Assert.AreEqual("Ensimmäinen", fileData[0]["selite"].Value<string>());

                File.Delete(path);
            }

            [Test]
            public void QueryToFileSingleXml()
            {
                var path = Path.Combine(_baseDir, "temps.xml");
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista",
                    Parameters = null
                };

                var output = new SaveQueryToFileProperties
                {
                    ReturnType = Enums.QueryReturnType.Xml,
                    Path = path,
                    Encoding = Enums.EncodingOptions.UTF8,
                    EnableBom = false,
                    XmlOptions = new XmlOutputProperties
                    {
                        RootElementName = "Root",
                        RowElementName = "Row"
                    },
                    Append = false
                };

                var options = new Options { ThrowErrorOnFailure = true };

                var result = PostgreOperations.ExecuteQueryToFile(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine($"RESULT: {result.Path}, {result.Rows}");

                Assert.IsTrue(File.Exists(path));
                var fileData = File.ReadAllText(path);

                XmlDocument doc = new XmlDocument();
                TestContext.Out.WriteLine("Content: " + fileData);
                doc.LoadXml(fileData);
                
                var node = doc.SelectSingleNode("/Root/Row[1]/id");
                Assert.IsNotNull(node);
                var value = node.SelectSingleNode("//id");
                Assert.IsNotNull(value);
                Assert.AreEqual("1", value.InnerText);
                value = node.SelectSingleNode("//selite");
                Assert.IsNotNull(value);
                Assert.AreEqual("Ensimmäinen", value.InnerText);

                File.Delete(path);
            }
        }
    }
}