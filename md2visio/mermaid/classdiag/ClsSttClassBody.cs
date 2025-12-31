using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttClassBody : SynState
    {
        static readonly Regex regBody = new(
            @"^\{(?<body>.*?)\}",
            RegexOptions.Compiled | RegexOptions.Singleline);

        static readonly Regex regMember = new(
            @"^(?<visibility>[+\-#~])?(?<content>.+)$",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            var match = regBody.Match(Ctx.Incoming.ToString());
            if (!match.Success)
            {
                throw new SynException("expected class body {...}", Ctx);
            }

            string body = match.Groups["body"].Value;
            int totalLen = match.Groups[0].Length;

            AddCompo("body", body);
            return Save($"{{{body}}}").Slide(totalLen).Forward<ClsSttChar>();
        }

        public static List<string> ParseMembers(string body)
        {
            var members = new List<string>();
            var lines = body.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed) && !trimmed.StartsWith("<<"))
                {
                    members.Add(trimmed);
                }
            }

            return members;
        }

        public static string? ParseAnnotation(string body)
        {
            var match = Regex.Match(body, @"<<(?<annot>[^>]+)>>");
            return match.Success ? match.Groups["annot"].Value : null;
        }
    }
}
