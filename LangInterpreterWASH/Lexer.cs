using System;
using System.Text.RegularExpressions;

class Lexer {
    Regex RG = new(".");

    public void Tokenize(string Line) {
        MatchCollection Matches = RG.Matches(Line);
        
        for (int i = 0; i < Matches.Count; i++)
            Console.WriteLine(Matches[i].Value);
    }
}