### 架构原则 (Architectural Principles)
- 单例模式 ：
    - 所有的全局系统（音频、剧情、关卡逻辑）必须通过继承 OHMonoSingleton.cs 实现。
    - 严禁在代码中手动 new 一个单例类。
- 解耦原则 (Event-Driven) ：
    - 跨模块通信 ：在直接引用对方的 Manager 前必须询问用户是否允许，否则必须通过 OHEventCenter.cs 发送和接收事件。
    - 事件注册 ：
        - 在 OnEnable 或 Awake 中注册： OHEventCenter.AddEventListener(OHEvent.Name, Callback) 。
        - 务必 在 OnDisable 或 OnDestroy 中注销： OHEventCenter.RemoveEventListener 。
- 数据驱动 (Data-Driven) ：
    - 在硬编码重要配置信息前必须询问用户是否允许，否则必须使用 ScriptableObject 进行存储，并放置在 Assets/数据文件/SO 目录下。

### 代码风格 (Coding Style)
- 扩展方法优先 ：
    - 优先检查 OHExtend.cs 中是否已有封装好的方法（如颜色转换、UI 特效）。
    - 通用的工具函数应作为 static 扩展方法归入 OHTools 命名空间。
    - 除非用户要求，否则必须避免做出多余的防御性编程操作，例如添加判空检查、日志打印、异常处理等。
- UI 逻辑 ：
    - UI 组件引用统一使用 TextMeshProUGUI 和 Image 。
    - 除非动画与物理有关，或用户指定要求使用其他插件，否则所有动画都必须使用 DoTween 插件实现。
- 异步处理 ：
    - 复杂的流式逻辑（如等待动画结束、连续输入处理）优先使用 UniRx 。
    - 异步或延时等操作优先使用 UniTask 。

### 协作与性能 (Collaboration & Performance)
- 注释规范 ：
    - 必须使用中文回复。
    - 所有公共类方法必须包含 /// <summary> 文档注释。
    - 复杂的逻辑块必须有 // 行内注释。
    - 没有使用[LabelText]进行注释的变量，必须包含有 // 名称注释。
- 内存优化 ：
    - 在 Update 中避免使用 GetComponent 或 GameObject.Find 。
    - 频繁生成的对象必须使用 ObjectPool<T> 对象池系统进行管理。

### 资源管理规范 (Resource Management)
- 目录结构 ：
    - 脚本放置于 Assets/脚本/[模块名] 。
    - 预制体放置于 Assets/Resources 或使用 Addressables （根据项目配置）。
    - 所有的第三方插件统一放置在 Assets/Plugins 。
- 本地化 (Localization) ：
    - 除非允许，否则所有玩家可见的文本（UI、对话、提示）必须通过本地化系统处理。
    - 文本条目需在 Assets/Localization/Data/UITable.xlsx 中维护。

### 命名规范 (Naming Conventions)
- 命名空间 ：
    - 框架层工具统一使用 OHTools 。
- 类名 ：
    - 逻辑管理器 ：使用 Manager 后缀，如 QuestManager 。
    - 如果是单例，且继承自MonoBehaviour，则必须继承 OHMonoSingleton<T> 。
    - 表现层/UI ：使用 DisplayManager 或 View 后缀，如 GuaMachineDisplayManager 。
    - 数据对象 ：使用 So 后缀（针对 ScriptableObject），如 LevelDataSo 。
- 方法与变量 ：
    - 公共方法/属性 ：使用 PascalCase （大驼峰），如 UpdateGameState() 。
    - 私有字段 ：使用 _camelCase （下划线+小驼峰），如 _currentGuaSo 。
    - 常量/静态只读 ：使用 PascalCase 或 UPPER_SNAKE_CASE 。
    - 变量通常情况下保持私有，非必要不公开。
    - 需要引用对象的私有变量或重要的数值私有变量，必须使用[SerializeField]进行序列化，并使用[LabelText]进行注释，以方便在 Inspector 中查看和调试。