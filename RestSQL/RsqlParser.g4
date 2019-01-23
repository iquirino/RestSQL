/*
 Licensed to the Apache Software Foundation (ASF) under one
 or more contributor license agreements.  See the NOTICE file
 distributed with this work for additional information
 regarding copyright ownership.  The ASF licenses this file
 to you under the Apache License, Version 2.0 (the
 "License"); you may not use this file except in compliance
 with the License.  You may obtain a copy of the License at
     http://www.apache.org/licenses/LICENSE-2.0
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 //Copied from https://github.com/camertron/SQLParser
 //Copied from https://github.com/mmrath/rsql-parser
 */
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