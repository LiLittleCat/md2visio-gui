using md2visio.Api;

namespace md2visio.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of ILogSink for capturing log messages in tests
    /// </summary>
    public class MockLogSink : ILogSink
    {
        public List<(string Level, string Message)> Messages { get; } = new();

        public void Debug(string message)
        {
            Messages.Add(("DEBUG", message));
        }

        public void Info(string message)
        {
            Messages.Add(("INFO", message));
        }

        public void Warning(string message)
        {
            Messages.Add(("WARNING", message));
        }

        public void Error(string message)
        {
            Messages.Add(("ERROR", message));
        }

        public bool HasDebugMessages => Messages.Any(m => m.Level == "DEBUG");
        public bool HasInfoMessages => Messages.Any(m => m.Level == "INFO");
        public bool HasWarningMessages => Messages.Any(m => m.Level == "WARNING");
        public bool HasErrorMessages => Messages.Any(m => m.Level == "ERROR");

        public void Clear() => Messages.Clear();

        public IEnumerable<string> GetMessages(string level) =>
            Messages.Where(m => m.Level == level).Select(m => m.Message);
    }
}
