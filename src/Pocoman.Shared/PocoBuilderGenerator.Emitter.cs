using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Pocoman;

public sealed partial class PocoBuilderGenerator
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

                using (source.StartPartialType(type))
                {
                    AddConstructorCalls(source, type, initers);
                    foreach (var prop in type.Properties)
                        AddProperty(source, prop, type.Name);
                    AddBuildMethod(source, type, otherSetters);
                    AddImplicitOperator(source, type);
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

        private static void AddConstructorCalls(CodeBuilder source, TypeModel type, List<string> initers)
        {
            var hasDefaultCtor = type.Constructors.Any(c => c.Parameters.Length == 0);

            source.AppendLine($"private global::System.Func<{type.FullType}> _builder;");

            using (source.StartBlock($"public {type.Name}()"))
            {
                if (hasDefaultCtor)
                    using (source.StartBlock("_builder = () => new()", "};"))
                    {
                        for (var i = 0; i < initers.Count; i++)
                            source.AppendLine(initers[i] + (i < initers.Count - 1 ? "," : ""));
                    }
                else
                    source.AppendLine($"_builder = () => throw new global::System.InvalidOperationException(\"A WithConstructor must be called before Build.\");");
            }

            if (hasDefaultCtor && type.Constructors.Count == 1) return;

            foreach (var c in type.Constructors)
            {
                var methodArgs = string.Join(", ", c.Parameters.Select(p => $"{p.Type.ToDisplayString(FullyQualifiedFormat)} {p.Name}"));
                var callArgs = string.Join(", ", c.Parameters.Select(p => p.Name));

                using (source.StartBlock($"public {type.Name} UsingConstructor({methodArgs})"))
                {
                    using (source.StartBlock($"_builder = () => new({callArgs})", "};"))
                    {
                        for (var i = 0; i < initers.Count; i++)
                            source.AppendLine(initers[i] + (i < initers.Count - 1 ? "," : ""));
                    }
                    source.AppendLine($"return this;");
                }
            }
        }

        private static void AddBuildMethod(CodeBuilder source, TypeModel type, List<IPropertySymbol> otherSetters)
        {
            using (source.StartBlock($"public {type.FullType} Build()"))
            {
                source.AppendLine($"var build = _builder();");
                for (var i = 0; i < otherSetters.Count; i++)
                {
                    var n = otherSetters[i].Name;
                    source.AppendLine($"if (_{n}_isSet)");
                    source.IncreaseIndent().AppendLine($"build.{n} = _{n};").DecreaseIndent();
                }
                source.AppendLine("return build;");
            }
        }

        private static void AddImplicitOperator(CodeBuilder source, TypeModel type)
        {
            source.AppendLine($"public static implicit operator {type.FullType}({type.Name} builder) => builder.Build();");
        }
    }
}
