using Npgsql;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

namespace Frends.Community.Postgre
{
    public class PostgreOperations
    {
        /// <summary>
        /// Query data using PostgreSQL. Documentation: https://github.com/CommunityHiQ/Frends.Community.Postgre
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="output"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object {string Result}</returns>
        public static async Task<dynamic> QueryData([PropertyTab] QueryParameters queryParameters, [PropertyTab] OutputProperties output, [PropertyTab] ConnectionInformation connectionInfo, CancellationToken cancellationToken)
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

                    string queryResult;

                    switch (output.ReturnType)
                     {
                         case QueryReturnType.Xml:
                             queryResult = await cmd.ToXmlAsync(output, cancellationToken);
                             break;
                         case QueryReturnType.Json:
                             queryResult = await cmd.ToJsonAsync(output, cancellationToken);
                             break;
                         case QueryReturnType.Csv:
                             queryResult = await cmd.ToCsvAsync(output, cancellationToken);
                             break;
                         default:
                             throw new ArgumentException("Task 'Return Type' was invalid! Check task properties.");
                     }

                     return new Output { Result = queryResult };
                }
            }
        }
    }
}
