Shader "Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 0, 0)

        [Header(Foam)]
        [Space(6)]
        _FoamTex("Texture", 2D) = "white"{}
        _FoamDistortion("Distortion", 2D) = "white"{}
        _FoamDistortionStrength("Distortion Strength", Range(0, 1)) = 0.5
        _FoamColor("Color", Color) = (1, 1, 1)
        _FoamStrength("Strength", float) = 0.15
        _FoamThreshold("Threshold", float) = 0.2

        [Header(Fog)]
        [Space(6)]
        _FogColor("Color", Color) = (1, 1, 1)
        _FogThreshold("Threshold", float) = 0.5

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
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 world_pos : TEXCOORD4;
                float4 screen_pos : TEXCOORD1;
                float eye_depth : TEXCOORD2;
                float dot_normal : TEXCOORD6;
            };


            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_TexelSize;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed3 _Color;

            sampler2D _FoamTex;
            float4 _FoamTex_ST;

            sampler2D _FoamDistortion;
            float4 _FoamDistortion_ST;

            float _FoamDistortionStrength, _FoamThreshold;
            fixed3 _FoamColor;
            float _FoamStrength;

            float _FogThreshold;
            fixed3 _FogColor;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                o.screen_pos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.eye_depth);

                o.dot_normal = dot(v.normal, float3(0.0, 1.0, 0.0));
                return o;
            }

            float SampleDepth(const float4 screen_pos, const float2 eye_depth)
            {
                const float raw_depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(screen_pos)).r;
                const float persp = LinearEyeDepth(raw_depth);
                const float ortho = (_ProjectionParams.z - _ProjectionParams.y) * (1 - raw_depth) + _ProjectionParams.y;
                const float depth = lerp(persp, ortho, unity_OrthoParams.w);
                return abs(eye_depth - depth);
            }


            fixed4 frag(v2f i) : SV_Target
            {
                const float2 dir_1 = float2(_Time.x, 0.0f);
                const float2 dir_2 = float2(0.0f, _Time.x);

                const float2 distortion_uv = (i.world_pos.xz * _FoamDistortion_ST.xy) + _FoamDistortion_ST.zw + dir_1;
                const float3 distortion_tex = UnpackNormalWithScale(tex2D(_FoamDistortion, distortion_uv),
                                                                    _FoamDistortionStrength);

                const float2 main_uv = (i.world_pos.xz * _MainTex_ST.xy) * _MainTex_ST.zw;
                fixed3 main_tex = tex2D(_MainTex, main_uv + distortion_tex.xz + dir_2);
                main_tex += (1 - main_tex) * 0.5f;
                main_tex *= _Color.xyz;

                const float depth = SampleDepth(i.screen_pos, i.eye_depth);

                const float fog_diff = saturate(depth / _FogThreshold);
                const float foam_diff = saturate(depth / _FoamThreshold);

                const float3 foam_tex = tex2D(
                    _FoamTex, (i.world_pos.xz * _FoamTex_ST.xy + _FoamTex_ST.zw) + (distortion_tex.xz));
                const float foam = saturate(step(foam_diff + foam_tex, 0.99f) + step(foam_tex, _FoamStrength));

                float3 col = lerp(_FogColor, main_tex.xyz, fog_diff);
                col = lerp(col, _FoamColor, foam);

                return float4(col, 1);
            }
            ENDCG
        }
    }
}