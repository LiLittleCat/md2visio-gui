using md2visio.Api;

namespace md2visio.Tests.Api
{
    public class ConversionRequestTests
    {
        [Fact]
        public void Create_WithValidPaths_SetsProperties()
        {
            // Arrange & Act
            var request = ConversionRequest.Create("input.md", "output.vsdx");

            // Assert
            Assert.Equal("input.md", request.InputPath);
            Assert.Equal("output.vsdx", request.OutputPath);
            Assert.False(request.ShowVisio);
            Assert.True(request.SilentOverwrite); // default is true
            Assert.False(request.Debug);
        }

        [Fact]
        public void Create_WithNullInputPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ConversionRequest.Create(null!, "output.vsdx"));
        }

        [Fact]
        public void Create_WithNullOutputPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ConversionRequest.Create("input.md", null!));
        }

        [Fact]
        public void WithShowVisio_SetsShowVisioTrue()
        {
            var request = ConversionRequest.Create("input.md", "output.vsdx")
                .WithShowVisio();

            Assert.True(request.ShowVisio);
        }

        [Fact]
        public void WithSilentOverwrite_SetsSilentOverwriteTrue()
        {
            var request = ConversionRequest.Create("input.md", "output.vsdx")
                .WithSilentOverwrite();

            Assert.True(request.SilentOverwrite);
        }

        [Fact]
        public void WithDebug_SetsDebugTrue()
        {
            var request = ConversionRequest.Create("input.md", "output.vsdx")
                .WithDebug();

            Assert.True(request.Debug);
        }

        [Fact]
        public void FluentChaining_SetsAllOptions()
        {
            var request = ConversionRequest.Create("input.md", "output.vsdx")
                .WithShowVisio()
                .WithSilentOverwrite()
                .WithDebug();

            Assert.True(request.ShowVisio);
            Assert.True(request.SilentOverwrite);
            Assert.True(request.Debug);
        }
    }
}
