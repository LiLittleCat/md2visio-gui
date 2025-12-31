namespace md2visio.Api
{
    /// <summary>
    /// 转换结果
    /// </summary>
    public sealed class ConversionResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 生成的输出文件路径数组
        /// </summary>
        public string[] OutputFiles { get; }

        /// <summary>
        /// 错误消息（失败时）
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// 异常详情（用于调试）
        /// </summary>
        public Exception? Exception { get; }

        private ConversionResult(bool success, string[] outputFiles, string? errorMessage, Exception? exception)
        {
            Success = success;
            OutputFiles = outputFiles;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        public static ConversionResult Succeeded(params string[] outputFiles)
        {
            return new ConversionResult(true, outputFiles ?? Array.Empty<string>(), null, null);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        public static ConversionResult Failed(string errorMessage, Exception? exception = null)
        {
            return new ConversionResult(false, Array.Empty<string>(), errorMessage, exception);
        }
    }
}
