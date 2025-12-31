using md2visio.Api;
using md2visio.mermaid.cmn;
using md2visio.mermaid.packet;
using md2visio.struc.figure;
using md2visio.vsdx.@base;

namespace md2visio.struc.packet
{
    internal class PacBuilder : FigureBuilder
    {
        readonly Packet packet = new();

        public PacBuilder(SttIterator iter, ConversionContext context, IVisioSession session)
            : base(iter, context, session) { }

        public override void Build(string outputFile)
        {
            while (iter.HasNext())
            {
                SynState cur = iter.Next();
                if (cur is SttMermaidStart) { }
                if (cur is SttMermaidClose) { packet.ToVisio(outputFile, _context, _session); break; }
                if (cur is PacSttTuple) { BuildBits((PacSttTuple) cur); }
                if (cur is SttComment) { packet.Config.LoadUserDirectiveFromComment(cur.Fragment); }
                if (cur is SttFrontMatter) { packet.Config.LoadUserFrontMatter(cur.Fragment); } 
            }
        }

        void BuildBits(PacSttTuple tuple)
        {
            PacBlock bits = new(tuple.GetPart("bits"), tuple.GetPart("name"));
            packet.AddInnerNode(bits);
        }
    }
}
