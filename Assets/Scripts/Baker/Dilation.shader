//Adapted from a blog from Snelha Belkhale, 
//https://snayss.medium.com/bake-a-pretty-or-computationally-challenging-shader-into-a-texture-unity-b524569d7384

Shader "TextureBake/Dilation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("BackgroundColor", Color) = (0,0,0,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            //Setting shader function names
            #pragma vertex Vertex
            #pragma fragment FragmentShader
            
            #include "UnityCG.cginc"

            //Varaible set by user
            sampler2D _MainTex;
            float4 _BackgroundColor;
            //Variables set by unity
            //Through the strongest black magic source code
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            //IO structs
            struct vertexData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct fragData
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            //Dilation helper function
            float4 Dilate (float2 uv) 
            {
                float4 closestColor = _BackgroundColor;
                float maxBright = 0;

                float4 neighborCol;
                float brightness;

                //Check all pixels in a 7 by 7 grid around uv
                for (float i = -3; i < 4; i++) 
                {
                    for(float j = -3; j < 4; j++) 
                    {
                        //Sample the pixel
                        neighborCol = clamp(tex2D(_MainTex, uv + float2(i, j) * _MainTex_TexelSize), 0, 1);
                        //Calc brightness
                        brightness = distance(neighborCol.rgb, _BackgroundColor.rgb);
                        //Collect brightest value
                        if(brightness > maxBright)
                        {
                            closestColor = neighborCol;
                            maxBright = brightness;
                        }
                    }
                }
                return closestColor; 
            }

            //Vertex shader
            fragData Vertex (vertexData i)
            {
                fragData o;
                //Transform from object- into camera clip-space
                o.vertex = UnityObjectToClipPos(i.vertex);
                //Scale and offset so it will fit on the given texture
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                return o;
            }
            //Fragment shader
            fixed4 FragmentShader (fragData i) : SV_Target
            {
                //Get color from the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //Is the diff small enough
                if(distance(col.rgb, _BackgroundColor.rgb) < 0.001) 
                {
                    col = Dilate(i.uv);
                } 
                return col;
            }
            ENDCG
        }
    }
}
