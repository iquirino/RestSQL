using RestSQL;
using System.Collections.Generic;
using System.Text;

namespace RestQL.Tests.Providers
{
    public class TestExpressionBuilder : IExpressionBuilder<TestExpression>
    {
        private static readonly string OR = " OR ";
        private static readonly string AND = " AND ";
        private static readonly string COMPARISION_TEMPLATE = "{0} {1} ?";
        private static readonly string IS_NULL_TEMPLATE = "{0} IS NULL";
        private static readonly string IS_NOT_NULL_TEMPLATE = "{0} IN NOT NULL";
        private static readonly string LIKE_TEMPLATE = "{0} LIKE ?";
        private static readonly string NOT_LIKE_TEMPLATE = "{0} NOT LIKE ?";
        private static readonly string BETWEEN_TEMPLATE = "{0} BETWEEN ? AND ?";
        private static readonly string NOT_BETWEEN_TEMPLATE = "{0} NOT BETWEEN ? AND ?";
        private static readonly string IN_TEMPLATE = "{0} IN";
        private static readonly string NOT_IN_TEMPLATE = "{0} NOT IN";
        private static readonly string COMMA = ",";

        public TestExpression Or(TestExpression statement1, TestExpression statement2)
        {
            return this.andOr(statement1, statement2, OR);
        }

        public TestExpression And(TestExpression statement1, TestExpression statement2)
        {
            return this.andOr(statement1, statement2, AND);
        }

        public TestExpression Parenthesize(TestExpression statement)
        {
            return new TestExpression(" ( " + statement.GetWhereClause() + " ) ", statement.GetParams());
        }

        private TestExpression andOr(TestExpression statement1, TestExpression statement2, string op)
        {
            if (statement1 == null)
                return statement2;
            if (statement2 == null)
                return statement1;

            string whereSql = statement1.GetWhereClause() + op + statement2.GetWhereClause();

            var p = new List<object>();
            p.AddRange(statement1.GetParams());
            p.AddRange(statement2.GetParams());

            return new TestExpression(whereSql, p);
        }

        public TestExpression Equal(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, "=", value);
        }

        public TestExpression NotEqual(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, "!=", value);
        }

        public TestExpression LessOrEqual(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, "<=", value);
        }

        public TestExpression LessThan(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, "<", value);
        }

        public TestExpression GreaterOrEqual(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, ">=", value);
        }

        public TestExpression GreaterThan(string columnCode, object value)
        {
            return this.comparisionPredicate(columnCode, ">", value);
        }

        public TestExpression IsNotNull(string column)
        {
            return new TestExpression(string.Format(IS_NOT_NULL_TEMPLATE, this.getColumnName(column)));
        }

        public TestExpression IsNull(string column)
        {
            return new TestExpression(string.Format(IS_NULL_TEMPLATE, this.getColumnName(column)));
        }

        public TestExpression NotLike(string column, object value)
        {
            return new TestExpression(string.Format(NOT_LIKE_TEMPLATE, this.getColumnName(column)), value);
        }

        public TestExpression Like(string column, object value)
        {
            return new TestExpression(string.Format(LIKE_TEMPLATE, this.getColumnName(column)), value);
        }

        public TestExpression NotBetween(string column, object start, object end)
        {
            return new TestExpression(string.Format(NOT_BETWEEN_TEMPLATE, this.getColumnName(column)), start, end);
        }

        public TestExpression Between(string column, object start, object end)
        {
            return new TestExpression(string.Format(BETWEEN_TEMPLATE, this.getColumnName(column)), start, end);
        }

        public TestExpression NotIn(string column, List<object> values)
        {

            var inClause = new StringBuilder(string.Format(NOT_IN_TEMPLATE, this.getColumnName(column)));
            inClause.Append(" (");
            inClause.Append(this.repeat("?", COMMA, values.Count));
            inClause.Append(")");

            return new TestExpression(inClause.ToString(), values);
        }

        public TestExpression In(string column, List<object> values)
        {
            var inClause = new StringBuilder(string.Format(IN_TEMPLATE, this.getColumnName(column)));
            inClause.Append(" (");
            inClause.Append(this.repeat("?", COMMA, values.Count));
            inClause.Append(")");

            return new TestExpression(inClause.ToString(), values);
        }

        private TestExpression comparisionPredicate(string columnCode, string op, object value)
        {
            var p = new List<object>
        {
            value
        };
            return new TestExpression(string.Format(COMPARISION_TEMPLATE, this.getColumnName(columnCode), op), p);
        }

        private string getColumnName(string columnCode)
        {
            return columnCode;
        }

        private string repeat(string s, string separator, int count)
        {
            var str = new StringBuilder((s.Length + separator.Length) * count);
            while (--count > 0)
                str.Append(s).Append(separator);

            return str.Append(s).ToString();
        }

        public TestExpression Build(string query)
        {
            return RestSQL<TestExpression>.Parse(query, this);
        }
    }
}
