Shader "Custom/LineGenerator"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Name "LinePass"
            Offset -1, -1
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
            };
            
            struct v2g
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
            };
            
            struct g2f
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
            };
            
            half4 _Color;
            
            v2g vert(Attributes input)
            {
                v2g output = (v2g)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color * _Color;
                return output;
            }
            
            [maxvertexcount(4)]
            void geom(triangle v2g input[3], inout LineStream<g2f> lineStream)
            {
                g2f output = (g2f)0;
                
                output.positionCS = input[0].positionCS;
                output.color = input[0].color;
                lineStream.Append(output);
                
                output.positionCS = input[1].positionCS;
                output.color = input[1].color;
                lineStream.Append(output);
                
                lineStream.RestartStrip();

                output.positionCS = input[1].positionCS;
                output.color = input[1].color;
                lineStream.Append(output);
                
                output.positionCS = input[2].positionCS;
                output.color = input[2].color;
                lineStream.Append(output);
                
                lineStream.RestartStrip();

                // 对于绕徐有更严苛的要求：最后一个参绕的line必须失效

                // output.positionCS = input[0].positionCS;
                // output.color = input[0].color;
                // lineStream.Append(output);
                //
                // output.positionCS = input[2].positionCS;
                // output.color = input[2].color;
                // lineStream.Append(output);
                //
                // lineStream.RestartStrip();
            }
            
            half4 frag(g2f input) : SV_Target
            {
                return input.color;
            }
            
            ENDHLSL
        }
    }
}