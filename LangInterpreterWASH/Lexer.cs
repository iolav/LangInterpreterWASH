using System.Collections;

class Token(string C, string V) // To store a tokens value and associating classifier
{
    public readonly string Classifier = C;
    public readonly string Value = V;
}

class Tokenizer {
    readonly private char[] Operators = ['+', '-'];
    readonly private string[] Keywords = [];

    public Queue Process(string Data) { // Tokenize all data
        Queue TokenQueue = new();

        int Len = Data.Length;
        int Pos = 0;

        while (Pos < Len) {
            char RawChar = Data[Pos];
            string Segment = RawChar.ToString();

            if (string.IsNullOrWhiteSpace(Segment)) { // Ignore whitespace
                Pos++; continue;
            }

            if (int.TryParse(Segment, out _)) { // Handle numbers
                int Start = Pos;
                bool Deci = false;

                while (Pos < Len && (int.TryParse(Data[Pos].ToString(), out _) || Data[Pos] == '.')) {
                    if (Data[Pos] == '.') {
                        if (Deci) break;
                        Deci = true;
                    }
                    Pos++;
                }

                TokenQueue.Enqueue(new Token("Number", Data[Start .. Pos]));

                continue;
            }

            if (char.IsLetter(RawChar)) { // Handle letters and keywords
                int Start = Pos;

                while (Pos < Len && (char.IsLetter(Data[Pos]) || int.TryParse(Data[Pos].ToString(), out _)))
                    Pos++;
                string SubSeg = Data[Start .. Pos];
                if (Keywords.Contains(SubSeg))
                    TokenQueue.Enqueue(new Token("Keyword", SubSeg));
                else
                    TokenQueue.Enqueue(new Token("Identifier", SubSeg));

                continue;
            }

            if (Operators.Contains(RawChar)) { // Handle operators
                TokenQueue.Enqueue(new Token("Operator", Segment));
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
    }
    public Lexer() { // Not using file, expects manual data later when calling Tokenize
        T = new Tokenizer();
    }

    public Queue Tokenize(string Data) { // Public/Main method to use tokenizer
        return T.Process(Data);
    }
    public Queue Tokenize() { // Public/Main method to use tokenizer
        return T.Process(FileData);
    }

    public void DebugTQ(Queue TokenQueue) { // Debug TokenQueue by printing it out
        foreach(Token T in TokenQueue)
            Console.WriteLine($"T:\n  : {T.Value}\n  : {T.Classifier}");
    }
}