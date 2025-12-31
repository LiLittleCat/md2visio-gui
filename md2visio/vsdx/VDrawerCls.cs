using md2visio.Api;
using md2visio.struc.classdiag;
using md2visio.vsdx.@base;
using Microsoft.Office.Interop.Visio;
using System.Text;

namespace md2visio.vsdx
{
    internal class VDrawerCls : VFigureDrawer<ClassDiagram>
    {
        const double CLASS_WIDTH = 2.0;
        const double CLASS_MIN_HEIGHT = 0.6;
        const double MEMBER_HEIGHT = 0.2;
        const double HEADER_HEIGHT = 0.35;
        const double SPACING_H = 1.2;
        const double SPACING_V = 0.8;

        readonly List<ClsClass> drawnClasses = new();

        public VDrawerCls(ClassDiagram figure, Application visioApp, ConversionContext context)
            : base(figure, visioApp, context) { }

        public override void Draw()
        {
            EnsureVisible();
            PauseForViewing(300);

            // 1. Draw all classes first (at origin)
            DrawClasses();
            PauseForViewing(500);

            // 2. Layout using BFS tree
            LayoutNodes();
            PauseForViewing(500);

            // 3. Draw relationships
            DrawRelations();
            PauseForViewing(300);

            DrawNamespaceBorders();
        }

        #region BFS Tree Layout

        void LayoutNodes()
        {
            var classes = figure.Classes.Values.ToList();
            if (classes.Count == 0) return;

            var inheritance = BuildInheritanceGraph();
            var sortedNodes = SortNodesBFS(classes, inheritance);

            // Calculate subtree widths (Post-order traversal)
            var subtreeCrossSizes = new Dictionary<string, double>();

            for (int i = sortedNodes.Count - 1; i >= 0; i--)
            {
                var node = sortedNodes[i];
                if (node.VisioShape == null) continue;

                var childrenIds = inheritance.ContainsKey(node.ID) ? inheritance[node.ID] : new List<string>();
                var children = childrenIds
                    .Select(id => figure.Classes.GetValueOrDefault(id))
                    .Where(c => c != null && c.VisioShape != null)
                    .ToList();

                double selfW = Width(node.VisioShape);

                if (children.Count == 0)
                {
                    subtreeCrossSizes[node.ID] = selfW;
                }
                else
                {
                    double childrenW = children.Sum(c => subtreeCrossSizes.GetValueOrDefault(c!.ID, Width(c.VisioShape)))
                                     + (children.Count - 1) * SPACING_H;
                    subtreeCrossSizes[node.ID] = Math.Max(selfW, childrenW);
                }
            }

            // Find roots (nodes not being children)
            var processed = new HashSet<string>();
            var allChildren = new HashSet<string>();
            foreach (var list in inheritance.Values)
                foreach (var c in list)
                    allChildren.Add(c);

            var roots = sortedNodes.Where(n => !allChildren.Contains(n.ID)).ToList();

            // Layout (Pre-order traversal)
            double currentRootX = 1.0;
            double startY = 10.0;

            foreach (var root in roots)
            {
                if (root.VisioShape == null) continue;
                double rootW = subtreeCrossSizes.GetValueOrDefault(root.ID, Width(root.VisioShape));
                PlaceTree(root, currentRootX + rootW / 2, startY, inheritance, subtreeCrossSizes, processed);
                currentRootX += rootW + SPACING_H;
            }
        }

        List<ClsClass> SortNodesBFS(List<ClsClass> nodes, Dictionary<string, List<string>> inheritance)
        {
            var nodeSet = new HashSet<ClsClass>(nodes);
            var sorted = new List<ClsClass>();
            var visited = new HashSet<string>();
            var queue = new Queue<ClsClass>();

            foreach (var startNode in nodes)
            {
                if (!nodeSet.Contains(startNode) || visited.Contains(startNode.ID)) continue;

                visited.Add(startNode.ID);
                queue.Enqueue(startNode);

                while (queue.Count > 0)
                {
                    var n = queue.Dequeue();
                    sorted.Add(n);

                    if (inheritance.TryGetValue(n.ID, out var children))
                    {
                        foreach (var childId in children)
                        {
                            if (figure.Classes.TryGetValue(childId, out var child) && !visited.Contains(childId))
                            {
                                visited.Add(childId);
                                queue.Enqueue(child);
                            }
                        }
                    }
                }
            }
            return sorted;
        }

        void PlaceTree(ClsClass node, double centerX, double topY,
            Dictionary<string, List<string>> inheritance,
            Dictionary<string, double> subtreeCrossSizes,
            HashSet<string> processed)
        {
            if (processed.Contains(node.ID) || node.VisioShape == null) return;
            processed.Add(node.ID);

            double selfH = Height(node.VisioShape);

            // Move node to position
            MoveTo(node.VisioShape, centerX, topY - selfH / 2);
            PauseForViewing(100);

            if (!inheritance.TryGetValue(node.ID, out var childrenIds)) return;

            var children = childrenIds
                .Select(id => figure.Classes.GetValueOrDefault(id))
                .Where(c => c != null && c.VisioShape != null && !processed.Contains(c!.ID))
                .ToList();

            if (children.Count == 0) return;

            double childrenTotalW = children.Sum(c => subtreeCrossSizes.GetValueOrDefault(c!.ID, Width(c.VisioShape)))
                                  + (children.Count - 1) * SPACING_H;

            double startChildX = centerX - childrenTotalW / 2;
            double nextY = topY - selfH - SPACING_V;

            foreach (var child in children)
            {
                double childW = subtreeCrossSizes.GetValueOrDefault(child!.ID, Width(child.VisioShape));
                double childCenterX = startChildX + childW / 2;

                PlaceTree(child!, childCenterX, nextY, inheritance, subtreeCrossSizes, processed);
                startChildX += childW + SPACING_H;
            }
        }

        Dictionary<string, List<string>> BuildInheritanceGraph()
        {
            var graph = new Dictionary<string, List<string>>();

            foreach (var rel in figure.Relations)
            {
                // Include all hierarchical relationships for tree layout
                if (rel.Type == ClsRelationType.Inheritance || rel.Type == ClsRelationType.Realization ||
                    rel.Type == ClsRelationType.Composition || rel.Type == ClsRelationType.Aggregation)
                {
                    string parent = rel.IsDecorationOnFrom ? rel.FromClass : rel.ToClass;
                    string child = rel.IsDecorationOnFrom ? rel.ToClass : rel.FromClass;
                    if (!graph.ContainsKey(parent))
                        graph[parent] = new List<string>();
                    graph[parent].Add(child);
                }
            }

            return graph;
        }

        #endregion

        #region Draw Classes

        void DrawClasses()
        {
            foreach (var cls in figure.Classes.Values)
            {
                DrawClass(cls);
                drawnClasses.Add(cls);
                PauseForViewing(150);
            }
        }

        void DrawClass(ClsClass cls)
        {
            double height = GetClassHeight(cls);

            // Draw at origin first, will be repositioned by LayoutNodes
            Shape mainShape = visioPage.DrawRectangle(0, 0, CLASS_WIDTH, height);

            cls.VisioShape = mainShape;

            mainShape.CellsU["LineWeight"].FormulaU = "1 pt";
            SetFillForegnd(mainShape, "config.themeVariables.primaryColor");
            SetLineColor(mainShape, "config.themeVariables.primaryBorderColor");

            StringBuilder textContent = new();

            if (!string.IsNullOrEmpty(cls.Annotation))
            {
                textContent.AppendLine($"<<{cls.Annotation}>>");
            }

            textContent.AppendLine(cls.DisplayName);

            if (cls.Properties.Count > 0)
            {
                textContent.AppendLine("─────────────");
                foreach (var prop in cls.Properties)
                {
                    textContent.AppendLine(prop.ToDisplayString());
                }
            }

            if (cls.Methods.Count > 0)
            {
                textContent.AppendLine("─────────────");
                foreach (var method in cls.Methods)
                {
                    textContent.AppendLine(method.ToDisplayString());
                }
            }

            mainShape.Text = textContent.ToString().TrimEnd();
            mainShape.CellsU["VerticalAlign"].FormulaU = "0";
            mainShape.CellsU["Para.HorzAlign"].FormulaU = "0";
            mainShape.CellsU["Char.Size"].FormulaU = "9 pt";

            SetTextColor(mainShape, "config.themeVariables.primaryTextColor");
        }

        double GetClassHeight(ClsClass cls)
        {
            double height = HEADER_HEIGHT;

            if (!string.IsNullOrEmpty(cls.Annotation))
                height += 0.15;

            if (cls.Properties.Count > 0)
                height += cls.Properties.Count * MEMBER_HEIGHT + 0.1;

            if (cls.Methods.Count > 0)
                height += cls.Methods.Count * MEMBER_HEIGHT + 0.1;

            return Math.Max(CLASS_MIN_HEIGHT, height);
        }

        #endregion

        #region Draw Relations

        void DrawRelations()
        {
            var drawnRelations = new HashSet<string>();

            foreach (var relation in figure.Relations)
            {
                string key = $"{relation.FromClass}->{relation.ToClass}";
                if (drawnRelations.Contains(key)) continue;

                if (!figure.Classes.TryGetValue(relation.FromClass, out var fromClass) ||
                    !figure.Classes.TryGetValue(relation.ToClass, out var toClass))
                    continue;

                if (fromClass.VisioShape == null || toClass.VisioShape == null)
                    continue;

                DrawRelation(relation, fromClass, toClass);
                drawnRelations.Add(key);
                PauseForViewing(100);
            }
        }

        void DrawRelation(ClsRelation relation, ClsClass fromClass, ClsClass toClass)
        {
            Shape connector = CreateConnector(relation);

            if (!string.IsNullOrEmpty(relation.Label))
            {
                connector.Text = relation.Label;
                connector.CellsU["Char.Size"].FormulaU = "8 pt";
            }

            // Use AutoConnect + Delete pattern like VDrawerG (flowchart)
            fromClass.VisioShape!.AutoConnect(toClass.VisioShape!, VisAutoConnectDir.visAutoConnectDirNone, connector);
            connector.Delete();
        }

        Shape CreateConnector(ClsRelation relation)
        {
            Master? master = GetMaster("-");
            Shape connector = visioPage.Drop(master, 0, 0);
            bool decorationOnFrom = relation.IsDecorationOnFrom;

            // Initialize both ends to NO arrow (prevents double-arrow bug)
            connector.CellsU["BeginArrow"].FormulaU = "0";
            connector.CellsU["EndArrow"].FormulaU = "0";
            connector.CellsU["BeginArrowSize"].FormulaU = "2";
            connector.CellsU["EndArrowSize"].FormulaU = "2";

            switch (relation.Type)
            {
                case ClsRelationType.Inheritance:
                    connector.CellsU["LinePattern"].FormulaU = "1";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "4";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "4";
                    break;

                case ClsRelationType.Realization:
                    connector.CellsU["LinePattern"].FormulaU = "2";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "4";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "4";
                    break;

                case ClsRelationType.Composition:
                    connector.CellsU["LinePattern"].FormulaU = "1";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "12";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "12";
                    break;

                case ClsRelationType.Aggregation:
                    connector.CellsU["LinePattern"].FormulaU = "1";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "11";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "11";
                    break;

                case ClsRelationType.Association:
                    connector.CellsU["LinePattern"].FormulaU = "1";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "1";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "1";
                    break;

                case ClsRelationType.Dependency:
                    connector.CellsU["LinePattern"].FormulaU = "2";
                    if (decorationOnFrom)
                        connector.CellsU["BeginArrow"].FormulaU = "1";
                    else
                        connector.CellsU["EndArrow"].FormulaU = "1";
                    break;

                case ClsRelationType.Link:
                    connector.CellsU["LinePattern"].FormulaU = "1";
                    break;

                case ClsRelationType.DashedLink:
                    connector.CellsU["LinePattern"].FormulaU = "2";
                    break;
            }

            connector.CellsU["LineWeight"].FormulaU = "0.75 pt";
            SetLineColor(connector, "config.themeVariables.lineColor");

            return connector;
        }

        #endregion

        #region Namespace Borders

        void DrawNamespaceBorders()
        {
            foreach (var ns in figure.Namespaces.Values)
            {
                if (ns.ClassIds.Count == 0) continue;

                var nsClasses = ns.ClassIds
                    .Where(id => figure.Classes.ContainsKey(id))
                    .Select(id => figure.Classes[id])
                    .Where(c => c.VisioShape != null)
                    .ToList();

                if (nsClasses.Count == 0) continue;

                VBoundary bound = new VBoundary(true);
                foreach (var cls in nsClasses)
                {
                    bound.Expand(cls.VisioShape!);
                }

                double padding = 0.2;
                Shape border = visioPage.DrawRectangle(
                    bound.Left - padding,
                    bound.Bottom - padding - 0.25,
                    bound.Right + padding,
                    bound.Top + padding + 0.25);

                border.CellsU["LinePattern"].FormulaU = "2";
                border.CellsU["FillPattern"].FormulaU = "0";
                border.CellsU["LineWeight"].FormulaU = "0.5 pt";
                border.Text = ns.Name;
                border.CellsU["VerticalAlign"].FormulaU = "0";
                border.CellsU["TxtPinY"].FormulaU = "Height - 0.15 in";

                SetLineColor(border, "config.themeVariables.secondaryBorderColor");
                SetTextColor(border, "config.themeVariables.secondaryTextColor");

                visioApp.DoCmd((short)VisUICmds.visCmdObjectSendToBack);

                ns.BorderShape = border;
            }
        }

        #endregion
    }
}
