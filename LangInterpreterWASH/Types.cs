class Type(string N)
{
    private readonly string NameField = N;

    public string Name {
        get { return NameField; }
    }
}

class Integer() : Type("Integer") {}
class Float() : Type("Float") {}
class String() : Type("String") {}
class Boolean() : Type("Boolean") {}
class Character() : Type("Character") {}

class Identifier(string ST) : Type("Identifier")
{
    private readonly string SubTypeField = ST;

    public string SubType {
        get { return SubTypeField; }
    }
}

class Array(int D, Type ST) : Type("Array")
{
    private readonly int DimensionsField = D;
    private readonly Type StoredTypeField = ST;

    public int Dimensions {
        get { return DimensionsField; }
    }
    public Type StoredType {
        get { return StoredTypeField; }
    }
}