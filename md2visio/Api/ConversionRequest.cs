namespace md2visio.Api
{
    /// <summary>
    /// 转换请求参数（不可变值对象）
    /// 支持 fluent builder 模式创建
    /// </summary>
    public sealed class ConversionRequest
    {
        /// <summary>
        /// 输入 Markdown 文件路径
        /// </summary>
        public string InputPath { get; }

        /// <summary>
        /// 输出路径（可以是 .vsdx 文件路径或目录）
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// 是否显示 Visio 窗口（默认：不显示）
        /// </summary>
        public bool ShowVisio { get; }

        /// <summary>
        /// 是否静默覆盖已存在文件（默认：是）
        /// </summary>
        public bool SilentOverwrite { get; }

        /// <summary>
        /// 是否启用调试日志（默认：否）
        /// </summary>
        public bool Debug { get; }

        public ConversionRequest(
            string inputPath,
            string outputPath,
            bool showVisio = false,
            bool silentOverwrite = true,
            bool debug = false)
        {
            InputPath = inputPath ?? throw new ArgumentNullException(nameof(inputPath));
            OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
            ShowVisio = showVisio;
            SilentOverwrite = silentOverwrite;
            Debug = debug;
        }

        #region Static Factory Methods

        /// <summary>
        /// 创建转换请求
        /// </summary>
        public static ConversionRequest Create(string inputPath, string outputPath)
        {
            return new ConversionRequest(inputPath, outputPath);
        }

        #endregion

        #region Fluent Builder Methods

        /// <summary>
        /// 设置显示 Visio 窗口
        /// </summary>
        public ConversionRequest WithShowVisio(bool showVisio = true)
        {
            return new ConversionRequest(InputPath, OutputPath, showVisio, SilentOverwrite, Debug);
        }

        /// <summary>
        /// 设置静默覆盖
        /// </summary>
        public ConversionRequest WithSilentOverwrite(bool silentOverwrite = true)
        {
            return new ConversionRequest(InputPath, OutputPath, ShowVisio, silentOverwrite, Debug);
        }

        /// <summary>
        /// 设置调试模式
        /// </summary>
        public ConversionRequest WithDebug(bool debug = true)
        {
            return new ConversionRequest(InputPath, OutputPath, ShowVisio, SilentOverwrite, debug);
        }

        #endregion
    }
}
