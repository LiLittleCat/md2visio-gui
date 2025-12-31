using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttInlineMember : SynState
    {
        static readonly Regex regInline = new(
            @"^:\s*(?<member>.+)$",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            if (!Ctx.Test(@"^:\s*(?<member>[^\n]+)"))
            {
                throw new SynException("expected inline member definition", Ctx);
            }

            string member = Ctx.TestGroups["member"].Value.Trim();
            int totalLen = Ctx.TestGroups[0].Length;

            AddCompo("member", member);
            return Save($":{member}").Slide(totalLen).Forward<ClsSttChar>();
        }

        public static (string visibility, string content) ParseMember(string memberText)
        {
            string visibility = "+";
            string content = memberText;

            if (memberText.Length > 0)
            {
                char first = memberText[0];
                if (first == '+' || first == '-' || first == '#' || first == '~')
                {
                    visibility = first.ToString();
                    content = memberText.Substring(1).Trim();
                }
            }

            return (visibility, content);
        }

        public static bool IsMethod(string content)
        {
            return content.Contains('(') && content.Contains(')');
        }
    }
}
