parser grammar RsqlParser;

options
    {
        language=csharp;
        tokenVocab = RsqlLexer;
    }


expression:booleanValueExpression;

booleanValueExpression
  : orPredicate
  ;

orPredicate
  : andPredicate (OR orPredicate)*
  ;

andPredicate
  : booleanPrimary (AND andPredicate)*
  ;

booleanPrimary
  : predicate
  | booleanPredicand
  ;

booleanPredicand
  : parenthesizedBooleanValueExpression
  ;

parenthesizedBooleanValueExpression
  : LEFT_PAREN booleanValueExpression RIGHT_PAREN
  ;

predicate
  : comparisonPredicate
  | betweenPredicate
  | inPredicate
  | patternMatchingPredicate // like predicate and other similar predicates
  | nullPredicate
  ;

comparisonPredicate
  : left=columnName c=comparisionOperator right=valueExpression
  ;

comparisionOperator
  : EQUAL
  | NOT_EQUAL
  | LTH
  | LEQ
  | GTH
  | GEQ
  ;

betweenPredicate
  : predicand=columnName (NOT)? BETWEEN betweenBegin AND betweenEnd
  ;

betweenBegin
  :valueExpression
  ;

betweenEnd
  :valueExpression
  ;

inPredicate
  : predicand=columnName  NOT? IN inPredicateValue
  ;

inPredicateValue
  : LEFT_PAREN inValueList RIGHT_PAREN
  ;

inValueList
  : valueExpression  (COMMA valueExpression)*
  ;

patternMatchingPredicate
  : f=columnName patternMatcher s=Character_String_Literal
  ;

patternMatcher
  : NOT? LIKE
  ;

nullPredicate
  : predicand=columnName IS (n=NOT)? NULL
  ;

valueExpression
  : Character_String_Literal
  | numericValueExpression
  | Identifier
  ;

numericValueExpression
  : (sign)? numericPrimary
  ;

numericPrimary
  : NUMBER
  | REAL_NUMBER
  ;

sign
  : PLUS | MINUS
  ;

columnName
  :Identifier
  ;