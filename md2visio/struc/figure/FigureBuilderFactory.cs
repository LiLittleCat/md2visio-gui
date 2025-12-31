using md2visio.mermaid.cmn;
using md2visio.Api;
using md2visio.vsdx.@base;
using System.Reflection;

namespace md2visio.struc.figure
{
    internal class FigureBuilderFactory
    {
        string outputFile;
        string? dir = string.Empty, name = string.Empty;
        Dictionary<string, Type> builderDict = TypeMap.BuilderMap;
        SttIterator iter;
        int count = 1;
        bool isFileMode = false;

        // 注入的依赖
        private readonly ConversionContext _context;
        private readonly IVisioSession _session;

        public FigureBuilderFactory(SttIterator iter, ConversionContext context, IVisioSession session)
        {
            this.iter = iter;
            this._context = context;
            this._session = session;
            outputFile = iter.Context.InputFile;
        }

        public void Build(string outputFile)
        {
            this.outputFile = outputFile;
            InitOutputPath();
            BuildFigures();
        }

        public void BuildFigures()
        {
            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigures: 开始构建图表");
                _context.Log($"[DEBUG] BuildFigures: iter.HasNext() = {iter.HasNext()}");
                if (iter.Context?.StateList != null)
                {
                    _context.Log($"[DEBUG] BuildFigures: StateList.Count = {iter.Context.StateList.Count}");
                    _context.Log($"[DEBUG] BuildFigures: iter.Pos = {iter.Pos}");

                    for (int i = 0; i < iter.Context.StateList.Count; i++)
                    {
                        var state = iter.Context.StateList[i];
                        _context.Log($"[DEBUG] StateList[{i}]: Type={state.GetType().Name}, Fragment='{state.Fragment}'");
                    }
                }
            }

            while (iter.HasNext())
            {
                List<SynState> list = iter.Context.StateList;
                for (int pos = iter.Pos + 1; pos < list.Count; ++pos)
                {
                    string word = list[pos].Fragment;

                    if (_context.Debug)
                    {
                        _context.Log($"[DEBUG] BuildFigures: 检查位置 {pos}, Fragment = '{word}'");
                        _context.Log($"[DEBUG] BuildFigures: SttFigureType.IsFigure('{word}') = {SttFigureType.IsFigure(word)}");
                    }

                    if (SttFigureType.IsFigure(word))
                    {
                        if (_context.Debug)
                        {
                            _context.Log($"[DEBUG] BuildFigures: 找到图表类型 '{word}'，开始构建");
                        }
                        BuildFigure(word);
                    }
                }
            }
        }

        public void Quit()
        {
            // Quit 逻辑已移至 VisioSession.Dispose()
            // 此方法保留为空，供向后兼容
        }

        void BuildFigure(string figureType)
        {
            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: 开始构建图表类型 '{figureType}'");
                _context.Log($"[DEBUG] BuildFigure: builderDict.ContainsKey('{figureType}') = {builderDict.ContainsKey(figureType)}");
            }

            if (!builderDict.ContainsKey(figureType))
                throw new NotImplementedException($"'{figureType}' builder not implemented");

            Type type = builderDict[figureType];

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: Builder类型 = {type.Name}");
            }

            // 使用注入的 session 和 context 创建 Builder
            object? obj = Activator.CreateInstance(type, iter, _context, _session);
            MethodInfo? method = type.GetMethod("Build", BindingFlags.Public | BindingFlags.Instance, null,
                new Type[] { typeof(string) }, null);

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: 创建Builder实例成功 = {obj != null}");
                _context.Log($"[DEBUG] BuildFigure: 找到Build方法 = {method != null}");
            }

            string outputFilePath;
            if (isFileMode)
            {
                outputFilePath = $"{dir}\\{name}.vsdx";
            }
            else
            {
                outputFilePath = $"{dir}\\{name}{count++}.vsdx";
            }

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] 构建图表: {figureType}");
                _context.Log($"[DEBUG] 输出模式: {(isFileMode ? "文件模式" : "目录模式")}");
                _context.Log($"[DEBUG] 输出路径: {outputFilePath}");
                _context.Log($"[DEBUG] 输出目录: {dir}");
                _context.Log($"[DEBUG] 文件名: {name}");
            }

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: 准备调用 {type.Name}.Build('{outputFilePath}')");
            }

            try
            {
                method?.Invoke(obj, new object[] { outputFilePath });

                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] BuildFigure: {type.Name}.Build() 调用完成");
                }
            }
            catch (Exception ex)
            {
                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] BuildFigure: {type.Name}.Build() 调用失败: {ex.Message}");
                    _context.Log($"[DEBUG] BuildFigure: 异常类型: {ex.GetType().Name}");
                    if (ex.InnerException != null)
                    {
                        _context.Log($"[DEBUG] BuildFigure: 内部异常: {ex.InnerException.Message}");
                    }
                }
                throw;
            }

            if (_context.Debug)
            {
                if (File.Exists(outputFilePath))
                {
                    _context.Log($"[DEBUG] ✅ 文件生成成功: {outputFilePath}");
                }
                else
                {
                    _context.Log($"[DEBUG] ❌ 文件生成失败: {outputFilePath}");
                }
            }
        }

        void InitOutputPath()
        {
            if (outputFile.ToLower().EndsWith(".vsdx"))
            {
                isFileMode = true;
                name = Path.GetFileNameWithoutExtension(outputFile);
                dir = Path.GetDirectoryName(outputFile);

                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            else if (Directory.Exists(outputFile))
            {
                isFileMode = false;
                name = Path.GetFileNameWithoutExtension(iter.Context.InputFile);
                dir = Path.GetFullPath(outputFile).TrimEnd(new char[] { '/', '\\' });
            }
            else
            {
                throw new ArgumentException($"输出路径无效: '{outputFile}'。请指定一个 .vsdx 文件路径或现有目录。");
            }
        }
    }
}
