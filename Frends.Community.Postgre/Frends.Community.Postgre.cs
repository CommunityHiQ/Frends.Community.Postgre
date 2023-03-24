<<<<<<< HEAD
﻿using Frends.Community.Postgre.Definitions;
using Npgsql;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

[assembly: InternalsVisibleTo("Frends.Community.Postgre.Tests")]
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
        public static async Task<QueryResult> ExecuteQuery([PropertyTab] QueryParameters queryParameters, [PropertyTab] OutputProperties output, [PropertyTab] ConnectionInformation connectionInfo, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionInfo.ConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandTimeout = connectionInfo.TimeoutSeconds;
                            cmd.CommandText = queryParameters.Query;

                            // Add parameters to command, if any were given.
                            if (queryParameters.Parameters != null)
                            {
                                foreach (var parameter in queryParameters.Parameters)
                                {
                                    var value = parameter.Value;
                                    if (value == null)
                                        value = DBNull.Value;
                                    cmd.Parameters.AddWithValue(parameter.Name, value);
                                }
                            }

                            string queryResult;

                            switch (output.ReturnType)
                            {
                                case Enums.QueryReturnType.Xml:
                                    queryResult = await cmd.ToXmlAsync(output, cancellationToken);
                                    break;
                                case Enums.QueryReturnType.Json:
                                    queryResult = await cmd.ToJsonAsync(output, cancellationToken);
                                    break;
                                case Enums.QueryReturnType.Csv:
                                    queryResult = await cmd.ToCsvAsync(output, cancellationToken);
                                    break;
                                default:
                                    throw new ArgumentException("Task 'Return Type' was invalid! Check task properties.");
                            }

                            return new QueryResult { Success = true, Output = queryResult };
                        }
                    
                    }
                    finally
                    {
                        // Close connection
                        connection.Dispose();
                        connection.Close();
                        NpgsqlConnection.ClearPool(connection);
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueryResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Query data using PostgreSQL. Documentation: https://github.com/CommunityHiQ/Frends.Community.Postgre
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="output"></param>
        /// <param name="connectionInfo"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Message, string Result } </returns>
        public static async Task<QueryToFileResult> ExecuteQueryToFile([PropertyTab] QueryParameters queryParameters, [PropertyTab] SaveQueryToFileProperties output, [PropertyTab] ConnectionInformation connectionInfo, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionInfo.ConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync(cancellationToken);

                        using (var command = new NpgsqlCommand(queryParameters.Query, connection))
                        {
                            command.CommandTimeout = connectionInfo.TimeoutSeconds;
                            command.CommandText = queryParameters.Query;

                            if (queryParameters.Parameters != null)
                            {
                                foreach (var parameter in queryParameters.Parameters)
                                {
                                    command.Parameters.AddWithValue(parameter.Name, parameter.Value);
                                }
                            }

                            int rows = 0;
                            string path = "";

                            (rows, path) = await command.WriteToFileAsync(output, cancellationToken);
                             
                            return new QueryToFileResult { Success = true,  Rows = rows, Path = path };
                        }
                    }
                    finally
                    {
                        // Close connection
                        connection.Dispose();
                        connection.Close();
                        NpgsqlConnection.ClearPool(connection);
                    }
                }
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueryToFileResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
=======
﻿using System;
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
>>>>>>> a8a49e7940f8af3a9029c0efa7681f528f12ee75
