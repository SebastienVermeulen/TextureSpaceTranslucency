Shader "Custom/TST"
{
    Properties
    {
        _MainTex("Maintex", 2D) = "white" {}
        _Detph("Detph", 2D) = "white" {}
        _Normal("Normal", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Glossiness ("Glossiness", Range(0, 1)) = 0
        _MolarMass ("(Beer-Lambert) Molar Mass", Range(0, 10)) = 1.3
        _MolarAbsorbtivity ("(Beer-Lambert) Molar Absorbtivity", Range(0, 10)) = 0.35
        _GlowPower ("GlowPower", Range(0, 5)) = 1.0
        _GlowScale ("GlowScale", Range(0, 5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf StandardTranslucent fullforwardshadows
        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        ///----------------------------- Input Data -----------------------------
        sampler2D _MainTex;
        sampler2D _Detph;
        sampler2D _Normal;
        float4 _Color;
        float _Metallic;
        float _Glossiness;
        float _GlowPower;
        float _GlowScale;
        //Beer law
        float _MolarMass;
        float _MolarAbsorbtivity;

        ///----------------------------- Local globals -----------------------------
        float3 normal;
        float detph;

        ///----------------------------- Custom lighting model -----------------------------
        inline float4 LightingStandardTranslucent(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
        {
            // Original color
            float4 pbr = LightingStandard(s, viewDir, gi);

            //Beer-Lambert
            float lightFraction = pow(10.0f, - _MolarAbsorbtivity * detph * _MolarMass);

            //Prevent oversaturation
            float3 H = normalize(gi.light.dir + s.Normal);
            float I = pow(saturate(dot(viewDir, -H)), _GlowPower) * _GlowScale;

            // Final add
            pbr.rgb = saturate(pbr.rgb + I * gi.light.color * lightFraction);
            return pbr;
        }
        inline void LightingStandardTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
        {
            gi = UnityGlobalIllumination(data, s.Occlusion, s.Smoothness, s.Normal);
        }

        ///----------------------------- Surface shader -----------------------------
        struct Input
        {
            float2 uv_MainTex;
        };
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            detph = tex2Dlod(_Detph, fixed4(IN.uv_MainTex, 0, 0)).r;
            normal = tex2Dlod(_Normal, fixed4(IN.uv_MainTex, 0, 0)).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
