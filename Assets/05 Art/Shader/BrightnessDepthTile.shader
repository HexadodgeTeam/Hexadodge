Shader "Custom/BrightnessDepthTile"
{
    Properties
    {
        [MainTexture][PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="TransparentCutout"
            "Queue"="AlphaTest"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Cutoff;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                return OUT;
            }

            struct FragOut
            {
                float4 color : SV_Target;
                float depth : SV_Depth;
            };

            FragOut frag(Varyings IN)
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                clip(col.a - _Cutoff);

                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));

                FragOut OUT;
                OUT.color = col;
                OUT.depth = luminance;
                return OUT;
            }
            ENDHLSL
        }
    }
}