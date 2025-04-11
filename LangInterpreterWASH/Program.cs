Lexer Lex = new(@"C:\Users\treys\Desktop\LangInterpreterWASH\test.wash");
Queue<Token> TokenQueue = Lex.Tokenize();
//Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Enviornment GlobalEnv = new();

Parser Parse = new(TokenQueue, GlobalEnv);
Queue<ASTNode> Roots = Parse.Parse();
Parse.DebugRoots(Roots);

Evaluator Eval = new(GlobalEnv);
Eval.StartEval(Roots);