using Pocoman;

[PocoBuilder]
public class Ctor
{
    public Ctor() { }
    public Ctor(int number) { }
    public Ctor(string name) { }
    public Ctor(int number, string name) { }
    public Ctor(int number, bool flag = true, string blank = null, string empty = "", string foo = "foo") { }
}
