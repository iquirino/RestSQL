using SqlKata;
using System.Collections.Generic;

namespace RestSQL.SqlKata
{
    public class SqlKataBuilder : IExpressionBuilder<Query>
    {
        private readonly IColumnNameTransform transform = null;

        public delegate string ColumnNameEventHandler(string columnName);
        public event ColumnNameEventHandler OnColumnName;

        public delegate bool CustomQueryEventHandler(out Query query, RSqlQueryType action, string columnName, params object[] values);
        public event CustomQueryEventHandler CustomQuery;

        public SqlKataBuilder(IColumnNameTransform transform = null)
        {
            if (transform != null)
                this.transform = transform;
        }

        public Query And(Query expression1, Query expression2)
        {
            if (expression1 == null)
                return expression2;
            if (expression2 == null)
                return expression1;

            foreach (var expClauses in expression2.Clauses)
            {
                if (expClauses is AbstractCondition exp)
                    exp.IsOr = false;
            }

            expression1.Clauses.AddRange(expression2.Clauses);
            return expression1;
        }

        public Query Or(Query expression1, Query expression2)
        {
            if (expression1 == null)
                return expression2;
            if (expression2 == null)
                return expression1;

            foreach (var expClauses in expression2.Clauses)
            {
                if (expClauses is AbstractCondition exp)
                    exp.IsOr = true;
            }

            expression1.Clauses.AddRange(expression2.Clauses);

            return expression1;
        }

        public Query Parenthesize(Query expression)
        {
            var nested = new NestedCondition<Query>
            {
                Component = "where",
                Query = new Query()
            };
            nested.Query.Clauses.AddRange(expression.Clauses);

            var q = new Query();
            q.Clauses.Add(nested);

            return q;
        }

        public Query Between(string column, object start, object end)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.Between, column, start, end))
                return query;

            return new Query().WhereBetween(GetColumnName(column), start, end);
        }

        public Query Equal(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.Equal, column, value))
                return query;

            return new Query().Where(GetColumnName(column), value);
        }

        public Query GreaterOrEqual(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.GreaterOrEqual, column, value))
                return query;

            return new Query().Where(GetColumnName(column), ">=", value);
        }

        public Query GreaterThan(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.GreatherThan, column, value))
                return query;

            return new Query().Where(GetColumnName(column), ">", value);
        }

        public Query In(string column, List<object> values)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.In, column, values.ToArray()))
                return query;

            return new Query().WhereIn(GetColumnName(column), values);
        }

        public Query IsNotNull(string column)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.IsNotNull, column))
                return query;

            return new Query().WhereNotNull(GetColumnName(column));
        }

        public Query IsNull(string column)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.IsNull, column))
                return query;

            return new Query().WhereNull(GetColumnName(column));
        }

        public Query LessOrEqual(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.LessOrEqual, column, value))
                return query;

            return new Query().Where(GetColumnName(column), "<=", value);
        }

        public Query LessThan(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.LessThan, column, value))
                return query;

            return new Query().Where(GetColumnName(column), "<", value);
        }

        public Query Like(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.Like, column, value))
                return query;

            return new Query().WhereLike(GetColumnName(column), value.ToString(), true);
        }

        public Query NotBetween(string column, object start, object end)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.NotBetween, column, start, end))
                return query;

            return new Query().WhereNotBetween(GetColumnName(column), start, end);
        }

        public Query NotEqual(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.NotEqual, column, value))
                return query;

            return new Query().Where(GetColumnName(column), "!=", value);
        }

        public Query NotIn(string column, List<object> values)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.NotIn, column, values.ToArray()))
                return query;

            return new Query().WhereNotIn(GetColumnName(column), values);
        }

        public Query NotLike(string column, object value)
        {
            if (CustomQuery != null && CustomQuery(out Query query, RSqlQueryType.NotLike, column, value))
                return query;

            return new Query().WhereNotLike(GetColumnName(column), value.ToString(), true);
        }

        public Query Build(string query)
        {
            return RestSQL<Query>.Parse(query, this);
        }

        public Query BuildFrom(string query, Query from)
        {
            var build = this.Build(query);

            var nested = new NestedCondition<Query>
            {
                Component = "where",
                Query = new Query()
            };
            nested.Query.Clauses.AddRange(build.Clauses);

            from.Clauses.Add(nested);
            return from;
        }
        private string GetColumnName(string columnName)
        {
            string cName = null;

            if (OnColumnName != null)
                cName = OnColumnName(columnName);

            if (string.IsNullOrWhiteSpace(cName) && this.transform != null)
                cName = this.transform.Transform(columnName);

            if (string.IsNullOrWhiteSpace(cName))
                return columnName;

            return cName;
        }
    }
}
