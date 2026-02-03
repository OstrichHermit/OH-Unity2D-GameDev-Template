---
name: oh-framework
description: 此 Skill 应用于 Unity 项目开发，特别是使用 OH 框架的项目。当用户需要创建符合 OH 框架规范的脚本、添加事件或单例、检查代码规范，或进行完整开发流程时使用此 Skill。关键词：Manager、DisplayManager、View、单例、事件、OHEvent、ScriptableObject、SO、DoTween、TextMeshPro、UI 动画、对象池、本地化。
---

# OH 框架开发 Skill

## 概述

此 Skill 为 Unity OH 框架提供完整的开发支持，确保生成的代码符合框架的架构原则、代码风格和资源管理规范。OH 框架是一个基于事件驱动、数据驱动的 Unity 开发框架，强调解耦、性能优化和代码规范性。

## 何时使用此 Skill

在以下场景中自动触发此 Skill：

1. **用户明确提及** OH 框架或 OHTools 命名空间
2. **关键词匹配**：Manager、DisplayManager、View、单例、事件、OHEvent、ScriptableObject、SO、DoTween、TextMeshPro、UI 动画、对象池、本地化
3. **Unity 项目环境**：检测到项目包含 `OHMonoSingleton.cs`、`OHEventCenter.cs`、`OHExtend.cs` 等 OH 框架核心文件
4. **代码规范检查**：需要检查代码是否符合 OH 框架规范
5. **脚本创建**：创建新的 Manager、DisplayManager、View、SO 数据类等

## 核心能力

### 1. 创建符合规范的脚本

根据不同的用途，使用对应的模板创建脚本：

#### Manager（逻辑管理器）
- 继承自 `OHMonoSingleton<T>`
- 使用 `_camelCase` 私有字段
- 公共属性使用 PascalCase
- 在 `OnEnable` 中注册事件，`OnDisable` 中注销事件
- 参考模板：`assets/ManagerTemplate.cs`

#### DisplayManager（表现层）
- 不继承单例（通常挂载在 UI 对象上）
- UI 组件统一使用 `TextMeshProUGUI` 和 `Image`
- 使用 `[SerializeField]` + `[LabelText]` 标注所有 UI 引用
- 参考模板：`assets/DisplayManagerTemplate.cs`

#### ScriptableObject（数据对象）
- 使用 `So` 后缀，如 `LevelDataSo`
- 使用 `[CreateAssetMenu]` 标记以便创建资源
- 重要的配置字段使用 `[LabelText]` 标注
- 参考模板：`assets/SoTemplate.cs`

### 2. 事件系统管理

#### 添加新事件
1. 在 `OHEvent` 枚举中添加事件名称（使用 PascalCase）
2. 在发送方使用 `OHEventCenter.EventTrigger(OHEvent.EventName)`
3. 在接收方的 `OnEnable` 中注册：`OHEventCenter.AddEventListener(OHEvent.EventName, Callback)`
4. 在接收方的 `OnDisable` 中注销：`OHEventCenter.RemoveEventListener(OHEvent.EventName, Callback)`

#### 事件参数支持
- 无参数：`EventTrigger(OHEvent.EventName)`
- 单参数：`EventTrigger<T>(OHEvent.EventName, arg)`
- 双参数：`EventTrigger<T1, T2>(OHEvent.EventName, arg1, arg2)`
- 三参数：`EventTrigger<T1, T2, T3>(OHEvent.EventName, arg1, arg2, arg3)`

### 3. 代码规范检查

在编写或审查代码时，确保遵循以下规范：

#### 架构原则检查
- ✅ 所有全局系统通过 `OHMonoSingleton<T>` 实现
- ✅ 跨模块通信使用 `OHEventCenter`，而非直接引用
- ✅ 重要配置使用 ScriptableObject 存储

#### 代码风格检查
- ✅ 扩展方法优先检查 `OHExtend.cs` 是否已有实现
- ✅ UI 组件使用 `TextMeshProUGUI` 和 `Image`
- ✅ 动画使用 DoTween（除非与物理相关）
- ✅ 异步使用 UniRx（流式逻辑）或 UniTask（异步/延时）

#### 性能优化检查
- ✅ Update 中避免使用 `GetComponent` 或 `GameObject.Find`
- ✅ 频繁生成的对象使用 `ObjectPool<T>` 管理

#### 注释规范检查
- ✅ 公共类方法包含 `/// <summary>` 文档注释
- ✅ 复杂逻辑块有 `//` 行内注释
- ✅ 私有变量使用 `[LabelText]` 或 `//` 注释

### 4. 扩展方法使用

在编写代码前，优先检查 `OHExtend.cs` 中是否已有封装好的方法：

#### OHStringExtend（字符串扩展）
- `OHHexToColor()` - 将十六进制颜色字符串转换为 Color

#### OHImageExtend（Image 扩展 - DoTween 动画）
- `OHEndEffect()` - 结束效果清理
- `OHFlashWhiteToBlack()` - 白到黑闪烁
- `OHFlashBlackToWhite()` - 黑到白闪烁
- `OHGlitchChangeSprite()` - 故障艺术切换图片
- `OHFadeChangeSprite()` - 渐变切换图片
- `OHNeonFlicker()` - 霓虹灯闪烁特效

## 开发工作流程

### 创建新功能模块的完整流程

1. **创建 SO 数据类**（如需要）
   - 参考 `assets/SoTemplate.cs`
   - 创建数据结构并存储在 `Assets/数据文件/SO/[模块名]/`

2. **创建 Manager**
   - 参考 `assets/ManagerTemplate.cs`
   - 继承 `OHMonoSingleton<T>`
   - 实现核心业务逻辑

3. **添加事件**（如需要跨模块通信）
   - 在 `OHEvent` 枚举中添加事件名称
   - 在 `OHEventCenter` 中注册和触发事件

4. **创建 DisplayManager 或 View**
   - 参考 `assets/DisplayManagerTemplate.cs`
   - 处理 UI 展示和交互
   - 通过事件系统与 Manager 通信

5. **代码审查**
   - 检查是否符合所有 OH 框架规范
   - 确保事件注册和注销配对
   - 验证性能优化措施

## 关键决策点

### 何时使用单例？
- ✅ 全局系统（音频、剧情、关卡逻辑等）
- ✅ 需要在多个场景间共享状态
- ❌ UI 或表现层（通常挂载在 GameObject 上）

### 何时使用事件通信？
- ✅ 跨模块通信（在直接引用对方 Manager 前必须询问）
- ✅ 一对多的广播式通知
- ❌ 同一模块内部的简单调用（可以直接调用）

### 何时使用 ScriptableObject？
- ✅ 重要配置信息（在硬编码前必须询问）
- ✅ 需要在编辑器中配置的数据
- ✅ 需要复用的数据模板
- ❌ 运行时临时数据

## 资源引用

### 模板文件
- `assets/ManagerTemplate.cs` - Manager 模板
- `assets/DisplayManagerTemplate.cs` - DisplayManager 模板
- `assets/SoTemplate.cs` - ScriptableObject 模板
- `assets/EventEnumTemplate.cs` - OHEvent 枚举模板

### 参考文档
- `assets/CodeStyleGuide.md` - 代码风格快速参考

### 框架核心文件
- `OHMonoSingleton.cs` - 单例模式基类
- `OHEventCenter.cs` - 事件中心
- `OHExtend.cs` - 扩展方法集合
- `OHEvent.cs` - 事件名称枚举

## 常见任务示例

### 创建一个音频管理器
```
用户需求：创建 AudioManager

生成步骤：
1. 基于 ManagerTemplate.cs 创建 AudioManager
2. 继承 OHMonoSingleton<AudioManager>
3. 添加播放 BGM、音效的方法
4. 在 OHEvent 中添加 AudioPlayBGM、AudioPlaySFX 事件
5. 通过事件中心触发播放
```

### 创建一个 UI 显示管理器
```
用户需求：创建主菜单 UI

生成步骤：
1. 基于 DisplayManagerTemplate.cs 创建 MainMenuDisplayManager
2. 添加 UI 组件引用（TextMeshProUGUI、Image、Button）
3. 在 OnEnable 中注册按钮事件
4. 通过 OHEventCenter 发送按钮点击事件
5. 在 OnDisable 中注销所有事件
```

### 创建关卡数据 SO
```
用户需求：创建关卡配置数据

生成步骤：
1. 基于 SoTemplate.cs 创建 LevelDataSo
2. 添加关卡名称、难度、敌人配置等字段
3. 使用 [LabelText] 标注重要字段
4. 在编辑器中创建 .asset 资源文件
```

## 注意事项

1. **禁止手动 new 单例**：所有单例必须通过 `OHMonoSingleton<T>` 实现
2. **事件注销配对**：每次 `AddEventListener` 必须有对应的 `RemoveEventListener`
3. **避免性能陷阱**：Update 中不使用 `GetComponent`，频繁对象使用对象池
4. **UI 统一规范**：只使用 TextMeshProUGUI 和 Image，动画只使用 DoTween
5. **数据驱动优先**：重要配置必须使用 ScriptableObject，不得硬编码