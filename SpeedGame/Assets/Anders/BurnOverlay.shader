Shader "Custom/BurnOverlay"
{
    // Overlay-shader: lægges som ekstra materiale oven på det originale.
    // Bruger multiply-blending, så originalens tekstur, lys og skygger bevares.
    Properties
    {
        [Header(Burn)]
        _BurnAmount ("Burn Amount", Range(0,1)) = 0
        _CharColor ("Char Color (multiplikator)", Color) = (0.08, 0.06, 0.05, 1)
        _ScorchColor ("Scorch Color (multiplikator)", Color) = (0.25, 0.12, 0.04, 1)
        _CharStrength ("Char Strength", Range(0,1)) = 0.9
        _NoiseScale ("Noise Scale", Float) = 8
        _Softness ("Softness", Range(0.05,1)) = 0.35
        _SootSpecks ("Soot Specks", Range(0,1)) = 0.5

        [Header(Direction and Edges)]
        _DirectionStrength ("Direction Strength (0 = random)", Range(0,2)) = 0
        _HeightMin ("Height Min (object Y)", Float) = -0.5
        _HeightMax ("Height Max (object Y)", Float) = 0.5
        _FresnelStrength ("Edges Burn First", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }

        Pass
        {
            Name "BurnOverlay"
            Tags { "LightMode"="UniversalForward" }

            Blend DstColor Zero      // multiply med det der allerede er tegnet
            ZWrite Off
            ZTest LEqual
            Offset -1, -1            // undgår z-fighting med originalen
            Cull Back
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half   _BurnAmount;
                half4  _CharColor;
                half4  _ScorchColor;
                half   _CharStrength;
                float  _NoiseScale;
                half   _Softness;
                half   _SootSpecks;
                half   _DirectionStrength;
                float  _HeightMin;
                float  _HeightMax;
                half   _FresnelStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
                float  heightOS   : TEXCOORD3;
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float ValueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                float a = Hash21(i);
                float b = Hash21(i + float2(1, 0));
                float c = Hash21(i + float2(0, 1));
                float d = Hash21(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(pos.positionWS);
                OUT.heightOS = IN.positionOS.y;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Støj
                float n1 = ValueNoise(IN.uv * _NoiseScale);
                float n2 = ValueNoise(IN.uv * _NoiseScale * 3.1 + 17.3);
                float noise = n1 * 0.6 + n2 * 0.4;

                // Retning (lav støj brænder først, så bunden får lavere værdier)
                float h = saturate((IN.heightOS - _HeightMin) / max(_HeightMax - _HeightMin, 0.0001));
                noise = (noise + h * _DirectionStrength) / (1.0 + _DirectionStrength);

                // Kanter brænder først
                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(n, v)), 3.0);
                noise = saturate(noise - fresnel * _FresnelStrength * 0.3);

                // Burn-maske (0 = urørt, 1 = fuldt brændt)
                float b = lerp(-_Softness, 1.0 + _Softness, _BurnAmount);
                float burn = 1.0 - smoothstep(b, b + _Softness, noise);
                float deep = pow(burn, 4.0);

                // Multiplikator der lægges oven på originalen (1 = ingen ændring)
                float3 mul = lerp(float3(1, 1, 1), _ScorchColor.rgb * 2.0, burn);
                mul = lerp(mul, _CharColor.rgb, deep * _CharStrength);

                // Sodpletter
                float speck = ValueNoise(IN.uv * _NoiseScale * 8.0);
                mul *= lerp(1.0, 0.6 + 0.4 * speck, burn * _SootSpecks);

                return half4(mul, 1);
            }
            ENDHLSL
        }
    }
}
