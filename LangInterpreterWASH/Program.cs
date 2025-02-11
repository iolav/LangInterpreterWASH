using System.Collections;

Lexer Lex = new(@"C:\Users\treys\Desktop\LangInterpreterWASH\LangInterpreterWASH\test.wash");

Queue TokenQueue = Lex.Tokenize();

Lex.DebugTQ(TokenQueue);