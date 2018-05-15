using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

namespace Frends.Community.Postgre
{
    public class PostgreOperations
    {
        /// <summary>
        /// Query data using PostgreSQL. Example SELECT * FROM table WHERE "id" = '||:condition||'
        /// Note: Normal query requires double quotes around Column and 2 single quotes around Value. Example: SELECT * FROM table WHERE "Column" = ''Value''
        ///       Query with params Example: SELECT * FROM table WHERE "Column" = '||:Value||' 
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<dynamic> QueryData(ConnectionInformation connectionInfo, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            using (var conn = new NpgsqlConnection(connectionInfo.ConnectionString))
            {
                await conn.OpenAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = connectionInfo.TimeoutSeconds;
                    cmd.CommandText = queryParameters.Query;

                    // Add parameters to command, if any were given.
                    if (queryParameters.Parameters != null)
                    {
                        foreach (var parameter in queryParameters.Parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Name, parameter.Value );
                        }
                    }

                    // Execute command.
                    //var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    var reader = cmd.ExecuteReader();
                    cancellationToken.ThrowIfCancellationRequested();

                     // Return the desired type
                     switch (queryParameters.ReturnType)
                     {
                         case PostgreQueryReturnType.XMLString:
                             return new Output() { Result = reader.ToXml(queryParameters.CultureInfo) };
                         case PostgreQueryReturnType.JSONString:
                             return new Output() { Result = reader.ToJson(queryParameters.CultureInfo) };
                         default:
                             throw new ArgumentException("Task 'Return Type' was invalid! Check task properties.");
                     }
                }
            }
        }
    }
}
