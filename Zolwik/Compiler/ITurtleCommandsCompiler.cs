using System;
using System.Collections.Generic;
using System.Text;
using TurtleSharp;

namespace Zolwik.Compiler
{
    interface ITurtleCommandsCompiler
    {
        Action<Turtle, ITurtlePresentation> CompileTurtleCommands(string script);
    }
}
