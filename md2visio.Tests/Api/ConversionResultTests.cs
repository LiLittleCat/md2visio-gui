using md2visio.Api;

namespace md2visio.Tests.Api
{
    public class ConversionResultTests
    {
        [Fact]
        public void Succeeded_WithOutputFiles_SetsPropertiesCorrectly()
        {
            // Arrange
            var outputFiles = new[] { "file1.vsdx", "file2.vsdx" };

            // Act
            var result = ConversionResult.Succeeded(outputFiles);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Null(result.Exception);
            Assert.Equal(2, result.OutputFiles.Length);
            Assert.Equal("file1.vsdx", result.OutputFiles[0]);
        }

        [Fact]
        public void Succeeded_WithEmptyArray_SetsEmptyOutputFiles()
        {
            var result = ConversionResult.Succeeded(Array.Empty<string>());

            Assert.True(result.Success);
            Assert.Empty(result.OutputFiles);
        }

        [Fact]
        public void Failed_WithMessage_SetsPropertiesCorrectly()
        {
            // Arrange
            var errorMessage = "Conversion failed";

            // Act
            var result = ConversionResult.Failed(errorMessage);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Exception);
            Assert.Empty(result.OutputFiles);
        }

        [Fact]
        public void Failed_WithMessageAndException_SetsAllProperties()
        {
            // Arrange
            var errorMessage = "Conversion failed";
            var exception = new InvalidOperationException("Test exception");

            // Act
            var result = ConversionResult.Failed(errorMessage, exception);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Same(exception, result.Exception);
            Assert.Empty(result.OutputFiles);
        }
    }
}
