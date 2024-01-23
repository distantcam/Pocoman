using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pocoman;
using Xunit.Abstractions;

public class ExampleTests
{
    private readonly VerifySettings _codeVerifySettings;

    public ExampleTests()
    {
        _codeVerifySettings = new();
        _codeVerifySettings.ScrubLinesContaining("Version:", "SHA:", "GeneratedCodeAttribute");
    }

    [Theory]
    [MemberData(nameof(BuildExamples))]
    public Task PocoBuilder_VerifyTests(CodeFileTheoryData theoryData) =>
        VerifyTest(theoryData, Path.Combine("Examples", "Builder", "Verified"), new PocoBuilderGenerator());

    [Theory]
    [MemberData(nameof(BuildExamples))]
    public void PocoBuilder_CompileTests(CodeFileTheoryData theoryData) =>
        CompileTest(theoryData, [], new PocoBuilderGenerator());

    public static IEnumerable<object[]> BuildExamples() => GetExamples("Builder");

    #region Support Code
#if ROSLYN_3_11
    private Task VerifyTest(CodeFileTheoryData theoryData, string verifyPath, params ISourceGenerator[] generators)
#elif ROSLYN_4_0 || ROSLYN_4_4
    private Task VerifyTest(CodeFileTheoryData theoryData, string verifyPath, params IIncrementalGenerator[] generators)
#endif
    {
        var compilation = Compile(theoryData.Code);
        var driver = CreateDriver(compilation, generators).RunGenerators(compilation);

        return Verify(driver, _codeVerifySettings)
            .UseDirectory(verifyPath)
            .UseTypeName(theoryData.Name);
    }

#if ROSLYN_3_11
    private Task CompileTest(CodeFileTheoryData theoryData, string[] ignoredWarnings, params ISourceGenerator[] generators)
#elif ROSLYN_4_0 || ROSLYN_4_4
    private void CompileTest(CodeFileTheoryData theoryData, string[] ignoredWarnings, params IIncrementalGenerator[] generators)
#endif
    {
        var compilation = Compile(theoryData.Code);
        CreateDriver(compilation, generators)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        outputCompilation.GetDiagnostics()
            .Where(d => !ignoredWarnings.Contains(d.Id))
            .Should().BeEmpty();
    }

#if ROSLYN_3_11
    private static GeneratorDriver CreateDriver(Compilation c, params ISourceGenerator[] generators)
        => CSharpGeneratorDriver.Create(generators, parseOptions: c.SyntaxTrees.FirstOrDefault().Options as CSharpParseOptions);
#elif ROSLYN_4_0 || ROSLYN_4_4
    private static GeneratorDriver CreateDriver(Compilation c, params IIncrementalGenerator[] generators)
        => CSharpGeneratorDriver.Create(generators).WithUpdatedParseOptions(c.SyntaxTrees.FirstOrDefault()?.Options ?? CSharpParseOptions.Default);
#endif

    private static CSharpCompilation Compile(params string[] code)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>()
            .Concat(new[] { MetadataReference.CreateFromFile(Path.Combine(Environment.CurrentDirectory, "Pocoman.Attributes.dll")) });

#if ROSLYN_3_11
        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
#elif ROSLYN_4_0 || ROSLYN_4_4
        var options = CSharpParseOptions.Default;
#endif

        return CSharpCompilation.Create(
            "PocomanTests",
            code.Select(c => CSharpSyntaxTree.ParseText(c, options)),
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary
            ));
    }

    private static IEnumerable<object[]> GetExamples(string dir)
    {
        var baseDir = new DirectoryInfo(Environment.CurrentDirectory)?.Parent?.Parent?.Parent;
        if (baseDir == null)
            yield break;

        var examples = Directory.GetFiles(Path.Combine(baseDir.FullName, "Examples", dir), "*.cs");

        foreach (var example in examples)
        {
            if (example.Contains(".g."))
                continue;

            var code = File.ReadAllText(example);
            yield return new object[] {
                new CodeFileTheoryData {
                    Code = code,
                    Name = Path.GetFileNameWithoutExtension(example)
                }
            };
        }
    }

    public class CodeFileTheoryData : IXunitSerializable
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public void Deserialize(IXunitSerializationInfo info)
        {
            Name = info.GetValue<string>("Name");
            Code = info.GetValue<string>("Code");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("Code", Code);
        }

        public override string ToString() => Name + ".cs";
    }
    #endregion
}
