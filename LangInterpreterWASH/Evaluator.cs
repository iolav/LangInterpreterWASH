using ValuePair = (string, object);

class Evaluator() {
    public void StartEval(Queue<ASTNode> Roots, Enviornment Env) { // Public method to evaluate all root nodes
        while (Roots.Count > 0) {
            Evaluate(Roots.Dequeue(), Env);
        }
    } 

    private ValuePair Evaluate(ASTNode Node, Enviornment Env) { // Start at top node and recursivly evaluate each one
        switch (Node.Action) { // Handle static values
            case "Integer":
                return (Node.Action, int.Parse(Node.Value));
            case "Float":
                return (Node.Action, float.Parse(Node.Value));
            case "Identifier" when Env.Find(Node.Value):
                ValuePair Fetched = Env.Fetch(Node.Value);
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

            if ((bool)Evaluate(Node.Left, Env).Item2)
                Evaluate(Node.Right, Env);
            
            return ("None", 0);
        }

        if (Node.Action == "Block") {
            foreach (ASTNode ChildNode in Node.Collection)
            {
                Evaluate(ChildNode, Env);
            }

            return ("None", 0);
        }

        if (Node.Action == "Operator") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            object Left = Evaluate(Node.Left, Env).Item2;
            object Right = Evaluate(Node.Right, Env).Item2;

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

            object Value = Evaluate(Node.Left, Env);

            return Value switch {
                int I => ("Integer", -I),
                float F => ("Float", -F),
                _ => throw new Exception() // Negating other than number
            };
        }

        else if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            ValuePair RightNode = Evaluate(Node.Right, Env);
            
            if ((RightNode.Item1 != Node.Left.Action) && !Env.Find(Node.Left.Value))
                throw new Exception(); // Type mismatch or missing type

            Env.Store(Node.Left.Value, RightNode);

            return ("None", 0);
        }

        else if (Node.Action == "Empty")
            return ("None", 0);

        throw new Exception(); // Can happen if no valid action
    }
}