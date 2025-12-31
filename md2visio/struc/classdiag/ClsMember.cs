namespace md2visio.struc.classdiag
{
    internal class ClsMember
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public ClsVisibility Visibility { get; set; } = ClsVisibility.Public;
        public bool IsMethod { get; set; }
        public string Parameters { get; set; } = "";
        public string ReturnType { get; set; } = "";
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public string RawText { get; set; } = "";

        public string VisibilitySymbol => Visibility switch
        {
            ClsVisibility.Public => "+",
            ClsVisibility.Private => "-",
            ClsVisibility.Protected => "#",
            ClsVisibility.Internal => "~",
            _ => "+"
        };

        public static ClsVisibility ParseVisibility(string symbol)
        {
            return symbol switch
            {
                "+" => ClsVisibility.Public,
                "-" => ClsVisibility.Private,
                "#" => ClsVisibility.Protected,
                "~" => ClsVisibility.Internal,
                _ => ClsVisibility.Public
            };
        }

        public string ToDisplayString()
        {
            if (IsMethod)
            {
                string ret = string.IsNullOrEmpty(ReturnType) ? "" : $" {ReturnType}";
                return $"{VisibilitySymbol}{Name}({Parameters}){ret}";
            }
            else
            {
                string type = string.IsNullOrEmpty(Type) ? "" : $" {Type}";
                return $"{VisibilitySymbol}{Name}{type}";
            }
        }
    }
}
