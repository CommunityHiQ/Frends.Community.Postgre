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
            private readonly PostgreOperations.ConnectionInformation _connection = new PostgreOperations.ConnectionInformation
            {
                ConnectionString = "Host=localhost;Username=testi;Password=test123;Database=frendstest",
                TimeoutSeconds = 10
            };

            /// <summary>
            /// Check that returned id values are 1,2,3.
            /// </summary>
            [Test]
            public void QuerydataThreeRows()
            {
                var input = new PostgreOperations.Input
                {
                    Query = "SELECT * FROM lista",
                    Parameters = null,
                    ReturnType = PostgreOperations.PostgreQueryReturnType.XMLString
                };

                string result = PostgreOperations.QueryData(input, _connection, new CancellationToken()).Result;

                Assert.IsTrue(result.Contains("<id>1</id>"));
                Assert.IsTrue(result.Contains("<id>2</id>"));
                Assert.IsTrue(result.Contains("<id>3</id>"));
            }

            /// <summary>
            /// Check that returns no rows with wrong id
            /// </summary>
            [Test]
            public void QuerydataNoRows()
            {
                var input = new PostgreOperations.Input
                {
                    Query = "SELECT * from lista WHERE id='||:ehto||'",
                    Parameters = new[] {new PostgreOperations.Parameter {Name = "ehto", Value = "0"}},
                    ReturnType = PostgreOperations.PostgreQueryReturnType.XMLString
                };

                string result = PostgreOperations.QueryData(input, _connection, new CancellationToken()).Result;

                var table = new XmlDocument();
                table.LoadXml(result);
                var node = table.SelectSingleNode("//*[local-name()='row'][1]");
                Assert.IsTrue(node == null);
            }

            /// <summary>
            /// Check that returns one row as xml
            /// </summary>
            [Test]
            public void QuerydataOneRowXmlDocument()
            {
                var input = new PostgreOperations.Input
                {
                    Query = "SELECT * FROM lista WHERE selite LIKE '||:ehto||' OR selite LIKE '||:toinenehto||'",
                    Parameters = new[]
                    {
                        new PostgreOperations.Parameter {Name = "ehto", Value = "foobar"},
                        new PostgreOperations.Parameter {Name = "toinenehto", Value = "'%Ensimm%'"}
                    },
                    ReturnType = PostgreOperations.PostgreQueryReturnType.XMLDocument
                };

                XmlDocument result = PostgreOperations.QueryData(input, _connection, new CancellationToken()).Result;

                var node = result.SelectSingleNode("//*[local-name()='row'][1]");
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
                var input = new PostgreOperations.Input
                {
                    Query = "SELECT * FROM lista WHERE selite LIKE '||:ehto||' OR selite LIKE '||:toinenehto||'",
                    Parameters = new[]
                    {
                        new PostgreOperations.Parameter {Name = "ehto", Value = "foobar"},
                        new PostgreOperations.Parameter {Name = "toinenehto", Value = "'%Ensimm%'"}
                    },
                    ReturnType = PostgreOperations.PostgreQueryReturnType.JSONString
                };

                string result = PostgreOperations.QueryData(input, _connection, new CancellationToken()).Result;

                Assert.IsTrue(result.Contains("\"row\":{\"id\":\"1\","));
            }
        }
    }
}
