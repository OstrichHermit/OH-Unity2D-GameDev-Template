using UnityEngine;
using OHTools;

// 【命名规范】逻辑管理器使用 Manager 后缀
// 【继承规范】如果是单例，且继承自 MonoBehaviour，必须继承 OHMonoSingleton<T>
// 【放置位置】Assets/脚本/[模块名]/
public class [Name]Manager : OHMonoSingleton<[Name]Manager>
{
    #region 字段与属性
    // 【序列化规范】需要引用对象的私有变量或重要的数值私有变量，必须使用 [SerializeField]
    [SerializeField, LabelText("示例配置")]
    private int _exampleConfig = 100;

    // 【属性规范】公共属性使用 PascalCase
    public int ExampleConfig => _exampleConfig;
    #endregion

    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        // 初始化逻辑
    }

    private void OnEnable()
    {
        // 【事件注册规范】在 OnEnable 中注册事件
        // OHEventCenter.AddEventListener(OHEvent.EventName, OnEventCallback);
    }

    private void OnDisable()
    {
        // 【事件注销规范】务必在 OnDisable 中注销事件
        // OHEventCenter.RemoveEventListener(OHEvent.EventName, OnEventCallback);
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 示例方法
    /// </summary>
    public void DoSomething()
    {
        // 方法实现
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 事件回调示例
    /// </summary>
    private void OnEventCallback()
    {
        // 事件处理
    }
    #endregion
}