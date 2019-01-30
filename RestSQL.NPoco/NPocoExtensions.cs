using RestSQL.NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPoco
{
    public static class NPocoExtensions
    {
        public static SqlBuilder Where(this SqlBuilder sql, string where)
        {
            var builder = new NPocoBuilder();
            var expression = builder.Build(where);
            return sql.Where(expression.Sql, expression.Params.ToArray());
        }
    }
}
