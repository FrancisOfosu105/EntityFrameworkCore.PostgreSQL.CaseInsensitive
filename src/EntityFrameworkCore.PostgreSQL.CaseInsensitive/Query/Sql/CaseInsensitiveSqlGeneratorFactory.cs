using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.CaseInsensitive.Query.Sql
{
    public class CaseInsensitiveSqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        /// <summary>
        /// Represents options for Npgsql that can only be set by the service provider.
        /// </summary>
        readonly INpgsqlOptions _npgsqlOptions;

        /// <inheritdoc />
        public CaseInsensitiveSqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
               INpgsqlOptions npgsqlOptions)
            : base(dependencies)
        {
            _npgsqlOptions = npgsqlOptions;
        }

        /// <inheritdoc />
        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new CaseInsensitiveSqlGenerator(
                Dependencies,
                selectExpression,
                _npgsqlOptions.ReverseNullOrderingEnabled,
                _npgsqlOptions.PostgresVersion);
    }
}
