# RestSQL

Port of: https://github.com/mmrath/rsql-parser

C# Sql Where Clause Parser to be used as Rest Parameters to filter data

This parser is strong using Visitor Pattern:
The IExpressionBuilder<T> should be implemented and then call to build:

    RestSQL<T>.Parse(query, this);

You can base on our test to implement your own strategy:

    var testExpressionBuilder = new TestExpressionBuilder();
    testExpressionBuilder.Build("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')");

Nuget Package: https://www.nuget.org/packages/RestSQL/

Implemented strategies:

  SqlKata.QueryBuilder

Nuget Package: https://www.nuget.org/packages/RestSQL.SqlKata/
  
	var t = new Query().From("tblLalala").Where("Name", "Igor").Where(c=>c.Where("status","1").OrWhere("status","2"));

	var compiler = new Oracle11gCompiler();
	var testExpressionBuilder = new SqlKataBuilder();

	var q = testExpressionBuilder.Build("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')");

	var qb = testExpressionBuilder.BuildFrom("ab > 'DE' and (c = d or d > 4.3) and e>4 or  rere in (20,30,40,50) and defer between 2 and 3 or rerer not in ('432','234324')", t);

	var sql = compiler.Compile(q.From("MyTable"));
	var sqlb = compiler.Compile(qb);
