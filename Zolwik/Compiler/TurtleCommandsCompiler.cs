using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using TurtleSharp;

namespace Zolwik.Compiler
{
    public class TurtleCommandsCompiler : ITurtleCommandsCompiler
    {
        private string[] References;
        private CompilerParameters Options = new CompilerParameters();
        private CSharpCodeProvider Compiler = new CSharpCodeProvider();

        private string ScriptPrefix =
            "using System;" +
            "using TurtleSharp;" +
            "" +
            "namespace Generated" +
            "{" +
            "   public static class GeneratedClass {" +
            "       public static void GeneratedMethod(Turtle Turtle, ITurtlePresentation Canvas) {\n";

        private string ScriptPostfix =
            "       }" +
            "   }" +
            "}";

        public TurtleCommandsCompiler()
        {
            References = new string[]
            {
                "System.dll",
                typeof(Turtle).Assembly.Location
            };

            Options.GenerateInMemory = true;
            Options.TreatWarningsAsErrors = false;
            Options.GenerateExecutable = false;
            Options.CompilerOptions = "/optimize";

            Options.ReferencedAssemblies.AddRange(References);
        }

        public Action<Turtle, ITurtlePresentation> CompileTurtleCommands(string script)
        {
            var compiled = Compiler.CompileAssemblyFromSource(Options, ComposeSource(script));

            if (compiled.Errors.HasErrors)
            {
                string text = "Compile error: ";
                foreach (var error in compiled.Errors)
                {
                    text += "rn" + error.ToString();
                }
                throw new Exception(text);
            }

            var module = compiled.CompiledAssembly.Modules.ElementAt(0);
            var method = module.GetType("Generated.GeneratedClass").GetMethod("GeneratedMethod");

            return method.CreateDelegate(typeof(Action<Turtle, ITurtlePresentation>)) as Action<Turtle, ITurtlePresentation>;
        }

        private string ComposeSource(string script) => string.Concat(ScriptPrefix, script, ScriptPostfix);
    }
}