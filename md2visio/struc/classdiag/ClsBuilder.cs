using md2visio.Api;
using md2visio.mermaid.cmn;
using md2visio.mermaid.classdiag;
using md2visio.struc.figure;
using md2visio.vsdx.@base;
using System.Text.RegularExpressions;

namespace md2visio.struc.classdiag
{
    internal class ClsBuilder : FigureBuilder
    {
        readonly ClassDiagram diagram = new();
        readonly Stack<ClsNamespace> namespaceStack = new();
        ClsClass? currentClass = null;
        string? pendingClassId = null;
        string? pendingFromCard = null;

        public ClsBuilder(SttIterator iter, ConversionContext context, IVisioSession session)
            : base(iter, context, session) { }

        public override void Build(string outputFile)
        {
            while (iter.HasNext())
            {
                SynState cur = iter.Next();

                if (cur is SttMermaidStart) { }
                else if (cur is SttMermaidClose)
                {
                    diagram.ToVisio(outputFile, _context, _session);
                    break;
                }
                else if (cur is ClsSttKeyword) BuildKeyword();
                else if (cur is ClsSttWord) BuildWord();
                else if (cur is ClsSttClassBody) BuildClassBody();
                else if (cur is ClsSttInlineMember) BuildInlineMember();
                else if (cur is ClsSttRelation) BuildRelation();
                else if (cur is ClsSttAnnotation) BuildAnnotation();
                else if (cur is ClsSttCardinality) BuildCardinality();
                else if (cur is ClsSttNamespace) BuildNamespace();
                else if (cur is ClsSttNamespaceEnd) EndNamespace();
                else if (cur is SttComment) diagram.Config.LoadUserDirectiveFromComment(cur.Fragment);
                else if (cur is SttFrontMatter) diagram.Config.LoadUserFrontMatter(cur.Fragment);
            }
        }

        void BuildKeyword()
        {
            string kw = iter.Current.Fragment;
            SynState next = iter.PeekNext();

            switch (kw)
            {
                case "classDiagram":
                    break;

                case "class":
                    if (next is ClsSttWord || next is ClsSttKeywordParam)
                    {
                        var wordState = iter.Next();
                        string classId = wordState.Fragment;

                        var classGeneric = ParseGeneric(classId);
                        currentClass = diagram.GetOrCreateClass(classGeneric.id);
                        if (!string.IsNullOrEmpty(classGeneric.generic))
                        {
                            currentClass.GenericType = classGeneric.generic;
                        }

                        if (namespaceStack.Count > 0)
                        {
                            var ns = namespaceStack.Peek();
                            currentClass.Namespace = ns.Name;
                            if (!ns.ClassIds.Contains(classGeneric.id))
                            {
                                ns.ClassIds.Add(classGeneric.id);
                            }
                        }
                    }
                    break;

                case "namespace":
                    break;

                case "direction":
                    if (next is ClsSttKeywordParam)
                    {
                        iter.Next();
                    }
                    break;
            }
        }

        void BuildWord()
        {
            string word = iter.Current.Fragment;
            if (string.IsNullOrWhiteSpace(word)) return;

            var parsed = ParseGeneric(word);
            pendingClassId = parsed.id;

            currentClass = diagram.GetOrCreateClass(parsed.id);
            if (!string.IsNullOrEmpty(parsed.generic))
            {
                currentClass.GenericType = parsed.generic;
            }
        }

        void BuildClassBody()
        {
            if (currentClass == null) return;

            string body = iter.Current.GetPart("body");

            var annotation = ClsSttClassBody.ParseAnnotation(body);
            if (annotation != null)
            {
                currentClass.Annotation = annotation;
            }

            var members = ClsSttClassBody.ParseMembers(body);
            foreach (var memberText in members)
            {
                var member = ParseMember(memberText);
                currentClass.AddMember(member);
            }

            currentClass = null;
        }

        void BuildInlineMember()
        {
            if (currentClass == null && !string.IsNullOrEmpty(pendingClassId))
            {
                currentClass = diagram.GetOrCreateClass(pendingClassId);
            }

            if (currentClass == null) return;

            string memberText = iter.Current.GetPart("member");
            var member = ParseMember(memberText);
            currentClass.AddMember(member);
        }

        void BuildRelation()
        {
            string relSymbol = iter.Current.GetPart("relation").Trim();
            string? fromCard = pendingFromCard;
            pendingFromCard = null;

            SynState next = iter.PeekNext();
            string? toCard = null;
            string? label = null;

            if (next is ClsSttCardinality)
            {
                iter.Next();
                toCard = iter.Current.GetPart("cardinality");
                next = iter.PeekNext();
            }

            if (next is ClsSttWord)
            {
                iter.Next();
                string toClassId = iter.Current.Fragment;
                var toParsed = ParseGeneric(toClassId);

                next = iter.PeekNext();
                if (next is ClsSttInlineMember)
                {
                    iter.Next();
                    label = iter.Current.GetPart("member");
                }

                if (!string.IsNullOrEmpty(pendingClassId))
                {
                    var relation = new ClsRelation
                    {
                        FromClass = pendingClassId,
                        ToClass = toParsed.id,
                        Type = ClsRelation.ParseRelationType(relSymbol),
                        IsDecorationOnFrom = ClsRelation.CheckDecorationOnFrom(relSymbol),
                        FromCardinality = fromCard ?? "",
                        ToCardinality = toCard ?? "",
                        Label = label ?? ""
                    };
                    diagram.AddRelation(relation);

                    diagram.GetOrCreateClass(pendingClassId);
                    var toClass = diagram.GetOrCreateClass(toParsed.id);
                    if (!string.IsNullOrEmpty(toParsed.generic))
                    {
                        toClass.GenericType = toParsed.generic;
                    }
                }
            }

            pendingClassId = null;
            currentClass = null;
        }

        void BuildAnnotation()
        {
            string annotation = iter.Current.GetPart("annotation");

            SynState next = iter.PeekNext();
            if (next is ClsSttWord)
            {
                iter.Next();
                string classId = iter.Current.Fragment;
                var cls = diagram.GetOrCreateClass(classId);
                cls.Annotation = annotation;
            }
            else if (currentClass != null)
            {
                currentClass.Annotation = annotation;
            }
        }

        void BuildCardinality()
        {
            pendingFromCard = iter.Current.GetPart("cardinality");
        }

        void BuildNamespace()
        {
            string name = iter.Current.GetPart("name");
            var ns = diagram.GetOrCreateNamespace(name);
            namespaceStack.Push(ns);
        }

        void EndNamespace()
        {
            if (namespaceStack.Count > 0)
            {
                namespaceStack.Pop();
            }
        }

        ClsMember ParseMember(string text)
        {
            var (visibility, content) = ClsSttInlineMember.ParseMember(text);
            bool isMethod = ClsSttInlineMember.IsMethod(content);

            var member = new ClsMember
            {
                Visibility = ClsMember.ParseVisibility(visibility),
                RawText = text,
                IsMethod = isMethod
            };

            if (isMethod)
            {
                var methodMatch = Regex.Match(content, @"^(?<name>\w+)\s*\((?<params>[^)]*)\)\s*(?<return>.*)$");
                if (methodMatch.Success)
                {
                    member.Name = methodMatch.Groups["name"].Value;
                    member.Parameters = methodMatch.Groups["params"].Value;
                    member.ReturnType = methodMatch.Groups["return"].Value.Trim();
                }
                else
                {
                    member.Name = content;
                }
            }
            else
            {
                var propMatch = Regex.Match(content, @"^(?<name>\w+)\s*(?<type>.*)$");
                if (propMatch.Success)
                {
                    member.Name = propMatch.Groups["name"].Value;
                    member.Type = propMatch.Groups["type"].Value.Trim();
                }
                else
                {
                    member.Name = content;
                }
            }

            return member;
        }

        (string id, string? generic) ParseGeneric(string text)
        {
            var match = Regex.Match(text, @"^(?<id>\w+)~(?<generic>[^~]+)~$");
            if (match.Success)
            {
                return (match.Groups["id"].Value, match.Groups["generic"].Value);
            }
            return (text, null);
        }
    }
}
