Shader "Custom/BurnOverlay"
{
    // Overlay-shader: lægges som ekstra materiale oven på det originale.
    // Bruger multiply-blending, så originalens tekstur, lys og skygger bevares.
    // Tre trin: Scorch (lysebrun svidning) -> Brown (mørk brun) -> Char (sort kul).
    Properties
    {
        [Header(Burn)]
        _BurnAmount ("Burn Amount", Range(0,1)) = 0
        _ScorchColor ("Scorch Color (lys brun, multiplikator)", Color) = (0.75, 0.5, 0.3, 1)
        _BrownColor ("Brown Color (mørk brun, multiplikator)", Color) = (0.32, 0.15, 0.06, 1)
        _CharColor ("Char Color (sort, multiplikator)", Color) = (0.03, 0.025, 0.02, 1)
        _CharStrength ("Char Strength", Range(0,1)) = 1
        _NoiseScale ("Noise Scale", Float) = 8
        _Softness ("Softness", Range(0.05,1)) = 0.3
        _SootSpecks ("Soot Specks", Range(0,1)) = 0.5

        [Header(Stage Timing)]
        _BrownStart ("Brown starts at Burn Amount", Range(0,1)) = 0.2
        _CharStart ("Char starts at Burn Amount", Range(0,1)) = 0.6

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
                half4  _BrownColor;
                half   _CharStrength;
                float  _NoiseScale;
                half   _Softness;
                half   _SootSpecks;
                half   _BrownStart;
                half   _CharStart;
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

            float2 Hash22(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            // Glat gradient-støj (Perlin-agtig) med quintic interpolation -> ingen firkantede klatter
            float GradNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

                float2 g00 = Hash22(i) * 2.0 - 1.0;
                float2 g10 = Hash22(i + float2(1, 0)) * 2.0 - 1.0;
                float2 g01 = Hash22(i + float2(0, 1)) * 2.0 - 1.0;
                float2 g11 = Hash22(i + float2(1, 1)) * 2.0 - 1.0;

                float a = dot(g00, f);
                float b = dot(g10, f - float2(1, 0));
                float c = dot(g01, f - float2(0, 1));
                float d = dot(g11, f - float2(1, 1));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y) * 0.7 + 0.5;
            }

            // Flere lag støj, drejet mellem lagene så grid-retningen skjules
            float FBM(float2 p, int octaves)
            {
                const float2x2 rot = float2x2(0.8, -0.6, 0.6, 0.8);
                float sum = 0.0;
                float amp = 0.5;
                float norm = 0.0;
                for (int k = 0; k < octaves; k++)
                {
                    sum += GradNoise(p) * amp;
                    norm += amp;
                    p = mul(rot, p) * 2.03 + 11.7;
                    amp *= 0.5;
                }
                return sum / norm;
            }

            // 1 = brændt, 0 = urørt, for en given fremdrift (0..1) af et trin
            float StageMask(float noise, float amount)
            {
                float b = lerp(-_Softness, 1.0 + _Softness, saturate(amount));
                return 1.0 - smoothstep(b, b + _Softness, noise);
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
                float noise = FBM(IN.uv * _NoiseScale, 4);

                // Støjen klumper sig omkring 0.5 -> stræk den ud til hele 0..1,
                // så Burn Amount opleves lineært
                noise = smoothstep(0.28, 0.72, noise);

                // Retning (lav støj brænder først, så bunden får lavere værdier)
                float h = saturate((IN.heightOS - _HeightMin) / max(_HeightMax - _HeightMin, 0.0001));
                noise = (noise + h * _DirectionStrength) / (1.0 + _DirectionStrength);

                // Kanter brænder først
                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(n, v)), 3.0);
                noise = saturate(noise - fresnel * _FresnelStrength * 0.3);

                // Tre trin med hvert sit tidsvindue
                float scorch = StageMask(noise, _BurnAmount);
                float brown  = StageMask(noise, (_BurnAmount - _BrownStart) / max(1.0 - _BrownStart, 0.0001));
                float charM  = StageMask(noise, (_BurnAmount - _CharStart)  / max(1.0 - _CharStart,  0.0001));

                // Multiplikator oven på originalen (1 = ingen ændring)
                float3 mul = float3(1, 1, 1);
                mul = lerp(mul, _ScorchColor.rgb, scorch);
                mul = lerp(mul, _BrownColor.rgb,  brown);

                // Sodpletter på den brune del, før kullet lægges på
                // Bløde pletter (lavere frekvens + glat støj) i stedet for pixelkorn
                float speck = FBM(IN.uv * _NoiseScale * 3.0 + 40.0, 3);
                speck = smoothstep(0.35, 0.65, speck);
                mul *= lerp(1.0, 0.65 + 0.35 * speck, brown * _SootSpecks);

                mul = lerp(mul, _CharColor.rgb, charM * _CharStrength);

                return half4(mul, 1);
            }
            ENDHLSL
        }
    }
}
