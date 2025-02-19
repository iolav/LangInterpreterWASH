Lexer Lex = new("C:\\Users\\treys\\Desktop\\LangInterpreterWASH\\test.wash");
Queue<Token> TokenQueue = Lex.Tokenize();
Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Parser Parse = new(TokenQueue);
ASTNode RootNode = Parse.Parse();
Parse.DebugNodes(RootNode);

Evaluator Eval = new();
Variables Vars = new();
Eval.Evaluate(RootNode, Vars);
Vars.DebugAll();