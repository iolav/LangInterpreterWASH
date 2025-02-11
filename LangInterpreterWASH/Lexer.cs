using System.Collections;

class Tokenizer {
    public Queue Process(string Data) { // Tokenize all data
        Queue TokenQueue = new();

        int Len = Data.Length;
        int Pos = 0;

        while (Pos < Len) {
            string Segment = Data[Pos].ToString();

            if (string.IsNullOrWhiteSpace(Segment)) { // Ignore whitespace
                Pos++; continue;
            }

            if (int.TryParse(Segment, out _)) { // Handle numbers
                int Start = Pos;
                string SubSeg = Data[Pos].ToString();

                while (Pos < Len && (int.TryParse(Data[Pos].ToString(), out _) || true )) {
                    Pos++;
                }
            }

            TokenQueue.Enqueue(("TEST", Segment));

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
        
        if (File.Exists(Path)) {
            FileData = File.ReadAllText(Path);
        }
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
        foreach(ValueTuple<string, string> Token in TokenQueue) Console.WriteLine(Token);
    }
}