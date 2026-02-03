using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace OHTools
{
    public class OHFixTargetRenderTexture : MonoBehaviour
    {
        [SerializeField, LabelText("目标 Raw Image")]
        private RawImage rawImage;
        
        private Camera cam;
        private RenderTexture m_renderTexture;
        
        private void Start()
        {
            cam = GetComponent<Camera>();
            
            // 获取原始目标纹理，确定尺寸和名称后重新创建
            RenderTexture originalRT = cam.targetTexture;
        
            int width  = originalRT.width;
            int height = originalRT.height;
            string newName = originalRT.name + "（已修复）";

            // 创建新的 RenderTexture，并改名
            m_renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            m_renderTexture.name = newName;

            // 将新纹理赋给 RawImage 和对应组件
            rawImage.texture = m_renderTexture; 
        
            cam.targetTexture = m_renderTexture;
        }
    }
}