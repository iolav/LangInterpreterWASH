class Variables {
    private Dictionary<string, (string, object)> Storage; // Variable storage

    public Variables() {
        Storage = [];
    }

    public void Store(string Identifier, (string, object) Value) { // Public method for storing a variable
        Storage[Identifier] = (Value.Item1, Value.Item2);
    }

    public object Fetch(string Identifier) { // Public method for fetching a variable
        return Storage[Identifier];
    }

    public bool Find(string Identifier) { // Public method for checking if a variable exists
        return Storage.ContainsKey(Identifier);
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, (string, object)> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}