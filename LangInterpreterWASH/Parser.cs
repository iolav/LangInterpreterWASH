class ASTNode(string A, string V, ASTNode? L, ASTNode? R) { // Node class to store things for the AST
    public string Action = A;
    public string Value = V;
    public ASTNode? Left = L;
    public ASTNode? Right = R;
}

class Parser(Queue<Token> TokenQueue) {
    private ASTNode Statement() {
        return Assignment();
    }
    
    public ASTNode Parse() { // Public method to invoke parsing
        ASTNode? Root = null;
        ASTNode? Current = null;

        while (TokenQueue.Count > 0) {
            ASTNode NextStatement = Statement();

            if (Root == null) {
                Root = NextStatement;
                Current = Root;
            } else if (Current != null) {
                Current.Right = NextStatement;
                Current = NextStatement;
            }
        }

        return Root ?? new ASTNode("Empty", "", null, null);
    }

    private ASTNode Expression() { // Handle addition and subtraction
        ASTNode Node = Term();

        while (Peek().Value == "+" || Peek().Value == "-") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Term());
        }

        return Node;
    }

    private ASTNode Term() { // Handle term
        ASTNode Node = Factor();

        while (Peek().Value == "*" || Peek().Value == "/") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Factor());
        }
        
        return Node;
    }

    private ASTNode Factor() { // Handle factors
        if (Peek().Value == "-") {
            Dequeue();
            
            if (Peek().Value == "(") {
                Dequeue();

                ASTNode Node = Expression();

                if (Peek().Value != ")")
                    throw new Exception(); // Test case: "-(1 + 1"
                
                Dequeue();

                return new ASTNode("Negate", "-", Node, null);
            }

            throw new Exception(); // Test case: "-"
        }

        Token Next = Peek();
        if (Next.Classifier == "Float" || Next.Classifier == "Integer")
            return new ASTNode(Next.Classifier, Dequeue().Value, null, null);
        else if (Next.Value == "(") {
            Dequeue();

            ASTNode Node = Expression();

            if (Next.Value != ")")
                throw new Exception(); // Test case: "(1 + 1"
            
            Dequeue();

            return Node;
        } else if (Next.Classifier == "Identifier") 
            return new ASTNode("Variable", Dequeue().Value, null, null);
        else if (Next.Classifier == "Keyword")
            return new ASTNode("Keyword", Dequeue().Value, null, null);

        throw new Exception(); // Gets here if no input is provided
    }

    private ASTNode Assignment() { // Handle variable assignments
        ASTNode Node = Expression();

        if (Peek().Value == "=") {
            Token AssignToken = Dequeue();

            if (Node.Action != "Variable")
                throw new Exception();

            ASTNode RightHandSide = Expression();

            return new ASTNode("Assignment", AssignToken.Value, Node, RightHandSide);
        }

        return Node;
    }

    private Token Peek() { // DRY method for peeking
        if (TokenQueue.Count > 0)
            return TokenQueue.Peek();

        return new Token("null", "null");
    }

    private Token Dequeue() { // DRY method for dequeueing
        if (TokenQueue.Count > 0)
            return TokenQueue.Dequeue();
        
        throw new Exception(); // Shouldnt ever get here
    }

    public void DebugNodes(ASTNode Node, int Indent = 0) { // To help debug by printing out all nodes formatted`
        Console.WriteLine($"{new string(' ', Indent)} {Node.Action} {Node.Value}");

        if (Node.Left != null)
            DebugNodes(Node.Left, Indent + 4);
        if (Node.Right != null)
            DebugNodes(Node.Right, Indent + 4);
    }
}
