using ValuePair = (Type, object);

class Evaluator(Enviornment GE) {
    readonly private Enviornment GlobalEnv = GE;
    private Enviornment WorkingEnv = GE;

    private ValuePair EvalWithEnv(ASTNode Node, Enviornment Env) {
        Enviornment PreviousEnv = WorkingEnv;

        WorkingEnv = Env;

        ValuePair Evaluated = Evaluate(Node);

        WorkingEnv = PreviousEnv;

        return Evaluated;
    }

    public void StartEval(Queue<ASTNode> Roots) { // Public method to evaluate all root nodes
        while (Roots.Count > 0)
            Evaluate(Roots.Dequeue());
    } 

    private ValuePair Evaluate(ASTNode Node) { // Start at top node and recursivly evaluate each one
        switch (Node.Action) { // Handle static values
            case "Integer":
                return (Node.Action, int.Parse(Node.Value));
            case "Float":
                return (Node.Action, float.Parse(Node.Value));
            case "Identifier":
                if (WorkingEnv.Fetch(Node.Value, out ValuePair Value, out Enviornment? _))
                    return (Value.Item1, Value.Item2);
                else
                    throw new Exception(); // Refrence to non existant variable
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
            bool Condition = Node.Left == null || (bool)Evaluate(Node.Left).Item2;

            if (Condition && Node.Middle != null)
                Evaluate(Node.Middle);
            else if (Node.Right != null)
                Evaluate(Node.Right);
            
            return ("None", 0);
        }

        if (Node.Action == "Iterative") {
            if (Node.Left?.Right == null || Node.Middle == null || Node.Extra?.Env == null)
                throw new Exception(); // Shouldnt ever get here

            Enviornment BlockEnv = Node.Extra.Env;

            ValuePair IndexValue = (ValuePair)EvalWithEnv(Node.Left, BlockEnv).Item2;
            ValuePair IndexLimit = Evaluate(Node.Middle);

            int Index = (int)IndexValue.Item2;
            int Limit = (int)IndexLimit.Item2;
            int Change = Node.Right != null ? (int)Evaluate(Node.Right).Item2 : 1;

            while (Index <= Limit) {
                Evaluate(Node.Extra);

                Node.Left.Right.Value = (Index + Change).ToString();

                IndexValue = (ValuePair)EvalWithEnv(Node.Left, BlockEnv).Item2;

                Index = (int)IndexValue.Item2;
            }

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

        if (Node.Action == "Array") {
            List<ValuePair> Array = [];
            foreach (ASTNode ChildNode in Node.Collection)
                Array.Add(Evaluate(ChildNode));

            return (Node.Action, Array);
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
                    ">" => ("Boolean", LInt > RInt),
                    "<" => ("Boolean", LInt < RInt),
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

                    "==" => ("Boolean", L == R),
                    _ => throw new Exception() // Invalid op
                };
        }

        if (Node.Action == "Negate") {
            if (Node.Left == null)
                throw new Exception(); // Shouldnt ever get here

            object Value = Evaluate(Node.Left);

            return Value switch {
                int I => ("Integer", -I),
                float F => ("Float", -F),
                _ => throw new Exception() // Negating other than number
            };
        }

        if (Node.Action == "Assignment") {
            if (Node.Left == null || Node.Right == null)
                throw new Exception(); // Shouldnt ever get here

            ValuePair RightNode = Evaluate(Node.Right);
            
            bool Fetched = WorkingEnv.Fetch(Node.Left.Value, out ValuePair Value, out Enviornment? FoundEnv);
            bool HasType = Node.Left.Action != "Identifier";
            
            if (!HasType && !Fetched)
                throw new Exception(); // Missing type
            else if (Fetched && Value.Item1 != RightNode.Item1)
                throw new Exception(); // Type mismatch
            /*else if (Fetched && HasType)
                throw new Exception(); // Same defined in same env*/

            if (RightNode.Item1 == "Array") {
                string LeftAction = Node.Left.Action;

                if (!LeftAction.EndsWith("Array"))
                    throw new Exception(); // Array defined with not array type

                string Type = LeftAction.Replace("Array", "");
                
                foreach (ValuePair Element in (List<ValuePair>)RightNode.Item2)
                {
                    if (Element.Item1 != Type)
                        throw new Exception(); // Type mismatch in array
                }
            }

            Enviornment StorageEnv = FoundEnv ?? WorkingEnv;
            
            StorageEnv.Store(Node.Left.Value, RightNode);

            return ("Assignment", RightNode);
        }

        if (Node.Action == "Empty")
            return ("None", 0);

        throw new Exception(); // Can happen if no valid action
    }
}