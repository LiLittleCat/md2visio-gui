using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttWord : SynState
    {
        public override SynState NextState()
        {
            SlideSpaces();

            if (Buffer.Length > 0)
            {
                if (ClsSttKeyword.IsKeyword(Buffer))
                {
                    return Forward<ClsSttKeyword>();
                }
                return Save(Buffer).ClearBuffer().Forward<ClsSttChar>();
            }

            return Forward<ClsSttChar>();
        }
    }
}
