using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using TurtleSharp;

namespace Zolwik.Compiler
{
    public class RoslynTurtleCommandsCompiler : ITurtleCommandsCompiler
    {
        static string runtimePath = typeof(object).Assembly.Location;
        static string turtlePath = typeof(Turtle).Assembly.Location;

        static MetadataReference[] References =
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location), //system reference
            MetadataReference.CreateFromFile(typeof(Turtle).Assembly.Location) //TurtleSharp reference
        };
        static string[] Namespaces =
        {
            "System",
            "TurtleSharp"
        };

        static CSharpCompilationOptions CompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(Namespaces);

        static string ScriptPrefix =
            "using TurtleSharp;" +
            "using System;" +
            "namespace Generated" +
            "{" +
            "   public static class GeneratedClass {" +
            "       public static void GeneratedMethod(Turtle Turtle, ITurtlePresentation Canvas) {\n";
        static string ScriptPostfix =
            "       }" +
            "   }" +
            "}";
        public Action<Turtle, ITurtlePresentation> CompileTurtleCommands(string script)
        {
            var assemblyName = "generated.dll";
            var source = ComposeSource(script);
            var syntaxTrees = new SyntaxTree[] { SyntaxFactory.ParseSyntaxTree(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8)) };

            var compiled = CSharpCompilation.Create(assemblyName)
                .WithOptions(CompilationOptions)
                .WithReferences(References)
                .AddSyntaxTrees(syntaxTrees);

            string generatedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyName);
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

        SourceText ComposeSource(string script) => SourceText.From(string.Concat(ScriptPrefix, script, ScriptPostfix), Encoding.UTF8);
    }
}