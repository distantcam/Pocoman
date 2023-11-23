using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Pocoman;

public sealed partial class PocomanGenerator
{
    private static class Emitter
    {
        public static void GenerateSource(
#if ROSLYN_3_11
            GeneratorExecutionContext context,
#elif ROSLYN_4_0 || ROSLYN_4_4
            SourceProductionContext context,
#endif
            ImmutableArray<INamedTypeSymbol> types)
        {
            if (types.IsDefaultOrEmpty) return;

            foreach (var type in types)
            {
                var source = new CodeBuilder()
                    .AppendHeader()
                    .AppendLine();

                if (!type.ContainingNamespace.IsGlobalNamespace)
                {
                    source.AppendLine($"namespace {type.ContainingNamespace}");
                    source.OpenBlock();
                }
                using (source.StartBlock($"public partial class {type.Name}Builder"))
                {
                    foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
                    {
                        AddProperty(source, prop, type.Name + "Builder");
                    }

                    AddBuildMethod(source, type);
                }
                if (!type.ContainingNamespace.IsGlobalNamespace)
                {
                    source.CloseBlock();
                }

                context.AddSource(GeneratorUtilities.GetHintName(type) + "Builder.g.cs", source);
            }
        }

        private static void AddProperty(CodeBuilder source, IPropertySymbol property, string builderName)
        {
            var n = property.Name;
            var t = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            source.AppendLine($"private bool _{n}_isSet;");
            source.AppendLine($"private global::System.Lazy<{t}> _{n} = new global::System.Lazy<{t}>(() => default);");
            source.AppendLine($"public {builderName} With{n}({t} value) => With{n}(() => value);");
            using (source.StartBlock($"public {builderName} With{n}(global::System.Func<{t}> func)"))
            {
                source.AppendLine($"_{n} = new global::System.Lazy<{t}>(func);");
                source.AppendLine($"_{n}_isSet = true;");
                source.AppendLine($"return this;");
            }
        }

        private static void AddBuildMethod(CodeBuilder source, INamedTypeSymbol type)
        {
            var t = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            using (source.StartBlock($"public {t} Build()"))
            {
                source.AppendLine($"return new {t}()");
                source.AppendLine("{").IncreaseIndent();

                var initers = new List<string>();
                foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
                {
                    var n = prop.Name;
                    var pt = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (prop.IsRequired)
                        initers.Add($"{n} = _{n}_isSet ? _{n}.Value : throw new global::System.InvalidOperationException(\"Property \\\"{n}\\\" ({pt}) must be set before build can be called.\")");
                    else
                        initers.Add($"{n} = _{n}_isSet ? _{n}.Value : default");
                }
                for (var i = 0; i < initers.Count; i++)
                {
                    source.AppendLine(initers[i] + (i < initers.Count - 1 ? "," : ""));
                }

                source.DecreaseIndent().AppendLine("};");
            }
        }
    }
}
