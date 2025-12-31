using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.classdiag
{
    internal class ClsRelation
    {
        public string FromClass { get; set; } = "";
        public string ToClass { get; set; } = "";
        public ClsRelationType Type { get; set; } = ClsRelationType.Association;
        public bool IsDecorationOnFrom { get; set; } = true;
        public string Label { get; set; } = "";
        public string FromCardinality { get; set; } = "";
        public string ToCardinality { get; set; } = "";
        public Shape? VisioShape { get; set; }

        public static bool CheckDecorationOnFrom(string symbol)
        {
            return symbol.StartsWith("<") || symbol.StartsWith("*") || symbol.StartsWith("o");
        }

        public static ClsRelationType ParseRelationType(string symbol)
        {
            return symbol switch
            {
                "<|--" or "--|>" => ClsRelationType.Inheritance,
                "*--" or "--*" => ClsRelationType.Composition,
                "o--" or "--o" => ClsRelationType.Aggregation,
                "-->" or "<--" => ClsRelationType.Association,
                "..>" or "<.." => ClsRelationType.Dependency,
                "..|>" or "<|.." => ClsRelationType.Realization,
                "--" => ClsRelationType.Link,
                ".." => ClsRelationType.DashedLink,
                _ => ClsRelationType.Association
            };
        }
    }
}
