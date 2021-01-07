using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using TurtleSharp;

namespace Zolwik.Compiler
{
    public class RoslynTurtleCommandsCompiler : ITurtleCommandsCompiler
    {
        private static string runtimePath = typeof(object).Assembly.Location;
        private static string turtlePath = typeof(Turtle).Assembly.Location;

        private static MetadataReference[] References =
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),                             //
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Core")).Location),      //  system references
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),   //
            MetadataReference.CreateFromFile(typeof(Turtle).Assembly.Location)                              //TurtleSharp reference
        };

        private static string[] Namespaces =
        {
            "System",
            "TurtleSharp"
        };

        private static CSharpCompilationOptions CompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(Namespaces);

        private static string ScriptPrefix =
            "using TurtleSharp;" +
            "using System;" +
            "namespace Generated" +
            "{" +
            "   public static class GeneratedClass {" +
            "       public static void GeneratedMethod(Turtle Turtle, ITurtlePresentation Canvas) {\n";

        private static string ScriptPostfix =
            "       }" +
            "   }" +
            "}";

        private static string assemblyName = "generated.dll";
        private static string generatedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyName);

        public Action<Turtle, ITurtlePresentation> CompileTurtleCommands(string script)
        {
            var source = ComposeSource(script);
            var syntaxTrees = new SyntaxTree[] { SyntaxFactory.ParseSyntaxTree(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8)) };

            var compiled = CSharpCompilation.Create(assemblyName)
                .WithOptions(CompilationOptions)
                .WithReferences(References)
                .AddSyntaxTrees(syntaxTrees);

            var emittedResult = compiled.Emit(generatedAssemblyPath);

            if (emittedResult.Success)
            {
                var generatedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(generatedAssemblyPath);
                return
                    generatedAssembly.GetType("Generated.GeneratedClass")
                    .GetMethod("GeneratedMethod")
                    .CreateDelegate(typeof(Action<Turtle, ITurtlePresentation>))
                    as Action<Turtle, ITurtlePresentation>;
            }
            else
            {
                foreach (var issue in emittedResult.Diagnostics)
                {
                    string issueDescriptor = $"ID: {issue.Id}, Message: {issue.GetMessage()}" +
                        $"Location: { issue.Location.GetLineSpan()}," +
                        $"Severity: { issue.Severity}";
                    Console.WriteLine(issueDescriptor);
                }
            }
            return null;
        }

        private SourceText ComposeSource(string script) => SourceText.From(string.Concat(ScriptPrefix, script, ScriptPostfix), Encoding.UTF8);
    }
}