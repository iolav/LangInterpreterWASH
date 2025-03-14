class Evaluator() {
    public void StartEval(Queue<ASTNode> Roots, Variables Vars) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue(), Vars);
        }
    } 

    private double Evaluate(ASTNode Node, Variables Vars) { // Start at top node and recursivly evaluate each one
        if (Node.Action == "Integer") {
            return int.Parse(Node.Value);
        } else if (Node.Action == "Float") {
            return float.Parse(Node.Value);
        } else if (Node.Action == "Variable" && Vars.Find(Node.Value)) {
            return (double)Vars.Fetch(Node.Value);
        }

        else if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            double Left = Evaluate(Node.Left, Vars);
            double Right = Evaluate(Node.Right, Vars);

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
                throw new Exception(); // Shouldnt ever get here

            return -Evaluate(Node.Left, Vars);
        }

        else if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            Vars.Store(Node.Left.Value, Evaluate(Node.Right, Vars));

            return 0;
        }

        else if (Node.Action == "Empty" || Node.Action == "Program") {
            return 0;
        }

        throw new Exception(); // Can happen if previous statements fail or no invalid Action
    }
}