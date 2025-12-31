namespace md2visio.Api
{
    /// <summary>
    /// Mermaid 到 Visio 转换器接口
    /// </summary>
    public interface IMd2VisioConverter : IDisposable
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="request">转换请求参数</param>
        /// <param name="progress">进度报告器（可选）</param>
        /// <param name="logger">日志接收器（可选）</param>
        /// <returns>转换结果</returns>
        ConversionResult Convert(
            ConversionRequest request,
            IProgress<ConversionProgress>? progress = null,
            ILogSink? logger = null);
    }
}
