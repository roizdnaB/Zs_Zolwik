using Xunit;
using Zolwik.Compiler;
using Xunit.Abstractions;

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