using ValuePair = (string, object);

class Enviornment(Enviornment? P = null) {
    private Dictionary<string, ValuePair> Storage = []; // Variable storage
    public Enviornment? Parent = P;

    public void Store(string Identifier, ValuePair Value) { // Public method for storing a variable
        Storage[Identifier] = (Value.Item1, Value.Item2);

        object PrintValue = Value.Item2;
        if (Value.Item1 == "Array") {
            PrintValue = "[";
            
            foreach (ValuePair Element in (List<ValuePair>)Value.Item2)
                PrintValue += Element.Item2.ToString() + ", ";

            string PrintValueStr = (string)PrintValue;
            PrintValue = PrintValueStr[..^2] + "]";
        }

        Console.WriteLine($"Stored variable \"{Identifier}\": {PrintValue}");
    }

    public bool Fetch(string Identifier, out ValuePair Value, out Enviornment? FoundEnv) { // Public method for fetching a variable
        ValuePair? Found = SimpleFind(Identifier);

        Enviornment? CurrentEnv = Found == null ? Parent : this;

        while (CurrentEnv != null && Found == null) {
            Found = CurrentEnv.SimpleFind(Identifier);

            if (Found == null) CurrentEnv = CurrentEnv.Parent;
        }

        Value = Found ?? default;
        FoundEnv = CurrentEnv;

        return Found != null;
    }

   public ValuePair? SimpleFind(string Identifier) {
        return Storage.TryGetValue(Identifier, out (string, object) Value) ? Value : null;
    }

    public void DebugValues() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, ValuePair> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}