Shader "OHEffect/OHNeonFlicker"
{
    Properties {
        _MainTex       ("Base Texture",       2D)    = "white" {}
        _Color         ("Tint Color",         Color) = (1,1,1,1)
        _EmissionColor ("Emission Color",     Color) = (1,1,1,1)
        
        // GPU 自驱动闪烁参数，始终生效
        _MinFlicker    ("Min Flicker",        Range(0,1)) = 0.2
        _MaxFlicker    ("Max Flicker",        Range(0,1)) = 1.0
        _Speed         ("Flicker Speed",      Float)      = 2.0

        // ======== 以下为 Mask 组件需要的属性 ========
        [HideInInspector] _Stencil           ("Stencil ID",            Float) = 0
        [HideInInspector] _StencilOp         ("Stencil Operation",     Float) = 0
        [HideInInspector] _StencilComp       ("Stencil Comparison",    Float) = 8
        [HideInInspector] _StencilReadMask   ("Stencil Read Mask",     Float) = 255
        [HideInInspector] _StencilWriteMask  ("Stencil Write Mask",    Float) = 255
        [HideInInspector] _ColorMask         ("Color Mask",            Float) = 15
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            // ---- Stencil 配置 ----
            Stencil {
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

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            struct v2f {
                float2 uv      : TEXCOORD0;
                float4 pos     : SV_POSITION;
            };

            sampler2D _MainTex;
            float4   _MainTex_ST;
            float4   _Color;
            float4   _EmissionColor;
            float    _MinFlicker;
            float    _MaxFlicker;
            float    _Speed;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // 基础贴图与色彩
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                // 自驱动正弦闪烁
                // _Time.y 是秒级时间，* _Speed 控制频率
                float t = _Time.y * _Speed;
                // sin 输出 [-1,1] → [0,1]
                float s = (sin(t * UNITY_PI * 2) + 1) * 0.5;
                float flick = lerp(_MinFlicker, _MaxFlicker, s);

                // 发光叠加
                baseCol.rgb += _EmissionColor.rgb * flick;
                return baseCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
