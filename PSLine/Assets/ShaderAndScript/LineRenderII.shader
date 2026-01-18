Shader "Geo/QuadWireframe"
{
    Properties
    {
        _Scale ("Scale", Range(-1, 1)) = 0
        _WeightPow ("WeightPow", Range(0, 2)) = 1
        _DotAngle("DotAngle", Range(0, 10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off
        
        Pass
        {
            // AlphaToMask On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0 // 几何着色器需要支持 Shader Model 4.0
            // #pragma target 5.0 // 几何着色器需要支持 Shader Model 5.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 posWS : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 color : COLOR0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                o.normal = v.normal;
                o.color = v.color;
                return o;
            }

            float _Scale;
            float _WeightPow;
            float _DotAngle;

            [maxvertexcount(3)]
            void geom(triangle v2f input[3], inout LineStream<v2f> stream)
            {
                stream.Append(input[1]);
                stream.Append(input[2]);
                stream.RestartStrip();
            }
            
            // [maxvertexcount(12)]
            // void geom(triangleadj v2f i[6], inout LineStream<v2f> stream)
            // {
            //     // 三角形的三个顶点
            //     v2f v1 = i[0];
            //     v2f v2 = i[1];
            //     v2f v3 = i[2];
            //     
            //     v2f a1 = i[3];
            //     v2f a2 = i[4];
            //     v2f a3 = i[5];
            //     
            //     a1.pos = TransformObjectToHClip(v1.color);
            //     a2.pos = TransformObjectToHClip(v2.color);
            //     a3.pos = TransformObjectToHClip(v3.color);
            //     
            //     a1.posWS = TransformObjectToWorld(v1.color);
            //     a2.posWS = TransformObjectToWorld(v2.color);
            //     a3.posWS = TransformObjectToWorld(v3.color);
            //     
            //     float3 normalWS = normalize(cross(v2.posWS - v1.posWS, v3.posWS - v1.posWS));
            //     float3 viewDirWS = normalize((v1.posWS + v2.posWS + v3.posWS) / 3 - _WorldSpaceCameraPos);
            //
            //     if (dot(normalWS, viewDirWS) < 0) // 首先剔除背面
            //     {
            //         float2 n1 = TransformWorldToViewNormal(TransformObjectToWorldNormal(v1.normal)).xy;
            //         float2 n2 = TransformWorldToViewNormal(TransformObjectToWorldNormal(v2.normal)).xy;
            //         float2 n3 = TransformWorldToViewNormal(TransformObjectToWorldNormal(v3.normal)).xy;
            //
            //         float2 da = float2(cos(_DotAngle), sin(_DotAngle));
            //         float value1 = abs(dot(n1, da));
            //         float value2 = abs(dot(n2, da));
            //         float value3 = abs(dot(n3, da));
            //
            //         value1 = pow(value1, _WeightPow);
            //         value2 = pow(value2, _WeightPow);
            //         value3 = pow(value3, _WeightPow);
            //         
            //         v1.color = float4(value1, value1, value1, 1);
            //         v2.color = float4(value2, value2, value2, 1);
            //         v3.color = float4(value3, value3, value3, 1);
            //         // v1.color = float4(1, 1, 1, 1);
            //         // v2.color = float4(1, 1, 1, 1);
            //         // v3.color = float4(1, 1, 1, 1);
            //         
            //         // 计算边的两个邻接面，判定是否同向
            //         if (dot(normalize(cross(v2.posWS - v1.posWS, a1.posWS - v1.posWS)), viewDirWS) < _Scale)
            //         {
            //             stream.Append(v1); stream.Append(v2); stream.RestartStrip();
            //         }
            //         if (dot(normalize(cross(v3.posWS - v2.posWS, a2.posWS - v2.posWS)), viewDirWS) < _Scale)
            //         {
            //             stream.Append(v3); stream.Append(v2); stream.RestartStrip();
            //         }
            //         if (dot(normalize(cross(v1.posWS - v3.posWS, a3.posWS - v3.posWS)), viewDirWS) < _Scale)
            //         {
            //             stream.Append(v3); stream.Append(v1); stream.RestartStrip();
            //         }
            //     }
            // }

            // 片段着色器
            float4 frag(v2f i) : SV_Target
            {
                return float4(1,1,1,1); // i.color;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            // Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0 // 几何着色器需要支持 Shader Model 4.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 posWS : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 color : COLOR0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                o.normal = v.normal;
                o.color = v.color;
                return o;
            }

            // 片段着色器
            float4 frag(v2f i) : SV_Target
            {
                return float4(0, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}
