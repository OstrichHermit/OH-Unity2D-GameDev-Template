Shader "OHEffect/OHCRTScanline"
{
    Properties
    {
        _MainTex         ("主贴图",        2D)   = "white" {}
        _ScanlineDensity ("扫描线密度",    Range(1,500)) = 200
        _ScanlineDarkness("扫描线深度",    Range(0,1))   = 0.3
        _ScanlineOffset  ("扫描线偏移",    Float)        = 0
        // ======= 以下为 Mask 组件需要的属性 =======
        // 隐藏在 Inspector 里，不想让人改的话用 [HideInInspector]
        [HideInInspector] _Stencil       ("Stencil ID",            Float) = 0
        [HideInInspector] _StencilOp     ("Stencil Operation",     Float) = 0
        [HideInInspector] _StencilComp   ("Stencil Comparison",    Float) = 8
        [HideInInspector] _StencilReadMask  ("Stencil Read Mask",  Float) = 255
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask     ("Color Mask",            Float) = 15
    }
    SubShader
    {
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
            float    _ScanlineDensity;
            float    _ScanlineDarkness;
            float    _ScanlineOffset;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.texcoord;
                o.color  = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样原始贴图
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算扫描线强度：sin 波在 [0,1] 之间振荡
                float scan = sin((i.uv.y + _ScanlineOffset) * _ScanlineDensity * 3.14159265)
                             * 0.5 + 0.5;

                // 根据扫描线深度，将亮度压低到 (1 - _ScanlineDarkness)~1 之间
                col.rgb *= (1.0 - _ScanlineDarkness * (1.0 - scan));

                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
