using System;
using TurtleSharp;

namespace Zolwik.Compiler
{
    internal interface ITurtleCommandsCompiler
    {
        Action<Turtle, ITurtlePresentation> CompileTurtleCommands(string script);
    }
}