using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace OHTools
{
    public class OHFixSelectedBtn : MonoBehaviour
    {
        private Button _selfBtn;

        private void Awake()
        {
            _selfBtn = GetComponent<Button>();
            _selfBtn.onClick.AddListener(CancelButtonClick);
        }

        public void CancelButtonClick()
        {
            // 清除按钮的选中状态
            EventSystem.current.SetSelectedGameObject(null);
        }

    }
}
