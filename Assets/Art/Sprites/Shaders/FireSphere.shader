Shader "URP/Particles/FireSphere"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Emission("Emission", Float) = 2
        _StartFrequency("Start Frequency", Float) = 4
        _Frequency("Frequency", Float) = 10
        _Amplitude("Amplitude", Float) = 1
        [Toggle]_Usedepth("Use depth?", Float) = 0
        _Depthpower("Depth power", Float) = 1
        [Toggle]_Useblack("Use black", Float) = 0
        _Opacity("Opacity", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float3 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float3 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float fogFactor : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Emission;
                float _StartFrequency;
                float _Frequency;
                float _Amplitude;
                float _Usedepth;
                float _Depthpower;
                float _Useblack;
                float _Opacity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.screenPos = vertexInput.positionNDC;
                output.color = input.color;
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }

            // Función de ruido (igual que el original)
            float noise(float2 uv, float freq)
            {
                float2 i = floor(uv * freq);
                float2 f = frac(uv * freq);
                
                float a = frac(sin(i.x + i.y * 57.0) * 473.5);
                float b = frac(sin(i.x + 1.0 + i.y * 57.0) * 473.5);
                float c = frac(sin(i.x + (i.y + 1.0) * 57.0) * 473.5);
                float d = frac(sin(i.x + 1.0 + (i.y + 1.0) * 57.0) * 473.5);
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Ruido de baja frecuencia
                float2 lowFreqUV = (input.uv.xy + float2(0.2, 0) * _Time.y + input.uv.z) * _StartFrequency;
                float lowNoise = noise(lowFreqUV, 1.0);
                
                // Distorsión animada
                float3 distortedUV = input.uv + lowNoise * _Amplitude;
                distortedUV += float3(0.5, 0.5, 0) * _Time.y;
                
                // Ruido de alta frecuencia
                float highNoise = noise(distortedUV.xy, _Frequency);
                
                // Sample textura con distorsión
                float2 finalUV = input.uv.xy + (highNoise * _Amplitude * 0.2);
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, finalUV);
                
                // Color emisivo
                float4 emission = _Emission * _Color * input.color;
                float3 finalColor = lerp(emission.rgb, (emission * texColor).rgb, _Useblack);
                
                // Alpha base
                float alpha = saturate(input.color.a * texColor.a * _Opacity);
                
                // Soft particles (depth fade)
                if (_Usedepth > 0.5)
                {
                    float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.screenPos.xy / input.screenPos.w), _ZBufferParams);
                    float thisZ = LinearEyeDepth(input.screenPos.z / input.screenPos.w, _ZBufferParams);
                    float fade = saturate((sceneZ - thisZ) / _Depthpower);
                    alpha *= fade;
                }
                
                float4 finalOutput = float4(finalColor, alpha);
                finalOutput.rgb = MixFog(finalOutput.rgb, input.fogFactor);
                
                return finalOutput;
            }
            ENDHLSL
        }
    }
}