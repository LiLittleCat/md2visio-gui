using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttKeyword : SynState
    {
        static readonly Regex regKW = new(
            @"^(classDiagram|class|namespace|note|direction|click|link|callback|cssClass)$",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            string keyword = Buffer;
            Save(Buffer).ClearBuffer();

            // class 和 namespace 不使用参数检查，让状态机正常解析
            if (keyword == "class" || keyword == "namespace")
            {
                return Forward<ClsSttChar>();
            }

            if (ClsSttKeywordParam.HasParam(Ctx)) return Forward<ClsSttKeywordParam>();
            return Forward<ClsSttChar>();
        }

        public static bool IsKeyword(SynContext ctx)
        {
            return regKW.IsMatch(ctx.Cache.ToString());
        }

        public static bool IsKeyword(string word)
        {
            return regKW.IsMatch(word);
        }
    }
}
