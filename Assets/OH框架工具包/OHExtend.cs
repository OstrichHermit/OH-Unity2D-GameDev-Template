using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OHTools
{
    public static class OHTMPExtend
    {
        // 对 TMP_Text 执行逐字符显示打字机效果（目前已使用 Text Animator 插件代替）
        // public static Sequence OHTypeWriter(this TextMeshProUGUI tmp, string newText, float duration, 
        //     bool isTotalDuration = false)
        // {
        //     // 创建一个暂停状态的 Sequence
        //     var seq = DOTween.Sequence();
        //
        //     // STEP 1: 播放到第一帧时才清空文本
        //     seq.AppendCallback(() => {
        //         tmp.text = string.Empty;
        //     });
        //     
        //     // 计算每个字符的间隔时长
        //     int charCount = newText.Length;
        //     float totalDuration = charCount > 0 ? duration / charCount : 0f;
        //     
        //     // STEP 2: 逐字符追加回调与间隔
        //     for (int i = 1; i <= newText.Length; i++)
        //     {
        //         int len = i; // 闭包保护
        //         seq.AppendCallback(() =>
        //         {
        //             tmp.text = newText.Substring(0, len);
        //         });
        //         // 根据是否为总时长设定文字显示速度
        //         seq.AppendInterval(isTotalDuration ? totalDuration : duration);
        //     }
        //     
        //     return seq;
        // }
    }

    public static class OHStringExtend
    {
        // 将十六进制的颜色字符串（例如 "#RRGGBB" 或 "#RRGGBBAA"）转换为 Color
        public static Color OHHexToColor(this string hex)
        {
            if (!hex.StartsWith("#"))
            {
                hex = "#" + hex;
            }

            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
 
            Debug.LogWarning($"无法解析颜色字符串：{hex}");
            return Color.white;
        }
    }
    
    public static class OHImageExtend
    {
        // 通用的效果结束清理方法
        public static Sequence OHEndEffect(this Image image, Material originalMat = null)
        {
            // 缓存当前效果材质
            var animMat = image.material;

            // 创建一个 Sequence
            var seq = DOTween.Sequence();
            
            if (originalMat == null)
            {
                originalMat = new Material(Shader.Find("UI/Default"));
            }

            // —— 播放完后清理，恢复原材质 —— //
            seq.AppendCallback(() =>
            {
                image.material = originalMat;
                image.SetMaterialDirty();
                Object.Destroy(animMat);
            });
            
            return seq;
        }
        
        // 从白到黑的闪烁
        public static Sequence OHFlashWhiteToBlack(this Image image, float duration, bool isClear = true)
        { 
            // 缓存原始颜色
            var originalColor = image.color;

            // 创建一个 Sequence
            var seq = DOTween.Sequence();

            // STEP 1: 构建“白→黑”动画
            if (isClear)
            {
                image.color = Color.clear;
            }
            seq.Append(image.DOColor(Color.white, duration));
            seq.Append(image.DOColor(Color.black, duration));

            // STEP 2: 播放完后恢复原始颜色
            seq.AppendCallback(() =>
            {
                image.color = originalColor;
            });

            return seq;
        }
        
        // 从黑到白的闪烁
        public static Sequence OHFlashBlackToWhite(this Image image ,float duration, bool isClear = true)
        {
            // 缓存原始颜色
            var originalColor = image.color;

            // 创建一个 Sequence
            var seq = DOTween.Sequence();

            // STEP 1: 构建“黑→白”动画
            if (isClear)
            {
                image.color = Color.clear;
            }
            seq.Append(image.DOColor(Color.black, duration));
            seq.Append(image.DOColor(Color.white, duration));

            // STEP 2: 播放完后恢复原始颜色
            seq.AppendCallback(() =>
            {
                image.color = originalColor;
            });

            return seq;
        }
        
        // 用故障艺术特效切换图片
        public static Sequence OHGlitchChangeSprite(this Image image, Sprite nextSprite, float duration)
        {
            // 缓存原材质
            var originalMat = image.material;
            Material animMat = null;

            // 创建一个 Sequence
            var seq = DOTween.Sequence();

            // STEP 1: 播放到第一帧时，new 材质并设置贴图/参数
            seq.AppendCallback(() =>
            {
                animMat = new Material(Shader.Find("OHEffect/OHGlitchChange"));
                image.material = animMat;
                image.SetMaterialDirty();

                animMat.SetTexture("_MainTex", image.sprite.texture);
                animMat.SetTexture("_NextTex", nextSprite.texture);
                animMat.SetFloat("_Transition", 0f);
            });

            // STEP 2: 用 runtimeMat 做 transition
            seq.Append(
                DOTween.To(
                    () => image.materialForRendering.GetFloat("_Transition"),
                    x => image.materialForRendering.SetFloat("_Transition", x),
                    1f,
                    duration
                ).SetEase(Ease.Linear)
            );

            // STEP 3: 播放完后替换 sprite、恢复材质并销毁临时材质
            seq.AppendCallback(() =>
            {
                image.sprite = nextSprite;
                image.OHEndEffect(originalMat);
            });

            return seq;
        }
        
        // 渐变切换图片
        public static Sequence OHFadeChangeSprite(this Image image, Sprite nextSprite, float duration)
        {
            // 1. 生成一个叠加用的 Image
            var overlayGO = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image));
            var overlayImage = overlayGO.GetComponent<Image>();
            overlayImage.sprite = nextSprite;
            overlayImage.SetNativeSize();
            overlayImage.color  = new Color(1,1,1,0);
            overlayImage.rectTransform.anchorMin = Vector2.zero;
            overlayImage.rectTransform.anchorMax = Vector2.one;
            overlayImage.rectTransform.offsetMin = overlayImage.rectTransform.offsetMax = Vector2.zero;
            overlayImage.raycastTarget = false;
            overlayGO.transform.SetParent(image.transform, false);
            overlayGO.transform.SetAsLastSibling();
            
            // 2. 同时淡出 baseImage 和淡入 overlay
            var seq = DOTween.Sequence();
            seq.Append(image.DOFade(0f, duration).SetEase(Ease.Linear));
            seq.Join(overlayImage.DOFade(1f, duration).SetEase(Ease.Linear));
            
            // 3. 结束后把 baseImage 换成新 sprite，恢复不透明，并删除 overlay
            seq.AppendCallback(() => 
            {
                image.sprite = nextSprite;
                image.color = Color.white;   // 恢复为全不透明
                GameObject.Destroy(overlayGO);
            });
            return seq;
        }
        
        // 霓虹灯故障闪烁特效
        public static Sequence OHNeonFlicker(this Image image,  Color emissionColor, float maxFlicker = 1f, 
            float minFlicker = 0.2f, float speed = 4f,bool isLoop = true, float totalDuration = 1f)
        {
            // 缓存原材质
            var originalMat = image.material;
            Material animMat = null;

            // 创建一个空 Sequence，默认 Pause
            var seq = DOTween.Sequence();

            // —— STEP 1: 播放到第一帧时才替换材质并设置参数 —— //
            seq.AppendCallback(() =>
            {
                animMat = new Material(Shader.Find("OHEffect/OHNeonFlicker"));
                image.material = animMat;
                image.SetMaterialDirty();

                // 初始化贴图、颜色、发光和起始亮度
                animMat.SetTexture("_MainTex", image.sprite.texture);
                animMat.SetColor("_Color", image.color);
                animMat.SetColor("_EmissionColor", emissionColor);
                animMat.SetFloat("_MinFlicker", minFlicker);
                animMat.SetFloat("_MaxFlicker", maxFlicker);
                animMat.SetFloat("_Speed", speed);
            });

            // 如果无限循环，就直接返回一个不会自动结束的 Sequence
            if (isLoop)
            {
                return seq;
            }

            // 否则定时 totalDuration 后关闭，然后调用通用清理
            seq.AppendInterval(totalDuration);
            
            seq.AppendCallback(() => 
            { 
                image.OHEndEffect(originalMat); 
            });

            return seq;
        }
    }
}
