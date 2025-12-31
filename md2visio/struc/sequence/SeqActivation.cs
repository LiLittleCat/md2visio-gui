using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.sequence
{
    internal class SeqActivation
    {
        public string ParticipantId { get; set; } = string.Empty;
        public double StartY { get; set; }
        public double EndY { get; set; }
        public Shape? ActivationShape { get; set; }
        public int NestingLevel { get; set; } = 0; // 支持嵌套激活

        public double Height => StartY - EndY; // 注意：Y轴向下递减
        public double CenterY => (StartY + EndY) / 2;

        public SeqActivation()
        {
        }

        public SeqActivation(string participantId, double startY)
        {
            ParticipantId = participantId;
            StartY = startY;
            EndY = startY; // 初始时结束位置与开始位置相同
        }

        public void SetEnd(double endY)
        {
            EndY = endY;
        }

        public bool IsActive(double y)
        {
            return y <= StartY && y >= EndY;
        }

        public override string ToString()
        {
            return $"Activation({ParticipantId}, {StartY:F1}-{EndY:F1}, Level:{NestingLevel})";
        }
    }
}