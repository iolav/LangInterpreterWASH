class Evaluator() {
    public float Evaluate(ASTNode Node, Variables Vars) { // Start at top node and recursivly evaluate each one
        if (Node.Action == "Number") {
            return float.Parse(Node.Value);
        } 

        else if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception("Internal Error - Left or right node null for operation"); // Shouldnt ever get here

            float Left = Evaluate(Node.Left, Vars);
            float Right = Evaluate(Node.Right, Vars);

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

            return -Evaluate(Node.Left, Vars);
        }

        else if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception("Internal Error - Left or right node null for assignment"); // Shouldnt ever get here

            Vars.Store(Node.Left.Value, Evaluate(Node.Right, Vars));

            return 0f;
        }

        throw new Exception("Internal Error - Unknown node action"); // Sometimes gets here, will fix later
    }
}