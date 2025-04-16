using ValuePair = (string, object);

class Enviornment(Enviornment? P = null) {
    private Dictionary<string, ValuePair> Storage = []; // Variable storage
    public Enviornment? Parent = P;

    public void Store(string Identifier, ValuePair Value) { // Public method for storing a variable
        Storage[Identifier] = (Value.Item1, Value.Item2);
        Console.WriteLine($"Stored variable \"{Identifier}\": {Value}");
    }

    public bool Fetch(string Identifier, out ValuePair Value) { // Public method for fetching a variable
        ValuePair? Found = SimpleFind(Identifier);

        Enviornment? CurrentEnv = Parent;

        while (CurrentEnv != null && Found == null) {
            Found = CurrentEnv.SimpleFind(Identifier);

            CurrentEnv = CurrentEnv.Parent;
        }

        Value = Found ?? default;

        return Found != null;
    }

   public ValuePair? SimpleFind(string Identifier) {
        return Storage.TryGetValue(Identifier, out (string, object) Value) ? Value : null;
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, ValuePair> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}