using md2visio.Api;
using md2visio.struc.pie;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderPie : VFigureBuilder<Pie>
    {
        public VBuilderPie(Pie figure, ConversionContext context, IVisioSession session)
            : base(figure, context, session) { }

        protected override void ExecuteBuild()
        {
            new VDrawerPie(figure, _session.Application, _context).Draw();
        }
    }
}
