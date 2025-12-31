---
日期: 2025-12-23 02:49
类别: REFACTOR
状态: ✅ 已完成
---

## 任务：Phase 2 消除全局状态与依赖注入重构

### 修改的文件

- **新建 (8)**：`ConversionContext.cs`, `IVisioSession.cs`, `VisioSession.cs`, 测试项目 (5个文件)
- **修改 (15)**：所有 Builder/Drawer 类, `Md2VisioConverter.cs`, `FigureBuilderFactory.cs`
- **删除 (1)**：`AppConfig.cs`

### 关键操作

1. 删除 `AppConfig.Instance` 全局单例，改为 `ConversionContext` 依赖注入
2. 删除 `VBuilder.VisioApp` 静态变量，改为 `IVisioSession` 实例管理 COM 生命周期
3. 创建 `md2visio.Tests` 单元测试项目，19 个测试全部通过

### 效果

- 73 处 `AppConfig.Instance` 调用已替换
- COM 对象统一由 `VisioSession.Dispose()` 管理
- 架构从全局状态改为显式依赖注入
