Shader "Custom/ColorblindURP"
{
    Properties
    {
        _Type ("Type (0: Normal, 1: Protanopia, 2: Deuteranopia, 3: Tritanopia)", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            Name "ColorblindPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // OBLIGATORIO EN URP: Constant Buffer para sincronizar el material con C# e Inspector
            CBUFFER_START(UnityPerMaterial)
                int _Type;
            CBUFFER_END

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);

                if (_Type == 1) // Protanopia
                {
                    float r = col.r * 0.56667 + col.g * 0.43333;
                    float g = col.r * 0.55833 + col.g * 0.44167;
                    float b = col.r * 0.0 + col.g * 0.24167 + col.b * 0.75833;
                    col = half4(r, g, b, col.a);
                }
                else if (_Type == 2) // Deuteranopia
                {
                    float r = col.r * 0.625 + col.g * 0.375;
                    float g = col.r * 0.7 + col.g * 0.3;
                    float b = col.r * 0.0 + col.g * 0.3 + col.b * 0.7;
                    col = half4(r, g, b, col.a);
                }
                else if (_Type == 3) // Tritanopia
                {
                    float r = col.r * 0.95 + col.g * 0.05;
                    float g = col.r * 0.0 + col.g * 0.43333 + col.b * 0.56667;
                    float b = col.r * 0.0 + col.g * 0.475 + col.b * 0.525;
                    col = half4(r, g, b, col.a);
                }

                return col;
            }
            ENDHLSL
        }
    }
}