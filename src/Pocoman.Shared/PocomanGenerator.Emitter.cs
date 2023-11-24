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
            ImmutableArray<TypeModel> types)
        {
            if (types.IsDefaultOrEmpty) return;

            foreach (var type in types)
            {
                var source = new CodeBuilder()
                    .AppendHeader()
                    .AppendLine();

                using (source.StartPartialType(type))
                {
                    foreach (var prop in type.Properties)
                    {
                        AddProperty(source, prop, type.Name);
                    }

                    AddBuildMethod(source, type);
                }

                context.AddSource(type.HintName + ".g.cs", source);
            }
        }

        private static void AddProperty(CodeBuilder source, IPropertySymbol property, string builderName)
        {
            if (property.SetMethod?.DeclaredAccessibility != Accessibility.Public)
                return;

            var n = property.Name;
            var t = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            source.AppendLine($"private bool _{n}_isSet;");
            source.AppendLine($"private {t} _{n} = default;");
            using (source.StartBlock($"public {builderName} With{n}({t} value)"))
            {
                source.AppendLine($"_{n} = value;");
                source.AppendLine($"_{n}_isSet = true;");
                source.AppendLine($"return this;");
            }
        }

        private static void AddBuildMethod(CodeBuilder source, TypeModel type)
        {
            var t = type.FullType;

            var initers = new List<string>();
            var otherSetters = new List<IPropertySymbol>();

            foreach (var prop in type.Properties)
            {
                if (prop.SetMethod is null) continue;

                var n = prop.Name;
                var pt = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (prop.IsRequired)
                    initers.Add($"{n} = _{n}_isSet ? _{n} : throw new global::System.InvalidOperationException(\"Property \\\"{n}\\\" ({pt}) must be set before build can be called.\")");
                else if (prop.SetMethod?.IsInitOnly == true)
                    initers.Add($"{n} = _{n}_isSet ? _{n} : default");
                else if (prop.SetMethod!.DeclaredAccessibility == Accessibility.Public)
                    otherSetters.Add(prop);
            }

            using (source.StartBlock($"public {t} Build()"))
            {
                source.AppendLine($"var build = new {t}()");
                source.AppendLine("{").IncreaseIndent();
                for (var i = 0; i < initers.Count; i++)
                    source.AppendLine(initers[i] + (i < initers.Count - 1 ? "," : ""));
                source.DecreaseIndent().AppendLine("};");

                for (var i = 0; i < otherSetters.Count; i++)
                {
                    var n = otherSetters[i].Name;
                    source.AppendLine($"if (_{n}_isSet)");
                    source.IncreaseIndent().AppendLine($"build.{n} = _{n};").DecreaseIndent();
                }

                source.AppendLine("return build;");
            }
        }
    }
}
