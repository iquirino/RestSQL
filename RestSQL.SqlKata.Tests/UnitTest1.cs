using SqlKata;
using SqlKata.Compilers;
using Xunit;

namespace RestSQL.SqlKata.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var t = new Query().From("tblLalala").Where("Name", "Igor").Where(c => c.Where("status", "1").OrWhere("status", "2"));

            var compiler = new Oracle11gCompiler();
            var testExpressionBuilder = new SqlKataBuilder();

            //testExpressionBuilder.OnColumnName += cname => cname + "_C";

            var q = testExpressionBuilder.Build("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')");

            var qb = testExpressionBuilder.BuildFrom("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')", t);

            var sql = compiler.Compile(q.From("MyTable"));
            var sqlb = compiler.Compile(qb);

            Assert.Equal("SELECT * FROM \"MyTable\" WHERE \"ab\" > ? AND (\"c\" = ? OR \"d\" > ?) AND \"e\" > ? OR \"rere\" IN (?, ?, ?, ?) OR \"defer\" BETWEEN ? AND ? OR \"rerer\" NOT IN (?, ?)", sql.RawSql);
            Assert.Equal("SELECT * FROM \"tblLalala\" WHERE \"Name\" = ? AND (\"status\" = ? OR \"status\" = ?) AND (\"ab\" > ? AND (\"c\" = ? OR \"d\" > ?) AND \"e\" > ? OR \"rere\" IN (?, ?, ?, ?) OR \"defer\" BETWEEN ? AND ? OR \"rerer\" NOT IN (?, ?))", sqlb.RawSql);
        }
    }
}
