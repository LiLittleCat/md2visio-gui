***
日期: 2025-12-23 02:05
类别: REFACTOR
状态: ✅ 已完成
***

## 任务：项目精简与 API Facade 层重构

### 修改的文件

- 删除: `md2visio/main/AppTest.cs`
- 删除: `md2visio/main/ConsoleApp.cs`
- 删除: `md2visio/md2visio - Backup.csproj`
- 删除: `md2visio/default/theme/default.yaml.backup`
- 修改: `md2visio/md2visio.csproj`
- 修改: `md2visio.GUI/Services/ConversionService.cs`
- 新增: `md2visio/Api/ConversionRequest.cs`
- 新增: `md2visio/Api/ConversionResult.cs`
- 新增: `md2visio/Api/ConversionProgress.cs`
- 新增: `md2visio/Api/ILogSink.cs`
- 新增: `md2visio/Api/IMd2VisioConverter.cs`
- 新增: `md2visio/Api/Md2VisioConverter.cs`

### 关键操作

1. 清理死代码：删除硬编码测试文件、CLI入口、空文件夹占位、PostBuild事件
2. 创建 API Facade 层：6 个文件，包装现有逻辑，GUI 直接调用 API 而非 string[] args
3. 重写 ConversionService：使用新 IMd2VisioConverter 接口，移除 AppConfig.Instance 直接操作

### 效果

- 警告数从 13 个减少到 3 个
- GUI 调用方式从 CLI 参数模式改为直接 API 调用
- 代码解耦：核心库与 GUI 通过接口隔离
