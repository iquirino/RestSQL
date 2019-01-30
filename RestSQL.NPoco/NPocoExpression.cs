using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestSQL.NPoco
{
    public class NPocoExpression
    {
        internal string rawSql;
        internal IReadOnlyCollection<object> parameters;

        public IReadOnlyCollection<object> Params
        {
            get
            {
                if (this.parameters == null || this.parameters.Count <= 0)
                    return this.parameters;

                return this.DoFlatParams(this.parameters).AsReadOnly();
            }
        }

        public string Sql
        {
            get
            {
                var fParams = this.Params;
                string[] rSql = this.rawSql.Split('?');

                var sbSql = new StringBuilder();

                for (int i = 0; i < rSql.Length; i++)
                {
                    sbSql.Append(rSql[i]);
                    if (i < fParams.Count)
                        sbSql.Append($"@{i}");
                }

                return sbSql.ToString();
            }
        }

        private List<object> DoFlatParams(IEnumerable<object> items)
        {
            var ret = new List<object>();
            foreach (object inner in items)
            {
                if (inner is IReadOnlyCollection<object> col)
                    ret.AddRange(this.DoFlatParams(col));
                else
                    ret.Add(inner);
            }
            return ret;
        }

        public NPocoExpression(string whereClause, List<object> parameters)
        {
            this.rawSql = whereClause;
            this.parameters = parameters.AsReadOnly();
        }

        public NPocoExpression(string whereClause, params object[] parameters) : this(whereClause, parameters?.ToList())
        {
        }

        public override string ToString()
        {
            return "NPocoExpression {" + " where = '" + this.rawSql + '\'' + ", params = [" + string.Join(", ", this.parameters.ToArray()) + "]}";
        }
    }
}
