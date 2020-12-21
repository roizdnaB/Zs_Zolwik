using System;
using Zolwik;
using Xunit;
using Zolwik.Compiler;

namespace TurtleTests
{
    public class TurtleCommandsCompilerTest
    {
        [Fact]
        public void ItProducesAnything()
        {
            Assert.NotNull(new TurtleCommandsCompiler().CompileTurtleCommands(""));
        }
    }
}
