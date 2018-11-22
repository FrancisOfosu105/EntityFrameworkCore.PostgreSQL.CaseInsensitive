using Microsoft.EntityFrameworkCore.Query.Sql;
using Npgsql.EntityFrameworkCore.PostgreSQL.CaseInsensitive.Query.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension
    {
        public static DbContextOptionsBuilder UseNpgsqlCaseInsensitive(this DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
            return builder.ReplaceService<IQuerySqlGeneratorFactory, CaseInsensitiveSqlGeneratorFactory>();
        }
    }
}
