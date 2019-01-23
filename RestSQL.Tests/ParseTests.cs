using Newtonsoft.Json;
using RestQL.Tests.Providers;
using System;
using Xunit;

namespace RestSQL.Tests
{
    public class ParseTests
    {
        [Fact]
        public void Test1()
        {
            var testExpressionBuilder = new TestExpressionBuilder();
            var p = RestSQL<TestExpression>.Parse("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')", testExpressionBuilder);
            //var gIgor = new FilterGroup();
            //gIgor.Add(new FilterNode("igor1", ComparisonOperator.Equal, LogicalOperator.Undefined, "gato1"));
            //gIgor.Add(new FilterNode("igor2", ComparisonOperator.Equal, LogicalOperator.And, "gato2"));
            //gIgor.Add(new FilterNode("igor3", ComparisonOperator.Equal, LogicalOperator.And, "gato3"));
            //gIgor.Add(new FilterNode("igor4", ComparisonOperator.Equal, LogicalOperator.And, "gato4"));

            //var root = new FilterGroup();
            //root.Add(new FilterNode("caio1", ComparisonOperator.Equal, LogicalOperator.Undefined, "faio1"));
            //root.Add(new FilterNode("caio2", ComparisonOperator.Equal, LogicalOperator.And, "faio2"));
            //root.Add(new FilterNode("caio3", ComparisonOperator.Equal, LogicalOperator.And, "faio3"));
            //root.Add(new FilterNode("caio4", ComparisonOperator.Equal, LogicalOperator.And, "faio4"));
            //root.Add(gIgor);


            //string json = JsonConvert.SerializeObject(root, Formatting.Indented);
        }
    }
}
