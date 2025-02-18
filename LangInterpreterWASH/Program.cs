using System.Collections;

Lexer Lex = new();
Queue<Token> TokenQueue = Lex.Tokenize("-(-(3 - (-(5 + 2)) + (-4 - (-1))) + (-(-6 + 2) - 3))");
Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Parser Parse = new(TokenQueue);
ASTNode RootNode = Parse.Parse();
Parse.DebugNodes(RootNode);

Evaluator Eval = new();
float Result = Eval.Evaluate(RootNode);
Console.WriteLine($"\n{Result}");