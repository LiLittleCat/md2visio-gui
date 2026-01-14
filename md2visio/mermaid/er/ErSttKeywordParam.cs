using md2visio.mermaid.cmn;

namespace md2visio.mermaid.er
{
    /// <summary>
    /// ER图关键字参数状态类
    /// 处理 direction TB, direction LR 等参数
    /// </summary>
    internal class ErSttKeywordParam : SynState
    {
        public override SynState NextState()
        {
            Save(Buffer).ClearBuffer();
            return Forward<ErSttChar>();
        }

        public static bool HasParam(SynContext ctx)
        {
            string next = ctx.Peek() ?? "";
            return next == " " || next == "\t";
        }
    }
}
