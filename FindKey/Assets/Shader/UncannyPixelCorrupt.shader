Shader "Custom/UncannyPixelCorrupt"
{
    Properties
    {
        _MainTex ("Current Image", 2D) = "white" {}
        _NextTex ("Next Image", 2D) = "white" {}
        _Transition ("Transition", Range(0,1)) = 0

        _Resolution ("Pixel Resolution", Vector) = (153,204,0,0)
        _BlockSize ("Block Size", Float) = 12

        _ShakeStrength ("Shake Strength", Float) = 0.002
        _ScrambleStrength ("Scramble Strength", Float) = 0.15
        _CorruptLines ("Line Corruption", Float) = 0.02
        _RGBShift ("RGB Shift", Float) = 0.002
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NextTex;

            float _Transition;
            float4 _Resolution;
            float _BlockSize;

            float _ShakeStrength;
            float _ScrambleStrength;
            float _CorruptLines;
            float _RGBShift;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // --- PIXEL PERFECT ---
                float2 pixelUV = floor(uv * _Resolution.xy) / _Resolution.xy;

                // --- BLOQUES ---
                float2 blockUV = floor(pixelUV * _BlockSize) / _BlockSize;
                float2 blockCenter = blockUV + (0.5 / _BlockSize);

                float noise = hash(blockUV);

                // --- VIBRACIÓN PREVIA ---
                float preShake = (1 - _Transition);
                pixelUV += sin(_Time.y * 300) * _ShakeStrength * preShake;

                // --- COLAPSO ---
                float collapse = smoothstep(0.0, 0.35, _Transition);
                pixelUV = lerp(pixelUV, blockCenter, collapse);

                // --- SCRAMBLE REAL (intercambio de bloques) ---
                float scramblePhase = smoothstep(0.2, 0.6, _Transition);

                float2 scrambleOffset = float2(
                    hash(blockUV + 5.3),
                    hash(blockUV + 9.1)
                );

                scrambleOffset = (scrambleOffset * 2 - 1) * _ScrambleStrength;

                pixelUV += scrambleOffset * scramblePhase;

                // --- LÍNEAS CORRUPTAS ---
                float lineNoise = hash(float2(0, floor(pixelUV.y * _Resolution.y)));
                float lineShift = step(0.85, lineNoise) * _CorruptLines * (1 - _Transition);
                pixelUV.x += lineShift;

                // --- CAMBIO NO SIMULTÁNEO ---
                float localDelay = hash(blockUV);
                float swap = step(localDelay, _Transition);

                fixed4 colA = tex2D(_MainTex, pixelUV);
                fixed4 colB = tex2D(_NextTex, pixelUV);

                fixed4 col = lerp(colA, colB, swap);

                // --- FLASH GLITCH ---
                if (_Transition > 0.45 && _Transition < 0.5)
                {
                    col.rgb = 1 - col.rgb;
                }

                // --- RGB SHIFT ---
                float2 rgbOffset = float2(_RGBShift * (1 - _Transition), 0);

                float r = tex2D(_MainTex, pixelUV + rgbOffset).r;
                float g = col.g;
                float b = tex2D(_MainTex, pixelUV - rgbOffset).b;

                col.rgb = lerp(col.rgb, float3(r,g,b), scramblePhase);

                return col;
            }
            ENDCG
        }
    }
}
