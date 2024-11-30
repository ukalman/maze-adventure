Shader "Custom/DamageEffect"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Fallback texture
        _DamageColor ("Damage Color", Color) = (1, 0, 0, 1) // Red tint
        _Intensity ("Intensity", Range(0, 1)) = 0 // Blend intensity
        _VignetteRadius ("Vignette Radius", Range(0, 1)) = 0.5 // Vignette size
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Blend mode
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _DamageColor;
            float _Intensity;
            float _VignetteRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture; fallback to a default color if missing
                fixed4 col = tex2D(_MainTex, i.uv);
                col = lerp(fixed4(1, 1, 1, 1), col, _Intensity); // Use white as fallback if _MainTex isn't bound

                // Calculate vignette effect
                float2 uv = i.uv - 0.5; // Center UV coordinates
                float dist = length(uv) / _VignetteRadius; // Distance from center
                float vignette = saturate(1.0 - dist * dist);

                // Combine damage color and vignette intensity
                fixed4 damageEffect = _DamageColor * vignette * _Intensity;

                // Blend with the original color
                return lerp(col, damageEffect + col, _Intensity);
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
