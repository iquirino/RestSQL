using System.Collections.Generic;
using System.Text;

namespace RestSQL.NPoco
{
    public class NPocoBuilder : IExpressionBuilder<NPocoExpression>
    {
        //TODO: Change Parameters "?" with @0, @1, @2, ...
        //https://github.com/schotime/NPoco/wiki/Sql-Templating
        private static readonly string OR = " OR ";
        private static readonly string AND = " AND ";
        private static readonly string COMPARE_TEMPLATE = "{0} {1} ?";
        private static readonly string IS_NULL_TEMPLATE = "{0} IS NULL";
        private static readonly string IS_NOT_NULL_TEMPLATE = "{0} IN NOT NULL";
        private static readonly string LIKE_TEMPLATE = string.Format(COMPARE_TEMPLATE, "{0}", "LIKE");
        private static readonly string NOT_LIKE_TEMPLATE = string.Format(COMPARE_TEMPLATE, "{0}", "NOT LIKE");
        private static readonly string BETWEEN_TEMPLATE = "{0} BETWEEN ? AND ?";
        private static readonly string NOT_BETWEEN_TEMPLATE = "{0} NOT BETWEEN ? AND ?";
        private static readonly string IN_TEMPLATE = "{0} IN";
        private static readonly string NOT_IN_TEMPLATE = "{0} NOT IN";
        private static readonly string COMMA = ",";

        public delegate string ColumnNameEventHandler(string columnName);
        public event ColumnNameEventHandler OnColumnName;

        public NPocoExpression Or(NPocoExpression statement1, NPocoExpression statement2)
        {
            return this.AndOr(statement1, statement2, OR);
        }

        public NPocoExpression And(NPocoExpression statement1, NPocoExpression statement2)
        {
            return this.AndOr(statement1, statement2, AND);
        }

        public NPocoExpression Parenthesize(NPocoExpression statement)
        {
            return new NPocoExpression(" ( " + statement.rawSql + " ) ", statement.parameters);
        }

        private NPocoExpression AndOr(NPocoExpression statement1, NPocoExpression statement2, string op)
        {
            if (statement1 == null)
                return statement2;
            if (statement2 == null)
                return statement1;

            string whereSql = statement1.rawSql + op + statement2.rawSql;

            var p = new List<object>();
            p.AddRange(statement1.parameters);
            p.AddRange(statement2.parameters);

            return new NPocoExpression(whereSql, p);
        }

        public NPocoExpression Equal(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, "=", value);
        }

        public NPocoExpression NotEqual(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, "!=", value);
        }

        public NPocoExpression LessOrEqual(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, "<=", value);
        }

        public NPocoExpression LessThan(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, "<", value);
        }

        public NPocoExpression GreaterOrEqual(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, ">=", value);
        }

        public NPocoExpression GreaterThan(string columnCode, object value)
        {
            return this.ComparePredicate(columnCode, ">", value);
        }

        public NPocoExpression IsNotNull(string column)
        {
            return new NPocoExpression(string.Format(IS_NOT_NULL_TEMPLATE, this.GetColumnName(column)));
        }

        public NPocoExpression IsNull(string column)
        {
            return new NPocoExpression(string.Format(IS_NULL_TEMPLATE, this.GetColumnName(column)));
        }

        public NPocoExpression NotLike(string column, object value)
        {
            return new NPocoExpression(string.Format(NOT_LIKE_TEMPLATE, this.GetColumnName(column)), value);
        }

        public NPocoExpression Like(string column, object value)
        {
            return new NPocoExpression(string.Format(LIKE_TEMPLATE, this.GetColumnName(column)), value);
        }

        public NPocoExpression NotBetween(string column, object start, object end)
        {
            return new NPocoExpression(string.Format(NOT_BETWEEN_TEMPLATE, this.GetColumnName(column)), start, end);
        }

        public NPocoExpression Between(string column, object start, object end)
        {
            return new NPocoExpression(string.Format(BETWEEN_TEMPLATE, this.GetColumnName(column)), start, end);
        }

        public NPocoExpression NotIn(string column, List<object> values)
        {

            var inClause = new StringBuilder(string.Format(NOT_IN_TEMPLATE, this.GetColumnName(column)));
            inClause.Append(" (");
            inClause.Append(this.Repeat("?", COMMA, values.Count));
            inClause.Append(")");

            return new NPocoExpression(inClause.ToString(), values);
        }

        public NPocoExpression In(string column, List<object> values)
        {
            var inClause = new StringBuilder(string.Format(IN_TEMPLATE, this.GetColumnName(column)));
            inClause.Append(" (");
            inClause.Append(this.Repeat("?", COMMA, values.Count));
            inClause.Append(")");

            return new NPocoExpression(inClause.ToString(), values);
        }

        private NPocoExpression ComparePredicate(string columnCode, string op, object value)
        {
            var p = new List<object> { value };
            return new NPocoExpression(string.Format(COMPARE_TEMPLATE, this.GetColumnName(columnCode), op), p);
        }

        private string GetColumnName(string columnName)
        {
            string cName = null;

            if (OnColumnName != null)
                cName = OnColumnName(columnName);

            if (string.IsNullOrWhiteSpace(cName))
                return columnName;

            return cName;
        }

        private string Repeat(string s, string separator, int count)
        {
            var str = new StringBuilder((s.Length + separator.Length) * count);
            while (--count > 0)
                str.Append(s).Append(separator);

            return str.Append(s).ToString();
        }

        public NPocoExpression Build(string query)
        {
            return RestSQL<NPocoExpression>.Parse(query, this);
        }
    }
}
