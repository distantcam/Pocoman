using Pocoman;

[PocoBuilder]
public class Ctor
{
    public Ctor(int number) { }
    public Ctor(string name) { }
    public Ctor(int number, string name) { }
}

[PocoBuilder]
public class CtorWithDefault
{
    public CtorWithDefault() { }
    public CtorWithDefault(int number) { }
    public CtorWithDefault(string name) { }
}
