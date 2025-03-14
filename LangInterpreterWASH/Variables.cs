class Variables {
    private Dictionary<string, object> Storage; // Variable storage

    public Variables() {
        Storage = [];
    }

    public void Store(string Identifier, object Value) { // Public method for storing a variable
        Storage[Identifier] = Value;
    }

    public object Fetch(string Identifier) { // Public method for fetching a variable
        return Storage[Identifier];
    }

    public bool Find(string Identifier) { // Public method for checking if a variable exists
        return Storage.ContainsKey(Identifier);
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, object> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}