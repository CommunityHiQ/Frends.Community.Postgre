using Npgsql;
using NUnit.Framework;
using System;
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
        ///  docker run --name mypostgres -p 5432:5432 -e POSTGRES_PASSWORD=mysecretpassword -d postgres
        ///
        /// </summary>
        [TestFixture]
        public class PostgreOperationsTests
        {
            private readonly ConnectionInformation _connection = new ConnectionInformation
            {
                ConnectionString = "Host=localhost;Database=postgres;Port=5432;User Id=postgres;Password=mysecretpassword;",
                TimeoutSeconds = 10
            };

            [OneTimeSetUp]
            public void TestSetup()
            {
                using (var conn = new NpgsqlConnection(_connection.ConnectionString))
                {

                    conn.Open();

                    using (var cmd = new NpgsqlCommand(@"CREATE TABLE IF NOT EXISTS ""lista"" (Id int, Selite varchar)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new NpgsqlCommand(@"INSERT INTO ""lista"" (Id, Selite) VALUES (1, 'Ensimmäinen'), (2, 'foobar'), (3, '')", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                using (var conn = new NpgsqlConnection(_connection.ConnectionString))
                {

                    conn.Open();

                    using (var cmd = new NpgsqlCommand(@"DROP TABLE ""lista""", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Check that returned id values are 1,2,3.
            /// </summary>
            /// 
            [Test]
            public void QuerydataThreeRows()
            {
                var input = new QueryParameters
                {
                    Query = "SELECT * FROM lista;",
                    Parameters = null,
                    ReturnType = PostgreQueryReturnType.XMLString
                };

                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;

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
                    ReturnType = PostgreQueryReturnType.XMLString
                };


                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;

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
                    ReturnType = PostgreQueryReturnType.XMLString
                };

                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;
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
                    },
                    ReturnType = PostgreQueryReturnType.JSONString
                };

                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;


                TestContext.Out.WriteLine("RESULT: " + result.Result);

                Assert.IsTrue(result.Result.Contains("\"id\": 1"));
            }
        }
    }
}
