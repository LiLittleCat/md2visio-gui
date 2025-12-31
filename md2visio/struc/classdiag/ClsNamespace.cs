using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.classdiag
{
    internal class ClsNamespace
    {
        public string Name { get; set; } = "";
        public List<string> ClassIds { get; set; } = new();
        public Shape? BorderShape { get; set; }
    }
}
