using System.Collections;

Lexer Lex = new();
Queue<Token> TokenQueue = Lex.Tokenize("-(5 - 1) + 2");
Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Parser Parse = new(TokenQueue);
ASTNode RootNode = Parse.Parse();
Parse.DebugNodes(RootNode);

Evaluator Eval = new();
float Result = Eval.Evaluate(RootNode);
Console.WriteLine($"\n{Result}");