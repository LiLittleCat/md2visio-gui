using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttChar : SynState
    {
        static readonly Regex regRelation = new(
            @"^(<\|--|\*--|o--|-->|\.\.>|\.\.\|>|<--|--\*|--o|<\.\.|<\|\.\.|\.\.|--)",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            if (next == "%") return Forward<SttPercent>();
            if (next == "\n")
            {
                if (Buffer.Length > 0)
                {
                    Create<ClsSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<SttFinishFlag>();
            }
            if (next == "`") return Forward<SttMermaidClose>();
            if (next == " " || next == "\t") return Forward<ClsSttWord>();

            if (next == "{")
            {
                if (Buffer.Length > 0)
                {
                    Create<ClsSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<ClsSttClassBody>();
            }

            if (next == ":") return Forward<ClsSttInlineMember>();
            if (next == "<") return CheckRelationOrAnnotation();
            if (next == "\"") return Forward<ClsSttCardinality>();

            if (IsRelationStart()) return Forward<ClsSttRelation>();

            return Take().Forward<ClsSttChar>();
        }

        SynState CheckRelationOrAnnotation()
        {
            if (Ctx.Test(@"^<<(?<annot>[^>]+)>>"))
            {
                return Forward<ClsSttAnnotation>();
            }
            if (IsRelationStart())
            {
                return Forward<ClsSttRelation>();
            }
            return Take().Forward<ClsSttChar>();
        }

        bool IsRelationStart()
        {
            return regRelation.IsMatch(Ctx.Incoming.ToString());
        }
    }
}
