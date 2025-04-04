Lexer Lex = new(@"C:\Users\treys\Desktop\LangInterpreterWASH\test.wash");
Queue<Token> TokenQueue = Lex.Tokenize();
//Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Parser Parse = new(TokenQueue);
Queue<ASTNode> Roots = Parse.Parse();
Parse.DebugRoots(Roots);

Evaluator Eval = new();
Variables Vars = new();
Eval.StartEval(Roots, Vars);
Vars.DebugAll();