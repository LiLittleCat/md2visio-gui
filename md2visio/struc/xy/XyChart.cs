using md2visio.Api;
using md2visio.struc.figure;
using md2visio.vsdx;
using md2visio.vsdx.@base;

namespace md2visio.struc.xy
{
    internal class XyChart : Figure
    {
        public XyAxis XAxis { get; set; } = Empty.Get<XyAxis>();
        public XyAxis YAxis { get; set; } = Empty.Get<XyAxis>();
        public MmdJsonArray Bar { get; set; } = new MmdJsonArray();
        public MmdJsonArray Line { get; set; } = new MmdJsonArray();

        public XyChart()
        {
        }

        public override void ToVisio(string path, ConversionContext context, IVisioSession session)
        {
            new VBuilderXy(this, context, session).Build(path);
        }
    }
}
