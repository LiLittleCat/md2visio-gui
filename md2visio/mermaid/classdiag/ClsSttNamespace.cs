using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttNamespace : SynState
    {
        public override SynState NextState()
        {
            SlideSpaces();

            if (!Ctx.Test(@"^(?<name>\w+)\s*\{"))
            {
                throw new SynException("expected namespace name and {", Ctx);
            }

            string name = Ctx.TestGroups["name"].Value;
            AddCompo("name", name);

            Slide(Ctx.TestGroups[0].Length - 1);

            return Save(name).Forward<ClsSttChar>();
        }
    }

    internal class ClsSttNamespaceEnd : SynState
    {
        public override SynState NextState()
        {
            return Save("}").Slide(1).Forward<ClsSttChar>();
        }
    }
}
