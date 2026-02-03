---
name: oh-audio-player
description: 此 Skill 应用于 Unity 项目中与 OHAudioPlayer 音频播放系统相关的所有任务。当用户需要创建模块化音频管理器、播放音效、管理背景音乐，或进行任何与 OHAudioPlayer 相关的音频操作时使用此 Skill。
---

# OHAudioPlayer Skill

## 概述

OHAudioPlayer 是一个音频播放基类，提供对象池管理的 AudioSource 实例和音频播放功能。**OHAudioPlayer 不是直接使用的单例服务，而是需要被继承的基类**。正确的用法是创建继承 OHAudioPlayer 的具体 AudioManager，每个模块管理自己的音频资源。

## 何时使用此 Skill

当用户请求以下任务时，应使用此 Skill：

- 创建音频管理器
- 添加音频播放功能
- 音频系统配置
- 事件驱动音频

**判断标准**：只要涉及到 OHAudioPlayer 的使用、继承、创建 AudioManager 或播放音效，都应使用此 Skill。

## 核心概念

### 架构设计

OHAudioPlayer 采用**继承式模块化设计**：

```
OHAudioPlayer (基类)
    ├── UIManagerAudio (UI 音效管理器)
    ├── BattleAudioManager (战斗音效管理器)
    ├── BGMManager (背景音乐管理器)
    └── ...其他模块音频管理器
```

每个具体的 AudioManager：
- 继承 `OHAudioPlayer`（已继承 `OHMonoSingleton<T>`）
- 在 Inspector 中引用该模块需要的 AudioClip
- 作为单例使用：`[模块名]AudioManager.Instance.方法名()`
- 或通过事件触发：在 `OnEnable` 中注册事件，事件回调中调用播放方法

### 为什么这样设计？

1. **模块化**：每个模块管理自己的音频，职责清晰
2. **解耦合**：不同模块的音频互不影响
3. **可视化配置**：音频资源在 Inspector 中直接引用和配置
4. **对象池自动管理**：继承即获得 AudioSource 对象池能力

## 使用流程

### 步骤 1：创建模块音频管理器

创建继承 OHAudioPlayer 的管理器类：

```csharp
using Sirenix.OdinInspector;
using UnityEngine;

namespace OHTools
{
    /// <summary>
    /// UI 音效管理器
    /// </summary>
    public class UIManagerAudio : OHAudioPlayer
    {
        [FoldoutGroup("UI 音效", expanded: true)]
        [LabelText("点击音效")]
        [SerializeField]
        private AudioClip _clickSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("悬停音效")]
        [SerializeField]
        private AudioClip _hoverSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("确认音效")]
        [SerializeField]
        private AudioClip _confirmSound;

        [FoldoutGroup("音量控制")]
        [LabelText("UI 音效音量")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 1f;

        protected override void Awake()
        {
            // 调用基类 RegisterSingleton 处理单例注册
            if (!RegisterSingleton()) return;
        }

        #region 公共播放方法

        /// <summary>
        /// 播放点击音效
        /// </summary>
        public void PlayClickSound()
        {
            if (_clickSound != null)
            {
                PlayAudio(_clickSound, _volume);
            }
        }

        /// <summary>
        /// 播放悬停音效
        /// </summary>
        public void PlayHoverSound()
        {
            if (_hoverSound != null)
            {
                PlayAudio(_hoverSound, _volume);
            }
        }

        /// <summary>
        /// 播放确认音效
        /// </summary>
        public void PlayConfirmSound()
        {
            if (_confirmSound != null)
            {
                PlayAudio(_confirmSound, _volume);
            }
        }

        #endregion
    }
}
```

### 步骤 2：在场景中使用

#### 方式 A：直接调用单例

```csharp
// UI 按钮点击事件
public void OnButtonClick()
{
    UIManagerAudio.Instance.PlayClickSound();
}

// UI 悬停事件
public void OnButtonHover()
{
    UIManagerAudio.Instance.PlayHoverSound();
}
```

#### 方式 B：通过事件触发（推荐）

在 AudioManager 中注册事件监听：

```csharp
public class UIManagerAudio : OHAudioPlayer
{
    // ... 音频字段定义 ...

    private void OnEnable()
    {
        OHEventCenter.AddEventListener("UIClick", OnUIClick);
        OHEventCenter.AddEventListener("UIHover", OnUIHover);
    }

    private void OnDisable()
    {
        OHEventCenter.RemoveEventListener("UIClick", OnUIClick);
        OHEventCenter.RemoveEventListener("UIHover", OnUIHover);
    }

    private void OnUIClick(object[] args)
    {
        PlayClickSound();
    }

    private void OnUIHover(object[] args)
    {
        PlayHoverSound();
    }

    // ... 播放方法 ...
}
```

在其他脚本中发送事件：

```csharp
// 触发 UI 点击音效
OHEventCenter.EmitEvent("UIClick");

// 或传递参数
OHEventCenter.EmitEvent("PlayAudio", "ClickSound");
```

### 步骤 3：配置音频资源

1. 在场景中创建空 GameObject，命名为 `UIManagerAudio`
2. 添加 `UIManagerAudio` 组件
3. 在 Inspector 中拖入对应的 AudioClip 资源
4. 调整音量参数

## BGM 管理器示例

```csharp
using Sirenix.OdinInspector;
using UnityEngine;

namespace OHTools
{
    /// <summary>
    /// 背景音乐管理器
    /// </summary>
    public class BGMManager : OHAudioPlayer
    {
        [FoldoutGroup("背景音乐", expanded: true)]
        [LabelText("主菜单 BGM")]
        [SerializeField]
        private AudioClip _menuBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("战斗 BGM")]
        [SerializeField]
        private AudioClip _battleBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("游戏 BGM")]
        [SerializeField]
        private AudioClip _gameBGM;

        [FoldoutGroup("音量控制")]
        [LabelText("BGM 音量")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 0.5f;

        // 当前播放的 BGM
        private AudioClip _currentBGM;

        protected override void Awake()
        {
            if (!RegisterSingleton()) return;
        }

        #region 公共播放方法

        /// <summary>
        /// 播放主菜单 BGM
        /// </summary>
        public void PlayMenuBGM()
        {
            PlayBGMInternal(_menuBGM);
        }

        /// <summary>
        /// 播放战斗 BGM
        /// </summary>
        public void PlayBattleBGM()
        {
            PlayBGMInternal(_battleBGM);
        }

        /// <summary>
        /// 播放游戏 BGM
        /// </summary>
        public void PlayGameBGM()
        {
            PlayBGMInternal(_gameBGM);
        }

        /// <summary>
        /// 停止当前 BGM
        /// </summary>
        public void StopCurrentBGM()
        {
            if (_currentBGM != null)
            {
                StopPlayerWithClip(_currentBGM);
                _currentBGM = null;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 播放 BGM 内部实现
        /// </summary>
        private void PlayBGMInternal(AudioClip clip)
        {
            if (clip == null) return;

            // 如果播放的是同一首 BGM，不做处理
            if (_currentBGM == clip) return;

            // 停止当前 BGM
            StopCurrentBGM();

            // 播放新 BGM
            PlayBGM(clip, _volume);
            _currentBGM = clip;
        }

        #endregion
    }
}
```

## 代码模板

本 Skill 包含以下代码模板，位于 `assets/templates/`：

- `UIManagerAudio.cs` - UI 音效管理器模板
- `BGMManager.cs` - 背景音乐管理器模板
- `BattleAudioManager.cs` - 战斗音效管理器模板

## 常见使用场景

### 场景 1：创建 UI 音效系统
用户请求：「为 UI 创建音效系统」

**实现步骤：**
1. 创建 `UIManagerAudio` 类继承 `OHAudioPlayer`
2. 添加常用 UI 音效字段（点击、悬停、确认、取消等）
3. 创建公共播放方法
4. 在场景中配置 GameObject 和音频资源
5. UI 组件通过单例或事件触发播放

### 场景 2：创建 BGM 管理器
用户请求：「创建背景音乐管理器，支持切换音乐」

**实现步骤：**
1. 创建 `BGMManager` 类继承 `OHAudioPlayer`
2. 添加各场景的 BGM 字段
3. 实现播放方法（自动停止当前 BGM）
4. 添加停止 BGM 方法
5. 在场景切换时调用对应方法

### 场景 3：创建战斗音效系统
用户请求：「为战斗系统创建音效管理」

**实现步骤：**
1. 创建 `BattleAudioManager` 类继承 `OHAudioPlayer`
2. 添加战斗相关音效（攻击、受击、技能、胜利、失败等）
3. 注册战斗相关事件监听
4. 在事件回调中调用对应的播放方法

## 注意事项

### 1. 继承关系
- 所有音频管理器必须**继承 `OHAudioPlayer`**，而不是 `OHMonoSingleton<T>`
- `OHAudioPlayer` 已继承 `OHMonoSingleton<OHAudioPlayer>`，所以子类自动获得单例能力
- **不要**在子类中重复继承 `OHMonoSingleton<T>`

### 2. 单例访问
- 子类通过 `[模块名]AudioManager.Instance` 访问
- `Instance` 是继承自 `OHAudioPlayer` 的静态属性

### 3. Awake 方法
- 必须调用 `if (!RegisterSingleton()) return;` 处理单例注册
- 如果在 Awake 中有其他初始化逻辑，放在这行代码之后

### 4. Protected 方法
- `PlayAudio()`、`PlayBGM()`、`StopPlayerWithClip()` 是 `protected` 方法
- 只能在继承的子类内部调用，不能从外部访问
- 子类应提供 `public` 方法封装这些调用

### 5. 音量控制
- 每个管理器可以有自己的音量控制
- 建议使用 `[Range(0f, 1f)]` 和 `[LabelText]` 标注
- 播放时传入音量：`PlayAudio(clip, _volume)`

### 6. 事件驱动（可选）
- 如果使用事件驱动，在 `OnEnable` 中注册，`OnDisable` 中注销
- 事件名称建议使用字符串常量或在 OHEventCenter 中统一定义

## 资源

### references/
- `oh-audio-player-api.md` - OHAudioPlayer 基类 API 参考

### assets/templates/
- `UIManagerAudio.cs` - UI 音效管理器模板
- `BGMManager.cs` - 背景音乐管理器模板
- `BattleAudioManager.cs` - 战斗音效管理器模板