using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.classdiag
{
    internal class ClsClass
    {
        public string ID { get; set; } = "";
        public string Label { get; set; } = "";
        public string Annotation { get; set; } = "";
        public string? Namespace { get; set; }
        public string? GenericType { get; set; }
        public List<ClsMember> Members { get; set; } = new();
        public Shape? VisioShape { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public List<ClsMember> Properties => Members.Where(m => !m.IsMethod).ToList();
        public List<ClsMember> Methods => Members.Where(m => m.IsMethod).ToList();

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(GenericType))
                    return Label;
                return $"{Label}<{GenericType}>";
            }
        }

        public void AddMember(ClsMember member)
        {
            Members.Add(member);
        }
    }
}
