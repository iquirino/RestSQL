lexer grammar RsqlLexer;

@header {
}

@members {
}


/*
===============================================================================
  Tokens for Case Insensitive Keywords
===============================================================================
*/
fragment A
	:	'A' | 'a';

fragment B
	:	'B' | 'b';

fragment C
	:	'C' | 'c';

fragment D
	:	'D' | 'd';

fragment E
	:	'E' | 'e';

fragment F
	:	'F' | 'f';

fragment G
	:	'G' | 'g';

fragment H
	:	'H' | 'h';

fragment I
	:	'I' | 'i';

fragment J
	:	'J' | 'j';

fragment K
	:	'K' | 'k';

fragment L
	:	'L' | 'l';

fragment M
	:	'M' | 'm';

fragment N
	:	'N' | 'n';

fragment O
	:	'O' | 'o';

fragment P
	:	'P' | 'p';

fragment Q
	:	'Q' | 'q';

fragment R
	:	'R' | 'r';

fragment S
	:	'S' | 's';

fragment T
	:	'T' | 't';

fragment U
	:	'U' | 'u';

fragment V
	:	'V' | 'v';

fragment W
	:	'W' | 'w';

fragment X
	:	'X' | 'x';

fragment Y
	:	'Y' | 'y';

fragment Z
	:	'Z' | 'z';

/*
===============================================================================
  Reserved Keywords
===============================================================================
*/

AND : A N D;
IN : I N;
IS : I S;
LIKE : L I K E;
NOT : N O T;
NULL : N U L L;
OR : O R;
BETWEEN : B E T W E E N;


// Operators
Similar_To : '~';
Not_Similar_To : '!~';
Similar_To_Case_Insensitive : '~*';
Not_Similar_To_Case_Insensitive : '!~*';

EQUAL  : '=';
COLON :  ':';
SEMI_COLON :  ';';
COMMA : ',';
NOT_EQUAL  : '<>' | '!=' | '~='| '^=' ;
LTH : '<' ;
LEQ : '<=';
GTH   : '>';
GEQ   : '>=';
LEFT_PAREN :  '(';
RIGHT_PAREN : ')';
PLUS  : '+';
MINUS : '-';
MULTIPLY: '*';
DIVIDE  : '/';
MODULAR : '%';
DOT : '.';
UNDERLINE : '_';
VERTICAL_BAR : '|';
QUOTE : '\'';
DOUBLE_QUOTE : '"';

NUMBER : Digit+;

fragment
Digit : '0'..'9';

REAL_NUMBER
    :   ('0'..'9')+ '.' ('0'..'9')* EXPONENT?
    |   '.' ('0'..'9')+ EXPONENT?
    |   ('0'..'9')+ EXPONENT
    ;

BlockComment
    :   '/*' .*? '*/' -> skip
    ;

LineComment
    :   '--' ~[\r\n]* -> skip
    ;

/*
===============================================================================
 Identifiers
===============================================================================
*/

Identifier
  : Regular_Identifier
  ;

fragment
Regular_Identifier
  : ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|Digit|'_')*
  ;

/*
===============================================================================
 Literal
===============================================================================
*/

// Some Unicode Character Ranges
fragment
Control_Characters                  :   '\u0001' .. '\u001F';
fragment
Extended_Control_Characters         :   '\u0080' .. '\u009F';

Character_String_Literal
  : QUOTE ( ESC_SEQ | ~('\\'|'\'') )* QUOTE
  ;

fragment
EXPONENT : ('e'|'E') ('+'|'-')? ('0'..'9')+ ;

fragment
HEX_DIGIT : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment
ESC_SEQ
    :   '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
    |   UNICODE_ESC
    |   OCTAL_ESC
    ;

fragment
OCTAL_ESC
    :   '\\' ('0'..'3') ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7')
    ;

fragment
UNICODE_ESC
    :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
    ;

/*
===============================================================================
 Whitespace Tokens
===============================================================================
*/

Space
  : ' ' -> skip
  ;

White_Space
  :	( Control_Characters  | Extended_Control_Characters )+ -> skip
  ;


BAD
  : . -> skip
  ;