Shader "OHEffect/OHGlitchChange" {
    Properties {
        _MainTex ("当前广告纹理", 2D) = "white" {}
        _NextTex ("下一个广告纹理", 2D) = "white" {}
        _Transition ("过渡参数", Range(0,1)) = 0.0
        _GlitchIntensity ("故障强度", Range(0,1)) = 0.5
        _GlitchFrequency ("故障频率", Range(1,100)) = 30
        // ======= 以下为 Mask 组件需要的属性 =======
        // 隐藏在 Inspector 里，不想让人改的话用 [HideInInspector]
        [HideInInspector] _Stencil       ("Stencil ID",            Float) = 0
        [HideInInspector] _StencilOp     ("Stencil Operation",     Float) = 0
        [HideInInspector] _StencilComp   ("Stencil Comparison",    Float) = 8
        [HideInInspector] _StencilReadMask  ("Stencil Read Mask",  Float) = 255
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask     ("Color Mask",            Float) = 15
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" /* ... */ }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            // ---- 这里插入 Stencil 块 ----
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            ColorMask [_ColorMask]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _NextTex;
            float _Transition;
            float _GlitchIntensity;
            float _GlitchFrequency;
            
            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };
            
            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
            
            // 伪随机函数
            float random(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                
                // 计算故障权重，在过渡中段（_Transition=0.5）最大
                float glitchFactor = _GlitchIntensity * (1.0 - abs(2.0 * _Transition - 1.0));
                
                // 根据故障频率计算当前行号，确保同一行像素一致性
                float row = floor(uv.y * _GlitchFrequency);
                // 基础水平偏移，范围在 [-0.1, 0.1] 内，再乘以故障权重
                float baseOffset = (random(float2(row, _Transition)) - 0.5) * glitchFactor * 0.2;
                
                // 随机决定是否对该行放大偏移，模拟块状故障（例如20%的概率）
                float blockChance = random(float2(row, _Transition * 10.0));
                float offset = baseOffset;
                if (blockChance > 0.8) {
                    offset *= 2.0;
                }
                
                // 为 RGB 通道施加略微不同的水平偏移，制造初步色偏效果
                float2 uvR = uv + float2(offset * 1.2, 0);
                float2 uvG = uv + float2(offset, 0);
                float2 uvB = uv + float2(offset * 0.8, 0);
                
                // 分别采样当前和下一张纹理的颜色
                fixed4 current;
                current.r = tex2D(_MainTex, uvR).r;
                current.g = tex2D(_MainTex, uvG).g;
                current.b = tex2D(_MainTex, uvB).b;
                current.a = tex2D(_MainTex, uv).a;
                
                fixed4 next;
                next.r = tex2D(_NextTex, uvR).r;
                next.g = tex2D(_NextTex, uvG).g;
                next.b = tex2D(_NextTex, uvB).b;
                next.a = tex2D(_NextTex, uv).a;
                
                // 根据 _Transition 平滑混合当前与下一张纹理
                fixed4 blended = lerp(current, next, _Transition);
                
                // 添加额外随机闪烁效果，使部分区域出现突发性强故障
                float flicker = step(0.95, random(uv * _GlitchFrequency + _Transition));
                blended.rgb += flicker * glitchFactor * 0.3;
                
                // 增加额外色彩偏移效果：基于故障频率和时间产生不同相位的水平位移
                float extraR = sin(_GlitchFrequency * uv.y + _Time.y * 5.0) * glitchFactor * 0.02;
                float extraG = sin(_GlitchFrequency * uv.y + _Time.y * 5.0 + 2.094) * glitchFactor * 0.02;
                float extraB = sin(_GlitchFrequency * uv.y + _Time.y * 5.0 + 4.188) * glitchFactor * 0.02;
                
                float2 uvR_extra = uv + float2(extraR, 0);
                float2 uvG_extra = uv + float2(extraG, 0);
                float2 uvB_extra = uv + float2(extraB, 0);
                
                fixed4 extraColor;
                extraColor.r = tex2D(_MainTex, uvR_extra).r;
                extraColor.g = tex2D(_MainTex, uvG_extra).g;
                extraColor.b = tex2D(_MainTex, uvB_extra).b;
                extraColor.a = tex2D(_MainTex, uv).a;
                
                // 将额外色彩偏移效果与原始混合，混合比例可根据需要调整
                blended.rgb = lerp(blended.rgb, extraColor.rgb, glitchFactor * 0.5);
                
                return blended;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
