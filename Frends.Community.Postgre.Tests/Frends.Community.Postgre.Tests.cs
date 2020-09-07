using System;
using System.IO;
using System.Threading;
using System.Xml;
using NUnit.Framework;

namespace Frends.Community.Postgre.Tests
{
    [TestFixture]
    public class DataBaseTests
    {
        [TestFixture]
        public class PostgreOperationsTests
        {
            private readonly ConnectionInformation _connection = new ConnectionInformation
            {
                ConnectionString = @"User ID = postgres; Password=;Host=localhost;Port=5432;Database=postgres",
                TimeoutSeconds = 10
            };

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
                    ReturnType = QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "ROW",
                        RowElementName = "ROWSET",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryData(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Result);


                Assert.IsTrue(result.Result.Contains("<id>1</id>"));
                Assert.IsTrue(result.Result.Contains("<id>2</id>"));
                Assert.IsTrue(result.Result.Contains("<id>3</id>"));
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
                    ReturnType = QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "Root",
                        RowElementName = "Row",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryData(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Result);

                var table = new XmlDocument();
                table.LoadXml(result.Result.ToString());
                var node = table.SelectSingleNode("/Root/Row[1]/id");
                Assert.IsTrue(node == null);
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
                    ReturnType = QueryReturnType.Xml,
                    XmlOutput = new XmlOutputProperties
                    {
                        RootElementName = "Root",
                        RowElementName = "Row",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryData(input, output, _connection, options, new CancellationToken()).Result;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result.Result);
                TestContext.Out.WriteLine("RESULT: " + result.Result);
                var node = doc.SelectSingleNode("/Root/Row[1]/id");
                Assert.IsTrue(node != null);
                var value = node.SelectSingleNode("//id");
                Assert.IsTrue(value != null && value.InnerText == "1");
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
                    ReturnType = QueryReturnType.Json,
                    JsonOutput = new JsonOutputProperties()
                    {
                        CultureInfo = "",
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryData(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Result);

                Assert.IsTrue(result.Result.Contains("\"id\": 1"));
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
                    ReturnType = QueryReturnType.Csv,
                    CsvOutput = new CsvOutputProperties()
                    {
                        IncludeHeaders = false,
                        CsvSeparator = ";"
                    }
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryData(input, output, _connection, options, new CancellationToken()).Result;

                
                Assert.IsTrue(result.Result.Contains("1"));
            }

            [Test]
            public void QueryToFileSingle()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista",
                    Parameters = null
                };

                var output = new SaveQueryToCsvOptions
                {
                    OutputFilePath = "c:/temp/temps.csv",
                    Encoding = "utf-8",
                    FieldDelimiter = CsvFieldDelimiter.Semicolon,
                    LineBreak = CsvLineBreak.CRLF,
                    IncludeHeadersInOutput = true,
                    SanitizeColumnHeaders = true,
                    AddQuotesToDates = true,
                    DateFormat = "yyyy-MM-dd",
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };

                var options = new Options { ThrowErrorOnFailure = true };

                Output result = PostgreOperations.QueryToFile(input, output, _connection, options, new CancellationToken()).Result;

                TestContext.Out.WriteLine("RESULT: " + result.Result);

                string fileData = File.ReadAllText("c:/temp/temps.csv");
                Assert.IsTrue(File.Exists("c:/temp/temps.csv"));
                Assert.IsTrue(fileData.Contains("1"));
                Assert.IsTrue(fileData.Contains("2"));
                Assert.IsTrue(fileData.Contains("3"));
            }
        }
    }
}
