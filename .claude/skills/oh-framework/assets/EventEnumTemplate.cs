namespace OHTools
{
    /// <summary>
    /// OH 框架事件名称枚举
    /// 【添加事件时】在此枚举中添加新事件名称
    /// 【命名规范】使用 PascalCase，描述性命名
    /// </summary>
    public enum OHEvent
    {
        // ============ 游戏流程事件 ============
        GameStart,           // 游戏开始
        GamePause,           // 游戏暂停
        GameResume,          // 游戏继续
        GameOver,            // 游戏结束

        // ============ 场景管理事件 ============
        SceneLoadStart,      // 场景加载开始
        SceneLoadComplete,   // 场景加载完成
        SceneUnloadComplete, // 场景卸载完成

        // ============ UI 事件 ============
        UIOpen,              // UI 打开
        UIClose,             // UI 关闭
        UIButtonClicked,     // UI 按钮点击

        // ============ 玩家事件 ============
        PlayerSpawn,         // 玩家生成
        PlayerDeath,         // 玩家死亡
        PlayerRespawn,       // 玩家重生

        // ============ 数据事件 ============
        DataLoaded,          // 数据加载完成
        DataSaved,           // 数据保存完成

        // ============ 音频事件 ============
        AudioPlayBGM,        // 播放背景音乐
        AudioStopBGM,        // 停止背景音乐
        AudioPlaySFX,        // 播放音效

        // ============ 自定义事件区域 ============
        // 在此区域添加项目特定事件
        // CustomEvent,       // 示例：自定义事件
    }
}