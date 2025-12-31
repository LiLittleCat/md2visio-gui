using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttAnnotation : SynState
    {
        public override SynState NextState()
        {
            if (!Ctx.Test(@"^<<(?<annot>[^>]+)>>"))
            {
                throw new SynException("expected annotation <<...>>", Ctx);
            }

            string annotation = Ctx.TestGroups["annot"].Value;
            int totalLen = Ctx.TestGroups[0].Length;

            AddCompo("annotation", annotation);
            return Save($"<<{annotation}>>").Slide(totalLen).Forward<ClsSttChar>();
        }
    }
}
