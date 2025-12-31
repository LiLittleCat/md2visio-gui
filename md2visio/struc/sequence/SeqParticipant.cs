using md2visio.struc.figure;
using md2visio.struc.graph;
using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.sequence
{
    internal class SeqParticipant : INode
    {
        public string ID { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty; // 支持 "participant a as 用户" 语法
        public double X { get; set; } // 水平位置
        
        // Visio图形对象
        public Shape? TopShape { get; set; }      // 顶部参与者框
        public Shape? BottomShape { get; set; }   // 底部参与者框
        public Shape? LifelineShape { get; set; } // 生命线

        public Shape? VisioShape { get; set; } // INode接口要求

        public Container Container { get; set; } = Empty.Get<Container>();

        // INode接口要求的边集合
        public List<GEdge> InputEdges { get; } = new List<GEdge>();
        public List<GEdge> OutputEdges { get; } = new List<GEdge>();

        public SeqParticipant()
        {
        }

        public SeqParticipant(string id, string label)
        {
            ID = id;
            Label = label;
        }

        public string DisplayName => !string.IsNullOrEmpty(Alias) ? Alias : Label;

        public List<INode> InputNodes()
        {
            return InputEdges.Select(e => e.From).Cast<INode>().ToList();
        }

        public List<INode> OutputNodes()
        {
            return OutputEdges.Select(e => e.To).Cast<INode>().ToList();
        }
    }
}