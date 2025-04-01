class Evaluator() {
    public void StartEval(Queue<ASTNode> Roots, Variables Vars) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue(), Vars);
        }
    } 

    private (string, object) Evaluate(ASTNode Node, Variables Vars) { // Start at top node and recursivly evaluate each one
        switch (Node.Action) { // Handle static values
            case "Integer":
                return (Node.Action, int.Parse(Node.Value));
            case "Float":
                return (Node.Action, float.Parse(Node.Value));
            case "Identifier" when Vars.Find(Node.Value):
                return (Node.Action, Vars.Fetch(Node.Value));
            case "String":
                return (Node.Action, Node.Value[1..^1]);
            case "Character":
                return (Node.Action, char.Parse(Node.Value[1..^1]));
            case "Boolean":
                return (Node.Action, Node.Value == "True");
        }

        if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            object Left = Evaluate(Node.Left, Vars).Item2;
            object Right = Evaluate(Node.Right, Vars).Item2;

            if (Left is string LStr && Right is string RStr) { // String operations
                return ("String", Node.Value switch {
                    "+" => LStr + RStr,
                    _ => throw new Exception() // Invalid op
                });
            }

            if (Left is bool LBool && Right is bool RBool) {
                return ("Boolean", Node.Value switch {
                    "and" => LBool && RBool,
                    _ => throw new Exception() // Invalid op
                });
            }

            if (Left is int LInt && Right is int RInt) {
                return ("Integer", Node.Value switch { // Integer operations
                    "+" => LInt + RInt,
                    "-" => LInt - RInt,
                    "*" => LInt * RInt,
                    "/" => LInt / RInt,
                    _ => throw new Exception() // Invalid op
                });
            }

            float L = Convert.ToSingle(Left);
            float R = Convert.ToSingle(Right);

            return ("Float", Node.Value switch { // Float operations
                "+" => L + R,
                "-" => L - R,
                "*" => L * R,
                "/" => L / R,
                _ => throw new Exception() // Invalid op
            });
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

        throw new Exception(); // Can happen if previous statements fail or no valid action (most likley)
    }
}