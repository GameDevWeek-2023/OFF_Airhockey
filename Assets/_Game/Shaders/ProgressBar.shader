Shader "Unlit/ProgressBar"
{
    Properties
    {
        [HDR] _Color("Color", color) = (1, 1, 1, 1)
        [HDR] _BgColor("Background Color", color) = (0, 0, 0, 1)
        _Progress("Progress", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 billboard(appdata v)
            {
                return mul(UNITY_MATRIX_P,
                           mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
                           + float4(v.vertex.x, v.vertex.y, 0.0, 0.0)
                           * float4(0.1, 0.1, 1.0, 1.0));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 _Color, _BgColor;
            float _Progress;

            fixed4 frag(v2f i) : SV_Target
            {
                const float t = step(i.uv.x, _Progress);
                const fixed4 color = lerp(_BgColor, _Color, t);
                return color;
            }
            ENDCG
        }
    }
}