Shader "Custom/BaseTST"
{
    Properties
    {
        _Noise("Noise", 2D) = "white" {}
        _InverseNormalStepSize("InverseNormalStepSize", Range(0,10)) = 0.01
        _NumberOfRays("NumberOfRays", Range(0,100)) = 1
        _NumberOfTris("NumberOfTris", Range(0,100000)) = 1000000
    }
    SubShader
    {
        Name "TST"
        Tags { "RenderType" = "Opaque" }

        pass 
        {
            Lighting On
                ColorMaterial Emission

            CGPROGRAM      
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "unityCG.cginc"
            #include "Lighting.cginc"
            //#pragma surface surf Lambert vertex:vert
            
            ///----------------------------- Input -----------------------------
            //Data buffers
            StructuredBuffer<float3> _Verts;
            uniform uint _VertSize;
            StructuredBuffer<uint3> _Tris;
            uniform uint _TrisSize;

            //Noise buffers
            sampler2D _Noise;
            uniform float4 _Noise_TexelSize;

            //Float values
            float _InverseNormalStepSize;

            //Number of rays
            uint _NumberOfRays;
            
            ///----------------------------- Custom structs -----------------------------
            struct Ray
        {
            float3 origin;
            float3 direction;
        };
            
            //----------------------------- Raycasting -----------------------------
            float3 RandomInUnitSphere(float2 uv)
            {
                return normalize(tex2Dlod(_Noise, float4(uv, 0.0, 0.0)).rgb);
            }
            void RayCast(Ray ray, uint3 triangleID, out float4 smallestDist, out float3 normal, out bool hit)
            {
                ///----------------------------- Data -----------------------------
                //Polygon corners
                float3 vertA = _Verts[triangleID.x];
                float3 vertB = _Verts[triangleID.y];
                float3 vertC = _Verts[triangleID.z];
                
                ///----------------------------- Intersection Calc -----------------------------
                //Poly normal
                normal = normalize(cross(vertB - vertA, vertC - vertA));

                //Float distance, hitposition
                float dist = dot(vertA - ray.origin, normal) / dot(ray.direction, normal);
                float3 hitPos = ray.origin + dist * ray.direction;

                dist = (dist < 0.0f) ? smallestDist.w : dist;
                dist = (smallestDist.w < dist) ? smallestDist.w : dist;

                //Check if in triangle
                float dotAB = dot(normal, cross(vertB - vertA, hitPos - vertA));
                float dotBC = dot(normal, cross(vertC - vertB, hitPos - vertB));
                float dotCA = dot(normal, cross(vertA - vertC, hitPos - vertC));
                
                bool AB = (dotAB >= 0) ? true : false;
                bool BC = (dotBC >= 0) ? true : false;
                bool CA = (dotCA >= 0) ? true : false;
                
                ///----------------------------- Result -----------------------------
                if ((AB && BC && CA || !AB && !BC && !CA) && !(length(normal) <= 0.001f))
                {
                    hit = true;
                    smallestDist = float4(hitPos, dist);
                }
                else
                {
                    hit = false;
                    smallestDist = smallestDist;
                }
            }
            
            //----------------------------- Vertex -----------------------------
            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 color : COLOR;
                float2 texcoord0 : TEXCOORD0;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };
            v2f vert(in appdata IN)
            {
                float3 lighting;
                float4 smallestDist = float4(0, 0, 0, 100000.0f);
                float3 normal;
                bool hitTest = false;
                for (uint idx= 0; idx < _NumberOfRays; idx++)
                {
                    ///----------------------------- Ray -----------------------------
                    //Our Ray
                    Ray ray;
                    ray.origin = IN.vertex.rgb - IN.normal * _InverseNormalStepSize;
                    ray.direction = RandomInUnitSphere(IN.texcoord0 * idx * 2.125348f);
            
                    ///----------------------------- Casting loop -----------------------------
                    //Go over all triangles/indecies
                    for (uint triangleNr = 0; (triangleNr < _TrisSize); triangleNr++)
                    {
                        hitTest = false;
                        RayCast(ray, _Tris[triangleNr], smallestDist, normal, hitTest);
                        if (hitTest) 
                        {
                            float3 lp = float3(unity_4LightPosX0[0], unity_4LightPosY0[0], unity_4LightPosZ0[0]);

                            float3 l = smallestDist.rgb - lp;
                            float3 v = smallestDist.rgb - _WorldSpaceCameraPos;
                            float3 h = (l + normal) / normalize(l + normal);
                            
                            float I = pow(saturate(dot(v, -h)), 2);

                            for (uint lightIdx = 0; lightIdx < 4; lightIdx++) 
                            {
                                lighting += unity_LightColor[lightIdx].rgb * I;
                            }
                        }
                    }
                }
            
                ///----------------------------- Result -----------------------------
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.color = float4(lighting, 1.0f);
                return OUT;
            }
            
            //----------------------------- Surface -----------------------------
            fixed4 frag(v2f i) : SV_Target
            { 
                return i.color; 
            }
            ENDCG
        }
    }
}
