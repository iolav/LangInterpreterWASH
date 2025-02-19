class Variables {
    private Dictionary<string, object> Storage; // Variable storage, duh

    public Variables() {
        Storage = [];
    }

    public void Store(string Identifier, object Value) { // Public method for storing a variable
        Storage[Identifier] = Value;
    }

    public void DebugAll() { // Debugging method to just dump all the values
        foreach (KeyValuePair<string, object> Entry in Storage) {
            Console.WriteLine($"{Entry.Key} = {Entry.Value}");
        }
    }
}