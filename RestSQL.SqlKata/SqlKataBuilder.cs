using SqlKata;
using System.Collections.Generic;
using SqlKata.Compilers;

namespace RestSQL.SqlKata
{
    public class SqlKataBuilder : IExpressionBuilder<Query>
    {
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
            var nested = new NestedCondition<Query>();
            nested.Component = "where";
            nested.Query = new Query();
            nested.Query.Clauses.AddRange(expression.Clauses);

            var q = new Query();
            q.Clauses.Add(nested);

            return q;
        }

        public Query Between(string column, object start, object end)
        {
            return new Query().WhereBetween(column, start, end);
        }

        public Query Equal(string column, object value)
        {
            return new Query().Where(column, value);
        }

        public Query GreaterOrEqual(string column, object value)
        {
            return new Query().Where(column, ">=", value);
        }

        public Query GreaterThan(string column, object value)
        {
            return new Query().Where(column, ">", value);
        }

        public Query In(string column, List<object> values)
        {
            return new Query().WhereIn(column, values);
        }

        public Query IsNotNull(string column)
        {
            return new Query().WhereNotNull(column);
        }

        public Query IsNull(string column)
        {
            return new Query().WhereNull(column);
        }

        public Query LessOrEqual(string column, object value)
        {
            return new Query().Where(column, "<=", value);
        }

        public Query LessThan(string column, object value)
        {
            return new Query().Where(column, "<", value);
        }

        public Query Like(string column, object value)
        {
            return new Query().WhereLike(column, value.ToString());
        }

        public Query NotBetween(string column, object start, object end)
        {
            return new Query().WhereNotBetween(column, start, end);
        }

        public Query NotEqual(string column, object value)
        {
            return new Query().Where(column, "!=", value);
        }

        public Query NotIn(string column, List<object> values)
        {
            return new Query().WhereNotIn(column, values);
        }

        public Query NotLike(string column, object value)
        {
            return new Query().WhereNotLike(column, value.ToString());
        }

        public Query Build(string query)
        {
            return RestSQL<Query>.Parse(query, this);
        }

        public Query BuildFrom(string query, Query from)
        {
            var build = this.Build(query);

            var nested = new NestedCondition<Query>();
            nested.Component = "where";
            nested.Query = new Query();
            nested.Query.Clauses.AddRange(build.Clauses);

            from.Clauses.Add(nested);
            return from;
        }
    }
}
