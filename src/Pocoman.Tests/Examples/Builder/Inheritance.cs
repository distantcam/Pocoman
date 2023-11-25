using Pocoman;

public class ClassA { public string A { get; set; } }

public class ClassB : ClassA { public string B { get; set; } }

[PocoBuilder]
public class ClassC : ClassB { public string C { get; set; } }
