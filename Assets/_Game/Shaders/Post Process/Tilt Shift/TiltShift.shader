Shader "Custom/Hidden/Tilt Shift"
{
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        #define PI 3.14159265359
        #define E 2.71828182846

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Intensity;
        float _BlurAmount;
        float _StandardDeviation;
        float _Center;
        float _TopAmount;
        float _BottomAmount;
        int _Steps;
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment ShiftX

            float4 ShiftX(VaryingsDefault i) : SV_Target
            {
                if (_StandardDeviation == 0) return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

                const float min_offs = float(_Steps - 1) / -2;
                const float max_offs = float(_Steps - 1) / 2;

                const float top = smoothstep(1.0 - _TopAmount, 1.0, i.texcoord.y);
                const float bottom = smoothstep(_BottomAmount, 0.0, i.texcoord.y);

                const float amount = (top + bottom) * 0.01 * _BlurAmount;

                //const float amount = pow((i.texcoord.y * _Center) * 2.0 - 1.0, 2.0) * _BlurAmount;
                float sum = 0;

                float4 blurred = 0;
                for (float offs_x = min_offs; offs_x <= max_offs; ++offs_x)
                {
                    float offset = offs_x * _BlurAmount * amount;
                    const float2 temp_coord = i.texcoord + float2(offset, 0);

                    const float st_dev_squared = _StandardDeviation * _StandardDeviation;
                    const float gauss = (1 / sqrt(2 * PI * st_dev_squared)) * pow(
                        E, -((offset * offset) / (2 * st_dev_squared)));
                    sum += gauss;

                    blurred += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, temp_coord) * gauss;
                }

                blurred /= sum;
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                color.rgb = lerp(color.rgb, blurred.rgb, _Intensity);

                return color;
            }
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment ShiftY

            float4 ShiftY(VaryingsDefault i) : SV_Target
            {
                if (_StandardDeviation == 0) return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

                const float min_offs = float(_Steps - 1) / -2;
                const float max_offs = float(_Steps - 1) / 2;

                const float top = smoothstep(1.0 - _TopAmount, 1.0, i.texcoord.y);
                const float bottom = smoothstep(_BottomAmount, 0.0, i.texcoord.y);

                const float amount = (top + bottom) * 0.01 * _BlurAmount;

                //const float amount = pow((i.texcoord.y * _Center) * 2.0 - 1.0, 2.0) * _BlurAmount * 0.01f;

                float4 blurred = 0;
                float sum = 0;

                for (float offs_y = min_offs; offs_y <= max_offs; ++offs_y)
                {
                    float offset = offs_y * _BlurAmount * amount;
                    const float2 temp_coord = i.texcoord + float2(0, offset);

                    const float st_dev_squared = _StandardDeviation * _StandardDeviation;
                    const float gauss = (1 / sqrt(2 * PI * st_dev_squared)) * pow(
                        E, -((offset * offset) / (2 * st_dev_squared)));
                    sum += gauss;

                    blurred += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, temp_coord) * gauss;
                }

                blurred /= sum;
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                color.rgb = lerp(color.rgb, blurred.rgb, _Intensity);

                return color;
            }
            ENDHLSL
        }
    }
}