using System;
using Sirenix.OdinInspector;
using UnityEngine;


namespace OHTools
{
    public class OHScanlinePostProcessing : MonoBehaviour
    {
        [LabelText("扫描线材质（自动创建）"), SerializeField,ReadOnly] private Material scanlineMaterial;
        [LabelText("扫描线密度"), SerializeField, Range(1, 500)] private float scanlineDensity = 200f;
        [LabelText("扫描线深度"), SerializeField, Range(0f, 1f)] private float scanlineDarkness = 0.3f;
        [LabelText("扫描线动画速度"), SerializeField] private float offsetSpeed = 1.0f;

        private float _scanlineOffset = 0f;

        private void Start()
        {
            // 创建材质
            scanlineMaterial = new Material(Shader.Find("OHEffect/OHCRTScanline"));
            // 写入参数
            scanlineMaterial.SetFloat("_ScanlineDensity", scanlineDensity);
            scanlineMaterial.SetFloat("_ScanlineDarkness", scanlineDarkness);
        }

        void Update()
        {
            if (scanlineMaterial == null)
                return;

            // 动画偏移累加
            _scanlineOffset += offsetSpeed * 0.001f * Time.deltaTime;
            if (_scanlineOffset > Mathf.PI * 2f)
                _scanlineOffset -= Mathf.PI * 2f;
            scanlineMaterial.SetFloat("_ScanlineOffset", _scanlineOffset);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (scanlineMaterial != null)
                Graphics.Blit(src, dest, scanlineMaterial);
            else
                Graphics.Blit(src, dest);
        }

        void OnDestroy()
        {
            Destroy(scanlineMaterial);
        }
    }
}