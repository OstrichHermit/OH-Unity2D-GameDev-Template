using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OHTools;

// 【命名规范】表现层使用 DisplayManager 或 View 后缀
// 【UI 规范】UI 组件引用统一使用 TextMeshProUGUI 和 Image
public class [Name]DisplayManager : MonoBehaviour
{
    #region UI 组件引用
    // 【UI 规范】使用 [SerializeField] + [LabelText] 标注所有 UI 引用
    [SerializeField, LabelText("标题文本")]
    private TextMeshProUGUI _titleText;

    [SerializeField, LabelText("图标图片")]
    private Image _iconImage;

    [SerializeField, LabelText("按钮")]
    private Button _actionButton;
    #endregion

    #region 生命周期
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
    /// 更新显示内容
    /// </summary>
    public void UpdateDisplay(string title, Sprite icon)
    {
        _titleText.text = title;
        _iconImage.sprite = icon;
    }
    #endregion

    #region 事件处理
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public void OnButtonClick()
    {
        // 通过事件中心发送事件，而非直接调用其他 Manager
        // OHEventCenter.EventTrigger(OHEvent.ButtonClicked);
    }

    /// <summary>
    /// 事件回调示例
    /// </summary>
    private void OnEventCallback()
    {
        // 事件处理
    }
    #endregion
}