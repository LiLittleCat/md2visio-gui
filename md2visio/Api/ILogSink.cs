namespace md2visio.Api
{
    /// <summary>
    /// 日志接收器接口
    /// </summary>
    public interface ILogSink
    {
        void Info(string message);
        void Debug(string message);
        void Warning(string message);
        void Error(string message);
    }

    /// <summary>
    /// 空日志接收器（丢弃所有日志）
    /// </summary>
    public sealed class NullLogSink : ILogSink
    {
        public static readonly NullLogSink Instance = new NullLogSink();
        private NullLogSink() { }

        public void Info(string message) { }
        public void Debug(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
    }

    /// <summary>
    /// 控制台日志接收器
    /// </summary>
    public sealed class ConsoleLogSink : ILogSink
    {
        public static readonly ConsoleLogSink Instance = new ConsoleLogSink();
        private ConsoleLogSink() { }

        public void Info(string message) => Console.WriteLine(message);
        public void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");
        public void Warning(string message) => Console.WriteLine($"[WARN] {message}");
        public void Error(string message) => Console.Error.WriteLine($"[ERROR] {message}");
    }
}
