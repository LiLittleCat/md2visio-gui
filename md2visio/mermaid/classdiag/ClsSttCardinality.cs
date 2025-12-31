using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttCardinality : SynState
    {
        public override SynState NextState()
        {
            if (!Ctx.Test(@"^""(?<card>[^""]+)"""))
            {
                throw new SynException("expected cardinality \"...\"", Ctx);
            }

            string cardinality = Ctx.TestGroups["card"].Value;
            int totalLen = Ctx.TestGroups[0].Length;

            AddCompo("cardinality", cardinality);
            return Save($"\"{cardinality}\"").Slide(totalLen).Forward<ClsSttChar>();
        }
    }
}
