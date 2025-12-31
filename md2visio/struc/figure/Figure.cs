using md2visio.Api;
using md2visio.vsdx.@base;

namespace md2visio.struc.figure
{
    internal abstract class Figure : Container
    {
        string title = string.Empty;
        Config config;

        public string Title
        {
            get => title;
            set => title = value.Trim();
        }
        public MmdFrontMatter FrontMatter { get => config.UserFrontMatter; }
        public MmdJsonObj Directive { get => config.UserDirective; }
        public Config Config { get => config; }

        public Figure()
        {
            config = new Config(this);
        }

        public abstract void ToVisio(string path, ConversionContext context, IVisioSession session);
    }

    class EmptyFigure : Figure
    {
        public static EmptyFigure Instance = new EmptyFigure();
        public EmptyFigure() { }

        public override void ToVisio(string path, ConversionContext context, IVisioSession session)
        {
            throw new NotImplementedException();
        }
    }
}
