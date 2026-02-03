### 资源管理规范 (Resource Management)
- 目录结构 ：
  - 脚本放置于 Assets/脚本/[模块名] 。
  - 预制体放置于 Assets/Resources 或使用 Addressables （根据项目配置）。
  - 所有的第三方插件统一放置在 Assets/Plugins 。
- 本地化 (Localization) ：
  - 除非允许，否则所有玩家可见的文本（UI、对话、提示）必须通过本地化系统处理。
  - 文本条目需在 Assets/Localization/Data/UITable.xlsx 中维护。