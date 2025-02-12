using System.Collections;

Lexer Lex = new();

Queue TokenQueue = Lex.Tokenize("13.5 + 2.65");

Lex.DebugTQ(TokenQueue);