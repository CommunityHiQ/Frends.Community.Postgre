using System.Threading;
using System.Xml;
using NUnit.Framework;
using System.Diagnostics;

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
                ConnectionString = "Server=testpostgr01.postgres.database.azure.com;Port=5432;User Id=testpostgr01@testpostgr01;Password=xRYiuJop8#1;Database=Test;SslMode=Require;",
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
                    Parameters = null,
                    ReturnType = PostgreQueryReturnType.XMLString
                };

                Output result = PostgreOperations.QueryData(_connection,input, new CancellationToken()).Result;


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
                    Query = "SELECT * from lista WHERE id='||:ehto||'",
                    Parameters = new[] { new Parameter { Name = "ehto", Value = "0"}},
                    ReturnType = PostgreQueryReturnType.XMLString
                };


                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;

                var table = new XmlDocument();
                table.LoadXml(result.Result.ToString());
                var node = table.SelectSingleNode("//*[local-name()='row'][1]");
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
                    Query = "SELECT * FROM lista WHERE selite LIKE '||:ehto||' OR selite LIKE '||:toinenehto||'",
                    Parameters = new[]
                    {
                        new Parameter {Name = "ehto", Value = "'foobar'"},
                        new Parameter {Name = "toinenehto", Value = "'%Ensimm%'"}
                    },
                    ReturnType = PostgreQueryReturnType.XMLString
                };

                Output result = PostgreOperations.QueryData(_connection, input, new CancellationToken()).Result;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result.Result);
                var node = doc.SelectSingleNode("//*[local-name()='row'][0]");
                Assert.IsTrue(node != null);
                var value = node.SelectSingleNode("*[local-name()='id']");
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
                    Query = "SELECT * FROM lista WHERE selite LIKE :ehto OR selite LIKE '||:toinenehto||' ",
                    Parameters = new[]
                    {
                        new Parameter {Name = "ehto", Value = "foobar"},
                        new Parameter {Name = "toinenehto", Value = "'%Ensimm%'"}
                    },
                    ReturnType = PostgreQueryReturnType.JSONString
                };

                Output result = PostgreOperations.QueryData( _connection, input, new CancellationToken()).Result;


                TestContext.Out.WriteLine("RESULT: "+result.Result);

                Assert.IsTrue(result.Result.Contains("[{{\"id\":1,"));
            }
        }
    }
}
