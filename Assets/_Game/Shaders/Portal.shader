Shader "Air/Portal"
{
    Properties
    {
        [Header(Noise)]
        [Space(6)]
        [HDR]_NoiseSwirlColor("Swirl Color", Color) = (1, 0, 0, 1)
        [HDR] _NoiseGlowColor("Glow Color", Color) = (1, 0, 0, 1)
        _NoiseEmission("Emission", Float) = 10
        _NoiseTex("Texture (R)", 2D) = "white"{}
        _NoiseThreshold("Threshold", Range(0.0, 1.0)) = 0.1

        [Header(Refraction)]
        [Space(6)]
        _RefractionTex("Texture (Bump)", 2D) = "bump"{}
        _RefractionStrength("Strength", Float) = 1

        [Header(Twirl)]
        [Space(6)]
        _TwirlStrength("Strength", Float) = 1
        _TwirlSpeed("Speed", Float) = 1

        [Header(Mask)]
        [Space(6)]
        _MaskTex("Mask (R)", 2D) = "white"{}
        _MaskScale("Scale", Float) = 1
        _MaskSmoothness("Smoothness", Float) = 1
        _MaskCenterX("Center X", Range(-1, 1)) = 0.5
        _MaskCenterY("Center Y", Range(-1, 1)) = 0.5

        [Header(Rim)]
        [Space(6)]
        _RimThickness("Thickness", float) = 0.2

        _Cutoff("Cutoff", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100
        Cull off

        GrabPass
        {
            "_GrabTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #define SCALE 100

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 grab_uv : TEXCOORD1;
            };

            sampler2D _GrabTexture;

            sampler2D _MaskTex;
            float _MaskScale, _MaskSmoothness;
            float _MaskCenterX, _MaskCenterY;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            fixed4 _NoiseSwirlColor, _NoiseGlowColor;
            float _NoiseEmission;
            float _NoiseThreshold;

            sampler2D _RefractionTex;
            float _RefractionStrength;

            float _TwirlStrength, _TwirlSpeed;

            float _RimThickness;

            float _Cutoff;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grab_uv = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            float2 Twirl(float2 UV, float2 Center, float Strength, float2 Offset)
            {
                float2 delta = UV - Center;
                const float angle = Strength * length(delta);
                const float x = cos(angle) * delta.x - sin(angle) * delta.y;
                const float y = sin(angle) * delta.x + cos(angle) * delta.y;
                return float2(x + Center.x + Offset.x, y + Center.y + Offset.y);
            }

            float Circle(in float2 uv, in float2 center, in float radius, float smoothness)
            {
                const float2 dist = uv - center;
                return 1. - smoothstep(radius - (radius * smoothness),
                                       radius + (radius * smoothness),
                                       dot(dist, dist) * 4.0);
            }

            float2 PolarCoordinates(float2 uv, float2 center, float radial_scale, float length_scale)
            {
                float2 delta = uv - center;

                float radius = length(delta) * 2.0 * radial_scale;
                float angle = atan2(delta.x, delta.y) * length_scale;

                return float2(radius, angle);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float f1 = saturate(_Cutoff * 2.0);

                float2 coords = PolarCoordinates(i.uv, float2(_MaskCenterX, _MaskCenterY), _MaskScale, _MaskSmoothness);
                float cutoff_noise = tex2D(_NoiseTex, i.uv + _Time.y * 0.1) * 0.25;

                float2 swirl_uv = float2(coords.x + _Time.y, coords.x + coords.y) * 5.0;
                swirl_uv += cutoff_noise * 0.5;

                const float swirl_1 = tex2D(_NoiseTex, swirl_uv * 0.05);
                const float swirl_2 = tex2D(_NoiseTex, float2(sin(swirl_uv.y), cos(swirl_uv.y)));

                const float swirl = (swirl_1 + swirl_2) / 2.0;

                cutoff_noise += swirl;
                const float cutoff = step(coords.r + cutoff_noise, f1);
                const float rim = smoothstep(f1 - _RimThickness, f1, coords.r + cutoff_noise);

                const float2 offset = swirl.xx * 0.1;
                fixed3 color = tex2Dproj(_GrabTexture, i.grab_uv + half4(offset, 0, 0));
                color = lerp(color, _NoiseGlowColor.rgb, _NoiseGlowColor.a);
                color = saturate(color + rim + swirl);

                clip(cutoff - 1.0);

                return fixed4(color, 1);


                /*const float2 uv = Twirl(i.uv, 0.5, _TwirlStrength, (_Time.x * 0.5) * _TwirlSpeed);

                //float mask = pow(tex2D(_MaskTex, i.uv).r, _MaskScale);
                const float mask = Circle(i.uv, float2(_MaskCenterX, _MaskCenterY), _MaskScale, _MaskSmoothness);
                const float noise = step(tex2D(_NoiseTex, uv), _NoiseThreshold) * mask;
                
                const half3 map = UnpackNormal(tex2D(_RefractionTex, uv));
                half3 offset = map * (_RefractionStrength / SCALE) * mask;

                fixed4 col = tex2Dproj(_GrabTexture, i.grab_uv + half4(offset, 0));
                col = lerp(col, _NoiseGlowColor , mask);
                col += noise * _NoiseSwirlColor * _NoiseEmission;*/
            }
            ENDCG
        }
    }
}