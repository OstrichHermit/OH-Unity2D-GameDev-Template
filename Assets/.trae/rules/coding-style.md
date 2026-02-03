### 代码风格 (Coding Style)
- 扩展方法优先 ：
  - 优先检查 OHExtend.cs 中是否已有封装好的方法（如颜色转换、UI 特效）。
  - 通用的工具函数应作为 static 扩展方法归入 OHTools 命名空间。
- UI 逻辑 ：
  - UI 组件引用统一使用 TextMeshProUGUI 和 Image 。
  - 简单的 UI 动画（缩放、渐变）必须使用 DOTween 插件实现。
- 异步处理 ：
  - 复杂的流式逻辑（如等待动画结束、连续输入处理）优先使用 UniRx 。