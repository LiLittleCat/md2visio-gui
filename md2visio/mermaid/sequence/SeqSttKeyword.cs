using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.sequence
{
    internal class SeqSttKeyword : SynState
    {
        public override SynState NextState()
        {
            if (!IsKeyword(Ctx)) throw new SynException($"unknown keyword '{Buffer}'", Ctx);

            Save(Buffer).ClearBuffer();
            
            string keyword = Fragment;
            
            // 根据关键字类型决定下一个状态
            switch (keyword)
            {
                case "sequenceDiagram":
                    return Forward<SeqSttChar>();
                    
                case "participant":
                    return Forward<SeqSttChar>();
                    
                case "activate":
                case "deactivate":
                    return Forward<SeqSttChar>();
                    
                case "note":
                    // TODO: 实现note语法解析
                    return Forward<SeqSttChar>();
                    
                default:
                    return Forward<SeqSttChar>();
            }
        }

        public static bool IsKeyword(SynContext ctx)
        {
            return Regex.IsMatch(ctx.Cache.ToString(),
                "^(sequenceDiagram|participant|activate|deactivate|note|loop|alt|else|opt|par|critical|break|end|autonumber)$");
        }
    }
}