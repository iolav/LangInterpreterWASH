class Evaluator() {
    public void StartEval(Queue<ASTNode> Roots, Variables Vars) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue(), Vars);
        }
    } 

    private (string, object) Evaluate(ASTNode Node, Variables Vars) { // Start at top node and recursivly evaluate each one
        if (Node.Action == "Integer") {
            return (Node.Action, int.Parse(Node.Value));
        } else if (Node.Action == "Float") {
            return (Node.Action, float.Parse(Node.Value));
        } else if (Node.Action == "Variable" && Vars.Find(Node.Value)) {
            return (Node.Action, Vars.Fetch(Node.Value));
        }

        else if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            object Left = Evaluate(Node.Left, Vars).Item2;
            object Right = Evaluate(Node.Right, Vars).Item2;

            if (Left is int LInt && Right is int RInt) {
                return Node.Value switch {
                    "+" => ("Integer", LInt + RInt),
                    "-" => ("Integer", LInt - RInt),
                    "*" => ("Integer", LInt * RInt),
                    "/" => ("Integer", LInt / RInt),
                    _ => throw new Exception() // Invalid op
                };
            }

            float L = Convert.ToSingle(Left);
            float R = Convert.ToSingle(Right);

            return Node.Value switch {
                "+" => ("Float", L + R),
                "-" => ("Float", L - R),
                "*" => ("Float", L * R),
                "/" => ("Float", L / R),
                _ => throw new Exception() // Invalid op
            };
        }

        else if (Node.Action == "Negate") {
            if (Node.Left == null)
                throw new Exception(); // Shouldnt ever get here

            object Value = Evaluate(Node.Left, Vars);

            return Value switch {
                int I => ("Integer", -I),
                float F => ("Float", -F),
                _ => throw new Exception() // Negating other than number
            };
        }

        else if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            Vars.Store(Node.Left.Value, Evaluate(Node.Right, Vars));

            return ("None", 0);
        }

        else if (Node.Action == "Empty" || Node.Action == "Program") {
            return ("None", 0);
        }

        throw new Exception(); // Can happen if previous statements fail or no invalid Action
    }
}