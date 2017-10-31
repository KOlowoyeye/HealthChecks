// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Npgsql;

namespace Microsoft.Extensions.HealthChecks
{
    public static class HealthCheckBuilderPostgreSqlExtensions
    {
        public static HealthCheckBuilder AddPostgreSqlCheck(this HealthCheckBuilder builder, string name, string connectionString)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return AddPostgreSqlCheck(builder, name, connectionString, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddPostgreSqlCheck(this HealthCheckBuilder builder, string name, string connectionString, TimeSpan cacheDuration)
        {
            builder.AddCheck($"PostgreSqlCheck({name})", async () =>
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new NpgsqlCommand("SELECT 1", connection))
                        {
                            int result = (int)await command.ExecuteScalarAsync();

                            if (result == 1)
                            {
                                return HealthCheckResult.Healthy($"PostgreSqlCheck({name}): Healthy");
                            }

                            return HealthCheckResult.Healthy($"PostgreSqlCheck({name}): Unhealthy");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"PostgreSqlCheck({name}): Exception during check: {ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }
    }
}