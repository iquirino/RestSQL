using System.Collections.Generic;
using System.Linq;

namespace RestQL.Tests.Providers
{
    public class TestExpression
    {
        private readonly string whereClause;
        private readonly IReadOnlyCollection<object> p;

        public TestExpression(string whereClause, List<object> parameters)
        {
            this.whereClause = whereClause;
            this.p = parameters.AsReadOnly();
        }

        public TestExpression(string whereClause, params object[] parameters) : this(whereClause, parameters?.ToList())
        {
        }

        public string toString()
        {
            return "TestExpression{" +
                    "whereClause='" + this.whereClause + '\'' +
                    ", params=" + this.p +
                    '}';
        }

        public string getWhereClause()
        {
            return this.whereClause;
        }

        public List<object> getParams()
        {
            return this.p.ToList();
        }
    }
}
