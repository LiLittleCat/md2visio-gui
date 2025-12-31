using md2visio.Api;
using md2visio.struc.journey;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderJo : VFigureBuilder<Journey>
    {
        public VBuilderJo(Journey figure, ConversionContext context, IVisioSession session)
            : base(figure, context, session) { }

        protected override void ExecuteBuild()
        {
            new VDrawerJo(figure, _session.Application, _context).Draw();
        }
    }
}
