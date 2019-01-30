using NPoco;
using System;
using Xunit;

namespace RestSQL.NPoco.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var testExpressionBuilder = new NPocoBuilder();

            var q = testExpressionBuilder.Build("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')");

            Assert.Equal("ab > @0 AND  ( c = @1 OR d > @2 )  AND e > @3 OR rere IN (@4,@5,@6,@7) AND defer BETWEEN @8 AND @9 OR rerer NOT IN (@10,@11)", q.Sql);
            Assert.Equal("DE, d, 4.3, 4, 20, 30, 40, 50, 2, 3, 432, 234324", string.Join(", ", q.Params));

            var s = new SqlBuilder();
            s.Where("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')");
        }
    }
}
