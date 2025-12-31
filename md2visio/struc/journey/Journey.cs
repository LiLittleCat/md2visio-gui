using md2visio.Api;
using md2visio.struc.figure;
using md2visio.vsdx;
using md2visio.vsdx.@base;

namespace md2visio.struc.journey
{
    internal class Journey : Figure
    {
        public Journey() { }

        public HashSet<string> JoinerSet()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (INode section in innerNodes.Values)
            {
                foreach (string joiner in ((JoSection)section).JoinerSet())
                    set.Add(joiner);
            }
            return set;
        }

        public override void ToVisio(string path, ConversionContext context, IVisioSession session)
        {
            new VBuilderJo(this, context, session).Build(path);
        }
    }
}
