using md2visio.Api;
using md2visio.struc.figure;
using md2visio.vsdx;
using md2visio.vsdx.@base;

namespace md2visio.struc.classdiag
{
    internal class ClassDiagram : Figure
    {
        Dictionary<string, ClsClass> classDict = new();
        Dictionary<string, ClsNamespace> namespaceDict = new();
        List<ClsRelation> relations = new();

        public ClassDiagram() { }

        public Dictionary<string, ClsClass> Classes => classDict;
        public List<ClsRelation> Relations => relations;
        public Dictionary<string, ClsNamespace> Namespaces => namespaceDict;

        public ClsClass GetOrCreateClass(string id)
        {
            if (!classDict.ContainsKey(id))
            {
                classDict[id] = new ClsClass { ID = id, Label = id };
            }
            return classDict[id];
        }

        public void AddRelation(ClsRelation relation)
        {
            relations.Add(relation);
        }

        public ClsNamespace GetOrCreateNamespace(string name)
        {
            if (!namespaceDict.ContainsKey(name))
            {
                namespaceDict[name] = new ClsNamespace { Name = name };
            }
            return namespaceDict[name];
        }

        public override void ToVisio(string path, ConversionContext context, IVisioSession session)
        {
            new VBuilderCls(this, context, session).Build(path);
        }
    }
}
