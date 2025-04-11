using ValuePair = (string, object);

class Enviornment(Enviornment? P = null) {
    private Dictionary<string, ValuePair> Storage = []; // Variable storage
    public Enviornment? Parent = P;

    public void Store(string Identifier, ValuePair Value) { // Public method for storing a variable
        Storage[Identifier] = (Value.Item1, Value.Item2);
        Console.WriteLine($"Stored variable \"{Identifier}\": {Value}");
    }

    public ValuePair Fetch(string Identifier) { // Public method for fetching a variable
        return Storage[Identifier];
    }

    public bool SimpleFind(string Identifier) {
        return Storage.ContainsKey(Identifier);
    }
    public bool Find(string Identifier) { // Public method for checking if a variable exists
        bool Found = Storage.ContainsKey(Identifier);
        
        if (Parent != null) {
            Enviornment EnvParent = Parent;

            while (EnvParent.Parent != null && !Found) {
                Found = EnvParent.SimpleFind(Identifier);

                EnvParent = EnvParent.Parent;
            }
        }

        return Found;
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, ValuePair> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}