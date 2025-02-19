class ASTNode(string A, string V, ASTNode? L, ASTNode? R) { // Node class to store things for the AST
    public string Action = A;
    public string Value = V;
    public ASTNode? Left = L;
    public ASTNode? Right = R;
}

class Parser(Queue<Token> TokenQueue) {
    public ASTNode Parse() { // Public method to invoke parsing
        return Assignment();
    }

    private ASTNode Expression() { // Handle addition and subtraction
        ASTNode Node = Term();

        while (Peek().Value == "+" || Peek().Value == "-") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Term());
        }

        return Node;
    }

    private ASTNode Term() { // Handle multiplication and division
        ASTNode Node = Factor();

        while (Peek().Value == "*" || Peek().Value == "/") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Factor());
        }
        
        return Node;
    }

    private ASTNode Factor() { // Handle numbers and parenthesis 
        if (Peek().Value == "-") {
            Dequeue();
            
            if (Peek().Value == "(") {
                Dequeue();

                ASTNode Node = Expression();

                if (Peek().Value != ")")
                    throw new Exception("Syntax Error - Missing closing parenthesis"); // Test case: "-(1 + 1"
                
                Dequeue();

                return new ASTNode("Negate", "-", Node, null);
            }

            throw new Exception("Syntax Error - Expected parenthesis or number after negative"); // Test case: "-"
        }

        if (Peek().Classifier == "Number")
            return new ASTNode("Number", Dequeue().Value, null, null);
        else if (Peek().Value == "(") {
            Dequeue();

            ASTNode Node = Expression();

            if (Peek().Value != ")")
                throw new Exception("Syntax Error - Missing closing parenthesis"); // Test case: "(1 + 1"
            
            Dequeue();

            return Node;
        } else if (Peek().Classifier == "Identifier")
            return new ASTNode("Variable", Dequeue().Value, null, null);

        throw new Exception("Internal Error - Unexpected input for factor"); // Shouldnt ever get here
    }

    private ASTNode Assignment() {
        ASTNode Node = Expression();

        if (Peek().Value == "=") {
            Token AssignToken = Dequeue();

            if (Node.Action != "Variable")
                throw new Exception("Syntax Error - No identifier found for assignment");

            ASTNode RightHandSide = Expression();

            return new ASTNode("Assignment", AssignToken.Value, Node, RightHandSide);
        }

        return Node;
    }

    private Token Peek() {
        if (TokenQueue.Count > 0)
            return TokenQueue.Peek();

        return new Token("null", "null");
    }

    private Token Dequeue() {
        if (TokenQueue.Count > 0)
            return TokenQueue.Dequeue();
        
        throw new Exception("Internal Error - Tried to pop next token in empty queue"); // Shouldnt ever get here
    }

    public void DebugNodes(ASTNode Node, int Indent = 0) {
        Console.WriteLine($"{new string(' ', Indent)} {Node.Action} {Node.Value}");

        if (Node.Left != null)
            DebugNodes(Node.Left, Indent + 4);
        if (Node.Right != null)
            DebugNodes(Node.Right, Indent + 4);
    }
}
