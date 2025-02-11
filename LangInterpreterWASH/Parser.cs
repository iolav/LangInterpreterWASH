class ASTNode(string A, Token V, Token L, Token R) { // Node class to store things for the AST
    public string Action = A;

    public Token Value = V;
    public Token Left = L;
    public Token Right = R;
}