using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttKeywordParam : SttKeywordParam
    {
        public override SynState NextState()
        {
            return Save(ExpectedGroups["param"].Value).Forward<ClsSttChar>();
        }
    }
}
