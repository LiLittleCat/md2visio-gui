using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence
{
    internal class SeqSttChar : SttCtxChar
    {
        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            char ch = next[0];

            if (char.IsWhiteSpace(ch) && ch != '\n' && ch != '\r')
            {
                // 如果 Buffer 包含消息箭头格式（如 a->>b:），继续读取到行尾
                // 不在空格处分割，保持消息行的完整性
                if (IsMessageArrowLine())
                {
                    return Take().Forward<SeqSttChar>();
                }

                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return SlideSpaces().Forward<SeqSttChar>();
            }
            else if (ch == '\n' || ch == '\r')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttFinishFlag>();
            }
            else if (ch == '%')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttComment>();
            }
            else if (ch == '`')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttMermaidClose>();
            }
            else
            {
                return Take().Forward<SeqSttChar>();
            }
        }

        /// <summary>
        /// 检查 Buffer 是否包含消息箭头格式
        /// 消息格式: from->>to: message 或 from-->>to: message 等
        /// </summary>
        private bool IsMessageArrowLine()
        {
            string buf = Buffer.ToString();
            // 检查是否包含消息箭头并且后面有冒号（表示这是消息行）
            return (buf.Contains("->>") || buf.Contains("-->>") ||
                    buf.Contains("->") || buf.Contains("-->")) &&
                   buf.Contains(":");
        }
    }
}