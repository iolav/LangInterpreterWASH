Lexer Lex = new(@"C:\Users\treys\Desktop\LangInterpreterWASH\test.wash");
Queue<Token> TokenQueue = Lex.Tokenize();
Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Parser Parse = new(TokenQueue);
Queue<ASTNode> Roots = Parse.Parse();
Parse.DebugRoots(Roots);

Enviornment GlobalEnv = new();

Evaluator Eval = new();
Eval.StartEval(Roots, GlobalEnv);

GlobalEnv.DebugAll();