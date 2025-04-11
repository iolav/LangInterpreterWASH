using System.Net.Security;

Lexer Lex = new(@"E:\Projects\LangInterpreterWASH\test.wash");
Queue<Token> TokenQueue = Lex.Tokenize();
//Lex.DebugTQ(TokenQueue);

Console.Write("\n");

Enviornment GlobalEnv = new();

Parser Parse = new(TokenQueue, GlobalEnv);
Queue<ASTNode> Roots = Parse.Parse();
Parse.DebugRoots(Roots);

Evaluator Eval = new();
Eval.StartEval(Roots, GlobalEnv);

GlobalEnv.DebugAll();