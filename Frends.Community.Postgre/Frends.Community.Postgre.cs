using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

#pragma warning disable 1591

namespace Frends.Community.Postgre
{
    public static class PostgreOperations
    {
        /// <summary>
        /// Query data using PostgreSQL. Documentation: https://github.com/CommunityHiQ/Frends.Community.Postgre
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object {string Result}</returns>
        public static async Task<dynamic> QueryData(ConnectionInformation connectionInfo, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            using (var conn = new NpgsqlConnection(connectionInfo.ConnectionString))
            {
                await conn.OpenAsync(cancellationToken);
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
                            cancellationToken.ThrowIfCancellationRequested();

                            // Convert parameter.Value to DBNull.Value if it is set to null.
                            if (parameter.Value == null)
                            {
                                cmd.Parameters.AddWithValue(parameter.Name, DBNull.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(parameter.Name, parameter.Value);
                            }
                        }
                    }

                    // Execute command.
                    var reader = cmd.ExecuteReader();

                    // Return the desired type.
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
