using md2visio.Api;
using md2visio.mermaid.cmn;
using md2visio.vsdx.@base;

namespace md2visio.struc.figure
{
    internal abstract class FigureBuilder
    {
        protected SttIterator iter;
        protected ConversionContext _context;
        protected IVisioSession _session;

        public FigureBuilder(SttIterator iter, ConversionContext context, IVisioSession session)
        {
            this.iter = iter;
            this._context = context;
            this._session = session;
        }

        abstract public void Build(string outputFile);
    }
}
