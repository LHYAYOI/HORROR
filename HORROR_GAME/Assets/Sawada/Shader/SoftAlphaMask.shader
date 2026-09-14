Shader "UI/SoftAlphaMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        _Feather ("Feather", Range(0.0, 1.0)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;

            float4 _Color;
            float _Feather;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col =
                    tex2D(_MainTex, i.uv) * i.color;

                float2 uv = i.uv;

                // 元のマスク
                float center =
                    tex2D(_MaskTex, uv).a;

                // 周囲のAlphaを取得
                float mask = 0;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(-_Feather, -_Feather)
                ).a;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(0, -_Feather)
                ).a;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(_Feather, -_Feather)
                ).a;


                mask += tex2D(
                    _MaskTex,
                    uv + float2(-_Feather, 0)
                ).a;

                mask += center;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(_Feather, 0)
                ).a;


                mask += tex2D(
                    _MaskTex,
                    uv + float2(-_Feather, _Feather)
                ).a;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(0, _Feather)
                ).a;

                mask += tex2D(
                    _MaskTex,
                    uv + float2(_Feather, _Feather)
                ).a;

                mask /= 9.0;

                // 元々マスク外だった場所は表示しない
                mask *= center;

                col.a *= mask;

                return col;
            }

            ENDCG
        }
    }
}