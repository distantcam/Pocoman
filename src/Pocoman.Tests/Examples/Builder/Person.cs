using Pocoman;

[PocoBuilder]
public class Person
{
    public Person(string name) { Name = name; }

    public string Name { get; }
    public required string Honorific { get; init; }
    public int Age { get; init; }

    public string Address { get; set; }
}
