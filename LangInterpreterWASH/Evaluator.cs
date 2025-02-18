class Evaluator() {
    public float Evaluate(ASTNode Node) { // Evaluation
        if (Node.Action == "Number") {
            return float.Parse(Node.Value);
        }
        
        if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception();

            float Left = Evaluate(Node.Left);
            float Right = Evaluate(Node.Right);

            if (Node.Value == "+")
                return Left + Right;
            else if (Node.Value == "-")
                return Left - Right;
            else if (Node.Value == "*")
                return Left * Right;
            else if (Node.Value == "/")
                return Left / Right;
        }

        if (Node.Action == "Negate") {
            if (Node.Left == null)
                throw new Exception();

            return -Evaluate(Node.Left);
        }

        throw new Exception();
    }
}