using System;
using System.Collections.Generic;
using System.Linq;

namespace RestSQL
{
    public class RsqlExpressionVisitor<T>
    {
        private readonly IExpressionBuilder<T> expressionBuilder;
        public RsqlExpressionVisitor(IExpressionBuilder<T> expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        public T Visit(RsqlParser.ExpressionContext context)
        {
            if (context.booleanValueExpression() != null)
                return this.VisitBooleanValueExpression(context.booleanValueExpression());
            return default;
        }

        private T VisitBooleanValueExpression(RsqlParser.BooleanValueExpressionContext booleanValueExpressionContext)
        {

            if (booleanValueExpressionContext.orPredicate() != null)
                return this.VisitOrPredicate(booleanValueExpressionContext.orPredicate());
            else
                throw new ArgumentNullException("Or predicate expected");
        }

        private T VisitOrPredicate(RsqlParser.OrPredicateContext orPredicateContext)
        {
            T andStmt = default;
            if (orPredicateContext.andPredicate() != null)
                andStmt = this.VisitAndStmt(orPredicateContext.andPredicate());

            T orStmt = default;
            if (orPredicateContext.orPredicate() != null && orPredicateContext.orPredicate().Any())
            {
                foreach (var orExp in orPredicateContext.orPredicate())
                    orStmt = this.expressionBuilder.Or(orStmt, this.VisitOrPredicate(orExp));
            }
            return this.expressionBuilder.Or(andStmt, orStmt);
        }

        private T VisitAndStmt(RsqlParser.AndPredicateContext andPredicateContext)
        {
            T primaryBooleanStmt = default;

            if (andPredicateContext.booleanPrimary() != null)
                primaryBooleanStmt = this.VisitPrimaryBooleanStmt(andPredicateContext.booleanPrimary());

            T andStmt = default;
            if (andPredicateContext.andPredicate() != null && andPredicateContext.andPredicate().Any())
            {
                foreach (var and in andPredicateContext.andPredicate())
                    andStmt = this.expressionBuilder.And(andStmt, this.VisitAndStmt(and));
            }
            return this.expressionBuilder.And(primaryBooleanStmt, andStmt);

        }

        private T VisitPrimaryBooleanStmt(RsqlParser.BooleanPrimaryContext booleanPrimaryContext)
        {
            if (booleanPrimaryContext.predicate() != null)
                return this.VisitPredicate(booleanPrimaryContext.predicate());
            else if (booleanPrimaryContext.booleanPredicand() != null)
                return this.VisitBooleanPredicand(booleanPrimaryContext.booleanPredicand());
            else
                return default;
        }

        private T VisitBooleanPredicand(RsqlParser.BooleanPredicandContext booleanPredicandContext)
        {
            if (booleanPredicandContext.parenthesizedBooleanValueExpression() != null &&
                    booleanPredicandContext.parenthesizedBooleanValueExpression().booleanValueExpression() != null)
                return this.expressionBuilder.Parenthesize(this.VisitBooleanValueExpression(
                        booleanPredicandContext.parenthesizedBooleanValueExpression().booleanValueExpression()));

            return default;
        }

        private T VisitPredicate(RsqlParser.PredicateContext predicate)
        {
            if (predicate.comparisonPredicate() != null)
                return this.VisitComparisionPredicate(predicate.comparisonPredicate());
            else if (predicate.betweenPredicate() != null)
                return this.VisitBetweenPredicate(predicate.betweenPredicate());
            else if (predicate.inPredicate() != null)
                return this.VisitInPredicate(predicate.inPredicate());
            else if (predicate.patternMatchingPredicate() != null)
                return this.VisitPatternMatchingPredicate(predicate.patternMatchingPredicate());
            else if (predicate.nullPredicate() != null)
                return this.VisitNullPredicate(predicate.nullPredicate());
            else
                throw new InvalidOperationException("Unknown predicate type in:" + predicate.GetText());
        }

        private T VisitNullPredicate(RsqlParser.NullPredicateContext nullPredicateContext)
        {
            string column = nullPredicateContext.columnName().Identifier().GetText();
            if (nullPredicateContext.NOT() != null)
                return this.expressionBuilder.IsNotNull(column);
            else
                return this.expressionBuilder.IsNull(column);
        }

        private T VisitPatternMatchingPredicate(RsqlParser.PatternMatchingPredicateContext patternMatchingPredicateContext)
        {
            string column = patternMatchingPredicateContext.columnName().Identifier().GetText();
            object value = valueFromStringLiteral(patternMatchingPredicateContext.Character_String_Literal().GetText());
            if (patternMatchingPredicateContext.patternMatcher().NOT() != null)
                return this.expressionBuilder.NotLike(column, value);
            else
                return this.expressionBuilder.Like(column, value);
        }

        private T VisitInPredicate(RsqlParser.InPredicateContext inPredicateContext)
        {
            string column = inPredicateContext.columnName().Identifier().GetText();
            var values = GetValues(inPredicateContext.inPredicateValue().inValueList().valueExpression());
            if (inPredicateContext.NOT() != null)
                return this.expressionBuilder.NotIn(column, values);
            else
                return this.expressionBuilder.In(column, values);
        }

        private T VisitBetweenPredicate(RsqlParser.BetweenPredicateContext betweenPredicateContext)
        {
            string column = betweenPredicateContext.columnName().Identifier().GetText();
            object start = this.GetValue(betweenPredicateContext.betweenBegin().valueExpression());
            object end = this.GetValue(betweenPredicateContext.betweenEnd().valueExpression());

            if (betweenPredicateContext.NOT() != null)
                return this.expressionBuilder.NotBetween(column, start, end);
            else
                return this.expressionBuilder.Between(column, start, end);
        }

        private T VisitComparisionPredicate(RsqlParser.ComparisonPredicateContext comparisonPredicateContext)
        {
            if (comparisonPredicateContext.comparisionOperator().EQUAL() != null)
            {
                return this.expressionBuilder.Equal(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else if (comparisonPredicateContext.comparisionOperator().NOT_EQUAL() != null)
            {
                return this.expressionBuilder.NotEqual(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else if (comparisonPredicateContext.comparisionOperator().LEQ() != null)
            {
                return this.expressionBuilder.LessOrEqual(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else if (comparisonPredicateContext.comparisionOperator().LTH() != null)
            {
                return this.expressionBuilder.LessThan(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else if (comparisonPredicateContext.comparisionOperator().GEQ() != null)
            {
                return this.expressionBuilder.GreaterOrEqual(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else if (comparisonPredicateContext.comparisionOperator().GTH() != null)
            {
                return this.expressionBuilder.GreaterThan(comparisonPredicateContext.left.Identifier().GetText(),
                        this.GetValue(comparisonPredicateContext.valueExpression()));
            }
            else
            {
                throw new InvalidOperationException("Comparision predicate not recognized:" + comparisonPredicateContext.GetText());
            }
        }

        private List<object> GetValues(RsqlParser.ValueExpressionContext[] valueExpressionContexts)
        {
            return GetValues(valueExpressionContexts.ToList());
        }

        private List<object> GetValues(List<RsqlParser.ValueExpressionContext> valueExpressionContexts)
        {
            var p = new List<object>();

            foreach (RsqlParser.ValueExpressionContext context in valueExpressionContexts)
                p.Add(this.GetValue(context));

            return p;
        }
        private object GetValue(RsqlParser.ValueExpressionContext valueExpressionContext)
        {
            object value;

            if (valueExpressionContext.Character_String_Literal() != null)
            {
                string literal = valueExpressionContext.Character_String_Literal().GetText();
                return this.valueFromStringLiteral(literal);
            }
            else if (valueExpressionContext.numericValueExpression() != null)
            {
                var numericValueExpressionContext = valueExpressionContext.numericValueExpression();
                if (numericValueExpressionContext.numericPrimary().NUMBER() != null)
                    value = long.Parse(numericValueExpressionContext.GetText());
                else
                    value = decimal.Parse(numericValueExpressionContext.GetText());
            }
            else if (valueExpressionContext.Identifier() != null)
            {
                value = valueExpressionContext.Identifier().GetText();
            }
            else
            {
                throw new InvalidOperationException("Value is illegal:" + valueExpressionContext.GetText());
            }
            return value;
        }

        private string valueFromStringLiteral(string literal)
        {
            literal = literal.Substring(1); // remove first quote
            literal = literal.Substring(0, literal.Length - 1); // remove last quote
            return literal;
        }
    }
}
