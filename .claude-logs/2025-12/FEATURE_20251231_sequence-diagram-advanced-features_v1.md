***
日期: 2025-12-31 09:09
类别: FEATURE
状态: ✅ 已完成
***

## 任务：时序图高级功能实现（alt/else组合片段、箭头对齐修复）

### 修改的文件

- md2visio/mermaid/sequence/SeqSttKeyword.cs - 添加 else/end/autonumber 关键字
- md2visio/struc/sequence/SeqFragment.cs - 新建组合片段数据结构
- md2visio/struc/sequence/Sequence.cs - 添加 Fragments、ShowSequenceNumbers
- md2visio/struc/sequence/SeqMessage.cs - 添加 SequenceNumber
- md2visio/struc/sequence/SeqBuilder.cs - 处理 alt/else/end/autonumber
- md2visio/vsdx/VDrawerSeq.cs - DrawFragments()、修复箭头对齐

### 关键操作

1. 实现 alt/else/end 组合片段解析与绘制（虚线框+标签+分隔线）
2. 修复箭头与激活框对齐问题（移除 NestingLevel + 1 中的 + 1）
3. 移除 autonumber 序号圆圈显示

### 效果

- 支持 Mermaid 时序图 alt/else 条件分支语法
- 箭头正确对齐到激活框边缘
