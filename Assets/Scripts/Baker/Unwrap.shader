//Adapted from a blog from Snelha Belkhale, 
//https://snayss.medium.com/bake-a-pretty-or-computationally-challenging-shader-into-a-texture-unity-b524569d7384

Shader "TextureBake/Unwrap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
            //Setting shader function names
            #pragma vertex Vertex
            #pragma fragment FragmentShader
            //Make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            //Varaible set by user
            sampler2D _MainTex;

            //IO structs
            struct vertIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct vertOut
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            //Vertex shader
            vertOut Vertex (vertIn input)
            {
                vertOut output;
                //Set vertex pos to the uv pos
                input.vertex = float4(input.uv.xy, 0.0, 1.0);
                //Multiply with the projectionmatrix
                output.vertex = mul(UNITY_MATRIX_P, input.vertex);
                //Carry over vertex color
                output.color = input.color;
                //Carry over uv
                output.uv = input.uv;
                return output;
            }
            //Fragment shader
            float4 FragmentShader (vertOut input) : Color
            {
                return input.color;
            }
            ENDCG
        }
    }
}
