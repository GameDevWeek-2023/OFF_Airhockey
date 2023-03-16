Shader "Custom/Hidden/Outline"
{

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float4 _MainTex_TexelSize;
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

            float4 _Color;
            float _Scale;

            float _DepthThreshold;
            float _DepthNormalThreshold;
            float _DepthNormalThresholdScale;
            float _NormalThreshold;

            float4x4 _ClipToView;

            float4 AlphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1 - top.a);

                return float4(color, alpha);
            }


            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord_stereo : TEXCOORD1;
                float3 view_space_dir : TEXCOORD2;
                #if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
                #endif
            };

            Varyings Vert(AttributesDefault v)
            {
                Varyings o;
                o.vertex = float4(v.vertex.xy, 0.0, 1.0);
                o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
                o.view_space_dir = mul(_ClipToView, o.vertex).xyz;

                #if UNITY_UV_STARTS_AT_TOP
                o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif

                o.texcoord_stereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

                return o;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                const float half_scale_floor = floor(_Scale * 0.5);
                const float half_scale_ceil = ceil(_Scale * 0.5);

                const float2 uv_bottom_left = i.texcoord - _MainTex_TexelSize.xy * half_scale_floor;
                const float2 uv_top_right = i.texcoord + _MainTex_TexelSize.xy * half_scale_ceil;

                const float2 uv_bottom_right = i.texcoord + float2(_MainTex_TexelSize.x * half_scale_ceil,
                                                                   -_MainTex_TexelSize.y * half_scale_floor);
                const float2 uv_top_left = i.texcoord = i.texcoord + float2(-_MainTex_TexelSize.x * half_scale_floor,
                                                                            _MainTex_TexelSize.y * half_scale_ceil);


                const float3 normal_bottom_left = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture,
                                                                   sampler_CameraDepthNormalsTexture,
                                                                   uv_bottom_left).rgb;
                const float3 normal_top_right = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture,
                                                                 sampler_CameraDepthNormalsTexture,
                                                                 uv_top_right).rgb;
                const float3 normal_bottom_right = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture,
                                                                    sampler_CameraDepthNormalsTexture,
                                                                    uv_bottom_right).rgb;
                const float3 normal_top_left = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture,
                                                                sampler_CameraDepthNormalsTexture,
                                                                uv_top_left).rgb;

                const float depth_bottom_left = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,
                                                                     sampler_CameraDepthTexture,
                                                                     uv_bottom_left).r;
                const float depth_top_left = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,
                                                                  sampler_CameraDepthTexture,
                                                                  uv_top_left).r;
                const float depth_bottom_right = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,
                                                                      sampler_CameraDepthTexture,
                                                                      uv_bottom_right).r;
                const float depth_top_right = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,
                                                                   sampler_CameraDepthTexture,
                                                                   uv_top_right).r;

                const float3 view_normal = normal_bottom_left * 2 - 1;
                const float n_dot_v = 1 - dot(view_normal, -i.view_space_dir);

                const float3 normal_finite_difference_0 = normal_top_right - normal_bottom_left;
                const float3 normal_finite_difference_1 = normal_top_left - normal_bottom_right;

                const float depth_finite_difference_0 = depth_top_right - depth_bottom_left;
                const float depth_finite_difference_1 = depth_top_left - depth_bottom_right;


                float normal_threshold = saturate((n_dot_v - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
                normal_threshold = normal_threshold * _DepthNormalThresholdScale + 1;

                float edge_depth = sqrt(pow(depth_finite_difference_0, 2) + pow(depth_finite_difference_1, 2)) * 100;

                const float depth_threshold = _DepthThreshold * depth_bottom_left * normal_threshold;
                //edge_depth = step(depth_threshold, edge_depth);
                edge_depth = edge_depth > depth_threshold ? 1 : 0;


                float edge_normal = sqrt(
                    dot(normal_finite_difference_0, normal_finite_difference_0) + dot(
                        normal_finite_difference_1, normal_finite_difference_1));
                edge_normal = step(_NormalThreshold, edge_normal);

                const float edge = max(edge_depth, edge_normal);
                const float4 edge_color = float4(_Color.rgb, _Color.a * edge);

                const float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

                return AlphaBlend(edge_color, color);
            }
            ENDHLSL
        }
    }
}