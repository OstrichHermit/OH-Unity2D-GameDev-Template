# OHAudioPlayer API 参考

## 类概述

```csharp
namespace OHTools
{
    public class OHAudioPlayer : OHMonoSingleton<OHAudioPlayer>
}
```

**OHAudioPlayer 是音频播放基类**，继承自 `OHMonoSingleton<OHAudioPlayer>`，提供对象池管理的 AudioSource 实例和音频播放功能。

**重要**：OHAudioPlayer 不是直接使用的服务类，而是需要被继承的基类。

## 设计理念

OHAudioPlayer 采用**继承式模块化设计**：
- 作为基类提供音频播放能力
- 子类管理具体的音频资源（AudioClip）
- 子类提供公共播放方法封装 protected 方法
- 每个子类自动获得单例能力

## Protected 方法

以下方法只能在继承 OHAudioPlayer 的子类内部调用：

### PlayAudio
```csharp
protected void PlayAudio(AudioClip clip, float volume = 1f)
```

播放一次性音效，播放完成后自动回收 AudioSource。

**参数：**
- `clip` - 要播放的音频片段
- `volume` - 音量（0-1），默认为 1

**使用场景：**
- UI 交互音效（按钮点击、悬停等）
- 游戏动作音效（攻击、跳跃等）
- 一次性环境音效

**子类示例：**
```csharp
public class UIManagerAudio : OHAudioPlayer
{
    [SerializeField] private AudioClip _clickSound;

    public void PlayClickSound()
    {
        // 在子类内部调用 protected 方法
        PlayAudio(_clickSound, 1f);
    }
}
```

### PlayBGM
```csharp
protected void PlayBGM(AudioClip clip, float volume = 1f)
```

播放循环背景音乐，需手动停止。

**参数：**
- `clip` - 要播放的音频片段
- `volume` - 音量（0-1），默认为 1

**使用场景：**
- 场景背景音乐
- 战斗音乐
- 氛围音乐

**子类示例：**
```csharp
public class BGMManager : OHAudioPlayer
{
    [SerializeField] private AudioClip _menuBGM;

    public void PlayMenuBGM()
    {
        PlayBGM(_menuBGM, 0.5f);
    }
}
```

### GetPlayerWithClip
```csharp
protected AudioSource GetPlayerWithClip(AudioClip clip)
```

获取正在播放指定 clip 的 AudioSource 实例。

**参数：**
- `clip` - 音频片段

**返回：**
- 正在播放该 clip 的 AudioSource，如果没有则返回 null

**使用场景：**
- 检查某个音频是否正在播放
- 获取播放器进行精细控制（如音量调整、淡出效果）

**子类示例：**
```csharp
public class BGMManager : OHAudioPlayer
{
    private AudioClip _currentBGM;

    public void SetBGMVolume(float volume)
    {
        var player = GetPlayerWithClip(_currentBGM);
        if (player != null)
        {
            player.volume = volume;
        }
    }
}
```

### StopPlayerWithClip
```csharp
protected void StopPlayerWithClip(AudioClip clip)
```

停止并回收正在播放指定 clip 的 AudioSource。

**参数：**
- `clip` - 要停止的音频片段

**使用场景：**
- 停止当前背景音乐
- 停止循环播放的音效

**子类示例：**
```csharp
public class BGMManager : OHAudioPlayer
{
    private AudioClip _currentBGM;

    public void StopCurrentBGM()
    {
        if (_currentBGM != null)
        {
            StopPlayerWithClip(_currentBGM);
            _currentBGM = null;
        }
    }
}
```

## 继承的单例功能

OHAudioPlayer 继承自 `OHMonoSingleton<OHAudioPlayer>`，子类自动获得单例能力：

### Instance 属性
```csharp
public static T Instance { get; }
```

获取子类的单例实例。

**使用示例：**
```csharp
// 在任何脚本中访问 UI 音效管理器
UIManagerAudio.Instance.PlayClickSound();

// 访问 BGM 管理器
BGMManager.Instance.PlayMenuBGM();
```

### RegisterSingleton 方法
```csharp
protected bool RegisterSingleton()
```

注册单例，处理重复实例。在子类的 Awake 中调用。

**返回：**
- true：注册成功，继续初始化
- false：检测到重复实例，已销毁当前对象，应立即返回

**使用示例：**
```csharp
protected override void Awake()
{
    // 必须首先调用，处理单例注册
    if (!RegisterSingleton()) return;

    // 其他初始化逻辑...
}
```

## 内部机制

### 对象池管理
OHAudioPlayer 使用 `ObjectPool<AudioSource>` 管理 AudioSource 实例：
- 默认容量：10
- 最大容量：100
- 自动回收播放完成的非循环音频

### 活跃播放器追踪
使用 `Dictionary<AudioClip, List<AudioSource>>` 追踪每个 clip 正在使用的 AudioSource 实例。

## 使用流程

### 1. 创建子类
```csharp
public class [模块名]AudioManager : OHAudioPlayer
{
    // 音频资源字段
    [SerializeField] private AudioClip _audioClip;

    // 音量控制
    [SerializeField] [Range(0f, 1f)] private float _volume = 1f;

    protected override void Awake()
    {
        if (!RegisterSingleton()) return;
    }

    // 公共播放方法
    public void PlayAudioEffect()
    {
        PlayAudio(_audioClip, _volume);
    }
}
```

### 2. 在场景中配置
1. 创建 GameObject
2. 添加子类组件
3. 在 Inspector 中配置 AudioClip 资源

### 3. 使用
```csharp
// 直接调用
[模块名]AudioManager.Instance.PlayAudioEffect();

// 或通过事件触发
OHEventCenter.EmitEvent("PlayAudioEffect");
```

## 注意事项

1. **Protected 访问级别**：
   - 所有播放方法都是 `protected`
   - 只能在子类内部调用
   - 子类应提供 `public` 方法封装

2. **不要直接实例化**：
   - OHAudioPlayer 不能直接使用
   - 必须通过子类访问

3. **继承链**：
   - OHAudioPlayer 继承 OHMonoSingleton<OHAudioPlayer>
   - 子类不需要重复继承 OHMonoSingleton<T>

4. **单例注册**：
   - 在 Awake 中必须调用 `RegisterSingleton()`
   - 处理返回值，如果返回 false 应立即返回

5. **内存管理**：
   - 非循环音频播放完成后自动回收
   - 循环音频（BGM）需要手动调用 `StopPlayerWithClip()` 停止