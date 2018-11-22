using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.CaseInsensitive.Extensions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Sql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.CaseInsensitive.Query.Sql
{
    public class CaseInsensitiveSqlGenerator : NpgsqlQuerySqlGenerator
    {
        private bool _predicateGenerating;
        private RelationalTypeMapping _typeMapping;

        public CaseInsensitiveSqlGenerator(QuerySqlGeneratorDependencies dependencies, SelectExpression selectExpression, bool reverseNullOrderingEnabled)
            : base(dependencies, selectExpression, reverseNullOrderingEnabled)
        {
        }

        protected override void GeneratePredicate(Expression predicate)
        {
            _predicateGenerating = true;

            base.GeneratePredicate(predicate);

            _predicateGenerating = false;
        }

        public override Expression VisitColumn(ColumnExpression columnExpression)
        {
            if (columnExpression.Property.PropertyInfo.PropertyType != typeof(string) || !_predicateGenerating)
                return base.VisitColumn(columnExpression);

            var builder = new StringBuilder();
            builder.Append(SqlGenerator.DelimitIdentifier(columnExpression.Table.Alias))
                .Append(".")
                .Append(SqlGenerator.DelimitIdentifier(columnExpression.Name));

            AddLowerFunctionToSqlQuery(builder.ToString());

            return columnExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            if (!_predicateGenerating || constantExpression.Type != typeof(string))
                return base.VisitConstant(constantExpression);

            _typeMapping = InferTypeMappingFromColumn(constantExpression);

            AddLowerFunctionToSqlQuery(GenerateSqlLiteral(constantExpression.Value));

            return constantExpression;
        }

        private string GenerateSqlLiteral(object value)
        {
            var mapping = _typeMapping;
            var mappingClrType = mapping?.ClrType;

            if (mappingClrType != null
                && (value == null
                    || mappingClrType.IsInstanceOfType(value)
                    || value.GetType().IsInteger()
                    && (mappingClrType.IsInteger()
                        || mappingClrType.IsEnum)))
            {
                if (value?.GetType().IsInteger() == true
                    && mappingClrType.IsEnum)
                {
                    value = Enum.ToObject(mappingClrType, value);
                }
            }
            else
            {
                mapping = Dependencies.TypeMappingSource.GetMappingForValue(value);
            }

            return mapping.GenerateSqlLiteral(value);
        }


        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            var parameterType = parameterExpression.Type;

            if (!_predicateGenerating || parameterType != typeof(string)) return base.VisitParameter(parameterExpression);

            var parameterName = SqlGenerator.GenerateParameterName(parameterExpression.Name);

            if (Sql.ParameterBuilder.Parameters
                .All(p => p.InvariantName != parameterExpression.Name))
            {
                var typeMapping = InferTypeMappingFromColumn(parameterExpression);

                if (typeMapping == null
                    || (!typeMapping.ClrType.IsAssignableFrom(parameterType)
                        && (parameterType.IsEnum
                            || !typeof(IConvertible).IsAssignableFrom(parameterType))))
                {
                    typeMapping = Dependencies.TypeMappingSource.GetMapping(parameterType);
                }

                var parameterIsNullable = Nullable.GetUnderlyingType(parameterExpression.Type) != null;

                Sql.AddParameter(
                    parameterExpression.Name,
                    parameterName,
                    typeMapping,
                    parameterIsNullable);
            }

            var parameterNamePlaceholder = SqlGenerator.GenerateParameterName(parameterExpression.Name);

            AddLowerFunctionToSqlQuery(parameterNamePlaceholder);

            return parameterExpression;
        }

        public void AddLowerFunctionToSqlQuery(string value)
            => Sql.Append("lower(")
                .Append(value)
                .Append(")");


    }
}
