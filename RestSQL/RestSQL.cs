using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestSQL
{
    public static class RestSQL<T>
    {
        public static T Parse(string query, IExpressionBuilder<T> expressionBuilder)
        {
            var lexer = new RsqlLexer(new AntlrInputStream(query));
            var tokenStream = new CommonTokenStream(lexer);
            var rsqlParser = new RsqlParser(tokenStream);
            var tree = rsqlParser.expression();
            var rsqlStatement = new RsqlExpressionVisitor<T>(expressionBuilder).Visit(tree);

            return rsqlStatement;
        }
    }
}
