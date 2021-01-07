using Xunit;
using Xunit.Abstractions;
using Zolwik.Compiler;

namespace TurtleTests
{
    public class TurtleCommandsCompilerTest
    {
        private readonly ITestOutputHelper output;

        public TurtleCommandsCompilerTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ItProducesAnything()
        {
            Assert.NotNull(new RoslynTurtleCommandsCompiler().CompileTurtleCommands(""));
        }
    }
}