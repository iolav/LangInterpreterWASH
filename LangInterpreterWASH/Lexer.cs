class Token(string C, string V) // To store a tokens value and associating classifier
{
    public readonly string Classifier = C;
    public readonly string Value = V;

    public override string ToString() {
        return $"Token:\n  : {Value}\n  : {Classifier}";
    }
}

class Tokenizer {
    readonly private string[] Operators = ["+", "-", "*", "/", "and", "or"];
    readonly private string[] Keywords = [];
    readonly private string[] Types = ["int", "float", "bool", "string"];
    
    private bool CheckUnary(char RawChar, string Data, int Pos) { // Check if a negative sign is unary or not
        bool IsHyphen = RawChar == '-';

        bool HasNextIsInt = Pos + 1 < Data.Length && int.TryParse(Data[Pos + 1].ToString(), out _);

        bool HasPrevious = Pos - 1 > -1;
        bool PrevIsOperator = HasPrevious && Operators.Contains(Data[Pos - 1].ToString());
        bool PrevIsOpenParen = HasPrevious && Data[Pos - 1] == '(';

        return IsHyphen && HasNextIsInt && (Pos == 0 || PrevIsOperator || PrevIsOpenParen || char.IsWhiteSpace(Data[Pos - 1]));
    }

    public Queue<Token> Process(string Data) { // Tokenize all data
        Queue<Token> TokenQueue = new();

        int Len = Data.Length;
        int Pos = 0;

        while (Pos < Len) {
            char RawChar = Data[Pos];
            string Segment = RawChar.ToString();

            if (Pos + 1 < Len && (Segment + Data[Pos + 1]).Equals("//")) { // Handle single-line comments
                while (Pos < Len && Data[Pos] != '\n')
                    Pos++;

                continue;
            }
            if (Pos + 1 < Len && (Segment + Data[Pos + 1]).Equals("/*")) { // Handle multi-line comments
                while (Pos < Len && Pos + 1 < Len && !Data[Pos..(Pos + 2)].Equals("*/"))
                    Pos++;
                Pos += 2;

                continue;
            }

            if (string.IsNullOrWhiteSpace(Segment)) { // Ignore whitespace
                Pos++; continue;
            }

            bool IsInt = int.TryParse(RawChar.ToString(), out _);
            if (IsInt || CheckUnary(RawChar, Data, Pos)) { // Handle numbers
                int Start = Pos;
                bool Deci = false;
                bool IsNeg = RawChar == '-';

                if (IsNeg)
                    Pos++;

                while (Pos < Len && (int.TryParse(Data[Pos].ToString(), out _) || Data[Pos] == '.')) {
                    if (Data[Pos] == '.') {
                        if (Deci) break;
                        Deci = true;
                    }
                    Pos++;
                }

                string Type = Deci ? "Float" : "Integer";
                TokenQueue.Enqueue(new Token(Type, Data[Start .. Pos]));

                continue;
            }

            if (RawChar == '\"') { // Handle strings
                int Start = Pos;

                Pos++;
                while (Pos < Len && Data[Pos] != '\"')
                    Pos++;
                Pos++;

                string SubSeg = Data[Start .. Pos];

                TokenQueue.Enqueue(new Token("String", SubSeg));

                continue;
            }
            if (RawChar == '\'') { // Handle characters
                string SubSeg = Data[Pos .. (Pos + 3)];

                if (SubSeg[2] != '\'')
                    throw new Exception(); // Char with more than one char (Ie. 'ab')

                TokenQueue.Enqueue(new Token("Character", SubSeg));

                Pos += 3;

                continue;
            }

            if (char.IsLetter(RawChar)) { // Handle letters and keywords
                int Start = Pos;

                while (Pos < Len && (char.IsLetter(Data[Pos]) || int.TryParse(Data[Pos].ToString(), out _)))
                    Pos++;
                
                string SubSeg = Data[Start .. Pos];
                if (Keywords.Contains(SubSeg))
                    TokenQueue.Enqueue(new Token("Keyword", SubSeg));
                else if (SubSeg == "True" || SubSeg == "False")
                    TokenQueue.Enqueue(new Token("Boolean", SubSeg));
                else if (Operators.Contains(SubSeg))
                    TokenQueue.Enqueue(new Token("Operator", SubSeg));
                else if (Types.Contains(SubSeg))
                    TokenQueue.Enqueue(new Token("Type", SubSeg));
                else
                    TokenQueue.Enqueue(new Token("Identifier", SubSeg));

                continue;
            }

            if (Operators.Contains(Segment)) { // Handle single char operators
                TokenQueue.Enqueue(new Token("Operator", Segment));
                Pos++; continue;
            }

            if (RawChar == '=') { // Handle assignment equals
                TokenQueue.Enqueue(new Token("Assignment", Segment));
                Pos++; continue;
            }

            if (RawChar == '(' || RawChar == ')') { // Handle parenthesis
                TokenQueue.Enqueue(new Token("Parenthesis", Segment));
                Pos++; continue;
            }

            TokenQueue.Enqueue(new Token("Unknown", Segment)); // Handle unknowns

            Pos++;
        }

        return TokenQueue;
    }
}

class Lexer {
    private Tokenizer T;
    private string FileData = "";

    public Lexer(string Path) { // Using file, reads and saves data
        T = new Tokenizer();
        
        if (File.Exists(Path))
            FileData = File.ReadAllText(Path);
        else
            throw new Exception(); // File does not exist
    }
    public Lexer() { // Not using file, expects manual data later when calling Tokenize
        T = new Tokenizer();
    }

    public Queue<Token> Tokenize(string Data) { // Public/Main method to use tokenizer
        return T.Process(Data);
    }
    public Queue<Token> Tokenize() { // Public/Main method to use tokenizer
        if (FileData.Length <= 0)
            throw new Exception(); // File existed, but was empty

        return T.Process(FileData);
    }

    public void DebugTQ(Queue<Token> TokenQueue) { // Debug TokenQueue by printing it out
        foreach(Token T in TokenQueue)
            Console.WriteLine(T);
    }
}