class Evaluator() {
    public float Evaluate(ASTNode Node) { // Start at top node and recursivly evaluate each one
        if (Node.Action == "Number") {
            return float.Parse(Node.Value);
        } 

        else if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception("Internal Error - Left or right node null"); // Shouldnt ever get here

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

        else if (Node.Action == "Negate") {
            if (Node.Left == null)
                throw new Exception("Internal Error - Left node null on negation"); // Shouldnt ever get here

            return -Evaluate(Node.Left);
        }

        throw new Exception("Internal Error - Unknown node action"); // Shouldnt ever get here
    }
}