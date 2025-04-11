using ValuePair = (string, object);

class Evaluator(Enviornment GE) {
    private Enviornment GlobalEnv = GE;
    private Enviornment WorkingEnv = GE;

    public void StartEval(Queue<ASTNode> Roots) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue());
        }
    } 

    private ValuePair Evaluate(ASTNode Node) { // Start at top node and recursivly evaluate each one
        switch (Node.Action) { // Handle static values
            case "Integer":
                return (Node.Action, int.Parse(Node.Value));
            case "Float":
                return (Node.Action, float.Parse(Node.Value));
            case "Identifier" when WorkingEnv.Find(Node.Value):
                ValuePair Fetched = WorkingEnv.Fetch(Node.Value);
                return (Fetched.Item1, Fetched.Item2);
            case "String":
                return (Node.Action, Node.Value[1..^1]);
            case "Character":
                return (Node.Action, char.Parse(Node.Value[1..^1]));
            case "Boolean":
                return (Node.Action, Node.Value == "True");
            case "Byte":
                if (byte.TryParse(Node.Value, out byte Parsed))
                    return (Node.Action, Parsed);
                else
                    throw new Exception(); // Byte out of range
        }

        if (Node.Action == "Conditional") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            if ((bool)Evaluate(Node.Left).Item2)
                Evaluate(Node.Right);
            
            return ("None", 0);
        }

        if (Node.Action == "Block") {
            if (Node.Env != null)
                WorkingEnv = Node.Env;

            foreach (ASTNode ChildNode in Node.Collection)
            {
                Evaluate(ChildNode);
            }

            WorkingEnv = WorkingEnv.Parent ?? GlobalEnv;

            return ("None", 0);
        }

        if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            object Left = Evaluate(Node.Left).Item2;
            object Right = Evaluate(Node.Right).Item2;

            if (Left is string LStr && Right is string RStr) { // String operations
                return Node.Value switch {
                    "+" => ("String", LStr + RStr),
                    "==" => ("Boolean", LStr == RStr),
                    _ => throw new Exception() // Invalid op
                };
            }

            if (Left is bool LBool && Right is bool RBool) {
                return ("Boolean", Node.Value switch {
                    "and" => LBool && RBool,
                    "or" => LBool || RBool,
                    _ => throw new Exception() // Invalid op
                });
            }

            if ((Left is int or byte) && (Right is int or byte))
            {
                int LInt = Left is byte v1 ? v1 : (int)Left;
                int RInt = Right is byte v ? v : (int)Right;

                return Node.Value switch {
                    "+" => ("Integer", LInt + RInt),
                    "-" => ("Integer", LInt - RInt),
                    "*" => ("Integer", LInt * RInt),
                    "/" => ("Integer", LInt / RInt),
                    "==" => ("Boolean", LInt == RInt),
                    _ => throw new Exception() // Invalid op
                };
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

            object Value = Evaluate(Node.Left);

            return Value switch {
                int I => ("Integer", -I),
                float F => ("Float", -F),
                _ => throw new Exception() // Negating other than number
            };
        }

        else if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            ValuePair RightNode = Evaluate(Node.Right);
            
            if ((RightNode.Item1 != Node.Left.Action) && !WorkingEnv.Find(Node.Left.Value))
                throw new Exception(); // Type mismatch or missing type

            WorkingEnv.Store(Node.Left.Value, RightNode);

            return ("None", 0);
        }

        else if (Node.Action == "Empty")
            return ("None", 0);

        throw new Exception(); // Can happen if no valid action
    }
}