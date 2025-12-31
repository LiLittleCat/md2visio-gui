using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence
{
    internal class SeqSttWord : SttWordFlag
    {
        public override SynState NextState()
        {
            string word = Buffer.ToString().Trim();
            
            if (SeqSttKeyword.IsKeyword(Ctx))
            {
                return Forward<SeqSttKeyword>();
            }
            else if (word.Contains("->>") || word.Contains("-->>") || word.Contains("->") || word.Contains("-->"))
            {
                // 包含消息箭头，当作消息处理
                Save(Buffer).ClearBuffer();
                return Forward<SeqSttChar>();
            }
            else
            {
                Save(Buffer).ClearBuffer();
                return Forward<SeqSttChar>();
            }
        }
    }
}