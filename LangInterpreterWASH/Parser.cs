class ASTNode { // Node class to store things for the AST
    public string Action;
    public string Value;
    public ASTNode? Left;
    public ASTNode? Right;
    public List<ASTNode> Collection = [];
    public Enviornment? Env;

    public ASTNode(string A, string V, ASTNode? L, ASTNode? R) { // For making a normal node
        Action = A;
        Value = V;
        Left = L;
        Right = R;

        Env = null;
    }

    public ASTNode(Enviornment E) { // For making a block node
        Action = "Block";
        Value = "";

        Env = E;
    }
}

class Parser(Queue<Token> TokenQueue, Enviornment GE) {
    readonly private string[] Expressions = ["+", "-", "and", "or", "=="];
    readonly private string[] Terms = ["*", "/"];

    private Enviornment GlobalEnv = GE;
    
    public Queue<ASTNode> Parse() { // Public method to invoke parsing    
        Queue<ASTNode> Roots = [];

        while (TokenQueue.Count > 0) {
            Roots.Enqueue(Statement());
        }

        return Roots;
    }

    private ASTNode Statement() {
        if (Peek().Classifier == "Conditional")
            return Conditional();
        else
            return Assignment();
    }

    private ASTNode Expression() { // Handle expressions
        ASTNode Node = Term();

        while (Expressions.Contains(Peek().Value)) {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Term());
        }

        return Node;
    }

    private ASTNode Term() { // Handle terms
        ASTNode Node = Factor();

        while (Terms.Contains(Peek().Value)) {
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

            if (Peek().Value != ")")
                throw new Exception(); // Test case: "(1 + 1"
            
            Dequeue();

            return Node;
        } else
            return new ASTNode(Next.Classifier, Dequeue().Value, null, null);

        throw new Exception(); // Gets here if no input is provided
    }

    private ASTNode Assignment() { // Handle variable assignments
        Token? TypeToken = null;
        if (Peek().Classifier == "Type") {
            TypeToken = Dequeue();
        }

        ASTNode Node = Expression();

        if (Peek().Classifier == "Assignment") {
            Token AssignToken = Dequeue();

            if (Node.Action != "Identifier")
                throw new Exception(); // Invalid identifier

            ASTNode RightHandSide = Expression();

            if (TypeToken != null) {
                Node.Action = TypeToken.Value;

                if (TypeToken.Value == "Byte" && RightHandSide.Action == "Integer")
                    RightHandSide.Action = "Byte";
            }

            return new ASTNode("Assignment", AssignToken.Value, Node, RightHandSide);
        }

        return Node;
    }

    private ASTNode Conditional() {
        Token Keyword = Dequeue();

        ASTNode Condition = Expression();

        Dequeue(); // Consume opening brace

        Enviornment LocalEnv = new(GlobalEnv);
        ASTNode Block = new(LocalEnv);
        
        while (TokenQueue.Count > 0 && Peek().Value != "}")
            Block.Collection.Add(Statement());

        Dequeue(); // Consume closing brace

        return new ASTNode("Conditional", Keyword.Value, Condition, Block);
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

    public void DebugRoots(Queue<ASTNode> Roots) { // To help debug by printing out all nodes formatted
        Queue<ASTNode> RootsCopy = new(Roots);
        while (RootsCopy.Count > 0) {
            DebugNodes(RootsCopy.Dequeue());
        }
    }
    private void DebugNodes(ASTNode Node, int Indent = 0) {
        Console.WriteLine($"{new string(' ', Indent)} {Node.Action} {Node.Value}");

        if (Node.Left != null)
            DebugNodes(Node.Left, Indent + 4);
        if (Node.Right != null)
            DebugNodes(Node.Right, Indent + 4);
        if (Node.Collection.Count > 0) {
            foreach (ASTNode ChildNode in Node.Collection)
            {
                DebugNodes(ChildNode, Indent + 4);
            }
        }
    }
}
