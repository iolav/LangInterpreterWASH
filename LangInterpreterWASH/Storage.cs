using ValuePair = (string, object);

class Enviornment(Enviornment? P = null) {
    private Dictionary<string, ValuePair> Storage = []; // Variable storage
    private Enviornment? Parent = P;

    public void Store(string Identifier, ValuePair Value) { // Public method for storing a variable
        Storage[Identifier] = (Value.Item1, Value.Item2);
    }

    public ValuePair Fetch(string Identifier) { // Public method for fetching a variable
        return Storage[Identifier];
    }

    public bool Find(string Identifier) { // Public method for checking if a variable exists
        return Storage.ContainsKey(Identifier);
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, ValuePair> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}