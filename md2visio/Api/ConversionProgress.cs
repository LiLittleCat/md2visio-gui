namespace md2visio.Api
{
    /// <summary>
    /// 转换进度信息
    /// </summary>
    public sealed class ConversionProgress
    {
        /// <summary>
        /// 进度百分比 (0-100)
        /// </summary>
        public int Percentage { get; }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 当前阶段
        /// </summary>
        public ConversionPhase Phase { get; }

        public ConversionProgress(int percentage, string message, ConversionPhase phase)
        {
            Percentage = Math.Clamp(percentage, 0, 100);
            Message = message ?? string.Empty;
            Phase = phase;
        }
    }

    /// <summary>
    /// 转换阶段枚举
    /// </summary>
    public enum ConversionPhase
    {
        /// <summary>启动中</summary>
        Starting,
        /// <summary>解析 Mermaid 语法</summary>
        Parsing,
        /// <summary>构建图表数据结构</summary>
        Building,
        /// <summary>渲染到 Visio</summary>
        Rendering,
        /// <summary>保存文件</summary>
        Saving,
        /// <summary>完成</summary>
        Completed
    }
}
