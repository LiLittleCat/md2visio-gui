using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiag
{
    internal class ClsSttRelation : SynState
    {
        static readonly Regex regRelation = new(
            @"^(?<rel><\|--|\*--|o--|-->|\.\.>|\.\.\|>|<--|--\*|--o|<\.\.|<\|\.\.|\.\.|--)",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            var match = regRelation.Match(Ctx.Incoming.ToString());
            if (!match.Success)
            {
                throw new SynException("expected relation operator", Ctx);
            }

            string relation = match.Groups["rel"].Value;
            AddCompo("relation", relation);

            return Save(relation).Slide(relation.Length).Forward<ClsSttRelationEnd>();
        }

        public static string NormalizeRelation(string rel)
        {
            return rel switch
            {
                "--|>" => "<|--",
                "--*" => "*--",
                "--o" => "o--",
                "<--" => "-->",
                "<.." => "..>",
                "<|.." => "..|>",
                _ => rel
            };
        }
    }

    internal class ClsSttRelationEnd : SynState
    {
        public override SynState NextState()
        {
            SlideSpaces();

            if (Ctx.Test(@"^:(?<label>[^\n]+)"))
            {
                string label = Ctx.TestGroups["label"].Value.Trim();
                AddCompo("label", label);
                Slide(Ctx.TestGroups[0].Length);
            }

            return Forward<ClsSttChar>();
        }
    }
}
