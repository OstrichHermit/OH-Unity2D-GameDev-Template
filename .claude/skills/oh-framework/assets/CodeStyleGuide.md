# OH 框架代码风格快速参考

## 1. 命名规范

### 类名
- **逻辑管理器**：`Manager` 后缀，如 `QuestManager`
- **表现层/UI**：`DisplayManager` 或 `View` 后缀，如 `GuaMachineDisplayManager`
- **数据对象**：`So` 后缀，如 `LevelDataSo`

### 方法与变量
- **公共方法/属性**：PascalCase（大驼峰），如 `UpdateGameState()`
- **私有字段**：_camelCase（下划线+小驼峰），如 `_currentGuaSo`
- **常量/静态只读**：PascalCase 或 UPPER_SNAKE_CASE

## 2. 架构原则

### 单例模式
```csharp
// 所有的全局系统必须通过继承 OHMonoSingleton<T> 实现
public class AudioManager : OHMonoSingleton<AudioManager> { }
```
- **严禁**在代码中手动 `new` 一个单例类

### 解耦原则（事件驱动）
```csharp
// 跨模块通信：必须通过 OHEventCenter 发送和接收事件
// 发送事件
OHEventCenter.EventTrigger(OHEvent.GameStart);

// 注册事件（在 OnEnable 中）
OHEventCenter.AddEventListener(OHEvent.GameStart, OnGameStart);

// 注销事件（在 OnDisable 中）
OHEventCenter.RemoveEventListener(OHEvent.GameStart, OnGameStart);
```

### 数据驱动
- **严禁**硬编码重要配置信息
- 必须使用 ScriptableObject 存储
- 放置在 `Assets/数据文件/SO/` 目录下

## 3. 代码风格

### 扩展方法优先
- 优先检查 `OHExtend.cs` 中是否已有封装好的方法
- 通用的工具函数应作为 `static` 扩展方法归入 `OHTools` 命名空间

### UI 逻辑
- UI 组件引用统一使用 `TextMeshProUGUI` 和 `Image`
- 所有动画必须使用 DoTween 插件实现（除非与物理有关）

### 异步处理
- 复杂的流式逻辑优先使用 UniRx
- 异步或延时操作优先使用 UniTask

## 4. 内存优化

### 避免性能问题
```csharp
// ❌ 错误：在 Update 中使用 GetComponent
void Update() {
    var comp = GetComponent<Component>();
}

// ✅ 正确：在 Awake 中缓存
private Component _comp;
void Awake() {
    _comp = GetComponent<Component>();
}
```

### 对象池管理
- 频繁生成的对象必须使用 `ObjectPool<T>` 对象池系统管理

## 5. 资源管理规范

### 目录结构
- 脚本：`Assets/脚本/[模块名]/`
- 预制体：`Assets/Resources/` 或 Addressables
- 第三方插件：`Assets/Plugins/`

### 本地化
- 所有玩家可见的文本必须通过本地化系统处理
- 文本条目维护在 `Assets/Localization/Data/UITable.xlsx`

## 6. 注释规范

### 文档注释
```csharp
/// <summary>
/// 公共类方法必须包含文档注释
/// </summary>
public void ExampleMethod() { }
```

### 字段注释
```csharp
// 使用 [LabelText] 进行注释（推荐）
[SerializeField, LabelText("最大生命值")]
private int _maxHealth;

// 或使用 // 名称注释
[SerializeField]
private int _maxHealth; // 最大生命值
```

### 行内注释
```csharp
// 复杂的逻辑块必须有 // 行内注释
if (condition) {
    // 处理特殊情况
    HandleSpecialCase();
}
```