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
    readonly private Dictionary<string, int> Precedence = new() {
        {"/", 7},
        {"*", 6},
        {"+", 5},
        {"-", 4},
        {">", 3}, {"<", 3}, {"==", 3},
        {"and", 2},
        {"or", 1}
    };

    readonly private Enviornment GlobalEnv = GE;
    private Enviornment WorkingEnv = GE;
    
    public Queue<ASTNode> Parse() { // Public method to invoke parsing    
        Queue<ASTNode> Roots = [];

        while (TokenQueue.Count > 0) {
            Roots.Enqueue(Statement(Roots.Count > 0 ? Roots.Peek() : null));
        }

        return Roots;
    }

    private void CheckExpected(string Expected) {
        if (Dequeue().Value != Expected)
            throw new Exception();
    }

    private ASTNode Statement(ASTNode? PreviousRoot) {
        Token Next = Peek();

        if (Next.Classifier == "Conditional")
            if (Next.Value == "else")
                return Conditional(PreviousRoot);
            else
                return Conditional(PreviousRoot);
        else
            return Assignment();
    }

    private ASTNode Expression(int MinPrecedence = 0) { // Handle expressions
        ASTNode Node = Factor();

        while (TokenQueue.Count > 0 && Precedence.ContainsKey(Peek().Value)
            && Precedence[Peek().Value] >= MinPrecedence) {
            
            Token Op = Dequeue();
            int Priority = Precedence[Op.Value];

            ASTNode RightNode = Expression(Priority + 1);

            Node = new(Op.Classifier, Op.Value, Node, RightNode);
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

    private ASTNode Conditional(ASTNode? PreviousRoot) {
        Token Keyword = Dequeue();
        ASTNode Condition;

        if (Keyword.Value == "else" && PreviousRoot != null && PreviousRoot.Left != null) {
            Condition = PreviousRoot.Left;
        } else {
            Condition = Expression();

            CheckExpected("then"); 
        }

        CheckExpected("{");

        Enviornment LocalEnv = new(WorkingEnv);
        WorkingEnv = LocalEnv;

        ASTNode Block = new(WorkingEnv);
        
        while (TokenQueue.Count > 0 && Peek().Value != "}")
            Block.Collection.Add(Statement(PreviousRoot));

        CheckExpected("}");

        WorkingEnv = LocalEnv.Parent ?? GlobalEnv;

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
