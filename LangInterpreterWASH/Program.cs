using System.Collections;

Lexer Lex = new(@"\test.wash");

Queue TokenQueue = Lex.Tokenize();
Lex.DebugTQ(TokenQueue);