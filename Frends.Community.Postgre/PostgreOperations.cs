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
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Message, string Result } </returns>
        public static async Task<dynamic> QueryData([PropertyTab] QueryParameters queryParameters, [PropertyTab] OutputProperties output, [PropertyTab] ConnectionInformation connectionInfo, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            try
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
                                cmd.Parameters.AddWithValue(parameter.Name, parameter.Value);
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

                        return new Output { Success = true, Result = queryResult};
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw;
                return new Output
                {
                    Success = false,
                    Message = ex.Message
                };
            }

        }

        /// <summary>
        /// Task for performing queries in Postgre databases and saves result to csv. See documentation at https://github.com/CommunityHiQ/Frends.Community.Postgre
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="output"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Message, string Result } </returns>
        public static async Task<Output> QueryToFile([PropertyTab] QueryParameters queryParameters, [PropertyTab] SaveQueryToCsvOptions output, [PropertyTab] ConnectionInformation connectionInfo, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            try
            {
                using (var c = new NpgsqlConnection(connectionInfo.ConnectionString))
                {
                    try
                    {
                        await c.OpenAsync(cancellationToken);

                        using (var command = new NpgsqlCommand(queryParameters.Query, c))
                        {
                            command.CommandTimeout = connectionInfo.TimeoutSeconds;
                            command.CommandText = queryParameters.Query;

                            // Add parameters to command, if any were given.
                            if (queryParameters.Parameters != null)
                            {
                                foreach (var parameter in queryParameters.Parameters)
                                {
                                    command.Parameters.AddWithValue(parameter.Name, parameter.Value);
                                }
                            }
                            
                            var result = await command.ToCsvFileAsync(output, cancellationToken);
                            return new Output { Success = true, Result = result.ToString() };
                        }
                    }
                    finally
                    {
                        // Close connection
                        c.Dispose();
                        c.Close();
                        NpgsqlConnection.ClearPool(c);
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw;
                return new Output
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
