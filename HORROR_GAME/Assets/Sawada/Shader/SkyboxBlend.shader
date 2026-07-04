Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _Tex1 ("Panorama 1", 2D) = "white" {}
        _Tex2 ("Panorama 2", 2D) = "black" {}

        _Blend ("Blend", Range(0, 1)) = 0
        _Exposure ("Exposure", Range(0, 8)) = 1

        _MoveStrength ("Move Strength", Range(0, 0.2)) = 0.05
        _ZoomStrength ("Zoom Strength", Range(0, 0.5)) = 0.15
        _Tex2StartZoomOut ("Tex2 Start Zoom Out", Range(0, 0.5)) = 0.12

        _FocusU ("Focus U", Float) = 0.5
        _FocusV ("Focus V", Float) = 0.5

        _SeamU ("Seam U", Range(0, 1)) = 0
        _ViewCenterU ("View Center U", Range(0, 1)) = 0.5
        _ViewCenterV ("View Center V", Range(0, 1)) = 0.5

        _Rotation ("Rotation", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Tex1;
            sampler2D _Tex2;

            float _Blend;
            float _Exposure;

            float _MoveStrength;
            float _ZoomStrength;
            float _Tex2StartZoomOut;

            float _FocusU;
            float _FocusV;

            float _SeamU;
            float _ViewCenterU;
            float _ViewCenterV;

            float _Rotation;

            struct APP_DATA
            {
                float4 vertex : POSITION;
            };

            struct VERTEX_TO_FRAGMENT
            {
                float4 position : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            VERTEX_TO_FRAGMENT vert(APP_DATA appData)
            {
                VERTEX_TO_FRAGMENT vertexToFragment;
                vertexToFragment.position = UnityObjectToClipPos(appData.vertex);
                vertexToFragment.direction = appData.vertex.xyz;
                return vertexToFragment;
            }

            float2 CalculateEquirectUV(float3 dir)
            {
                dir = normalize(dir);

                float longitude = atan2(dir.x, dir.z);
                float latitude = asin(dir.y);

                float2 uv;
                uv.x = longitude / (2.0 * UNITY_PI) + 0.5;
                uv.y = latitude / UNITY_PI + 0.5;

                return uv;
            }

            float2 CalculateZoomUV(float2 uv, float zoom, float2 focus)
            {
                // 横方向だけ、focusを中心に -0.5 ～ +0.5 の範囲へ変換する
                // これにより U=0/1 の境界をまたいでも近い方向として扱える
                float uDistance = frac(uv.x - focus.x + 0.5) - 0.5;

                float vDistance = uv.y - focus.y;

                float2 uvDistance = float2(uDistance, vDistance);

                float2 zoomUV = focus + uvDistance / zoom;

                zoomUV.x = frac(zoomUV.x);
                zoomUV.y = saturate(zoomUV.y);

                return zoomUV;
            }


            fixed4 frag(VERTEX_TO_FRAGMENT inputData) : SV_Target
            {
                float2 baseUV = CalculateEquirectUV(inputData.direction);

                // ----------------------------------------
                // カメラ後方へ移動
                // ----------------------------------------
                //baseUV.x = frac(baseUV.x - _SeamU);
                float rotationU = _Rotation / 360.0;
                baseUV.x = frac(baseUV.x - _SeamU + rotationU);

                // 今見ている中心UV
                float2 viewCenterUV;
                viewCenterUV.x = frac(_ViewCenterU);
                viewCenterUV.y = _ViewCenterV;

                float transition = _Blend;
                float blendValue = smoothstep(0.0, 1.0, _Blend);

                float2 focusUV = float2(frac(_FocusU - _SeamU + rotationU), _FocusV);
                // -------------------------
                // 画像1
                // viewCenterUV 基準でズーム
                // -------------------------
                float tex1Zoom = 1.0 + _ZoomStrength * transition;
                float2 tex1UV = baseUV;
                tex1UV = CalculateZoomUV(tex1UV, tex1Zoom, focusUV);

                // -------------------------
                // 画像2
                // viewCenterUV 基準でズーム
                // -------------------------
                float tex2Zoom = lerp(1.0 - _Tex2StartZoomOut, 1.0, blendValue);
                float2 tex2UV = baseUV;
                tex2UV = CalculateZoomUV(tex2UV, tex2Zoom, focusUV);

                fixed4 tex1Color = tex2D(_Tex1, tex1UV);
                fixed4 tex2Color = tex2D(_Tex2, tex2UV);

                fixed4 blendColor = lerp(tex1Color, tex2Color, blendValue);
                blendColor.rgb *= _Exposure;

                return blendColor;
            }
            ENDCG
        }
    }
}