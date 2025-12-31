***
日期: 2025-12-23 04:15
类别: BUGFIX
状态: ✅ 已完成
***

## 任务：修复时序图消息箭头位置错误问题

### 修改的文件

- `md2visio/vsdx/VDrawerSeq.cs`

### 关键操作

1. 诊断问题：Visio 线条使用 `Geometry1.*` 是局部坐标，导致消息箭头在 (0,0)
2. 修复 `DrawRegularMessage`：改用 `BeginX/BeginY/EndX/EndY` 页面坐标
3. 修复 `DrawSelfCallMessage`：使用 `Page.DrawLine()` API 绘制 U 形自调用
4. 修复 `CreateVerticalDashedLine`：生命线同样改用页面坐标

### 效果

- 消息箭头、生命线将在正确位置显示
