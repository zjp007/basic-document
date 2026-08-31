Shader "Custom/URP/Hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HologramColor ("Hologram Color", Color) = (0, 1, 1, 1)
        _RimColor ("Rim Color", Color) = (0, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 3.0
        
        // 扫描线效果
        _ScanlineSpeed ("Scanline Speed", Float) = 1.0
        _ScanlineWidth ("Scanline Width", Range(0.01, 1)) = 0.1
        _ScanlineIntensity ("Scanline Intensity", Range(0, 5)) = 2.0
        
        // 闪烁效果
        _FlickerSpeed ("Flicker Speed", Float) = 5.0
        _FlickerAmount ("Flicker Amount", Range(0, 1)) = 0.3
        
        // 透明度
        _Alpha ("Alpha", Range(0, 1)) = 0.5
        
        // 噪点效果
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Name "HologramPass"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
                float fogFactor : TEXCOORD4;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _HologramColor;
                float4 _RimColor;
                float _RimPower;
                float _ScanlineSpeed;
                float _ScanlineWidth;
                float _ScanlineIntensity;
                float _FlickerSpeed;
                float _FlickerAmount;
                float _Alpha;
                float _GlitchIntensity;
            CBUFFER_END
            
            // 简单噪声函数
            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // 基础纹理
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Rim Light (边缘光)
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                float rimAmount = 1.0 - saturate(dot(normalWS, viewDirWS));
                float rimLight = pow(rimAmount, _RimPower);
                
                // 扫描线效果
                float scanline = frac((input.positionWS.y + _Time.y * _ScanlineSpeed) * 10.0);
                scanline = smoothstep(0.0, _ScanlineWidth, scanline) * smoothstep(1.0, 1.0 - _ScanlineWidth, scanline);
                scanline *= _ScanlineIntensity;
                
                // 闪烁效果
                float flicker = sin(_Time.y * _FlickerSpeed) * 0.5 + 0.5;
                flicker = lerp(1.0, flicker, _FlickerAmount);
                
                // 噪点/故障效果
                float noise = random(input.uv + _Time.y);
                float glitch = lerp(1.0, noise, _GlitchIntensity);
                
                // 合成最终颜色
                half3 hologramEffect = _HologramColor.rgb * texColor.rgb;
                hologramEffect += _RimColor.rgb * rimLight;
                hologramEffect += scanline * _HologramColor.rgb;
                hologramEffect *= flicker * glitch;
                
                // 最终透明度
                float alpha = _Alpha * texColor.a * flicker;
                alpha += rimLight * 0.5;
                alpha = saturate(alpha);
                
                half4 finalColor = half4(hologramEffect, alpha);
                
                // 应用雾效
                finalColor.rgb = MixFog(finalColor.rgb, input.fogFactor);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Unlit"
}