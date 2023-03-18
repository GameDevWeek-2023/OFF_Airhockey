Shader "Hidden/Custom/CRT"
{
    Subshader{
        Cull Off 
        ZWrite Off 
        ZTest Always
    
        HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Bend;
        
        float _CornerInset;
        float _Smoothness;
           
        int _Samples;
        float _Offset;
        float _Scale;
        float _Weight;
        
        float _Frequency;
        float _Amplitude;
        float _ScanlineStrength;
        
        float2 _Resolution;
        
        #define PI 3.14159265

        ENDHLSL
        
        Pass{
        
           HLSLPROGRAM
           #pragma vertex VertDefault
           #pragma fragment Frag
           
           float2 crt(float2 coord, float bend)
            {
                // put in symmetrical coords
                coord = (coord - 0.5) * 2.0;
            
                coord *= 1.1;	
            
                // deform coords
                coord.x *= 1.0 + pow((abs(coord.y) / bend), 2.0);
                coord.y *= 1.0 + pow((abs(coord.x) / bend), 2.0);
            
                // transform back to 0.0 - 1.0 space
                coord  = (coord / 2.0) + 0.5;
            
                return coord;
            }
            
            float Vignette(float2 uv, float cornerInset, float smoothness){
                uv = (uv - 0.5) * 2.0;
                float2 vignetteOffset = pow(abs(uv), float2(8.0, 8.0));
                float vignette = 1.0 - 0.5 * max(vignetteOffset.x, vignetteOffset.y);
                float2 cornerVec = max(abs(uv) + cornerInset - 1, 0.0) / cornerInset;
                vignette *= 1.0 - smoothstep(1, smoothness, length(cornerVec));
                return vignette;
            }
            
            float Flicker(float strength){
                float x = 1 - strength;
                return fmod(float(((int)(_Time.y / unity_DeltaTime.x))), 4.0) >= 2.0 ? 1.0 : x;
            }
            
            float3 rgb2yiq(float3 c){   
					return float3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
					);
				}

	        float3 yiq2rgb(float3 c){				
					return float3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
					);
				}
            
            float2 Circle(float Start, float Points, float Point) 
            {
                float Rad = (3.141592 * 2.0 * (1.0 / Points)) * (Point + Start);
                //return vec2(sin(Rad), cos(Rad));
                return float2(-(.3+Rad), cos(Rad));
            }
            
            float3 Blur(float2 uv, float frequency, float offset, float scale, float weight){
                float t = 0.0;
                
                //d=abs(d);
                //t = (sin(_Time.y * 5.0 + uv.y * 5.0)) / 10.0;
                t = (sin(_Time.y * 5.0 + frequency) / 10.0) * 0.08;
                //uv += Flicker(0.05) * 0.01;
                
                float2 PixelOffset = float2(offset + t, 0);
                
                float Start = 2.0 / _Samples - 1;
                float3 blurred = float4(0, 0, 0, 1);
                
                for(int i = 0; i < _Samples-1; ++i){
                    blurred += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + Circle(Start, 14.0, float(i)) * (scale * PixelOffset.xy)).rgb;
                }
                
                blurred /= _Samples;
        
                return  blurred.rgb * weight;
            }
            
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }
            
            float VerticalBar(float pos, float uvY, float offset, float range)
            {
                float edge0 = (pos - range);
                float edge1 = (pos + range);
            
                float x = smoothstep(edge0, pos, uvY) * offset;
                x -= smoothstep(pos, edge1, uvY) * offset;
                return x;
            }
            
            float scanline(float2 uv, float frequency) {
                return sin(frequency * uv.y * 0.7 + _Time.y * 10.0);
            }
            
            float slowscan(float2 uv) {
                return sin(_Resolution.y * uv.y * 0.02 + _Time.y * 6.0);
            }
            
            float Glitch(float2 uv){
                float time360 = fmod(_Time.y, 360);
                float pos = 1 - fmod(_Time.y * 0.3 + rand(float2(time360, time360)), 1);
                return VerticalBar(pos, uv.y, 0.005 * rand(float2(time360, time360)), 0.01);
            }
            
           float4 Frag(VaryingsDefault input) : SV_Target{
                float2 uv = input.texcoord.xy;
                uv = crt(uv, _Bend);
                float4 color = float4(0, 0, 0, 1);
                
                float vignette = Vignette(uv, _CornerInset, _Smoothness);
                
                uv.x -= Glitch(uv);
                
                float s = _Amplitude * 0.005;
                uv.x -= scanline(uv, _Frequency) * s;
                                                                		              
		        float c = 0.02;
		        color.rgb = Blur(uv, _Offset + 0.0, c+c*(uv.x), _Scale, _Weight);
		        float y = rgb2yiq(color.rgb).r;
		        
		        c *= 6.0;
		        color.rgb = Blur(uv, _Offset + 0.333, c, _Scale, _Weight);
		        float i = rgb2yiq(color.rgb).g;

		        c *= 2.5;
		        color.rgb = Blur(uv, _Offset + 0.666, c, _Scale, _Weight);
		        float q = rgb2yiq(color.rgb).b;
		        
		        color.rgb = yiq2rgb(float3(y,i,q));
		        
		        color = lerp(color, lerp(slowscan(uv) * _ScanlineStrength, 
		        scanline(uv, _Frequency) * _ScanlineStrength, 0.5), 0.015);
                return color * vignette;
           }
           
           ENDHLSL
        
        }
        
    }
}

