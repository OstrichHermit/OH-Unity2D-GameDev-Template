using UnityEngine;
using OHTools;

// 【命名规范】数据对象使用 So 后缀
// 【数据驱动】重要配置信息必须使用 ScriptableObject 存储
// 【放置位置】Assets/数据文件/SO/[模块名]/
[CreateAssetMenu(fileName = "[Name]Data", menuName = "OH框架/[模块名]/[Name]数据")]
public class [Name]So : ScriptableObject
{
    #region 数据字段
    // 【序列化规范】没有使用 [LabelText] 的变量，必须包含 // 名称注释
    [Header("基础配置")]
    [SerializeField]
    private string _configName; // 配置名称

    [SerializeField]
    private float _configValue; // 配置数值

    [Header("高级选项")]
    [SerializeField, LabelText("启用功能")]
    private bool _enableFeature = true;

    [SerializeField, LabelText("最大数量")]
    private int _maxCount = 10;
    #endregion

    #region 公共属性
    /// <summary>
    /// 配置名称
    /// </summary>
    public string ConfigName => _configName;

    /// <summary>
    /// 配置数值
    /// </summary>
    public float ConfigValue => _configValue;

    /// <summary>
    /// 是否启用功能
    /// </summary>
    public bool EnableFeature => _enableFeature;

    /// <summary>
    /// 最大数量
    /// </summary>
    public int MaxCount => _maxCount;
    #endregion

    #region 公共方法
    /// <summary>
    /// 重置数据到默认状态
    /// </summary>
    public void ResetData()
    {
        _configName = string.Empty;
        _configValue = 0f;
        _enableFeature = true;
        _maxCount = 10;
    }

    /// <summary>
    /// 验证数据有效性
    /// </summary>
    public bool ValidateData()
    {
        return !string.IsNullOrEmpty(_configName) && _maxCount > 0;
    }
    #endregion

    #region 编辑器方法
    /// <summary>
    /// 编辑器下调用，用于数据验证
    /// </summary>
    private void OnValidate()
    {
        if (_maxCount < 0)
        {
            _maxCount = 0;
        }

        if (_configValue < 0f)
        {
            _configValue = 0f;
        }
    }
    #endregion
}