using md2visio.Api;
using md2visio.struc.figure;
using md2visio.vsdx;
using md2visio.vsdx.@base;

namespace md2visio.struc.packet
{
    internal class Packet : Figure
    {
        public override void ToVisio(string path, ConversionContext context, IVisioSession session)
        {
            new VBuilderPac(this, context, session).Build(path);
        }
    }
}
