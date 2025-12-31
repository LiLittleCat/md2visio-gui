using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.vsdx.@base
{
    /// <summary>
    /// Visio COM 会话接口
    /// 用于管理 Visio Application 的生命周期
    /// </summary>
    public interface IVisioSession : IDisposable
    {
        /// <summary>
        /// Visio 应用程序实例
        /// </summary>
        Visio.Application Application { get; }

        /// <summary>
        /// 是否显示 Visio 窗口
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// 创建新的空白文档
        /// </summary>
        Visio.Document CreateDocument();

        /// <summary>
        /// 打开模板文档
        /// </summary>
        Visio.Document OpenStencil(string path);

        /// <summary>
        /// 保存文档到指定路径
        /// </summary>
        void SaveDocument(Visio.Document doc, string path, bool overwrite = true);

        /// <summary>
        /// 关闭文档
        /// </summary>
        void CloseDocument(Visio.Document doc);
    }
}
