using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

class ASTNode(string A, string V, ASTNode? L, ASTNode? R) { // Node class to store things for the AST
    public string Action = A;
    public string Value = V;
    public ASTNode? Left = L;
    public ASTNode? Right = R;
}

class Parser(Queue<Token> TokenQueue) {
    public ASTNode Parse() {
        return Expression();
    }

    private ASTNode Expression() {
        ASTNode Node = Term();

        while (Peek() == "Plus" || Peek() == "Minus") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Term());
        }

        return Node;
    }

    private ASTNode Term() {
        ASTNode Node = Factor();

        while (Peek() == "Multiply" || Peek() == "Divide") {
            Token Operator = Dequeue();
            Node = new ASTNode(Operator.Classifier, Operator.Value, Node, Factor());
        }
        
        return Node;
    }

    private ASTNode Factor() {
        if (Peek() == "Number")
            return new ASTNode("Number", Dequeue().Value, null, null);
        else if (Peek() == "LParen") {
            Dequeue();

            ASTNode Node = Expression();

            if (Peek() != "RParen")
                throw new Exception();
            
            Dequeue();

            return Node;
        }

        throw new Exception();
    }

    private string? Peek() {
        if (TokenQueue.Count > 0)
            return TokenQueue.Peek().Classifier;

        return null;
    }

    private Token Dequeue() {
        if (TokenQueue.Count > 0)
            return TokenQueue.Dequeue();
        
        throw new Exception();
    }
}
