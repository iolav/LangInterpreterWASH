class Evaluator() {
    public void StartEval(Queue<ASTNode> Roots, Variables Vars) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue(), Vars);
        }
    } 

    private object Evaluate(ASTNode Node, Variables Vars) { // Start at top node and recursivly evaluate each one
        if (Node.Action == "Integer") {
            return int.Parse(Node.Value);
        } else if (Node.Action == "Float") {
            return float.Parse(Node.Value);
        } else if (Node.Action == "Variable" && Vars.Find(Node.Value)) {
            return Vars.Fetch(Node.Value);
        }

        else if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            object Left = Evaluate(Node.Left, Vars);
            object Right = Evaluate(Node.Right, Vars);

            if (Left is int LInt && Right is int RInt) {
                return Node.Value switch {
                    "+" => LInt + RInt,
                    "-" => LInt - RInt,
                    "*" => LInt * RInt,
                    "/" => LInt / RInt,
                    _ => throw new Exception() // Invalid op
                };
            }

            float L = Convert.ToSingle(Left);
            float R = Convert.ToSingle(Right);

            return Node.Value switch {
                "+" => L + R,
                "-" => L - R,
                "*" => L * R,
                "/" => L / R,
                _ => throw new Exception() // Invalid op
            };
        }

        else if (Node.Action == "Negate") {
            if (Node.Left == null)
                throw new Exception(); // Shouldnt ever get here

            object value = Evaluate(Node.Left, Vars);

            return value switch {
                int I => -I,
                float F => -F,
                _ => throw new Exception() // Negating other than number
            };
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